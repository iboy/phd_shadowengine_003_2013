#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Uni2DEditorSmoothBindingUtils
{
	///// Enums /////
	public enum BoneEditMode
	{
		None,
		Posing,
		Anim
	}

	// Describes the various bone handles
	public enum BoneHandle
	{
		None,
		OuterDisc,
		InnerDisc,
		Link
	};
	
	// The bone editor tools
	public enum BoneEditorTool
	{
		Select,
		Move,
		Create
	};

	// Pick search radius
	public const float bonePosingPickRadius     = 20.0f;
	public const float boneAnimPickRadius       = 5.0f;
	public const float boneLinkAnimPickRadius   = 5.0f;
	public const float boneLinkPosingPickRadius = 5.0f;

	public static float BoneInfluenceDistanceFunc( float a_fDistance, float a_fAlpha )
	{
		return 1.0f / Mathf.Pow( a_fDistance, a_fAlpha );
	}

	// Adds a bone to a sprite from a given mouse GUI position
	public static Uni2DSmoothBindingBone AddBoneToSprite( Uni2DSprite a_rSprite, Vector2 a_f2MouseGUIPos, Uni2DSmoothBindingBone a_rBoneRoot )
	{
		Transform rSpriteTransform = a_rSprite.transform;
		Ray oWorldRay = HandleUtility.GUIPointToWorldRay( a_f2MouseGUIPos );
		Plane oSpritePlane = new Plane( rSpriteTransform.forward, rSpriteTransform.position );	// INFO: normal specific to Uni2D's planes

		float fDistance;
		if( oSpritePlane.Raycast( oWorldRay, out fDistance ) )
		{
			Uni2DSmoothBindingBone oBone;

			if( a_rBoneRoot == null )
			{
				oBone = Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rSprite );
			}
			else
			{
				switch( a_rBoneRoot.ChildCount )
				{
					case 0:
					{
						// Add child
						oBone = Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rBoneRoot );
					}
					break;

					case 1:
					{
						// Create branch for existing child + add another one
						Uni2DSmoothBindingBone oFakeParent1 = Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rBoneRoot, true );
						Uni2DSmoothBindingBone oFakeParent2 = Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rBoneRoot, true );

						a_rBoneRoot.Children[ 0 ].Parent = oFakeParent1;
						oBone = Uni2DEditorSmoothBindingUtils.CreateNewBone( oFakeParent2 );
					}
					break;

					default:
					{
						// Add branch
						Uni2DSmoothBindingBone oFakeParent = Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rBoneRoot, true );
						oBone = Uni2DEditorSmoothBindingUtils.CreateNewBone( oFakeParent );
					}
					break;
				}
			}

			oBone.transform.position = oWorldRay.GetPoint( fDistance );

			return oBone;
		}

		return null;
	}

	public static void DeleteBone( Uni2DSprite a_rSprite, Uni2DSmoothBindingBone a_rBoneToDelete )
	{
		if( a_rBoneToDelete != null )
		{
			Uni2DSmoothBindingBone rBoneParent = a_rBoneToDelete.Parent;
	
			// Need to pack bone hierarchy
			if( rBoneParent != null && rBoneParent.IsFakeRootBone )
			{
				Uni2DSmoothBindingBone rBoneGrandParent = rBoneParent.Parent;
	
				Object.DestroyImmediate( rBoneParent.gameObject );
	
				// We broke the branch => flatten bone hierarchy
				if( rBoneGrandParent != null && rBoneGrandParent.IsBranchBone == false )
				{
					Uni2DSmoothBindingBone rFakeParent = rBoneGrandParent.Children[ 0 ];
					rFakeParent.Children[ 0 ].Parent = rBoneGrandParent;
	
					Object.DestroyImmediate( rFakeParent.gameObject );
				}
			}
			else
			{
				Object.DestroyImmediate( a_rBoneToDelete.gameObject );
			}
		}
	}

	///// Bone creation /////
	private static Uni2DSmoothBindingBone CreateNewBone( Transform a_rTransform, bool a_bCreateFakeBone = false )
	{
		GameObject oBoneGameObject   = new GameObject( );
		Transform oBoneTransform     = oBoneGameObject.transform;
		Uni2DSmoothBindingBone oBone = oBoneGameObject.AddComponent<Uni2DSmoothBindingBone>( );
		
		oBoneGameObject.name = ( a_bCreateFakeBone ? "Uni2DFakeBone_" : "Uni2DBone_" ) + Mathf.Abs( oBoneGameObject.GetInstanceID( ) );
		
		oBoneTransform.parent = a_rTransform;
		oBoneTransform.localPosition = Vector3.zero;
		oBoneTransform.localRotation = Quaternion.identity;
		oBoneTransform.localScale = Vector3.one;
		
		return oBone;		
	}

	public static Uni2DSmoothBindingBone CreateNewBone( Uni2DSmoothBindingBone a_rBoneParent, bool a_bCreateFakeBone = false )
	{
		return Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rBoneParent.transform, a_bCreateFakeBone );
	}

	public static Uni2DSmoothBindingBone CreateNewBone( Uni2DSprite a_rSprite )
	{
		return Uni2DEditorSmoothBindingUtils.CreateNewBone( a_rSprite.transform, false );
	}
	
	///// Picking /////
	private static Uni2DSmoothBindingBone PickNearestBone( Uni2DSprite a_rSprite,
		Vector2 a_f2MouseGUIPos,
		out BoneHandle a_ePickedHandle,
		Uni2DSmoothBindingBone a_rExclude = null,
		bool a_bAlsoPickBoneLink    = false,
		float a_fBonePickRadius     = Uni2DEditorSmoothBindingUtils.bonePosingPickRadius,
		float a_fBoneLinkPickRadius = Uni2DEditorSmoothBindingUtils.boneLinkAnimPickRadius )
	{
		Uni2DSmoothBindingBone rNearestBone     = null;
		Uni2DSmoothBindingBone rNearestBoneLink = null;
		
		// Squarred pick radius
		float fMinSqrBoneDistance  = a_fBonePickRadius * a_fBonePickRadius;
		float fMinBoneLinkDistance = a_fBoneLinkPickRadius;
		
		Uni2DSmoothBindingBone[ ] rBones = a_rSprite.Bones; //rSpriteTransform.GetComponentsInChildren<Transform>( false ).Except( oBonesToExclude ).ToArray( );
		
		// Look for nearest bone
		for( int iBoneIndex = 0, iBoneCount = rBones.Length; iBoneIndex < iBoneCount; ++iBoneIndex )
		{
			Uni2DSmoothBindingBone rBone = rBones[ iBoneIndex ];
			
			if( a_rExclude != rBone && rBone.IsFakeRootBone == false )
			{
				Vector2 f2BoneGUIPos = HandleUtility.WorldToGUIPoint( rBone.transform.position );
				Vector2 f2BoneToMouseGUI = a_f2MouseGUIPos - f2BoneGUIPos;
				
				float fSqrDistance = f2BoneToMouseGUI.sqrMagnitude;
				
				// New min/nearest
				if( fSqrDistance < fMinSqrBoneDistance )
				{
					rNearestBone = rBone;
					fMinSqrBoneDistance = fSqrDistance;
				}
				
				// Look for nearest bone link
				Uni2DSmoothBindingBone rBoneParent = rBone.Parent;
				if( a_bAlsoPickBoneLink && rBoneParent != null )
				{
					float fLinkDistance = HandleUtility.DistancePointToLineSegment( a_f2MouseGUIPos,
						f2BoneGUIPos,
						HandleUtility.WorldToGUIPoint( rBoneParent.transform.position ) );
					
					if( fLinkDistance < fMinBoneLinkDistance )
					{
						fMinBoneLinkDistance = fLinkDistance;
						rNearestBoneLink = rBone;
					}
				}
			}
		}
		
		// Picking result
		if( rNearestBone == null && rNearestBoneLink == null )
		{
			a_ePickedHandle = BoneHandle.None;
		}
		else if( rNearestBone != null )
		{
			if( fMinSqrBoneDistance <= a_fBonePickRadius * a_fBonePickRadius * 0.25f )
			{
				a_ePickedHandle = /*invertActionAreas == false ?*/ BoneHandle.InnerDisc; //: BoneHandle.OuterDisc;
			}
			else
			{
				a_ePickedHandle = /*invertActionAreas == false ?*/ BoneHandle.OuterDisc; //: BoneHandle.InnerDisc;
			}
		}
		else
		{
			rNearestBone    = rNearestBoneLink;
			a_ePickedHandle = BoneHandle.Link; 
		}
		
		return rNearestBone;
	}

	// Specialized picking call used in anim mode
	public static Uni2DSmoothBindingBone PickNearestBoneInAnimMode( Uni2DSprite a_rSprite,
		Vector2 a_f2MouseGUIPos,
		out BoneHandle a_ePickedHandle,
		Uni2DSmoothBindingBone a_rExclude = null )
	{
		return Uni2DEditorSmoothBindingUtils.PickNearestBone( a_rSprite,
			a_f2MouseGUIPos,
			out a_ePickedHandle,
			a_rExclude,
			true,
		 	Uni2DEditorSmoothBindingUtils.boneAnimPickRadius,
			Uni2DEditorSmoothBindingUtils.boneLinkAnimPickRadius );
	}

	// Specialized picking call used in posing mode
	public static Uni2DSmoothBindingBone PickNearestBoneInPosingMode( Uni2DSprite a_rSprite,
		Vector2 a_f2MouseGUIPos,
		out BoneHandle a_ePickedHandle,
		Uni2DSmoothBindingBone a_rExclude = null )
	{
		return Uni2DEditorSmoothBindingUtils.PickNearestBone( a_rSprite,
			a_f2MouseGUIPos,
			out a_ePickedHandle,
			a_rExclude,
			true,
			Uni2DEditorSmoothBindingUtils.bonePosingPickRadius,
			Uni2DEditorSmoothBindingUtils.boneLinkPosingPickRadius );
	}

	// Specialized picking call used in posing mode and when drawing disc handles
	public static Uni2DSmoothBindingBone PickNearestBoneArticulationInPosingMode( Uni2DSprite a_rSprite,
		Vector2 a_f2MouseGUIPos,
		out BoneHandle a_ePickedHandle,
		Uni2DSmoothBindingBone a_rExclude = null )
	{
		return Uni2DEditorSmoothBindingUtils.PickNearestBone( a_rSprite,
			a_f2MouseGUIPos,
			out a_ePickedHandle,
			a_rExclude,
			false,
			Uni2DEditorSmoothBindingUtils.bonePosingPickRadius,
			Uni2DEditorSmoothBindingUtils.boneLinkPosingPickRadius );
	}


	/*
	public static void DumpHeatMap( string a_rFilename )
	{
		const int c_iTextureWidth = 1024;
		const int c_iTextureHeight = 1024;

		// WARNING: hardcoded min/max values. Edit them for each texture you want to dump
		const float c_fMinWidth = 0.0f;
		const float c_fMaxWidth = 512.0f;
		const float c_fMinHeight = 0.0f;
		const float c_fMaxHeight = 512.0f;

		const float c_fWidthRange = c_fMaxWidth - c_fMinWidth;
		const float c_fHeigthRange = c_fMaxHeight - c_fMinHeight;

		Dictionary<Transform, Color[ ]> oHeatMapsDict = new Dictionary<Transform, Color[ ]>( );

		for( int iY = 0; iY < c_iTextureHeight; ++iY )
		{
			for( int iX = 0; iX < c_iTextureWidth; ++iX )
			{
				Vector3 f3VertexCoords = new Vector3( c_fMinWidth + ( ( iX * c_fWidthRange ) / (float) c_iTextureWidth ),
					c_fMinHeight + ( ( iY * c_fHeigthRange ) / (float) c_iTextureHeight ),
					0.0f );

				Dictionary<Transform,float> rBoneInfluenceDict = this.GetBonesInfluences( f3VertexCoords );

				float fInvDistanceSum = 1.0f / rBoneInfluenceDict.Sum( x => x.Value );

				foreach( KeyValuePair<Transform, float> rBoneInfluencePair in rBoneInfluenceDict )
				{
					Color[ ] oHeatMapPixels32;

					if( oHeatMapsDict.TryGetValue( rBoneInfluencePair.Key, out oHeatMapPixels32 ) == false )
					{
						oHeatMapPixels32 = new Color[ c_iTextureWidth * c_iTextureHeight ];
					}

					float fBoneInfluence = rBoneInfluencePair.Value * fInvDistanceSum;
					oHeatMapPixels32[ iX + iY * c_iTextureWidth ] = Color.white * fBoneInfluence;
					oHeatMapsDict[ rBoneInfluencePair.Key ] = oHeatMapPixels32;
				}		
			}
		}

		foreach( KeyValuePair<Transform, Color[ ]> rBoneHeatMapPair in oHeatMapsDict )
		{
			Texture2D oHeatMap = new Texture2D( c_iTextureWidth, c_iTextureHeight, TextureFormat.RGB24, false );
			oHeatMap.SetPixels( rBoneHeatMapPair.Value );
			oHeatMap.Apply( );

			FileStream oFileStream = new FileStream( Application.dataPath + "/" + a_rFilename + "_" + rBoneHeatMapPair.Key.name + rBoneHeatMapPair.Key.GetInstanceID( ).ToString( ) + ".png", FileMode.OpenOrCreate );
			BinaryWriter oBinaryWriter = new BinaryWriter( oFileStream );
			oBinaryWriter.Write( oHeatMap.EncodeToPNG( ) );
			oBinaryWriter.Close( );
			oFileStream.Close( );
			
			AssetDatabase.ImportAsset( "Assets/" + a_rFilename + "_" + rBoneHeatMapPair.Key.name + rBoneHeatMapPair.Key.GetInstanceID( ).ToString( ) + ".png" );
		}
	}
	*/
}

#endif
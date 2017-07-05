#if UNITY_EDITOR

// Warning : Uni2D only supported on Unity 3.5.7 and higher
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
	#define BEFORE_UNITY_4_3
#else
	#define AFTER_UNITY_4_3
#endif

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Uni2DSmoothBindingBone : MonoBehaviour
{
	public int ChildCount
	{
		get
		{
			int iChildCount = 0;
			foreach( Transform rChildTransform in this.transform )
			{
				if( rChildTransform.GetComponent<Uni2DSmoothBindingBone>( ) != null )
				{
					++iChildCount;
				}
			}

			return iChildCount;
		}
	}

	public Uni2DSmoothBindingBone[ ] Children
	{
		get
		{
			List<Uni2DSmoothBindingBone> oBones = new List<Uni2DSmoothBindingBone>( );
			foreach( Transform rChildTransform in this.transform )
			{
				Uni2DSmoothBindingBone rBone = rChildTransform.GetComponent<Uni2DSmoothBindingBone>( );
				if( rBone != null )
				{
					oBones.Add( rBone );
				}
			}

			return oBones.ToArray( );
		}
	}

	public bool IsFakeRootBone
	{
		get
		{
			Uni2DSmoothBindingBone rBoneParent = this.Parent;
			return this.ChildCount == 1 && rBoneParent != null && rBoneParent.IsBranchBone;
		}
	}
	
	public bool IsBranchBone
	{
		get
		{
			return this.ChildCount > 1;
		}
	}

	public bool HasInfluence
	{
		get
		{
			int iChildCount = this.ChildCount;
			return iChildCount == 1 || ( iChildCount == 0 && this.Parent == null );
		}
	}

	public Uni2DSmoothBindingBone Parent
	{
		get
		{
			Uni2DSmoothBindingBone rParent = null;
			for( Transform rTransform = this.transform.parent; rParent == null && rTransform != null; rTransform = rTransform.parent )
			{
				// No parent found so far but found a sprite
				// => root bone of this sprite so no parent
				if( rTransform.GetComponent<Uni2DSprite>( ) != null )
				{
					return null;
				}

				rParent = rTransform.GetComponent<Uni2DSmoothBindingBone>( );
			}

			return rParent;
		}

		set
		{
			Transform rBoneTransform = this.transform;

			if( value != null )
			{
				rBoneTransform.parent = value.transform;
			}
			else
			{
				Uni2DSprite rSprite = this.Sprite;
				if( rSprite != null )
				{
					rBoneTransform.parent = rSprite.transform;
				}
			}
		}
	}

	public Uni2DSprite Sprite
	{
		get
		{
			Uni2DSprite rSprite = null;
			for( Transform rTransform = this.transform.parent; rSprite == null && rTransform != null; rTransform = rTransform.parent )
			{
				rSprite = rTransform.GetComponent<Uni2DSprite>( );
			}

			return rSprite;
		}
	}


	public void MoveBoneAlongSpritePlane( Vector2 a_f2MouseGUIPos, bool a_bMoveChildren = true )
	{
		Uni2DSprite rSprite = this.Sprite;
		if( rSprite != null )
		{
			Transform rSpriteTransform = rSprite.transform;
			Ray oWorldRay      = HandleUtility.GUIPointToWorldRay( a_f2MouseGUIPos );
			Plane oSpritePlane = new Plane( rSpriteTransform.forward, rSpriteTransform.position );
	
			float fDistance;
			if( oSpritePlane.Raycast( oWorldRay, out fDistance ) )
			{
				Vector3 f3Delta = oWorldRay.GetPoint( fDistance ) - this.transform.position;
				this.transform.position += f3Delta;
				EditorUtility.SetDirty( this );
	
				if( a_bMoveChildren == false )
				{
					foreach( Uni2DSmoothBindingBone rBoneChild in this.Children )
					{
						if( rBoneChild.IsFakeRootBone == false )
						{
							rBoneChild.transform.position -= f3Delta;
						}
						else
						{
							foreach( Uni2DSmoothBindingBone rFakeBoneChild in rBoneChild.Children )
							{
								rFakeBoneChild.transform.position -= f3Delta;
								EditorUtility.SetDirty( rFakeBoneChild.transform );
							}
						}

						EditorUtility.SetDirty( rBoneChild.transform );
					}
				}
			}
		}
	}

	public void RotateBoneAlongSpritePlane( Vector2 a_f2MouseGUIPos )
	{
		Uni2DSprite rSprite = this.Sprite;
		if( rSprite != null )
		{
			Transform rSpriteTransform = rSprite.transform;
			Ray oWorldRay      = HandleUtility.GUIPointToWorldRay( a_f2MouseGUIPos );
			Plane oSpritePlane = new Plane( rSpriteTransform.forward, rSpriteTransform.position );
	
			float fDistance;
			if( oSpritePlane.Raycast( oWorldRay, out fDistance ) )
			{
				Uni2DSmoothBindingBone rBoneToMoveParent = this.Parent;
				if( rBoneToMoveParent != null )
				{
					float fAngle;
					Vector3 f3Axis;

					Transform rBoneToMoveParentTransform = rBoneToMoveParent.transform;
					Vector3 f3BoneToMoveParentPosition   = rBoneToMoveParentTransform.position;

					Vector3 f3CurrentDirection = this.transform.position - f3BoneToMoveParentPosition;
					Vector3 f3NewDirection     = oWorldRay.GetPoint( fDistance ) - f3BoneToMoveParentPosition;
	
					Quaternion oRot = Quaternion.FromToRotation( f3CurrentDirection, f3NewDirection );
					oRot.ToAngleAxis( out fAngle, out f3Axis );
					
					#if AFTER_UNITY_4_3
					Undo.RecordObject(rBoneToMoveParentTransform, "Rotate Bone : " + rBoneToMoveParentTransform);
					#endif
					rBoneToMoveParentTransform.Rotate( f3Axis, fAngle, Space.World );
					EditorUtility.SetDirty( rBoneToMoveParentTransform );
				}
				else
				{
					#if AFTER_UNITY_4_3
					Undo.RecordObject(transform, "Move Bone : " + transform);
					#endif
					this.transform.position = oWorldRay.GetPoint( fDistance );
					EditorUtility.SetDirty( this.transform );
				}
			}
		}
	}

	public void Break( )
	{
		Transform rSpriteTransform = this.Sprite.transform;
		Transform rBoneTransform   = this.transform;

		// Need to pack bone hierarchy
		Uni2DSmoothBindingBone rBoneParent = this.Parent;
		if( rBoneParent != null && rBoneParent.IsFakeRootBone )
		{
			// Break bone then destroy fake parent
			rBoneTransform.parent = rSpriteTransform;
			Uni2DSmoothBindingBone rBoneGrandParent = rBoneParent.Parent;
			Object.DestroyImmediate( rBoneParent.gameObject );

			// We broke the branch => flatten bone hierarchy
			if( rBoneGrandParent != null && rBoneGrandParent.IsBranchBone == false )	// Grand parent is no longer a branch
			{
				Transform rBoneGrandParentTransform = rBoneGrandParent.transform;

				Uni2DSmoothBindingBone rFakeChild = rBoneGrandParent.Children[ 0 ];	// Fake child

				Transform rBoneChildTransform = rFakeChild.Children[ 0 ].transform;
				rBoneChildTransform.parent = rBoneGrandParentTransform;	// Set grand parent as new parent
				Object.DestroyImmediate( rFakeChild.gameObject );	// Delete fake child

				EditorUtility.SetDirty( rBoneChildTransform );
			}
		}
		else
		{
			rBoneTransform.parent = rSpriteTransform;
		}

		EditorUtility.SetDirty( rBoneTransform );
	}

}
#endif
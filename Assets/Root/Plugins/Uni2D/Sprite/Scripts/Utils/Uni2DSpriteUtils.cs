using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using SpriteRenderMesh = Uni2DSprite.SpriteRenderMesh;
using PivotType        = Uni2DSprite.PivotType;

// Sprite utility
public static class Uni2DSpriteUtils 
{
	// To Sprite Unit
	public const float mc_fSpriteUnitToUnity = 0.01f;
	
	// Set sprite frame
	public static void SetSpriteFrame(Uni2DSprite a_rSpriteComponent, Texture2DContainer a_rTextureContainer, Uni2DTextureAtlas a_rTextureAtlas, float a_fWidth, float a_fHeight, bool a_bUpdateMesh, bool a_bUpdateUV)
	{
		MeshFilter rMeshFilter = a_rSpriteComponent.GetComponent<MeshFilter>( );
		SkinnedMeshRenderer rSkinnedMeshRenderer = a_rSpriteComponent.GetComponent<SkinnedMeshRenderer>( );

		// The mesh to update
		Mesh rSpriteMesh = a_rSpriteComponent.EditableRenderMesh;

		if( rSpriteMesh != null )
		{			
			if( a_bUpdateMesh )
			{
				rSpriteMesh = UpdateSpriteMeshSizeForAnim(a_rSpriteComponent, rSpriteMesh, a_fWidth, a_fHeight);
			}
	
			UpdateMaterialForAnim( a_rSpriteComponent, a_rTextureContainer, a_rTextureAtlas );
	
			if( a_bUpdateUV )
			{
				// Rebuild UVs
				rSpriteMesh.uv = BuildUVs( a_rTextureContainer,
					a_rSpriteComponent.SpriteData.renderMeshUVs,
					a_rTextureAtlas );
			}

			if( rSkinnedMeshRenderer != null && rSkinnedMeshRenderer.sharedMesh != rMeshFilter.sharedMesh )
			{
				rSkinnedMeshRenderer.sharedMesh = rSpriteMesh;
			}
		}
	}
	
	// Update material and texture
	private static void UpdateMaterialForAnim(Uni2DSprite a_rSpriteComponent, Texture2DContainer a_rTextureContainer, Uni2DTextureAtlas a_rTextureAtlas)
	{
		Material oSpriteMaterial = a_rSpriteComponent.SpriteData.renderMeshMaterial;

		if(a_rTextureAtlas == null)
		{
			Material rAnimationMaterial = a_rSpriteComponent.RuntimeData.animationMaterial;

			if( rAnimationMaterial != null )
			{
				oSpriteMaterial = rAnimationMaterial;
			}
			else
			{
				// Instantiate the material to prevent other sprites using this material
				// to be animated too
				oSpriteMaterial = (Material) Material.Instantiate( oSpriteMaterial );
				a_rSpriteComponent.RuntimeData.animationMaterial = oSpriteMaterial;
			}

			oSpriteMaterial.mainTexture = a_rTextureContainer;
		}
		else
		{
			oSpriteMaterial = a_rTextureAtlas.GetMaterial( a_rTextureContainer.GUID );
		}
		
		Renderer rRenderer = a_rSpriteComponent.renderer;
		if(rRenderer != null)
		{
			rRenderer.sharedMaterial = oSpriteMaterial;
			rRenderer.material = oSpriteMaterial;
		}
	}

	public static Vector2[ ] BuildUVs( Texture2DContainer a_rTextureContainer, Vector2[ ] a_rRenderMeshUVs, Uni2DTextureAtlas a_rTextureAtlas )
	{
		// UV data
		Rect oUVRect;
		bool bIsUVRectFlipped;

		// No atlas
		if( a_rTextureAtlas == null )
		{
			oUVRect = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
			bIsUVRectFlipped = false;
		}
		else if( !string.IsNullOrEmpty( a_rTextureContainer.GUID ) && a_rTextureAtlas.GetUVs( a_rTextureContainer.GUID, out oUVRect, out bIsUVRectFlipped ) == false )	// Atlas
		{
			// Not found in atlas!!!
			Debug.LogError( "Uni2D: Atlas \'" + a_rTextureAtlas.name + "\' does not contain the texture with GUID \'" + a_rTextureContainer.GUID + "\'."
				+ "\nCheck your animation clip and atlas settings.", a_rTextureAtlas );
		}

		Vector2[ ] oMeshUVs = a_rRenderMeshUVs;
		int iVertexCount    = oMeshUVs.Length;

		// UV normalization (computes mesh UVs coords in atlas UVs space)
		Vector2[ ] oNormalizedMeshUVs = new Vector2[ iVertexCount ];
		Vector2 f2UVOffset;	// Position (== min coords) of sprite UV rect in atlas. (0;0) if not atlased
		Vector2 f2UVRange;	// UV range (== max coords - position) in atlas. (1;1) if not atlased

		// If UV rect is not flipped...
		if( bIsUVRectFlipped == false )
		{
			f2UVOffset = new Vector2( oUVRect.xMin, oUVRect.yMin );
			f2UVRange  = new Vector2( oUVRect.xMax, oUVRect.yMax ) - f2UVOffset;

			for( int iUVIndex = 0; iUVIndex < iVertexCount; ++iUVIndex )
			{
				oNormalizedMeshUVs[ iUVIndex ] = f2UVOffset + Vector2.Scale( f2UVRange, oMeshUVs[ iUVIndex ] );
			}
		}
		else // UV rect is flipped
		{
			f2UVOffset = new Vector2( oUVRect.xMax, oUVRect.yMin );
			f2UVRange  = new Vector2( oUVRect.xMin, oUVRect.yMax ) - f2UVOffset;

			for( int iUVIndex = 0; iUVIndex < iVertexCount; ++iUVIndex )
			{
				Vector2 f2UV = oMeshUVs[ iUVIndex ];

				// Need to swap UV coords
				oNormalizedMeshUVs[ iUVIndex ] = f2UVOffset + new Vector2( f2UVRange.x * f2UV.y, f2UVRange.y * f2UV.x );
			}
		}

		return oNormalizedMeshUVs;
	}

	// Update the quad mesh size
	public static Mesh UpdateSpriteMeshSizeForAnim( Uni2DSprite a_rSprite, Mesh a_rSpriteMesh, float a_fWidth, float a_fHeight)
	{
		Uni2DEditorSpriteSettings rSpriteSettings = a_rSprite.SpriteSettings;
		Uni2DEditorSpriteData rSpriteData         = a_rSprite.SpriteData;

		// Scale
		float fScale  = rSpriteData.scale;
		float fWidth  = fScale * a_fWidth;
		float fHeight = fScale * a_fHeight;
		
		// Pivot
		Vector2 f2ScaledPivotCoords = fScale * ComputePivotCoords( a_fWidth, a_fHeight, rSpriteSettings.pivotType, rSpriteSettings.pivotCustomCoords );		

		if( a_rSpriteMesh != null )
		{
			switch( rSpriteSettings.renderMesh )
			{
				case SpriteRenderMesh.Quad:
				{
					// Update vertices
					a_rSpriteMesh.vertices = GenerateQuadVertices(fWidth, fHeight, f2ScaledPivotCoords );
				}
				break;
				
				default:
				case SpriteRenderMesh.TextureToMesh:
				case SpriteRenderMesh.Grid:
				{
					// Resize the sprite mesh
					// From the saved original vertices Vo in ( 0; 0 ) base,
					// apply mesh dimensions ratio R = ( Wn; Hn ) / ( Wo; Ho )
					// and finally sub new pivot Pn to have Vn
					// Vo * R - Pn = Vn
					Vector3 f3Ratio  = new Vector3( a_fWidth / rSpriteData.spriteWidth, a_fHeight / rSpriteData.spriteHeight, 0.0f );
					Vector3 f3ScaledPivotCoords = f2ScaledPivotCoords;
					int iVertexCount = a_rSpriteMesh.vertexCount;
					
					Vector3[ ] oResizedVertices = new Vector3[ iVertexCount ];
					
					for( int iVertexIndex = 0; iVertexIndex < iVertexCount; ++iVertexIndex )
					{
						oResizedVertices[ iVertexIndex ] = Vector3.Scale( rSpriteData.renderMeshVertices[ iVertexIndex ], f3Ratio ) - f3ScaledPivotCoords;
					}
					
					a_rSpriteMesh.vertices = oResizedVertices;
				}
				break;
			}
			a_rSpriteMesh.RecalculateBounds( );
		}

		return a_rSpriteMesh;
	}
	
	// Create Quad mesh
	private static Mesh CreateQuadMesh(float a_fWidth, float a_fHeight, Vector3 a_f3PivotPoint, Color a_oVertexColor)
	{
		// Quad mesh
		Mesh oQuadMesh = new Mesh( );

		// Quad vertices
		oQuadMesh.vertices = GenerateQuadVertices(a_fWidth, a_fHeight, a_f3PivotPoint);

		// Quad triangles index (CW)
		oQuadMesh.triangles = new int[ 6 ]
		{
			1, 0, 2,
			2, 0, 3
		};

		// Quad mesh UV coords
		oQuadMesh.uv = new Vector2[ 4 ]
		{
			new Vector2( 0.0f, 0.0f ),
			new Vector2( 1.0f, 0.0f ),
			new Vector2( 1.0f, 1.0f ),
			new Vector2( 0.0f, 1.0f )
		};
		
		// Quad mesh vertex color
		oQuadMesh.colors32 = new Color32[ 4 ]
		{
			a_oVertexColor,
			a_oVertexColor,
			a_oVertexColor,
			a_oVertexColor
		};

		oQuadMesh.RecalculateBounds();
		oQuadMesh.RecalculateNormals();

		// Commented because theorically CreateQuadMesh only used at runtime
		// (and this will be impacting)
		//oQuadMesh.Optimize();

		return oQuadMesh;
	}
	
	// Generate the quad vertices
	public static Vector3[ ] GenerateQuadVertices(float a_fWidth, float a_fHeight, Vector3 a_f3PivotPoint)
	{
		// Quad vertices
		return new Vector3[ 4 ]
		{
			new Vector3(     0.0f,      0.0f, 0.0f ) - a_f3PivotPoint,
			new Vector3( a_fWidth,      0.0f, 0.0f ) - a_f3PivotPoint,
			new Vector3( a_fWidth, a_fHeight, 0.0f ) - a_f3PivotPoint,
			new Vector3(     0.0f, a_fHeight, 0.0f ) - a_f3PivotPoint
		};
	}

	public static void UpdateMeshVertexColor( Mesh a_rMesh, Color32 a_f4VertexColor )
	{
		if( a_rMesh != null )
		{
			int iVertexCount = a_rMesh.vertexCount;
			Color32[ ] oVertexColor32Array = new Color32[ iVertexCount ];
			for( int iVertexIndex = 0; iVertexIndex < iVertexCount; ++iVertexIndex )
			{
				oVertexColor32Array[ iVertexIndex ] = a_f4VertexColor;
			}
	
			a_rMesh.colors32 = oVertexColor32Array;
		}
	}
	
	public static Vector2 ComputePivotCoords( Texture2D a_rTexture, PivotType a_eSpritePivotType, Vector2 a_f2CustomPivotPoint )
	{
		if( a_rTexture == null )
		{
			return Uni2DSpriteUtils.ComputePivotCoords( 0.0f, 0.0f, a_eSpritePivotType, a_f2CustomPivotPoint );
		}
		else
		{
			return Uni2DSpriteUtils.ComputePivotCoords( a_rTexture.width, a_rTexture.height, a_eSpritePivotType, a_f2CustomPivotPoint );
		}
	}
	
	// Sets the pivot point according to its type and given dimensions.
	// If type == custom, a_fWidth and a_fHeight are used as custom pivot point coords
	public static Vector2 ComputePivotCoords( float a_fWidth, float a_fHeight, PivotType a_ePivotType, Vector2 a_f2CustomPivotPoint )
	{
		// Compute new pivot point from pivot type
		switch( a_ePivotType )
		{
			default:
			case PivotType.Custom: 			return a_f2CustomPivotPoint; //new Vector2( a_fWidth,        a_fHeight );
			case PivotType.BottomLeft: 		return new Vector2( 0.0f,            0.0f );
			case PivotType.BottomCenter: 	return new Vector2( a_fWidth * 0.5f, 0.0f );
			case PivotType.BottomRight: 	return new Vector2( a_fWidth,        0.0f );
			case PivotType.MiddleLeft: 		return new Vector2( 0.0f,            a_fHeight * 0.5f );
			case PivotType.MiddleCenter: 	return new Vector2( a_fWidth * 0.5f, a_fHeight * 0.5f );
			case PivotType.MiddleRight: 	return new Vector2( a_fWidth,        a_fHeight * 0.5f );
			case PivotType.TopLeft: 		return new Vector2( 0.0f,            a_fHeight );
			case PivotType.TopCenter: 		return new Vector2( a_fWidth * 0.5f, a_fHeight );
			case PivotType.TopRight: 		return new Vector2( a_fWidth,        a_fHeight );
		}
	}
}
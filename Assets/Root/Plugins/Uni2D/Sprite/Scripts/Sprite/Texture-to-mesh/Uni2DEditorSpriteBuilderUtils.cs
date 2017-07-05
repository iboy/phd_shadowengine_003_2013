#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditorInternal;

using SpriteRenderMesh = Uni2DSprite.SpriteRenderMesh;
using Uni2DTextureImporterSettingsPair = System.Collections.Generic.KeyValuePair<UnityEngine.Texture2D, UnityEditor.TextureImporterSettings>;

/**
 * Static util class to create a mesh collider from the edges
 * of a sprite (texture2D)
 */
public static class Uni2DEditorSpriteBuilderUtils 
{
	public const string mc_oSpriteDefaultShader = "Particles/Alpha Blended";
	
	// Undo
	private static bool ms_bUndoEnabled = true;

	// NEW
	// Same as CreateSpriteFromSettings but updates instead of create if a_rSpriteToUpdate != null
	// Rebuild only if necessary but can be forced with a_bForceRebuild == true.
	public static GameObject GenerateSpriteFromSettings( Uni2DEditorSpriteSettings a_rNewSpriteSettings, Uni2DSprite a_rSprite = null, bool a_bForceRebuild = false, bool a_bInABatch = false )
	{
		// Are we updating or creating?
		bool bUpdating = ( a_rSprite != null );

		// Are we allowed to undo if updating
		bool bRegisterUpdateUndo = ms_bUndoEnabled && bUpdating;

		// Game object
		GameObject oSpriteGameObject = bUpdating
			? a_rSprite.gameObject
			: new GameObject( "Sprite_" + a_rNewSpriteSettings.textureContainer.Texture.name );

		// Sprite component
		Uni2DSprite rSprite = bUpdating
			? a_rSprite
			: Uni2DSprite.Create( oSpriteGameObject );
		
		if( a_bInABatch == false )
		{
			// Get texture importer...
			string oTexturePath = AssetDatabase.GetAssetPath( a_rNewSpriteSettings.textureContainer.Texture );
			TextureImporter rTextureImporter = TextureImporter.GetAtPath( oTexturePath ) as TextureImporter;
	
			// ...if any...
			if( rTextureImporter != null )
			{
				if( bRegisterUpdateUndo )
				{
					// Allow undo this update
					Undo.RegisterSceneUndo( "Update Uni2D sprite" );
				}
	
				TextureImporterSettings oTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin( rTextureImporter );
	
				// Create/update sprite material
				GenerateSpriteMatFromSettings( a_rNewSpriteSettings, rSprite );
	
				// Create/update sprite mesh
				GenerateSpriteMeshFromSettings( a_rNewSpriteSettings, rSprite );
	
				// Create/update colliders
				GenerateCollidersFromSettings( a_rNewSpriteSettings, rSprite, a_bForceRebuild );
	
				// Post build operations
				rSprite.AfterBuild();
	
				Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd( rTextureImporter, oTextureImporterSettings );
			}
	
			// Allow undo the creation (i.e. bUpdating == false)
			if( bUpdating == false && ms_bUndoEnabled )
			{
				Undo.RegisterCreatedObjectUndo( oSpriteGameObject, "Create Uni2D sprite" );
			}
		}
		else // Batching: no texture imports, no register undos...
		{
			// Create/update sprite material
			GenerateSpriteMatFromSettings( a_rNewSpriteSettings, rSprite );

			// Create/update sprite mesh
			GenerateSpriteMeshFromSettings( a_rNewSpriteSettings, rSprite );

			// Create/update colliders
			GenerateCollidersFromSettings( a_rNewSpriteSettings, rSprite, a_bForceRebuild );

			// Post build operations
			rSprite.AfterBuild();
		}

		return oSpriteGameObject;
	}

	// NEW
	// Creates/updates the quad mesh of a sprite according to given new settings.
	public static GameObject GenerateSpriteMeshFromSettings( Uni2DEditorSpriteSettings a_rNewSpriteSettings, Uni2DSprite a_rSprite )
	{
		GameObject rSpriteGameObject = a_rSprite.gameObject;

		Uni2DEditorSpriteData rCurrentSpriteData         = a_rSprite.SpriteData;
		Uni2DEditorSpriteSettings rCurrentSpriteSettings = a_rSprite.SpriteSettings;

		Texture2DContainer rTextureContainer = a_rNewSpriteSettings.textureContainer;
		Texture2D rSpriteTexture = rTextureContainer;

		// Init. mesh filter component
		// Add it if not created
		MeshFilter rSpriteMeshFilter = a_rSprite.GetComponent<MeshFilter>( );
		if( rSpriteMeshFilter == null )
		{
			rSpriteMeshFilter = rSpriteGameObject.AddComponent<MeshFilter>( );
		}

		// Retrieve pivot coords
		Vector2 f2ScaledPivotCoords = a_rNewSpriteSettings.ScaledPivotCoords;

		// Quad mesh
		Mesh oSpriteMesh;
		Uni2DTextureAtlas rTextureAtlas = a_rNewSpriteSettings.atlas;
		Vector3[ ] oRenderMeshVertices = null;
		Vector2[ ] oRenderMeshNormalizedUVs = null;
		int iVertexCount;

		SpriteRenderMesh eRenderMesh = a_rNewSpriteSettings.renderMesh;

		// Check if using atlas and texture is well included in this atlas
		if( a_rNewSpriteSettings.ShouldUseAtlas( ) == false )
		{
			// No atlas => clean settings
			a_rNewSpriteSettings.atlas = null;
			rTextureAtlas = null;
		}

		// Render Mesh
		switch( eRenderMesh )
		{
			// Quad
			default:
			case SpriteRenderMesh.Quad:
			{
				// Mesh creation
				oSpriteMesh = new Mesh( );
				
				// Quad vertices
				oRenderMeshVertices = Uni2DSpriteUtils.GenerateQuadVertices( a_rNewSpriteSettings.SpriteWidth,
					a_rNewSpriteSettings.SpriteHeight,
					f2ScaledPivotCoords );
				
				oSpriteMesh.vertices = oRenderMeshVertices;
				iVertexCount = 4;
				
				// Quad triangles
				oSpriteMesh.triangles = new int[ 6 ]
				{
					1, 0, 2,
					2, 0, 3
				};
				
				// Quad UVs
				oRenderMeshNormalizedUVs = new Vector2[ 4 ]
				{
					new Vector2( 0.0f, 0.0f ),
					new Vector2( 1.0f, 0.0f ),
					new Vector2( 1.0f, 1.0f ),
					new Vector2( 0.0f, 1.0f )
				};
			}
			break;

			// Create a sprite via texture polygonization
			case SpriteRenderMesh.Grid:
			case SpriteRenderMesh.TextureToMesh:
			{
				oSpriteMesh = PolygonizeTextureToMeshFromSettings( a_rNewSpriteSettings );
				
				// Copy render mesh vertices/UVs (.vertices & .uv return a copy)
				oRenderMeshNormalizedUVs = oSpriteMesh.uv;
				oRenderMeshVertices      = oSpriteMesh.vertices;
				iVertexCount             = oSpriteMesh.vertexCount;
			}
			break;
		}


		// Build UVs from various settings (atlasing, render mesh, etc)
		oSpriteMesh.uv = Uni2DSpriteUtils.BuildUVs( rTextureContainer, oRenderMeshNormalizedUVs, rTextureAtlas );

		// Re-iterate vertices and unapply pivot in order to have
		// a copy of all mesh vertices in ( 0; 0 ) base
		Vector3 f3ScaledPivotCoords = f2ScaledPivotCoords;
		for( int iVertexIndex = 0; iVertexIndex < iVertexCount; ++iVertexIndex )
		{
			oRenderMeshVertices[ iVertexIndex ] += f3ScaledPivotCoords;
		}

		// Set vertex color array (32bits)
		Uni2DSpriteUtils.UpdateMeshVertexColor( oSpriteMesh, a_rNewSpriteSettings.vertexColor );


		// Optim. mesh
		oSpriteMesh.RecalculateBounds( );
		oSpriteMesh.RecalculateNormals( );
		oSpriteMesh.Optimize( );
		
		oSpriteMesh.name = "mesh_Sprite_";
		
		if(rSpriteTexture != null)
		{
			oSpriteMesh.name += rSpriteTexture.name;	
		}
		else
		{
			oSpriteMesh.name += "MissingTexture";
		}
		
		// Set new sprite settings
		rCurrentSpriteSettings.pivotType         = a_rNewSpriteSettings.pivotType;
		rCurrentSpriteSettings.pivotCustomCoords = a_rNewSpriteSettings.pivotCustomCoords;
		rCurrentSpriteSettings.spriteScale       = a_rNewSpriteSettings.spriteScale;
		rCurrentSpriteSettings.atlas             = rTextureAtlas;
		rCurrentSpriteSettings.textureContainer  = new Texture2DContainer( rSpriteTexture, rTextureAtlas == null );

		rCurrentSpriteSettings.renderMeshAlphaCutOff            = a_rNewSpriteSettings.renderMeshAlphaCutOff;
		rCurrentSpriteSettings.renderMeshPolygonizationAccuracy = a_rNewSpriteSettings.renderMeshPolygonizationAccuracy;
		rCurrentSpriteSettings.renderMeshPolygonizeHoles		= a_rNewSpriteSettings.renderMeshPolygonizeHoles;
		rCurrentSpriteSettings.usePhysicBuildSettings           = a_rNewSpriteSettings.usePhysicBuildSettings;
		rCurrentSpriteSettings.renderMesh                       = a_rNewSpriteSettings.renderMesh;
		rCurrentSpriteSettings.renderMeshGridHorizontalSubDivs  = a_rNewSpriteSettings.renderMeshGridHorizontalSubDivs;
		rCurrentSpriteSettings.renderMeshGridVerticalSubDivs    = a_rNewSpriteSettings.renderMeshGridVerticalSubDivs;

		// Set new sprite generated data
		rCurrentSpriteData.renderMesh         = oSpriteMesh;
		rCurrentSpriteData.renderMeshVertices = oRenderMeshVertices;
		rCurrentSpriteData.renderMeshUVs      = oRenderMeshNormalizedUVs;
		if(rSpriteTexture != null)
		{
			rCurrentSpriteData.spriteWidth        = rSpriteTexture.width;
			rCurrentSpriteData.spriteHeight       = rSpriteTexture.height;
		}
		// Set computed pivot coords
		rCurrentSpriteData.pivotCoords        = a_rNewSpriteSettings.PivotCoords;
		rCurrentSpriteData.scale              = a_rNewSpriteSettings.ScaleFactor;

		a_rSprite.atlasGenerationID = ( rTextureAtlas != null )
			? rTextureAtlas.generationId
			: "";

		// Set mesh to mesh filter component
		rSpriteMeshFilter.sharedMesh = oSpriteMesh;

		// Compute bone weights if needed
		GenerateBoneWeightsFromSettings( a_rNewSpriteSettings, a_rSprite );

		
		return rSpriteGameObject;
	}

	// NEW
	// Creates/updates the quad mesh material of a sprite according to given new settings.
	public static GameObject GenerateSpriteMatFromSettings( Uni2DEditorSpriteSettings a_rNewSpriteSettings, Uni2DSprite a_rSprite )
	{
		Material oSpriteRendererMaterial;
		Material oSpriteMeshMaterial;

		Uni2DEditorSpriteData rCurrentSpriteData         = a_rSprite.SpriteData;
		Uni2DEditorSpriteSettings rCurrentSpriteSettings = a_rSprite.SpriteSettings;

		Texture2D rNewSpriteTexture = a_rNewSpriteSettings.textureContainer;

		GameObject rSpriteGameObject = a_rSprite.gameObject;
		
		// If a shared material is used with a texture different from the sprite texture
		// or if an atlas is used
		if(a_rNewSpriteSettings.ShouldUseAtlas( ) 
			|| (a_rNewSpriteSettings.sharedMaterial != null && a_rNewSpriteSettings.textureContainer.Texture != a_rNewSpriteSettings.sharedMaterial.mainTexture) )
		{
			// don't use the shared material
			a_rNewSpriteSettings.sharedMaterial = null;
		}
		
		Material rGeneratedMaterial = null;
		if(a_rNewSpriteSettings.sharedMaterial == null && a_rNewSpriteSettings.ShouldUseAtlas( ) == false)
		{
			// If the material doesn't exist yet, create it
			rGeneratedMaterial = rCurrentSpriteData.generatedMaterial;
			if( rGeneratedMaterial == null )
			{
				// create a new one
				rGeneratedMaterial = new Material( Shader.Find( mc_oSpriteDefaultShader ) );
	
				// Set mat name
				rGeneratedMaterial.name = "mat_Generated_Sprite_" + rNewSpriteTexture.name;
				
				rCurrentSpriteData.generatedMaterial = rGeneratedMaterial;
			}
		}
		rCurrentSpriteData.generatedMaterial = rGeneratedMaterial;
		
		Material rSharedMaterial = a_rNewSpriteSettings.sharedMaterial;
		if(rSharedMaterial == null)
		{
			oSpriteMeshMaterial = rGeneratedMaterial;
		}
		else
		{
			oSpriteMeshMaterial = rSharedMaterial;
		}
		
		// If no atlas...
		if( a_rNewSpriteSettings.ShouldUseAtlas( ) == false )
		{
			oSpriteRendererMaterial = oSpriteMeshMaterial;
			a_rNewSpriteSettings.atlas = null;
			
			// Set mat texture
			oSpriteMeshMaterial.mainTexture = rNewSpriteTexture;
		}
		else
		{
			oSpriteRendererMaterial = a_rNewSpriteSettings.atlas.GetMaterial( rNewSpriteTexture );
			oSpriteMeshMaterial = oSpriteRendererMaterial;
		}
		
		// Init. rendering component
		// Add it if not created
		Renderer rSpriteMeshRendererComponent = a_rSprite.renderer;// a_rSprite.GetComponent<MeshRenderer>( );

		if( rSpriteMeshRendererComponent == null )
		{
			if( a_rSprite.Bones.Length > 0 )
			{
				rSpriteMeshRendererComponent = rSpriteGameObject.AddComponent<SkinnedMeshRenderer>( );
			}
			else
			{
				rSpriteMeshRendererComponent = rSpriteGameObject.AddComponent<MeshRenderer>( );
			}
		}

		// Set new material to mesh renderer
		rSpriteMeshRendererComponent.sharedMaterial = oSpriteRendererMaterial;

		// Update sprite settings
		rCurrentSpriteSettings.textureContainer = new Texture2DContainer( rNewSpriteTexture, a_rNewSpriteSettings.atlas == null );
		rCurrentSpriteSettings.atlas            = a_rNewSpriteSettings.atlas;

		// Update sprite generated data
		rCurrentSpriteData.renderMeshMaterial   = oSpriteMeshMaterial;

		return rSpriteGameObject;
	}

	// NEW
	// Creates/updates colliders according to given new settings
	public static GameObject GenerateCollidersFromSettings( Uni2DEditorSpriteSettings a_rNewSpriteSettings, Uni2DSprite a_rSprite, bool a_bForceRebuild )
	{
		Uni2DEditorSpriteSettings rCurrentSpriteSettings = a_rSprite.SpriteSettings;
		Uni2DEditorSpriteData rCurrentSpriteData = a_rSprite.SpriteData;

		GameObject rSpriteGameObject = a_rSprite.gameObject;

		// Is rebuilding needed?
		bool bRebuild = a_bForceRebuild
			|| a_rSprite.isPhysicsDirty
			|| rCurrentSpriteSettings.DoNewSettingsImplyToRebuildPhysics( a_rNewSpriteSettings );

		if( bRebuild )
		{
			// Clean the sprite (if needed)
			RemoveGeneratedColliders( a_rSprite );

			// Generate meshes
			List<Mesh> rMeshList = PolygonizeTextureToMeshColliderFromSettings( a_rNewSpriteSettings );
			
			if( rMeshList == null )
			{
				rMeshList = new List<Mesh>( );
			}

			List<MeshCollider> oMeshColliderComponentsList = new List<MeshCollider>( rMeshList.Count );

			// Add collider children
			// Attach a mesh collider collider to current game object
			// if collider is not compound
			GameObject oColliderParentGameObject = null;

			// Mesh collider triangle count
			int iMeshColliderTriangleCount = 0;

			// Components creation
			if( a_rNewSpriteSettings.physicsMode != Uni2DSprite.PhysicsMode.NoPhysics )
			{
				// Compound
				if( a_rNewSpriteSettings.collisionType == Uni2DSprite.CollisionType.Compound )
				{
					oColliderParentGameObject = new GameObject( "root_Colliders" );
		
					// Create a game object for each mesh collider and attach them to sprite game object
					for( int iColliderIndex = 0, iMeshCount = rMeshList.Count; iColliderIndex < iMeshCount; ++iColliderIndex )
					{
						GameObject oMeshColliderGameObject  = new GameObject( "mesh_Collider_" + iColliderIndex );
						MeshCollider oMeshColliderComponent = oMeshColliderGameObject.AddComponent<MeshCollider>( );
						oMeshColliderComponent.sharedMesh   = rMeshList[ iColliderIndex ];
						oMeshColliderComponent.convex       = true;
		
						oMeshColliderComponentsList.Add( oMeshColliderComponent );
		
						// Child -> parent attachment
						oMeshColliderGameObject.transform.parent = oColliderParentGameObject.transform;

						iMeshColliderTriangleCount += rMeshList[ iColliderIndex ].triangles.Length;
					}
					Transform rColliderParentTransform = oColliderParentGameObject.transform;
					rColliderParentTransform.parent        = rSpriteGameObject.transform;
					rColliderParentTransform.localPosition = Vector3.zero;
					rColliderParentTransform.localRotation = Quaternion.identity;
					rColliderParentTransform.localScale    = Vector3.one;
				}
				else // Static / Dynamic
				{
					MeshCollider oMeshColliderComponent = rSpriteGameObject.GetComponent<MeshCollider>( );
					if( oMeshColliderComponent == null )
					{
						oMeshColliderComponent = rSpriteGameObject.AddComponent<MeshCollider>( );
					}

					oMeshColliderComponent.sharedMesh = rMeshList[ 0 ];
					oMeshColliderComponentsList.Add( oMeshColliderComponent );

					// Set whether or not mesh collider is convex
					oMeshColliderComponent.convex = ( a_rNewSpriteSettings.collisionType == Uni2DSprite.CollisionType.Convex );
					iMeshColliderTriangleCount = rMeshList[ 0 ].triangles.Length;
				}

				// Add rigidbody to sprite game object if any dynamic mode is specified
				Uni2DEditorSpriteBuilderUtils.SetupRigidbodyFor2D( a_rSprite, a_rNewSpriteSettings );
			}

			// Settings
			rCurrentSpriteSettings.alphaCutOff            = a_rNewSpriteSettings.alphaCutOff;
			rCurrentSpriteSettings.collisionType          = a_rNewSpriteSettings.collisionType;
			rCurrentSpriteSettings.extrusionDepth         = a_rNewSpriteSettings.extrusionDepth;
			rCurrentSpriteSettings.isKinematic            = a_rNewSpriteSettings.isKinematic;
			rCurrentSpriteSettings.physicsMode            = a_rNewSpriteSettings.physicsMode;
			rCurrentSpriteSettings.pivotType              = a_rNewSpriteSettings.pivotType;
			rCurrentSpriteSettings.pivotCustomCoords      = a_rNewSpriteSettings.pivotCustomCoords;
			rCurrentSpriteSettings.polygonizationAccuracy = a_rNewSpriteSettings.polygonizationAccuracy;
			rCurrentSpriteSettings.polygonizeHoles        = a_rNewSpriteSettings.polygonizeHoles;

			// Generated data
			rCurrentSpriteData.meshColliderComponentsList  = oMeshColliderComponentsList;
			rCurrentSpriteData.meshCollidersList           = rMeshList;
			rCurrentSpriteData.meshCollidersRootGameObject = oColliderParentGameObject;
			rCurrentSpriteData.colliderTriangleCount       = ( iMeshColliderTriangleCount / 3 );
			rCurrentSpriteData.pivotCoords                 = a_rNewSpriteSettings.PivotCoords;

		}

		// Is up to date!
		a_rSprite.isPhysicsDirty = false;

		return rSpriteGameObject;
	}

	public static void GenerateBoneWeightsFromSettings( Uni2DEditorSpriteSettings a_rNewSpriteSettings, Uni2DSprite a_rSprite )
	{
		a_rSprite.RestorePosePosition( );
		a_rSprite.UpdatePosing( );

		a_rSprite.SpriteSettings.boneInfluenceFalloff = a_rNewSpriteSettings.boneInfluenceFalloff;
		a_rSprite.SpriteSettings.skinQuality          = a_rNewSpriteSettings.skinQuality;
	}

	// NEW
	// Parse a 2D texture and make mesh colliders from edges
	public static Mesh PolygonizeTextureToMeshFromSettings( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		// Ensure we have an input texture and an output render mesh requiring to parse that texture to be built
		if( a_rSpriteSettings.textureContainer == null || a_rSpriteSettings.renderMesh == SpriteRenderMesh.Quad )
		{
			return null;
		}

		// The texture to polygonize
		Texture2D rTextureToPolygonize = a_rSpriteSettings.textureContainer;
		float fAlphaCutOff;
		float fPolygonizationAccuracy;

		if( a_rSpriteSettings.usePhysicBuildSettings )
		{
			fAlphaCutOff            = a_rSpriteSettings.alphaCutOff;
			fPolygonizationAccuracy = a_rSpriteSettings.polygonizationAccuracy;
		}
		else
		{
			fAlphaCutOff            = a_rSpriteSettings.renderMeshAlphaCutOff;
			fPolygonizationAccuracy = a_rSpriteSettings.renderMeshPolygonizationAccuracy;
		}

		// Step 1
		// Distinguish completely transparent pixels from significant pixel by "binarizing" the texture.
		Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
		BinarizedImage rBinarizedImage = Uni2DEditorShapeExtractionUtils.BinarizeTexture( rTextureToPolygonize, fAlphaCutOff );


		// Mesh creation
		Mesh rMesh;
		switch( a_rSpriteSettings.renderMesh )
		{
			case SpriteRenderMesh.TextureToMesh:
			{
				// Step 2
				// Build binarized outer/inner contours and label image regions
				List<Contour> oOuterContours;
				List<Contour> oInnerContours;
				
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
				Uni2DEditorContourExtractionUtils.CombinedContourLabeling( rBinarizedImage, a_rSpriteSettings.renderMeshPolygonizeHoles, out oOuterContours, out oInnerContours );
				
				// Step 3: vectorization (determine dominant points)
				if( a_rSpriteSettings.renderMeshPolygonizeHoles )
				{
					// Step 3a: if hole support asked by user, merge inner contours into outer contours first
					oOuterContours = Uni2DEditorContourPolygonizationUtils.MergeInnerAndOuterContours( oOuterContours, oInnerContours );
				}
			
				// Simplify contours
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
				List<Contour> oDominantContoursList = Uni2DEditorContourPolygonizationUtils.SimplifyContours( oOuterContours, fPolygonizationAccuracy );
				
				// Step 4: triangulation
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
				rMesh = Uni2DEditorPolygonTriangulationUtils.PolygonizeContours( oDominantContoursList,
					a_rSpriteSettings.ScaleFactor,
					a_rSpriteSettings.PivotCoords,
					rTextureToPolygonize.width,
					rTextureToPolygonize.height );
				
			}
			break;

			case SpriteRenderMesh.Grid:
			{
				// Step 2
				// Triangulate the grid straight from binarized image
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
				rMesh = Uni2DEditorPolygonTriangulationUtils.PolygonizeGrid( rBinarizedImage,
					a_rSpriteSettings.ScaleFactor,
					a_rSpriteSettings.PivotCoords,
					a_rSpriteSettings.renderMeshGridHorizontalSubDivs,
					a_rSpriteSettings.renderMeshGridVerticalSubDivs );
			}
			break;

			default:
			{
				return null;
			}
		}

		// Wrap up the mesh before delivering ;^)
		rMesh.name = "mesh_Sprite_" + rTextureToPolygonize.name;
		rMesh.RecalculateBounds( );
		rMesh.RecalculateNormals( );
		rMesh.Optimize( );

		return rMesh;
	}

	// NEW
	// Parse a 2D texture and make mesh colliders from edges
	public static List<Mesh> PolygonizeTextureToMeshColliderFromSettings( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		if( a_rSpriteSettings.textureContainer == null || a_rSpriteSettings.physicsMode == Uni2DSprite.PhysicsMode.NoPhysics )
		{
			return null;
		}
		// The texture to polygonize
		Texture2D rTextureToPolygonize = a_rSpriteSettings.textureContainer;

		// Polygonize holes?
		bool bPolygonizeHoles = ( a_rSpriteSettings.collisionType != Uni2DSprite.CollisionType.Convex && a_rSpriteSettings.polygonizeHoles );

		// Step 1
		// Distinguish completely transparent pixels from significant pixel by "binarizing" the texture.
		Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
		BinarizedImage rBinarizedImage = Uni2DEditorShapeExtractionUtils.BinarizeTexture( rTextureToPolygonize, a_rSpriteSettings.alphaCutOff );
		
		// Step 2
		// Build binarized outer/inner contours and label image regions
		List<Contour> oOuterContours;
		List<Contour> oInnerContours;

		Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
		Uni2DEditorContourExtractionUtils.CombinedContourLabeling( rBinarizedImage, bPolygonizeHoles, out oOuterContours, out oInnerContours );


		// Step 3: vectorization (determine dominant points)
		if( a_rSpriteSettings.polygonizeHoles )
		{
			// Step 3a: if hole support asked by user, merge inner contours into outer contours first
			oOuterContours = Uni2DEditorContourPolygonizationUtils.MergeInnerAndOuterContours( oOuterContours, oInnerContours );
		}

		// Simplify contours
		Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
		List<Contour> oDominantContoursList = Uni2DEditorContourPolygonizationUtils.SimplifyContours( oOuterContours, a_rSpriteSettings.polygonizationAccuracy );

		// Step 4: triangulation
		List<Mesh> oMeshesList;

		Uni2DEditorUtilsBuildingProgressBar.AddProcessedSpriteBuildStep( );
		if( a_rSpriteSettings.collisionType == Uni2DSprite.CollisionType.Compound )
		{
			// Compound mesh
			oMeshesList = Uni2DEditorPolygonTriangulationUtils.CompoundPolygonizeAndExtrudeContours( oDominantContoursList,
				a_rSpriteSettings.extrusionDepth,
				a_rSpriteSettings.ScaleFactor,
				a_rSpriteSettings.PivotCoords );

			int iMeshIndex = 0;
            foreach( Mesh rMesh in oMeshesList )
            {
                rMesh.name = "mesh_Collider_" + rTextureToPolygonize.name + "_" + iMeshIndex;
                ++iMeshIndex;
            }
		}
		else
		{
			// Single mesh
			Mesh rMesh = Uni2DEditorPolygonTriangulationUtils.PolygonizeAndExtrudeContours( oDominantContoursList,
				a_rSpriteSettings.extrusionDepth,
				a_rSpriteSettings.ScaleFactor,
				a_rSpriteSettings.PivotCoords );

			rMesh.name = "mesh_Collider_" + rTextureToPolygonize.name;
			oMeshesList = new List<Mesh>( 1 );
			oMeshesList.Add( rMesh );
		}

		// Optimize meshes
		foreach( Mesh rMesh in oMeshesList )
		{
			rMesh.RecalculateBounds( );
			rMesh.RecalculateNormals( );
			rMesh.Optimize( );
		}

		return oMeshesList;
	}

	// NEW
	// Removes the generated physic colliders attached to a sprite
	private static void RemoveGeneratedColliders( Uni2DSprite a_rSpriteToClean )
	{
		if( a_rSpriteToClean != null )
		{
			// Retrieve the generated data of the sprite to clean
			Uni2DEditorSpriteData rSpriteData = a_rSpriteToClean.SpriteData;

			// Delete resources
			// Delete game objects in excess
			foreach( MeshCollider rMeshColliderComponent in rSpriteData.meshColliderComponentsList )
			{
				if( rMeshColliderComponent != null )
				{
					// If the mesh collider component is attached to another game object
					// than the sprite itself, we destroy it too (because that component was its unique purpose)
					if( rMeshColliderComponent.gameObject != a_rSpriteToClean.gameObject )
					{
						// Destroy game object AND component
						GameObject.DestroyImmediate( rMeshColliderComponent.gameObject );
					}
					else
					{
						// Destroy component
						MonoBehaviour.DestroyImmediate( rMeshColliderComponent );
					}
				}
			}

			// Destroy mesh collider root game object (if any)
			if( rSpriteData.meshCollidersRootGameObject != null )
			{
				// Destroy game object
				GameObject.DestroyImmediate( rSpriteData.meshCollidersRootGameObject );
			}

			// Delete components in excess
			Rigidbody rRigidbodyComponent = a_rSpriteToClean.GetComponent<Rigidbody>( );
			if( rRigidbodyComponent != null )
			{
				// Destroy component
				MonoBehaviour.DestroyImmediate( rRigidbodyComponent );
			}

			// Reset mesh list
			rSpriteData.meshCollidersList          = null;
			rSpriteData.meshColliderComponentsList = null;
			rSpriteData.colliderTriangleCount      = 0;
		}
	}

	////////////////////////////////////////////////////////////////////////////////

	public static void RegenerateSpritesInABatch( List<Uni2DSprite> a_rSprites, bool a_bForce = false )
	{
		// Prepare textures for processing
		List<Uni2DSprite> oDirtySprites           = new List<Uni2DSprite>();
		HashSet<Texture> oAlreadyPreparedTextures = new HashSet<Texture>();

		try
		{
			// Create progress bar
			Uni2DEditorUtilsBuildingProgressBar.CreateBuildingProgressBar( a_rSprites.Count );
	
			List<Uni2DTextureImporterSettingsPair> oTextureImportersSettings = new List<Uni2DTextureImporterSettingsPair>();
			foreach( Uni2DSprite rSprite in a_rSprites )
			{
				if( rSprite != null )
				{
					// If we haven't yet prepare this sprite texture
					Texture2D rTexture = rSprite.SpriteSettings.textureContainer;
					if( oAlreadyPreparedTextures.Contains( rTexture ) == false )
					{
						oAlreadyPreparedTextures.Add( rTexture );
						TextureImporterSettings rTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin( rTexture );
						oTextureImportersSettings.Add( new Uni2DTextureImporterSettingsPair( rTexture, rTextureImporterSettings ) );
					}
					
					// We have to update this sprite
					oDirtySprites.Add( rSprite );
				}
	
				// Add processed texture import to current progress
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedTextureImport( );
			}
						
			// Loop through all the dirty sprites and update them
			foreach( Uni2DSprite rSprite in oDirtySprites )
			{
				rSprite.RebuildInABatch( a_bForce );
	
				// Add processed sprite to current progress
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedSprite( );
			}
	
			// Restore textures settings
			foreach( Uni2DTextureImporterSettingsPair rTextureImporterSettings in oTextureImportersSettings )
			{
				Uni2DAssetPostprocessor.Enabled = false;
				Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporterSettings.Key, rTextureImporterSettings.Value);
	
				// Add processed texture reimport to current progress
				Uni2DEditorUtilsBuildingProgressBar.AddProcessedTextureReimport( );
			}
		}
		finally
		{
			Uni2DAssetPostprocessor.Enabled = true;

			// Clear progress bar
			Uni2DEditorUtilsBuildingProgressBar.ClearBuildingProgressBar( );
		}
	}

	public static void UpdateAllDirtySpritesPhysic( List<Uni2DSprite> a_rSprites )
	{
		// Prepare textures for processing
		List<Uni2DSprite> oDirtySprites = new List<Uni2DSprite>();
		HashSet<Texture> oAlreadyPreparedTextures = new HashSet<Texture>();
		List<Uni2DTextureImporterSettingsPair> oTextureImportersSettings = new List<Uni2DTextureImporterSettingsPair>();
		foreach(Uni2DSprite rSprite in a_rSprites)
		{
			if( rSprite != null && rSprite.isPhysicsDirty )
			{
				// If we haven't yet prepare this sprite texture
				Texture2D rTexture = rSprite.SpriteSettings.textureContainer;
				if( oAlreadyPreparedTextures.Contains( rTexture ) == false )
				{
					oAlreadyPreparedTextures.Add(rTexture);
					TextureImporterSettings rTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTexture);
					oTextureImportersSettings.Add(new Uni2DTextureImporterSettingsPair(rTexture, rTextureImporterSettings));
				}
				
				// We have to update this sprite
				oDirtySprites.Add(rSprite);
			}
		}
					
		// Loop through all the dirty sprites and update them
		foreach(Uni2DSprite rSprite in oDirtySprites)
		{
			rSprite.RebuildInABatch();
		}

		// Restore textures settings
		foreach(Uni2DTextureImporterSettingsPair rTextureImporterSettings in oTextureImportersSettings)
		{
			Uni2DAssetPostprocessor.Enabled = false;
			Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporterSettings.Key, rTextureImporterSettings.Value);
		}
		Uni2DAssetPostprocessor.Enabled = true;
	}
	
	
	public static void UpdateAllSceneSpritesOfTexture(Texture2D a_rTextureToPolygonize)
	{
		if(a_rTextureToPolygonize == null)
		{
			return;
		}
		
		string oTexturePath = AssetDatabase.GetAssetPath(a_rTextureToPolygonize);
		TextureImporter rTextureImporter = TextureImporter.GetAtPath(oTexturePath) as TextureImporter;
		if(rTextureImporter != null)
		{	
			TextureImporterSettings oTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTextureImporter);
		
			DoUpdateAllSceneSpritesOfTexture(a_rTextureToPolygonize);
			
			Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporter, oTextureImporterSettings);
			EditorUtility.UnloadUnusedAssets( );
		}
	}
	
	public static void DoUpdateAllSceneSpritesOfTexture(Texture2D a_rTexture)
	{
		// Loop through all the scene sprites
		foreach(Uni2DSprite rSprite in MonoBehaviour.FindObjectsOfType(typeof(Uni2DSprite)))
		{
			if(rSprite.SpriteSettings.textureContainer.Texture == a_rTexture)
			{
				rSprite.RebuildInABatch();
			}
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////

	public static void RegenerateInteractiveDataFromSettings( Uni2DSprite a_rSprite )
	{
		// New sprite settings
		Uni2DEditorSpriteSettings rSpriteSettings = a_rSprite.SpriteSettings;

		// Current sprite data
		Uni2DEditorSpriteData rSpriteData = a_rSprite.SpriteData;
		
		
		if(rSpriteSettings.ShouldUseAtlas())
		{
			// If using the atlas remove the shared material
			rSpriteSettings.sharedMaterial = null;
		}
		else if(rSpriteSettings.sharedMaterial != null)
		{
			// If the shared material is in use
			// Ensure the sprite texture is the shared material texture
			
			Texture rSharedMaterialTexture = rSpriteSettings.sharedMaterial.mainTexture; 
			if(rSharedMaterialTexture != null)
			{
				rSpriteSettings.textureContainer = new Texture2DContainer( (Texture2D)rSharedMaterialTexture, false );
			}
			else
			{
				rSpriteSettings.sharedMaterial.mainTexture = rSpriteSettings.textureContainer;
			}
		}
		
		Texture2DContainer rSpriteTextureContainer = rSpriteSettings.textureContainer;
		Texture2D rSpriteTexture = rSpriteTextureContainer;
		
		a_rSprite.RestorePosePosition( );
		// If texture changed...
		if( a_rSprite.m_oTextureImportGUID != Uni2DEditorUtils.GetTextureImportGUID( rSpriteTexture ) )
		{
			string oTexturePath = AssetDatabase.GetAssetPath( rSpriteTexture );
			TextureImporter rTextureImporter = TextureImporter.GetAtPath(oTexturePath) as TextureImporter;

			if(rTextureImporter != null)
			{
				// Regenerate sprite mat & mesh
				TextureImporterSettings oTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTextureImporter);
				Uni2DEditorSpriteBuilderUtils.GenerateSpriteMatFromSettings( rSpriteSettings, a_rSprite );
				Uni2DEditorSpriteBuilderUtils.GenerateSpriteMeshFromSettings( rSpriteSettings, a_rSprite );

				// Don't forget to update texture import ID
				a_rSprite.AfterBuild();
				Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd( rTextureImporter, oTextureImporterSettings );
			}
		}
		else
		{
			Uni2DEditorSpriteBuilderUtils.GenerateSpriteMatFromSettings( rSpriteSettings, a_rSprite );
			
			float fRealScale           = rSpriteSettings.ScaleFactor;

			// Current pivot point
			Vector2 f2CurrentPivotPoint = rSpriteData.pivotCoords;
			Vector2 f2NewPivotCoords    = Uni2DSpriteUtils.ComputePivotCoords( rSpriteData.spriteWidth, rSpriteData.spriteHeight, rSpriteSettings.pivotType, rSpriteSettings.pivotCustomCoords );
		
			// The delta to apply
			float fScalingDelta        = fRealScale / rSpriteData.scale;
			Vector3 f3ScaledDeltaPivot = ( f2NewPivotCoords - f2CurrentPivotPoint ) * fRealScale;
	
			// Rigidbody settings (kinematic, constraints...)
			Uni2DEditorSpriteBuilderUtils.SetupRigidbodyFor2D( a_rSprite );
	
			// Apply delta to mesh colliders
			for( int iMeshIndex = 0, iMeshCount = rSpriteData.meshCollidersList.Count; iMeshIndex < iMeshCount; ++iMeshIndex )
			{
				Mesh rMesh = rSpriteData.meshCollidersList[ iMeshIndex ];
	
				Vector3[ ] oMeshVerticesArray = rMesh.vertices;
	
				for( int iVertexIndex = 0, iVertexCount = rMesh.vertexCount; iVertexIndex < iVertexCount; ++iVertexIndex )
				{
					Vector3 f3Vertex = oMeshVerticesArray[ iVertexIndex ];
	
					f3Vertex   -= f3ScaledDeltaPivot;
					f3Vertex.x *= fScalingDelta;
					f3Vertex.y *= fScalingDelta;
					f3Vertex.z  = Mathf.Sign( f3Vertex.z ) * rSpriteSettings.extrusionDepth * 0.5f;
	
					oMeshVerticesArray[ iVertexIndex ] = f3Vertex;
				}
	
				// Must set array again ("vertices" getter gives a copy)
				rMesh.vertices = oMeshVerticesArray;

				// Force mesh collider update by setting it null then reassigning it
				MeshCollider rMeshCollider = rSpriteData.meshColliderComponentsList[ iMeshIndex ];
				rMeshCollider.sharedMesh   = null;
				rMeshCollider.sharedMesh   = rMesh;
			}
	
			// Apply delta to sprite mesh
			Mesh rSpriteMesh = rSpriteData.renderMesh;
			Vector3[ ] oSpriteQuadMeshVerticesArray = rSpriteMesh.vertices;
	
			for( int iVertexIndex = 0, iVertexCount = rSpriteMesh.vertexCount; iVertexIndex < iVertexCount; ++iVertexIndex )
			{
				Vector3 f3Vertex = oSpriteQuadMeshVerticesArray[ iVertexIndex ];
	
				f3Vertex   -= f3ScaledDeltaPivot;
				f3Vertex.x *= fScalingDelta;
				f3Vertex.y *= fScalingDelta;
	
				oSpriteQuadMeshVerticesArray[ iVertexIndex ] = f3Vertex;
			}

			// Must set array again ("vertices" getter gives a copy)
			rSpriteMesh.vertices = oSpriteQuadMeshVerticesArray;

			// Recalc mesh bounds
			rSpriteMesh.RecalculateBounds( );
	
			// Update vertex color
			Uni2DSpriteUtils.UpdateMeshVertexColor( rSpriteMesh, rSpriteSettings.vertexColor );
	
			// Update UV
			// Check first the atlas is still valid
			Uni2DTextureAtlas rTextureAtlas = rSpriteSettings.atlas;
			Material rSpriteMaterial;
			if( rSpriteSettings.ShouldUseAtlas( ) )
			{
				// Update atlas generation ID
				a_rSprite.atlasGenerationID = rTextureAtlas.generationId;
				rSpriteMaterial = rTextureAtlas.GetMaterial( rSpriteTextureContainer.GUID );
			}
			else
			{
				a_rSprite.atlasGenerationID = "";
				rSpriteSettings.atlas = null;
				rTextureAtlas = null;
				rSpriteMaterial = rSpriteData.renderMeshMaterial;
			}

			// .. and rebuild UVs
			rSpriteMesh.uv = Uni2DSpriteUtils.BuildUVs( rSpriteTextureContainer, rSpriteData.renderMeshUVs, rTextureAtlas );

			// Update material
			Renderer rRenderer = a_rSprite.renderer;
			if( rRenderer != null )
			{
				rRenderer.sharedMaterial = rSpriteMaterial;
			}
			
			// If the pivot point has changed
			if( f2CurrentPivotPoint != f2NewPivotCoords )
			{
				// Compute the local position change
                Vector3 f3LocalPivotMovement = f3ScaledDeltaPivot;
				Vector3 f3PivotMovement      = f3ScaledDeltaPivot;
                
                Vector3 f3SpriteTransformLocalScale = a_rSprite.transform.localScale;
    
                f3PivotMovement.Scale( f3SpriteTransformLocalScale );
                f3PivotMovement = a_rSprite.transform.TransformDirection( f3PivotMovement );
                
                Transform rParentTransform = a_rSprite.transform.parent;
                if(rParentTransform != null)
                {
                    f3PivotMovement = rParentTransform.InverseTransformDirection(f3PivotMovement);
                }
                
                a_rSprite.transform.localPosition += f3PivotMovement;

                // Update bone roots local pos (if any)
                /*a_rSprite.RestorePosePosition( );
                foreach( Uni2DSmoothBindingBone rBone in a_rSprite.Bones )
                {
                    if( rBone.Parent == null )
                    {
                        Vector3 f3BonePivotMovement = rBone.transform.InverseTransformDirection( f3PivotMovement );
                        rBone.transform.localPosition -= f3BonePivotMovement;
                    }
                }*/
                
				Transform rMeshCollidersRootTransform = rSpriteData.meshCollidersRootGameObject != null
					? rSpriteData.meshCollidersRootGameObject.transform 
					: null;

                foreach(Transform rChild in a_rSprite.transform)
                {
					if( rChild != rMeshCollidersRootTransform )
					{
	                    rChild.localPosition -= f3LocalPivotMovement;
					}
				}
			}
			
			a_rSprite.UpdatePosing( );

			// Save new pivot coords
			rSpriteData.pivotCoords = f2NewPivotCoords;
			rSpriteData.scale       = rSpriteSettings.ScaleFactor;
		}
	}
	
	public static void UpdateSpriteSize( Uni2DSprite a_rSprite, float a_fWidth, float a_fHeight )
	{
		Uni2DEditorSpriteSettings rSpriteSettings = a_rSprite.SpriteSettings;
		Uni2DEditorSpriteData rSpriteData         = a_rSprite.SpriteData;

		float fExtrusionDepth    = rSpriteSettings.extrusionDepth;
		Vector2 f2OldPivotCoords = rSpriteData.pivotCoords;
		
		// The delta to apply
		Vector2 f2ScalingDelta = new Vector2( a_fWidth / rSpriteData.spriteWidth, a_fHeight / rSpriteData.spriteHeight );
		
		// Apply to the pivot point
		Vector2 f2NewPivotCoords = Vector2.Scale( f2OldPivotCoords, f2ScalingDelta );
		
		// Apply delta to mesh colliders
		List<Mesh> rMeshCollidersList = rSpriteData.meshCollidersList;
		if( rMeshCollidersList != null)
		{
			for( int iMeshIndex = 0, iMeshCount = rMeshCollidersList.Count; iMeshIndex < iMeshCount; ++iMeshIndex )
			{
				Mesh rMesh = rMeshCollidersList[ iMeshIndex ];
				
				Vector3[ ] oMeshVerticesArray = rMesh.vertices;
				for( int iVertexIndex = 0, iVertexCount = rMesh.vertexCount; iVertexIndex < iVertexCount; ++iVertexIndex )
				{
					Vector3 f3Vertex = oMeshVerticesArray[ iVertexIndex ];
					f3Vertex.x *= f2ScalingDelta.x;
					f3Vertex.y *= f2ScalingDelta.y;
					//f3Vertex -= f3ScaledDeltaPivot;
					f3Vertex.z = Mathf.Sign( f3Vertex.z ) * fExtrusionDepth * 0.5f;
					oMeshVerticesArray[ iVertexIndex ] = f3Vertex;
				}
	
				// Must set array again ("vertices" getter gives a copy)
				rMesh.vertices = oMeshVerticesArray;
				
				MeshCollider rMeshCollider = rSpriteData.meshColliderComponentsList[ iMeshIndex ];
				if(rMeshCollider != null)
				{
					rMeshCollider.sharedMesh = null;
					rMeshCollider.sharedMesh = rMesh;
				}
			}
		}
		
		// Apply delta to sprite quad mesh
		Mesh rSpriteMesh = rSpriteData.renderMesh;
		Vector3[ ] oSpriteQuadMeshVerticesArray = rSpriteMesh.vertices;
		for( int iVertexIndex = 0, iVertexCount = rSpriteMesh.vertexCount; iVertexIndex < iVertexCount; ++iVertexIndex )
		{
			Vector3 f3Vertex = oSpriteQuadMeshVerticesArray[ iVertexIndex ];
			f3Vertex.x *= f2ScalingDelta.x;
			f3Vertex.y *= f2ScalingDelta.y;
			//f3Vertex -= f3ScaledDeltaPivot;
			oSpriteQuadMeshVerticesArray[ iVertexIndex ] = f3Vertex;
		}

		rSpriteSettings.pivotCustomCoords = f2NewPivotCoords;
		
		// Must set array again ("vertices" getter gives a copy)
		rSpriteMesh.vertices = oSpriteQuadMeshVerticesArray;
	}

	////////////////////////////////////////////////////////////////////////////////
	
	public static void DisplayNoTextureWarning(GameObject a_rSpriteGameObject)
	{
		Debug.LogWarning(a_rSpriteGameObject.name + " has no texture.");
	}

	public static void DisplayNoTextureSoWeDontCreateWarning( GameObject a_rSpriteGameObject )
	{
		Debug.LogWarning( a_rSpriteGameObject.name + " has no texture. Settings have been reverted." );
	}

	public static void OnPrefabPostProcess(GameObject a_rPrefab)
	{
		// Get all the sprites in the prefab
		List<Uni2DSprite> oSpritesPrefab = new List<Uni2DSprite>();
		GetSpritesInResourceHierarchy(a_rPrefab.transform, ref oSpritesPrefab);
		
		// Check if the prefab is a sprite
		if(oSpritesPrefab.Count > 0)
		{	
			// Check if the prefab needs to be saved
			bool bNeedToBeSaved = false;
			foreach(Uni2DSprite rSprite in oSpritesPrefab)
			{
				if(rSprite.BeforePrefabPostProcess())
				{
					bNeedToBeSaved = true;
					break;
				}
			}
			
			// Save the sprite prefab
			if(bNeedToBeSaved)
			{
				// Instantiate the prefab
				GameObject rPrefabInstance = InstantiateSpritePrefab(a_rPrefab);
				
				ReplaceSpritePrefab(rPrefabInstance, a_rPrefab, ReplacePrefabOptions.Default);
				
				// Clear the prefab instance
				Editor.DestroyImmediate(rPrefabInstance);
			}
		}
	}
	
	// Save prefab
	private static GameObject InstantiateSpritePrefab(GameObject a_rPrefab)
	{			
		// Instantiate the prefab
		GameObject rPrefabInstance = MonoBehaviour.Instantiate(a_rPrefab) as GameObject;
		
		rPrefabInstance.name = rPrefabInstance.name.Replace( "(Clone)", "" );
		
		return rPrefabInstance;
	}
	
	// Save prefab
	private static GameObject InstantiateSpritePrefabWithoutConnection(GameObject a_rPrefab)
	{			
		// Instantiate the prefab
		GameObject rPrefabInstance = PrefabUtility.InstantiatePrefab(a_rPrefab) as GameObject;
		PrefabUtility.DisconnectPrefabInstance(rPrefabInstance);
		
		return rPrefabInstance;
	}
	
	// Save prefab
	private static void ReplaceSpritePrefab(GameObject a_rPrefabInstance, GameObject a_rPrefab, ReplacePrefabOptions a_eReplacePrefabOption = ReplacePrefabOptions.Default)
	{		
		Uni2DSprite[ ] rSpritePrefabInstances = a_rPrefabInstance.GetComponentsInChildren<Uni2DSprite>();
	
		// Save its resources
		string oPrefabPath;
		string oPrefabResourcesName;
		string oPrefabResourcesPath;
		string oPrefabResourcesPath_Absolute;
		GetPrefabResourcesDirectoryPaths(a_rPrefab, out oPrefabPath, out oPrefabResourcesName, out oPrefabResourcesPath, out oPrefabResourcesPath_Absolute);
	
		// Create the resources folder
		if(Directory.Exists(oPrefabResourcesPath_Absolute) == false)
		{
			try
			{
				string oPrefabResourcesFolderPathGUID = AssetDatabase.CreateFolder(oPrefabPath, oPrefabResourcesName);
				oPrefabResourcesPath = AssetDatabase.GUIDToAssetPath(oPrefabResourcesFolderPathGUID);
			}
			catch( Exception e )
			{
				Debug.LogError( "Uni2D can't create prefab folder: " + e.Message );
			}
		}

		int iSpriteCount = rSpritePrefabInstances.Length;
		string[ ] oTextureGUIDs = new string[ iSpriteCount ];
		string[ ] oAtlasGUIDs   = new string[ iSpriteCount ];
		Dictionary<string, int> oSubfolderNameOccurences = new Dictionary<string, int>();
		HashSet<string> oCreatedSubfolderNames = new HashSet<string>();
		for( int iSpriteIndex = 0; iSpriteIndex < iSpriteCount; ++iSpriteIndex )
		{
			Uni2DSprite rSpritePrefabInstance = rSpritePrefabInstances[ iSpriteIndex ];
			Uni2DEditorSpriteSettings rSpriteSettings = rSpritePrefabInstance.SpriteSettings;
			
			// Select a unique subfolder name
			string oGameObjectName = rSpritePrefabInstance.gameObject.name.Replace("(Clone)", "");
			string oSubfolderName = oGameObjectName;
			int iOccurenceCount = 0;
			if(oSubfolderNameOccurences.TryGetValue(oSubfolderName, out iOccurenceCount))
			{
				oSubfolderNameOccurences[oSubfolderName] = iOccurenceCount + 1;
				oSubfolderName += " " + iOccurenceCount;
			}
			else
			{
				oSubfolderNameOccurences.Add(oSubfolderName, 1);
			}
			
			// Save the created subfolder name in order to be able to clean the useless subfolders
			oCreatedSubfolderNames.Add(oSubfolderName);
			
			rSpritePrefabInstance.SaveSpriteAsPartOfAPrefab(oPrefabResourcesPath, oPrefabResourcesPath_Absolute, oSubfolderName);

			// Save texture/atlas dependencies
			oTextureGUIDs[ iSpriteIndex ] = rSpriteSettings.textureContainer.GUID;
			oAtlasGUIDs[ iSpriteIndex   ] = rSpriteSettings.atlas != null
				? Uni2DEditorUtils.GetUnityAssetGUID( rSpriteSettings.atlas )
				: null;
		}
		
		// Remove all the unused assets in the folder
		string[] oFiles = Directory.GetFiles(oPrefabResourcesPath_Absolute);
		string[] oDirectory = Directory.GetDirectories(oPrefabResourcesPath_Absolute);
		List<string> oAssets = new List<string>();
		oAssets.AddRange(oFiles);
		foreach(string oDirectoryPath in oDirectory)
		{
			string oDirectoryName = oDirectoryPath.Replace(oPrefabResourcesPath_Absolute + "/", "");
			
			// Don't remove the subdirectory we created
			if(oCreatedSubfolderNames.Contains(oDirectoryName) == false)
			{
				oAssets.Add(oDirectoryPath);
			}
		}
		foreach(string oAssetPath in oAssets)
		{
			string oAssetName = oAssetPath.Replace(oPrefabResourcesPath_Absolute + "/", "");
			AssetDatabase.DeleteAsset(oPrefabResourcesPath + "/" + oAssetName);
		}
		
		// Replace prefab
		GameObject rNewPrefab = PrefabUtility.ReplacePrefab(a_rPrefabInstance, a_rPrefab, a_eReplacePrefabOption);
		
		// Update asset table with sprite prefabs dependencies
		string rPrefabGUID = Uni2DEditorUtils.GetUnityAssetGUID( rNewPrefab );
		Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
		
		for( int iSpriteIndex = 0; iSpriteIndex < iSpriteCount; ++iSpriteIndex )
		{
			rAssetTable.AddSpritePrefabUsingTexture( rPrefabGUID, oTextureGUIDs[ iSpriteIndex ] );

			if( !string.IsNullOrEmpty( oAtlasGUIDs[ iSpriteIndex ] ) )
			{
				rAssetTable.AddSpritePrefabUsingAtlas( rPrefabGUID, oAtlasGUIDs[ iSpriteIndex ] );
			}
		}
		rAssetTable.Save( );
		
		AssetDatabase.SaveAssets();
	}
	
	// Get resource directory path local
	public static string GetPrefabResourcesDirectoryPathLocal(GameObject a_rPrefab)
	{
		// Ensure we have the root
		a_rPrefab = PrefabUtility.FindPrefabRoot(a_rPrefab);
		
		string oPrefabPath;
		string oPrefabResourcesName;
		string oPrefabResourcesPath;
		string oPrefabResourcesPathAbsolute;
		GetPrefabResourcesDirectoryPaths(a_rPrefab, out oPrefabPath, out oPrefabResourcesName, out oPrefabResourcesPath, out oPrefabResourcesPathAbsolute);
		
		return oPrefabResourcesPath;
	}
	
	// Get resource directory path absoluter
	private static void GetPrefabResourcesDirectoryPaths(GameObject a_rPrefab, out string a_rPrefabPath, out string a_rPrefabResourcesName, 
		out string a_rPrefabResourcesPath, out string a_rPrefabResourcesPathAbsolute)
	{
		a_rPrefabPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(a_rPrefab));
		a_rPrefabResourcesName = a_rPrefab.name + "_Resources";
		a_rPrefabResourcesPath = a_rPrefabPath + "/" + a_rPrefabResourcesName;
		a_rPrefabResourcesPathAbsolute = Application.dataPath.Replace("Assets", "") + a_rPrefabResourcesPath;
	}
	
	// Get all the sprite mesh components in a resource hierarchy
	public static void GetSpritesInResourceHierarchy(Transform a_rRoot, ref List<Uni2DSprite> a_rSprites)
	{
		a_rSprites.AddRange(a_rRoot.GetComponents<Uni2DSprite>());
		
		// Recursive call
		foreach(Transform rChild in a_rRoot)
		{	
			GetSpritesInResourceHierarchy(rChild, ref a_rSprites);
		}
	}
	
	// Is there at least one sprite in a resource hierarchy
	public static bool IsThereAtLeastOneSpriteContainingTheTextureInResourceHierarchy(Transform a_rRoot, Texture2D a_rTexture)
	{
		Uni2DSprite rSprite = a_rRoot.GetComponent<Uni2DSprite>();

		if(rSprite != null)
		{
			if(rSprite.SpriteSettings.textureContainer.Texture == a_rTexture)
			{
				return true;
			}
		}
		
		// Recursive call
		foreach(Transform rChild in a_rRoot)
		{	
			if(IsThereAtLeastOneSpriteContainingTheTextureInResourceHierarchy(rChild, a_rTexture))
			{
				return true;
			}
		}
		
		return false;
	}
	
	// Is there at least one sprite in a resource hierarchy
	public static bool IsThereAtLeastOneSpriteContainingTheAtlasInResourceHierarchy(Transform a_rRoot, Uni2DTextureAtlas a_rTextureAtlas)
	{
		Uni2DSprite rSprite = a_rRoot.GetComponent<Uni2DSprite>();
		if(rSprite != null)
		{
			if(rSprite.SpriteSettings.atlas == a_rTextureAtlas)
			{
				return true;
			}
		}
		
		// Recursive call
		foreach(Transform rChild in a_rRoot)
		{	
			if(IsThereAtLeastOneSpriteContainingTheAtlasInResourceHierarchy(rChild, a_rTextureAtlas))
			{
				return true;
			}
		}
		
		return false;
	}
	
	// Setup rigidbody 2d
	private static void SetupRigidbodyFor2D( Uni2DSprite a_rSprite, Uni2DEditorSpriteSettings a_rNewSettings = null )
	{
		Uni2DEditorSpriteSettings rCurrentSettings = a_rNewSettings == null ? a_rSprite.SpriteSettings : a_rNewSettings;
		Rigidbody rSpriteRigidbody = a_rSprite.GetComponent<Rigidbody>( );

		// Setup the rigidbody
		if( rCurrentSettings.physicsMode == Uni2DSprite.PhysicsMode.Dynamic )
		{
			// Add the rigidbody component
			if( rSpriteRigidbody == null )
			{
				rSpriteRigidbody = a_rSprite.gameObject.AddComponent<Rigidbody>( );
			}

			rSpriteRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
			rSpriteRigidbody.isKinematic = rCurrentSettings.isKinematic;
		}
		else if( rSpriteRigidbody != null ) // a rigidbody is not needed in the other modes
		{
			MonoBehaviour.DestroyImmediate( rSpriteRigidbody );
		}
	}
	
	// Texture processing Begin
	public static TextureImporterSettings TextureProcessingBegin(Texture2D a_rTexture)
	{
		TextureImporter rTextureImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(a_rTexture)) as TextureImporter;
		if(rTextureImporter == null)
		{
			return null;
		}
		else
		{
			return TextureProcessingBegin(rTextureImporter);
		}
	}
	
	// Texture processing End
	public static void TextureProcessingEnd(Texture2D a_rTexture, TextureImporterSettings a_rTextureImporterSettings)
	{
		TextureImporter rTextureImporer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(a_rTexture)) as TextureImporter;
		TextureProcessingEnd(rTextureImporer, a_rTextureImporterSettings);
	}
	
	// Texture processing Begin
	public static TextureImporterSettings TextureProcessingBegin(TextureImporter a_rTextureImporter)
	{
		Uni2DAssetPostprocessor.Enabled = false;
		
		// If it's the first time Uni2d use this texture
		// Set the default texture importer settings
		Texture2D rTexture = AssetDatabase.LoadAssetAtPath(a_rTextureImporter.assetPath, typeof(Texture2D)) as Texture2D;
		if(Uni2DEditorUtils.ItIsTheFirstTimeWeUseTheTexture(rTexture))
		{
			Uni2DEditorUtils.MarkAsSourceTexture(rTexture);
			Uni2DEditorUtils.GenerateTextureImportGUID(rTexture);
			SetDefaultTextureImporterSettings( rTexture, false );
		}
		
		TextureImporterSettings rTextureImporterSettings = new TextureImporterSettings();
		a_rTextureImporter.ReadTextureSettings(rTextureImporterSettings);
			
		// Reimport texture as readable and at original size
		SetDefaultTextureImporterSettings(a_rTextureImporter);
		
		if(AssetDatabase.WriteImportSettingsIfDirty(a_rTextureImporter.assetPath))
		{
			AssetDatabase.ImportAsset(a_rTextureImporter.assetPath, ImportAssetOptions.ForceSynchronousImport );
			AssetDatabase.Refresh();
		}

		return rTextureImporterSettings;
	}
	
	// Texture processing End
	public static void TextureProcessingEnd(TextureImporter a_rTextureImporter, TextureImporterSettings a_rTextureImporterSettings)
	{
		if(a_rTextureImporter != null)
		{
			a_rTextureImporter.SetTextureSettings(a_rTextureImporterSettings);
			
			if(AssetDatabase.WriteImportSettingsIfDirty(a_rTextureImporter.assetPath))
			{
				AssetDatabase.ImportAsset(a_rTextureImporter.assetPath, ImportAssetOptions.ForceSynchronousImport );
				AssetDatabase.Refresh();
			}
		}

		Uni2DAssetPostprocessor.Enabled = true;
	}
	
	// Set default texture importer settings
	public static void SetDefaultTextureImporterSettings(Texture2D a_rTexture, bool a_bIsReadable = true )
	{
		TextureImporter rTextureImporter = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(a_rTexture)) as TextureImporter;
		if( Uni2DEditorUtils.IsMarkedAsTextureAtlas( a_rTexture ) )
		{
			Uni2DTextureAtlas.SetDefaultAtlasTextureImportSettings( rTextureImporter );
		}
		else
		{
			SetDefaultTextureImporterSettings(rTextureImporter, a_bIsReadable );
		}
	}
	
	// Texture processing End
	public static void SetDefaultTextureImporterSettings( TextureImporter a_rTextureImporter, bool a_bIsReadable = true )
	{
		//Debug.Log("Set Default Importer Settings");
		a_rTextureImporter.textureType    = TextureImporterType.Advanced;
		a_rTextureImporter.maxTextureSize = 4096;
		a_rTextureImporter.textureFormat  = TextureImporterFormat.AutomaticTruecolor;
		a_rTextureImporter.npotScale      = TextureImporterNPOTScale.None;
		a_rTextureImporter.isReadable     = a_bIsReadable;
		a_rTextureImporter.mipmapEnabled  = false;
		a_rTextureImporter.wrapMode       = TextureWrapMode.Clamp;
	}
	
	// Processing begin for multiple textures
	public static List<Uni2DTextureImporterSettingsPair> TexturesProcessingBegin( IEnumerable<Texture2D> a_rTexturesEnumerable )
	{
		// Prepare textures for processing
		HashSet<Texture> oAlreadyPreparedTextures = new HashSet<Texture>();
		List<Uni2DTextureImporterSettingsPair> oTextureImportersSettingPairs = new List<Uni2DTextureImporterSettingsPair>();
		foreach(Texture2D rTexture in a_rTexturesEnumerable)
		{
			// If we haven't yet prepare this sprite texture
			if(oAlreadyPreparedTextures.Contains(rTexture) == false)
			{
				oAlreadyPreparedTextures.Add(rTexture);
				TextureImporterSettings rTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTexture);
				oTextureImportersSettingPairs.Add(new Uni2DTextureImporterSettingsPair(rTexture, rTextureImporterSettings));
			}
		}

		return oTextureImportersSettingPairs;
	}
	
	// Processing end for multiple textures
	public static void TexturesProcessingEnd(List<Uni2DTextureImporterSettingsPair> a_rTextureImporterSettingsPairs)
	{
		// Restore textures settings
		foreach(Uni2DTextureImporterSettingsPair rTextureImporterSettingsPair in a_rTextureImporterSettingsPairs)
		{
			Uni2DAssetPostprocessor.Enabled = false;
			Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporterSettingsPair.Key, rTextureImporterSettingsPair.Value);
		}
		Uni2DAssetPostprocessor.Enabled = true;
	}
	
	//----------------
	// Update sprite
	//----------------

	// Update the sprite in current scene and resources accordingly to texture change
	public static void UpdateSpriteInCurrentSceneAndResourcesAccordinglyToTextureChange(Texture2D a_rTexture, string a_oNewTextureImportGUID)
	{
		string oTexturePath = AssetDatabase.GetAssetPath(a_rTexture);
		TextureImporter rTextureImporter = TextureImporter.GetAtPath(oTexturePath) as TextureImporter;
		if(rTextureImporter != null)
		{	
			TextureImporterSettings oTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTextureImporter);
	
			Uni2DEditorSpriteBuilderUtils.DoUpdateAllResourcesSpritesAccordinglyToTextureChange(a_rTexture, a_oNewTextureImportGUID);
			Uni2DEditorSpriteBuilderUtils.DoUpdateAllSceneSpritesAccordinglyToTextureChange(a_rTexture, a_oNewTextureImportGUID);
			
			Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporter, oTextureImporterSettings);
	
			EditorUtility.UnloadUnusedAssets( );
		}
	}
	
	// Do Update all scene sprites accordingly to texture change for a texture change
	public static void DoUpdateAllSceneSpritesAccordinglyToTextureChange( Texture2D a_rTexture, string a_oNewTextureImportGUID )
	{	
		// Loop through all the scene sprites
		foreach( Uni2DSprite rSprite in MonoBehaviour.FindObjectsOfType( typeof( Uni2DSprite ) ) )
		{
			if( rSprite.SpriteSettings.textureContainer.Texture == a_rTexture)
			{
				rSprite.UpdateAccordinglyToTextureChange( a_oNewTextureImportGUID );
			}
		}
	}
	
	// Do Update all scene sprites accordingly to texture change for a texture change
	private static void DoUpdateAllResourcesSpritesAccordinglyToTextureChange(Texture2D a_rTexture, string a_oNewTextureImportGUID)
	{	
		// Loop through all the prefab containing at least a sprite
		List<GameObject> rGameObjectsList = Uni2DEditorUtils.GetProjectResources<GameObject>( );
		foreach( GameObject rGameObject in rGameObjectsList )
		{
			if( IsThereAtLeastOneSpriteContainingTheTextureInResourceHierarchy( rGameObject.transform, a_rTexture) )
			{
				DoUpdatePrefabContainingSpriteAccordinglyToTextureChange( rGameObject, a_rTexture, a_oNewTextureImportGUID );
			}
		}
	}
	
	// Do Update all scene prefab containing at least a sprite accordingly to texture change for a texture change
	private static void DoUpdatePrefabContainingSpriteAccordinglyToTextureChange(GameObject a_rPrefab, Texture2D a_rTexture, string a_oNewTextureImportGUID)
	{		
		DoUpdatePrefabContainingSpriteAccordinglyToTextureChangeRecursively(a_rPrefab.transform, a_rTexture, a_oNewTextureImportGUID);
	}
	
	// Do Update all scene prefab containing at least a sprite accordingly to texture change for a texture change
	private static void DoUpdatePrefabContainingSpriteAccordinglyToTextureChangeRecursively( Transform a_rRoot, Texture2D a_rTexture, string a_oNewTextureImportGUID )
	{	
		// loop through the sprite containing the changed texture
		foreach( Uni2DSprite rSpritePrefabInstance in a_rRoot.GetComponents<Uni2DSprite>( ) )
		{
			if( rSpritePrefabInstance.SpriteSettings.textureContainer.Texture == a_rTexture )
			{
				rSpritePrefabInstance.UpdateAccordinglyToTextureChange( a_oNewTextureImportGUID );
			}
		}
		
		// Recursive call
		foreach( Transform rChild in a_rRoot )
		{	
			DoUpdatePrefabContainingSpriteAccordinglyToTextureChangeRecursively( rChild, a_rTexture, a_oNewTextureImportGUID );
		}
	}

	//----------------
	// Update Uvs
	//----------------
	
	// Update the sprite in current scene and resources accordingly to texture change
	public static void UpdateSpriteInCurrentSceneAndResourcesAccordinglyToAtlasChange(Uni2DTextureAtlas a_rAtlas)
	{
		Uni2DEditorSpriteBuilderUtils.DoUpdateAllResourcesSpritesAccordinglyToAtlasChange(a_rAtlas);
		Uni2DEditorSpriteBuilderUtils.DoUpdateAllSceneSpritesAccordinglyToAtlasChange(a_rAtlas);
	}
	
	// Do Update all scene sprites accordingly to texture change for a texture change
	private static void DoUpdateAllSceneSpritesAccordinglyToAtlasChange(Uni2DTextureAtlas a_rAtlas)
	{	
		// Loop through all the scene sprites
		foreach(Uni2DSprite rSprite in MonoBehaviour.FindObjectsOfType(typeof(Uni2DSprite)))
		{
			if(rSprite.SpriteSettings.atlas == a_rAtlas)
			{
				rSprite.UpdateUvs();
			}
		}
	}
	
	// Do Update all scene sprites accordingly to texture change for a texture change
	private static void DoUpdateAllResourcesSpritesAccordinglyToAtlasChange( Uni2DTextureAtlas a_rAtlas )
	{
		Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;

		string rAtlasGUID = Uni2DEditorUtils.GetUnityAssetGUID( a_rAtlas );
		string[ ] rSpritePrefabGUIDs = rAssetTable.GetSpritePrefabGUIDsUsingThisAtlas( rAtlasGUID );

		foreach( string rSpritePrefabGUID in rSpritePrefabGUIDs )
		{
			GameObject rPrefab = Uni2DEditorUtils.GetAssetFromUnityGUID<GameObject>( rSpritePrefabGUID );
			
			// Don't trust the table to be completly up to date
			if(rPrefab != null)
			{
				Uni2DSprite[ ] rSpritePrefabComponents = rPrefab.GetComponentsInChildren<Uni2DSprite>( true );
				foreach( Uni2DSprite rSpritePrefabComponent in rSpritePrefabComponents )
				{
					Uni2DEditorSpriteSettings rSpriteSettings = rSpritePrefabComponent.SpriteSettings;
					Uni2DTextureAtlas rAtlas = rSpriteSettings.atlas;
					string rSpritePrefabAtlasGUID = rAtlas != null ? Uni2DEditorUtils.GetUnityAssetGUID( rAtlas ) : null;
	
					if( !string.IsNullOrEmpty( rSpritePrefabAtlasGUID ) && rSpritePrefabGUID == rAtlasGUID )
					{
						rSpritePrefabComponent.Regenerate( true );
					}
					
					EditorUtility.UnloadUnusedAssets( );
				}
				
				rPrefab = null;
				rSpritePrefabComponents = null;
				EditorUtility.UnloadUnusedAssets( );
			}
			else
			{
				//Debug.Log("Sprite prefab " + rSpritePrefabGUID + " don't found.");
				rAssetTable.RemoveSpritePrefabUsingAtlas(rSpritePrefabGUID, rAtlasGUID);
			}
		}

		/*
		List<GameObject> rGameObjectsList = Uni2DEditorUtils.GetProjectResources<GameObject>( );

		// Loop through all the prefab containing at least a sprite
		foreach( GameObject rGameObject in rGameObjectsList )
		{
			// Is there at least one sprite containing the atlas in the prefab
			if( IsThereAtLeastOneSpriteContainingTheAtlasInResourceHierarchy( rGameObject.transform, a_rAtlas ) )
			{
				DoUpdatePrefabContainingSpriteAccordinglyToAtlasChange( rGameObject, a_rAtlas );
			}
		}
		*/
	}
	
	// Do Update all scene prefab containing at least a sprite accordingly to atlas change for a texture change
	private static void DoUpdatePrefabContainingSpriteAccordinglyToAtlasChange(GameObject a_rPrefab, Uni2DTextureAtlas a_rAtlas)
	{		
		DoUpdatePrefabContainingSpriteAccordinglyToAtlasChangeRecursively(a_rPrefab.transform, a_rAtlas);
	}
	
	// Do Update all scene prefab containing at least a sprite accordingly to atlas change for a texture change
	private static void DoUpdatePrefabContainingSpriteAccordinglyToAtlasChangeRecursively(Transform a_rRoot, Uni2DTextureAtlas a_rAtlas)
	{	
		// loop through the sprite containing the changed texture
		foreach(Uni2DSprite rSpritePrefabInstance in a_rRoot.GetComponents<Uni2DSprite>())
		{
			if(rSpritePrefabInstance.SpriteSettings.atlas == a_rAtlas)
			{
				rSpritePrefabInstance.UpdateUvs();
			}
		}
		
		// Recursive call
		foreach(Transform rChild in a_rRoot)
		{	
			DoUpdatePrefabContainingSpriteAccordinglyToAtlasChangeRecursively(rChild, a_rAtlas);
		}
	}

	//----------------
	// Update phyisc
	//----------------
	
	// Update a sprite in resource 
	public static void UpdateSpriteInResource(Uni2DSprite a_rSprite, bool a_bForce = false )
	{
		ms_bUndoEnabled = false;
		//Uni2DSprite.prefabUpdateInProgress = true;
		GameObject rPrefab = PrefabUtility.FindPrefabRoot(a_rSprite.gameObject);
			
		// Instantiate the prefab
		GameObject rPrefabInstance = InstantiateSpritePrefabWithoutConnection(rPrefab);
		Uni2DSprite[] oSpritesPrefabInstance = rPrefabInstance.GetComponentsInChildren<Uni2DSprite>();
		
		// Retrieve the instance of the sprite
		Uni2DSprite rSpriteInstance = null;
		foreach(Uni2DSprite rSpritePrefabInstance in oSpritesPrefabInstance)
		{
			if(PrefabUtility.GetPrefabParent(rSpritePrefabInstance) == a_rSprite)
			{
				rSpriteInstance = rSpritePrefabInstance;
			}
		}
		
		if(rSpriteInstance != null)
		{
			// Rebuild the sprite
			rSpriteInstance.Regenerate( a_bForce );
			
			// Replace prefab
			ReplaceSpritePrefab(rPrefabInstance, rPrefab);
		}
			
		// Clear the prefab instance
		Editor.DestroyImmediate(rPrefabInstance);
		//Uni2DSprite.prefabUpdateInProgress = false;
		
		ms_bUndoEnabled = true;
	}
	
	// Update a sprite in resource 
	public static void UpdateSpriteInResourceInABatch(Uni2DSprite a_rSprite)
	{
		ms_bUndoEnabled = false;
		
		//Uni2DSprite.prefabUpdateInProgress = true;
		GameObject rPrefab = PrefabUtility.FindPrefabRoot(a_rSprite.gameObject);
			
		// Instantiate the prefab
		GameObject rPrefabInstance = InstantiateSpritePrefabWithoutConnection(rPrefab);
		Uni2DSprite[] oSpritesPrefabInstance = rPrefabInstance.GetComponentsInChildren<Uni2DSprite>();
		
		// Retrieve the instance of the sprite
		Uni2DSprite rSpriteInstance = null;
		foreach(Uni2DSprite rSpritePrefabInstance in oSpritesPrefabInstance)
		{
			if(PrefabUtility.GetPrefabParent(rSpritePrefabInstance) == a_rSprite)
			{
				rSpriteInstance = rSpritePrefabInstance;
			}
		}
		
		if(rSpriteInstance != null)
		{
			// Rebuild the sprite
			rSpriteInstance.RebuildInABatch();
			
			// Replace prefab
			ReplaceSpritePrefab(rPrefabInstance, rPrefab);
		}
			
		// Clear the prefab instance
		Editor.DestroyImmediate(rPrefabInstance);
		//Uni2DSprite.prefabUpdateInProgress = false;
		
		AssetDatabase.SaveAssets( );
		AssetDatabase.Refresh( );
		
		ms_bUndoEnabled = true;
	}
}
#endif
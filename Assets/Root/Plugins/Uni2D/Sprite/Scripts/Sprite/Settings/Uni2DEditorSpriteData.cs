using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * Uni2DEditorSpriteData
 * 
 * Contains the data generated and used by an Uni2DSprite (mesh collider, sprite mesh, material, etc.)
 * 
 * Editor use only.
 * 
 */
[System.Serializable]
public class Uni2DEditorSpriteData
{
	///// Sprite data /////
	// Sprite width computed from sprite scale and texture width
	public float spriteWidth                             = 0.0f;
	// Sprite height computed from sprite scale and texture height
	public float spriteHeight                            = 0.0f;
	// The render mesh used by the game object
	public Mesh renderMesh                               = null;
	// The render mesh original vertex pos.
	public Vector3[ ] renderMeshVertices                 = null;
	// The render mesh normalized UVs
	public Vector2[ ] renderMeshUVs                      = null;
	// The material used by the render mesh
	public Material renderMeshMaterial                   = null;
	// The generated material
	// Used by default if no atlases and no shared material 
	public Material generatedMaterial                   = null;
	// The computed pivot used by the sprite and physic meshes
	public Vector2 pivotCoords                           = Vector2.zero;
	// The computed scale used by the sprite and physic meshes
	public float scale                                   = Uni2DSpriteUtils.mc_fSpriteUnitToUnity;

	///// Physic /////
	// Mesh collider triangle count (cache purpose only)
	public int colliderTriangleCount                     = 0;
	// (Compound mode only)
	// The game object parent of mesh collider game objects
	public GameObject meshCollidersRootGameObject        = null;
	// The mesh(es) built from the sprite texture
	public List<Mesh> meshCollidersList                  = new List<Mesh>( );
	// The mesh collider components
	public List<MeshCollider> meshColliderComponentsList = new List<MeshCollider>( );
	//public bool physicIsDirty                            = true;

	public Vector2 ScaledPivotCoords
	{
		get
		{
			return this.pivotCoords * this.scale;
		}
	}
#if UNITY_EDITOR

	// Default constructor
	public Uni2DEditorSpriteData( )
	{
		// Default values used
	}

	// Shallow copy constructor
	public Uni2DEditorSpriteData( Uni2DEditorSpriteData a_rSpriteData )
	{
		this.spriteWidth                 = a_rSpriteData.spriteWidth;
		this.spriteHeight                = a_rSpriteData.spriteHeight;
		this.renderMesh                  = a_rSpriteData.renderMesh;
		this.renderMeshMaterial          = a_rSpriteData.renderMeshMaterial;
		this.generatedMaterial			 = a_rSpriteData.generatedMaterial;
		this.renderMeshVertices          = a_rSpriteData.renderMeshVertices;
		this.renderMeshUVs               = a_rSpriteData.renderMeshUVs;
		this.pivotCoords                 = a_rSpriteData.pivotCoords;
		this.scale                       = a_rSpriteData.scale;
		this.colliderTriangleCount       = a_rSpriteData.colliderTriangleCount;
		this.meshCollidersRootGameObject = a_rSpriteData.meshCollidersRootGameObject;
		this.meshCollidersList           = new List<Mesh>( a_rSpriteData.meshCollidersList );
		this.meshColliderComponentsList  = new List<MeshCollider>( a_rSpriteData.meshColliderComponentsList );
		//this.physicIsDirty = a_rSpriteData.physicIsDirty;
	}

	// Returns true if the Uni2DSprite data have been generated
	// for a given physic mode
	public bool AreDataGenerated( Uni2DSprite.PhysicsMode a_ePhysicMode )
	{
		return this.renderMesh     != null
			&& this.renderMeshMaterial != null
			&& ( a_ePhysicMode           == Uni2DSprite.PhysicsMode.NoPhysics	// No physic == no mesh collider to generate
				 || ( this.meshCollidersList                            != null
					&& this.meshCollidersList.Contains( null )          == false
					&& this.meshColliderComponentsList                  != null
					&& this.meshColliderComponentsList.Contains( null ) == false
					)
				);
	}

	// Returns true if and only if a_rObject is not null and if data are equal
	public override bool Equals( System.Object a_rObject )
	{
		return this.Equals( a_rObject as Uni2DEditorSpriteData );
	}

	// Same as above
	public bool Equals( Uni2DEditorSpriteData a_rSpriteData )
	{
		return a_rSpriteData != null
			&& this.colliderTriangleCount       == a_rSpriteData.colliderTriangleCount			// Not sure if relevant...
			&& this.spriteWidth                 == a_rSpriteData.spriteWidth
			&& this.spriteHeight                == a_rSpriteData.spriteHeight
			&& this.renderMesh                  == a_rSpriteData.renderMesh
			&& this.renderMeshVertices.Equals( a_rSpriteData.renderMeshVertices )
			&& this.renderMeshUVs.Equals( a_rSpriteData.renderMeshUVs )
			&& this.renderMeshMaterial          == a_rSpriteData.renderMeshMaterial
			&& this.generatedMaterial          == a_rSpriteData.generatedMaterial
			&& this.pivotCoords                 == a_rSpriteData.pivotCoords
			&& this.scale                       == a_rSpriteData.scale
			&& this.meshCollidersRootGameObject == a_rSpriteData.meshCollidersRootGameObject
			&& this.meshCollidersList.Equals( a_rSpriteData.meshCollidersList )
			&& this.meshColliderComponentsList.Equals( a_rSpriteData.meshColliderComponentsList );
	}

	// Avoids warning
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	// Has shared resources
	public bool HasSharedResources()
	{
		return EditorUtility.IsPersistent( this.renderMesh )
			//|| EditorUtility.IsPersistent( this.spriteQuadMaterial )
			|| ( this.meshCollidersList != null && Uni2DEditorUtils.IsThereAtLeastOnePersistentObject( this.meshCollidersList ) );
	}

#endif
}

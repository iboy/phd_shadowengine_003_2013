#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Uni2DEditorResourceCopyUtils
{
	// Save Mesh resources
	public static void DuplicateResources( Uni2DSprite a_rSprite, bool a_bPrefab = false)
	{
		// Sprite mesh
		Mesh rSpriteMesh = a_rSprite.SpriteData.renderMesh;

		if( rSpriteMesh != null )
		{
			Mesh rDuplicatedMesh = Uni2DEditorResourceCopyUtils.DuplicateMesh( rSpriteMesh );
			
			MeshFilter rMeshFilterComponent = a_rSprite.GetComponent<MeshFilter>( );
			SkinnedMeshRenderer rSkinnedMeshRendererComponent = a_rSprite.GetComponent<SkinnedMeshRenderer>( );

			// Apply the new generated mesh to the mesh filter component
			// and save it as generated data
			if( rMeshFilterComponent != null )
			{
				rMeshFilterComponent.sharedMesh = rDuplicatedMesh;
				a_rSprite.SpriteData.renderMesh = rDuplicatedMesh;
			}

			if( rSkinnedMeshRendererComponent != null )
			{
				rSkinnedMeshRendererComponent.sharedMesh = rDuplicatedMesh;
			}
		}
		
		// Duplicate the generated material
		if(a_rSprite.SpriteData.generatedMaterial != null)
		{
			// Duplicate generated mesh
			a_rSprite.SpriteData.generatedMaterial = new Material(a_rSprite.SpriteData.generatedMaterial);
			
			// Change the render mesh material ref to the duplicate
			a_rSprite.SpriteData.renderMeshMaterial = a_rSprite.SpriteData.generatedMaterial;
			
			// Update the renderer material
			a_rSprite.renderer.sharedMaterial = a_rSprite.SpriteData.generatedMaterial;
		}
		
		// Mesh collider(s)
		List<Mesh> rMeshCollidersList                  = a_rSprite.SpriteData.meshCollidersList;
		List<MeshCollider> rMeshColliderComponentsList = a_rSprite.SpriteData.meshColliderComponentsList;
		for( int iMeshIndex = 0, iMeshCount = rMeshCollidersList.Count; iMeshIndex < iMeshCount; ++iMeshIndex )
		{
			Mesh rDuplicatedMeshCollider = Uni2DEditorResourceCopyUtils.DuplicateMesh( rMeshCollidersList[ iMeshIndex ] );
			MeshCollider rMeshCollider = rMeshColliderComponentsList[ iMeshIndex ];

			if( rMeshCollider != null )
			{
				rMeshCollider.sharedMesh = rDuplicatedMeshCollider;
			}
			rMeshCollidersList[ iMeshIndex ] = rDuplicatedMeshCollider;
		}

		EditorUtility.SetDirty( a_rSprite );
	}
	
	// Duplicate mesh 
	public static Mesh DuplicateMesh( Mesh a_rMeshToDuplicate )
	{
		Mesh rNewMesh = new Mesh( );

		if( a_rMeshToDuplicate == null )
		{
			return rNewMesh;
		}

		rNewMesh.vertices  = a_rMeshToDuplicate.vertices;
		rNewMesh.uv        = a_rMeshToDuplicate.uv;
		rNewMesh.uv2       = a_rMeshToDuplicate.uv2;
		rNewMesh.colors32  = a_rMeshToDuplicate.colors32;
		rNewMesh.tangents  = a_rMeshToDuplicate.tangents;
		rNewMesh.normals   = a_rMeshToDuplicate.normals;

		rNewMesh.boneWeights = a_rMeshToDuplicate.boneWeights;
		rNewMesh.bindposes   = a_rMeshToDuplicate.bindposes;
		rNewMesh.bounds      = a_rMeshToDuplicate.bounds;

		rNewMesh.name        = a_rMeshToDuplicate.name;
	
		// Iterate submeshes to copy their triangles
		int iSubMeshCount      = a_rMeshToDuplicate.subMeshCount;
		rNewMesh.subMeshCount  = iSubMeshCount;
		for( int iSubMeshIndex = 0; iSubMeshIndex < iSubMeshCount; ++iSubMeshIndex )
		{
			#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			rNewMesh.SetTriangles( a_rMeshToDuplicate.GetTriangles( iSubMeshIndex ), iSubMeshIndex );
			#else
			rNewMesh.SetIndices( a_rMeshToDuplicate.GetIndices( iSubMeshIndex ), a_rMeshToDuplicate.GetTopology( iSubMeshIndex ), iSubMeshIndex );
			#endif
		}

		rNewMesh.RecalculateBounds( );
		rNewMesh.Optimize( );
		
		return rNewMesh;
	}
}
#endif
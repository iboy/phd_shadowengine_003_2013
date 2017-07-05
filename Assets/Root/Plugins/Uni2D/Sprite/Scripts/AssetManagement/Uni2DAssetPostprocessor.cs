#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Uni2DAssetPostprocessor : AssetPostprocessor {

	private static bool ms_bEnabled = true;
	private static bool ms_bLocked  = false;

	private static HashSet<string> ms_oImportedTextureGUIDs       = new HashSet<string>( );
	private static HashSet<string> ms_oAtlasGUIDsToUpdate         = new HashSet<string>( );
	private static HashSet<string> ms_oAnimationClipGUIDsToUpdate = new HashSet<string>( );
	private static HashSet<string> ms_oSpritePrefabGUIDsToUpdate  = new HashSet<string>( );

	private static List<string> ms_oGameObjectGUIDsToPostProcess = new List<string>( );

	public static bool Enabled
	{
		get
		{
			return ms_bEnabled;
		}

		set
		{
			if( !ms_bLocked )
			{
				if( value )
				{
					ms_oImportedTextureGUIDs.Clear( );
				}
				
				ms_bEnabled = value;
			}
		}
	}

	public static bool IsLocked
	{
		get
		{
			return ms_bLocked;
		}
	}

	private static void LockTo( bool a_bValue )
	{
		ms_bLocked = false;
		Uni2DAssetPostprocessor.Enabled = a_bValue;
		ms_bLocked = true;
	}

	private static void Unlock( )
	{
		ms_bLocked = false;
	}

	private static void OnPostprocessAllAssets( string[ ] a_rImportedAssets, string[ ] a_rDeletedAssets, string[ ] a_rMovedAssets, string[ ] a_rMovedFromPath )
	{
		if( ms_bEnabled )
		{
			bool bUpdateAssets = false;
			bool bPostprocessPrefabs = false;
			bool bSaveTable = false;

			Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
			
			foreach( string rImportedAssetPath in a_rImportedAssets )
			{
				Texture2D rImportedTexture = (Texture2D) AssetDatabase.LoadAssetAtPath( rImportedAssetPath, typeof( Texture2D ) );
	
				if( rImportedTexture != null )
				{
					if(Uni2DEditorUtils.IsMarkedAsSourceTexture(rImportedTexture))
					{
						//Debug.Log ( "Imported " + rImportedAssetPath );
		
						string rImportedTextureGUID = AssetDatabase.AssetPathToGUID( rImportedAssetPath );
						ms_oImportedTextureGUIDs.Add( rImportedTextureGUID );
	
						Uni2DEditorUtils.GenerateTextureImportGUID(rImportedTexture);
						
						bUpdateAssets = true;
	
						rImportedTexture = null;
						EditorUtility.UnloadUnusedAssets( );
						continue;
					}
				}

				Uni2DTextureAtlas rImportedAtlas = (Uni2DTextureAtlas) AssetDatabase.LoadAssetAtPath( rImportedAssetPath, typeof( Uni2DTextureAtlas ) );
				if( rImportedAtlas != null )
				{
					//Debug.Log ( "Imported atlas " + rImportedAssetPath );
					
					bSaveTable = true;

					rAssetTable.AddAtlasPath( rImportedAssetPath, AssetDatabase.AssetPathToGUID( rImportedAssetPath ) );
					
					rImportedAtlas = null;
					EditorUtility.UnloadUnusedAssets( );
					continue;
				}
				
				Uni2DAnimationClip rImportedClip = (Uni2DAnimationClip) AssetDatabase.LoadAssetAtPath( rImportedAssetPath, typeof( Uni2DAnimationClip ) );
				if( rImportedClip != null )
				{
					//Debug.Log ( "Imported clip " + rImportedClip );

					bSaveTable = true;

					rAssetTable.AddClipPath( rImportedAssetPath, AssetDatabase.AssetPathToGUID( rImportedAssetPath ) );
					
					rImportedClip = null;
					EditorUtility.UnloadUnusedAssets( );
					continue;
				}

				GameObject rImportedGameObject = (GameObject) AssetDatabase.LoadAssetAtPath( rImportedAssetPath, typeof( GameObject ) );
				if( rImportedGameObject != null )
				{
					//Debug.Log ( "Imported game object " + rImportedAssetPath );
					ms_oGameObjectGUIDsToPostProcess.Add( AssetDatabase.AssetPathToGUID( rImportedAssetPath ) );
					
					bPostprocessPrefabs = true;
					
					rImportedGameObject = null;
					EditorUtility.UnloadUnusedAssets( );
				}
			}

			// Moved assets
			for( int iIndex = 0, iCount = a_rMovedAssets.Length; iIndex < iCount; ++iIndex )
			{
				//Debug.Log ( "Importing moved asset" );
				Uni2DTextureAtlas rMovedAtlas = (Uni2DTextureAtlas) AssetDatabase.LoadAssetAtPath( a_rMovedAssets[ iIndex ], typeof( Uni2DTextureAtlas ) );
				if( rMovedAtlas != null )
				{
					rAssetTable.RemoveAtlasFromPath( a_rMovedFromPath[ iIndex ], false );
					rAssetTable.AddAtlasPath( a_rMovedAssets[ iIndex ], AssetDatabase.AssetPathToGUID( a_rMovedAssets[ iIndex ] ) );
					
					bSaveTable = true;
					
					rMovedAtlas = null;
					EditorUtility.UnloadUnusedAssets( );
					continue;
				}

				Uni2DAnimationClip rMovedClip = (Uni2DAnimationClip) AssetDatabase.LoadAssetAtPath( a_rMovedAssets[ iIndex ], typeof( Uni2DAnimationClip ) );
				if( rMovedClip != null )
				{
					rAssetTable.RemoveClipFromPath( a_rMovedFromPath[ iIndex ], false );
					rAssetTable.AddClipPath( a_rMovedAssets[ iIndex ], AssetDatabase.AssetPathToGUID( a_rMovedAssets[ iIndex ] ) );
					
					bSaveTable = true;
					
					rMovedClip = null;
					EditorUtility.UnloadUnusedAssets( );
				}
			}

			// Deleted assets
			foreach( string rDeletedAsset in a_rDeletedAssets )
			{
				string[ ] rSpritePrefabGUIDs = rAssetTable.GetSpritePrefabGUIDsUsingThisAtlasPath( rDeletedAsset );

				if( rSpritePrefabGUIDs.Length > 0 )
				{
					bUpdateAssets = true;
					ms_oSpritePrefabGUIDsToUpdate.UnionWith( rSpritePrefabGUIDs );
				}
				
				/*
				// TODO: mettre des paths au lieu d'IDs
				string[ ] rClipGUIDs = rAssetTable.GetClipGUIDsUsingThisTexturePath( rDeletedAsset );
				if( rClipGUIDs.Length > 0 )
				{
					bUpdateAssets = true;
					ms_oAnimationClipGUIDsToUpdate.UnionWith( rClipGUIDs );
				}
				*/

				bSaveTable = rAssetTable.RemoveAtlasFromPath( rDeletedAsset, true ) || bSaveTable;
				bSaveTable = rAssetTable.RemoveClipFromPath( rDeletedAsset, true ) || bSaveTable;
			}

			if( bSaveTable )
			{
				rAssetTable.Save( );
			}

			if( bUpdateAssets )
			{
				ms_oAtlasGUIDsToUpdate.UnionWith( rAssetTable.GetAtlasGUIDsUsingTheseTextures( ms_oImportedTextureGUIDs ) );
				ms_oAnimationClipGUIDsToUpdate.UnionWith( rAssetTable.GetClipGUIDsUsingTheseTextures( ms_oImportedTextureGUIDs ) );
				
				ms_oSpritePrefabGUIDsToUpdate.UnionWith( rAssetTable.GetSpritePrefabGUIDsUsingTheseTextures( ms_oImportedTextureGUIDs ) );
				ms_oSpritePrefabGUIDsToUpdate.UnionWith( rAssetTable.GetSpritePrefabGUIDsUsingTheseAtlases( ms_oAtlasGUIDsToUpdate ) );

				EditorApplication.delayCall += UpdateUni2DAssets;
			}

			if( bPostprocessPrefabs )
			{
				EditorApplication.delayCall += OnSpritePrefabPostprocess;
			}
		}
	}

	private static void OnSpritePrefabPostprocess( )
	{
		EditorApplication.delayCall -= OnSpritePrefabPostprocess;

		try
		{
			Uni2DAssetPostprocessor.LockTo( false );

			foreach( string rGameObjectPrefabGUID in ms_oGameObjectGUIDsToPostProcess )
			{
				GameObject rGameObjectPrefab = Uni2DEditorUtils.GetAssetFromUnityGUID<GameObject>( rGameObjectPrefabGUID );
				if( rGameObjectPrefab != null )
				{
					//Debug.Log ( "Post processing game object prefab " + rGameObjectPrefabGUID );
					Uni2DEditorSpriteBuilderUtils.OnPrefabPostProcess( rGameObjectPrefab );
					
					rGameObjectPrefab = null;
					EditorUtility.UnloadUnusedAssets( );
				}
			}
		}
		finally
		{
			ms_oGameObjectGUIDsToPostProcess.Clear( );
			Uni2DAssetPostprocessor.Unlock( );
			Uni2DAssetPostprocessor.Enabled = true;
		}
	}

	private static void UpdateUni2DAssets( )
	{
		EditorApplication.delayCall -= UpdateUni2DAssets;

		Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;

		try
		{
			Uni2DAssetPostprocessor.LockTo( false );

			// Update animation clips first, because they can change the atlases
			foreach( string rAnimationClipGUID in ms_oAnimationClipGUIDsToUpdate )
			{
				Uni2DAnimationClip rAnimationClip = Uni2DEditorUtils.GetAssetFromUnityGUID<Uni2DAnimationClip>( rAnimationClipGUID );
				if( rAnimationClip != null )
				{
					//Debug.Log ( "Updating clip " + rAnimationClipGUID );
					rAnimationClip.OnTexturesChange( ms_oImportedTextureGUIDs );
	
					rAnimationClip = null;
					EditorUtility.UnloadUnusedAssets( );
				}
				else
				{
					// Clean asset table
					foreach( string rTextureGUID in ms_oImportedTextureGUIDs )
					{
						rAssetTable.RemoveClipUsingTexture( rAnimationClipGUID, rTextureGUID );
					}
				}
			}
	
			foreach( string rAtlasGUID in ms_oAtlasGUIDsToUpdate )
			{
				Uni2DTextureAtlas rAtlas = Uni2DEditorUtils.GetAssetFromUnityGUID<Uni2DTextureAtlas>( rAtlasGUID );
	
				if( rAtlas != null )
				{
					//Debug.Log( "Updating atlas " + rAtlasGUID );
					rAtlas.OnTextureChange( );
	
					rAtlas = null;
					EditorUtility.UnloadUnusedAssets( );
				}
				else
				{
					// Clean
					foreach( string rTextureGUID in ms_oImportedTextureGUIDs )
					{
						rAssetTable.RemoveAtlasUsingTexture( rAtlasGUID, rTextureGUID );
					}
				}
			}
	
			foreach( string rSpritePrefabGUID in ms_oSpritePrefabGUIDsToUpdate )
			{
				GameObject rSpritePrefab = Uni2DEditorUtils.GetAssetFromUnityGUID<GameObject>( rSpritePrefabGUID );
	
				if( rSpritePrefab != null )
				{
					//Debug.Log( "Updating sprite prefab " + rSpritePrefabGUID );
					foreach( Uni2DSprite rSpritePrefabComponent in rSpritePrefab.GetComponentsInChildren<Uni2DSprite>( true ) )
					{
						Uni2DEditorSpriteSettings rSpriteSettings = rSpritePrefabComponent.SpriteSettings;
						string rSpriteTextureGUID = rSpriteSettings.textureContainer.GUID;
						string rSpriteAtlasGUID = rSpriteSettings.atlas != null ? Uni2DEditorUtils.GetUnityAssetGUID( rSpriteSettings.atlas ) : null;

						if( ms_oImportedTextureGUIDs.Contains( rSpriteTextureGUID ) || ( !string.IsNullOrEmpty( rSpriteAtlasGUID ) && ms_oAtlasGUIDsToUpdate.Contains( rSpriteAtlasGUID ) ) )
						{
							rSpritePrefabComponent.Regenerate( true );
						}
						
						EditorUtility.UnloadUnusedAssets( );
					}

					rSpritePrefab = null;
					EditorUtility.UnloadUnusedAssets( );					
				}
				else
				{
					// Clean
					foreach( string rTextureGUID in ms_oImportedTextureGUIDs )
					{
						rAssetTable.RemoveSpritePrefabUsingTexture( rSpritePrefabGUID, rTextureGUID );
					}

					foreach( string rAtlasGUID in ms_oAtlasGUIDsToUpdate )
					{
						rAssetTable.RemoveSpritePrefabUsingAtlas( rSpritePrefabGUID, rAtlasGUID );
					}
				}
			}
		}
		finally
		{
	
			ms_oImportedTextureGUIDs.Clear( );
	
			ms_oAtlasGUIDsToUpdate.Clear( );
			ms_oAnimationClipGUIDsToUpdate.Clear( );
			ms_oSpritePrefabGUIDsToUpdate.Clear( );

			rAssetTable.Save( );
	
			Uni2DAssetPostprocessor.Unlock( );
			Uni2DAssetPostprocessor.Enabled = true;
		}
	}

	public static void ForceImportAssetIfLocked( string a_rAssetPath, ImportAssetOptions a_eImportOptions )
	{
		bool bWasLocked     = ms_bLocked;
		bool bPreviousValue = ms_bEnabled;

		if( bWasLocked )
		{
			Uni2DAssetPostprocessor.Unlock( );
		}

		Uni2DAssetPostprocessor.Enabled = true;
		{
			AssetDatabase.ImportAsset( a_rAssetPath, a_eImportOptions );
		}

		if( bWasLocked )
		{
			Uni2DAssetPostprocessor.LockTo( bPreviousValue );
		}
		else
		{
			Uni2DAssetPostprocessor.Enabled = bPreviousValue;
		}
	}
}
#endif
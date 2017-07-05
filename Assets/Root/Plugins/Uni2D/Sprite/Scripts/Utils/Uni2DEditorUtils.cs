#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditorInternal;

// Uni2D Utils
public static class Uni2DEditorUtils 
{	
	// The source texture label
	private const string mc_oSourceTextureLabel = "Uni2D_SourceTexture";
	
	// The atlas label for the texture import guid label
	private const string mc_oTextureAtlasLabel = "Uni2D_TextureAtlas";
	
	// Clear log
	public static void ClearLog()
	{
		/*Assembly assembly = Assembly.GetAssembly(typeof(Macros));
        Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod ("Clear");
        method.Invoke (new object (), null);*/
	}

	// Returns the asset GUID from its instance
	public static string GetUnityAssetGUID( UnityEngine.Object a_rAssetInstance )
	{
		if( a_rAssetInstance != null )
		{
			return AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( a_rAssetInstance.GetInstanceID( ) ) );
		}

		return null;
	}

	// Returns an asset instance from a GUID
	public static T GetAssetFromUnityGUID<T>( string a_rAssetGUID ) where T : UnityEngine.Object
	{
		if( a_rAssetGUID != null )
		{
			return AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath( a_rAssetGUID ), typeof( T ) ) as T;
		}
		return null;
	}

	public static string GetAssetNameFromUnityGUID( string a_rAssetGUID )
	{
		if( a_rAssetGUID != null )
		{
			string rPath = AssetDatabase.GUIDToAssetPath( a_rAssetGUID );
			if( rPath != null )
			{
				return Path.GetFileNameWithoutExtension( rPath );
			}
		}

		return null;
	}
	
	// Get selected asset folder path
	public static string GenerateNewPrefabLocalPath(string a_rPrefabName)
	{
		string oAssetPath = GetLocalSelectedAssetFolderPath() + a_rPrefabName + ".prefab";
		oAssetPath = AssetDatabase.GenerateUniqueAssetPath(oAssetPath);
		
		return oAssetPath;
	}
	
	// Get selected asset folder path
	public static string GetLocalSelectedAssetFolderPath()
	{
		return GetLocalAssetFolderPath(Selection.activeObject);
	}
	
	// Get selected asset folder path
	public static string GetGlobalSelectedAssetFolderPath()
	{
		return GetGlobalAssetFolderPath(Selection.activeObject);
	}
	
	// Get asset folder path
	public static string GetLocalAssetFolderPath(UnityEngine.Object a_rAsset)
	{
		return GlobalToLocalAssetPath(GetGlobalAssetFolderPath(a_rAsset));
	}
	
	// Get asset folder path
	public static string GetGlobalAssetFolderPath(UnityEngine.Object a_rAsset)
	{
		string oAssetPath = AssetDatabase.GetAssetPath(a_rAsset);
		string oAssetFolderPath = "";
		if (oAssetPath.Length > 0)
		{
			oAssetFolderPath = Application.dataPath + "/" + oAssetPath.Substring(7);
			oAssetFolderPath = oAssetFolderPath.Replace('\\', '/');
			if ((File.GetAttributes(oAssetFolderPath) & FileAttributes.Directory) != FileAttributes.Directory)
			{
				for (int i = oAssetFolderPath.Length - 1; i > 0; --i)
				{
					if (oAssetFolderPath[i] == '/')
					{
						oAssetFolderPath = oAssetFolderPath.Substring(0, i);
						break;
					}
				}
			}
			oAssetFolderPath += "/";
		}
		else
		{
			oAssetFolderPath = Application.dataPath + "/";
		}
		
		return oAssetFolderPath;
	}
	
	// Local to global asset path
	public static string LocalToGlobalAssetPath(string a_rLocalPath)
	{
		return Application.dataPath.Replace("Assets", "") + a_rLocalPath;	// TODO: Bug? (multiple occurrences of "Assets")
	}
	
	// Global to local asset path
	public static string GlobalToLocalAssetPath(string a_rGlobalPath)
	{
		return a_rGlobalPath.Replace(Application.dataPath.Replace("Assets", ""), "");	// TODO: same here
	}
	
	// Get global asset path
	public static string GetGlobalAssetPath(UnityEngine.Object a_rAsset)
	{
		return LocalToGlobalAssetPath(GetLocalAssetPath(a_rAsset));
	}
		
	// Get local asset path
	public static string GetLocalAssetPath(UnityEngine.Object a_rAsset)
	{
		return AssetDatabase.GetAssetPath(a_rAsset);
	}	
	
	// Fins the first texture atlas
	public static Uni2DTextureAtlas FindFirstTextureAtlas( string a_rTextureGUID )
	{
		Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
		Uni2DTextureAtlas rAtlas = null;
		bool bSaveTable = false;

		string[ ] rAtlasGUIDs = rAssetTable.GetAtlasGUIDsUsingThisTexture( a_rTextureGUID );

		foreach( string rAtlasGUID in rAtlasGUIDs )
		{
			rAtlas = Uni2DEditorUtils.GetAssetFromUnityGUID<Uni2DTextureAtlas>( rAtlasGUID );

			if( rAtlas != null && rAtlas.Contains( a_rTextureGUID ) )
			{
				//rAtlas = null;
				break;
			}
			else
			{
				rAssetTable.RemoveAtlasUsingTexture( rAtlasGUID, a_rTextureGUID );
				bSaveTable = true;
			}
		}

		if( bSaveTable )
		{
			rAssetTable.Save( );
		}

		return rAtlas;
	}
	
	// Mark as a source texture for uni2d use
	public static void MarkAsSourceTexture(Texture2D a_rTexture)
	{
		int iLabelIndex;
		string[] oLabels = AssetDatabase.GetLabels(a_rTexture);
		if(TryFindTextureSourceLabel(oLabels, out iLabelIndex))
		{
			oLabels[iLabelIndex] = mc_oSourceTextureLabel;
			AssetDatabase.SetLabels(a_rTexture, oLabels);
		}
		else
		{
			string[] oNewLabels = new string[oLabels.Length + 1];
			Array.Copy(oLabels, oNewLabels, oLabels.Length);
			oNewLabels[oNewLabels.Length - 1] = mc_oSourceTextureLabel;
			AssetDatabase.SetLabels(a_rTexture, oNewLabels);
		}

		AssetDatabase.Refresh();
	}
	
	// Is marked as a source texture for uni2D?
	public static bool IsMarkedAsSourceTexture(Texture2D a_rTexture)
	{
		int iLabelIndex;
		string[] oLabels = AssetDatabase.GetLabels(a_rTexture);
		return TryFindTextureSourceLabel(oLabels, out iLabelIndex);
	}
	
	// Generate Texture import GUID label
	// return true if there was no GUID on the texture
	public static string GenerateTextureImportGUID(Texture2D a_rTexture)
	{
		//Debug.Log("Generate texture import GUID");
	
		string oTextureImportGUID = Guid.NewGuid().ToString();
		
		Uni2DEditorAssetTable.Instance.SetTextureImportGUID(GetUnityAssetGUID(a_rTexture), oTextureImportGUID);
		
		return oTextureImportGUID;
	}
	
	// Get Texture import GUID label
	public static string GetTextureImportGUID(Texture2D a_rTexture)
	{
		return Uni2DEditorAssetTable.Instance.GetTextureImportGUID(GetUnityAssetGUID(a_rTexture));
	}
	
	// Generate Texture import GUID label
	private static bool TryFindTextureSourceLabel(string[] a_rLabels, out int a_iIndex)
	{
		a_iIndex = -1;
		int iIndex = 0;
		foreach(string rLabel in a_rLabels)
		{
			if(rLabel.Contains(mc_oSourceTextureLabel))
			{
				a_iIndex = iIndex;
				return true;
			}
			++iIndex;
		}
		return false;
	}
	
	// It'is the first time we use the texture?
	public static bool ItIsTheFirstTimeWeUseTheTexture(Texture2D a_rTexture)
	{
		if(a_rTexture != null)
		{
			string[] rLabels = AssetDatabase.GetLabels(a_rTexture);
			foreach(string rLabel in rLabels)
			{
				if(rLabel.Contains(mc_oSourceTextureLabel))
				{
					return false;
				}
			}
			return true;
		}
		else
		{
			return false;
		}
	}
	
	// Mark as atlas
	public static void MarkAsTextureAtlas(Texture2D a_rTexture)
	{
		int iLabelIndex;
		string[] oLabels = AssetDatabase.GetLabels(a_rTexture);
		if(TryFindTextureAtlasLabel(oLabels, out iLabelIndex))
		{
			oLabels[iLabelIndex] = mc_oTextureAtlasLabel;
			AssetDatabase.SetLabels(a_rTexture, oLabels);
		}
		else
		{
			string[] oNewLabels = new string[oLabels.Length + 1];
			Array.Copy(oLabels, oNewLabels, oLabels.Length);
			oNewLabels[oNewLabels.Length - 1] = mc_oTextureAtlasLabel;
			AssetDatabase.SetLabels(a_rTexture, oNewLabels);
		}
	}

	public static bool IsMarkedAsTextureAtlas( Texture2D a_rTexture )
	{
		int iLabelIndex;
		string[ ] oLabels = AssetDatabase.GetLabels( a_rTexture );
		return TryFindTextureAtlasLabel( oLabels, out iLabelIndex );
	}

	// Generate Texture import GUID label
	private static bool TryFindTextureAtlasLabel(string[] a_rLabels, out int a_iIndex)
	{
		a_iIndex = -1;
		int iIndex = 0;
		foreach(string rLabels in a_rLabels)
		{
			if(rLabels.Contains(mc_oTextureAtlasLabel))
			{
				a_iIndex = iIndex;
				return true;
			}
			iIndex++;
		}
		return false;
	}
	
	// Contains at least a sprite
	public static bool IsPrefabContainsAtLeastASprite(GameObject a_rPrefab)
	{
		return IsPrefabContainsAtLeastASprite(a_rPrefab.transform);
	}
	
	// Contains at least a sprite
	private static bool IsPrefabContainsAtLeastASprite(Transform a_rRoot)
	{
		Uni2DSprite rSprite = a_rRoot.GetComponent<Uni2DSprite>();
		if(rSprite != null)
		{
			return true;
		}
		
		// Recursive call
		foreach(Transform rChild in a_rRoot)
		{	
			if(IsPrefabContainsAtLeastASprite(rChild))
			{
				return true;
			}
		}
		
		return false;
	}
	
	// Ping a prefab or a visible parent in project view
	public static void PingPrefabInProjectView(GameObject a_rGameObject)
	{
		GameObject rGameObjectToPing = a_rGameObject;
		if(a_rGameObject.transform.parent != null && a_rGameObject.transform.parent.parent != null)
		{
			rGameObjectToPing = PrefabUtility.FindPrefabRoot(a_rGameObject);
		}
		
		EditorGUIUtility.PingObject(rGameObjectToPing);
	}

	// Generic project/custom resource getter
	public static List<T> GetProjectResources<T>( ) where T : UnityEngine.Object
	{
		List<T> oResourcesList = new List<T>( );

		// Iterate all assets in project
		foreach( string rAssetPath in AssetDatabase.GetAllAssetPaths( ) )
		{
			// Load asset
			T rPrefab = (T) AssetDatabase.LoadAssetAtPath( rAssetPath, typeof( T ) );

			// If not null...
			if( rPrefab != null )
			{
				oResourcesList.Add( rPrefab );
			}
		}

		return oResourcesList;
	}

	// Returns true if at least one given object is persistent
	// Returns false if all given objects are not persistent 
	public static bool IsThereAtLeastOnePersistentObject( IEnumerable a_rEnumerableObject )
	{
		foreach( UnityEngine.Object rObject in a_rEnumerableObject )
		{
			if( EditorUtility.IsPersistent( rObject ) )
			{
				return true;
			}
		}

		return false;
	}
}
#endif
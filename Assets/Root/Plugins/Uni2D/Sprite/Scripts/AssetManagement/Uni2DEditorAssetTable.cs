#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class Uni2DEditorAssetTable : ScriptableObject
{

	// 1 Key => 0..n value(s)
	[System.Serializable]
	private class GenericValueEntry<T>
	{
		public string key = null;
		public T genericValue = default( T );
		
		public GenericValueEntry( string a_rKey, T a_rValue )
		{
			key = a_rKey;
			genericValue = a_rValue;
		}
	}

	[System.Serializable]
	private class SimpleValueEntry : GenericValueEntry<string>
	{
		public SimpleValueEntry( string a_rKey, string a_rValue )
			:base( a_rKey, a_rValue )
		{
		}
	}

	[System.Serializable]
	private class MultiValueEntry : GenericValueEntry<List<string>>
	{
		public MultiValueEntry( string a_rKey, List<string> a_rValue )
			:base( a_rKey, a_rValue )
		{
			
		}
	}

	private const string mc_oDefaultAssetLocation = "Assets/Uni2D_AssetTable.asset";


	// Singleton ref.
	private static Uni2DEditorAssetTable ms_rInstance = null;

	[SerializeField]
	[HideInInspector]
	private List<SimpleValueEntry> m_oAtlasPathGUIDEntries = new List<SimpleValueEntry>( );

	[SerializeField]
	[HideInInspector]
	private List<SimpleValueEntry> m_oClipPathGUIDEntries = new List<SimpleValueEntry>( );
	
	[SerializeField]
	[HideInInspector]
	private List<SimpleValueEntry> m_oTextureImportGUIDEntries = new List<SimpleValueEntry>( );

	// Texture GUIDs (multi atlas values dict key list)
	[SerializeField]
	[HideInInspector]
	private List<MultiValueEntry> m_oTextureGUIDAtlasEntries = new List<MultiValueEntry>( );

	// Texture GUIDs (multi clip values dict key list)
	[SerializeField]
	[HideInInspector]
	private List<MultiValueEntry> m_oTextureGUIDClipEntries = new List<MultiValueEntry>( );

	[SerializeField]
	[HideInInspector]
	private List<MultiValueEntry> m_oTextureGUIDSpritePrefabEntries = new List<MultiValueEntry>( );

	[SerializeField]
	[HideInInspector]
	private List<MultiValueEntry> m_oAtlasGUIDSpritePrefabEntries = new List<MultiValueEntry>( );

	// Multi-value dictionary ( 1 tex. GUID => 0..n atlas GUIDs)
	private MultiValueDictionary<string, string> m_oTextureGUIDAtlasGUIDsMultiDict = null;

	// Multi-value dictionary ( 1 tex. GUID => 0..n clip GUIDs)
	private MultiValueDictionary<string, string> m_oTextureGUIDClipGUIDsMultiDict = null;

	// Multi-value dictionary ( 1 tex. GUID => 0..n sprite prefab GUIDs)
	private MultiValueDictionary<string, string> m_oTextureGUIDSpritePrefabGUIDsMultiDict = null;

	// Multi-value dictionary ( 1 atlas GUID => 0..n sprite prefab GUIDs)
	private MultiValueDictionary<string, string> m_oAtlasGUIDSpritePrefabGUIDsMultiDict = null;

	// Single-value dictionary
	private Dictionary<string, string> m_oAtlasPathGUIDDict = null;

	// Single-value dictionary
	private Dictionary<string, string> m_oClipPathGUIDDict = null;
	
	// Texture Import GUID
	private Dictionary<string, string> m_oTextureImportGUIDDict = null;

	// Singleton accessors
	public static Uni2DEditorAssetTable Instance
	{
		get
		{
			if( ms_rInstance == null )
			{
				// Try to load from database
				ms_rInstance = (Uni2DEditorAssetTable) AssetDatabase.LoadAssetAtPath( mc_oDefaultAssetLocation, typeof( Uni2DEditorAssetTable ) );
	
				if( ms_rInstance == null )
				{
					Uni2DEditorAssetTable[ ] rInstances = (Uni2DEditorAssetTable[ ]) Resources.FindObjectsOfTypeAll( typeof( Uni2DEditorAssetTable ) );
					
					ms_rInstance = rInstances != null && rInstances.Length > 0 ? rInstances[ 0 ] : null;
					
					// Create the instance
					if( ms_rInstance == null )
					{
						ms_rInstance = ScriptableObject.CreateInstance<Uni2DEditorAssetTable>( );
						ms_rInstance.name = "Uni2D_AssetTable";

						// Import it to database
						AssetDatabase.CreateAsset( ms_rInstance, mc_oDefaultAssetLocation );
						AssetDatabase.ImportAsset( mc_oDefaultAssetLocation );
						ms_rInstance = (Uni2DEditorAssetTable) AssetDatabase.LoadAssetAtPath( mc_oDefaultAssetLocation, typeof( Uni2DEditorAssetTable ) );
					}
					else if( EditorUtility.IsPersistent( ms_rInstance ) == false )
					{
						// Import it to database
						AssetDatabase.CreateAsset( ms_rInstance, mc_oDefaultAssetLocation );
						AssetDatabase.ImportAsset( mc_oDefaultAssetLocation );
						ms_rInstance = (Uni2DEditorAssetTable) AssetDatabase.LoadAssetAtPath( mc_oDefaultAssetLocation, typeof( Uni2DEditorAssetTable ) );
					}
				}
			}

			return ms_rInstance;
		}

		set
		{
			ms_rInstance = value;
		}
	}

	public static bool AssetTableCreated
	{
		get
		{
			if( ms_rInstance == null )
			{
				Uni2DEditorAssetTable rAssetTable = (Uni2DEditorAssetTable) AssetDatabase.LoadAssetAtPath( mc_oDefaultAssetLocation, typeof( Uni2DEditorAssetTable ) );

				if( rAssetTable == null )
				{
					Uni2DEditorAssetTable[ ] rInstances = (Uni2DEditorAssetTable[ ]) Resources.FindObjectsOfTypeAll( typeof( Uni2DEditorAssetTable ) );
					
					return rInstances != null && rInstances.Length > 0;
				}
				
				return true;
			}

			return false;
		}
	}

	void OnEnable( )
	{
		//Debug.Log( this.name + ": Deserializing..." );

		// Create the multi value dictionary
		m_oTextureGUIDAtlasGUIDsMultiDict        = this.BuildMultiDictFromEntries( m_oTextureGUIDAtlasEntries );
		m_oTextureGUIDClipGUIDsMultiDict         = this.BuildMultiDictFromEntries( m_oTextureGUIDClipEntries );	
		m_oTextureGUIDSpritePrefabGUIDsMultiDict = this.BuildMultiDictFromEntries( m_oTextureGUIDSpritePrefabEntries );
		m_oAtlasGUIDSpritePrefabGUIDsMultiDict   = this.BuildMultiDictFromEntries( m_oAtlasGUIDSpritePrefabEntries );

		m_oAtlasPathGUIDDict = this.BuildDictFromEntries( m_oAtlasPathGUIDEntries );
		m_oClipPathGUIDDict  = this.BuildDictFromEntries( m_oClipPathGUIDEntries );
		m_oTextureImportGUIDDict = BuildDictFromEntries( m_oTextureImportGUIDEntries );
	}

	void OnDisable( )
	{
		this.Save( );
	}

	public void Save( )
	{
		//Debug.Log( this.name + ": Serializing..." );
		
		// Save multi value dictionaries into a serializable form
		this.BuildEntriesFromMultiDict( m_oTextureGUIDAtlasGUIDsMultiDict, out m_oTextureGUIDAtlasEntries );
		this.BuildEntriesFromMultiDict( m_oTextureGUIDClipGUIDsMultiDict, out m_oTextureGUIDClipEntries );
		this.BuildEntriesFromMultiDict( m_oTextureGUIDSpritePrefabGUIDsMultiDict, out m_oTextureGUIDSpritePrefabEntries );
		this.BuildEntriesFromMultiDict( m_oAtlasGUIDSpritePrefabGUIDsMultiDict, out m_oAtlasGUIDSpritePrefabEntries );
		
		this.BuildEntriesFromDict( m_oAtlasPathGUIDDict, out m_oAtlasPathGUIDEntries );
		this.BuildEntriesFromDict( m_oClipPathGUIDDict, out m_oClipPathGUIDEntries );
		this.BuildEntriesFromDict( m_oTextureImportGUIDDict, out m_oTextureImportGUIDEntries );
		
		EditorUtility.SetDirty( this );
		AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( this.GetInstanceID( ) ) );		
	}

	public void Rebuild( )
	{
		//Debug.Log( "Rebuilding Uni2D Asset Table..." );

		m_oAtlasGUIDSpritePrefabGUIDsMultiDict.Clear( );
		m_oTextureGUIDAtlasGUIDsMultiDict.Clear( );
		m_oTextureGUIDClipGUIDsMultiDict.Clear( );
		m_oTextureGUIDSpritePrefabGUIDsMultiDict.Clear( );

		m_oAtlasPathGUIDDict.Clear( );
		m_oClipPathGUIDDict.Clear( );
		m_oTextureImportGUIDDict.Clear();

		// Iterate project's assets
		string[ ] rAssetPaths = AssetDatabase.GetAllAssetPaths( );

		int iAssetCount = rAssetPaths.Length;
		float fInvAssetCount = 1.0f / (float) iAssetCount;
		int iProcessedAssets = 0;
		try
		{
			foreach( string rPath in rAssetPaths )
			{
				EditorUtility.DisplayProgressBar( "Uni2D - Asset Table Rebuilding Progress",
					iProcessedAssets + " out of " + iAssetCount + " asset(s) processed...",
					fInvAssetCount * iProcessedAssets );

				Object rAssetObject = null;
	
				// Might be an atlas or a clip
				if( rPath.EndsWith( ".prefab" ) )
				{
					rAssetObject = AssetDatabase.LoadAssetAtPath( rPath, typeof( Uni2DTextureAtlas ) );
					if( rAssetObject != null )	// It's an atlas
					{
						Uni2DTextureAtlas rAtlasAsset = (Uni2DTextureAtlas) rAssetObject;
						string rAtlasGUID = AssetDatabase.AssetPathToGUID( rPath );

						foreach( string rTextureGUID in rAtlasAsset.GetTextureGUIDs( ) )
						{
							this.AddAtlasUsingTexture( rAtlasGUID, rTextureGUID );
						}

						m_oAtlasPathGUIDDict.Add( rPath, rAtlasGUID );
	
						rAtlasAsset = null;
						EditorUtility.UnloadUnusedAssets( );
					}
	
					rAssetObject = AssetDatabase.LoadAssetAtPath( rPath, typeof( Uni2DAnimationClip ) );
					if( rAssetObject != null )	// It's an animation clip
					{
						Uni2DAnimationClip rAnimationClipAsset = (Uni2DAnimationClip) rAssetObject;
						string rAnimationClipGUID = AssetDatabase.AssetPathToGUID( rPath );
						
						foreach( string rTextureGUID in rAnimationClipAsset.GetAllFramesTextureGUIDs( ) )
						{
							this.AddClipUsingTexture( rAnimationClipGUID, rTextureGUID );
						}

						m_oClipPathGUIDDict.Add( rPath, rAnimationClipGUID );
	
						rAnimationClipAsset = null;
						EditorUtility.UnloadUnusedAssets( );
					}
	
					rAssetObject = AssetDatabase.LoadAssetAtPath( rPath, typeof( GameObject ) );
					if( rAssetObject != null )	// It's a sprite prefab
					{
						GameObject rPrefabAsset = (GameObject) rAssetObject;
						string rPrefabGUID = AssetDatabase.AssetPathToGUID( rPath );
						Uni2DSprite[ ] rSpritePrefabComponents = rPrefabAsset.GetComponentsInChildren<Uni2DSprite>( true );
	
						foreach( Uni2DSprite rSpritePrefabComponent in rSpritePrefabComponents )
						{
							Uni2DEditorSpriteSettings rSpriteSettings = rSpritePrefabComponent.SpriteSettings;

							this.AddSpritePrefabUsingTexture( rPrefabGUID, rSpriteSettings.textureContainer.GUID );

							if( rSpriteSettings.atlas != null )
							{
								this.AddSpritePrefabUsingAtlas( rPrefabGUID, Uni2DEditorUtils.GetUnityAssetGUID( rSpriteSettings.atlas ) );
							}
						}
	
						rPrefabAsset = null;
						rSpritePrefabComponents = null;
						EditorUtility.UnloadUnusedAssets( );
					}
				}

				++iProcessedAssets;
			}
		}
		finally
		{
			this.Save( );

			EditorUtility.UnloadUnusedAssets( );

			EditorUtility.SetDirty( this );
			EditorUtility.ClearProgressBar( );
		}

		//Debug.Log( "Uni2D Asset Table Rebuild: Done." );
	}

	///// Single query /////
	public string GetTextureImportGUID(string a_oTextureGUID)
	{
		string oTextureImportGUID = "";
		if(m_oTextureImportGUIDDict.TryGetValue(a_oTextureGUID, out oTextureImportGUID))
		{
			return oTextureImportGUID;
		}
		else
		{
			return "";
		}
	}
	
	public void SetTextureImportGUID(string a_oTextureGUID, string a_oTextureImportGUID)
	{
		if(m_oTextureImportGUIDDict.ContainsKey(a_oTextureGUID))
		{
			m_oTextureImportGUIDDict[a_oTextureGUID] = a_oTextureImportGUID;
		}
		else
		{
			m_oTextureImportGUIDDict.Add(a_oTextureGUID, a_oTextureImportGUID);
		}
	}
	
	public string[ ] GetAtlasGUIDsUsingThisTexture( string a_rTextureGUID )
	{
		return QueryMultiValueDict( m_oTextureGUIDAtlasGUIDsMultiDict, a_rTextureGUID );
	}
	
	public string[ ] GetClipGUIDsUsingThisTexture( string a_rTextureGUID )
	{
		return QueryMultiValueDict( m_oTextureGUIDClipGUIDsMultiDict, a_rTextureGUID );
	}

	public string[ ] GetSpritePrefabGUIDsUsingThisTexture( string a_rTextureGUID )
	{
		return QueryMultiValueDict( m_oTextureGUIDSpritePrefabGUIDsMultiDict, a_rTextureGUID );
	}

	public Dictionary<string,string> GetClipNamesUsingThisTexture( string a_rTextureGUID )
	{
		return this.GetAssetNamesContainingThisTexture( m_oTextureGUIDClipGUIDsMultiDict, a_rTextureGUID );
	}
	
	public Dictionary<string,string> GetAtlasNamesUsingThisTexture( string a_rTextureGUID )
	{
		return this.GetAssetNamesContainingThisTexture( m_oTextureGUIDAtlasGUIDsMultiDict, a_rTextureGUID );
	}

	public string[ ] GetSpritePrefabGUIDsUsingThisAtlas( string a_rAtlasGUID )
	{
		return this.QueryMultiValueDict( m_oAtlasGUIDSpritePrefabGUIDsMultiDict, a_rAtlasGUID );
	}

	///// Multi query /////
	public string[ ] GetAtlasGUIDsUsingTheseTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		return MultiQueryMultiValueDict( m_oTextureGUIDAtlasGUIDsMultiDict, a_rTextureGUIDs );
	}
	
	public string[ ] GetClipGUIDsUsingTheseTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		return MultiQueryMultiValueDict( m_oTextureGUIDClipGUIDsMultiDict, a_rTextureGUIDs );
	}

	public string[ ] GetSpritePrefabGUIDsUsingTheseTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		return MultiQueryMultiValueDict( m_oTextureGUIDSpritePrefabGUIDsMultiDict, a_rTextureGUIDs );
	}

	public Dictionary<string,string> GetClipNamesUsingTheseTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		return this.GetAssetNamesContainingTheseTextures( m_oTextureGUIDClipGUIDsMultiDict, a_rTextureGUIDs );
	}

	public Dictionary<string,string> GetAtlasNamesUsingTheseTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		return this.GetAssetNamesContainingTheseTextures( m_oTextureGUIDAtlasGUIDsMultiDict, a_rTextureGUIDs );
	}

	public string[ ] GetSpritePrefabGUIDsUsingTheseAtlases( IEnumerable<string> a_rAtlasGUIDs )
	{
		return this.MultiQueryMultiValueDict( m_oAtlasGUIDSpritePrefabGUIDsMultiDict, a_rAtlasGUIDs );
	}

	public Dictionary<string, string> GetAllAtlasNames( )
	{
		Dictionary<string, string> oAtlasGUIDNameDict = new Dictionary<string, string>( m_oAtlasPathGUIDDict.Count );
		foreach( KeyValuePair<string, string> rKeyValuePair in m_oAtlasPathGUIDDict )
		{
			oAtlasGUIDNameDict.Add( rKeyValuePair.Value, Path.GetFileNameWithoutExtension( rKeyValuePair.Key ) );
		}
		return oAtlasGUIDNameDict;
	}

	public Dictionary<string, string> GetAllClipNames( )
	{
		Dictionary<string, string> oClipGUIDNameDict = new Dictionary<string, string>( m_oClipPathGUIDDict.Count );
		foreach( KeyValuePair<string, string> rKeyValuePair in m_oClipPathGUIDDict )
		{
			oClipGUIDNameDict.Add( rKeyValuePair.Value, Path.GetFileNameWithoutExtension( rKeyValuePair.Key ) );
		}
		return oClipGUIDNameDict;
	}

	///// Add methods /////
	public void AddAtlasUsingTexture( string a_rAtlasGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDAtlasGUIDsMultiDict.Add( a_rTextureGUID, a_rAtlasGUID );
		EditorUtility.SetDirty( this );
	}

	public void AddClipUsingTexture( string a_rAnimationClipGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDClipGUIDsMultiDict.Add( a_rTextureGUID, a_rAnimationClipGUID );
		EditorUtility.SetDirty( this );
	}

	public void AddSpritePrefabUsingTexture( string a_rSpritePrefabGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDSpritePrefabGUIDsMultiDict.Add( a_rTextureGUID, a_rSpritePrefabGUID );
		EditorUtility.SetDirty( this );
	}

	public void AddSpritePrefabUsingAtlas( string a_rSpritePrefabGUID, string a_rAtlasGUID )
	{
		m_oAtlasGUIDSpritePrefabGUIDsMultiDict.Add( a_rAtlasGUID, a_rSpritePrefabGUID );
		EditorUtility.SetDirty( this );
	}

	///// Remove methods /////
	public void RemoveAtlasUsingTexture( string a_rAtlasGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDAtlasGUIDsMultiDict.Remove( a_rTextureGUID, a_rAtlasGUID );
		EditorUtility.SetDirty( this );
	}

	public void RemoveClipUsingTexture( string a_rAnimationClipGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDClipGUIDsMultiDict.Remove( a_rTextureGUID, a_rAnimationClipGUID );
		EditorUtility.SetDirty( this );
	}

	public void RemoveSpritePrefabUsingTexture( string a_rSpritePrefabGUID, string a_rTextureGUID )
	{
		m_oTextureGUIDSpritePrefabGUIDsMultiDict.Remove( a_rTextureGUID, a_rSpritePrefabGUID );
		EditorUtility.SetDirty( this );
	}

	public void RemoveSpritePrefabUsingAtlas( string a_rSpritePrefabGUID, string a_rAtlasGUID )
	{
		m_oAtlasGUIDSpritePrefabGUIDsMultiDict.Remove( a_rAtlasGUID, a_rSpritePrefabGUID );
		EditorUtility.SetDirty( this );
	}

	public void AddAtlasPath( string a_rAtlasPath, string a_rAtlasGUID )
	{
		m_oAtlasPathGUIDDict[ a_rAtlasPath ] = a_rAtlasGUID;
		EditorUtility.SetDirty( this );
	}

	public void AddClipPath( string a_rClipPath, string a_rClipGUID )
	{
		m_oClipPathGUIDDict[ a_rClipPath ] = a_rClipGUID;
		EditorUtility.SetDirty( this );
	}

	public bool RemoveAtlasFromPath( string a_rAtlasPath, bool a_bRemoveDependencies )
	{
		string rAtlasGUID;
		if( m_oAtlasPathGUIDDict.TryGetValue( a_rAtlasPath, out rAtlasGUID ) )
		{
			if( a_bRemoveDependencies )
			{
				string[ ] rSpritePrefabGUIDs = this.GetSpritePrefabGUIDsUsingThisAtlas( rAtlasGUID );
				foreach( string rSpritePrefabGUID in rSpritePrefabGUIDs )
				{
					this.RemoveSpritePrefabUsingAtlas( rSpritePrefabGUID, rAtlasGUID );
				}

				m_oTextureGUIDAtlasGUIDsMultiDict.RemoveAll( rAtlasGUID );
			}

			m_oAtlasPathGUIDDict.Remove( a_rAtlasPath );
			EditorUtility.SetDirty( this );

			return true;
		}
		return false;
	}

	public bool RemoveClipFromPath( string a_rClipPath, bool a_bRemoveDependencies )
	{
		string rClipGUID;
		if( m_oClipPathGUIDDict.TryGetValue( a_rClipPath, out rClipGUID ) )
		{
			if( a_bRemoveDependencies )
			{
				m_oTextureGUIDClipGUIDsMultiDict.RemoveAll( rClipGUID );
			}
				
			m_oClipPathGUIDDict.Remove( a_rClipPath );
			
			EditorUtility.SetDirty( this );
		
			return true;
		}
		
		return false;
	}

	public string[ ] GetSpritePrefabGUIDsUsingThisAtlasPath( string a_rAtlasPath )
	{
		string oAtlasGUID;
		if( m_oAtlasPathGUIDDict.TryGetValue( a_rAtlasPath, out oAtlasGUID ) )
		{
			return this.GetSpritePrefabGUIDsUsingThisAtlas( oAtlasGUID );
		}
		return new string[ 0 ];
	}

	///// Internal logic methods /////

	private Dictionary<string,string> GetAssetNamesContainingThisTexture( MultiValueDictionary<string,string> a_rMultiDict, string a_rTextureGUID )
	{
		string[ ] rGUIDs = this.QueryMultiValueDict( a_rMultiDict, a_rTextureGUID );
		return this.GetAssetNamesFromGUIDsAndRemoveOutdatedOnes( a_rMultiDict, rGUIDs );
	}

	private Dictionary<string,string> GetAssetNamesContainingTheseTextures( MultiValueDictionary<string,string> a_rMultiDict, IEnumerable<string> a_rTextureGUIDs )
	{
		string[ ] rGUIDs = this.MultiQueryMultiValueDict( a_rMultiDict, a_rTextureGUIDs );
		return this.GetAssetNamesFromGUIDsAndRemoveOutdatedOnes( a_rMultiDict, rGUIDs );
	}

	private Dictionary<string,string> GetAssetNamesFromGUIDsAndRemoveOutdatedOnes( MultiValueDictionary<string,string> a_rMultiDict, IEnumerable<string> a_rGUIDs )
	{
		Dictionary<string,string> oGUIDNamesDict = new Dictionary<string,string>( );
		List<string> oOutdatedGUIDs = new List<string>( );

		foreach( string rGUID in a_rGUIDs )
		{
			string oName = Uni2DEditorUtils.GetAssetNameFromUnityGUID( rGUID );

			if( oName != null )
			{
				oGUIDNamesDict.Add( rGUID, oName );
			}
			else
			{
				// Name is null => asset doesn't exist anymore
				oOutdatedGUIDs.Add( rGUID );
			}
		}

		// Remove the outdated GUID from our multi-value dict
		foreach( string rOutdatedGUID in oOutdatedGUIDs )
		{
			MultiValueDictionary<string,string>.KeyCollection rKeys = a_rMultiDict.Keys;
			foreach( string rKey in rKeys )
			{
				if( a_rMultiDict.ContainsValue( rKey, rOutdatedGUID ) )
				{
					a_rMultiDict.Remove( rKey, rOutdatedGUID );
				}
			}
			// TODO: save?
		}
	
		return oGUIDNamesDict;
	}

	private Dictionary<string, string> BuildDictFromEntries( List<SimpleValueEntry> a_rEntries )
	{
		Dictionary<string, string> oDict = new Dictionary<string, string>( a_rEntries.Count );

		foreach( SimpleValueEntry rEntry in a_rEntries )
		{
			oDict.Add( rEntry.key, rEntry.genericValue );
		}
	
		return oDict;
	}

	private MultiValueDictionary<string,string> BuildMultiDictFromEntries( List<MultiValueEntry> a_rEntries )
	{
		MultiValueDictionary<string,string> oMultiDict = new MultiValueDictionary<string, string>( );

		foreach( MultiValueEntry rEntry in a_rEntries )
		{
			string rKey = rEntry.key;
			foreach( string rValue in rEntry.genericValue )
			{
				oMultiDict.Add( rKey, rValue );
			}
		}

		return oMultiDict;
	}
	
	private void BuildEntriesFromMultiDict( MultiValueDictionary<string, string> a_rMultiDictToSplit, out List<MultiValueEntry> a_rEntries )
	{
		a_rEntries = new List<MultiValueEntry>( a_rMultiDictToSplit.Count );
		
		foreach( KeyValuePair<string, HashSet<string> > rEntry in a_rMultiDictToSplit )
		{
			a_rEntries.Add( new MultiValueEntry( rEntry.Key, new List<string>( rEntry.Value ) ) );
		}
	}

	private void BuildEntriesFromDict( Dictionary<string, string> a_rDictToSplit, out List<SimpleValueEntry> a_rEntries )
	{
		a_rEntries = new List<SimpleValueEntry>( a_rDictToSplit.Count );

		foreach( KeyValuePair<string, string> rEntry in a_rDictToSplit )
		{
			a_rEntries.Add( new SimpleValueEntry( rEntry.Key, rEntry.Value ) );
		}
	}

	private string[ ] QueryMultiValueDict( MultiValueDictionary<string, string> a_rMultiDict, string a_rKey )
	{
		HashSet<string> rValues;
		if( a_rMultiDict.TryGetValue( a_rKey, out rValues ) )
		{
			return new List<string>( rValues ).ToArray( );
		}
		else
		{
			return new string[ 0 ];
		}		
	}

	private string[ ] MultiQueryMultiValueDict( MultiValueDictionary<string, string> a_rMultiDict, IEnumerable<string> a_rKeys, bool a_bUnionResults = true )
	{
		HashSet<string> rResults = new HashSet<string>( );

		if( a_bUnionResults )
		{
			foreach( string rKey in a_rKeys )
			{
				HashSet<string> rValues = null;
				if( a_rMultiDict.TryGetValue( rKey, out rValues ) )
				{
					rResults.UnionWith( rValues );
				}
			}
		}
		else
		{
			foreach( string rKey in a_rKeys )
			{
				HashSet<string> rValues = null;
				if( a_rMultiDict.TryGetValue( rKey, out rValues ) )
				{
					rResults.IntersectWith( rValues );
				}
			}			
		}

		return new List<string>( rResults ).ToArray( );
	}
}
#endif
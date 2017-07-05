using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;

using Uni2DTextureImporterSettingsPair = System.Collections.Generic.KeyValuePair<UnityEngine.Texture2D, UnityEditor.TextureImporterSettings>;
#endif

// Texture Atlas
[AddComponentMenu("Uni2D/Sprite/Uni2DTextureAtlas")]
public class Uni2DTextureAtlas : MonoBehaviour
{	
	// Atlas size
	public enum AtlasSize
	{
		_2		=	2,
		_4		=	4,
		_8		=	8,
		_16		=	16,
		_32		=	32,
		_64		=	64,
		_128	=	128,
		_256 	=	256,
		_512	=	512,
		_1024	=	1024,
		_2048	=	2048,
		_4096	=	4096,
	}

	// Atlas entry
	[Serializable]
	private class AtlasEntry
	{
		[SerializeField]
		private Texture2D m_rAtlasTexture = null;

		[SerializeField]
		private Material m_rAtlasMaterial = null;

		[SerializeField]
		private string[ ] m_rAtlasedTextureGUIDs = null;
		
		[SerializeField]
		private PackedRect[ ] m_rUVs = null;

		public Material material
		{
			get
			{
				return m_rAtlasMaterial;
			}
		}

		public Texture2D atlasTexture
		{
			get
			{
				return m_rAtlasTexture;
			}
		}

		public string[ ] atlasedTextureGUIDs
		{
			get
			{
				return m_rAtlasedTextureGUIDs;
			}
		}

		public AtlasEntry( Texture2D a_rAtlasTexture, Material a_rAtlasMaterial, string[ ] a_rTextureGUIDs, PackedRect[ ] a_rUVs )
		{
			m_rAtlasTexture        = a_rAtlasTexture;
			m_rAtlasMaterial       = a_rAtlasMaterial;
			m_rAtlasedTextureGUIDs = a_rTextureGUIDs;
			m_rUVs                 = a_rUVs;
		}

		public bool Contains( string a_rTextureGUID )
		{
			if( m_rAtlasedTextureGUIDs != null )
			{
				int iGUIDCount = m_rAtlasedTextureGUIDs.Length;
				for( int iGUIDIndex = 0; iGUIDIndex < iGUIDCount; ++iGUIDIndex )
				{
					if( m_rAtlasedTextureGUIDs[ iGUIDIndex ] == a_rTextureGUID )
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool GetUVs( string a_rTextureGUID, out Rect a_rUVs, out bool a_bIsFlipped )
		{
			if( m_rAtlasedTextureGUIDs != null && m_rUVs != null )
			{
				int iGUIDCount = m_rAtlasedTextureGUIDs.Length;
				for( int iGUIDIndex = 0; iGUIDIndex < iGUIDCount; ++iGUIDIndex )
				{
					if( m_rAtlasedTextureGUIDs[ iGUIDIndex ] == a_rTextureGUID )
					{
						a_bIsFlipped = m_rUVs[ iGUIDIndex ].isFlipped;
						a_rUVs = m_rUVs[ iGUIDIndex ].rect;
						return true;
					}
				}
			}

			a_bIsFlipped = false;
			a_rUVs = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
			return false;
		}
	}
	
	// Inspector settings
	// The inspector texture field
	public Texture2DContainer[ ] textures = new Texture2DContainer[ 0 ];
	
	// Padding
	public int padding = 1;
	
	// Maximum atlas size
	public AtlasSize maximumAtlasSize = (AtlasSize) 2048;

	// Base material
	public Material materialOverride = null;
	
	// Generated Data
	[HideInInspector]
	[SerializeField]
	private AtlasEntry[ ] m_oAtlasEntries = null;

	private Dictionary<string, AtlasEntry> m_oTextureGUIDAtlasEntriesDict = null;
	
	// The generation id
	[HideInInspector]
	public string generationId = "";
	
	// Parameters
	// The textures
	[SerializeField]
	[HideInInspector]
	private Texture2DContainer[ ] m_rTextureContainers = new Texture2DContainer[ 0 ];
	
// Disable unused var
#pragma warning disable 414
	// Padding
	[HideInInspector]
	[SerializeField]
	private int m_iPadding = 1;
	
	// Maximum atlas size
	[SerializeField]
	[HideInInspector]
	private AtlasSize m_eMaximumAtlasSize = (AtlasSize) 2048;
#pragma warning restore 414
	
	private Dictionary<string, AtlasEntry> TextureGUIDAtlasEntriesDict
	{
		get
		{
			if( m_oTextureGUIDAtlasEntriesDict == null )
			{
				RebuildDict( );
			}

			return m_oTextureGUIDAtlasEntriesDict;
		}
	}

	public bool Contains( IEnumerable<string> a_rTextureGUIDs )
	{
		foreach( string rTextureGUID in a_rTextureGUIDs )
		{
			if( !this.Contains( rTextureGUID ) )
			{
				return false;
			}
		}
		
		return true;
	}

	// Contains the texture ?
	public bool Contains( string a_rTextureGUID )
	{
		Dictionary<string,AtlasEntry> rDict = this.TextureGUIDAtlasEntriesDict;
		return a_rTextureGUID != null && rDict.ContainsKey( a_rTextureGUID );
	}

	// Returns the atlas material for the given texture
	public Material GetMaterial( string a_rTextureGUID )
	{
		AtlasEntry rAtlasEntry;
		Dictionary<string, AtlasEntry> rDict = this.TextureGUIDAtlasEntriesDict;
		
		if( a_rTextureGUID != null && rDict.TryGetValue( a_rTextureGUID, out rAtlasEntry ) )
		{
			return rAtlasEntry.material;
		}
		else
		{
			return null;
		}
	}

	// Returns the atlas texture for the given texture
	public Texture2D GetAtlasTexture( string a_rTextureGUID )
	{
		AtlasEntry rAtlasEntry;
		Dictionary<string, AtlasEntry> rDict = this.TextureGUIDAtlasEntriesDict;
		
		if( a_rTextureGUID != null && rDict.TryGetValue( a_rTextureGUID, out rAtlasEntry ) )
		{
			return rAtlasEntry.atlasTexture;
		}
		else
		{
			return null;
		}
	}
	
	// Returns the texture UVs in the atlas
	public bool GetUVs( string a_rTextureGUID, out Rect a_rUVs, out bool a_bIsFlipped )
	{
		AtlasEntry rAtlasEntry;
		Dictionary<string, AtlasEntry> rDict = this.TextureGUIDAtlasEntriesDict;

		if( a_rTextureGUID != null && rDict.TryGetValue( a_rTextureGUID, out rAtlasEntry ) )
		{
			return rAtlasEntry.GetUVs( a_rTextureGUID, out a_rUVs, out a_bIsFlipped );
		}
		else
		{
			a_bIsFlipped = false;
			a_rUVs       = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
			return false;
		}
	}

#if UNITY_EDITOR
	public void AddTextures( IEnumerable<string> a_rTextureGUIDs )
	{
		if( a_rTextureGUIDs != null )
		{
			HashSet<Texture2DContainer> oTextureContainers = new HashSet<Texture2DContainer>( this.textures );
			//List<Texture2DContainer> oTextureContainers = new List<Texture2DContainer>( this.textures );

			foreach( string rTextureGUID in a_rTextureGUIDs )
			{
				if( !string.IsNullOrEmpty( rTextureGUID ) )
				{
					oTextureContainers.Add( new Texture2DContainer( rTextureGUID, false ) );
				}
			}

			this.textures = oTextureContainers.ToArray( );

			EditorUtility.SetDirty( this );
		}
	}

	public bool GetUVs( Texture2D a_rTexture, out Rect a_rUVs, out bool a_bIsFlipped )
	{
		return this.GetUVs( Uni2DEditorUtils.GetUnityAssetGUID( a_rTexture ), out a_rUVs, out a_bIsFlipped );
	}
	
	public bool Contains( Texture2D a_rTexture )
	{
		return this.Contains( Uni2DEditorUtils.GetUnityAssetGUID( a_rTexture ) );
	}
	
	public Material GetMaterial( Texture2D a_rTexture )
	{
		return this.GetMaterial( Uni2DEditorUtils.GetUnityAssetGUID( a_rTexture ) );
	}
	
	public Texture2D GetAtlasTexture( Texture2D a_rTexture )
	{
		return this.GetAtlasTexture( Uni2DEditorUtils.GetUnityAssetGUID( a_rTexture ) );
	}

	// Unapplied Settings?
	public bool UnappliedSettings
	{
		get
		{
			if(	m_eMaximumAtlasSize            != maximumAtlasSize 
				|| m_iPadding                  != padding
				|| m_rTextureContainers.Length != textures.Length )
			{
				return true;
			}
			else
			{
				int iTextureIndex = 0;
				foreach( Texture2DContainer rTextureContainer in m_rTextureContainers )
				{
					if( rTextureContainer != textures[ iTextureIndex ] )
					{
						return true;
					}
					++iTextureIndex;
				}
			}
			
			return false;
		}
	}

	// Removes duplicates and empty containers
	private Texture2DContainer[ ] SanitizeInputTextures( Texture2DContainer[ ] a_rTextureContainers )
	{
		int iUniqueTextureContainerCount = 0;
		int iTextureContainerCount       = a_rTextureContainers.Length;
		List<Texture2DContainer> oUniqueTextureContainersList = new List<Texture2DContainer>( iTextureContainerCount );

		// Iterate all texture containers...
		for( int iTextureContainerIndex = 0; iTextureContainerIndex < iTextureContainerCount; ++iTextureContainerIndex )
		{
			Texture2DContainer rTextureContainer = a_rTextureContainers[ iTextureContainerIndex ];

			// if not null and pointing to an existing texture (== not empty)...
			if( rTextureContainer != null && !string.IsNullOrEmpty( rTextureContainer.GUID ) && rTextureContainer.Texture != null )
			{
				bool bIsDuplicate = false;
				
				// Look for duplicates
				for( int iUniqueTextureContainerIndex = 0; !bIsDuplicate && iUniqueTextureContainerIndex < iUniqueTextureContainerCount; ++iUniqueTextureContainerIndex )
				{
					bIsDuplicate = rTextureContainer.GUID == oUniqueTextureContainersList[ iUniqueTextureContainerIndex ].GUID;
				}

				// No duplicate => add to list
				if( !bIsDuplicate )
				{
					oUniqueTextureContainersList.Add( rTextureContainer );
					++iUniqueTextureContainerCount;
				}
			}
		}

		// List -> array
		return oUniqueTextureContainersList.ToArray( );
	}

	// Apply settings
	public bool ApplySettings( bool a_bUpdateSprites = true )
	{
		bool bSuccess;

		Uni2DAssetPostprocessor.Enabled = false;
		{
			int iContainerCount = textures.Length;
			Texture2D[ ] oTexturesToPack = new Texture2D[ iContainerCount ];
			for( int iContainerIndex = 0; iContainerIndex < iContainerCount; ++iContainerIndex )
			{
				oTexturesToPack[ iContainerIndex ] = textures[ iContainerIndex ].Texture;
			}
	
			List<Uni2DTextureImporterSettingsPair> rTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TexturesProcessingBegin( oTexturesToPack );
			oTexturesToPack = null;
	
			// Look if the atlas is set properly regarding to texture sizes
			int iOversizedTextures = this.LookForOversizedTextures( textures, padding, maximumAtlasSize );
			if( iOversizedTextures == 0 )
			{
				textures = this.SanitizeInputTextures( textures );
				iContainerCount = textures.Length;
	
				string rAtlasGUID = Uni2DEditorUtils.GetUnityAssetGUID( this );
				Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
	
				for( int iTextureIndex = 0, iTextureCount = m_rTextureContainers.Length; iTextureIndex < iTextureCount; ++iTextureIndex )
				{
					rAssetTable.RemoveAtlasUsingTexture( rAtlasGUID, m_rTextureContainers[ iTextureIndex ].GUID );
				}
	
				m_rTextureContainers = new Texture2DContainer[ iContainerCount ];
	
				// Deep copy
				for( int iContainerIndex = 0; iContainerIndex < iContainerCount; ++iContainerIndex )
				{
					Texture2DContainer oTextureContainer = new Texture2DContainer( textures[ iContainerIndex ] );
					m_rTextureContainers[ iContainerIndex ] = oTextureContainer;
					rAssetTable.AddAtlasUsingTexture( rAtlasGUID, oTextureContainer.GUID );
				}
	
				rAssetTable.Save( );
	
				m_iPadding = padding;			
				m_eMaximumAtlasSize = maximumAtlasSize;
				
				bSuccess = Generate( );

				if( a_bUpdateSprites )
				{
					Uni2DEditorSpriteBuilderUtils.UpdateSpriteInCurrentSceneAndResourcesAccordinglyToAtlasChange( this );
				}
			}
			else // Some textures can't fit
			{			
				bSuccess = false;
				Debug.LogWarning( "Uni2D could not regenerate atlas '" + ( this.gameObject.name ) + "' properly: "
					+ iOversizedTextures + " texture" + ( iOversizedTextures > 1 ? "s are" : " is" )
					+ " too large to fit in the atlas.", this.gameObject );
			}
	
			Uni2DEditorSpriteBuilderUtils.TexturesProcessingEnd( rTextureImporterSettings );
		}
		Uni2DAssetPostprocessor.Enabled = true;

		EditorUtility.UnloadUnusedAssets( );

		return bSuccess;
	}

	
	// Revert settings
	public void RevertSettings( )
	{
		int iContainerCount = m_rTextureContainers != null ? m_rTextureContainers.Length : 0;

		textures = new Texture2DContainer[ iContainerCount ];

		// Deep copy
		for( int iContainerIndex = 0; iContainerIndex < iContainerCount; ++iContainerIndex )
		{
			textures[ iContainerIndex ] = new Texture2DContainer( m_rTextureContainers[ iContainerIndex ] );
		}
		
		padding = m_iPadding;
		
		maximumAtlasSize = m_eMaximumAtlasSize;

		EditorUtility.SetDirty( this );
	}
	
	// On texture change
	public void OnTextureChange( )
	{
		this.ApplySettings( false );
	}
	
	// Generate
	public bool Generate( )
	{
		// Make sure the data directory exist
		string oGeneratedDataPathLocal  = Uni2DEditorUtils.GetLocalAssetFolderPath(gameObject) + gameObject.name + "_GeneratedData" + "/";
		string oGeneratedDataPathGlobal = Uni2DEditorUtils.LocalToGlobalAssetPath(oGeneratedDataPathLocal);

		if(!Directory.Exists(oGeneratedDataPathGlobal))
		{
			Directory.CreateDirectory(oGeneratedDataPathGlobal);
		}

		// Generate data!
		bool bSuccess = GenerateAtlasData( oGeneratedDataPathLocal );

		generationId = System.Guid.NewGuid( ).ToString( );
		
		EditorUtility.SetDirty( this );

		AssetDatabase.SaveAssets( );

		return bSuccess;
	}

	// Generates enough atlases to contain all given textures
	private bool GenerateAtlasData( string a_oGeneratedDataPathLocal )
	{
		// Build texture rects dict
		int iTexturesCount = m_rTextureContainers.Length;
		Dictionary<int,Rect> oRectsDict = new Dictionary<int,Rect>( iTexturesCount );

		for( int iTextureIndex = 0; iTextureIndex < iTexturesCount; ++iTextureIndex )
		{
			if( m_rTextureContainers[ iTextureIndex ] != null )
			{
				Texture2D rTexture = m_rTextureContainers[ iTextureIndex ];
				if( rTexture != null )
				{
					oRectsDict.Add( iTextureIndex, new Rect( 0.0f, 0.0f, rTexture.width, rTexture.height ) );
				}
			}
		}

		bool bSuccess = false;
		int iAtlasCounter = 0;
		int iRectCount = oRectsDict.Count;
		List<AtlasEntry> oAtlasEntriesList = new List<AtlasEntry>( );

		try
		{
			while( iRectCount > 0 )	// if there's remaining rects to pack
			{
				// Progress bar
				EditorUtility.DisplayProgressBar( "Uni2D Texture Atlasing",
					"Processing... " + iRectCount + " tex. remaining, " + iAtlasCounter + " atlas(es) created so far.",
					1.0f - iRectCount / (float) m_rTextureContainers.Length );
	
				int iAtlasWidth;
				int iAtlasHeight;
				Dictionary<int,PackedRect> oTextureIndexPackedUVRectsDict;
	
				// Pack textures the most as possible
				this.PackUVRects( ref oRectsDict, out oTextureIndexPackedUVRectsDict, out iAtlasWidth, out iAtlasHeight );
	
				// Copy the atlased textures pixels into an atlas texture
				Texture2D oAtlasTexture = this.GenerateAtlasTexture( oTextureIndexPackedUVRectsDict, iAtlasWidth, iAtlasHeight );
	
				// The packer did not success to pack the textures: the atlas seems too small
				if( oTextureIndexPackedUVRectsDict == null || oTextureIndexPackedUVRectsDict.Count == 0 )
				{
					Debug.LogWarning( "Uni2D atlas '" + this.gameObject.name + "' could not pack all the specified textures.\n"
						+ "Please check the maximum allowed atlas size and the input textures.", this.gameObject );
					break;
				}
	
				// Add atlas entry
				oAtlasEntriesList.Add( this.GenerateAtlasEntry( iAtlasCounter, oAtlasTexture, oTextureIndexPackedUVRectsDict, a_oGeneratedDataPathLocal ) );
	
				// Clean
				oTextureIndexPackedUVRectsDict = null;
	
				// Increment atlas counter
				iRectCount = oRectsDict.Count;
				++iAtlasCounter;
			}

			bSuccess = iRectCount <= 0;
		}
		finally
		{
			// Clean exceeding atlas assets (material/texture)
			EditorUtility.DisplayProgressBar( "Uni2D Texture Atlasing", "Cleaning exceeding atlas assets...", 1.0f );

			int iAtlasEntryCount = m_oAtlasEntries != null ? m_oAtlasEntries.Length : 0;
			for( int iEntryIndex = iAtlasCounter; iEntryIndex < iAtlasEntryCount; ++iEntryIndex )
			{
				Material rEntryMaterial = m_oAtlasEntries[ iEntryIndex ].material;
				Texture2D rEntryTexture = m_oAtlasEntries[ iEntryIndex ].atlasTexture;
				
				if( rEntryMaterial != null )
				{
					AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( rEntryMaterial ) );
				}

				if( rEntryTexture != null )
				{
					AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( rEntryTexture ) );
				}
			}

			// List -> array
			m_oAtlasEntries = oAtlasEntriesList.ToArray( );
	
			// Rebuild dict with final GUID/atlas entries lists
			this.RebuildDict( );
	
			// Clean
			oAtlasEntriesList = null;
			oRectsDict        = null;

			EditorUtility.ClearProgressBar( );
		}

		return bSuccess;
	}
	
	// Generate atlas materials
	private Material GenerateAtlasMaterial( int a_iEntryIndex, Material a_rCurrentAtlasMaterial, Texture2D a_rAtlasTexture, string a_oGeneratedDataPathLocal )
	{
		bool bNewMaterial = false;
		Material rAtlasMaterial = a_rCurrentAtlasMaterial;

		// Create material 
		if( a_rCurrentAtlasMaterial == null )
		{
			// Clone base material
			rAtlasMaterial = this.materialOverride != null
				? new Material( this.materialOverride )
				: new Material( Shader.Find( Uni2DEditorSpriteBuilderUtils.mc_oSpriteDefaultShader ) );

			bNewMaterial = true;
		}
		else
		{
			string oFolderPathLocal = Uni2DEditorUtils.GetLocalAssetFolderPath(a_rCurrentAtlasMaterial);
			if(oFolderPathLocal != a_oGeneratedDataPathLocal)
			{	
				// Duplicate
				rAtlasMaterial = new Material(a_rCurrentAtlasMaterial);
				
				bNewMaterial = true;
			}
		}
		
		// If we have created a new material
		if(bNewMaterial)
		{
			rAtlasMaterial.name = gameObject.name + "_AtlasMaterial" + a_iEntryIndex;
			string oMaterialPathLocal = a_oGeneratedDataPathLocal + rAtlasMaterial.name + ".mat";
			
			// Ensure the material can be created
			Material rMaterialAtWantedPath = AssetDatabase.LoadAssetAtPath(oMaterialPathLocal, typeof(Texture2D)) as Material;
			if(rMaterialAtWantedPath != null)
			{
				// Todo_Sev : ask user before deletion?
				AssetDatabase.DeleteAsset(oMaterialPathLocal);
			}
			
			// Create material
			AssetDatabase.CreateAsset(rAtlasMaterial, oMaterialPathLocal);
		}
		
		// Assign the atlas texture
		rAtlasMaterial.mainTexture = a_rAtlasTexture;
		
		return rAtlasMaterial;
	}

	private void PackUVRects( ref Dictionary<int, Rect> a_rRectsToPackDict, out Dictionary<int, PackedRect> a_rPackedUVRectsDict, out int a_iAtlasWidth, out int a_iAtlasHeight )
	{
		bool bAllPacked;
		int iMaximumAtlasSize = (int) m_eMaximumAtlasSize;

		// Estimate atlas size for this loop
		Uni2DEditorTextureAtlasPacker.EstimateMinAtlasSize( a_rRectsToPackDict.Values, iMaximumAtlasSize, m_iPadding, out a_iAtlasWidth, out a_iAtlasHeight );
		Dictionary<int, Rect> oRectsToPackCopyDict = null;
		Uni2DEditorTextureAtlasPacker oPacker = new Uni2DEditorTextureAtlasPacker( a_iAtlasWidth, a_iAtlasHeight, m_iPadding, true );

		// Looking for best atlas size
		// Save infos about the best size we'll found
		Vector2 f2BestSize = new Vector2( a_iAtlasWidth, a_iAtlasHeight );
		Dictionary<int, PackedRect> rBestResultsOutput = null;
		Dictionary<int, Rect> rBestResultsInput = null;
		float fBestOccupancy = float.NegativeInfinity;

		// Breadth-first search data
		Queue<Vector2> oNextSizesToTest = new Queue<Vector2>( );
		oNextSizesToTest.Enqueue( f2BestSize );

		while( oNextSizesToTest.Count != 0 )
		{
			Vector2 f2AtlasSize = oNextSizesToTest.Dequeue( );

			// Create a working copy of the input dictionary
			oRectsToPackCopyDict = new Dictionary<int, Rect>( a_rRectsToPackDict );

			// Reinit bin packer
			oPacker.Init( (int) f2AtlasSize.x, (int) f2AtlasSize.y, m_iPadding, true );

			// Pack the most as possible with BSSF heuristic
			bAllPacked = oPacker.Insert( oRectsToPackCopyDict, Uni2DEditorTextureAtlasPacker.FreeRectChoiceHeuristic.RectBestShortSideFit );

			if( bAllPacked )
			{
				float fOccupancy = oPacker.Occupancy( );

				// if better occupancy than current best or same occupancy and smaller surface area...
				if( fOccupancy > fBestOccupancy || ( fOccupancy == fBestOccupancy && f2AtlasSize.sqrMagnitude < f2BestSize.sqrMagnitude ) )
				{
					// New best
					f2BestSize         = f2AtlasSize;
					fBestOccupancy     = fOccupancy;
					rBestResultsOutput = oPacker.UsedRectangles;
					rBestResultsInput  = oRectsToPackCopyDict;
				}
			}

			// Is tested atlas size is max?
			if( f2AtlasSize.x != iMaximumAtlasSize || f2AtlasSize.y != iMaximumAtlasSize )
			{
				// Add 2 sizes to test: doubled width & doubled height
				Vector2 f2AtlasDoubleWidthSize  = new Vector2( Mathf.Min( f2AtlasSize.x * 2, iMaximumAtlasSize ), f2AtlasSize.y );
				Vector2 f2AtlasDoubleHeightSize = new Vector2( f2AtlasSize.x, Mathf.Min( f2AtlasSize.y * 2, iMaximumAtlasSize ) );

				// Ensure we don't enqueue these sizes twice
				if( f2AtlasSize != f2AtlasDoubleWidthSize && oNextSizesToTest.Contains( f2AtlasDoubleWidthSize ) == false )
				{
					oNextSizesToTest.Enqueue( f2AtlasDoubleWidthSize );
				}
				
				// If sizes equal, it's a square => don't test doubled height or doubled width.
				// These cases are equal (double width square = flipped double height square)
				if( f2AtlasSize.x != f2AtlasSize.y && f2AtlasSize != f2AtlasDoubleHeightSize && oNextSizesToTest.Contains( f2AtlasDoubleHeightSize ) == false )
				{
					oNextSizesToTest.Enqueue( f2AtlasDoubleHeightSize );
				}
			}
			else if( rBestResultsOutput == null )	// Max square, no best results so far
			{
				// Set it as default best
				f2BestSize         = f2AtlasSize;
				//fBestOccupancy = oPacker.Occupancy( );	// Won't be used
				rBestResultsOutput = oPacker.UsedRectangles;
				rBestResultsInput  = oRectsToPackCopyDict;
			}

			oRectsToPackCopyDict = null;
		}

		a_rRectsToPackDict   = rBestResultsInput;
		a_rPackedUVRectsDict = rBestResultsOutput;

		a_iAtlasWidth  = (int) f2BestSize.x;
		a_iAtlasHeight = (int) f2BestSize.y;

		oPacker = null;
	}

	private AtlasEntry GenerateAtlasEntry( int a_iAtlasEntryIndex, Texture2D a_rAtlasTexture, Dictionary<int,PackedRect> a_rTextureIndexUVRectsDict, string a_oGeneratedDataPathLocal )
	{
		Texture2D rCurrentAtlasTexture;
		Material rCurrentAtlasMaterial;

		float fWidth = (float) a_rAtlasTexture.width;
		float fHeight = (float) a_rAtlasTexture.height;

		if( m_oAtlasEntries != null && m_oAtlasEntries.Length > a_iAtlasEntryIndex && m_oAtlasEntries[ a_iAtlasEntryIndex ] != null )
		{
			AtlasEntry rAtlasEntry = m_oAtlasEntries[ a_iAtlasEntryIndex ];
			rCurrentAtlasTexture   = rAtlasEntry.atlasTexture;
			rCurrentAtlasMaterial  = rAtlasEntry.material;
		}
		else
		{
			rCurrentAtlasTexture  = null;
			rCurrentAtlasMaterial = null;
		}

		// rSavedAtlasTexture = instance of the saved/exported/serialized atlas texture at a_oGeneratedDataPathLocal 
		Texture2D rSavedAtlasTexture = this.ExportAndSaveAtlasTexture( a_iAtlasEntryIndex, rCurrentAtlasTexture, a_rAtlasTexture, a_oGeneratedDataPathLocal );
		Material rSavedAtlasMaterial = this.GenerateAtlasMaterial( a_iAtlasEntryIndex, rCurrentAtlasMaterial, rSavedAtlasTexture, a_oGeneratedDataPathLocal );

		int iTextureCount             = a_rTextureIndexUVRectsDict.Count;
		string[ ] oTextureGUIDs       = new string[ iTextureCount ];
		PackedRect[ ] oPackedUVRects  = new PackedRect[ iTextureCount ];

		int iTextureIndex = 0;
		foreach( KeyValuePair<int,PackedRect> rTextureIndexPackedUVRectPair in a_rTextureIndexUVRectsDict )
		{
			oTextureGUIDs[ iTextureIndex ] = m_rTextureContainers[ rTextureIndexPackedUVRectPair.Key ].GUID;

			PackedRect oNormalizedPackedRect = rTextureIndexPackedUVRectPair.Value;

			// Normalize UVs
			Rect rUVRect = oNormalizedPackedRect.rect;
			oNormalizedPackedRect.rect = new Rect( rUVRect.x / fWidth,
					rUVRect.y / fHeight,
					rUVRect.width / fWidth,
					rUVRect.height / fHeight );

			oPackedUVRects[ iTextureIndex ] = oNormalizedPackedRect;

			++iTextureIndex;
		}

		return new AtlasEntry( rSavedAtlasTexture, rSavedAtlasMaterial, oTextureGUIDs, oPackedUVRects );
	}

	private Texture2D ExportAndSaveAtlasTexture( int a_iAtlasEntryIndex, Texture2D a_rCurrentAtlasTexture, Texture2D a_rNewAtlasTexture, string a_oGeneratedDataPathLocal )
	{
		bool bNewTexture = false;

		// Look if there's already a texture at desired path
		if( a_rCurrentAtlasTexture == null )
		{
			// No => create the texture
			bNewTexture = true;
		}
		else
		{
			string oFolderPathLocal = Uni2DEditorUtils.GetLocalAssetFolderPath( a_rCurrentAtlasTexture );
			if( oFolderPathLocal != a_oGeneratedDataPathLocal )
			{	
				bNewTexture = true;
			}
		}

		// Set atlas name accordingly
		if( bNewTexture )
		{
			a_rNewAtlasTexture.name = gameObject.name + "_AtlasTexture" + a_iAtlasEntryIndex;
		}
		else
		{
			a_rNewAtlasTexture.name = a_rCurrentAtlasTexture.name;
		}

		// Get the atlas texture path
		string oAtlasTexturePathLocal  = a_oGeneratedDataPathLocal + a_rNewAtlasTexture.name + ".png";
		string oAtlasTexturePathGlobal = Uni2DEditorUtils.LocalToGlobalAssetPath( oAtlasTexturePathLocal );

		// Save the atlas
		FileStream oFileStream     = new FileStream( oAtlasTexturePathGlobal, FileMode.Create );
		BinaryWriter oBinaryWriter = new BinaryWriter( oFileStream );

		oBinaryWriter.Write( a_rNewAtlasTexture.EncodeToPNG( ) );

		// Close IO resources
		oBinaryWriter.Close( );
		oFileStream.Close( );

		// If we had just created a new texture set the default import settings
		if( bNewTexture )
		{
			ImportNewAtlasTexture( oAtlasTexturePathLocal );
		}
		else
		{
			// or reimport
			AssetDatabase.ImportAsset( oAtlasTexturePathLocal, ImportAssetOptions.ForceUpdate );
		}

		// Destroy the runtime-created instance
		DestroyImmediate( a_rNewAtlasTexture );

		// Save a ref to new atlas texture (by instancing its Unity serialized model)
		a_rNewAtlasTexture = AssetDatabase.LoadAssetAtPath( oAtlasTexturePathLocal, typeof( Texture2D ) ) as Texture2D;
	
		// Mark the atlas texture
		Uni2DEditorUtils.MarkAsTextureAtlas( a_rNewAtlasTexture );

		return a_rNewAtlasTexture;
	}

	private Texture2D GenerateAtlasTexture( Dictionary<int,PackedRect> a_rTextureIndexPackedUVRectsDict, int a_iAtlasWidth, int a_iAtlasHeight )
	{
		Texture2D oAtlasTexture = new Texture2D( a_iAtlasWidth, a_iAtlasHeight, TextureFormat.ARGB32, false );

		// GetPixels32 returns a copy of the atlas pixels
		//Color32[ ] oAtlasColor32 = oAtlasTexture.GetPixels32( );

		// Create ourself the pixel array so we don't need to fill it: array is zero initialized
		Color32[ ] oAtlasColor32 = new Color32[ a_iAtlasWidth * a_iAtlasHeight ];

		/*
		//Fill with black zero alpha
		Color32 f4ClearColor = new Color32( 0, 0, 0, 0 );
		for( int iColorIndex = 0, iColorCount = oAtlasColor32.Length; iColorIndex < iColorCount; ++iColorIndex )
		{
			oAtlasColor32[ iColorIndex ] = f4ClearColor;
		}
		*/

		// Copy the atlased textures pixels into the atlas texture
		foreach( KeyValuePair<int,PackedRect> rIndexedPackedRect in a_rTextureIndexPackedUVRectsDict )
		{
			// The texture to copy
			Texture2D rPackedTexture = m_rTextureContainers[ rIndexedPackedRect.Key ];

			PackedRect rPackedRect = rIndexedPackedRect.Value;

			// The UV rect
			Rect rUVRect = rPackedRect.rect;

			//Debug.Log( rPackedTexture.name + ": " + rPackedRect.rect.x + " / " + rPackedRect.rect.y + " / " + rPackedTexture.width + "px ; " + rPackedTexture.height + "px / flipped: " + rPackedRect.isFlipped );

			// Copy texture into the atlas
			SetPixelBlock32( rPackedTexture.GetPixels32( ), oAtlasColor32,
				(int) rUVRect.x, (int) rUVRect.y,
				(int) rUVRect.width, (int) rUVRect.height,
				a_iAtlasWidth, a_iAtlasHeight,
				rPackedRect.isFlipped );
		}

		// Set the new atlas pixels
		oAtlasTexture.SetPixels32( oAtlasColor32 );
		oAtlasTexture.Apply( );
		oAtlasColor32 = null;

		return oAtlasTexture;
	}

	// Prepares Texture
	private void ImportNewAtlasTexture(string a_rTexturePathLocal)
	{
		Uni2DAssetPostprocessor.ForceImportAssetIfLocked( a_rTexturePathLocal, ImportAssetOptions.ForceSynchronousImport );
	}
	
	// Prepares Texture
	public static void SetDefaultAtlasTextureImportSettings(TextureImporter a_rTextureImporter)
	{
		a_rTextureImporter.textureType    = TextureImporterType.Advanced;
		a_rTextureImporter.maxTextureSize = 4096;
		a_rTextureImporter.textureFormat  = TextureImporterFormat.AutomaticTruecolor;
		a_rTextureImporter.mipmapEnabled  = false;
		a_rTextureImporter.isReadable     = false;
	}

	// Returns the number of textures which won't fit into the atlas
	private int LookForOversizedTextures( Texture2DContainer[ ] a_rTextureContainers, int a_iPadding, AtlasSize a_eMaximumAtlasSize )
	{
		float fMaximumAtlasSize = (float) a_eMaximumAtlasSize;
		float fPadding = (float) a_iPadding;
		int iOversized = 0;
		
		if( a_rTextureContainers != null )
		{
			foreach( Texture2DContainer rTextureContainer in a_rTextureContainers )
			{
				if( rTextureContainer != null )
				{
					Texture2D rTextureToPack = rTextureContainer;
					
					if( rTextureToPack != null && ( rTextureToPack.width + fPadding > fMaximumAtlasSize || rTextureToPack.height + fPadding > fMaximumAtlasSize ) )
					{
						++iOversized;
					}
				}
			}
		}
		return iOversized;
	}

	// Copies a color32 block, which can be flipped, to a larger color32 block
	private void SetPixelBlock32( Color32[ ] a_rSource, Color32[ ] a_rDestination,
		int a_iDestinationX, int a_iDestinationY,
		int a_iSourceWidth, int a_iSourceHeight,
		int a_iDestinationWidth, int a_iDestinationHeight,
		bool a_bIsFlipped )
	{
		if( a_bIsFlipped )
		{
			for( int iY = 0; iY < a_iSourceHeight; ++iY )
			{
				for( int iX = 0; iX < a_iSourceWidth; ++iX )
				{
					a_rDestination[ ( iX + a_iDestinationX ) + ( iY + a_iDestinationY ) * a_iDestinationWidth ] = a_rSource[ iY + ( a_iSourceWidth - 1 - iX ) * a_iSourceHeight ];
				}
			}
		}
		else
		{
			for( int iY = 0; iY < a_iSourceHeight; ++iY )
			{
				for( int iX = 0; iX < a_iSourceWidth; ++iX )
				{
					a_rDestination[ ( iX + a_iDestinationX ) + ( iY + a_iDestinationY ) * a_iDestinationWidth ] = a_rSource[ iX + iY * a_iSourceWidth ];
				}
			}
		}
	}

	public string[ ] GetTextureGUIDs( )
	{
		int iTextureCount = m_rTextureContainers.Length;
		string[ ] oTextureGUIDs = new string[ iTextureCount ];

		for( int iTextureIndex = 0; iTextureIndex < iTextureCount; ++iTextureIndex )
		{
			oTextureGUIDs[ iTextureIndex ] = m_rTextureContainers[ iTextureIndex ].GUID;
		}

		return oTextureGUIDs;
	}

	public Material[ ] GetAllMaterials( )
	{
		int iAtlasEntryCount = m_oAtlasEntries != null ? m_oAtlasEntries.Length : 0;
		Material[ ] oMaterials = new Material[ iAtlasEntryCount ];

		for( int iAtlasEntryIndex = 0; iAtlasEntryIndex < iAtlasEntryCount; ++iAtlasEntryIndex )
		{
			oMaterials[ iAtlasEntryIndex ] = m_oAtlasEntries[ iAtlasEntryIndex ].material;
		}

		return oMaterials;
	}

	public Texture2D[ ] GetAllAtlasTextures( )
	{
		int iAtlasEntryCount = m_oAtlasEntries != null ? m_oAtlasEntries.Length : 0;
		Texture2D[ ] oAtlasTextures = new Texture2D[ iAtlasEntryCount ];

		for( int iAtlasEntryIndex = 0; iAtlasEntryIndex < iAtlasEntryCount; ++iAtlasEntryIndex )
		{
			oAtlasTextures[ iAtlasEntryIndex ] = m_oAtlasEntries[ iAtlasEntryIndex ].atlasTexture;
		}

		return oAtlasTextures;
	}
#endif	// UNITY_EDITOR

	private void RebuildDict( )
	{
		m_oTextureGUIDAtlasEntriesDict = new Dictionary<string, AtlasEntry>( );

		if( m_rTextureContainers != null && m_oAtlasEntries != null )
		{
			// Build texture <-> atlas entry dict.
			int iAtlasEntriesCount = m_oAtlasEntries.Length;
			for( int iAtlasEntryIndex = 0; iAtlasEntryIndex < iAtlasEntriesCount; ++iAtlasEntryIndex )
			{
				AtlasEntry rAtlasEntry = m_oAtlasEntries[ iAtlasEntryIndex ];

				string[ ] rContainedTextureGUIDs = rAtlasEntry.atlasedTextureGUIDs;
				int iContainedTextureGUIDsCount = rContainedTextureGUIDs.Length;
				for( int iContainedTextureGUIDIndex = 0; iContainedTextureGUIDIndex < iContainedTextureGUIDsCount; ++iContainedTextureGUIDIndex )
				{
					m_oTextureGUIDAtlasEntriesDict.Add( rContainedTextureGUIDs[ iContainedTextureGUIDIndex ], rAtlasEntry );
				}
			}
		}
	}
}

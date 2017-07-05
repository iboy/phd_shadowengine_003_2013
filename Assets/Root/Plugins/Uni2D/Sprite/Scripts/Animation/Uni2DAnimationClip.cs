using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using Uni2DTextureImporterSettingsPair = System.Collections.Generic.KeyValuePair<UnityEngine.Texture2D, UnityEditor.TextureImporterSettings>;
#endif

// Animation clip
[Serializable]
[AddComponentMenu( "Uni2D/Sprite/Uni2DAnimationClip" )]
public class Uni2DAnimationClip : MonoBehaviour
{
#if UNITY_EDITOR
	public enum AnimationClipRegeneration
	{
		RegenerateNothing,
		RegenerateAnimationClipOnly,
		RegenerateAlsoAtlasIfNeeded,
		RegenerateAll
	}
#endif

	/// Wrap mode for the clip
	public enum WrapMode
	{
		// Loop
		Loop,
		
		// Play forward then backward infinitely
		PingPong,
		
		// Play once and reset the sprite to the main frame (the sprite without animation)
		Once,
		
		// Play once and stop but don't reset
		ClampForever
	};

	private const string mc_oProgressBarTitle = "Uni2D Animation Clip";

	// Default frame rate
	public const float defaultFrameRate = 12.0f;
	
	///// Clip settings /////
	// Frame rate
	public float frameRate = defaultFrameRate;
	
	// Wrap mode
	public WrapMode wrapMode;
	
	// Frames
	public List<Uni2DAnimationFrame> frames = new List<Uni2DAnimationFrame>( );

	// Global atlas
	public Uni2DTextureAtlas globalAtlas = null;
	
	// Frame index by name
	private Dictionary<string, int> m_oFrameIndexByName;
	
	// Disables unused var warnings because these members are only unused in edit mode
	// but can't be removed since they're serialized...
#pragma warning disable 414
	///// Inspector settings /////
	// Frame rate
	[HideInInspector]
	[SerializeField]
	private float m_fSavedFrameRate = defaultFrameRate;

	// Wrap mode
	[HideInInspector]
	[SerializeField]
	private WrapMode m_eSavedWrapMode;

	// Frames
	[HideInInspector]
	[SerializeField]
	private List<Uni2DAnimationFrame> m_oSavedFrames = new List<Uni2DAnimationFrame>( );

	// Global atlas
	[HideInInspector]
	[SerializeField]
	private Uni2DTextureAtlas m_rSavedGlobalAtlas = null;
#pragma warning restore 414
	// Unused var warnings restored
	
	// Frame count
	public int FrameCount
	{
		get
		{
			return frames.Count;
		}
	}
	
	// Try Get Frame Index By Name
	public bool TryGetFrameIndex(string a_oFrameName, out int a_iFrameIndex)
	{
		Dictionary<string, int> rFrameIndexByName = GetFrameIndexByName();
		return rFrameIndexByName.TryGetValue(a_oFrameName, out a_iFrameIndex);
	}

#if UNITY_EDITOR
	
	// Unapplied Settings? (inspector)
	public bool UnappliedSettings
	{
		get
		{
			if( m_eSavedWrapMode != wrapMode || m_fSavedFrameRate != frameRate || m_rSavedGlobalAtlas != globalAtlas || ( globalAtlas != null && globalAtlas.UnappliedSettings ) )
			{
				return true;
			}
			else
			{
				return this.FramesAreDifferents( );	// Are settings inspector settings different from object ones?
			}
		}
	}
	
	// Frames are differents
	private bool FramesAreDifferents( )
	{
		if( frames != null && m_oSavedFrames != null )
		{
			if( frames.Count != m_oSavedFrames.Count )
			{
				return true;
			}
			else
			{
				int iFrameIndex = 0;
				foreach( Uni2DAnimationFrame rFrame in frames )
				{
					if( rFrame.IsDifferentFrom( m_oSavedFrames[ iFrameIndex ] ) )
					{
						return true;
					}
					++iFrameIndex;
				}
			}
			return false;
		}

		return frames != m_oSavedFrames;
	}


	// Copy clips
	private void CopyFrames( List<Uni2DAnimationFrame> a_rFramesSource, List<Uni2DAnimationFrame> a_rFramesDestination )
	{
		// Update asset table
		Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
		string rClipGUID = Uni2DEditorUtils.GetUnityAssetGUID( this );
		
		foreach( Uni2DAnimationFrame rOldFrame in a_rFramesDestination )
		{
			string oTextureGUID = rOldFrame.textureContainer != null ? rOldFrame.textureContainer.GUID : null;
			if( !string.IsNullOrEmpty( oTextureGUID ) )
			{
				rAssetTable.RemoveClipUsingTexture( rClipGUID, oTextureGUID );
			}
		}
		
		a_rFramesDestination.Clear( );
		foreach( Uni2DAnimationFrame rFrameSource in a_rFramesSource )
		{
			a_rFramesDestination.Add( new Uni2DAnimationFrame( rFrameSource ) );
			
			string oTextureGUID = rFrameSource.textureContainer != null ? rFrameSource.textureContainer.GUID : null;
			if( !string.IsNullOrEmpty( oTextureGUID ) )
			{
				rAssetTable.AddClipUsingTexture( rClipGUID, oTextureGUID );
			}
		}
		
		rAssetTable.Save( );
	}

	public void SwapFrames( int a_iFrameIndexA, int a_iFrameIndexB )
	{
		int iFrameCount = frames.Count;
		if( a_iFrameIndexA != a_iFrameIndexB && 0 <= a_iFrameIndexA && 0 <= a_iFrameIndexB && a_iFrameIndexA < iFrameCount && a_iFrameIndexB < iFrameCount )
		{
			// Swap frames
			Uni2DAnimationFrame rTmp = frames[ a_iFrameIndexA ];
			frames[ a_iFrameIndexA ] = frames[ a_iFrameIndexB ];
			frames[ a_iFrameIndexB ] = rTmp;
		}
	}

	// On textures change
	public void OnTexturesChange( ICollection<string> a_rTextureGUIDs )
	{
		List<Uni2DAnimationFrame> oFramesToUpdate = new List<Uni2DAnimationFrame>( );

		foreach( Uni2DAnimationFrame rFrame in frames )
		{
			if( a_rTextureGUIDs.Contains( rFrame.textureContainer.GUID ) )
			{
				oFramesToUpdate.Add( rFrame );
			}
		}

		GenerateTextureFramesInfos( oFramesToUpdate );

		CopyFrames( frames, m_oSavedFrames );

		EditorUtility.SetDirty( this );
		AssetDatabase.SaveAssets( );
	}

	// Apply settings from inspector
	public void ApplySettings( AnimationClipRegeneration a_eRegeneration = AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded )
	{
		m_eSavedWrapMode    = wrapMode;
		m_fSavedFrameRate   = frameRate;
		m_rSavedGlobalAtlas = globalAtlas;
		
		if( a_eRegeneration != AnimationClipRegeneration.RegenerateNothing )
		{
			Generate( a_eRegeneration );	// Generate frames infos (and atlas if needed/asked)

			CopyFrames( frames, m_oSavedFrames );	// frames -> m_oFrames (apply)
		}

		EditorUtility.SetDirty( this );
		AssetDatabase.SaveAssets( );
	}
	
	// Revert settings
	public void RevertSettings( )
	{
		wrapMode    = m_eSavedWrapMode;
		frameRate   = m_fSavedFrameRate;
		globalAtlas = m_rSavedGlobalAtlas;

		if( globalAtlas != null )
		{
			globalAtlas.RevertSettings( );
		}

		CopyFrames( m_oSavedFrames, frames );	// m_oFrames -> frames (revert)

		EditorUtility.SetDirty( this );
		AssetDatabase.SaveAssets( );
	}
	
	// Generate
	private void Generate( AnimationClipRegeneration a_eRegeneration )
	{
		GenerateAllFramesInfos( );

		GenerateAtlas( a_eRegeneration );
	}

	// Generate atlas
	private void GenerateAtlas( AnimationClipRegeneration a_eRegeneration )
	{
		int iFrameCount = this.FrameCount;

		if( globalAtlas != null ) // global atlas
		{
			string[ ] oTextureGUIDs = this.GetAllFramesTextureGUIDs( );

			if( a_eRegeneration == AnimationClipRegeneration.RegenerateAll
				|| ( a_eRegeneration == AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded
					&& ( globalAtlas.UnappliedSettings || globalAtlas.Contains( oTextureGUIDs ) == false ) ) )
			{
				globalAtlas.ApplySettings( );
			}

			globalAtlas.AddTextures( oTextureGUIDs );
		}
		else // Atlas per frame
		{
			HashSet<Uni2DTextureAtlas> oFrameAtlases = new HashSet<Uni2DTextureAtlas>( );

			for( int iFrameIndex = 0; iFrameIndex < iFrameCount; ++iFrameIndex )
			{
				Uni2DAnimationFrame rFrame = frames[ iFrameIndex ];
				Uni2DTextureAtlas rFrameAtlas = rFrame.atlas;
				if( rFrameAtlas != null )
				{
					string oFrameTextureGUID = rFrame.textureContainer != null ? rFrame.textureContainer.GUID : null;

					if( a_eRegeneration == AnimationClipRegeneration.RegenerateAll
						|| ( a_eRegeneration == AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded
							&& ( rFrameAtlas.UnappliedSettings || rFrameAtlas.Contains( oFrameTextureGUID ) == false ) ) )
					{
						oFrameAtlases.Add( rFrameAtlas );
					}
	
					rFrameAtlas.AddTextures( new string[ 1 ]{ oFrameTextureGUID } );
				}
			}

			// Regenerate atlases
			foreach( Uni2DTextureAtlas rFrameAtlas in oFrameAtlases )
			{
				rFrameAtlas.ApplySettings( );
			}
		}
	}
	
	// Generate all frames infos
	private void GenerateAllFramesInfos( )
	{
		this.GenerateTextureFramesInfos( frames );
	}
	
	// Update all frames infos about given textures
	private void GenerateTextureFramesInfos( ICollection<Uni2DAnimationFrame> a_rFramesToGenerate )
	{
		Uni2DAssetPostprocessor.Enabled = false;
		EditorUtility.DisplayProgressBar( mc_oProgressBarTitle, "Reading texture settings...", 0.0f );

		int iFrameCount = a_rFramesToGenerate.Count;
		int iFrameIndex = 0;
		Texture2D[ ] oFrameTextures = new Texture2D[ iFrameCount ];
		foreach( Uni2DAnimationFrame rFrame in a_rFramesToGenerate )
		{
			oFrameTextures[ iFrameIndex++ ] = rFrame.textureContainer.Texture;
		}

		// Prepare textures
		List<Uni2DTextureImporterSettingsPair> oImporterSettingsPair = Uni2DEditorSpriteBuilderUtils.TexturesProcessingBegin( oFrameTextures );

		try
		{
			iFrameIndex = 1;
			float fInvCount = 1.0f / (float) iFrameCount;
			foreach( Uni2DAnimationFrame rFrame in a_rFramesToGenerate )
			{
				EditorUtility.DisplayProgressBar( mc_oProgressBarTitle,
					"Generating frame #" + iFrameIndex + " out of " + iFrameCount,
					iFrameIndex * fInvCount );
	
				rFrame.GenerateInfos( );
				++iFrameIndex;
			}
	
			// Restore Texture import settings
			EditorUtility.DisplayProgressBar( mc_oProgressBarTitle, "Restoring texture settings...", 1.0f );
		}
		finally
		{
			Uni2DEditorSpriteBuilderUtils.TexturesProcessingEnd( oImporterSettingsPair );
			EditorUtility.ClearProgressBar( );
			Uni2DAssetPostprocessor.Enabled = true;
		}
	}
	
	// Get Textures
	private List<Texture2D> GetAllFramesTextures( )
	{
		// Get textures
		List<Texture2D> rTextures = new List<Texture2D>( this.FrameCount );

		foreach(Uni2DAnimationFrame rFrame in frames )
		{
			rTextures.Add( rFrame.textureContainer );
		}
		
		return rTextures;
	}

	public string[ ] GetAllFramesTextureGUIDs( )
	{
		HashSet<string> oUniqueTextureGUIDs = new HashSet<string>( );

		foreach( Uni2DAnimationFrame rFrame in frames )
		{
			Texture2DContainer rTextureContainer = rFrame.textureContainer;
			if( rTextureContainer != null && !string.IsNullOrEmpty( rTextureContainer.GUID ) )
			{
				oUniqueTextureGUIDs.Add( rTextureContainer.GUID );
			}
		}

		string[ ] oTextureGUIDs = new string[ oUniqueTextureGUIDs.Count ];
		oUniqueTextureGUIDs.CopyTo( oTextureGUIDs );
		return oTextureGUIDs;
	}

	public bool AreClipAndAtlasSynced( )
	{
		if( m_rSavedGlobalAtlas != null )
		{
			foreach( Uni2DAnimationFrame rFrame in frames )
			{
				Texture2D rFrameTexture = rFrame.textureContainer;
				if( rFrame.atlas != globalAtlas || ( rFrameTexture != null && globalAtlas.Contains( rFrameTexture ) == false ) )
				{
					return false;
				}
			}
		}
		else
		{
			foreach( Uni2DAnimationFrame rFrame in frames )
			{
				Texture2D rFrameTexture = rFrame.textureContainer;
				if( rFrameTexture != null && rFrame.atlas != null && rFrame.atlas.Contains( rFrameTexture ) == false )
				{
					return false;
				}
			}
		}

		return true;
	}
#endif
	
	// Get Frame Index By Name
	private Dictionary<string, int> GetFrameIndexByName()
	{
		if(m_oFrameIndexByName == null)
		{
			BuildFrameIndexByName();
		}
		return m_oFrameIndexByName;
	}
	
	// Build Frame Index By Name
	private void BuildFrameIndexByName()
	{
		m_oFrameIndexByName = new Dictionary<string, int>();
		int iFrameIndex = 0;
		foreach(Uni2DAnimationFrame rFrame in frames)
		{
			if(rFrame != null)
			{
				if(m_oFrameIndexByName.ContainsKey(rFrame.name) == false)
				{
					m_oFrameIndexByName.Add(rFrame.name, iFrameIndex);
				}
			}
			iFrameIndex++;
		}
	}
}
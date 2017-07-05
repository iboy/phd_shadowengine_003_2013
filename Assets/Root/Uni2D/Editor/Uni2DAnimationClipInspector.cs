using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AtlasSize = Uni2DTextureAtlas.AtlasSize;
using AnimationGUIAction = Uni2DEditorGUIUtils.AnimationGUIAction;
using AnimationClipRegeneration = Uni2DAnimationClip.AnimationClipRegeneration;

[CanEditMultipleObjects]
[CustomEditor( typeof( Uni2DAnimationClip ) )]
public class Uni2DAnimationClipInspector : Editor
{
	// Atlas section foldout
	private static bool ms_bAtlasFoldout = true;

	// Clips foldouts
	private static List<BitArray> ms_bClipsFoldouts = new List<BitArray>( 1 );

	// Preview title
	private static GUIContent ms_oPreviewTitleContent = new GUIContent( "Animation Clip Preview" );

	// Animation player, used in clip header
	private Uni2DAnimationPlayer m_oAnimationPlayerClipHeader = new Uni2DAnimationPlayer( );

	// Animation player, used in clip preview
	private Uni2DAnimationPlayer m_oAnimationPlayerClipPreview = new Uni2DAnimationPlayer( );

	// Last saved tick count
	private long m_lLastTimeTicks;

	// Last saved tick count in preview
	private long m_lLastPreviewTimeTicks;

	private bool m_bAreClipsAndAtlasSynced = true;

	// Texture GUIDs contained in the clip(s)
	private HashSet<string> m_oTextureGUIDs = new HashSet<string>( );

	private static Uni2DAnimationClip[ ] ms_oAnimationClipsWithUnappliedSettings = null;

	void OnEnable( )
	{
		SetupAnimationPlayers( );

		CheckClipsAndAtlasSynced( );

		UpdateTextureGUIDsList( );
	}

	void OnDisable( )
	{
		if( this.DoTargetsHaveUnappliedSettings( ) )
		{
			ms_oAnimationClipsWithUnappliedSettings = targets.Cast<Uni2DAnimationClip>( ).ToArray( );

			EditorApplication.delayCall += Uni2DAnimationClipInspector.AskAboutUnappliedSettings;
		}
	}

	private static void AskAboutUnappliedSettings( )
	{
		if( ms_oAnimationClipsWithUnappliedSettings != null && ms_oAnimationClipsWithUnappliedSettings.Length > 0 )
		{
			bool bApply = EditorUtility.DisplayDialog( "Unapplied settings on animation clip", "Some animation clip settings haven't be applied. Applied them now?", "Apply", "Revert" );

			if( bApply )
			{
				Uni2DAnimationClipInspector.ApplySettings( ms_oAnimationClipsWithUnappliedSettings, AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded );
			}
			else
			{
				Uni2DAnimationClipInspector.RevertSettings( ms_oAnimationClipsWithUnappliedSettings );
			}
			
			ms_oAnimationClipsWithUnappliedSettings = null;
		}
	}


	
	public override void OnInspectorGUI( )
	{
		// Update animation player used in clip header
		m_oAnimationPlayerClipHeader.Update( this.ComputeDeltaTime( ref m_lLastTimeTicks ) );

		// Atlas
		bool bHasMultipleDifferentGlobalAtlasValues;
		Uni2DTextureAtlas rGlobalAtlas;
		this.GetGlobalAtlas( out rGlobalAtlas, out bHasMultipleDifferentGlobalAtlasValues );

		// Atlas size
		AtlasSize eMaximumAtlasSize = AtlasSize._2048;
		//SerializedSetting<EAtlasSize> oSerializedSetting_MaximumAtlasSize;

		// Selected object count
		int iTargetObjectCount = targets.Length;
		bool bDisplayFrames = ( iTargetObjectCount == 1 );

		EditorGUI.BeginDisabledGroup( Application.isPlaying );
		{
			EditorGUILayout.BeginVertical( );
			{
				if( bDisplayFrames == false )
				{
					EditorGUILayout.HelpBox( "Animation Clip edition is restricted when inspecting multiple clips, but you can change the atlas settings to pack the clips together.", MessageType.Info, true );
				}
	
				if( m_bAreClipsAndAtlasSynced == false )
				{
					EditorGUILayout.HelpBox( "At least one animation frame is referencing an atlas which does not contain its texture.\nPlease regenerate the clip or manually add the frame texture to the atlas.", MessageType.Error, true );
				}
	
				///// Atlas section /////
				ms_bAtlasFoldout = EditorGUILayout.Foldout( ms_bAtlasFoldout, "Atlas" );
	
				if( ms_bAtlasFoldout )
				{
					bool bHasAtlasSettingsChanged;
					bool bHasAtlasChanged;

					++EditorGUI.indentLevel;
					{
						EditorGUILayout.BeginVertical( );
						{
							EditorGUILayout.BeginHorizontal( );
							{
								EditorGUILayout.PrefixLabel( "Use Atlas" );
	
								// Atlas popup
								EditorGUI.BeginChangeCheck( );
								{
									EditorGUI.showMixedValue = bHasMultipleDifferentGlobalAtlasValues;
									{
										rGlobalAtlas = Uni2DEditorGUIUtils.AtlasPopup( rGlobalAtlas, m_oTextureGUIDs );
									}
									EditorGUI.showMixedValue = false;
								}
								bHasAtlasChanged = EditorGUI.EndChangeCheck( );
	
								// Atlas select button
								EditorGUI.BeginDisabledGroup( rGlobalAtlas == null );
								{
									if( GUILayout.Button( "Select", GUILayout.Width( 80.0f ) ) )
									{
										EditorGUIUtility.PingObject( rGlobalAtlas.gameObject );
									}
								}
								EditorGUI.EndDisabledGroup( );
							}
							EditorGUILayout.EndHorizontal( );
	
							// Atlas max size
							EditorGUI.BeginChangeCheck( );
							{
								EditorGUI.BeginDisabledGroup( ( bHasAtlasChanged == false && bHasMultipleDifferentGlobalAtlasValues ) || rGlobalAtlas == null );
								{
									eMaximumAtlasSize = (AtlasSize) EditorGUILayout.EnumPopup( "Maximum Atlas Size", rGlobalAtlas != null ? rGlobalAtlas.maximumAtlasSize : eMaximumAtlasSize );
								}
								EditorGUI.EndDisabledGroup( );
							}
							bHasAtlasSettingsChanged = EditorGUI.EndChangeCheck( );
						}
						EditorGUILayout.EndVertical( );
					}
					--EditorGUI.indentLevel;
	
					if( rGlobalAtlas != null && bHasAtlasSettingsChanged )
					{
						rGlobalAtlas.maximumAtlasSize = eMaximumAtlasSize;
						EditorUtility.SetDirty( rGlobalAtlas );
					}
	
					if( bHasAtlasChanged )
					{
						this.ApplyAnimationClipGlobalAtlas( rGlobalAtlas );
					}
				}
				
				EditorGUILayout.Space( );
	
				///// Animation Clip Header /////
				for( int iClipIndex = 0, iClipCount = iTargetObjectCount; iClipIndex < iClipCount; ++iClipIndex )
				{
					Uni2DAnimationClip rAnimationClip = (Uni2DAnimationClip) this.targets[ iClipIndex ];
					int iFrameCount = rAnimationClip.FrameCount;
					
					this.UpdateClipsFoldouts( iClipIndex, iFrameCount );
	
					bool bClipHeaderFoldout = this.GetClipFoldout( iClipIndex );
	
					EditorGUI.BeginDisabledGroup( bDisplayFrames == false );
					{
						EditorGUI.BeginChangeCheck( );
						{
							bClipHeaderFoldout = Uni2DEditorGUIUtils.DisplayAnimationClipHeader( rAnimationClip, m_oAnimationPlayerClipHeader, bClipHeaderFoldout );
						}
						if( EditorGUI.EndChangeCheck( ) && bDisplayFrames )
						{
							// Update animation preview player settings
							m_oAnimationPlayerClipPreview.FrameRate = rAnimationClip.frameRate;
							m_oAnimationPlayerClipPreview.WrapMode  = rAnimationClip.wrapMode;
						}
					}
					EditorGUI.EndDisabledGroup( );
	
					this.SetClipFoldout( bClipHeaderFoldout, iClipIndex );
	
					///// Clip frames section /////
					if( bDisplayFrames && bClipHeaderFoldout )
					{
						EditorGUILayout.BeginHorizontal( );
						{
							// Dummy space
							GUILayout.Space( 32.0f );
		
							EditorGUILayout.BeginVertical( );
							{
								// Display all frames
								for( int iFrameIndex = 0; iFrameIndex < iFrameCount; ++iFrameIndex )
								{
									// Display animation frame
									AnimationGUIAction eAction;	// = AnimationGUIAction.None;
									bool bFrameFoldout = this.GetClipFoldout( iClipIndex, iFrameIndex );
									
									EditorGUI.BeginChangeCheck( );
									{
										eAction = Uni2DEditorGUIUtils.DisplayAnimationFrame( rAnimationClip.frames[ iFrameIndex ], rGlobalAtlas, ref bFrameFoldout );
									}
									if( EditorGUI.EndChangeCheck( ) )
									{
										EditorUtility.SetDirty( rAnimationClip );
									}
	
									this.SetClipFoldout( bFrameFoldout, iClipIndex, iFrameIndex );
		
									// Handle returned action (if any)
									switch( eAction )
									{
										// No action
										case AnimationGUIAction.None:
										default:
											break;
	
										case AnimationGUIAction.AddUp:
										{
											Uni2DAnimationFrame oNewFrame = new Uni2DAnimationFrame( );
											oNewFrame.atlas = rGlobalAtlas;
		
											rAnimationClip.frames.Insert( iFrameIndex, oNewFrame );
											++iFrameCount;
											++iFrameIndex;
	
											this.UpdateClipsFoldouts( iClipIndex, iFrameCount );
	
											EditorUtility.SetDirty( rAnimationClip );
										}
										break;
										
										case AnimationGUIAction.AddDown:
										{
											Uni2DAnimationFrame oNewFrame = new Uni2DAnimationFrame( );
											oNewFrame.atlas = rGlobalAtlas;
		
											rAnimationClip.frames.Insert( iFrameIndex + 1, oNewFrame );
											++iFrameCount;
	
											this.UpdateClipsFoldouts( iClipIndex, iFrameCount );
	
											EditorUtility.SetDirty( rAnimationClip );
										}
										break;
										
										case AnimationGUIAction.MoveUp:
										{
											if( iFrameIndex != 0 )
											{
												rAnimationClip.SwapFrames( iFrameIndex, iFrameIndex - 1 );
	
												EditorUtility.SetDirty( rAnimationClip );
											}
										}
										break;
										
										case AnimationGUIAction.MoveDown:
										{
											if( iFrameIndex != iFrameCount - 1 )
											{
												rAnimationClip.SwapFrames( iFrameIndex, iFrameIndex + 1 );
	
												EditorUtility.SetDirty( rAnimationClip );
											}
										}
										break;
										
										case AnimationGUIAction.Close:
										{
											rAnimationClip.frames.RemoveAt( iFrameIndex );
											UpdateTextureGUIDsList( );
		
											--iFrameCount;
											--iFrameIndex;
	
											EditorUtility.SetDirty( rAnimationClip );
										}
										break;
		
									}
								}		
								EditorGUILayout.Space( );
							}
							EditorGUILayout.EndVertical( );
						}
						EditorGUILayout.EndHorizontal( );
	
						EditorGUILayout.BeginHorizontal( );
						{
							GUILayout.FlexibleSpace( );
							if( GUILayout.Button( "Add Frame", GUILayout.Width( 225.0f ) ) )
							{
								Uni2DAnimationFrame oNewFrame = new Uni2DAnimationFrame( );
								oNewFrame.atlas = rGlobalAtlas;
								rAnimationClip.frames.Add( oNewFrame );
								EditorUtility.SetDirty( rAnimationClip );
							}
							GUILayout.FlexibleSpace( );
						}
						EditorGUILayout.EndHorizontal( );
	
					}	// End of clip header foldout
	
					EditorGUILayout.Space( );
				}
	
				///// Apply / Revert / Force regeneration buttons /////
				EditorGUILayout.BeginVertical( );
				{
					EditorGUILayout.BeginHorizontal( );
					{
						EditorGUI.BeginDisabledGroup( this.DoTargetsHaveUnappliedSettings( ) == false );
						{
							if( GUILayout.Button( "Apply" ) )
							{
								this.ApplySettings( AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded );
							}
	
							if( GUILayout.Button( "Revert" ) )
							{
								this.RevertSettings( );
							}
						}
						EditorGUI.EndDisabledGroup( );
					}
					EditorGUILayout.EndHorizontal( );
	
					if( GUILayout.Button( "Force Clip Regeneration" ) )
					{
						this.ApplySettings( AnimationClipRegeneration.RegenerateAll );
					}
				}
				EditorGUILayout.EndVertical( );
			}
			EditorGUILayout.EndVertical( );
		}
		EditorGUI.EndDisabledGroup( );

		if( Application.isPlaying == false )
		{
			this.CheckTextureDragAndDrop( GUILayoutUtility.GetLastRect( ) );
		}

		// Force repaint if needed (== animation players enabled)
		if( m_oAnimationPlayerClipHeader.Enabled || m_oAnimationPlayerClipPreview.Enabled )
		{
			this.Repaint( );
		}
	}

	///// Inspector utils /////

	private void CheckClipsAndAtlasSynced( )
	{
		for( int iClipIndex = 0, iClipCount = targets.Length; iClipIndex < iClipCount; ++iClipIndex )
		{
			if( targets[ iClipIndex ] != null )
			{
				Uni2DAnimationClip rAnimationClip = (Uni2DAnimationClip) targets[ iClipIndex ];
				if( rAnimationClip.AreClipAndAtlasSynced( ) == false )
				{
					m_bAreClipsAndAtlasSynced = false;
					return;
				}
			}
		}

		m_bAreClipsAndAtlasSynced = true;
	}

	private void UpdateTextureGUIDsList( )
	{
		m_oTextureGUIDs.Clear( );

		foreach( Object rTarget in targets )
		{
			if( rTarget != null )
			{
				m_oTextureGUIDs.UnionWith( ( (Uni2DAnimationClip) rTarget ).GetAllFramesTextureGUIDs( ) );
			}
		}
	}

	private void ApplySettings( AnimationClipRegeneration a_eRegenerate )
	{
		Uni2DAnimationClipInspector.ApplySettings( targets.Cast<Uni2DAnimationClip>( ).ToArray( ), a_eRegenerate );

		this.CheckClipsAndAtlasSynced( );
	}

	private void RevertSettings( )
	{
		Uni2DAnimationClipInspector.RevertSettings( targets.Cast<Uni2DAnimationClip>( ).ToArray( ) );

		this.CheckClipsAndAtlasSynced( );
	}

	private static void ApplySettings( Uni2DAnimationClip[ ] a_rAnimationClips, AnimationClipRegeneration a_eRegenerate )
	{
		int iClipCount = a_rAnimationClips.Length;
		
		if( iClipCount == 1 )
		{
			a_rAnimationClips[ 0 ].ApplySettings( a_eRegenerate );
		}
		else if( iClipCount > 1 )
		{
			// Apply clip settings first, atlases will be generated only after
			// (Atlases can be shared accross clips, so prevent them to be uselessly regenerated several times)
			HashSet<Uni2DTextureAtlas> oAtlases = new HashSet<Uni2DTextureAtlas>( );
			
			for( int iClipIndex = 0; iClipIndex < iClipCount; ++iClipIndex )
			{
				Uni2DAnimationClip rAnimationClip = a_rAnimationClips[ iClipIndex ];
				Uni2DTextureAtlas rGlobalAtlas = rAnimationClip.globalAtlas;
				
				if( rGlobalAtlas != null )
				{
					// Add the atlas if not already added
					if( a_eRegenerate == AnimationClipRegeneration.RegenerateAll
						|| ( a_eRegenerate == AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded
						&& ( rGlobalAtlas.UnappliedSettings || rGlobalAtlas.Contains( rAnimationClip.GetAllFramesTextureGUIDs( ) ) == false ) ) )
					{
						oAtlases.Add( rGlobalAtlas );
					}
				}
				else // Null => atlas per frame
				{
					for( int iFrameIndex = 0, iFrameCount = rAnimationClip.FrameCount; iFrameIndex < iFrameCount; ++iFrameIndex )
					{
						Uni2DAnimationFrame rFrame = rAnimationClip.frames[ iFrameIndex ];
						Uni2DTextureAtlas rFrameAtlas = rFrame.atlas;
						
						if( rFrameAtlas != null
							&& ( a_eRegenerate == AnimationClipRegeneration.RegenerateAll
							|| ( a_eRegenerate == AnimationClipRegeneration.RegenerateAlsoAtlasIfNeeded
							&& ( rFrameAtlas.UnappliedSettings || rFrameAtlas.Contains( rFrame.textureContainer.GUID ) == false ) ) ) )
						{
							oAtlases.Add( rFrameAtlas );
						}
					}
				}
				
				// Regenerate clips only
				rAnimationClip.ApplySettings( AnimationClipRegeneration.RegenerateAnimationClipOnly );
			}
			
			// Then, regenerate atlases
			foreach( Uni2DTextureAtlas rAtlas in oAtlases )
			{
				rAtlas.ApplySettings( );
			}
		}
	}

	private static void RevertSettings( Uni2DAnimationClip[ ] a_rAnimationClips )
	{
		for( int iIndex = 0, iCount = a_rAnimationClips.Length; iIndex < iCount; ++iIndex )
		{
			a_rAnimationClips[ iIndex ].RevertSettings( );
		}
	}

	private bool DoTargetsHaveUnappliedSettings( )
	{
		for( int iClipIndex = 0, iClipCount = targets.Length; iClipIndex < iClipCount; ++iClipIndex )
		{
			if( targets[ iClipIndex ] != null && ( (Uni2DAnimationClip) targets[ iClipIndex ] ).UnappliedSettings )
			{
				return true;
			}
		}

		return false;
	}

	private void ApplyAnimationClipGlobalAtlas( Uni2DTextureAtlas a_rGlobalAtlas )
	{
		for( int iClipIndex = 0, iClipCount = this.targets.Length; iClipIndex < iClipCount; ++iClipIndex )
		{
			Uni2DAnimationClip rAnimationClip = (Uni2DAnimationClip) this.targets[ iClipIndex ];
			rAnimationClip.globalAtlas = a_rGlobalAtlas;
			
			for( int iFrameIndex = 0, iFrameCount = rAnimationClip.FrameCount; iFrameIndex < iFrameCount; ++iFrameIndex )
			{
				rAnimationClip.frames[ iFrameIndex ].atlas = a_rGlobalAtlas;
			}

			EditorUtility.SetDirty( rAnimationClip );
		}
	}

	// Because of an Unity bug(?), SerializedProperty.hasMultipleDifferentValues and objectReferenceValue properties are inconsistent when
	// handling Uni2D atlases. This method does it manually.
	private void GetGlobalAtlas( out Uni2DTextureAtlas a_rGlobalAtlas, out bool a_bHasMultipleDifferentGlobalAtlasValues )
	{
		int iTargetCount = this.targets.Length;

		a_rGlobalAtlas = ( (Uni2DAnimationClip) targets[ 0 ] ).globalAtlas;
		a_bHasMultipleDifferentGlobalAtlasValues = false;

		for( int iTargetIndex = 1; iTargetIndex < iTargetCount; ++iTargetIndex )
		{
			if( a_rGlobalAtlas != ( (Uni2DAnimationClip) targets[ iTargetIndex ] ).globalAtlas )
			{
				a_rGlobalAtlas = null;
				a_bHasMultipleDifferentGlobalAtlasValues = true;
				return;
			}
		}
	}

	////////// Animation clip utils //////////

	// Handles texture drop over inspector
	private void CheckTextureDragAndDrop( Rect a_rDropAreaRect )
	{
		// Handle drag only if not multi selecting
		if( targets.Length == 1 )
		{
			// Current Event
			Event rCurrentEvent = Event.current;
			EventType rCurrentEventType = rCurrentEvent.type;
			Vector2 f2MousePos = rCurrentEvent.mousePosition;

			// Dragging over clip inspector area
			if( a_rDropAreaRect.Contains( f2MousePos ) )
			{
				bool bShouldAccept = false;

				// Does contain at least one texture2D?
				foreach( Object rObject in DragAndDrop.objectReferences )
				{
					if( rObject is Texture2D )
					{
						bShouldAccept = true;
						break;
					}
				}
	
				if( bShouldAccept )
				{
					// Accept
					if( rCurrentEventType == EventType.DragPerform )
					{
						DragAndDrop.AcceptDrag( );
						rCurrentEvent.Use( );
	
						DragAndDrop.activeControlID = 0;
	
						Uni2DTextureAtlas rGlobalAtlas = ( (Uni2DAnimationClip) target ).globalAtlas;
						List<Uni2DAnimationFrame> rAnimationFrames = ( (Uni2DAnimationClip) target ).frames;

						// Sort the dropped elements
						IOrderedEnumerable<Object> rSortedObjects = DragAndDrop.objectReferences.OrderBy( x => x.name );

						// Add the textures
						foreach( Object rObject in rSortedObjects )
						{
							if( rObject is Texture2D )
							{
								Texture2D rTexture = (Texture2D) rObject;

								// TODO: refactor animation frame insert
								// Create a new frame
								Uni2DAnimationFrame oNewAnimationFrame = new Uni2DAnimationFrame( );
								oNewAnimationFrame.atlas = rGlobalAtlas;

								// Keep texture reference if no atlas
								oNewAnimationFrame.textureContainer = new Texture2DContainer( rTexture, rGlobalAtlas == null );

								// Add frame to clip
								rAnimationFrames.Add( oNewAnimationFrame );

							}
						}
						EditorUtility.SetDirty( target );
					}
					else if( rCurrentEventType == EventType.DragUpdated ) // Hint drop is possible
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
				}
				else // Reject
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}
		}
	}

	///// Clips foldouts utils /////
	private void UpdateClipsFoldouts( int a_iClipIndex, int a_iSize )
	{
		int iClipCount = ms_bClipsFoldouts.Count;
		BitArray rFoldoutBitArray;
	
		if( a_iClipIndex >= iClipCount )
		{
			for( int iClipIndex = iClipCount - 1; iClipIndex < a_iClipIndex; ++iClipIndex )
			{
				ms_bClipsFoldouts.Add( new BitArray( 1, true ) );
			}
		}

		rFoldoutBitArray = ms_bClipsFoldouts[ a_iClipIndex ];

		if( rFoldoutBitArray.Count < a_iSize + 1 )
		{
			rFoldoutBitArray.Length = a_iSize + 1;
		}
	}

	private bool GetClipFoldout( int a_iClipIndex, int a_iFrame = -1 )
	{
		return ms_bClipsFoldouts[ a_iClipIndex ][ a_iFrame + 1 ];
	}

	private void SetClipFoldout( bool a_bValue, int a_iClipIndex, int a_iFrame = -1 )
	{
		ms_bClipsFoldouts[ a_iClipIndex ][ a_iFrame + 1 ] = a_bValue;
	}

	///// Preview GUI /////
	
	// Asks to display a preview GUI when selecting one non-empty clip
	public override bool HasPreviewGUI( )
	{
		return this.targets.Length == 1 && ( (Uni2DAnimationClip) this.target ).FrameCount > 0;
	}

	public override GUIContent GetPreviewTitle( )
	{
		return ms_oPreviewTitleContent;
	}

	public override string GetInfoString( )
	{
		Uni2DAnimationFrame rAnimationFrame = m_oAnimationPlayerClipPreview.Frame;
		if( rAnimationFrame != null )
		{
			string oTextureInfo;

			if( rAnimationFrame.textureContainer == null || rAnimationFrame.textureContainer.Texture == null )
			{
				oTextureInfo = "(None)";
			}
			else
			{
				oTextureInfo = rAnimationFrame.textureContainer.Texture.name;
			}

			return "Wrap mode: " + m_oAnimationPlayerClipPreview.WrapMode + " - Frame texture: " + oTextureInfo;
		}

		return "(None)";
	}

	public override void OnPreviewSettings( )
	{
		EditorGUILayout.BeginHorizontal( );
		{
			/*
			float fMinWidth;
			float fMaxWidth;
			EditorStyles.whiteBoldLabel.CalcMinMaxWidth( new GUIContent( "Animation Clip Preview" ), out fMinWidth, out fMaxWidth );
			
			GUILayout.Space( fMinWidth );
			*/
	
			// Frame label
			GUILayout.Label( "Frame " + ( m_oAnimationPlayerClipPreview.FrameIndex + 1 ) + "/" + m_oAnimationPlayerClipPreview.FrameCount, EditorStyles.miniLabel );

			// Frame slider
			float fNormalizedTime;
			EditorGUI.BeginChangeCheck( );
			{
				fNormalizedTime = GUILayout.HorizontalSlider( m_oAnimationPlayerClipPreview.NormalizedTime, 0.0f, 1.0f, new GUILayoutOption[ ]{ GUILayout.MinWidth( 50.0f ), GUILayout.MaxWidth( 100.0f ) } );
			}
			if( EditorGUI.EndChangeCheck( ) )
			{
				m_oAnimationPlayerClipPreview.Paused = true;
				m_oAnimationPlayerClipPreview.NormalizedTime = fNormalizedTime;
			}

			// Rewind to first frame |<<
			// Unity 4 doesn't handle these chars very well, so...
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if( GUILayout.Button( "\u2503\u25c0\u25c0", EditorStyles.toolbarButton ) )
#else
			if( GUILayout.Button( "\u2503\u25c1\u25c1", EditorStyles.toolbarButton ) )
#endif
			{
				m_oAnimationPlayerClipPreview.Paused = true;
				m_oAnimationPlayerClipPreview.FrameIndex = 0;
			}

			// Previous frame <|
			// Unity 4 doesn't handle these chars very well, so...
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if( GUILayout.Button( "\u25c0\u2503", EditorStyles.toolbarButton ) )
#else
			if( GUILayout.Button( "\u25c1\u2503", EditorStyles.toolbarButton ) )
#endif
			{
				m_oAnimationPlayerClipPreview.Paused = true;
				--m_oAnimationPlayerClipPreview.FrameIndex;
			}

			// Play / pause
			// Is paused or not enabled?
			if( m_oAnimationPlayerClipPreview.Paused || m_oAnimationPlayerClipPreview.Enabled == false )
			{
				// Display play button >
				if( GUILayout.Button( "\u25B6", EditorStyles.toolbarButton ) )
				{
					if( m_oAnimationPlayerClipPreview.Paused )
					{
						m_oAnimationPlayerClipPreview.Paused = false;
					}
					else
					{
						m_oAnimationPlayerClipPreview.Play( );
					}
				}
			}	// Playing
			else if( GUILayout.Button( "\u2590\u2590", EditorStyles.toolbarButton ) )	// Display pause button ||
			{
				m_oAnimationPlayerClipPreview.Paused = true;
			}

			// Next frame |>
			if( GUILayout.Button( "\u2503\u25B6", EditorStyles.toolbarButton ) )
			{
				m_oAnimationPlayerClipPreview.Paused = true;
				++m_oAnimationPlayerClipPreview.FrameIndex;
			}

			// Go to last frame >>|
			if( GUILayout.Button( "\u25B6\u25B6\u2503", EditorStyles.toolbarButton ) )
			{
				m_oAnimationPlayerClipPreview.Paused = true;
				m_oAnimationPlayerClipPreview.FrameIndex = m_oAnimationPlayerClipPreview.FrameCount - 1;
			}
		}
		EditorGUILayout.EndHorizontal( );
	}

	public override void OnPreviewGUI( Rect a_rRect, GUIStyle a_rBackgroundStyle )
	{
		float fDeltaTime = this.ComputeDeltaTime( ref m_lLastPreviewTimeTicks );
		m_oAnimationPlayerClipPreview.Update( fDeltaTime );

		if( Event.current.type == EventType.Repaint )
		{
			Uni2DAnimationFrame rFrame = m_oAnimationPlayerClipPreview.Frame;
			Texture2D rFrameTexture;

			if( rFrame == null || rFrame.textureContainer == null || rFrame.textureContainer.Texture == null )
			{
				rFrameTexture = null;
			}
			else
			{
				rFrameTexture = rFrame.textureContainer;
			}

			GUIContent rPreviewContent = new GUIContent( rFrameTexture );

			a_rBackgroundStyle.alignment = TextAnchor.MiddleCenter;
			a_rBackgroundStyle.Draw( a_rRect, rPreviewContent, GUIUtility.GetControlID( rPreviewContent, FocusType.Passive ) );
		}
	}
	

	///// Animation player utils /////
	
	private float ComputeDeltaTime( ref long a_lBaseTicks )
	{
		// Current tick count
		long lNow = System.DateTime.Now.Ticks;

		// Compute delta time. 1ms = 10,000 ticks => divide by 10,000,000 to have seconds
		float fDeltaTime = ( lNow - a_lBaseTicks ) * 0.0000001f;

		// Save tick count
		a_lBaseTicks = lNow;

		return fDeltaTime;
	}

	private void SetupAnimationPlayers( )
	{
		if( target != null )
		{
			m_oAnimationPlayerClipPreview.onAnimationNewFrameEvent += this.ForceRepaint;
			m_oAnimationPlayerClipPreview.onAnimationInactiveEvent += this.RestartAnimationPlayer;
			m_oAnimationPlayerClipPreview.Play( (Uni2DAnimationClip) target );
			m_oAnimationPlayerClipPreview.Stop( false );
	
			m_oAnimationPlayerClipHeader.onAnimationNewFrameEvent += this.ForceRepaint;
			m_oAnimationPlayerClipHeader.onAnimationInactiveEvent += this.RestartAnimationPlayer;
			m_oAnimationPlayerClipHeader.Play( (Uni2DAnimationClip) target );
			m_oAnimationPlayerClipHeader.Stop( false );
	
			ComputeDeltaTime( ref m_lLastTimeTicks );
			ComputeDeltaTime( ref m_lLastPreviewTimeTicks );
		}
	}

	private void ForceRepaint( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		this.Repaint( );
	}

	private void RestartAnimationPlayer( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		a_rAnimationPlayer.Play( (Uni2DAnimationClip) target );
		a_rAnimationPlayer.Stop( false );
	}
}

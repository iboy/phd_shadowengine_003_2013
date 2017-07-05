#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* Uni2DEditorGUIUtils
 * 
 * Various common GUI drawing methods used by Uni2D inspectors.
 * 
 * Editor use only.
 */
public static class Uni2DEditorGUIUtils
{
	// Describes the GUI action performed by the user when displaying the animation frame GUI
	public enum AnimationGUIAction
	{
		None,
		MoveUp,
		MoveDown,
		AddUp,
		AddDown,
		Close
	}

	private class PopupCallbackInfo<T> where T : UnityEngine.Object
	{
		public static PopupCallbackInfo<T> instance = null;

		private int m_iControlID = 0;
		private T m_rValue = default( T );
		private bool m_bHasBeenSetted = false;
		
		public PopupCallbackInfo( int a_iControlID, T a_rValue )
		{
			m_iControlID = a_iControlID;
			m_rValue = a_rValue;
		}

		// Returns the selected value
		public static T GetSelectedValueForControl( int a_iControlID, T a_rValue )
		{
			if( Event.current.type == EventType.Repaint )
			{
				// Check if any instance exists and if it concerns the right control ID
				if( instance != null && instance.m_iControlID == a_iControlID )
				{
					// Yes, if it has been setted, return the value and clear the instance ref.
					
					if( instance.m_bHasBeenSetted )
					{
						GUI.changed = true;
						a_rValue = instance.m_rValue;
					}
	
					instance = null;
				}
			}
			// if not, return a_rValue
			
			return a_rValue;
		}

		// Sets value selected in the popup menu
		public void SetValue( T a_rUserData )
		{
			m_rValue = a_rUserData;
			m_bHasBeenSetted = true;
		}
	}

	///// Custom popup callbacks /////
	private static void AtlasPopupCallback( object a_rValue )
	{
		if( a_rValue != null && a_rValue is string )
		{
			PopupCallbackInfo<Uni2DTextureAtlas> rPopupCallbackInfoInstance = PopupCallbackInfo<Uni2DTextureAtlas>.instance;
			string rGUID = (string) a_rValue;

			if( string.IsNullOrEmpty( rGUID ) )
			{
				rPopupCallbackInfoInstance.SetValue( null );
			}
			else if( rGUID == "NEW" )
			{
				string oTextureAtlasPath = EditorUtility.SaveFilePanelInProject( "Create new Uni2D texture atlas",
					"TextureAtlas_New",
					"prefab",
					"Create a new Uni2D texture atlas:" );

				if( string.IsNullOrEmpty( oTextureAtlasPath ) == false )
				{
					// TODO: refactor with Sprite Builder Window
					// Create model
					GameObject oPrefabModel = new GameObject( );
					oPrefabModel.AddComponent<Uni2DTextureAtlas>( );
					
					// Save it as a prefab
					GameObject rTextureAtlasGameObject = PrefabUtility.CreatePrefab( oTextureAtlasPath, oPrefabModel );
					
					// Destroy model
			        GameObject.DestroyImmediate( oPrefabModel );
	
					rPopupCallbackInfoInstance.SetValue( rTextureAtlasGameObject.GetComponent<Uni2DTextureAtlas>( ) );
				}
			}
			else
			{
				rPopupCallbackInfoInstance.SetValue( Uni2DEditorUtils.GetAssetFromUnityGUID<Uni2DTextureAtlas>( rGUID ) );
			}
		}
	}

	private static void ClipPopupCallback( object a_rValue )
	{
		if( a_rValue != null && a_rValue is string )
		{
			PopupCallbackInfo<Uni2DAnimationClip> rPopupCallbackInfoInstance = PopupCallbackInfo<Uni2DAnimationClip>.instance;
			string rGUID = (string) a_rValue;

			if( string.IsNullOrEmpty( rGUID ) )
			{
				rPopupCallbackInfoInstance.SetValue( null );
			}
			else if( rGUID == "NEW" )
			{
				string oAnimationClipPath = EditorUtility.SaveFilePanelInProject( "Create new Uni2D animation clip",
					"AnimationClip_New",
					"prefab",
					"Create a new Uni2D animation clip:" );

				if( string.IsNullOrEmpty( oAnimationClipPath ) == false )
				{
					// TODO: refactor with Sprite Builder Window
					// Create model
					GameObject oPrefabModel = new GameObject( );
					oPrefabModel.AddComponent<Uni2DAnimationClip>( );
					
					// Save it as a prefab
					GameObject rAnimationClipGameObject = PrefabUtility.CreatePrefab( oAnimationClipPath, oPrefabModel );
					
					// Destroy model
			        GameObject.DestroyImmediate( oPrefabModel );
	
					rPopupCallbackInfoInstance.SetValue( rAnimationClipGameObject.GetComponent<Uni2DAnimationClip>( ) );
				}
			}
			else
			{
				rPopupCallbackInfoInstance.SetValue( Uni2DEditorUtils.GetAssetFromUnityGUID<Uni2DAnimationClip>( rGUID ) );
			}
		}
	}

	// Displays a preview of an animation clip at given position rect
	// The animation is played when the mouse is hovering the preview
	public static void DrawAnimationClipPreview( Rect a_rPositionRect, Uni2DAnimationClip a_rAnimationClip, Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		Uni2DAnimationFrame rFrame;
		Texture2D rFrameTexture;

		if( a_rPositionRect.Contains( Event.current.mousePosition ) )	// If mouse hovering preview rect...
		{
			if( a_rAnimationPlayer.Enabled == false || a_rAnimationPlayer.Clip != a_rAnimationClip )	// ... and player not enabled or not set with current clip...
			{
				a_rAnimationPlayer.Play( a_rAnimationClip );	// ... play clip
			}

			rFrame = a_rAnimationPlayer.Frame;
		}
		else if( Event.current.type != EventType.Layout && a_rAnimationPlayer.Enabled && a_rAnimationPlayer.Clip == a_rAnimationClip )	// Stop player if playing current clip
		{
			a_rAnimationPlayer.Stop( false );
			a_rAnimationPlayer.FrameIndex = 0;

			rFrame = a_rAnimationPlayer.Frame;
		}
		else // Use first clip frame otherwise (if any)
		{
			rFrame = a_rAnimationClip != null && a_rAnimationClip.frames != null && a_rAnimationClip.frames.Count > 0 ? a_rAnimationClip.frames[ 0 ] : null;
		}

		if( rFrame == null || rFrame.textureContainer == null || rFrame.textureContainer.Texture == null )
		{
			rFrameTexture = EditorGUIUtility.whiteTexture;
		}
		else
		{
			rFrameTexture = rFrame.textureContainer;
		}

		EditorGUI.DrawPreviewTexture( a_rPositionRect, rFrameTexture );

		return;
	}

	// Displays the animation frame GUI
	// Returns the performed user action
	public static AnimationGUIAction DisplayAnimationFrame( Uni2DAnimationFrame a_rAnimationFrame, Uni2DTextureAtlas a_rGlobalAtlas, ref bool a_bEventFoldout )
	{
		AnimationGUIAction eAction = AnimationGUIAction.None;

		// Box
		EditorGUILayout.BeginVertical( EditorStyles.textField );
		{
			///// Top toolbar /////
			EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
			{
				// ^
				if( GUILayout.Button( "\u25B2" /*"\u2191"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
				{
					eAction = AnimationGUIAction.MoveUp;
				}

				// v
				if( GUILayout.Button( "\u25BC" /*"\u2193"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
				{
					eAction = AnimationGUIAction.MoveDown;
				}

				// + ^
				if( GUILayout.Button( "+ \u25B2" /*"+ \u2191"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) ) )
				{
					eAction = AnimationGUIAction.AddUp;
				}

				// X
				if( GUILayout.Button( "X", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
				{
					eAction = AnimationGUIAction.Close;
				}
			}
			EditorGUILayout.EndHorizontal( );

			///////////////

			EditorGUILayout.Space( );

			///// Animation Frame box /////
			EditorGUILayout.BeginHorizontal( );
			{
				Texture2D rFrameTexture = a_rAnimationFrame.textureContainer;
				string rFrameTextureGUID = a_rAnimationFrame.textureContainer.GUID;
				bool bHasFrameTextureChanged;

				// Display frame texture on the left
				Rect oClipTextureRect = GUILayoutUtility.GetRect( 64.0f, 64.0f, 64.0f, 64.0f, GUILayout.ExpandWidth( false ) );

				EditorGUI.BeginChangeCheck( );
				{
					rFrameTexture = (Texture2D) EditorGUI.ObjectField( oClipTextureRect, GUIContent.none, rFrameTexture, typeof( Texture2D ), false );
				}
				bHasFrameTextureChanged = EditorGUI.EndChangeCheck( );

	
				EditorGUILayout.BeginVertical( GUILayout.ExpandWidth( true ) );
				{
					// Frame texture name
					GUILayout.Label( rFrameTexture != null ? rFrameTexture.name : "(No Texture)", EditorStyles.boldLabel, GUILayout.ExpandWidth( false ) );
					
					// Frame Name
					a_rAnimationFrame.name = EditorGUILayout.TextField( "Frame Name", a_rAnimationFrame.name);
					
					// Frame atlas
					EditorGUILayout.BeginHorizontal( );
					{
						// Disable popup menu if global atlas is set
						EditorGUI.BeginDisabledGroup( a_rGlobalAtlas != null );
						{
							// Atlas popup
							string[ ] oTextureGUID = ( rFrameTexture != null ) ? new string[ 1 ]{ rFrameTextureGUID } : new string[ 0 ];

							EditorGUILayout.PrefixLabel( "Use Atlas" );
							a_rAnimationFrame.atlas = Uni2DEditorGUIUtils.AtlasPopup( a_rAnimationFrame.atlas, oTextureGUID );
						}
						EditorGUI.EndDisabledGroup( );

						// Atlas select button
						EditorGUI.BeginDisabledGroup( a_rAnimationFrame.atlas == null);
						{
							if( GUILayout.Button( "Select", GUILayout.Width( 80.0f ) ) )
							{
								EditorGUIUtility.PingObject( a_rAnimationFrame.atlas.gameObject );
							}
						}
						EditorGUI.EndDisabledGroup( );
					}
					EditorGUILayout.EndHorizontal( );

					// Trigger?
					a_rAnimationFrame.triggerEvent = EditorGUILayout.Toggle( "Trigger Event", a_rAnimationFrame.triggerEvent );

					// Event param
					a_bEventFoldout = EditorGUILayout.Foldout( a_bEventFoldout, "Frame Infos" );
					if( a_bEventFoldout )
					{
						Uni2DAnimationFrameInfos rFrameInfos = a_rAnimationFrame.frameInfos;

						++EditorGUI.indentLevel;
						{	
							rFrameInfos.stringInfo = EditorGUILayout.TextField( "String Info", rFrameInfos.stringInfo );
							rFrameInfos.intInfo    = EditorGUILayout.IntField( "Int Info", rFrameInfos.intInfo );
							rFrameInfos.floatInfo  = EditorGUILayout.FloatField( "Float Info", rFrameInfos.floatInfo );
							rFrameInfos.objectInfo = EditorGUILayout.ObjectField( "Object Info", rFrameInfos.objectInfo, typeof( Object ), true );
						}
						--EditorGUI.indentLevel;
					}
					EditorGUILayout.Space( );
				}
				EditorGUILayout.EndVertical( );

				if( bHasFrameTextureChanged )
				{
					// Save texture in texture container, keep reference to the asset if not using an atlas
					a_rAnimationFrame.textureContainer = new Texture2DContainer( rFrameTexture, a_rGlobalAtlas == null );
				}
			}
			EditorGUILayout.EndHorizontal( );

			///////////////

			EditorGUILayout.Space( );

			///// Bottom toolbar /////
			EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
			{
				// + v
				if( GUILayout.Button( "+ \u25BC" /*"+ \u2193"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) ) )
				{
					eAction = AnimationGUIAction.AddDown;
				}
			}
			EditorGUILayout.EndHorizontal( );
		}
		EditorGUILayout.EndVertical( );

		return eAction;
	}

	// Displays an animation clip header
	public static AnimationGUIAction DisplayCompactAnimationClipHeader( Uni2DAnimationClip a_rAnimationClip, Uni2DAnimationPlayer a_rAnimationPreviewPlayer, int a_iClipIndex = -1 )
	{
		AnimationGUIAction eAction;
		Uni2DEditorGUIUtils.DoDisplayAnimationClipHeader( a_rAnimationClip, a_rAnimationPreviewPlayer, out eAction, true, false, a_iClipIndex );

		return eAction;
	}

	public static bool DisplayAnimationClipHeader( Uni2DAnimationClip a_rAnimationClip, Uni2DAnimationPlayer a_rAnimationPreviewPlayer, bool a_bFolded )
	{
		AnimationGUIAction eAction;
		return Uni2DEditorGUIUtils.DoDisplayAnimationClipHeader( a_rAnimationClip, a_rAnimationPreviewPlayer, out eAction, false, a_bFolded );
	}

	private static bool DoDisplayAnimationClipHeader( Uni2DAnimationClip a_rAnimationClip, Uni2DAnimationPlayer a_rAnimationPreviewPlayer, out AnimationGUIAction a_eAction, bool a_bCompactMode, bool a_bFolded, int iClipIndex = -1 )
	{
		a_eAction = AnimationGUIAction.None;

		///// Clip header /////
		EditorGUILayout.BeginVertical( EditorStyles.textField );
		{
			if( a_bCompactMode )
			{
				///// Top toolbar /////
				EditorGUILayout.BeginHorizontal( EditorStyles.toolbar, GUILayout.ExpandWidth( true ) );
				{
					// ^
					if( GUILayout.Button( "\u25B2" /*"\u2191"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
					{
						a_eAction = AnimationGUIAction.MoveUp;
					}
	
					// v
					if( GUILayout.Button( "\u25BC" /*"\u2193"*/, EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
					{
						a_eAction = AnimationGUIAction.MoveDown;
					}

					if( iClipIndex > -1 )
					{
						if( GUILayout.Button( "Clip #" + iClipIndex, EditorStyles.toolbarButton, GUILayout.ExpandWidth( true ) ) )
						{
							EditorGUIUtility.PingObject( a_rAnimationClip.gameObject );
						}
					}
					else
					{
						GUILayout.FlexibleSpace( );
					}

					// X
					if( GUILayout.Button( "X", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
					{
						a_eAction = AnimationGUIAction.Close;
					}
				}
				EditorGUILayout.EndHorizontal( );
			}
			///////////////

			EditorGUILayout.Space( );

			EditorGUILayout.BeginHorizontal( );
			{
				Rect oClipTextureRect = GUILayoutUtility.GetRect( 64.0f, 64.0f, 64.0f, 64.0f, GUILayout.ExpandWidth( false ) );

				// Animation clip preview
				Uni2DEditorGUIUtils.DrawAnimationClipPreview( oClipTextureRect, a_rAnimationClip, a_rAnimationPreviewPlayer );

				// Clip settings
				EditorGUILayout.BeginVertical( );
				{
					//string rName;
					float fFrameRate;
					Uni2DAnimationClip.WrapMode eWrapMode;

					// Name
					//rName = EditorGUILayout.TextField( "Clip Name", a_rAnimationClip.name );
					EditorGUILayout.LabelField( a_rAnimationClip.name, EditorStyles.boldLabel );
					
					EditorGUI.BeginChangeCheck( );
					{
						EditorGUI.BeginChangeCheck( );
						{
							// Frame rate
							fFrameRate = EditorGUILayout.FloatField( "Frame Rate", a_rAnimationClip.frameRate );
							// Wrap mode
							eWrapMode = (Uni2DAnimationClip.WrapMode) EditorGUILayout.EnumPopup( "Wrap Mode", a_rAnimationClip.wrapMode );
						}
						if( EditorGUI.EndChangeCheck( ) )
						{
							// Update animation players settings
							a_rAnimationPreviewPlayer.FrameRate = fFrameRate;
							a_rAnimationPreviewPlayer.WrapMode  = eWrapMode;
						}
					}
					if( EditorGUI.EndChangeCheck( ) )
					{
						//a_rAnimationClip.name      = rName;
						a_rAnimationClip.frameRate = fFrameRate;
						a_rAnimationClip.wrapMode  = eWrapMode;
						
						if( a_bCompactMode )
						{
							a_rAnimationClip.ApplySettings( Uni2DAnimationClip.AnimationClipRegeneration.RegenerateNothing );
						}

						EditorUtility.SetDirty( a_rAnimationClip );
					}

					// Clip length infos
					// TODO: refactor with AnimationPlayer
					int iClipFrameCount = a_rAnimationClip.FrameCount;
					int iWrappedFrameCount = iClipFrameCount;
					if( a_rAnimationClip.wrapMode == Uni2DAnimationClip.WrapMode.PingPong && iWrappedFrameCount > 2 )
					{
						iWrappedFrameCount = ( iWrappedFrameCount * 2 - 2 );
					}

					float fClipLength = Mathf.Abs( iWrappedFrameCount / a_rAnimationClip.frameRate );

					EditorGUILayout.BeginHorizontal( );
					{
						EditorGUILayout.LabelField( iClipFrameCount + " frame(s) = "
							+ ( iClipFrameCount != iWrappedFrameCount
								? ( iWrappedFrameCount + " wrapped frame(s) = " )
								: null
							)
							+ fClipLength + " sec. @ "
							+ a_rAnimationClip.frameRate + " FPS",
							EditorStyles.miniLabel, GUILayout.ExpandWidth( false ) );

						if( a_bCompactMode && GUILayout.Button( "Edit", EditorStyles.miniButton, GUILayout.ExpandWidth( true ) ) )
						{
						 	Selection.activeObject = a_rAnimationClip;
						}
					}
					EditorGUILayout.EndHorizontal( );
				}
				EditorGUILayout.EndVertical( );
			}
			EditorGUILayout.EndHorizontal( );

			// Frame foldout
			if( !a_bCompactMode )
			{
				EditorGUILayout.BeginHorizontal( );
				{
					a_bFolded = EditorGUILayout.Foldout( a_bFolded, GUIContent.none );
				}
				EditorGUILayout.EndHorizontal( );
			}
			else
			{
				EditorGUILayout.Space( );
			}
		}
		EditorGUILayout.EndVertical( );

		return a_bFolded;
	}

	public static Uni2DTextureAtlas AtlasPopup( Uni2DTextureAtlas a_rTextureAtlas, IEnumerable<string> a_rTextureGUIDs, params GUILayoutOption[ ] a_rGUILayoutOptions )
	{
		// Get button control ID
		int iControlID = GUIUtility.GetControlID( FocusType.Native );

		// Get selected value for our control
		// If no PopupCallbackInfo instance exists, the returned value is a_rTextureAtlas
		a_rTextureAtlas = PopupCallbackInfo<Uni2DTextureAtlas>.GetSelectedValueForControl( iControlID, a_rTextureAtlas );

		// Create a new generic menu
		// Each item menu will use AtlasPopupCallback as callback
		// AtlasPopupCallback will perform the logic and save the selected atlas to
		// the PopupCallbackInfo instance.
		string oPopupSelected = EditorGUI.showMixedValue ? "-" : ( a_rTextureAtlas != null ? a_rTextureAtlas.name : "(None)" );
		
		if( GUILayout.Button( oPopupSelected, EditorStyles.popup, a_rGUILayoutOptions ) )
		{
			string rAtlasGUID = Uni2DEditorUtils.GetUnityAssetGUID( a_rTextureAtlas );

			// Create a new popup callback info (control ID) and save it as current instance
			PopupCallbackInfo<Uni2DTextureAtlas>.instance = new PopupCallbackInfo<Uni2DTextureAtlas>( iControlID, a_rTextureAtlas );

			// Create our generic menu
			GenericMenu oPopupMenu = new GenericMenu( );
			
			if( a_rTextureAtlas != null )
			{
				oPopupMenu.AddItem( new GUIContent( a_rTextureAtlas.name ), true, AtlasPopupCallback, rAtlasGUID );
				oPopupMenu.AddSeparator( "" );
			}

			// "None" special item menu
			oPopupMenu.AddItem( new GUIContent( "(None)", "No atlasing" ), a_rTextureAtlas == null, AtlasPopupCallback, "" );

			oPopupMenu.AddSeparator( "" );
			
			// "Create" special item menu
			oPopupMenu.AddItem( new GUIContent( "Create a new atlas...", "Creates a new Uni2D atlas and add the texture(s) right away" ), false, AtlasPopupCallback, "NEW" );
				
			Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;

			// List atlases containing the texture(s)
			Dictionary<string, string> oAtlasesReadyToUse = rAssetTable.GetAtlasNamesUsingTheseTextures( a_rTextureGUIDs );
			
			if( !string.IsNullOrEmpty( rAtlasGUID ) )
			{
				oAtlasesReadyToUse.Remove( rAtlasGUID );
			}

			if( oAtlasesReadyToUse.Count > 0 )
			{
				oPopupMenu.AddSeparator( "" );

				// Add an item menu for each ready to use atlas
				foreach( KeyValuePair<string, string> rAtlasNameGUIDPair in oAtlasesReadyToUse )
				{
					oPopupMenu.AddItem( new GUIContent( rAtlasNameGUIDPair.Value ), rAtlasNameGUIDPair.Key == rAtlasGUID, AtlasPopupCallback, rAtlasNameGUIDPair.Key );
				}
			}

			// List all available atlases
			Dictionary<string, string> oAvailableAtlases = rAssetTable.GetAllAtlasNames( );
			if( oAvailableAtlases.Count > 0 )
			{
				oPopupMenu.AddSeparator( "" );

				// Add an item menu for each available atlas, in a submenu
				foreach( KeyValuePair<string, string> rAtlasNameGUIDPair in oAvailableAtlases )
				{
					oPopupMenu.AddItem( new GUIContent( "All atlases/" + rAtlasNameGUIDPair.Value ), rAtlasNameGUIDPair.Key == rAtlasGUID, AtlasPopupCallback, rAtlasNameGUIDPair.Key );
				}
			}

			// Finally show up the menu
			oPopupMenu.ShowAsContext( );
		}

		return a_rTextureAtlas;
	}

	public static bool AddClipPopup( string a_rLabel, out Uni2DAnimationClip a_rAnimationClip, params GUILayoutOption[ ] a_rGUILayoutOptions )
	{
		bool bHasChanged;
		
		// Get button control ID
		int iControlID = GUIUtility.GetControlID( FocusType.Native );

		EditorGUI.BeginChangeCheck( );
		{
			// Get selected value for our control
			// If no PopupCallbackInfo instance exists, the returned value is a_rClip
			a_rAnimationClip = PopupCallbackInfo<Uni2DAnimationClip>.GetSelectedValueForControl( iControlID, null );
		}
		bHasChanged = EditorGUI.EndChangeCheck( );
	
		// Create a new generic menu
		// Each item menu will use AtlasPopupCallback as callback
		// AtlasPopupCallback will perform the logic and save the selected atlas to
		// the PopupCallbackInfo instance.
		if( GUILayout.Button( a_rLabel, a_rGUILayoutOptions ) )
		{
			// Create a new popup callback info (control ID) and save it as current instance
			PopupCallbackInfo<Uni2DAnimationClip>.instance = new PopupCallbackInfo<Uni2DAnimationClip>( iControlID, null );

			// Create our generic menu
			GenericMenu oPopupMenu = new GenericMenu( );
			
			// "Create" special item menu
			oPopupMenu.AddItem( new GUIContent( "Create a new animation clip...", "Creates a new Uni2D animation clip" ), false, ClipPopupCallback, "NEW" );

			Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;

			// List all available atlases
			Dictionary<string, string> oAvailableAnimationClips = rAssetTable.GetAllClipNames( );
			if( oAvailableAnimationClips.Count != 0 )
			{
				oPopupMenu.AddSeparator( "" );

				// Add an item menu for each ready to use atlas
				foreach( KeyValuePair<string, string> rAnimationClipNameGUIDPair in oAvailableAnimationClips )
				{
					oPopupMenu.AddItem( new GUIContent( rAnimationClipNameGUIDPair.Value ), false, ClipPopupCallback, rAnimationClipNameGUIDPair.Key );
				}
			}

			// Finally show up the menu
			oPopupMenu.ShowAsContext( );

		}

		return bHasChanged;
	}

	// Templated enum popup control for serialized object
	public static T SerializedEnumPopup<T>( SerializedSetting<T> a_rSerializedSetting, string a_rLabel = "" ) where T : struct
	{
		System.Enum eNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			eNewValue = EditorGUILayout.EnumPopup( a_rLabel, a_rSerializedSetting.Value as System.Enum );
		}

		T eResult = (T) System.Enum.Parse( typeof( T ), eNewValue.ToString( ) );

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = eResult;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return eResult;
	}

	// Displays a popup menu filled with all potential atlases for the given input textures
	// Displays all available atlases if a_rTexturesToContain is null
	// Manages the serialized settings update
	public static Uni2DTextureAtlas SerializedAtlasPopup( SerializedSetting<Uni2DTextureAtlas> a_rSerializedTextureAtlas, IEnumerable<string> a_rTextureGUIDsToContain = null )
	{
		Uni2DTextureAtlas rCurrentAtlas = a_rSerializedTextureAtlas.HasMultipleDifferentValues ? null : a_rSerializedTextureAtlas.Value;

		bool bSavedShowMixedValue = EditorGUI.showMixedValue;
		EditorGUI.showMixedValue  = a_rSerializedTextureAtlas.HasMultipleDifferentValues;
		{
			EditorGUI.BeginChangeCheck( );
			{
				rCurrentAtlas = Uni2DEditorGUIUtils.AtlasPopup( rCurrentAtlas, a_rTextureGUIDsToContain );
			}
			if( EditorGUI.EndChangeCheck( ) )
			{
				a_rSerializedTextureAtlas.Value = rCurrentAtlas;
			}
		}
		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return rCurrentAtlas;
	}
}
#endif
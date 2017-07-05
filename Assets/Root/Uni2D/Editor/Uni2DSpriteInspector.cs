#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using SpriteRenderMesh = Uni2DSprite.SpriteRenderMesh;
using PivotType        = Uni2DSprite.PivotType;
using CollisionType    = Uni2DSprite.CollisionType;
using PhysicsMode      = Uni2DSprite.PhysicsMode;

using BuildSettings          = Uni2DEditorSerializedSprite_BuildSettings;
using InteracteBuildSettings = Uni2DEditorSerializedSprite_InteractiveBuildSettings;

using AnimationGUIAction = Uni2DEditorGUIUtils.AnimationGUIAction;
using BoneEditMode       = Uni2DEditorSmoothBindingUtils.BoneEditMode;

/*
 * Uni2DSpriteInspector
 * 
 * Creates the Uni2DSprite inspector which allows to tweak and update the sprite build settings and
 * to regenerate the sprite(s) in consequence.
 * 
 * This inspector is a bit special because it handles "interactive" values
 * and "non-interactive" values:
 * _ Interactive values can be easily updated and need light/quick computations that can be done on-the-fly
 * _ Non-interactive values are harder to update (i.e. must regenerate the sprite) and need much more heavier/longer computations
 * 
 * Before Uni2D v2, because of the regeneration duration,
 * the user had to press a button to validate his settings and to regenerate the sprite.
 * This behaviour hints the user that the operation would be a bit "long" (~1.5s) but it was not very Unity-like.
 * 
 * Since Uni2D v2, the inspector tries to rebuild the sprite the best it can.
 * To avoid long delays when tweaking a sprite (or more sprites, via multi-selection),
 * the inspector manages a timer which is started when the user is typing
 * a non-interactive value. At each key press, the timer is reinitialized.
 * After a while (when the value is assumed to be "fully typed"),
 * the inspector inits the sprite regeneration.
 * 
 * Editor use only.
 * 
 */
[CustomEditor( typeof( Uni2DSprite ) )]
[CanEditMultipleObjects]
public class Uni2DSpriteInspector : Editor 
{
	private static string ms_GUIWarningMessagePrefabEdition          = "Prefab edition is restricted.\nOnly in-scene sprite objects can be edited.";
	private static string ms_GUIWarningMessageSmoothBindingInAnimationModeEdition = "Sprite can't be edited in animation mode.\nBone edition is still available.";
//	private static string ms_GUIWarningMessageSettingsChanged        = "Settings have been changed.\nPress \"Apply\" to save them.";

//	private static string ms_GUIDialogTitleUnappliedSettings   = "Unapplied sprite settings";
//	private static string ms_GUIDialogMessageUnappliedSettings = "Unapplied settings for '{0}'";
//	private static string ms_GUIDialogOKUnappliedSettings      = "Apply";
//	private static string ms_GUIDialogCancelUnappliedSettings  = "Revert";

//	private static string ms_GUIButtonLabelOK          = "Apply";
//	private static string ms_GUIButtonLabelCancel      = "Revert";
	private static string ms_GUILabelSpriteTexture     = "Sprite Texture";
	private static string ms_GUILabelSharedMaterial     = "Shared Material";
	private static string ms_GUILabelInfoTriangleCount = "Collider Triangles";
	private const string mc_GUISelectLabel      = "Select";
	private const float mc_GUISmallButtonWidth  = 80.0f;
	private const float mc_GUIMediumButtonWidth = 120.0f;

	// Editor window GUI
	private const string mc_oGUILabelScale            = "Sprite Scale";
	private const string mc_oGUILabelPivot            = "Pivot";
	private const string mc_oGUILabelCustomPivotPoint = "Custom pivot point";
	private const string mc_oGUILabelCollider         = "Advanced Collider Settings";
	private const string mc_oGUILabelAlphaCutOff      = "Alpha Cut Off";
	private const string mc_oGUILabelAccuracy         = "Edge Simplicity";
	private const string mc_oGUILabelExtrusion        = "Extrusion Depth";
	private const string mc_oGUILabelHoles            = "Polygonize Holes";

	private const string mc_oGUILabelBoneInfluenceFalloff = "Bone Influence Falloff";
	private const string mc_oGUILabelSkinQuality          = "Skin Quality";
	private const string mc_oGUILabelHorizontalSubDivs    = "Horizontal Sub Divs";
	private const string mc_oGUILabelVerticalSubDivs      = "Vertical Sub Divs";

	// Foldouts states
	private static bool ms_bGUIFoldoutPhysicMode = true;
	private static bool ms_bGUIFoldoutRenderMeshSettings = true;
	private static bool ms_bGUIFoldoutPivot = true;
	private static bool ms_bGUIFoldoutColliderSettings = false;
	private static bool ms_bGUIFoldoutAnimation = true;
	private static bool ms_bGUIFoldoutSkeletalSmoothBinding = true;

	// Used to prevent a unintended cleanup (in OnDisable) when the target Renderer has changed
	// (because Unity editor reloads the inspector)
	private static bool ms_bIgnoreInspectorChanges;

	private BuildSettings m_oBuildSettings                     = null;
	private InteracteBuildSettings m_oInteractiveBuildSettings = null;
	//private RegularSettings m_oRegularSettings                 = null;

	// Delay to regen when tweaking non interactive settings
	private static float ms_fRegenDelay = 0.75f;
	private float m_fTimeToRegenAt      = -1.0f;

	// Animation preview
	private Uni2DAnimationPlayer m_oAnimationPlayer = new Uni2DAnimationPlayer( );
	private long m_lLastTimeTicks;

	// Skeletal animation editor
	//private Uni2DEditorSmoothBindingGUI m_oSmoothBindingEditor = null;

	// Init inspector
	void OnEnable( )
	{
		m_oBuildSettings            = new BuildSettings( serializedObject );
		m_oInteractiveBuildSettings = new InteracteBuildSettings( serializedObject );

		this.SetupAnimationPlayers( );
		//m_oSmoothBindingEditor = new Uni2DEditorSmoothBindingGUI( (Uni2DSprite) this.target );
	}

	// Inspector cleanup
	void OnDisable( )
	{
		if( ms_bIgnoreInspectorChanges == false )
		{	
			// Stop timer and try to rebuild a last time if necessary
			this.StopRegenTimer( );

			RegenerateSpriteData( );

			//m_oSmoothBindingEditor.OnDisable( );
		}
	}

	// Inspector GUI draw pass
	public override void OnInspectorGUI()
	{
		// Update serialized representation of the target object(s)
		serializedObject.Update( );

		SerializedSetting<CollisionType> rSerializedCollisionType = m_oBuildSettings.serializedCollisionType;
		SerializedSetting<PhysicsMode> rSerializedPhysicsMode     = m_oBuildSettings.serializedPhysicsMode;
		SerializedSetting<Texture2D> rSerializedSpriteTexture     = m_oInteractiveBuildSettings.serializedTexture;
		SerializedSetting<string> rSerializedSpriteTextureGUID    = m_oInteractiveBuildSettings.serializedTextureGUID;
		SerializedSetting<Material> rSerializedSharedMaterial	  = m_oInteractiveBuildSettings.serializedSharedMaterial;
		SerializedSetting<SpriteRenderMesh> rSerializedSpriteRenderMesh = m_oBuildSettings.serializedRenderMesh;

		// Update animation player used in clip header
		m_oAnimationPlayer.Update( this.ComputeDeltaTime( ref m_lLastTimeTicks ) );

		ms_bIgnoreInspectorChanges = false;

		// Check persistence and editor mode
		bool bIsSelectionPersistent = Uni2DEditorUtils.IsThereAtLeastOnePersistentObject( serializedObject.targetObjects );
		bool bIsInAnimationMode     = AnimationUtility.InAnimationMode( );

		// Message not editable in animation mode
		if( bIsInAnimationMode )
		{
			EditorGUILayout.BeginVertical( );
			{
				EditorGUILayout.HelpBox( ms_GUIWarningMessageSmoothBindingInAnimationModeEdition, MessageType.Warning, true );
			}
			EditorGUILayout.EndVertical( );
		}

		SerializedSetting<bool> rSerializedIsPhysicsDirty = m_oBuildSettings.serializedIsPhysicsDirty;
		
		// Message not editable in resource
		if( bIsSelectionPersistent && bIsInAnimationMode == false )
		{
			EditorGUILayout.BeginVertical( );
			{
				EditorGUILayout.HelpBox( ms_GUIWarningMessagePrefabEdition, MessageType.Warning, true );
			}
			EditorGUILayout.EndVertical( );
		}

		// Can we edit?
		bool bForceUpdate;
		bool bNotEditable = bIsSelectionPersistent || bIsInAnimationMode;
		int iTargetCount  = targets.Length;

		EditorGUILayout.BeginVertical( );
		{
			EditorGUI.BeginDisabledGroup( bNotEditable );
			{
				///// Help message /////
				if( rSerializedPhysicsMode.HasMultipleDifferentValues == false && rSerializedCollisionType.HasMultipleDifferentValues == false )
				{
					EditorGUILayout.HelpBox( GetHelpMessage( rSerializedPhysicsMode.Value, rSerializedCollisionType.Value ), MessageType.Info, true );
				
					EditorGUILayout.Space( );
				}

				if( rSerializedSpriteTextureGUID.HasMultipleDifferentValues == false && rSerializedSpriteTextureGUID.Value == null )
				{
					EditorGUILayout.HelpBox( "The sprite has no texture. Settings have been reverted.", MessageType.Warning, true );
					EditorGUILayout.Space( );
				}
			}
			EditorGUI.EndDisabledGroup( );

			// Update physic?
			EditorGUI.BeginDisabledGroup( bIsInAnimationMode );
			{
				string oUpdatePhysicButtonLabel = "Force Update";
	
				if( rSerializedIsPhysicsDirty.HasMultipleDifferentValues == false && rSerializedIsPhysicsDirty.Value )
				{
					EditorGUILayout.HelpBox( "The texture has changed since the last physics computation. Press the \"Update Physics\" button to update the physics shape.",
						MessageType.Warning,
						true );
	
					oUpdatePhysicButtonLabel = "Update Physics";
				}
		
				EditorGUILayout.BeginHorizontal( );
				{
					GUILayout.FlexibleSpace( );
					bForceUpdate = GUILayout.Button( oUpdatePhysicButtonLabel, EditorStyles.miniButton );
				}
				EditorGUILayout.EndHorizontal( );
			}
			EditorGUI.EndDisabledGroup( );

			EditorGUI.BeginDisabledGroup( bNotEditable );
			{
				///// Sprite texture /////
				SerializedTexture2DField( rSerializedSpriteTextureGUID, ms_GUILabelSpriteTexture, false );

				if( rSerializedSpriteTextureGUID.IsDefined )
				{
					// Revert the texture and physics dirty setting
					// if no texture is selected
					if( string.IsNullOrEmpty( rSerializedSpriteTextureGUID.Value ) )
					{
						//rSerializedSpriteTexture.Revert( );
						rSerializedSpriteTextureGUID.Revert( );
						rSerializedIsPhysicsDirty.Revert( );
					}
					else // New texture set => mark the physic part of the sprite as dirty
					{
						//rSerializedSpriteTextureGUID.Value = Uni2DEditorUtils.GetUnityAssetGUID( rSerializedSpriteTexture.Value );
						rSerializedSpriteTexture.Value = null;
						rSerializedIsPhysicsDirty.Value = true;
					}
				}
				
				///// Atlas /////
				EditorGUILayout.BeginHorizontal( );
				{
					string[ ] rTextureGUIDsToContain = new string[ iTargetCount ];

					for( int iTargetIndex = 0; iTargetIndex < iTargetCount; ++iTargetIndex )
					{
						rTextureGUIDsToContain[ iTargetIndex ] = ( (Uni2DSprite) targets[ iTargetIndex ] ).SpriteSettings.textureContainer.GUID;
					}

					Uni2DTextureAtlas rSelectedAtlas;
					EditorGUI.BeginDisabledGroup( bNotEditable );
					{
						EditorGUILayout.PrefixLabel( "Use Atlas" );
						rSelectedAtlas = Uni2DEditorGUIUtils.SerializedAtlasPopup( m_oInteractiveBuildSettings.serializedAtlas, rTextureGUIDsToContain );
					}
					EditorGUI.EndDisabledGroup( );

					EditorGUI.BeginDisabledGroup( rSelectedAtlas == null );
					{
						if( GUILayout.Button( mc_GUISelectLabel, GUILayout.Width( mc_GUISmallButtonWidth ) ) )
						{
							EditorGUIUtility.PingObject( rSelectedAtlas.gameObject );
						}
					}
					EditorGUI.EndDisabledGroup( );

					if( m_oInteractiveBuildSettings.serializedAtlas.IsDefined && rSelectedAtlas != null && !rSelectedAtlas.Contains( rTextureGUIDsToContain ) )
					{
						bool bBuildAtlas = EditorUtility.DisplayDialog( "Uni2D",
							"The selected Uni2D atlas does not contain the sprite(s) texture(s)."
							+ " Adding them implies to rebuild the atlas and can be a long process.\n\n"
							+ "Do it anyway?",
							"Yes, Build Atlas",
							"Cancel" );

						if( bBuildAtlas )
						{
							rSelectedAtlas.AddTextures( rTextureGUIDsToContain );
							rSelectedAtlas.ApplySettings( true );
						}
						else
						{
							m_oInteractiveBuildSettings.serializedAtlas.Revert( );
							rSelectedAtlas = m_oInteractiveBuildSettings.serializedAtlas.Value;
						}
					}
				}
				EditorGUILayout.EndHorizontal( );
				
				EditorGUI.BeginDisabledGroup( m_oInteractiveBuildSettings.serializedAtlas.HasMultipleDifferentValues || m_oInteractiveBuildSettings.serializedAtlas.Value != null);
				{
					///// Shared Material /////
					SerializedMaterialField( rSerializedSharedMaterial, ms_GUILabelSharedMaterial );
				}
				EditorGUI.EndDisabledGroup( );
				
				///// Vertex color /////
				if( Application.isPlaying == false )
				{
					SerializedColorField( m_oInteractiveBuildSettings.serializedVertexColor, "Vertex Color" );
				}
				else
				{
					EditorGUI.showMixedValue = m_oInteractiveBuildSettings.serializedVertexColor.HasMultipleDifferentValues;
					{
						Color f4VertexColor;
						EditorGUI.BeginChangeCheck( );
						{
							f4VertexColor = EditorGUILayout.ColorField( "Vertex Color", m_oInteractiveBuildSettings.serializedVertexColor.Value );
						}
						if( EditorGUI.EndChangeCheck( ) )
						{
							foreach( Uni2DSprite rSprite in targets )
							{
								rSprite.VertexColor = f4VertexColor;
							}
						}
					}
					EditorGUI.showMixedValue = false;
				}
				///// Collider triangle count /////
				SerializedProperty rSerializedTriangleCount = serializedObject.FindProperty( "m_rSpriteData.colliderTriangleCount" );
				if( rSerializedTriangleCount != null )
				{
					EditorGUILayout.LabelField( ms_GUILabelInfoTriangleCount, rSerializedTriangleCount.hasMultipleDifferentValues ? "-" : rSerializedTriangleCount.intValue.ToString( ) );
				}

				SerializedSetting<float> rSerializedExtrusionDepth = m_oInteractiveBuildSettings.serializedExtrusionDepth;
				SerializedSetting<float> rSerializedSpriteScale    = m_oInteractiveBuildSettings.serializedSpriteScale;

				// Sprite settings
				EditorGUILayout.BeginVertical( );
				{
					// Scale
					EditorGUILayout.BeginHorizontal( );
					{
						SerializedFloatField( rSerializedSpriteScale, mc_oGUILabelScale );

						// Pixel perfect
						EditorGUI.BeginDisabledGroup( rSerializedSpriteScale.HasMultipleDifferentValues == false && rSerializedSpriteScale.Value == 1.0f );
						{
							if( GUILayout.Button( "Pixel Perfect", GUILayout.Width( mc_GUISmallButtonWidth ) ) )
							{
								rSerializedSpriteScale.Value = 1.0f;
							}
						}
						EditorGUI.EndDisabledGroup( );
					}
					EditorGUILayout.EndHorizontal( );
					
					///// Pivot /////
					// Type
					SerializedSetting<PivotType> rSerializedPivotType = m_oInteractiveBuildSettings.serializedPivotType;

					ms_bGUIFoldoutPivot = EditorGUILayout.Foldout( ms_bGUIFoldoutPivot, mc_oGUILabelPivot );
					if( ms_bGUIFoldoutPivot )
					{
						++EditorGUI.indentLevel;
						{
							Uni2DEditorGUIUtils.SerializedEnumPopup<PivotType>( rSerializedPivotType, "Position" );

							bool bDisablePivotCoordsField = rSerializedPivotType.HasMultipleDifferentValues
								|| rSerializedPivotType.Value != Uni2DSprite.PivotType.Custom;
	
							// Coords
							EditorGUI.BeginDisabledGroup( bDisablePivotCoordsField );
							{
								SerializedVector2Field( m_oInteractiveBuildSettings.serializedPivotCustomCoords, mc_oGUILabelCustomPivotPoint );
							}
							EditorGUI.EndDisabledGroup( );
						}
						--EditorGUI.indentLevel;

						EditorGUILayout.Space( );
					}

					bool bDisablePhysicGroup = rSerializedPhysicsMode.HasMultipleDifferentValues
						|| rSerializedPhysicsMode.Value == Uni2DSprite.PhysicsMode.NoPhysics;

					///// Render mesh /////
					ms_bGUIFoldoutRenderMeshSettings = EditorGUILayout.Foldout( ms_bGUIFoldoutRenderMeshSettings, "Sprite Mesh" );
					if( ms_bGUIFoldoutRenderMeshSettings )
					{
						++EditorGUI.indentLevel;
						{
			
							Uni2DEditorGUIUtils.SerializedEnumPopup<SpriteRenderMesh>( rSerializedSpriteRenderMesh, "Mesh" );
	
							if( rSerializedSpriteRenderMesh.HasMultipleDifferentValues == false )
							{
								switch( rSerializedSpriteRenderMesh.Value )
								{
									default:
										break;
	
									case SpriteRenderMesh.TextureToMesh:
									{
										SerializedSetting<bool> rSerializedUsePhysicBuildSettings = m_oBuildSettings.serializedUsePhysicBuildSettings;
										EditorGUI.BeginDisabledGroup( bDisablePhysicGroup == false && ( rSerializedUsePhysicBuildSettings.HasMultipleDifferentValues || rSerializedUsePhysicBuildSettings.Value ) );
										{
											EditorGUI.BeginChangeCheck( );
											{
												SerializedSlider( m_oBuildSettings.serializedRenderMeshAlphaCutOff, Uni2DEditorSpriteSettings.AlphaCutOffMin, Uni2DEditorSpriteSettings.AlphaCutOffMax, mc_oGUILabelAlphaCutOff );
												SerializedSlider( m_oBuildSettings.serializedRenderMeshPolygonizationAccuracy, Uni2DEditorSpriteSettings.PolygonizationAccuracyMin, Uni2DEditorSpriteSettings.PolygonizationAccuracyMax, mc_oGUILabelAccuracy );
											}
											if( EditorGUI.EndChangeCheck( ) )
											{
												this.StopRegenTimer( );
												this.StartRegenTimer( );
											}
										}
										EditorGUI.EndDisabledGroup( );
										
										EditorGUI.BeginDisabledGroup( bDisablePhysicGroup );
										{
											SerializedToggle( rSerializedUsePhysicBuildSettings, "Use Physics Settings" );
										}
										EditorGUI.EndDisabledGroup( );
									
										SerializedToggle( m_oBuildSettings.serializedRenderMeshPolygonizeHoles, mc_oGUILabelHoles );
									}
									break;
	
									case SpriteRenderMesh.Grid:
									{
										EditorGUI.BeginChangeCheck( );
										{
											SerializedSlider( m_oBuildSettings.serializedRenderMeshAlphaCutOff, Uni2DEditorSpriteSettings.AlphaCutOffMin, Uni2DEditorSpriteSettings.AlphaCutOffMax, mc_oGUILabelAlphaCutOff );
											SerializedSlider( m_oBuildSettings.serializedRenderMeshGridHorizontalSubDivs, Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMin, Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMax, mc_oGUILabelHorizontalSubDivs );
											SerializedSlider( m_oBuildSettings.serializedRenderMeshGridVerticalSubDivs, Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMin, Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMax, mc_oGUILabelVerticalSubDivs );
										}
										if( EditorGUI.EndChangeCheck( ) )
										{
											this.StopRegenTimer( );
											this.StartRegenTimer( );
										}
									}
									break;
								}
							}
						}
						--EditorGUI.indentLevel;

						EditorGUILayout.Space( );
					}

					///// Physics /////
					ms_bGUIFoldoutPhysicMode = EditorGUILayout.Foldout( ms_bGUIFoldoutPhysicMode, "Physics" );
					if( ms_bGUIFoldoutPhysicMode )
					{
						++EditorGUI.indentLevel;
						{
							// Mode
							Uni2DEditorGUIUtils.SerializedEnumPopup<PhysicsMode>( rSerializedPhysicsMode, "Physics Mode" );
	
							// Physic settings group
							EditorGUI.BeginDisabledGroup( bDisablePhysicGroup );
							{
								// Collision type
								Uni2DEditorGUIUtils.SerializedEnumPopup<CollisionType>( m_oBuildSettings.serializedCollisionType, "Collision Type" );
		
								// Is Kinematic
								bool bDisableIsKinematicToggle = rSerializedPhysicsMode.HasMultipleDifferentValues
									|| rSerializedPhysicsMode.Value != Uni2DSprite.PhysicsMode.Dynamic;
				
								EditorGUI.BeginDisabledGroup( bDisableIsKinematicToggle );
								{
									SerializedToggle( m_oInteractiveBuildSettings.serializedIsKinematic, "Is Kinematic" );
								}
								EditorGUI.EndDisabledGroup( );
								
								// Collider settings
								ms_bGUIFoldoutColliderSettings = EditorGUILayout.Foldout( ms_bGUIFoldoutColliderSettings, mc_oGUILabelCollider );
								if( ms_bGUIFoldoutColliderSettings )
								{
									++EditorGUI.indentLevel;
									{
										EditorGUI.BeginChangeCheck( );
										{
											// Alpha cut off
											SerializedSlider( m_oBuildSettings.serializedAlphaCutOff, Uni2DEditorSpriteSettings.AlphaCutOffMin, Uni2DEditorSpriteSettings.AlphaCutOffMax, mc_oGUILabelAlphaCutOff );	// Threshold or cut-out?
											// Polygonization accuracy
											SerializedSlider( m_oBuildSettings.serializedPolygonizationAccuracy, Uni2DEditorSpriteSettings.PolygonizationAccuracyMin, Uni2DEditorSpriteSettings.PolygonizationAccuracyMax, mc_oGUILabelAccuracy );
										}
										// If the user has typed/tweaked non-interactive values,
										// start the timer
										if( EditorGUI.EndChangeCheck( ) )
										{
											// Reset timer
											this.StopRegenTimer( );
											this.StartRegenTimer( );
										}
		
										// Extrusion depth
										SerializedFloatField( rSerializedExtrusionDepth, mc_oGUILabelExtrusion );
		
										// Collision type
										bool bDisablePolygonizeHoles = rSerializedCollisionType.HasMultipleDifferentValues
											|| rSerializedCollisionType.Value == Uni2DSprite.CollisionType.Convex;
		
										// Polygonize holes
										EditorGUI.BeginDisabledGroup( bDisablePolygonizeHoles );
										{
											SerializedToggle( m_oBuildSettings.serializedPolygonizeHoles, mc_oGUILabelHoles );
										}
										EditorGUI.EndDisabledGroup();
									}
									--EditorGUI.indentLevel;
								}
							}
							EditorGUI.EndDisabledGroup();
						}
						--EditorGUI.indentLevel;

						EditorGUILayout.Space( );
					}
				}
				EditorGUILayout.EndVertical( );

				// Sprite scale
				if( rSerializedSpriteScale.IsDefined )
				{
					rSerializedSpriteScale.Value = Mathf.Clamp( rSerializedSpriteScale.Value, Uni2DEditorSpriteSettings.ScaleMin, float.MaxValue );
				}

				// Extrusion depth
				if( rSerializedExtrusionDepth.IsDefined )
				{
					rSerializedExtrusionDepth.Value = Mathf.Clamp( rSerializedExtrusionDepth.Value, Uni2DEditorSpriteSettings.ExtrusionMin, float.MaxValue );
				}
			}
			EditorGUI.EndDisabledGroup( );

			///// Skeletal animation section /////
			ms_bGUIFoldoutSkeletalSmoothBinding = EditorGUILayout.Foldout( ms_bGUIFoldoutSkeletalSmoothBinding, "Skeletal Smooth Binding" );
			if( ms_bGUIFoldoutSkeletalSmoothBinding )
			{
				++EditorGUI.indentLevel;
				{
					SerializedSetting<float> rSerializedBoneInfluenceFalloff = m_oInteractiveBuildSettings.serializedBoneInfluenceFalloff;
					SerializedSetting<SkinQuality> rSerializedSkinQuality    = m_oInteractiveBuildSettings.serializedSkinQuality;
					
					EditorGUI.BeginDisabledGroup( bNotEditable );
					{
						Uni2DEditorGUIUtils.SerializedEnumPopup<SkinQuality>( rSerializedSkinQuality, mc_oGUILabelSkinQuality );
						SerializedFloatField( rSerializedBoneInfluenceFalloff, mc_oGUILabelBoneInfluenceFalloff );
						
						EditorGUILayout.BeginHorizontal( );
						{
							GUILayout.FlexibleSpace( );
							if( GUILayout.Button( "Open Skeletal Animation Editor", GUILayout.Width( 225.0f ) ) )
							{
								Uni2DEditorSmoothBindingWindow.CreateEditorWindow( );
							}
							GUILayout.FlexibleSpace( );
						}
						EditorGUILayout.EndHorizontal( );
					}
					EditorGUI.EndDisabledGroup( );

				}
				--EditorGUI.indentLevel;
	
				EditorGUILayout.Space( );
			}
			
			EditorGUI.BeginDisabledGroup( bNotEditable );
			{
				///// Animation section /////
				this.DisplaySpriteAnimationSection( );
			}
			EditorGUI.EndDisabledGroup( );
			
			EditorGUILayout.Space( );
		}
		EditorGUILayout.EndVertical( );

		if( Application.isPlaying == false )
		{
			// Retrieve inspector area
			// Handle drag'n'drop
			this.CheckAnimationClipDragAndDrop( GUILayoutUtility.GetLastRect( ) );
		}
		
		// Force repaint if animation player enabled
		if( m_oAnimationPlayer.Enabled )
		{
			this.Repaint( );
		}

		// Apply non-interactive settings
		// Check texture value
		if( rSerializedSpriteTextureGUID.HasMultipleDifferentValues == false && string.IsNullOrEmpty( rSerializedSpriteTextureGUID.Value ) )
		{	
			this.StopRegenTimer( );
			//m_oBuildSettings.RevertValues( );
			//m_oInteractiveBuildSettings.RevertValues( );
			//m_oRegularSettings.RevertValues( );
		}
		else
		{
			if( this.IsTimerStarted == false && m_oBuildSettings.AreValuesModified( ) )
			{
				this.StopRegenTimer( );
				//this.StartRegenTimer( );
				RegenerateSpriteData( );
				return;
				/*
				if( this.IsTimerStarted == false  )
				{
					this.StopRegenTimer( );
					RegenerateSpriteData( );
					return;
				}*/
			}
			
			if( m_oInteractiveBuildSettings.AreValuesModified( ) )
			{
				this.StopRegenTimer( );
				RegenerateSpriteInteractiveData( );
			}

			if( bForceUpdate )
			{
				RegenerateSpriteData( true );
			}
		}
	}

	////////// Timer methods //////////

	// Tells if the timer has started
	private bool IsTimerStarted
	{
		get
		{
			return m_fTimeToRegenAt != -1.0f;
		}
	}

	// Starts the timer
	private void StartRegenTimer( )
	{
		if( this.IsTimerStarted == false )
		{
			m_fTimeToRegenAt = Time.realtimeSinceStartup + ms_fRegenDelay;
			EditorApplication.delayCall += RegenerateSpriteDataDelegate;
		}
	}

	// Stops the timer
	private void StopRegenTimer( )
	{
		EditorApplication.delayCall -= RegenerateSpriteDataDelegate;
		m_fTimeToRegenAt = -1.0f;
	}

	// Timer delegate
	private void RegenerateSpriteDataDelegate( )
	{
		if( Time.realtimeSinceStartup >= m_fTimeToRegenAt )
		{
			this.StopRegenTimer( );
			this.RegenerateSpriteData( );
		}
		else
		{
			EditorApplication.delayCall += RegenerateSpriteDataDelegate;
		}
	}

	////////// Sprite building //////////

	// Regenerates the sprite data
	private void RegenerateSpriteData( bool a_bForceFullRegeneration = false )
	{
		if( a_bForceFullRegeneration || m_oBuildSettings.AreValuesModified( ) ) // Non interactive data update
		{
			ms_bIgnoreInspectorChanges = true;
			m_oBuildSettings.serializedIsPhysicsDirty.Value = true;

			// Apply ALL settings
			m_oBuildSettings.ApplyValues( );
			m_oInteractiveBuildSettings.ApplyValues( );
	
			serializedObject.ApplyModifiedProperties( );

			// Regenerate in a batch
			List<Uni2DSprite> oSpritesToRegenerate = new List<Uni2DSprite>( serializedObject.targetObjects.Length );
			foreach( Object rTargetObject in serializedObject.targetObjects )
			{
				oSpritesToRegenerate.Add( (Uni2DSprite) rTargetObject );
			}

			Uni2DEditorSpriteBuilderUtils.RegenerateSpritesInABatch( oSpritesToRegenerate, a_bForceFullRegeneration );
		}
	}

	// Regenerates sprite interactive data only
	private void RegenerateSpriteInteractiveData( bool a_bForce = false )
	{
		if( a_bForce || m_oInteractiveBuildSettings.AreValuesModified( ) )
		{
			ms_bIgnoreInspectorChanges = true;

			// Apply only interactive and regular settings
			m_oInteractiveBuildSettings.ApplyValues( );
			//m_oRegularSettings.ApplyValues( );
	
			serializedObject.ApplyModifiedProperties( );

			foreach( Object rTargetObject in serializedObject.targetObjects )
			{
				( (Uni2DSprite) rTargetObject ).RegenerateInteractiveData( );
			}
		}
	}

	////////// Custom GUI //////////

	// Get help message
	private string GetHelpMessage( Uni2DSprite.PhysicsMode a_ePhysicMode, Uni2DSprite.CollisionType a_eCollisionType )
	{
		string oHelpMessage = "";
		if(a_ePhysicMode == Uni2DSprite.PhysicsMode.NoPhysics)
		{
			oHelpMessage = "In no physics mode, there is no collider attached to the sprite.";
		}
		else if(a_ePhysicMode == Uni2DSprite.PhysicsMode.Static)
		{
			if(a_eCollisionType == Uni2DSprite.CollisionType.Convex)
			{
				oHelpMessage = "In static convex mode, the mesh collider does not respond to collisions (e.g. not a rigidbody) as a convex mesh.\n" +
						"Unity computes a convex hull if the mesh collider is not convex.";
			}
			else if(a_eCollisionType == Uni2DSprite.CollisionType.Concave)
			{
				oHelpMessage = "In static concave mode, mesh collider does not respond to collisions (e.g. not a rigidbody) as a concave mesh.\n" +
						"A mesh collider marked as concave only interacts with primitive colliders (boxes, spheres...) and convex meshes.";
			}
			else if(a_eCollisionType == Uni2DSprite.CollisionType.Compound)
			{
				oHelpMessage = "In static compound mode, mesh collider does not respond to collisions (e.g. not a rigidbody) as a concave mesh composed of small convex meshes.\n" +
						"It allows the collider to block any other collider at the expense of performances.";
			}
		}
		else if(a_ePhysicMode == Uni2DSprite.PhysicsMode.Dynamic)
		{
			if(a_eCollisionType == Uni2DSprite.CollisionType.Convex)
			{
				oHelpMessage = "In dynamic convex mode, mesh collider does respond to collisions (e.g. rigidbody) as a convex mesh.\n" +
						"Unity computes a convex hull if the mesh collider is not convex.";
			}
			else if(a_eCollisionType == Uni2DSprite.CollisionType.Concave)
			{
				oHelpMessage = "In dynamic concave mode, mesh collider does respond to collisions (e.g. rigidbody) as a concave mesh.\n" +
						"A mesh collider marked as concave only interacts with primitive colliders (boxes, spheres...).";
			}
			else if(a_eCollisionType == Uni2DSprite.CollisionType.Compound)
			{
				oHelpMessage = "In dynamic compound mode, mesh collider does respond to collisions (e.g. rigidbody) as a concave mesh composed of small convex meshes.\n" +
						"It allows the collider to interact with any other collider at the expense of performances.";
			}
		}
		
		return oHelpMessage;
	}

	///// Animation /////
	private void DisplaySpriteAnimationSection( )
	{
		/// Animation clip ///
		ms_bGUIFoldoutAnimation = EditorGUILayout.Foldout( ms_bGUIFoldoutAnimation, "Sprite Animation" );
		if( ms_bGUIFoldoutAnimation )
		{
			// The sprite animation object
			Uni2DSpriteAnimation rSpriteAnimation = ( (Uni2DSprite) target ).spriteAnimation;
			int iClipCount = rSpriteAnimation.ClipCount;
			bool bSingleEdit = ( targets.Length == 1 );
			
			SerializedProperty rSerializedProperty_PlayAutomatically = serializedObject.FindProperty( "spriteAnimation.playAutomatically" );
			SerializedProperty rSerializedProperty_StartClipIndex = serializedObject.FindProperty( "spriteAnimation.m_iStartClipIndex" );

			///// Sprite animation settings /////
			EditorGUI.BeginDisabledGroup( Application.isPlaying );
			{
				EditorGUILayout.BeginVertical( );
				{
					++EditorGUI.indentLevel;
					{
						int iStartClipIndex;
	
						// Play automatically
						EditorGUI.BeginChangeCheck( );
						{
							EditorGUILayout.PropertyField( rSerializedProperty_PlayAutomatically );
						}
						if( EditorGUI.EndChangeCheck( ) )
						{
							serializedObject.ApplyModifiedProperties( );
						}
	
						// Start clip
						//EditorGUI.BeginDisabledGroup( rSerializedProperty_PlayAutomatically.hasMultipleDifferentValues || rSerializedProperty_PlayAutomatically.boolValue == false );
						//{
							EditorGUI.BeginChangeCheck( );
							{
								EditorGUI.showMixedValue = rSerializedProperty_StartClipIndex.hasMultipleDifferentValues;
								{
									iStartClipIndex = EditorGUILayout.IntField( "Start Clip Index", rSerializedProperty_StartClipIndex.intValue );
								}
								EditorGUI.showMixedValue = false;
							}
							if( EditorGUI.EndChangeCheck( ) )
							{
								this.SetSpriteAnimationStartClip( iStartClipIndex );
								serializedObject.SetIsDifferentCacheDirty( );
							}
						//}
						//EditorGUI.EndDisabledGroup( );
	
					}
					--EditorGUI.indentLevel;
	
					if( bSingleEdit )
					{
						// Iterate clips
						for( int iClipIndex = 0; iClipIndex < iClipCount; ++iClipIndex )
						{
							// Clip
							Uni2DAnimationClip rAnimationClip = rSpriteAnimation.GetClipByIndex( iClipIndex );
	
							// Clip header
							AnimationGUIAction eAnimationClipHeaderAction;
			
							EditorGUILayout.BeginHorizontal( );
							{
								GUILayout.Space( 12.0f );
								EditorGUI.BeginChangeCheck( );
								{
									eAnimationClipHeaderAction = Uni2DEditorGUIUtils.DisplayCompactAnimationClipHeader( rAnimationClip, m_oAnimationPlayer, iClipIndex );
								}
								if( EditorGUI.EndChangeCheck( ) )
								{
									// Update animation preview player settings
									m_oAnimationPlayer.FrameRate = rAnimationClip.frameRate;
									m_oAnimationPlayer.WrapMode  = rAnimationClip.wrapMode;
								}
							}
							EditorGUILayout.EndHorizontal( );
			
							// Handle returned action (if any)
							switch( eAnimationClipHeaderAction )
							{
								// No action
								case AnimationGUIAction.None:
								default:
									break;
								
								case AnimationGUIAction.MoveUp:
								{
									if( iClipIndex != 0 )
									{
										rSpriteAnimation.SwapClips( iClipIndex, iClipIndex - 1 );
	
										EditorUtility.SetDirty( target );
									}
								}
								break;
								
								case AnimationGUIAction.MoveDown:
								{
									if( iClipIndex != iClipCount - 1 )
									{
										rSpriteAnimation.SwapClips( iClipIndex, iClipIndex + 1 );
	
										EditorUtility.SetDirty( target );
									}
								}
								break;
								
								case AnimationGUIAction.Close:
								{
									rSpriteAnimation.RemoveClip( iClipIndex );
									--iClipIndex;
									--iClipCount;
	
									EditorUtility.SetDirty( target );
								}
								break;
							}	// End of eAnimationFrameAction handling
						} // End for clip
	
						///// Add / new clip /////
						EditorGUILayout.BeginHorizontal( );
						{
							// Add menu
							Uni2DAnimationClip rAddedAnimationClip;
	
							GUILayout.FlexibleSpace( );
							if( Uni2DEditorGUIUtils.AddClipPopup( "Add Animation Clip", out rAddedAnimationClip, GUILayout.Width( 225.0f ) ) )
							{
								// Add to sprite animation
								( (Uni2DSprite) target ).spriteAnimation.AddClip( rAddedAnimationClip );
								EditorUtility.SetDirty( target );
							}
		
							GUILayout.FlexibleSpace( );
						}
						EditorGUILayout.EndHorizontal( );
	
					}	// End targets.length == 1
				}
				EditorGUILayout.EndVertical( );
			}
			EditorGUI.EndDisabledGroup( );
		}
		// End animation	
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
		m_oAnimationPlayer.onAnimationNewFrameEvent += this.ForceRepaint;
		m_oAnimationPlayer.onAnimationInactiveEvent += this.RestartAnimationPlayer;
		m_oAnimationPlayer.Play( ( (Uni2DSprite) target ).spriteAnimation.Clip );
		m_oAnimationPlayer.Stop( false );

		ComputeDeltaTime( ref m_lLastTimeTicks );
	}

	private void ForceRepaint( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		this.Repaint( );
	}

	private void RestartAnimationPlayer( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		a_rAnimationPlayer.Play( ( (Uni2DSprite) target ).spriteAnimation.Clip );
		a_rAnimationPlayer.Stop( false );
	}

	////////// Animation clip utils //////////

	// Handles animation clips drag'n'drops
	private void CheckAnimationClipDragAndDrop( Rect a_rDropAreaRect )
	{
		// Handle drag and drop only if inspecting a single element
		if( targets.Length == 1 )
		{
			Event rCurrentEvent = Event.current;
			EventType rCurrentEventType = rCurrentEvent.type;
			Vector2 f2MousePos = rCurrentEvent.mousePosition;
	
			if( a_rDropAreaRect.Contains( f2MousePos ) )
			{
				bool bShouldAccept = false;
	
				foreach( Object rObject in DragAndDrop.objectReferences )
				{
					if( ( rObject is GameObject ) && ( (GameObject) rObject ).GetComponent<Uni2DAnimationClip>( ) != null )
					{
						bShouldAccept = true;
						break;
					}
				}
	
				if( bShouldAccept )
				{
					if( rCurrentEventType == EventType.DragPerform )
					{
						DragAndDrop.AcceptDrag( );
						rCurrentEvent.Use( );
	
						DragAndDrop.activeControlID = 0;
	
						Uni2DSpriteAnimation rSpriteAnimation = ( (Uni2DSprite) target ).spriteAnimation;
	
						foreach( Object rObject in DragAndDrop.objectReferences )
						{
							if( rObject is GameObject )
							{
								Uni2DAnimationClip rAnimationClip = ( (GameObject) rObject ).GetComponent<Uni2DAnimationClip>( );
								if( rAnimationClip != null )
								{
									rSpriteAnimation.AddClip( rAnimationClip );
									EditorUtility.SetDirty( target );
								}
							}
						}
					}
					else if( rCurrentEventType == EventType.DragUpdated )
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
				}
				else
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
				}
			}
		}
	}

	/*
	private void ApplyAnimationClipGlobalAtlas( Uni2DTextureAtlas a_rGlobalAtlas )
	{
		if( targets.Length == 1 )
		{
			Uni2DSpriteAnimation rSpriteAnimation = ( (Uni2DSprite) target ).spriteAnimation;
			Uni2DAnimationClip[ ] rAnimationClips = rSpriteAnimation.Clips;

			for( int iClipIndex = 0, iClipCount = rSpriteAnimation.ClipCount; iClipIndex < iClipCount; ++iClipIndex )
			{
				Uni2DAnimationClip rAnimationClip = rAnimationClips[ iClipIndex ];
				rAnimationClip.globalAtlas = a_rGlobalAtlas;
				
				for( int iFrameIndex = 0, iFrameCount = rAnimationClip.FrameCount; iFrameIndex < iFrameCount; ++iFrameIndex )
				{
					rAnimationClip.frames[ iFrameIndex ].atlas = a_rGlobalAtlas;
				}
	
				EditorUtility.SetDirty( rAnimationClip );
			}
		}
	}
	*/

	private void SetSpriteAnimationStartClip( int a_iStartClipIndex )
	{
		for( int iTargetIndex = 0, iTargetCount = targets.Length; iTargetIndex < iTargetCount; ++iTargetIndex )
		{
			Uni2DSpriteAnimation rSpriteAnimation = ( (Uni2DSprite) targets[ iTargetIndex ] ).spriteAnimation;
			rSpriteAnimation.StartClipIndex = a_iStartClipIndex;
			EditorUtility.SetDirty( targets[ iTargetIndex ] );
		}
	}

	////////// Serialized controls //////////

	// Templated object field control for serialized object
	//public void SerializedObjectField<T>( SerializedSetting<T> a_rSerializedSetting, string a_rLabel = "", bool a_bAllowSceneObject = false ) where T : Object
	public void SerializedTexture2DField( SerializedSetting<string> a_rSerializedTextureGUID, string a_rLabel = "", bool a_bAllowSceneObject = false )
	{
		Texture2D rObject;
		
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedTextureGUID.HasMultipleDifferentValues;
			rObject = Uni2DEditorUtils.GetAssetFromUnityGUID<Texture2D>( a_rSerializedTextureGUID.Value );
			rObject = (Texture2D) EditorGUILayout.ObjectField( a_rLabel, rObject, typeof( Texture2D ), a_bAllowSceneObject );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedTextureGUID.Value = Uni2DEditorUtils.GetUnityAssetGUID( rObject );
		}
			
		EditorGUI.showMixedValue = bSavedShowMixedValue;
	}
	
	// Templated object field control for serialized object
	//public void SerializedObjectField<T>( SerializedSetting<T> a_rSerializedSetting, string a_rLabel = "", bool a_bAllowSceneObject = false ) where T : Object
	public void SerializedMaterialField(SerializedSetting<Material> a_rSerializedMaterial, string a_rLabel = "")
	{
		Material rObject;
		
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedMaterial.HasMultipleDifferentValues;
			rObject = (Material) EditorGUILayout.ObjectField( a_rLabel, a_rSerializedMaterial.Value, typeof( Material ), false );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedMaterial.Value = rObject;
		}
			
		EditorGUI.showMixedValue = bSavedShowMixedValue;
	}

	// Slider control for serialized object
	public void SerializedSlider( SerializedSetting<float> a_rSerializedSetting, float a_fMinValue, float a_fMaxValue, string a_rLabel = "" )
	{
		float fNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			fNewValue = EditorGUILayout.Slider( a_rLabel, a_rSerializedSetting.Value, a_fMinValue, a_fMaxValue );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = fNewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;
	}

	// Slider control for serialized object
	public void SerializedSlider( SerializedSetting<int> a_rSerializedSetting, int a_iMinValue, int a_iMaxValue, string a_rLabel = "" )
	{
		int iNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			iNewValue = EditorGUILayout.IntSlider( a_rLabel, a_rSerializedSetting.Value, a_iMinValue, a_iMaxValue );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = iNewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;
	}

	// Toggle control for serialized object
	public void SerializedToggle( SerializedSetting<bool> a_rSerializedSetting, string a_rLabel = "" )
	{
		bool bNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			bNewValue = EditorGUILayout.Toggle( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = bNewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;
	}

	// Float field control for serialized object
	public float SerializedFloatField( SerializedSetting<float> a_rSerializedSetting, string a_rLabel = "" )
	{
		float fNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			fNewValue = EditorGUILayout.FloatField( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = fNewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return fNewValue;
	}

	// Int field control for serialized object
	public int SerializedIntField( SerializedSetting<int> a_rSerializedSetting, string a_rLabel = "" )
	{
		int iNewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			iNewValue = EditorGUILayout.IntField( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = iNewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return iNewValue;
	}

	// Color field control for serialized object
	public Color SerializedColorField( SerializedSetting<Color> a_rSerializedSetting, string a_rLabel = "" )
	{
		Color f4NewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			f4NewValue = EditorGUILayout.ColorField( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = f4NewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return f4NewValue;
	}
	
	// Vector2 field control for serialized object
	public Vector2 SerializedVector2Field( SerializedSetting<Vector2> a_rSerializedSetting, string a_rLabel = "" )
	{
		Vector2 f2NewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			f2NewValue = EditorGUILayout.Vector2Field( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = f2NewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return f2NewValue;
	}

	// Vector3 field control for serialized object
	public Vector3 SerializedVector3Field( SerializedSetting<Vector3> a_rSerializedSetting, string a_rLabel = "" )
	{
		Vector3 f3NewValue;
		bool bSavedShowMixedValue = EditorGUI.showMixedValue;

		EditorGUI.BeginChangeCheck( );
		{
			EditorGUI.showMixedValue = a_rSerializedSetting.HasMultipleDifferentValues;
			f3NewValue = EditorGUILayout.Vector3Field( a_rLabel, a_rSerializedSetting.Value );
		}

		if( EditorGUI.EndChangeCheck( ) )
		{
			a_rSerializedSetting.Value = f3NewValue;
		}

		EditorGUI.showMixedValue = bSavedShowMixedValue;

		return f3NewValue;
	}
}
#endif
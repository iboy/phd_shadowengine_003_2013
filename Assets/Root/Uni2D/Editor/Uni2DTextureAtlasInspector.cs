#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using AtlasSize = Uni2DTextureAtlas.AtlasSize;

[CustomEditor(typeof(Uni2DTextureAtlas))]
public class Uni2DTextureAtlasInspector : Editor 
{
	private static bool ms_bTexturesFoldout = true;
	private static bool ms_bGeneratedMaterialsFoldout = true;
	private static bool ms_bGeneratedTexturesFoldout = true;

	private static Material[ ] ms_oAtlasMaterials = null;
	private static Texture2D[ ] ms_oAtlasTextures = null;

	private static Queue<Uni2DTextureAtlas> ms_oAtlasesWithUnappliedSettings = new Queue<Uni2DTextureAtlas>( );

	// On enable
	void OnEnable( )
	{
		Uni2DTextureAtlas rAtlas = target as Uni2DTextureAtlas;

		rAtlas.RevertSettings( );

		ms_oAtlasMaterials = rAtlas.GetAllMaterials( );
		ms_oAtlasTextures = rAtlas.GetAllAtlasTextures( );
	}
	
	// On destroy
	void OnDisable( )
	{
		Uni2DTextureAtlas rAtlas = target as Uni2DTextureAtlas;
		
		if( rAtlas.UnappliedSettings )
		{
			ms_oAtlasesWithUnappliedSettings.Enqueue( rAtlas );

			EditorApplication.delayCall += Uni2DTextureAtlasInspector.AskAboutUnappliedSettings;
		}
	}

	private static void AskAboutUnappliedSettings( )
	{
		if( ms_oAtlasesWithUnappliedSettings.Count > 0 )
		{
			Uni2DTextureAtlas rAtlas = ms_oAtlasesWithUnappliedSettings.Dequeue( );
			
			bool bApply = EditorUtility.DisplayDialog( "Unapplied atlas settings",
				"Unapplied settings for '"+ rAtlas.name + "'",
				"Apply",
				"Revert" );
	
			if( bApply )
			{
				Uni2DTextureAtlasInspector.ApplySettings( rAtlas );
			}
			else
			{
				rAtlas.RevertSettings( );
			}
		}
	}
	
	// On inspector gui
	public override void OnInspectorGUI( )
	{	
		Uni2DTextureAtlas rAtlas = target as Uni2DTextureAtlas;
		
		EditorGUIUtility.LookLikeInspector( );

		// Material override
		EditorGUILayout.BeginVertical( );
		{
			rAtlas.materialOverride = (Material) EditorGUILayout.ObjectField( "Material Override", rAtlas.materialOverride, typeof( Material ), false );
			rAtlas.maximumAtlasSize = (AtlasSize) EditorGUILayout.EnumPopup( "Maximum Atlas Size", rAtlas.maximumAtlasSize );

			rAtlas.padding = EditorGUILayout.IntField( "Padding", rAtlas.padding );
			rAtlas.padding = Mathf.Abs( rAtlas.padding );

			// Texture to pack list
			// Custom list GUI: displays Texture2D objects, handles asset GUIDs		
			serializedObject.Update( );
			SerializedProperty rSerializedProperty_Textures = serializedObject.FindProperty( "textures" );
			int iContainerIndex = 0;

			EditorGUILayout.Space( );

	    	while( true )
			{
				string oPropertyPath = rSerializedProperty_Textures.propertyPath;
				string oPropertyName = rSerializedProperty_Textures.name;
				bool bIsTextureContainer = oPropertyPath.Contains( "textures" );
	
				// Indent
				EditorGUI.indentLevel = rSerializedProperty_Textures.depth;
				
				if( bIsTextureContainer )
				{
					if( oPropertyName == "textures" )
					{
						GUIContent oGUIContentTexturesLabel = new GUIContent( "Textures" );
						Rect rFoldoutRect   = GUILayoutUtility.GetRect( oGUIContentTexturesLabel, EditorStyles.foldout );
						Event rCurrentEvent = Event.current;

						switch( rCurrentEvent.type )
						{
							// Drag performed
							case EventType.DragPerform:
							{
								// Check if dragged objects are inside the foldout rect
								if( rFoldoutRect.Contains( rCurrentEvent.mousePosition ) )
								{
									// Accept and use the event
									DragAndDrop.AcceptDrag( );
									rCurrentEvent.Use( );

									EditorGUIUtility.hotControl = 0;
									DragAndDrop.activeControlID = 0;

									// Add the textures to the current list
									foreach( Object rDraggedObject in DragAndDrop.objectReferences )
									{
										if( rDraggedObject is Texture2D )
										{
											int iCurrentSize = rSerializedProperty_Textures.arraySize;
											++rSerializedProperty_Textures.arraySize;
											SerializedProperty rSerializedProperty_Data = rSerializedProperty_Textures.GetArrayElementAtIndex( iCurrentSize );
											rSerializedProperty_Data = rSerializedProperty_Data.FindPropertyRelative( "m_oTextureGUID" );

											rSerializedProperty_Data.stringValue = rDraggedObject != null
												? Uni2DEditorUtils.GetUnityAssetGUID( (Texture2D) rDraggedObject )
												: null;
										}
									}
								}
							}
							break;

							case EventType.DragUpdated:
							{
								if( rFoldoutRect.Contains( rCurrentEvent.mousePosition ) )
								{
									DragAndDrop.visualMode = DragAndDropVisualMode.Copy;						
								}
							}
							break;
						}
						EditorGUI.indentLevel = 0;
						ms_bTexturesFoldout = EditorGUI.Foldout( rFoldoutRect, ms_bTexturesFoldout, oGUIContentTexturesLabel );
					}
					else if( oPropertyName == "data" )
					{
						SerializedProperty rSerializedProperty_TextureGUID = rSerializedProperty_Textures.FindPropertyRelative( "m_oTextureGUID" );
						Texture2D rTexture = Uni2DEditorUtils.GetAssetFromUnityGUID<Texture2D>( rSerializedProperty_TextureGUID.stringValue );
		
						EditorGUI.BeginChangeCheck( );
						{
							rTexture = (Texture2D) EditorGUILayout.ObjectField( "Element " + iContainerIndex, rTexture, typeof( Texture2D ), false );
							++iContainerIndex;
						}
						if( EditorGUI.EndChangeCheck( ) )
						{
							rSerializedProperty_TextureGUID.stringValue = rTexture != null
								? Uni2DEditorUtils.GetUnityAssetGUID( rTexture )
								: null;
						}
					}
					else 
					{
						// Default draw of the property field
						EditorGUILayout.PropertyField( rSerializedProperty_Textures );
					}
				}
	
				if( rSerializedProperty_Textures.NextVisible( ms_bTexturesFoldout ) == false )
				{
					break;
				}
			}
	
			serializedObject.ApplyModifiedProperties( );

			EditorGUILayout.Space( );

			EditorGUI.indentLevel = 0;

			///// Generated assets section /////
	
			// Materials
			ms_bGeneratedMaterialsFoldout = EditorGUILayout.Foldout( ms_bGeneratedMaterialsFoldout, "Generated Materials" );
			if( ms_bGeneratedMaterialsFoldout )
			{
				++EditorGUI.indentLevel;
				{
					if( ms_oAtlasMaterials.Length > 0 )
					{
						foreach( Material rAtlasMaterial in ms_oAtlasMaterials )
						{
							EditorGUILayout.BeginHorizontal( );
							{
								GUILayout.Space( 16.0f );
								if( GUILayout.Button( EditorGUIUtility.ObjectContent( rAtlasMaterial, typeof( Material ) ), EditorStyles.label, GUILayout.ExpandWidth( false ), GUILayout.MaxWidth( 225.0f ), GUILayout.MaxHeight( 16.0f ) ) )
								{
									EditorGUIUtility.PingObject( rAtlasMaterial );
								}
							}
							EditorGUILayout.EndHorizontal( );
						}
					}
					else
					{
						EditorGUILayout.PrefixLabel( "(None)" );
					}
				}
				--EditorGUI.indentLevel;
			}

			EditorGUILayout.Space( );

			// Atlas textures
			ms_bGeneratedTexturesFoldout = EditorGUILayout.Foldout( ms_bGeneratedTexturesFoldout, "Generated Textures" );
			if( ms_bGeneratedTexturesFoldout )
			{
				++EditorGUI.indentLevel;
				{
					if( ms_oAtlasTextures.Length > 0 )
					{
						foreach( Texture2D rAtlasTexture in ms_oAtlasTextures )
						{
							EditorGUILayout.BeginHorizontal( );
							{
								GUILayout.Space( 16.0f );
								if( GUILayout.Button( EditorGUIUtility.ObjectContent( rAtlasTexture, typeof( Texture2D ) ), EditorStyles.label, GUILayout.ExpandWidth( false ), GUILayout.MaxWidth( 225.0f ), GUILayout.MaxHeight( 16.0f ) ) )
								{
									EditorGUIUtility.PingObject( rAtlasTexture );
								}
							}
							EditorGUILayout.EndHorizontal( );
						}
					}
					else
					{
						EditorGUILayout.PrefixLabel( "(None)" );
					}
				}
				--EditorGUI.indentLevel;
			}

			bool bUnappliedSettings = rAtlas.UnappliedSettings;
			EditorGUI.BeginDisabledGroup( bUnappliedSettings == false );
			{
				// Apply/Revert
				EditorGUILayout.BeginHorizontal( );
				{
					if(GUILayout.Button( "Apply" ) )
					{
						this.ApplySettings( );
					}
					
					if( GUILayout.Button( "Revert" ) )
					{
						rAtlas.RevertSettings( );
					}
				}
				EditorGUILayout.EndHorizontal( );
			}
			EditorGUI.EndDisabledGroup();

			// Generate
			if( GUILayout.Button( "Force atlas regeneration" ) )
			{
				this.ApplySettings( );
			}
		}
		EditorGUILayout.EndVertical( );
	}

	private void ApplySettings( )
	{
		if( target != null )
		{
			Uni2DTextureAtlas rAtlas = (Uni2DTextureAtlas) target;
			
			Uni2DTextureAtlasInspector.ApplySettings( rAtlas );

			ms_oAtlasMaterials = rAtlas.GetAllMaterials( );
			ms_oAtlasTextures  = rAtlas.GetAllAtlasTextures( );
		}
	}

	private static void ApplySettings( Uni2DTextureAtlas a_rAtlas )
	{
		if( a_rAtlas.ApplySettings( ) == false )
		{
			EditorUtility.DisplayDialog( "Uni2D Texture Atlasing",
				"Uni2D could not pack all the specified textures into '" + a_rAtlas.name + "'.\n"
				+ "Please check the maximum allowed atlas size and the input textures.",
				"OK" );
		}
	}
}
#endif
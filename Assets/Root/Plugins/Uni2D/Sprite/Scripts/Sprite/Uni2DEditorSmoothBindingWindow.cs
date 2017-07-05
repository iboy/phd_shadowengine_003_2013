#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using BoneEditMode = Uni2DEditorSmoothBindingUtils.BoneEditMode;

public class Uni2DEditorSmoothBindingWindow : EditorWindow
{
	private const string mc_oWindowTitle = "Skeletal Animation Editor";

	private const string mc_oNoneModeHelp   = "Please first select an Uni2D sprite and click \"Posing\" to skin it.";
	private const string mc_oNoneModeHelpAndAdvice = mc_oNoneModeHelp + "\nUsing a grid as a render mesh is strongly advised to obtain a beautiful skinning.";
	
	private const string mc_oPosingModeHelp = "Left click:\n\tOn empty space: creates a new bone\n" +
		"\tOn a bone / bone articulation: selects the bone\n" +
		"\tOn inner disc: drag to create a new bone\n" +
		"\tOn outer disc: drag to move the bone / ALT + drag to move the bone and its hierarchy\n\n" +
		"Right click:\n\tOn a bone articulation: deletes bone and its hierarchy\n\tOn a bone: breaks the bone\n\n" +
		"Backspace while creating a bone chain:\n\tDeletes the latest created bone in the chain\n\n" +
		"Escape:\n\tUnselects current bone / exits posing mode if nothing selected";

	private const string mc_oAnimModeHelp = "Left click:\n\tSelects bone\n\n" +
		"Drag:\n\tRotates the selected bone\n\n" +
		"Escape:\n\tUnselects current bone / exits anim mode if nothing selected";

	private static bool ms_bHideHelp = false;

	private Uni2DEditorSmoothBindingGUI m_rSmoothBindingGUI = null;

	private Vector2 m_f2ScrollPos = Vector2.zero;

	private Uni2DSprite CurrentSprite
	{
		get
		{
			Transform rSelection = Selection.activeTransform;
			//return rSelection != null ? rSelection.GetComponent<Uni2DSprite>( ) : null;
			if(rSelection == null)
			{
				return null;
			}
		
			Transform rSelectionParent = rSelection;	
			while(rSelectionParent != null)
			{
				Uni2DSprite rSprite = rSelectionParent.GetComponent<Uni2DSprite>();
				if(rSprite != null)
				{
					return rSprite;
				}
				
				rSelectionParent = rSelectionParent.parent;
			}
			
			return null;
		}
		
		set
		{
			Selection.activeTransform = value != null ? value.transform : null;
		}
	}

	[MenuItem( "Uni2D/" + mc_oWindowTitle )]
	public static void CreateEditorWindow( )
	{
		EditorWindow.GetWindow<Uni2DEditorSmoothBindingWindow>( false, mc_oWindowTitle, false );
	}

	void OnEnable( )
	{
		m_rSmoothBindingGUI = new Uni2DEditorSmoothBindingGUI( this.CurrentSprite );
		SceneView.onSceneGUIDelegate += m_rSmoothBindingGUI.OnSceneGUI;
		
		EditorApplication.delayCall += this.SelectionPolling;
		
		this.autoRepaintOnSceneChange = true;
	}

	void OnDisable( )
	{
		SceneView.onSceneGUIDelegate -= m_rSmoothBindingGUI.OnSceneGUI;
		EditorApplication.delayCall -= this.SelectionPolling;

		m_rSmoothBindingGUI.OnDisable( );
	}

	void OnDestroy( )
	{
		SceneView.onSceneGUIDelegate -= m_rSmoothBindingGUI.OnSceneGUI;
		EditorApplication.delayCall -= this.SelectionPolling;
	}

	void OnSelectionChange( )
	{
		Uni2DSprite rLastUsedSprite = Uni2DEditorSmoothBindingGUI.CurrentSprite;
		Uni2DSprite rCurrentSprite = CurrentSprite;
		if(rLastUsedSprite != rCurrentSprite)
		{
			m_rSmoothBindingGUI.Reset( rCurrentSprite, true );
		}
		this.Repaint( );
	}

	void OnHierarchyChange( )
	{
		this.SelectionPolling( false );
	}

	void OnGUI( )
	{
		BoneEditMode eCurrentMode = Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode;
		bool bPosingMode = eCurrentMode == BoneEditMode.Posing;
		bool bAnimMode   = eCurrentMode == BoneEditMode.Anim;
		string oHelpMessage = null;
		MessageType eHelpMessageType = MessageType.Info;

		switch( eCurrentMode )
		{
			default:
			case BoneEditMode.None:
			{
				Uni2DSprite rSprite = this.CurrentSprite;
			
				if( rSprite != null && rSprite.SpriteSettings.renderMesh != Uni2DSprite.SpriteRenderMesh.Grid )
				{
					oHelpMessage = mc_oNoneModeHelpAndAdvice;
					eHelpMessageType = MessageType.Warning;
				}
				else
				{
					oHelpMessage = mc_oNoneModeHelp;
				}
			}
			break;

			case BoneEditMode.Posing:	oHelpMessage = mc_oPosingModeHelp; break;
			case BoneEditMode.Anim:		oHelpMessage = mc_oAnimModeHelp; break;
		}

		m_f2ScrollPos = EditorGUILayout.BeginScrollView( m_f2ScrollPos, false, false );
		{
			EditorGUILayout.BeginVertical( );
			{
				
				EditorGUILayout.BeginHorizontal( );
				{
					// Posing button
					EditorGUI.BeginChangeCheck( );
					{
						EditorGUI.BeginDisabledGroup( Uni2DEditorSmoothBindingGUI.CanUsePosingMode == false );
						{
							bPosingMode = GUILayout.Toggle( bPosingMode, "Posing", EditorStyles.miniButtonLeft );
						}
						EditorGUI.EndDisabledGroup( );
					}
					if( EditorGUI.EndChangeCheck( ) )
					{
						Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = bPosingMode ? BoneEditMode.Posing : BoneEditMode.None;
					}
					
					// Anim button
					EditorGUI.BeginChangeCheck( );
					{
						EditorGUI.BeginDisabledGroup( Uni2DEditorSmoothBindingGUI.CanUseAnimMode == false );
						{
							bAnimMode = GUILayout.Toggle( bAnimMode, "Anim", EditorStyles.miniButtonRight );
						}
						EditorGUI.EndDisabledGroup( );
					}
					if( EditorGUI.EndChangeCheck( ) )
					{
						Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = bAnimMode ? BoneEditMode.Anim : BoneEditMode.None;
					}
				}
				EditorGUILayout.EndHorizontal( );
	
				EditorGUILayout.Space( );
	
				if( GUILayout.Button( ms_bHideHelp ? "Show Help" : "Hide Help", EditorStyles.miniButton ) )
				{
					ms_bHideHelp = !ms_bHideHelp;
				}
	
				if( ms_bHideHelp == false )
				{
					EditorGUILayout.HelpBox( oHelpMessage, eHelpMessageType );
				}
			}
			EditorGUILayout.EndVertical( );
		}
		EditorGUILayout.EndScrollView( );
	}

	private void SelectionPolling( )
	{
		this.SelectionPolling( true );
	}

	private void SelectionPolling( bool a_bDelayedCall )
	{
		Uni2DSprite rCurrentSprite = this.CurrentSprite;

		if( rCurrentSprite != Uni2DEditorSmoothBindingGUI.CurrentSprite )
		{
			m_rSmoothBindingGUI.Reset( rCurrentSprite, true );
			this.Repaint( );
		}

		if( a_bDelayedCall )
		{
			EditorApplication.delayCall += this.SelectionPolling;
		}
	}
}
#endif
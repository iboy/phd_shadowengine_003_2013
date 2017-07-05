#if UNITY_EDITOR

// Warning : Uni2D only supported on Unity 3.5.7 and higher
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
	#define BEFORE_UNITY_4_3
#else
	#define AFTER_UNITY_4_3
#endif

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;

using BoneEditMode   = Uni2DEditorSmoothBindingUtils.BoneEditMode;
using BoneHandle     = Uni2DEditorSmoothBindingUtils.BoneHandle;
using BoneEditorTool = Uni2DEditorSmoothBindingUtils.BoneEditorTool;

public class Uni2DEditorSmoothBindingGUI
{
	private static Uni2DSprite ms_rSprite = null;

	private Uni2DSmoothBindingBone m_rLastAddedBone   = null;
	private Uni2DSmoothBindingBone m_rBoneChainOrigin = null;
	private Vector2 m_f2MouseGUIOffset = Vector2.zero;
	private bool m_bIsDragging         = false;

	private static Uni2DSmoothBindingBone ms_rActiveBone = null;
	public static Uni2DSmoothBindingBone activeBone
	{
		get
		{
			return ms_rActiveBone;
		}
		
		set
		{
			if(ms_rActiveBone != value)
			{
				ms_rActiveBone = value;
				#if AFTER_UNITY_4_3
				if(AnimationMode.InAnimationMode())
				{
					if(ms_rActiveBone != null)
					{
						Transform rParent = ms_rActiveBone.transform.parent;
						if(rParent != null)
						{
							if(ms_rSprite != null && rParent == ms_rSprite.transform)
							{
								Selection.activeTransform = ms_rActiveBone.transform;
							}
							else
							{
								Selection.activeTransform = rParent;
							}
						}
					}
				}
				#endif
			}
		}
	}

	private static BoneEditMode ms_eBoneEditMode = BoneEditMode.None;
	private static BoneEditorTool ms_eEditorTool = BoneEditorTool.Select;
	private static Tool ms_ePreviousTool;
	private static ViewTool ms_ePreviousViewTool;

	public static Uni2DSprite CurrentSprite
	{
		get
		{
			return ms_rSprite;
		}
	}

	public static BoneEditMode CurrentBoneEditMode
	{
		get
		{
			return ms_eBoneEditMode;
		}
		set
		{
			if( value != BoneEditMode.None )
			{
				EditorWindow.FocusWindowIfItsOpen<SceneView>( );

				// Prevent to go in anim mode if the sprite is not skinned
				if( value == BoneEditMode.Anim && !CanUseAnimMode )
				{
					return;
				}
				else if( value == BoneEditMode.Posing )
				{
					if( !CanUsePosingMode )
					{
						return;
					}

					ms_rSprite.RestorePosePosition( );
				}

				switch( ms_eBoneEditMode )
				{
					case BoneEditMode.None:
					{
						// Save tool and set to none
						ms_ePreviousTool = Tools.current;

						// Tools.viewTool is prone to null exceptions
						try
						{
							ms_ePreviousViewTool = Tools.viewTool;
						}
						catch
						{
							ms_ePreviousViewTool = ViewTool.None;
						}
						
						Tools.current  = Tool.None;
						Tools.viewTool = ViewTool.None;					
					}
					break;

					case BoneEditMode.Posing:
					{
						// Update posing only when exiting posing mode
						ms_rSprite.UpdatePosing( );
					}
					break;
				
					default:
						break;
				}
				
			}
			else if( ms_eBoneEditMode != BoneEditMode.None )	// Restore only if exiting edit mode
			{
				// Restore saved tool
				Tools.current  = ms_ePreviousTool;
				Tools.viewTool = ms_ePreviousViewTool;
				
				if( ms_eBoneEditMode == BoneEditMode.Posing && ms_rSprite != null )
				{
					// Specify to clean skinning if the sprite needs it.
					// Can only do it in this case (exiting edition), because Unity considers components addition/deletion
					// as an update, reloads/resets inspectors and leads bone edit mode to be resetted too.
					ms_rSprite.UpdatePosing( true );
				}	
			}

			ms_eBoneEditMode = value;
			activeBone = null;
			SceneView.RepaintAll( );
		}
	}

	public static bool CanUseAnimMode
	{
		get
		{
			return ms_rSprite != null && ms_rSprite.Bones.Length > 0;
		}
	}

	public static bool CanUsePosingMode
	{
		get
		{
			return ms_rSprite != null && !AnimationUtility.InAnimationMode( );
		}
	}

	public Uni2DEditorSmoothBindingGUI( Uni2DSprite a_rSprite )
	{
		this.Reset( a_rSprite );
	}

	// Destructor
	public void OnDisable( )
	{
		Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;
	}

	public void Reset( Uni2DSprite a_rSprite, bool a_bKeepCurrentModeIfPossible = false )
	{
		m_rLastAddedBone   = null;
		m_rBoneChainOrigin = null;
		Uni2DEditorSmoothBindingGUI.activeBone = null;

		m_f2MouseGUIOffset = Vector2.zero;
		ms_eEditorTool = BoneEditorTool.Select;

		BoneEditMode eSavedEditMode = Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode;

		Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;

		ms_rSprite = a_rSprite;

		if( a_bKeepCurrentModeIfPossible )
		{
			Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = eSavedEditMode;
		}
	}

	public void OnSceneGUI( )
	{
		if( Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode != BoneEditMode.None && ms_rSprite != null )
		{
			// Current event
			Event rCurrentEvent = Event.current;
	
			// GUI setup: set the current context as receptive to mouse events (but not to keyboard focus)
			int iControlID = GUIUtility.GetControlID( ms_rSprite.GetHashCode( ), FocusType.Passive );
			if( rCurrentEvent.type == EventType.Layout )
			{
				HandleUtility.AddDefaultControl( iControlID );
			}
	
			/*
			// Check Alt key state
			bool bAltPressed = rCurrentEvent.alt;
			if( bAltPressed != m_bAltPressed )
			{
				this.Repaint( );
			}
	
			m_bAltPressed = bAltPressed;
			*/
	
			// If an edit mode is enabled, handle GUI interactions
			switch( Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode )
			{
				case BoneEditMode.Posing:
				{
					this.UpdatePoseEditGUI( rCurrentEvent );
					this.DrawPoseEditBoneHandles( rCurrentEvent );
				}
				break;
		
				case BoneEditMode.Anim:
				{
					this.UpdateAnimEditGUI( rCurrentEvent );
					//this.DrawAnimEditBoneHandles( rCurrentEvent );
				}
				break;
			}
		}
	}

	public void OnSceneGUI( SceneView a_rSceneView )
	{
		this.OnSceneGUI( );
	}

	private void UpdateAnimEditGUI( Event a_rEvent )
	{
		Vector2 f2MouseGUIPos = a_rEvent.mousePosition;

		if( a_rEvent.isMouse )
		{
			int iMouseButton = a_rEvent.button;

			switch( a_rEvent.type )
			{
				case EventType.MouseMove:
				{
					// Reset state
					m_bIsDragging = false;

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseMove

				case EventType.MouseDown:
				{
					// Left click
					if( iMouseButton == 0 )
					{
						// Reset drag state
						m_bIsDragging = false;

						// Pick bones in scene
						BoneHandle ePickedHandle;
						Uni2DSmoothBindingBone rNearestBone = Uni2DEditorSmoothBindingUtils.PickNearestBoneInAnimMode( ms_rSprite, f2MouseGUIPos, out ePickedHandle, null );

						// Selection
						if( ms_eEditorTool == BoneEditorTool.Select && rNearestBone != null )
						{
							// Change selection and editor tool
							Uni2DEditorSmoothBindingGUI.activeBone = rNearestBone;

							m_f2MouseGUIOffset = f2MouseGUIPos - HandleUtility.WorldToGUIPoint( rNearestBone.transform.position );
						}	// end if( editor tool == select )
					}

					// Consume mouse event
					a_rEvent.Use( );
				}
				break;	// end MouseDown

				case EventType.MouseDrag:
				{
					if( iMouseButton == 0 )
					{
						switch( ms_eEditorTool )
						{
							case BoneEditorTool.Move:
							{
								// Something dragged? MOVE IT
								if( Uni2DEditorSmoothBindingGUI.activeBone != null )
								{
									// We're moving an existing bone => register undo at first drag event
									if( m_bIsDragging == false )
									{
										Undo.SetSnapshotTarget( Uni2DEditorSmoothBindingGUI.activeBone, "Move Uni2D Bone" );
										Undo.CreateSnapshot( );
										Undo.RegisterSnapshot( );
									}
			
									// Move/drag along sprite plane
									Uni2DEditorSmoothBindingGUI.activeBone.RotateBoneAlongSpritePlane( f2MouseGUIPos - m_f2MouseGUIOffset );
								}
								m_bIsDragging = true;
							}
							break;

							case BoneEditorTool.Select:
							{
								// Dragging a bone in select mode == dragging on inner disc => add a child to active bone
								if( Uni2DEditorSmoothBindingGUI.activeBone != null && m_bIsDragging == false )
								{
									ms_eEditorTool = BoneEditorTool.Move;
									m_f2MouseGUIOffset = f2MouseGUIPos - HandleUtility.WorldToGUIPoint( Uni2DEditorSmoothBindingGUI.activeBone.transform.position );

									Uni2DEditorSmoothBindingGUI.activeBone.RotateBoneAlongSpritePlane( f2MouseGUIPos - m_f2MouseGUIOffset );
								}
								m_bIsDragging = true;
							}
							break;
						}
					}

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseDrag

				case EventType.MouseUp:
				{
					if( iMouseButton == 0 )
					{
						if( ms_eEditorTool == BoneEditorTool.Move )
						{
							Undo.ClearSnapshotTarget( );
							Undo.RegisterSnapshot( );
							ms_eEditorTool = BoneEditorTool.Select;
						}

						// Reset dragging state
						m_bIsDragging = false;
						m_f2MouseGUIOffset = Vector2.zero;						
					}	// end if( left button )
					else if( iMouseButton == 1 && ms_eEditorTool == BoneEditorTool.Move )	// Delete / stop bone creation
					{
						ms_eEditorTool = BoneEditorTool.Select;
					}	// End if( right button )

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseUp
			}	// End switch( event.type )
		}	// end if( mouse events )
		else if( a_rEvent.isKey && a_rEvent.type == EventType.keyDown )
		{
			switch( a_rEvent.keyCode )
			{
				case KeyCode.Escape:
				{
					switch( ms_eEditorTool )
					{
						case BoneEditorTool.Select:
						{
							if( Uni2DEditorSmoothBindingGUI.activeBone != null )
							{
								Uni2DEditorSmoothBindingGUI.activeBone = null;
							}
							else
							{
								Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;
							}
						}
						break;

						case BoneEditorTool.Move:
						{
							Undo.RestoreSnapshot( );
							ms_eEditorTool = BoneEditorTool.Select;
						}
						break;
					}
					a_rEvent.Use( );
				}
				break;	// End Escape
				
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
				{
					ms_eEditorTool = BoneEditorTool.Select;
					a_rEvent.Use( );
				}
				break;	// End Return / KeypadEnter
			}	// end switch( event.type )
		}	// end if( key down events )
		else
		{
			switch( a_rEvent.type )
			{
				case EventType.ValidateCommand:
				{
					if( a_rEvent.commandName == "Delete" || a_rEvent.commandName == "UndoRedoPerformed" )
					{
						a_rEvent.Use( );
					}
				}
				break;	// end ValidateCommand
					
				case EventType.ExecuteCommand:
				{
					if( a_rEvent.commandName == "Delete" )
					{
						a_rEvent.Use( );
					}
					else if( a_rEvent.commandName == "UndoRedoPerformed" )
					{
						Uni2DEditorSmoothBindingGUI.activeBone = null;
						ms_eEditorTool = BoneEditorTool.Select;
						//Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;
						a_rEvent.Use( );
					}
				}
				break;	// end ExecuteCommand
			}	// end switch( event.type )
		}	// end else
	}

	/*
	private void DrawAnimEditBoneHandles( Event a_rEvent )
	{
		// TODO
	}
	*/

	private void DrawPoseEditBoneHandles( Event a_rEvent )
	{
		Color oInnerBoneDiscColor  = Uni2DEditorPreferences.InnerBoneDiscHandleColor;
		Color oOuterBoneDiscColor = Uni2DEditorPreferences.OuterBoneDiscHandleColor;

		Color oInnerRootBoneDiscColor  = Uni2DEditorPreferences.InnerRootBoneDiscHandleColor;
		Color oOuterRootBoneDiscColor = Uni2DEditorPreferences.OuterRootBoneDiscHandleColor;

		Vector3 f3CameraForward = SceneView.currentDrawingSceneView.camera.transform.forward;
		
		BoneHandle eBoneHandle;
		Uni2DSmoothBindingBone rNearestBone = Uni2DEditorSmoothBindingUtils.PickNearestBoneArticulationInPosingMode( ms_rSprite, a_rEvent.mousePosition, out eBoneHandle, null );
		Uni2DSmoothBindingBone[ ] oBones = ms_rSprite.Bones.Except( new Uni2DSmoothBindingBone[ ]{ rNearestBone } ).ToArray( );
		//Transform rActiveBone = Uni2DEditorSmoothBindingGUI.activeBone;
		//Selection.activeTransform.GetComponentsInChildren<Transform>( false ).Except( new Transform[ ]{ rRootBone, rNearestBone } ).ToArray( );

		Transform rActiveBoneTransform = Uni2DEditorSmoothBindingGUI.activeBone != null ? Uni2DEditorSmoothBindingGUI.activeBone.transform : null;

		oInnerBoneDiscColor.a      = 0.2f;
		oOuterBoneDiscColor.a     = 0.2f;
		oInnerRootBoneDiscColor.a  = 0.2f;
		oOuterRootBoneDiscColor.a = 0.2f;

		for( int iBoneIndex = 0, iBoneCount = oBones.Length; iBoneIndex < iBoneCount; ++iBoneIndex )
		{
			Uni2DSmoothBindingBone rBone = oBones[ iBoneIndex ];

			if( rBone.IsFakeRootBone == false )
			{
				Vector3 f3BonePosition = rBone.transform.position;
				float fHandleSize = HandleUtility.GetHandleSize( f3BonePosition );
	
				if( rBone.Parent != null )
				{
					// Outer disc
					Handles.color = oOuterBoneDiscColor;
					Handles.DrawSolidDisc( f3BonePosition,
						f3CameraForward,
						0.25f * fHandleSize );
		
					// Inner disc
					Handles.color = oInnerBoneDiscColor;
					Handles.DrawSolidDisc( f3BonePosition,
						f3CameraForward,
						0.125f * fHandleSize );
				}
				else
				{
					// Outer disc
					Handles.color = oOuterRootBoneDiscColor;
					Handles.DrawSolidDisc( f3BonePosition,
						f3CameraForward,
						0.25f * fHandleSize );
		
					// Inner disc
					Handles.color = oInnerRootBoneDiscColor;
					Handles.DrawSolidDisc( f3BonePosition,
						f3CameraForward,
						0.125f * fHandleSize );				
				}
			}
		}

		if( rNearestBone != null )
		{
			MouseCursor eMouseCursor;
			//Color oHandleColor;

			if( eBoneHandle == BoneHandle.InnerDisc )
			{
				#if UNITY_3_5
				// Unity 3.5.x: Display an arrow while moving, and a link cursor while hovering
				eMouseCursor = rActiveBoneTransform != null ? MouseCursor.Arrow : MouseCursor.Link;
				#else
				// Unity 4.x.y: Display an arrow with a plus sign
				eMouseCursor = MouseCursor.ArrowPlus;
				#endif

				oInnerBoneDiscColor.a  = 0.8f;
				oInnerRootBoneDiscColor.a = 0.8f;
				oOuterBoneDiscColor.a = 0.2f;
				oOuterRootBoneDiscColor.a = 0.2f;
			}
			else
			{
				eMouseCursor = MouseCursor.MoveArrow;
				oInnerBoneDiscColor.a  = 0.2f;
				oInnerRootBoneDiscColor.a = 0.2f;
				oOuterBoneDiscColor.a = 0.8f;
				oOuterRootBoneDiscColor.a = 0.8f;
			}

			Handles.BeginGUI( );
			{
				Vector2 f2MousePos = a_rEvent.mousePosition;
				Rect oCursorRect = new Rect( f2MousePos.x - 16.0f, f2MousePos.y - 16.0f, 32.0f, 32.0f );

				EditorGUIUtility.AddCursorRect( oCursorRect, eMouseCursor );
			}
			Handles.EndGUI( );

			Vector3 f3NearestBonePos = rNearestBone.transform.position;
			float fHandleSize = HandleUtility.GetHandleSize( f3NearestBonePos );

			if( rNearestBone.Parent != null )
			{
				// Outer disc
				Handles.color = oOuterBoneDiscColor;
				Handles.DrawSolidDisc( f3NearestBonePos,
					f3CameraForward,
					0.25f * fHandleSize );
	
				// Inner disc
				Handles.color = oInnerBoneDiscColor;
				Handles.DrawSolidDisc( f3NearestBonePos,
					f3CameraForward,
					0.125f * fHandleSize );
			}
			else
			{
				// Outer disc
				Handles.color = oOuterRootBoneDiscColor;
				Handles.DrawSolidDisc( f3NearestBonePos,
					f3CameraForward,
					0.25f * fHandleSize );
	
				// Inner disc
				Handles.color = oInnerRootBoneDiscColor;
				Handles.DrawSolidDisc( f3NearestBonePos,
					f3CameraForward,
					0.125f * fHandleSize );
			}
		}

		if( rActiveBoneTransform != null )
		{
			Handles.color = Uni2DEditorSmoothBindingGUI.activeBone.Parent != null
				? Uni2DEditorPreferences.SelectedBoneDiscHandleOutlineColor
				: Uni2DEditorPreferences.SelectedRootBoneDiscHandleOutlineColor;
			
			float fHandleSize = HandleUtility.GetHandleSize( rActiveBoneTransform.position );
			Handles.DrawWireDisc( rActiveBoneTransform.position, f3CameraForward, 0.25f * fHandleSize );
		}
	}

	private void UpdatePoseEditGUI( Event a_rEvent )
	{
		//Transform rRootTransform = Selection.activeTransform;
		Vector2 f2MouseGUIPos = a_rEvent.mousePosition;

		if( a_rEvent.isMouse )
		{
			int iMouseButton = a_rEvent.button;

			switch( a_rEvent.type )
			{
				case EventType.MouseMove:
				{
					// Reset state
					m_bIsDragging = false;

					// Something selected, edit tool used => move bone
					if( Uni2DEditorSmoothBindingGUI.activeBone != null && ms_eEditorTool != BoneEditorTool.Select )
					{
						Uni2DEditorSmoothBindingGUI.activeBone.MoveBoneAlongSpritePlane( f2MouseGUIPos - m_f2MouseGUIOffset, a_rEvent.alt );
					}

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseMove

				case EventType.MouseDown:
				{
					// Left click
					if( iMouseButton == 0 )
					{
						// Reset drag state
						m_bIsDragging = false;


						// Selection
						if( ms_eEditorTool == BoneEditorTool.Select )
						{
							// Pick bones in scene
							BoneHandle ePickedHandle;
							Uni2DSmoothBindingBone rNearestBone = Uni2DEditorSmoothBindingUtils.PickNearestBoneInPosingMode( ms_rSprite, f2MouseGUIPos, out ePickedHandle, null );

							// Outer disc => move
							if( ePickedHandle == BoneHandle.OuterDisc || ePickedHandle == BoneHandle.Link )
							{
								if( rNearestBone != null )
								{
									// Change selection and editor tool
									Uni2DEditorSmoothBindingGUI.activeBone = rNearestBone;
									ms_eEditorTool = BoneEditorTool.Move;

									m_f2MouseGUIOffset = f2MouseGUIPos - HandleUtility.WorldToGUIPoint( rNearestBone.transform.position );
								}
							}
							else // Bone picked via inner disc or no bone picked
							{
								// A bone has been picked (via inner disc)
								// Set selection to this bone
								if( rNearestBone != null )
								{
									Uni2DEditorSmoothBindingGUI.activeBone = rNearestBone;
								}
								else // No bone picked
								{
									// Get rid of selection if any
									if( Uni2DEditorSmoothBindingGUI.activeBone != null )
									{
										Uni2DEditorSmoothBindingGUI.activeBone = null;
									}
									else // no selection, no bone picked => create a new bone chain
									{
										m_rLastAddedBone = null;

										// Create a new root and set it as current active bone
										Uni2DEditorSmoothBindingGUI.activeBone = Uni2DEditorSmoothBindingUtils.AddBoneToSprite( ms_rSprite, f2MouseGUIPos, null );
										m_rBoneChainOrigin = Uni2DEditorSmoothBindingGUI.activeBone;

										// The bone is not a child of a newly created root
										//m_bStartFromExistingBone = false;

										// Change editor tool
										ms_eEditorTool = BoneEditorTool.Create;
									}
								}
							}
						}	// end if( editor tool == select )
					}

					// Consume mouse event
					a_rEvent.Use( );
				}
				break;	// end MouseDown

				case EventType.MouseDrag:
				{
					if( iMouseButton == 0 )
					{
						switch( ms_eEditorTool )
						{
							case BoneEditorTool.Move:
							{
								// Something dragged? MOVE IT
								if( Uni2DEditorSmoothBindingGUI.activeBone != null )
								{
									// We're moving an existing bone => register undo at first drag event
									if( m_bIsDragging == false )
									{
										Undo.SetSnapshotTarget( Uni2DEditorSmoothBindingGUI.activeBone, "Move Uni2D Bone" );
										Undo.CreateSnapshot( );
										Undo.RegisterSnapshot( );
									}
			
									// Move/drag along sprite plane;
									Uni2DEditorSmoothBindingGUI.activeBone.MoveBoneAlongSpritePlane( f2MouseGUIPos - m_f2MouseGUIOffset, a_rEvent.alt );
								}
								m_bIsDragging = true;
							}
							break;

							case BoneEditorTool.Select:
							{
								// Dragging a bone in select mode == dragging on inner disc => add a child to active bone
								if( Uni2DEditorSmoothBindingGUI.activeBone != null && m_bIsDragging == false )
								{
									m_rBoneChainOrigin       = Uni2DEditorSmoothBindingGUI.activeBone;
									m_rLastAddedBone         = null;
									Uni2DEditorSmoothBindingGUI.activeBone = Uni2DEditorSmoothBindingUtils.AddBoneToSprite( ms_rSprite, f2MouseGUIPos, Uni2DEditorSmoothBindingGUI.activeBone );
									//m_bStartFromExistingBone = true;
									ms_eEditorTool            = BoneEditorTool.Create;
								}
								m_bIsDragging = true;
							}
							break;

							case BoneEditorTool.Create:
							{
								if( Uni2DEditorSmoothBindingGUI.activeBone != null )
								{
									Uni2DEditorSmoothBindingGUI.activeBone.MoveBoneAlongSpritePlane( f2MouseGUIPos );
								}
								m_bIsDragging = true;
							}
							break;
						}
					}

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseDrag

				case EventType.MouseUp:
				{
					if( iMouseButton == 0 )
					{
						switch( ms_eEditorTool )
						{
							// Creation
							case BoneEditorTool.Create:
							{
								BoneHandle ePickedHandle;
								Uni2DSmoothBindingBone rNearestBone = Uni2DEditorSmoothBindingUtils.PickNearestBoneInPosingMode( ms_rSprite, f2MouseGUIPos, out ePickedHandle, Uni2DEditorSmoothBindingGUI.activeBone );

								// Mouse up near the last added bone => close bone chain
								if( ePickedHandle == BoneHandle.InnerDisc && rNearestBone == m_rLastAddedBone )
								{
									Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );

									m_rLastAddedBone = null;
									Uni2DEditorSmoothBindingGUI.activeBone = null;
									ms_eEditorTool = BoneEditorTool.Select;
								}
								else
								{
									m_rLastAddedBone = Uni2DEditorSmoothBindingGUI.activeBone;
									Undo.RegisterCreatedObjectUndo( m_rLastAddedBone.gameObject, "Add Uni2D Bone" );

									// Creating => validate bone and create another one
									Uni2DEditorSmoothBindingGUI.activeBone = Uni2DEditorSmoothBindingUtils.AddBoneToSprite( ms_rSprite, f2MouseGUIPos, m_rLastAddedBone );

								}
							}
							break;

							// Move
							case BoneEditorTool.Move:
							{
								Undo.ClearSnapshotTarget( );
								Undo.RegisterSnapshot( );
								ms_eEditorTool = BoneEditorTool.Select;
							}
							break;
						}

						// Reset dragging state
						m_bIsDragging = false;
						m_f2MouseGUIOffset = Vector2.zero;						
					}	// end if( left button )
					else if( iMouseButton == 1 )	// Delete / stop bone creation
					{
						switch( ms_eEditorTool )
						{
							case BoneEditorTool.Select:
							{
								BoneHandle eBoneHandle;
								Uni2DSmoothBindingBone rNearestBone = Uni2DEditorSmoothBindingUtils.PickNearestBoneInPosingMode( ms_rSprite, f2MouseGUIPos, out eBoneHandle, null );
								
								if( rNearestBone != null )
								{
									if( eBoneHandle == BoneHandle.Link )
									{
										//Undo.RegisterSetTransformParentUndo( rNearestBone.transform, rRootTransform, "Break Uni2D Bone" );
										rNearestBone.Break( );
									}
									else
									{
										Undo.RegisterSceneUndo( "Delete Uni2D Bone" );
										Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, rNearestBone );
									}
								}
							}
							break;

							case BoneEditorTool.Create:
							{
								// Close bone chain
								Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );
								ms_eEditorTool = BoneEditorTool.Select;
							}
							break;

							case BoneEditorTool.Move:
							{
								ms_eEditorTool = BoneEditorTool.Select;
							}
							break;
						}
					}	// End if( right button )

					// Consume event
					a_rEvent.Use( );
				}
				break;	// End MouseUp
			}	// End switch( event.type )
		}	// end if( mouse events )
		else if( a_rEvent.isKey && a_rEvent.type == EventType.keyDown )
		{
			switch( a_rEvent.keyCode )
			{
				case KeyCode.Escape:
				{
					switch( ms_eEditorTool )
					{
						case BoneEditorTool.Select:
						{
							if( Uni2DEditorSmoothBindingGUI.activeBone != null )
							{
								Uni2DEditorSmoothBindingGUI.activeBone = null;
							}
							else
							{
								Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;
							}
						}
						break;

						case BoneEditorTool.Move:
						{
							Undo.RestoreSnapshot( );
							ms_eEditorTool = BoneEditorTool.Select;
						}
						break;
					
						case BoneEditorTool.Create:
						{
							Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );
							ms_eEditorTool = BoneEditorTool.Select;
						}
						break;
					}
					a_rEvent.Use( );
				}
				break;	// End Escape

				// Delete last created bone (if any)
				case KeyCode.Backspace:
				{
					if( ms_eEditorTool == BoneEditorTool.Create )
					{
						if( m_rLastAddedBone != null && m_rLastAddedBone != m_rBoneChainOrigin )
						{
							Undo.RegisterSceneUndo( "Delete Uni2D Bone" );
							Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );
						
							Uni2DEditorSmoothBindingGUI.activeBone = Uni2DEditorSmoothBindingUtils.AddBoneToSprite( ms_rSprite, f2MouseGUIPos, m_rLastAddedBone.Parent );
							Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, m_rLastAddedBone );

							Uni2DSmoothBindingBone rParentActiveBone = Uni2DEditorSmoothBindingGUI.activeBone.Parent;
							if( rParentActiveBone != null && rParentActiveBone.IsFakeRootBone )
							{
								m_rLastAddedBone = rParentActiveBone.Parent;	// Grand-pa'
							}
							else
							{
								m_rLastAddedBone = rParentActiveBone;
							}
						}
					}
					a_rEvent.Use( );
				}
				break;	// End Escape
				
				case KeyCode.Return:
				case KeyCode.KeypadEnter:
				{
					ms_eEditorTool = BoneEditorTool.Select;
					a_rEvent.Use( );
				}
				break;	// End Return / KeypadEnter
			}	// end switch( event.type )
		}	// end if( key down events )
		else
		{
			switch( a_rEvent.type )
			{
				case EventType.ValidateCommand:
				{
					if( a_rEvent.commandName == "Delete" || a_rEvent.commandName == "UndoRedoPerformed" )
					{
						a_rEvent.Use( );
					}
				}
				break;	// end ValidateCommand
					
				case EventType.ExecuteCommand:
				{
					if( a_rEvent.commandName == "Delete" )
					{
						if( Uni2DEditorSmoothBindingGUI.activeBone != null && ms_eEditorTool != BoneEditorTool.Create )
						{
							Undo.RegisterSceneUndo( "Delete Uni2D Bone" );
							Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );
							ms_eEditorTool = BoneEditorTool.Select;
						}
						a_rEvent.Use( );
					}
					else if( a_rEvent.commandName == "UndoRedoPerformed" )
					{
						if( ms_eEditorTool == BoneEditorTool.Create && Uni2DEditorSmoothBindingGUI.activeBone != null )
						{
							Uni2DEditorSmoothBindingUtils.DeleteBone( ms_rSprite, Uni2DEditorSmoothBindingGUI.activeBone );
						}
						m_rLastAddedBone = null;
						Uni2DEditorSmoothBindingGUI.activeBone = null;
						ms_eEditorTool = BoneEditorTool.Select;
						//Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode = BoneEditMode.None;

						a_rEvent.Use( );
					}
				}
				break;	// end ExecuteCommand
			}	// end switch( event.type )
		}	// end else
	}
}
#endif
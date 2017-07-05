#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/* 
 * Uni2DEditorPreferences
 * 
 * Displays the Uni2D settings GUI into Unity preferences window.
 * 
 * Editor use only.
 * 
 */

[InitializeOnLoad]
public static class Uni2DEditorPreferences
{
	///// Gizmo /////

	// Bones
	private const string mc_oUnselectedBoneGizmoColorKeyPref = "Uni2DPref_UnselectedBoneGizmoColor";
	private const string mc_oSelectedBoneGizmoColorKeyPref   = "Uni2DPref_SelectedBoneGizmoColor";
	private const string mc_oEditableBoneGizmoColorKeyPref   = "Uni2DPref_EditableBoneGizmoColor";
	private const string mc_oActiveBoneGizmoColorKeyPref     = "Uni2DPref_ActiveBoneGizmoColor";

	// Root bones
	private const string mc_oUnselectedRootBoneGizmoColorKeyPref = "Uni2DPref_UnselectedRootBoneGizmoColor";
	private const string mc_oSelectedRootBoneGizmoColorKeyPref   = "Uni2DPref_SelectedRootBoneGizmoColor";
	private const string mc_oEditableRootBoneGizmoColorKeyPref   = "Uni2DPref_EditableRootBoneGizmoColor";
	private const string mc_oActiveRootBoneGizmoColorKeyPref     = "Uni2DPref_ActiveRootBoneGizmoColor";

	// Handles
	private const string mc_oInnerBoneDiscHandleColorKeyPref           = "Uni2DPref_InnerBoneDiscHandleColor";
	private const string mc_oOuterBoneDiscHandleColorKeyPref           = "Uni2DPref_OuterBoneDiscHandleColor";
	private const string mc_oSelectedBoneDiscHandleOutlineColorKeyPref = "Uni2DPref_SelectedBoneDiscHandleOutlineColor";

	private const string mc_oInnerRootBoneDiscHandleColorKeyPref           = "Uni2DPref_InnerRootBoneDiscHandleColor";
	private const string mc_oOuterRootBoneDiscHandleColorKeyPref           = "Uni2DPref_OuterRootBoneDiscHandleColor";
	private const string mc_oSelectedRootBoneDiscHandleOutlineColorKeyPref = "Uni2DPref_SelectedRootBoneDiscHandleOutlineColor";

	///// Skinning /////
	private const string mc_oSmoothBindingDefaultSkinQualityKeyPref = "Uni2DPref_SmoothBindingDefaultSkinQuality";

	///// Preference values /////
	private static Color32 ms_oUnselectedBoneGizmoColor32 = Color.blue;
	private static Color32 ms_oSelectedBoneGizmoColor32   = Color.green;
	private static Color32 ms_oEditableBoneGizmoColor32   = Color.red;
	private static Color32 ms_oActiveBoneGizmoColor32     = Color.green;

	private static Color32 ms_oUnselectedRootBoneGizmoColor32 = Color.cyan;
	private static Color32 ms_oSelectedRootBoneGizmoColor32   = Color.green;
	private static Color32 ms_oEditableRootBoneGizmoColor32   = Color.magenta;
	private static Color32 ms_oActiveRootBoneGizmoColor32     = Color.green;

	private static Color32 ms_oInnerBoneDiscHandleColor32           = Color.red;
	private static Color32 ms_oOuterBoneDiscHandleColor32           = Color.white;
	private static Color32 ms_oSelectedBoneDiscHandleOutlineColor32 = Color.green;

	private static Color32 ms_oInnerRootBoneDiscHandleColor32           = Color.magenta;
	private static Color32 ms_oOuterRootBoneDiscHandleColor32           = Color.white;
	private static Color32 ms_oSelectedRootBoneDiscHandleOutlineColor32 = Color.magenta;

	private static SkinQuality ms_oSmoothBindingDefaultSkinQuality = SkinQuality.Bone4;

	// Load all Uni2D editor prefs at Unity editor startup
	static Uni2DEditorPreferences ()
	{
		Uni2DEditorPreferences.UnselectedBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oUnselectedBoneGizmoColorKeyPref, ms_oUnselectedBoneGizmoColor32);
		Uni2DEditorPreferences.SelectedBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oSelectedBoneGizmoColorKeyPref, ms_oSelectedBoneGizmoColor32);
		Uni2DEditorPreferences.EditableBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oEditableBoneGizmoColorKeyPref, ms_oEditableBoneGizmoColor32);
		Uni2DEditorPreferences.ActiveBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oActiveBoneGizmoColorKeyPref, ms_oActiveBoneGizmoColor32);

		Uni2DEditorPreferences.UnselectedRootBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oUnselectedRootBoneGizmoColorKeyPref, ms_oUnselectedRootBoneGizmoColor32);
		Uni2DEditorPreferences.SelectedRootBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oSelectedRootBoneGizmoColorKeyPref, ms_oSelectedRootBoneGizmoColor32);
		Uni2DEditorPreferences.EditableRootBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oEditableRootBoneGizmoColorKeyPref, ms_oEditableRootBoneGizmoColor32);
		Uni2DEditorPreferences.ActiveRootBoneGizmoColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oActiveRootBoneGizmoColorKeyPref, ms_oActiveRootBoneGizmoColor32);

		Uni2DEditorPreferences.InnerBoneDiscHandleColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oInnerBoneDiscHandleColorKeyPref, ms_oInnerBoneDiscHandleColor32);
		Uni2DEditorPreferences.OuterBoneDiscHandleColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oOuterBoneDiscHandleColorKeyPref, ms_oOuterBoneDiscHandleColor32);
		Uni2DEditorPreferences.SelectedBoneDiscHandleOutlineColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oSelectedBoneDiscHandleOutlineColorKeyPref, ms_oSelectedBoneDiscHandleOutlineColor32);

		Uni2DEditorPreferences.InnerRootBoneDiscHandleColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oInnerRootBoneDiscHandleColorKeyPref, ms_oInnerRootBoneDiscHandleColor32);
		Uni2DEditorPreferences.OuterRootBoneDiscHandleColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oOuterRootBoneDiscHandleColorKeyPref, ms_oOuterRootBoneDiscHandleColor32);
		Uni2DEditorPreferences.SelectedRootBoneDiscHandleOutlineColor = Uni2DEditorPreferences.GetEditorPrefColor32 (mc_oSelectedRootBoneDiscHandleOutlineColorKeyPref, ms_oSelectedRootBoneDiscHandleOutlineColor32);

		Uni2DEditorPreferences.SmoothBindingDefaultSkinQuality = (SkinQuality)EditorPrefs.GetInt (mc_oSmoothBindingDefaultSkinQualityKeyPref, (int)ms_oSmoothBindingDefaultSkinQuality); 
	}

	public static Color32 UnselectedBoneGizmoColor
	{
		get
		{
			return ms_oUnselectedBoneGizmoColor32;
		}
		set
		{
			ms_oUnselectedBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oUnselectedBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 SelectedBoneGizmoColor
	{
		get
		{
			return ms_oSelectedBoneGizmoColor32;
		}
		set
		{
			ms_oSelectedBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oSelectedBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 EditableBoneGizmoColor
	{
		get
		{
			return ms_oEditableBoneGizmoColor32;
		}
		set
		{
			ms_oEditableBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oEditableBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 ActiveBoneGizmoColor
	{
		get
		{
			return ms_oActiveBoneGizmoColor32;
		}
		set
		{
			ms_oActiveBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oActiveBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 UnselectedRootBoneGizmoColor
	{
		get
		{
			return ms_oUnselectedRootBoneGizmoColor32;
		}
		set
		{
			ms_oUnselectedRootBoneGizmoColor32 = value;	
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oUnselectedRootBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 SelectedRootBoneGizmoColor
	{
		get
		{
			return ms_oSelectedRootBoneGizmoColor32;
		}
		set
		{
			ms_oSelectedRootBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oSelectedRootBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 EditableRootBoneGizmoColor
	{
		get
		{
			return ms_oEditableRootBoneGizmoColor32;
		}
		set
		{
			ms_oEditableRootBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oEditableRootBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 ActiveRootBoneGizmoColor
	{
		get
		{
			return ms_oActiveRootBoneGizmoColor32;
		}
		set
		{
			ms_oActiveRootBoneGizmoColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oActiveRootBoneGizmoColorKeyPref, value );
		}
	}

	public static Color32 InnerBoneDiscHandleColor
	{
		get
		{
			return ms_oInnerBoneDiscHandleColor32;
		}
		set
		{
			ms_oInnerBoneDiscHandleColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oInnerBoneDiscHandleColorKeyPref, value );
		}
	}

	public static Color32 OuterBoneDiscHandleColor
	{
		get
		{
			return ms_oOuterBoneDiscHandleColor32;
		}
		set
		{
			ms_oOuterBoneDiscHandleColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oOuterBoneDiscHandleColorKeyPref, value );
		}
	}

	public static Color32 SelectedBoneDiscHandleOutlineColor
	{
		get
		{
			return ms_oSelectedBoneDiscHandleOutlineColor32;
		}
		set
		{
			ms_oSelectedBoneDiscHandleOutlineColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oSelectedBoneDiscHandleOutlineColorKeyPref, value );
		}
	}

	public static Color32 InnerRootBoneDiscHandleColor
	{
		get
		{
			return ms_oInnerRootBoneDiscHandleColor32;
		}
		set
		{
			ms_oInnerRootBoneDiscHandleColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oInnerRootBoneDiscHandleColorKeyPref, value );
		}
	}

	public static Color32 OuterRootBoneDiscHandleColor
	{
		get
		{
			return ms_oOuterRootBoneDiscHandleColor32;
		}
		set
		{
			ms_oOuterRootBoneDiscHandleColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oOuterRootBoneDiscHandleColorKeyPref, value );
		}
	}

	public static Color32 SelectedRootBoneDiscHandleOutlineColor
	{
		get
		{
			return ms_oSelectedRootBoneDiscHandleOutlineColor32;
		}
		set
		{
			ms_oSelectedRootBoneDiscHandleOutlineColor32 = value;
			Uni2DEditorPreferences.SetEditorPrefColor32( mc_oSelectedRootBoneDiscHandleOutlineColorKeyPref, value );
		}
	}

	public static SkinQuality SmoothBindingDefaultSkinQuality
	{
		get
		{
			return ms_oSmoothBindingDefaultSkinQuality;
		}
		set
		{
			ms_oSmoothBindingDefaultSkinQuality = value;
			EditorPrefs.SetInt( mc_oSmoothBindingDefaultSkinQualityKeyPref, (int) value );
		}
	}

	[PreferenceItem( "Uni2D" )]
	public static void ShowUni2DPreferences( )
	{
		GUILayoutOption oMaxWidthOption = GUILayout.MaxWidth( 135.0f );
		GUILayoutOption oMaxColorPickedWidthOption = GUILayout.MaxWidth( 100.0f );

		//EditorGUILayout.HelpBox( "Uni2D Beta", MessageType.Warning, true );
		
		EditorGUILayout.BeginVertical( );
		{			
			// Header
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Bone Gizmo State", EditorStyles.boldLabel, oMaxWidthOption );
				EditorGUILayout.LabelField( "Bone", EditorStyles.boldLabel, oMaxColorPickedWidthOption );
				EditorGUILayout.LabelField( "Root Bone", EditorStyles.boldLabel, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
			
			// Unselected
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Unselected", oMaxWidthOption );
				Uni2DEditorPreferences.UnselectedBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.UnselectedBoneGizmoColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.UnselectedRootBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.UnselectedRootBoneGizmoColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
			
			// Selected
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Selected", oMaxWidthOption );
				Uni2DEditorPreferences.SelectedBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.SelectedBoneGizmoColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.SelectedRootBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.SelectedRootBoneGizmoColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
			
			// Editable
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Editable", oMaxWidthOption );
				Uni2DEditorPreferences.EditableBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.EditableBoneGizmoColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.EditableRootBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.EditableRootBoneGizmoColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
	
			// Active
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Active", oMaxWidthOption );
				Uni2DEditorPreferences.ActiveBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.ActiveBoneGizmoColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.ActiveRootBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.ActiveRootBoneGizmoColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );

			EditorGUILayout.Space( );

			// Header
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Disc Handle", EditorStyles.boldLabel, oMaxWidthOption );
				EditorGUILayout.LabelField( "Bone", EditorStyles.boldLabel, oMaxColorPickedWidthOption );
				EditorGUILayout.LabelField( "Root Bone", EditorStyles.boldLabel, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );

			// Inner disc handle
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Inner", oMaxWidthOption );
				Uni2DEditorPreferences.InnerBoneDiscHandleColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.InnerBoneDiscHandleColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.InnerRootBoneDiscHandleColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.InnerRootBoneDiscHandleColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
	
			// Outer disc handle
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Outer", oMaxWidthOption );
				Uni2DEditorPreferences.OuterBoneDiscHandleColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.OuterBoneDiscHandleColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.OuterRootBoneDiscHandleColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.OuterRootBoneDiscHandleColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
	
			// Selected disc handle outline
			EditorGUILayout.BeginHorizontal( );
			{
				EditorGUILayout.LabelField( "Outline (when selected)", oMaxWidthOption );
				Uni2DEditorPreferences.SelectedBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.SelectedBoneDiscHandleOutlineColor, oMaxColorPickedWidthOption );
				Uni2DEditorPreferences.SelectedRootBoneGizmoColor = EditorGUILayout.ColorField( Uni2DEditorPreferences.SelectedRootBoneDiscHandleOutlineColor, oMaxColorPickedWidthOption );
			}
			EditorGUILayout.EndHorizontal( );
	
			EditorGUILayout.Space( );
	
		}
		EditorGUILayout.EndVertical( );

		Uni2DEditorPreferences.SmoothBindingDefaultSkinQuality = (SkinQuality) EditorGUILayout.EnumPopup( "Default Skin Quality", Uni2DEditorPreferences.SmoothBindingDefaultSkinQuality );

		EditorGUILayout.Space( );	
	
		EditorGUILayout.BeginHorizontal( );
		{
			if( GUILayout.Button( "Use Default" ) )
			{
				Uni2DEditorPreferences.RestoreDefaults( );
			}
			GUILayout.FlexibleSpace( );
		}
		EditorGUILayout.EndHorizontal( );

		EditorGUILayout.Space( );

		// Asset table rebuild
		EditorGUILayout.BeginVertical( );
		{
			EditorGUILayout.LabelField( "Rebuild the Uni2D asset table if you think Uni2D is not handling your assets properly.", EditorStyles.wordWrappedLabel );
			
			if( GUILayout.Button( "Rebuild Uni2D Asset Table" ) )
			{
				Uni2DEditorAssetTable rAssetTable = Uni2DEditorAssetTable.Instance;
				rAssetTable.Rebuild( );
			}
		}
		EditorGUILayout.EndVertical( );
	}

	private static void RestoreDefaults( )
	{
		Uni2DEditorPreferences.ActiveBoneGizmoColor     = Color.green;
		Uni2DEditorPreferences.ActiveRootBoneGizmoColor = Color.green;

		Uni2DEditorPreferences.EditableBoneGizmoColor     = Color.yellow;
		Uni2DEditorPreferences.EditableRootBoneGizmoColor = Color.magenta;

		Uni2DEditorPreferences.SelectedBoneGizmoColor     = Color.green;
		Uni2DEditorPreferences.SelectedRootBoneGizmoColor = Color.green;

		Uni2DEditorPreferences.UnselectedBoneGizmoColor     = Color.blue;
		Uni2DEditorPreferences.UnselectedRootBoneGizmoColor = Color.cyan;

		Uni2DEditorPreferences.InnerBoneDiscHandleColor = Color.white;
		Uni2DEditorPreferences.InnerRootBoneDiscHandleColor = Color.white;

		Uni2DEditorPreferences.OuterBoneDiscHandleColor = Color.red;
		Uni2DEditorPreferences.OuterRootBoneDiscHandleColor = Color.magenta;

		Uni2DEditorPreferences.SelectedBoneGizmoColor = Color.green;
		Uni2DEditorPreferences.SelectedRootBoneDiscHandleOutlineColor = Color.magenta;

		Uni2DEditorPreferences.SmoothBindingDefaultSkinQuality = SkinQuality.Bone4;
	}

	private static Color32 GetEditorPrefColor32( string a_rEditorKeyPref, Color32 a_rDefaultColor32 )
	{
		return IntToColor32( EditorPrefs.GetInt( a_rEditorKeyPref, Color32ToInt( a_rDefaultColor32 ) ) );
	}

	private static void SetEditorPrefColor32( string a_rEditorKeyPref, Color32 a_rColor32 )
	{
		EditorPrefs.SetInt( a_rEditorKeyPref, Color32ToInt( a_rColor32 ) );
	}

	private static Color32 IntToColor32( int a_iInt )
	{
		return new Color32(
			(byte) ( a_iInt >> 24 ),
			(byte) ( a_iInt >> 16 ),
			(byte) ( a_iInt >>  8 ),
			(byte) ( a_iInt )
			);
	}
	
	private static int Color32ToInt( Color32 a_rColor32 )
	{
		return ( (int) a_rColor32.r ) << 24
			| ( (int) a_rColor32.g ) << 16
			| ( (int) a_rColor32.b ) <<  8
			| ( (int) a_rColor32.a );
	}
}
#endif
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor( typeof( Uni2DEditorAssetTable ) )]
public class Uni2DAssetTableInspector : Editor
{
	public override void OnInspectorGUI( )
	{
		EditorGUILayout.BeginVertical( );
		{
			EditorGUILayout.HelpBox( "Uni2D uses a custom asset table to rebuild the dependencies of Uni2D assets.\n\n"
				+ "Rebuild this table manually if you think Uni2D doesn't rebuild your Uni2D assets dependencies properly.\n\n"
				+ "Please be patient if you do so: all assets must be checked.", MessageType.Info, true );

			if( GUILayout.Button( "Rebuild Asset Table" ) )
			{
				( (Uni2DEditorAssetTable) target ).Rebuild( );
			}
		}
		EditorGUILayout.EndVertical( );
	}
}

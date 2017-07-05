
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(MegaWireSpan))]
public class MegaWireSpanEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUIUtility.LookLikeControls();
	}
}

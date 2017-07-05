﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CC_Negative))]
public class CC_NegativeEditor : Editor
{
	SerializedObject srcObj;

	SerializedProperty amount;

	void OnEnable()
	{
		srcObj = new SerializedObject(target);

		amount = srcObj.FindProperty("amount");
	}

	public override void OnInspectorGUI()
	{
		srcObj.Update();

		EditorGUILayout.Slider(amount, 0.0f, 1.0f, "Amount");

		srcObj.ApplyModifiedProperties();
    }
}

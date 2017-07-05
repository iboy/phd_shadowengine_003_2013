// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

[CustomEditor(typeof(ChipmunkPivotJoint))]
[CanEditMultipleObjects]
public class ChipmunkPivotJointEditor : ChipmunkEditor
{
	public void OnSceneGUI(){
		ChipmunkPivotJoint joint = target as ChipmunkPivotJoint;
		if(joint != null && joint._handle == IntPtr.Zero){
			SetupUndo("edited ChipmunkPivotJoint");
			Transform t = joint.transform;
			
			Vector3 pivot = t.TransformPoint(joint.pivot);
			Vector2 pivotDelta = DotHandle(pivot) - (Vector2) pivot;
			if(pivotDelta != Vector2.zero){
				joint.pivot = t.InverseTransformPoint((Vector2) pivot + pivotDelta);
				EditorUtility.SetDirty(target);
			}
		}
	}
}

// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


[CustomEditor(typeof(ChipmunkSlideJoint))]
[CanEditMultipleObjects]
public class ChipmunkSlideJointEditor : ChipmunkEditor
{
	public void OnSceneGUI(){
		ChipmunkSlideJoint joint = target as ChipmunkSlideJoint;
		if(joint != null && joint._handle == IntPtr.Zero){
			SetupUndo("edited ChipmunkSlideJoint");
			Transform t = joint.transform;
			
			Vector3 anchr1 = t.TransformPoint(joint.anchr1);
			Vector2 anchr1Delta = DotHandle(anchr1) - (Vector2) anchr1;
			if(anchr1Delta != Vector2.zero){
				joint.anchr1 = t.InverseTransformPoint((Vector2) anchr1 + anchr1Delta);
				EditorUtility.SetDirty(target);
			}
			
			Vector3 anchr2 = t.TransformPoint(joint.anchr2);
			Vector2 anchr2Delta = DotHandle(anchr2) - (Vector2) anchr2;
			if(anchr2Delta != Vector2.zero){
				joint.anchr2 = t.InverseTransformPoint((Vector2) anchr2 + anchr2Delta);
				EditorUtility.SetDirty(target);
			}
		}
	}
}

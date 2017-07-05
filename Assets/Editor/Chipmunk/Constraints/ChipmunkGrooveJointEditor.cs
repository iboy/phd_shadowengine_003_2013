// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


[CustomEditor(typeof(ChipmunkGrooveJoint))]
[CanEditMultipleObjects]
public class ChipmunkGrooveJointEditor : ChipmunkEditor
{
	public void OnSceneGUI(){
		ChipmunkGrooveJoint joint = target as ChipmunkGrooveJoint;
		if(joint != null && joint._handle == IntPtr.Zero){
			SetupUndo("edited ChipmunkGrooveJoint");
			Transform t = joint.transform;
			
			Vector3 grooveA = t.TransformPoint(joint.grooveA);
			Vector3 grooveB = t.TransformPoint(joint.grooveB);
			Vector3 anchr2 = t.TransformPoint(joint.anchr2);
			
			Vector2 grooveADelta = DotHandle(grooveA) - (Vector2) grooveA;
			if(grooveADelta != Vector2.zero){
				joint.grooveA = t.InverseTransformPoint((Vector2) grooveA + grooveADelta);
				
				Vector2 snapped = HandleUtility.ProjectPointLine((Vector2) anchr2, (Vector2) grooveA + grooveADelta, grooveB);
				joint.anchr2 = t.InverseTransformPoint(snapped);
				
				EditorUtility.SetDirty(target);
			}
			
			Vector2 grooveBDelta = DotHandle(grooveB) - (Vector2) grooveB;
			if(grooveBDelta != Vector2.zero){
				joint.grooveB = t.InverseTransformPoint((Vector2) grooveB + grooveBDelta);
				
				Vector2 snapped = HandleUtility.ProjectPointLine((Vector2) anchr2, (Vector2) grooveA, (Vector2) grooveB + grooveBDelta);
				joint.anchr2 = t.InverseTransformPoint(snapped);
				
				EditorUtility.SetDirty(target);
			}
			
			Vector2 anchr2Delta = CircleHandle(anchr2) - (Vector2) anchr2;
			if(anchr2Delta != Vector2.zero){
				Vector2 snapped = HandleUtility.ProjectPointLine((Vector2) anchr2 + anchr2Delta, (Vector2) grooveA, grooveB);
				joint.anchr2 = t.InverseTransformPoint(snapped);
				EditorUtility.SetDirty(target);
			}
		}
	}
}

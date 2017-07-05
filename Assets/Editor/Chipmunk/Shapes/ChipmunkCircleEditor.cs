// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ChipmunkCircleShape))]
[CanEditMultipleObjects]
public class ChipmunkCircleEditor : ChipmunkEditor
{
	protected void OnSceneGUI(){
		ChipmunkCircleShape circle = target as ChipmunkCircleShape;
		if(circle != null){
			SetupUndo("edited ChipmunkCircleShape");
			Transform t = circle.transform;
			
			Vector3 center = t.TransformPoint(circle.center);
			Vector2 centerDelta = CircleHandle(center) - (Vector2) center;
			if(centerDelta != Vector2.zero){
				circle.center = t.InverseTransformPoint( (Vector2) center + centerDelta);
				EditorUtility.SetDirty(target);
			}
			
			float radius = circle.radius;
			if(radius > 0f){
				float scaledRadius = radius*circle._maxScale;
				float radiusDelta = Handles.RadiusHandle(t.rotation, center, scaledRadius, true) - scaledRadius;
				if(radiusDelta != 0f){
					circle.radius = (scaledRadius + radiusDelta)/circle._maxScale;
					EditorUtility.SetDirty(target);
				}
			}
			
		}
	}
}

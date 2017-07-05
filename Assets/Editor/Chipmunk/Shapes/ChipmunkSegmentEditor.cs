// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ChipmunkSegmentShape))]
[CanEditMultipleObjects]
public class ChipmunkSegmentEditor : ChipmunkEditor
{
	public void OnSceneGUI(){
		ChipmunkSegmentShape seg = target as ChipmunkSegmentShape;
		if(seg != null){
			SetupUndo("edited ChipmunkSegmentShape");
			Transform t = seg.transform;
			
			Vector3 endPoint1 = t.TransformPoint(seg.center + seg.endPoint);
			Vector3 endPoint2 = t.TransformPoint(seg.center - seg.endPoint);
			
			Vector2 endPoint1Delta = CircleHandle(endPoint1) - (Vector2)endPoint1;
			Vector2 endPoint2Delta = CircleHandle(endPoint2) - (Vector2)endPoint2;
			
			if(endPoint1Delta != Vector2.zero || endPoint2Delta != Vector2.zero){
				endPoint1 = endPoint1 + (Vector3) endPoint1Delta;
				endPoint2 = endPoint2 + (Vector3) endPoint2Delta;
				
				Vector2 center = Vector2.Lerp(endPoint1, endPoint2, 0.5f);
				seg.center = t.InverseTransformPoint(center);
				seg.endPoint = t.InverseTransformPoint(endPoint1) - t.InverseTransformPoint(center);
				
				EditorUtility.SetDirty(target);	
			}
			
			float maxScale = seg._maxScale;
			float radius = seg.radius;
			float radiusDelta = (
				Handles.RadiusHandle(t.rotation, endPoint1, radius*maxScale, false)/maxScale +
				Handles.RadiusHandle(t.rotation, endPoint2, radius*maxScale, false)/maxScale -
				2f*radius
			);
			
			if(radiusDelta != 0f){
				seg.radius += radiusDelta;
				EditorUtility.SetDirty(target);	
			}
		}
	}
}

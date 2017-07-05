// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ChipmunkBoxShape))]
[CanEditMultipleObjects]
public class ChipmunkBoxEditor : ChipmunkEditor
{
	protected void OnSceneGUI(){
		ChipmunkBoxShape box = target as ChipmunkBoxShape;
		if(box != null){
			SetupUndo("edited ChipmunkBoxShape");
			Transform t = box.transform;
			
			Vector3 center = t.TransformPoint(box.center);
			
			Vector2 centerDelta = CircleHandle(center) - (Vector2) center;
			if(centerDelta != Vector2.zero){
				box.center = t.InverseTransformPoint( (Vector2) center + centerDelta);
				EditorUtility.SetDirty(target);
			}
			
			Vector3 extent = t.TransformPoint(box.center + box.size/2f);
			Vector2 extentDelta = (Vector2) (DotHandle(extent) - (Vector2) extent);
			if(extentDelta != Vector2.zero){
				box.size = 2f*((Vector2)t.InverseTransformPoint((Vector2) extent + extentDelta) - box.center);
				EditorUtility.SetDirty(target);
			}
			
			float radius = box.radius;
			if(radius > 0f){
				float scaledRadius = radius*box._maxScale;
				float radiusDelta = Handles.RadiusHandle(t.rotation, extent, scaledRadius, false) - scaledRadius;
				if(radiusDelta != 0f){
					box.radius = radius + radiusDelta/box._maxScale;
					EditorUtility.SetDirty(target);
				}
			}
		}
	}
}

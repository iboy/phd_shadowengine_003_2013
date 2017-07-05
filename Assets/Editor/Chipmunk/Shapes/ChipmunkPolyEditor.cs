// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(ChipmunkPolyShape))]
[CanEditMultipleObjects]
public class ChipmunkPolyEditor : ChipmunkEditor
{
	protected Vector3 ClosestPoint(Transform t, Vector2[] verts){
		// Duplicate the verts list and make it into a loop.
		var loop = new List<Vector2>(verts);
		loop.Add(verts[0]);
		Vector3[] arr = loop.Select<Vector2, Vector3>((arg) => (Vector3)t.TransformPoint(arg) ).ToArray();
		return HandleUtility.ClosestPointToPolyLine(arr);
	}
	
	private bool insertedThisClick = false;
	
	protected void OnSceneGUI(){
		ChipmunkPolyShape poly = target as ChipmunkPolyShape;
		if(poly != null){
			SetupUndo("edited ChipmunkPolyShape");
			Transform t = poly.transform;
			
			bool dirty = false;
			var verts = new List<Vector2>(poly.verts);
			
			switch(Event.current.type){
				case EventType.mouseUp:
					verts = new List<Vector2>(poly.hull);
					dirty = true;
					insertedThisClick = false;
					break;
				case EventType.mouseMove:
					// If you repaint too much, you really use a lot of CPU. Only during mouse move seems reasonable.
					HandleUtility.Repaint();
					break;
				default: break;
			}
				
			// find closest new point handle.
			if(!insertedThisClick){
				Handles.color = Color.red;
				Vector3 v = ClosestPoint(t, poly.verts);
				Vector2 vDelta = DotHandle(v) - (Vector2) v;
				if(vDelta != Vector2.zero){
					// Insert a new vert at the front of the party.
					verts.Insert (0, t.InverseTransformPoint((Vector2) v + vDelta));
					insertedThisClick = true;
					dirty = true;
				}
			}
		
			Handles.color = Color.white;
			for(int i=0; i<verts.Count; i++){
				Vector3 v = t.TransformPoint(verts[i]);
				Vector2 vDelta = DotHandle(v) - (Vector2) v;
				if(vDelta != Vector2.zero){
					verts[i] = t.InverseTransformPoint((Vector2) v + vDelta);
					dirty = true;
				}
			}
			
			if(dirty){
				poly.verts = verts.ToArray();
				EditorUtility.SetDirty(target);
			}
		}
	}
}
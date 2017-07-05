// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;	
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;


/// Chipmunk beveled segment shape type.
/// Can be given a radius to make it shaped like a capsule.
public class ChipmunkSegmentShape : ChipmunkShape {
	public float _radius = 0.5f;
	
	/// Radius (thickness) of the segment shape.
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			if(_handle != IntPtr.Zero){
				CP.cpSegmentShapeSetRadius(_handle, _maxScale*value);
				CP.cpSpaceReindexShape(space._handle, _handle);
				
				if(body != null) body.Activate();
			}
		}
	}
	
	protected void UpdateEndpoints(){
		if(_handle != IntPtr.Zero){
			Matrix4x4 bmatrix = BodyRelativeMatrix(body);
			Vector2 pointA = bmatrix.MultiplyPoint(_center - _endPoint);
			Vector2 pointB = bmatrix.MultiplyPoint(_center + _endPoint);
			CP.cpSegmentShapeSetEndpoints(_handle, pointA, pointB);
			CP.cpSpaceReindexShape(space._handle, _handle);
			
			if(body != null) body.Activate();
		}
	}
	
	public Vector2 _center = Vector2.zero;
	
	/// Center of the segment shape relative to it's transform.
	public Vector2 center {
		get { return _center; }
		set {
			_center = value;
			UpdateEndpoints();
		}
	}
	
	public Vector2 _endPoint = new Vector2(0f, 0.5f);
	
	/// Location of the first endpoint of the segment shape relative to it's transform.
	/// The second endpoint is mirrored across the center from this one.
	public Vector2 endPoint {
		get { return _endPoint; }
		set {
			_endPoint = value;
			UpdateEndpoints();
		}
	}
		
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		base.Awake();
		
		Matrix4x4 bmatrix = BodyRelativeMatrix(body);
		Vector2 pointA = bmatrix.MultiplyPoint(_center - _endPoint);
		Vector2 pointB = bmatrix.MultiplyPoint(_center + _endPoint);
		
		_handle = CP.cpSegmentShapeNew(IntPtr.Zero, pointA, pointB, _maxScale*_radius);
		if(body != null) body._AddMassForShape(this);
		
		var gch = GCHandle.Alloc(this);
		CP._cpShapeSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	public override ChipmunkBody _UpdatedTransform(){
		UpdateParentBody();
		UpdateEndpoints();
		
		return body;
	}
	
	protected void OnDrawGizmosSelected(){
		Gizmos.color = this.gizmoColor;
		
		Matrix4x4 m = transform.localToWorldMatrix;
		float radius = _radius*_maxScale;
		
		Vector3 pointA = m.MultiplyPoint(_center - _endPoint);
		Vector3 pointB = m.MultiplyPoint(_center + _endPoint);
		
		if(radius > 0f){
			Vector3 n = radius*(pointB - pointA).normalized;
			Vector3 t = new Vector2(n.y, -n.x);
			
			Gizmos.DrawWireSphere(pointA, radius);
			Gizmos.DrawWireSphere(pointB, radius);
			Gizmos.DrawLine(pointA + t, pointB + t);
			Gizmos.DrawLine(pointA - t, pointB - t);
		} else {
			Gizmos.DrawLine(pointA, pointB);
		}
	}
}


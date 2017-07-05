// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;	
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;


/// Chipmunk circle shape type.
public class ChipmunkCircleShape : ChipmunkShape {
	public float _radius = 0.5f;
	
	/// Radius of circle relative to it's transform.
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			if(_handle != IntPtr.Zero){
				CP.cpCircleShapeSetRadius(_handle, _maxScale*value);
				CP.cpSpaceReindexShape(space._handle, _handle);
				
				if(body != null) body.Activate();
			}
		}
	}
	
	public Vector2 _center = Vector2.zero;
	
	/// Center of the circle relative to it's transform;
	public Vector2 center {
		get { return _center; }
		set {
			_center = value;
			if(_handle != IntPtr.Zero){
				Vector2 offset = BodyRelativeMatrix(body).MultiplyPoint(_center);
				CP.cpCircleShapeSetOffset(_handle, offset);
				CP.cpSpaceReindexShape(space._handle, _handle);
				
				if(body != null) body.Activate();
			}
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		base.Awake();
		
		Vector2 offset = BodyRelativeMatrix(body).MultiplyPoint(_center);
		
		_handle = CP.cpCircleShapeNew(IntPtr.Zero, _maxScale*_radius, offset);
		if(body != null) body._AddMassForShape(this);
		
		var gch = GCHandle.Alloc(this);
		CP._cpShapeSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	public override ChipmunkBody _UpdatedTransform(){
		UpdateParentBody();
		
		// Force the properties to update themselves.
		this.radius = this.radius;
		this.center = this.center;
		
		return body;
	}
	
	protected void OnDrawGizmosSelected(){
		Gizmos.color = this.gizmoColor;
		
		Transform t = this.transform;
		Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, _maxScale*Vector3.one);
		
		Gizmos.DrawWireSphere(_center, _radius);
	}
}


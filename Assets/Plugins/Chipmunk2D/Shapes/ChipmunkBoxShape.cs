// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;


/// Chipmunk box collision shape type.
public class ChipmunkBoxShape : ChipmunkShape {
	protected Vector2[] MakeVerts(){
		float hw = _size.x/2f;
		float hh = _size.y/2f;
		Matrix4x4 bmatrix = BodyRelativeMatrix(body);
		
		return new Vector2[]{
			bmatrix.MultiplyPoint3x4(new Vector2(_center.x - hw, _center.y - hh)),
			bmatrix.MultiplyPoint3x4(new Vector2(_center.x + hw, _center.y - hh)),
			bmatrix.MultiplyPoint3x4(new Vector2(_center.x + hw, _center.y + hh)),
			bmatrix.MultiplyPoint3x4(new Vector2(_center.x - hw, _center.y + hh)),
		};
	}
	
	protected void UpdateVerts(){
		if(_handle != IntPtr.Zero){
			var transformed = MakeVerts();
			CP.UpdateConvexPolyShapeWithVerts(_handle, transformed.Length, transformed);
			CP.cpPolyShapeSetRadius(_handle, _maxScale*_radius);
			CP.cpSpaceReindexShape(space._handle, _handle);
			
			if(body) body.Activate();
		}
	}
		
	public Vector2 _center = Vector2.zero;
	
	/// Center of the box relative to it's transform.
	public Vector2 center {
		get { return _center; }
		set {
			_center = value;
			UpdateVerts();
		}
	}
	
	public Vector2 _size = Vector2.one;
	
	/// Dimensions of the box relative to it's transform.
	public Vector2 size {
		get { return _size; }
		set {
			_size = value;
			UpdateVerts();
		}
	}
	
	public float _radius = 0f;
	
	/// Beveling radius of a box shape relative to it's transform.
	/// This is the extra thickness added onto the outside of the box's dimensions.
	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			UpdateVerts();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		base.Awake();
		
		var verts = MakeVerts();
		_handle = CP.NewConvexPolyShapeWithVerts(verts.Length, verts);
		CP.cpPolyShapeSetRadius(_handle, _maxScale*_radius);
		if(body != null) body._AddMassForShape(this);
		
		var gch = GCHandle.Alloc(this);
		CP._cpShapeSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	public override ChipmunkBody _UpdatedTransform(){
		UpdateParentBody();
		UpdateVerts();
		
		return body;
	}
	
	protected void OnDrawGizmosSelected(){
		Gizmos.color = this.gizmoColor;
		Gizmos.matrix = transform.localToWorldMatrix;
		
		float scale = 2f*_radius*_maxScale;
		Vector2 localScale = transform.localScale;
		Gizmos.DrawWireCube(_center, _size + new Vector2(scale/localScale.x, scale/localScale.y));
	}	
}

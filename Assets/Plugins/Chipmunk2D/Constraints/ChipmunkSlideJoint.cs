// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Similar to a pin joint, but the distance between the anchors is constrained to a certain range.
public class ChipmunkSlideJoint : ChipmunkConstraint {
	public Vector2 _anchr1 = new Vector2(0f, 0.5f);
	
	/// The location of the anchor point on the parent body.
	public Vector2 anchr1 {
		get { return _anchr1; }
		set {
			_anchr1 = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public Vector2 _anchr2 = new Vector2(0f, 1f);
	
	/// The location of the anchor point on bodyB.
	public Vector2 anchr2 {
		get { return _anchr2; }
		set {
			_anchr2 = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _min = 0.5f;
	
	/// The minimum distance between the anchor points.
	public float min {
		get { return _min; }
		set {
			_min = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _max = 1f;
	
	/// The maximum distance between the anchor points.
	public float max {
		get { return _max; }
		set {
			_max = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpSlideJointNew(IntPtr.Zero, IntPtr.Zero, Vector2.zero, Vector2.zero, 0f, 1e9f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		Vector2 p1 = transform.TransformPoint(anchr1);
		Vector2 p2 = transform.TransformPoint(anchr2);
		
		CP._cpSlideJointSetAnchr1(_handle, CP._cpBodyWorld2Local(handleA, p1));
		CP._cpSlideJointSetAnchr2(_handle, CP._cpBodyWorld2Local(handleB, p2));
		CP._cpSlideJointSetMin(_handle, min);
		CP._cpSlideJointSetMax(_handle, max);
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = GizmoColor;
		
		if(_handle == IntPtr.Zero){
			Vector2 p1 = transform.TransformPoint(anchr1);
			Vector2 p2 = transform.TransformPoint(anchr2);
			Gizmos.DrawLine(p1, p2);
		} else {
			Vector2 p1 = CP._cpBodyLocal2World(handleA, CP._cpSlideJointGetAnchr1(_handle));
			Vector2 p2 = CP._cpBodyLocal2World(handleB, CP._cpSlideJointGetAnchr2(_handle));
			Gizmos.DrawLine(p1, p2);
		}
	}
}

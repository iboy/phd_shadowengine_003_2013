// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Distance constraint that holds a point on two bodies a specified distance apart.
/// Think of it as connecting the anchor points of two bodies using a weightless pin or rod.
/// The length of the constraint is calculated on initialization and when anchor points are modified.
public class ChipmunkPinJoint : ChipmunkConstraint {
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
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpPinJointNew(IntPtr.Zero, IntPtr.Zero, Vector2.zero, Vector2.zero);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		Vector2 p1 = transform.TransformPoint(anchr1);
		Vector2 p2 = transform.TransformPoint(anchr2);
		
		CP._cpPinJointSetAnchr1(_handle, CP._cpBodyWorld2Local(handleA, p1));
		CP._cpPinJointSetAnchr2(_handle, CP._cpBodyWorld2Local(handleB, p2));
		CP._cpPinJointSetDist(_handle, Vector2.Distance(p1, p2));
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = GizmoColor;
		
		if(_handle == IntPtr.Zero){
			Vector2 p1 = transform.TransformPoint(anchr1);
			Vector2 p2 = transform.TransformPoint(anchr2);
			Gizmos.DrawLine(p1, p2);
		} else {
			Vector2 p1 = CP._cpBodyLocal2World(handleA, CP._cpPinJointGetAnchr1(_handle));
			Vector2 p2 = CP._cpBodyLocal2World(handleB, CP._cpPinJointGetAnchr2(_handle));
			Gizmos.DrawLine(p1, p2);
		}
	}
}

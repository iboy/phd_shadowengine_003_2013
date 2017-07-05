// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Constrains two bodies to rotate around a shared pivot point.
public class ChipmunkPivotJoint : ChipmunkConstraint {
	public Vector2 _pivot = new Vector2(0f, 1f);
	
	/// The location of the pivot point relative to the parent body.
	public Vector2 pivot {
		get { return _pivot; }
		set {
			_pivot = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpPivotJointNew2(IntPtr.Zero, IntPtr.Zero, Vector2.zero, Vector2.zero);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		Vector2 p = transform.TransformPoint(pivot);
		CP._cpPivotJointSetAnchr1(_handle, CP._cpBodyWorld2Local(handleA, p));
		CP._cpPivotJointSetAnchr2(_handle, CP._cpBodyWorld2Local(handleB, p));
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.DrawIcon(transform.TransformPoint(pivot), "ChipmunkJointIcon.psd", true);
	}
}

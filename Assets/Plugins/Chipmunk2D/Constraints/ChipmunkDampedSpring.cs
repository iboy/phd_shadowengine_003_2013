// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Works like a regular compression spring + damper.
public class ChipmunkDampedSpring : ChipmunkConstraint {
	public Vector2 _anchr1 = new Vector2(0f, 0.5f);
	
	/// Anchor point on the first body in local transform coordinates where the spring is attached.
	public Vector2 anchr1 {
		get { return _anchr1; }
		set {
			_anchr1 = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public Vector2 _anchr2 = new Vector2(0f, 1f);
	
	/// Anchor point on the second body in local transform coordinates where the spring is attached.
	public Vector2 anchr2 {
		get { return _anchr2; }
		set {
			_anchr2 = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _restLength = 0f;
	
	/// Length of the spring where it's at equilibrium.
	public float restLength {
		get { return _restLength; }
		set {
			_restLength = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _stiffness = 10f;
	
	/// Stiffness of the spring.
	public float stiffness {
		get { return _stiffness; }
		set {
			_stiffness = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _damping = 0f;
	
	/// Amount of linear damping applied by the spring.
	public float damping {
		get { return _damping; }
		set {
			_damping = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpDampedSpringNew(IntPtr.Zero, IntPtr.Zero, Vector2.zero, Vector2.zero, 10f, 10f, 10f);
//		_handle = CP.cpDampedSpringNew(IntPtr.Zero, IntPtr.Zero, new Vector2(10f, 10f), new Vector2(10f, 10f), 0f, 1f, 0f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		Vector2 p1 = transform.TransformPoint(anchr1);
		Vector2 p2 = transform.TransformPoint(anchr2);
		
//		CP._cpDampedSpringSetAnchr1(_handle, new Vector2(10f, 10f));
//		CP._cpDampedSpringSetAnchr2(_handle, new Vector2(0f, 10f));
		CP._cpDampedSpringSetAnchr1(_handle, CP._cpBodyWorld2Local(handleA, p1));
		CP._cpDampedSpringSetAnchr2(_handle, CP._cpBodyWorld2Local(handleB, p2));
		CP._cpDampedSpringSetRestLength(_handle, _restLength);
		CP._cpDampedSpringSetStiffness(_handle, _stiffness);
		CP._cpDampedSpringSetDamping(_handle, _damping);
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = GizmoColor;
		
		if(_handle == IntPtr.Zero){
			Vector2 p1 = transform.TransformPoint(anchr1);
			Vector2 p2 = transform.TransformPoint(anchr2);
			Gizmos.DrawLine(p1, p2);
		} else {
			Vector2 p1 = CP._cpBodyLocal2World(handleA, CP._cpDampedSpringGetAnchr1(_handle));
			Vector2 p2 = CP._cpBodyLocal2World(handleB, CP._cpDampedSpringGetAnchr2(_handle));
			Gizmos.DrawLine(p1, p2);
		}
	}
}

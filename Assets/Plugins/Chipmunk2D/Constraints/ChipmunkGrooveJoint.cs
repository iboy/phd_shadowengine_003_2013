// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Works like a peg that slides along a straight groove in a second body.
public class ChipmunkGrooveJoint : ChipmunkConstraint {
	public Vector2 _grooveA = new Vector2(1f, 1f);
	
	/// The first endpoint of the groove on the parent body.
	public Vector2 grooveA {
		get { return _grooveA; }
		set {
			_grooveA = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public Vector2 _grooveB = new Vector2(-1f, 1f);
	
	/// The second endopoint of the groove on the parent body.
	public Vector2 grooveB {
		get { return _grooveB; }
		set {
			_grooveB = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public Vector2 _anchr2 = new Vector2(0f, 1f);
	
	/// The anchor point on bodyB.
	public Vector2 anchr2 {
		get { return _anchr2; }
		set {
			_anchr2 = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpGrooveJointNew(IntPtr.Zero, IntPtr.Zero, Vector2.zero, Vector2.zero, Vector2.zero);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		Vector2 p1 = transform.TransformPoint(grooveA);
		Vector2 p2 = transform.TransformPoint(grooveB);
		Vector2 p3 = transform.TransformPoint(anchr2);
		
		CP.cpGrooveJointSetGrooveA(_handle, CP._cpBodyWorld2Local(handleA, p1));
		CP.cpGrooveJointSetGrooveB(_handle, CP._cpBodyWorld2Local(handleA, p2));
		CP._cpGrooveJointSetAnchr2(_handle, CP._cpBodyWorld2Local(handleB, p3));
	}
	
	public void OnDrawGizmosSelected(){
		Gizmos.color = GizmoColor;
		
		if(_handle == IntPtr.Zero){
			Vector2 p1 = transform.TransformPoint(grooveA);
			Vector2 p2 = transform.TransformPoint(grooveB);
			Vector2 p3 = transform.TransformPoint(anchr2);
			
			Gizmos.DrawLine(p1, p2);
			Gizmos.DrawIcon(p3, "ChipmunkJointIcon.psd", true);
		} else {
			Vector2 p1 = CP._cpBodyLocal2World(handleA, CP._cpGrooveJointGetGrooveA(_handle));
			Vector2 p2 = CP._cpBodyLocal2World(handleA, CP._cpGrooveJointGetGrooveB(_handle));
			Vector2 p3 = CP._cpBodyLocal2World(handleB, CP._cpGrooveJointGetAnchr2(_handle));
			
			Gizmos.DrawLine(p1, p2);
			Gizmos.DrawIcon(p3, "ChipmunkJointIcon.psd", true);
		}
	}
}

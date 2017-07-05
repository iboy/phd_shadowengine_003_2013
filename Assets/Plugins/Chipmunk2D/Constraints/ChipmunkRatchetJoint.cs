// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Constrains the angles of two bodies like a ratcheting socket wrench.
public class ChipmunkRatchetJoint : ChipmunkConstraint {
	public float _angle = 0f;
	
	/// The current angle of the ratchet mechanism in degrees.
	public float angle {
		get { return _angle; }
		set {
			_angle = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _phase = 0f;
	
	/// The phase offset where the ratchet engages at in degrees.
	public float phase {
		get { return _phase; }
		set {
			_phase = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _ratchet = 0f;
	
	/// The angle between "clicks" of the ratchet in degrees.
	public float ratchet {
		get { return _ratchet; }
		set {
			_ratchet = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpDampedRotarySpringNew(IntPtr.Zero, IntPtr.Zero, 0f, 0f, 0f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		CP._cpRatchetJointSetAngle(_handle, _angle*Mathf.Deg2Rad);
		CP._cpRatchetJointSetPhase(_handle, _phase*Mathf.Deg2Rad);
		CP._cpRatchetJointSetRatchet(_handle, _ratchet*Mathf.Deg2Rad);
	}
}

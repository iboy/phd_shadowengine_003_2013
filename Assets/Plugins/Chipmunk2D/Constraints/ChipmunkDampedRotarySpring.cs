// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Works like a torsion spring + damper.
public class ChipmunkDampedRotarySpring : ChipmunkConstraint {
	public float _restAngle = 0f;
	
	/// Relative angle between the bodies in degrees where the spring is at equilibrium.
	public float restAngle {
		get { return _restAngle; }
		set {
			_restAngle = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _stiffness = 0f;
	
	/// Stiffness of the spring.
	public float stiffness {
		get { return _stiffness; }
		set {
			_stiffness = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _damping = 0f;
	
	/// Amount of angular damping applied.
	public float damping {
		get { return _damping; }
		set {
			_damping = value;
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
		
		CP._cpDampedRotarySpringSetRestAngle(_handle, _restAngle*Mathf.Deg2Rad);
		CP._cpDampedRotarySpringSetStiffness(_handle, _stiffness*Mathf.Deg2Rad);
		CP._cpDampedRotarySpringSetDamping(_handle, _damping);
	}
}

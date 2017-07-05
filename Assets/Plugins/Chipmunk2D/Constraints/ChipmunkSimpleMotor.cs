// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Force a specific relative angular velocity between two bodies.
public class ChipmunkSimpleMotor : ChipmunkConstraint {
	public float _rate = 0f;
	
	/// The relative angular velocity between two bodies in degrees per second.
	public float rate {
		get { return _rate; }
		set {
			_rate = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpSimpleMotorNew(IntPtr.Zero, IntPtr.Zero, 0f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		CP._cpSimpleMotorSetRate(_handle, _rate*Mathf.Deg2Rad);
	}
}

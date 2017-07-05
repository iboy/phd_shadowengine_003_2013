// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Limit the relative rotation of two bodies to a certain range.
public class ChipmunkRotaryLimitJoint : ChipmunkConstraint {
	public float _min = -180f;
	
	/// The minimum relative angle between the two bodies in degrees.
	public float min {
		get { return _min; }
		set {
			_min = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _max = 180f;
	
	/// The maximum relative angle between two bodies in degrees.
	public float max {
		get { return _max; }
		set {
			_max = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpRotaryLimitJointNew(IntPtr.Zero, IntPtr.Zero, 0f, 0f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		CP._cpRotaryLimitJointSetMin(_handle, _min*Mathf.Deg2Rad);
		CP._cpRotaryLimitJointSetMax(_handle, _max*Mathf.Deg2Rad);
	}
}

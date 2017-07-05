// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// Forces one body to rotate at a specific ratio of another.
public class ChipmunkGearJoint : ChipmunkConstraint {
	public float _phase = 0f;
	
	/// The initial phase offset of the bodies' rotations in degrees.
	public float phase {
		get { return _phase; }
		set {
			_phase = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _ratio = 0f;
	
	/// The gear ratio.
	public float ratio {
		get { return _ratio; }
		set {
			_ratio = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
		
		_handle = CP.cpGearJointNew(IntPtr.Zero, IntPtr.Zero, 0f, 0f);
		
		var gch = GCHandle.Alloc(this);
		CP._cpConstraintSetUserData(_handle, GCHandle.ToIntPtr(gch));
	}
	
	protected override void UpdateConstraint(){
		base.UpdateConstraint();
		
		CP._cpGearJointSetPhase(_handle, _phase*Mathf.Deg2Rad);
		CP.cpGearJointSetRatio(_handle, _ratio);
	}
}

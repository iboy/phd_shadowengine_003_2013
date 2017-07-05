// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// ChipmunkConstraints are things like joints or motors that
/// constrain the motion of two bodies relative to each other.
public abstract class ChipmunkConstraint : ChipmunkBinding.Base {
	protected ChipmunkSpace space;
	protected ChipmunkBody bodyA;
	
	public ChipmunkBody _bodyB;
	
	/// The other body that the constraint attaches to.
	public ChipmunkBody bodyB {
		get { return _bodyB; }
		set {
			_bodyB = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	protected IntPtr handleA {
		get {
			if(bodyA){
				return bodyA.handle;
			} else {
				throw new Exception("No ChipmunkBody on this or any parent node.");
			}
		}
	}
	
	protected IntPtr handleB {
		get {
			if(bodyB){
				return bodyB.handle;
			} else {
				return Chipmunk.manager._space._staticBody;
			}
		}
	}
	
	public float _maxForce = Mathf.Infinity;
	
	/// Maximum force the constraint is allowed to use.
	/// Defaults to infinity.
	public float maxForce {
		get { return _maxForce; }
		set {
			_maxForce = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _errorBias = Mathf.Pow(1f - 0.1f, 60f);
	
	/// Rate that error (stretching of the constraint) is fixed.
	/// Defaults to 0.9^60 (Fix 10% of the error every 1/60th of a second).
	public float errorBias {
		get { return _errorBias; }
		set {
			_errorBias = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	public float _maxBias = Mathf.Infinity;
	
	/// Maximum rate that constraint error is fixed.
	/// This allows you to make constraints act like servos.
	public float maxBias {
		get { return _maxBias; }
		set {
			_maxBias = value;
			if(_handle != IntPtr.Zero) UpdateConstraint();
		}
	}
	
	/// Get the impulse applied by the constraint on the previous time step.
	/// Divide by Time.fixedDeltaTime to get the force.
	public float impulse {
		get { return (_handle != IntPtr.Zero ? CP._cpConstraintGetImpulse(_handle) : 0f); }
	}
	
	
	protected virtual void UpdateConstraint(){
		if(space != null) space._Remove(this);
		CP.ConstraintSetBodies(_handle, handleA, handleB);
		if(space != null) space._Add(this);
		
		CP._cpConstraintSetMaxForce(_handle, _maxForce);
		CP._cpConstraintSetErrorBias(_handle, _errorBias);
		CP._cpConstraintSetMaxBias(_handle, _maxBias);
	}

	protected void OnEnable(){
		if(_handle == IntPtr.Zero){
			Debug.LogError("ChipmunkConstraint handle is not set.");
			return;
		}
		
		bodyA = this.GetComponentUpwards<ChipmunkBody>();
		
		CP.ConstraintSetBodies(_handle, handleA, handleB);
		UpdateConstraint();
		
		// add to space:
		space = Chipmunk.manager._space;
		space._Add(this);
	}
	
	protected void OnDisable(){
		space._Remove(this);
		space = null;
	}
	
	protected override void OnDestroy(){
//		Debug.Log("Destroying a Constraint.");
		if(_handle == IntPtr.Zero){
			Debug.LogError("ChipmunkConstraint handle is already null.");
			return;
		}
		
		if(CP._cpConstraintGetSpace(_handle) != IntPtr.Zero){
			Debug.LogError("Attempted to destroy a ChipmunkConstraint that was still added to a space.");
			return;
		}
		
		var gch = GCHandle.FromIntPtr(CP._cpConstraintGetUserData(_handle));
		if(gch.Target != this) Debug.Log("ChipmunkConstaint handle does not match");
		gch.Free();
	}
	
	~ChipmunkConstraint(){
		CP.cpConstraintFree(_handle);
	}
	
	// Most constraints do nothing on transform changes.
	public override ChipmunkBody _UpdatedTransform(){
		var bodyA = this.GetComponentUpwards<ChipmunkBody>();
		UpdateConstraint();
		
		return null;
	}
	
	protected static Color GizmoColor = new Color(0.5f, 1.0f, 0.5f, 1.0f);
}

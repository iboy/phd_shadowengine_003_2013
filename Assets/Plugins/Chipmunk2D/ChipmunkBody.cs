// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

/// Editor enumeration used to specify how the moment of inertia of a ChipmunkBody should be calculated.
public enum ChipmunkBodyMomentMode {
	/// Calculate the moment of inertia from the shapes attached to the body.
	CalculateFromShapes,
	
	/// Give the body an infinite moment of inertia so it cannot rotate.
	DoNotRotate,
	
	/// Use the custom moment of inertia value specified in the editor.
	Override,
}

/// Used to specify if a ChipmunkBody should have it's position and rotation interpolated outside of the fixed time step.
public enum ChipmunkBodyInterpolationMode {
	/// Don't interpolate the body's position.
	None,
	
	/// Extrapolate the body's position.
	Extrapolate
	
	// Actual Interpolation will be added in the future.
}


/// A ChipmunkBody is the Chipmunk equivalent of Unity's Ridigbody component.
public partial class ChipmunkBody : ChipmunkBinding.Base {
	public float _mass = 1.0f;
	
	/// How the moment of inertia should be calculated when the object is created.
	/// The default value is CalculateFromShapes.
	/// If you change this value, you'll need to call Chipmunk.UpdateTransform() on the gameObject.
	public ChipmunkBodyMomentMode momentCalculationMode = ChipmunkBodyMomentMode.CalculateFromShapes;
	public float _customMoment = Mathf.Infinity;
	
	public bool _isKinematic = false;
	public bool isKinematic {
		get { return _isKinematic; }
		set {
			_isKinematic = value;
			if(value){
				if(this.enabled) space._Remove(this);
				CP.cpBodySetMass(_handle, Mathf.Infinity);
				CP.cpBodySetMoment(_handle, Mathf.Infinity);
				
				// TODO is this a bad idea?
				this.velocity = Vector2.zero;
				this.angularVelocity = 0.0f;
			} else {
				if(this.enabled) space._Add(this);
				this.mass = _mass;
				this.moment = _customMoment;
			}
		}
	}
	
	public ChipmunkBodyInterpolationMode _interpolationMode = ChipmunkBodyInterpolationMode.None;
	
	/// How the transform position should be set before Update() methods are called.
	/// Defaults to None.
	public ChipmunkBodyInterpolationMode interpolationMode {
		get { return _interpolationMode; }
		set {
			_interpolationMode = value;
			ChipmunkInterpolationManager._Register(this);
		}
	}
	
	
	[HideInInspector]
	public Transform _transform;
	
	protected override void Awake(){
		if(_handle != IntPtr.Zero) return;
//		Debug.Log ("ChipmunkBody Awake: " + gameObject.name);
		
		_handle = CP.cpBodyNew(0f, 0f);
		var gch = GCHandle.Alloc(this);
		CP._cpBodySetUserData(_handle, GCHandle.ToIntPtr(gch));
		
		_UpdatedTransform();
	}
	
	public void _AddMassForShape(ChipmunkShape shape){
		CP.cpBodyAddMassForShape(this.handle, shape.handle, 1f);
//		Debug.Log("Added shape to body. mass: " + mass + " moment: " + moment + ")");
	}
	
	protected void RescaleMass(){
		// Rescale the mass back to the user value.
		float calculated = CP._cpBodyGetMass(_handle);
		CP.cpBodySetMass(_handle, _mass);
		
//		Debug.Log("Rescaling body mass. (mass: " + calculated + " moment: " + CP._cpBodyGetMoment(_handle) + ")");
		
		if(calculated > 0f && momentCalculationMode == ChipmunkBodyMomentMode.CalculateFromShapes){
			float moment = CP._cpBodyGetMoment(_handle);
			this.moment = moment*_mass/calculated;
		} else if(momentCalculationMode == ChipmunkBodyMomentMode.Override){
			this.moment = _customMoment;
		} else {
			this.moment = Mathf.Infinity;
		}
		
		// Cache the calculated moment.
		_customMoment = this.moment;
		if(_isKinematic){
			CP.cpBodySetMass(_handle, Mathf.Infinity);
			CP.cpBodySetMoment(_handle, Mathf.Infinity);
		}
		
//		Debug.Log("Rescaled body mass. (mass: " + this.mass + " moment: " + this.moment + ")");
	}
	
	[HideInInspector]
	public float _savedZ;
	
	protected void Start(){
		RescaleMass();
		_savedZ = transform.position.z;
	}
	
	protected ChipmunkSpace space;
	
	protected void OnEnable(){
		// add to space:
		space = Chipmunk.manager._space;
		if(!_isKinematic) space._Add(this);
		ChipmunkInterpolationManager._Register(this);
	}
	
	public void OnDisable(){
		if(!_isKinematic) space._Remove(this);
		ChipmunkInterpolationManager._Register(this);
		space = null;
	}
	
	public void _RecalculateMass(){
		CP.cpBodySetMass(_handle, 0f);
		CP.cpBodySetMoment(_handle, 0f);
		this.cogOffset = Vector2.zero;
		
		foreach(var shape in GetComponentsInChildren<ChipmunkShape>()){
			// Only add the mass contribution if the shape is actuall attached to this body.
			if(this == shape.body) _AddMassForShape(shape);
		}
		
		RescaleMass();
	}
	
	public override ChipmunkBody _UpdatedTransform(){
		CP.cpBodySetPos(_handle, transform.position);
		CP.cpBodySetAngle(_handle, transform.eulerAngles.z*Mathf.Deg2Rad);
		
		_transform = base.transform;
		
		return this;
	}
	
	protected override void OnDestroy(){
//		Debug.Log("Destroying a body: " + _handle);
		if(_handle == IntPtr.Zero){
			Debug.LogError("ChipmunkBody handle is already null.");
			return;
		}
		
		if(!_isKinematic && CP._cpBodyGetSpace(_handle) != IntPtr.Zero){
			Debug.LogError("Attempted to destroy a ChipmunkBody that was still added to a space.");
			return;
		}
		
		var gch = GCHandle.FromIntPtr(CP._cpBodyGetUserData(_handle));
		if(gch.Target != this) Debug.Log("ChipmunkBody handle does not match");
		gch.Free();
	}
	
	~ChipmunkBody(){
		CP.cpBodyFree(_handle);
	}
	
	public void OnDrawGizmosSelected(){
		if(_handle != IntPtr.Zero){
			Gizmos.DrawIcon(transform.TransformPoint(cogOffset), "ChipmunkBodyIcon.png", true);
		}
	}
	
	/// Force a body to wake up if it has fallen asleep.
	public void Activate(){
		if(_handle != IntPtr.Zero) CP.cpBodyActivate(_handle);
	}
	
	/// Force a body to fall asleep immediately even if it's in motion or unsupported.
	public void Sleep(){
		if(_handle != IntPtr.Zero) CP.cpBodySleep(_handle);
	}
	
//	public void Sleep(ChipmunkBody group){
//		// TODO handle errors here.
//		if(_handle != IntPtr.Zero) CP.cpBodySleepWithGroup(_handle, group._handle);
//	}
	
	/// Position of the body.
	/// NOTE: Changing a body's position causes it to "teleport" to the new location.
	/// Setting the position of a kinematic body will also update it's velocity,
	/// but will cause issues if you modify the position multiple times in a single update.
	public Vector2 position {
		get { return (_handle != IntPtr.Zero ? CP._cpBodyGetPos(_handle) : Vector2.zero); }
		set {
			_transform.position = value;
			if(_handle != IntPtr.Zero){
				if(_isKinematic){
					Vector2 velocity = (value - CP._cpBodyGetPos(_handle))/Time.deltaTime;
					CP._cpBodySetVel(_handle, velocity);
				}
				CP.cpBodySetPos(_handle, value);
			}
		}
	}
	
	/// Angle of the body in radians.
	public float angle {
		get { return (_handle != IntPtr.Zero ? CP._cpBodyGetAngle(_handle) : 0f); }
		set {
			_transform.rotation = Quaternion.AngleAxis(value*Mathf.Rad2Deg, Vector3.forward);
			if(_handle != IntPtr.Zero){
				if(_isKinematic){
					float velocity = (value - CP._cpBodyGetAngle(_handle))/Time.deltaTime;
					CP._cpBodySetAngVel(_handle, velocity);
				}
				CP.cpBodySetAngle(_handle, value);
			}
		}
	}
	
	/// Mass of the body.
	public float mass {
		get { return _mass; }
		set {
			_mass = value;
			if(_handle != IntPtr.Zero && !this.isKinematic){
				ChipmunkBinding.cpBodySetMass(_handle, value);
			}
		}
	}
	
	/// Moment of inertia of the body. (Like a mass for it's rotation)
	public float moment {
		get { return _customMoment; }
		set {
			_customMoment = value;
			if(_handle != IntPtr.Zero && !this.isKinematic){
				ChipmunkBinding.cpBodySetMoment(_handle, value);
			}
		}
	}
	
	/// Is the body currently sleeping?
	public bool isSleeping {
		get { return (_handle != IntPtr.Zero ? CP._cpBodyIsSleeping(_handle) : false); }
		set {
			if(_handle != IntPtr.Zero){
				if(value) Sleep(); else Activate();
			}
		}
	}
	
	/// Get the KE of the body in units of mass * velocity^2.
	public float kineticEnergy {
		get { return (_handle != IntPtr.Zero ? CP._cpBodyKineticEnergy(_handle) : 0f); }
	}
	
	/// The offset of the center of gravity from the transform's origin.
	public Vector2 cogOffset {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetCOGOffset(_handle) : Vector2.zero); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetCOGOffset(_handle, value); }
	}
	
	/// The linear velocity of the center of gravity of the body.
	public Vector2 velocity {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetVel(_handle) : Vector2.zero); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetVel(_handle, value); }
	}
	
	/// The linear force applied to the center of gravity of the body.
	/// Does not automatically reset to 0 after a fixed time step.
	public Vector2 force {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetForce(_handle) : Vector2.zero); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetForce(_handle, value); }
	}
	
	/// The angular velocity of the rigid body.
	public float angularVelocity {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetAngVel(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetAngVel(_handle, value); }
	}
	
	/// The torque applied to the rigid body.
	/// Does not automatically reset to 0 after a fixed time step.
	public float torque {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetTorque(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetTorque(_handle, value); }
	}
	
	/// An upper limit on the magnitude of the linear velocity for the body.
	/// Useful for preventing bodies from moving too quickly and becoming uncontrollable.
	public float velocityLimit {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetVelLimit(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetVelLimit(_handle, value); }
	}

	/// An upper limit on the magnitude of the angular velocity for the body.
	/// Useful for preventing bodies from rotating too quickly and becoming uncontrollable.
	public float angularVelocityLimit {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpBodyGetAngVelLimit(_handle) : 0f); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpBodySetAngVelLimit(_handle, value); }
	}
	
	/// Reset the force and torque applied to this body.
	public void ResetForces(){
		if(_handle != IntPtr.Zero) CP.cpBodyResetForces(_handle);
	}
	
	/// Apply (accumulate) a force on the body at the given point in the transform's local coords.
	public void ApplyForce( Vector2 force, Vector2 localPosition){
		if(_handle != IntPtr.Zero){
			var offset = _transform.TransformDirection(localPosition);
			CP.cpBodyApplyForce(_handle, force, offset);
		}
	}
	 
	/// Apply an impulse on the body at the given point in the transform's local coords.
	public void ApplyImpulse(Vector2 impulse, Vector2 localPosition){
		if(_handle != IntPtr.Zero){
			var offset = _transform.TransformDirection(localPosition);
			CP.cpBodyApplyImpulse(_handle, impulse, offset);
		}
	}
	
	/// Get the velocity of a specific point on the rigid body as specified in world coords.
	public Vector2 VelocityAtWorldPoint(Vector2 position){
		var offset = Vector2.Scale(_transform.InverseTransformPoint(position), _transform.localScale);
		return (_handle != IntPtr.Zero ? CP.cpBodyGetVelAtWorldPoint(_handle, offset) : Vector2.zero);
	}
	
	/// Get the velocity of a specific point on the rigid body as specified in the transform's local coords.
	public Vector2 VelocityAtLocalPoint(Vector2 localPosition){
		var offset = Vector2.Scale(localPosition, _transform.localScale);
		return (_handle != IntPtr.Zero ? CP.cpBodyGetVelAtLocalPoint(_handle, offset) : Vector2.zero);
	}
	
	/// Delegate type used with EachArbiter().
	public delegate void EachArbiterDelegate(ChipmunkArbiter arbiter);
	
	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	private static extern void _EachArbiter(IntPtr handle, EachArbiterDelegate del);
	
	/// Iterate over all of the arbiters associated with this body.
	/// The delegate is called once for each arbiter.
	public void EachArbiter(EachArbiterDelegate del){
		_EachArbiter(this.handle, del);
	}
}

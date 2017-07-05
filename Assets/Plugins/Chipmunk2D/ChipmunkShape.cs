// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using CP = ChipmunkBinding;
using System.Runtime.InteropServices;


/// A ChipmunkShape is the Chipmunk equivalent of Unity's Collider component.
public abstract partial class ChipmunkShape : ChipmunkBinding.Base {
	protected ChipmunkSpace space;
	
	public static ChipmunkShape _FromHandle(IntPtr handle){
		if(handle == IntPtr.Zero){
			return null;
		} else {
			return GCHandle.FromIntPtr(CP._cpShapeGetUserData(handle)).Target as ChipmunkShape;
		}
	}
	
	/// ChipmunkBody that this shape is attached to.
	public ChipmunkBody body { get; protected set; }
	
	public float _friction = 0.7f;
	
	/// Friction of this shape.
	/// The final coefficient of friction for a collision is computed by multiplying the friction
	/// the two shapes together. This can be overridden in a ChipmunkCollisionManager preSolve function. 
	public float friction {
		get { return _friction; }
		set {
			_friction = value;
			if(_handle != IntPtr.Zero) CP._cpShapeSetFriction(_handle, value);
		}
	}
	
	public float _elasticity = 0f;
	
	/// Elasticity of this shapes.
	/// The final elasticity for a collision is computed by multiplying the elasticity
	/// the two shapes together. This can be overridden in a ChipmunkCollisionManager preSolve function. 
	public float elasticity {
		get { return _elasticity; }
		set {
			_elasticity = value;
			if(_handle != IntPtr.Zero) CP._cpShapeSetElasticity(_handle, value);
		}
	}
	
	public bool _isSensor = false;
	
	/// Sensor shapes are collision shapes that trigger @CollisionManager@ callbacks, but don't produce collision forces.
	/// They are basically the same as Unity trigger colliders.
	public bool isSensor {
		get { return _isSensor; }
		set {
			_isSensor = value;
			if(_handle != IntPtr.Zero) CP._cpShapeSetSensor(_handle, value);
		}
	}
	
	public string _collisionType = "";
	
	/// Collision ID string used when defining ChipmunkCollisionManager methods.
	public string collisionType {
		get { return _collisionType; }
		set {
			_collisionType = value;
			
			var id = ChipmunkBinding.InternString(value);
			if(_handle != IntPtr.Zero) CP._cpShapeSetCollisionType(_handle, id);
		}
	}
	
	public string _collisionGroup = "";
	
	/// String that identifies the group a collision shape is in.
	/// Shapes in the same group do not collide with one another.
	public string collisionGroup {
		get { return _collisionGroup; }
		set {
			_collisionGroup = value;
			
			var id = ChipmunkBinding.InternString(value);
			if(_handle != IntPtr.Zero) CP._cpShapeSetGroup(this.handle, id);
		}
	}
	
	/// A bitmask of which Chipmunk layers (Not the same as Unity layers) that a shape is in.
	/// Two shapes will collide if they share at least one layer.
	public uint layers {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpShapeGetLayers(_handle) : 0); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpShapeSetLayers(_handle, value); }
	}
	
	/// The velocity of the surface of a shape separate from its rigid body's velocity.
	/// This is useful for things like conveyor belts, where the surface of a shape is moving even though the shape is not.
	/// Another example is a player's feet. The simplified player collision shape doesn't change or move, but the feet (surface) do.
	public Vector2 surfaceVelocity {
		get { return (_handle != IntPtr.Zero ? ChipmunkBinding._cpShapeGetSurfaceVelocity(_handle) : Vector2.zero); }
		set { if(_handle != IntPtr.Zero) ChipmunkBinding._cpShapeSetSurfaceVelocity(_handle, value); }
	}
	
	protected override void Awake(){
		body = this.GetComponentUpwards<ChipmunkBody>();
	}
	
	protected void OnEnable(){
		if(_handle == IntPtr.Zero){
			Debug.LogError("ChipmunkShape handle is not set.");
			return;
		}
		
		space = Chipmunk.manager._space;

		// add body to shape
		CP.cpShapeSetBody(_handle, body == null ? space._staticBody : body.handle);
		this.friction = _friction;
		this.elasticity = _elasticity;
		this.collisionGroup = _collisionGroup;
		this.collisionType = _collisionType;
		this.isSensor = _isSensor;
		
		// add to space:
		space._Add(this);
	}
	
	protected void OnDisable(){
		space._Remove(this);
		space = null;
	}
	
	protected override void OnDestroy(){
//		Debug.Log("Destroying a Shape.");
		if(_handle == IntPtr.Zero){
			Debug.LogError("ChipmunkShape handle is already null.");
			return;
		}
		
		if(CP._cpShapeGetSpace(_handle) != IntPtr.Zero){
			Debug.LogError("Attempted to destroy a shape that was still added to a space.");
			return;
		}
		
		var gch = GCHandle.FromIntPtr(CP._cpShapeGetUserData(_handle));
		if(gch.Target != this) Debug.Log("ChipmunkShape handle does not match");
		gch.Free();
	}
	
	protected void UpdateParentBody(){
		var body = this.GetComponentUpwards<ChipmunkBody>();
		if(body != this.body && space != null){
			space._Remove(this);
			CP.cpShapeSetBody(_handle, body == null ? space._staticBody : body.handle);
			space._Add(this);
		}
		
		this.body = body;
	}
	
	~ChipmunkShape(){
		CP.cpShapeFree(_handle);
	}
	
	public float _maxScale {
		get {
			Vector3 scale = this.transform.lossyScale;
			return Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y));
		}
	}
	
	protected static Color GizmoStatic = new Color(1.0f, 0.5f, 0.5f, 1.0f);
	protected static Color GizmoDynamic = new Color(1.0f, 1.0f, 0.5f, 1.0f);
	protected static Color GizmoSensor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	protected static Color GizmoSleeping = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	
	protected Color gizmoColor {
		get {
			if(_isSensor){
				return GizmoSensor;
			} else if(body != null){
				return (body.isSleeping ? GizmoSleeping : GizmoDynamic);
			} else {
				return GizmoStatic;
			}
		}
	}
}

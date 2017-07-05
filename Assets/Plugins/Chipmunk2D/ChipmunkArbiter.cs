// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CP = ChipmunkBinding;
using System;
using System.Runtime.InteropServices;

/// A ChipmunkArbiter contains collision information for two colliding ChipmunkShapes.
/// You cannot create them and only recieve them in ChipmunkCollisionManager methods or ChipmunkBody.EachArbiter callbacks.
/// WARNING: Do not store a ChipmunkArbiter reference to be used outside the
/// method it was passed to since it referenes memory managed by C code.
public struct ChipmunkArbiter {
	public IntPtr _handle;
	
	/// This constructor only exists to get rid of a compiler warning.
	/// Arbiters will *always* be constructed in C code.
	public ChipmunkArbiter(IntPtr value){
		_handle = value;
		throw new Exception(
			"This constructor only exists to get rid of a compiler warning. " +
			"Arbiters will *always* be constructed from C code."
		);
	}
	
	/// The number of contact points between the colliding shapes.
	public int contactCount {
		get { return CP.cpArbiterGetCount(_handle); }
	}
	
	/// The size and direction of the impulse applied by this arbiter.
	/// NOTE: The value will only be valid inside of an ChipmunkCollisionManager PostSolve method.
	public Vector2 impulse {
		get { return CP.cpArbiterTotalImpulse(_handle); }
	}
	
	/// Same as the impulse, but also includes the impulse from friction.
	public Vector2 impulseWithFriction {
		get { return CP.cpArbiterTotalImpulseWithFriction(_handle); }
	}
	
	/// Kinetic energy lost to heat/deformation during the collision.
	/// For perfectly elastic collisions, this will always be 0.
	/// Units are in mass*velocity^2.
	public float kineticEnergy {
		get { return CP.cpArbiterTotalKE(_handle); }
	}
	
	[DllImport(CP.IMPORT)] private static extern void _cpArbiterGetShapes(IntPtr arb, out IntPtr shapeA, out IntPtr shapeB);
	
	/// Get references to the two colliding shapes.
	/// The collision types will match the order of the ChipmunkCollisionManager method.
	/// Say you defined a ChipmunkCollisionManager method "ChipmunkBegin_player_enemy".
	/// Then shapeA.collisionType == "player" and shapeB.collisionType == "enemy"
	public void GetShapes(out ChipmunkShape shapeA, out ChipmunkShape shapeB){
		IntPtr _shapeA, _shapeB;
		_cpArbiterGetShapes(_handle, out _shapeA, out _shapeB);
		
		shapeA = ChipmunkShape._FromHandle(_shapeA);
		shapeB = ChipmunkShape._FromHandle(_shapeB);
	}
	
	/// Modify the computed elasticity of the colliding shapes.
	/// Defaults to shapeA.elasticity*shapeB.elasticity
	/// NOTE: This must be done in a ChipmunkCollisionManager pre-solve method.
	public float elasticity {
		get { return CP._cpArbiterGetElasticity(_handle); }
		set { CP._cpArbiterSetElasticity(_handle, value); }
	}
	
	/// Modify the computed friction of the colliding shapes.
	/// Defaults to shapeA.friction*shapeB.friction
	/// NOTE: This must be done in a ChipmunkCollisionManager pre-solve method.
	public float friction {
		get { return CP._cpArbiterGetFriction(_handle); }
		set { CP._cpArbiterSetFriction(_handle, value); }
	}
	
	/// Modify the computed relative surface velocity of the colliding shapes.
	/// Defaults to shapeB.surfaceVelocity - shapeA.surfaceVelocity and then projected onto the collision plane.
	/// NOTE: This must be done in a ChipmunkCollisionManager pre-solve method.
	public Vector2 surfaceVelocity {
		get { return -CP.cpArbiterGetSurfaceVelocity(_handle); }
		set { CP.cpArbiterSetSurfaceVelocity(_handle, -value); }
	}
	
	/// Returns true if this is the first fixed step where the objects were colliding.
	/// i.e. It will always return true the same step where the ChipmunkCollisionManager begin method is called.
	public bool isFirstContact {
		get { return CP.cpArbiterIsFirstContact(_handle); }
	}
	
	/// Begin ignoring this colliding pair of shapes *after* the current fixed step.
	/// The ChipmunkCollisionManager separate method will still be called,
	/// but pre/post-solve will not and no collision forces will be applied.
	/// Effectively the same as returning false from a ChipmunkCollisionManager begin method.
	public void Ignore(){
		CP.cpArbiterIgnore(_handle);
	}
	
	/// Get the position of the ith contact point.
	public Vector2 GetPoint(int i){
		if(0 <= i && i < contactCount){
			return CP.cpArbiterGetPoint(_handle, i);
		} else {
			Debug.LogError("Index out of bounds");
			return Vector2.zero;
		}
	}
	
	/// Get the normal of the ith contact point.
	public Vector2 GetNormal(int i){
		if(0 <= i && i < contactCount){
			return CP.cpArbiterGetNormal(_handle, i);
		} else {
			Debug.LogError("Index out of bounds");
			return Vector2.zero;
		}
	}
	
	/// Get the penetration depth (negative value) of the ith contact point.
	public float GetDepth(int i){
		if(0 <= i && i < contactCount){
			return CP.cpArbiterGetDepth(_handle, i);
		} else {
			Debug.LogError("Index out of bounds");
			return 0f;
		}
	}
}

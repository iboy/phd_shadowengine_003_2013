// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CP = ChipmunkBinding;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// Used with Chipmunk.NearestPointQueryNearest().
public struct ChipmunkNearestPointQueryInfo {
	public IntPtr _shapeHandle;
	
	/// The nearest point on the shapes's surface to the query point.
	public Vector2 point;
	
	/// The distance of the query point to the nearest point.
	public float distance;
	
	/// The normalized direction from the query point to the closest point on the shape's surface.
	public Vector2 gradient;
	
	/// The shape that was nearest to the query point.
	public ChipmunkShape shape {
		get { return ChipmunkShape._FromHandle(_shapeHandle); }
	}
}

/// Used with Chipmunk.SegmentQueryFirst().
public struct ChipmunkSegmentQueryInfo {
	public IntPtr _shapeHandle;
	
	/// The normalized value (from 0 to 1) between the start and end points where the segment query hit.
	public float t;
	
	/// The surface normal of the shape where it was struck by the segment query.
	public Vector2 normal;
	
	/// The shape that was first struck by the segment query.
	public ChipmunkShape shape {
		get { return ChipmunkShape._FromHandle(_shapeHandle); }
	}
}

/// Similar in function to Unity's Physics class, but for working with Chipmunk2D.
public static class Chipmunk {
	public static ChipmunkManager _manager;
	public static ChipmunkInterpolationManager _interpolationManager;
	
	private static ChipmunkManager CreateManager(){
		var manager = GetManager();
		
//		Debug.Log("Configuring new space.");
		if(manager._space != null) Debug.LogError("Space was already set?");
		ChipmunkSpace space = manager._space = new ChipmunkSpace();
		
		space.gravity = Physics.gravity;
		space.iterations = Physics.solverIterationCount;
		space.collisionSlop = Physics.minPenetrationForPenalty;
		space.sleepTimeThreshold = 0.5f;
		
		return manager;
	}
	
	private static ChipmunkManager GetManager(){
		var go = GameObject.Find("ChipmunkManager");
		ChipmunkManager manager;
		
		if(go == null){
//			Debug.Log("Creating new ChipmunkManager.");
			
			go = new GameObject("ChipmunkManager");
			manager = go.AddComponent<ChipmunkManager>();
		} else {
//			Debug.Log("Found existing ChipmunkManager.");
			
			manager = go.GetComponent<ChipmunkManager>();
			
			if(manager == null){
				Debug.LogError("A ChipmunkManager game object already exists but does not have a ChipmunkManager component attached.");
				Debug.Break();
			}
		}
		
		return manager;
	}
	
	public static ChipmunkManager manager {
		get {
			if(_manager == null){
				_manager = CreateManager();
				_interpolationManager = _manager.gameObject.AddComponent<ChipmunkInterpolationManager>();
			}
			
			return _manager;
		}
	}
	
	/// Amount of gravity to use.
	/// Is an alias for Physics.gravity.
	public static Vector2 gravity {
		get { return Physics.gravity; }
		set {
			manager._space.gravity = value;
			Physics.gravity = value;
		}
	}
	
	/// Number of iterations to use in the solver. 
	/// Is an alias for Physics.solverIterationCount.
	public static int solverIterationCount {
		get { return Physics.solverIterationCount; }
		set {
			manager._space.iterations = value;
			Physics.solverIterationCount = value;
		}
	}
	
	/// Amount of allowed overlap of physics shapes.
	/// Is an alias for Physics.minPenetrationForPenalty.
	public static float minPenetrationForPenalty {
		get { return Physics.minPenetrationForPenalty; }
		set {
			manager._space.collisionSlop = value;
			Physics.minPenetrationForPenalty = value;
		}
	}
	
	/// Amount of damping to apply to velocity and angularVelocity.
	/// A value of 0.9 for instance means that the body
	/// will have 90% of it's velocity after a second. 
	public static float damping {
		get { return manager._space.damping; }
		set { manager._space.damping = value; }
	}
	
	/// When objects are moving slower than this for longer
	/// than the sleepTimeThreshold they are considered idle.
	/// The default idleSpeedThreshold is set based on the amount of gravity.
	public static float idleSpeedThreshold {
		get { return manager._space.idleSpeedThreshold; }
		set { manager._space.idleSpeedThreshold = value; }
	}
	
	/// When objects are moving slower than the idleSpeedThreshold for longer
	/// than the sleepTimeThreshold they are considered idle.
	/// The sleepTimeThreshold default is 0.5
	public static float sleepTimeThreshold {
		get { return manager._space.sleepTimeThreshold; }
		set { manager._space.sleepTimeThreshold = value; }
	}
	
	/// Rate at which overlapping objects are pushed apart.
	/// Defaults to 0.9^60, meaning it will fix 10% of overlap per 1/60th second.
	public static float collisionBias {
		get { return manager._space.collisionBias; }
		set { manager._space.collisionBias = value; }
	}
	
	/// Unity doesn't provide notification events for when a transform is modified.
	/// Unfortunately that means that you need to call this to let Chipmunk know when you modify a transform.
	public static void UpdatedTransform(GameObject root){
		var bodies = new HashSet<ChipmunkBody>();
		
		foreach(var component in root.GetComponentsInChildren<ChipmunkBinding.Base>()){
			var affectedBody = component._UpdatedTransform();
			if(affectedBody != null) bodies.Add(affectedBody);
		}
		
		// Update the mass properties of the bodies.
		foreach(var body in bodies) body._RecalculateMass();
	}
	
	//MARK: Nearest Point Query
	
	/// Delegate type to use with nearest point queries.
	/// Parmeters passed are the shape, the nearest point on the surface of that shape, and the distance to the query point.
	public delegate void NearestPointQueryDelegate(ChipmunkShape shape, float dist, Vector2 point);
	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	private static extern void _NearestPointQuery(IntPtr handle, Vector2 point, float maxDist, uint layers, int group, NearestPointQueryDelegate del);
	
	/// Iterate all the shapes within 'maxDist' of 'point' by calling 'del' for each.
	/// Shapes are filtered using the group and layers in the same way as collisions.
	public static void NearestPointQuery(Vector2 point, float maxDist, uint layers, string group, NearestPointQueryDelegate del){
		var handle = manager._space._handle;
		_NearestPointQuery(handle, point, maxDist, layers, ChipmunkBinding.InternString(group), del);
	}
	
	/// Calls NearestPointQuery() with all layers and no group.
	public static void NearestPointQuery(Vector2 point, float maxDist, NearestPointQueryDelegate del){
		NearestPointQuery(point, maxDist, ~(uint)0, "", del);
	}
	
	[DllImport(CP.IMPORT)] private static extern IntPtr
	cpSpaceNearestPointQueryNearest(IntPtr handle, Vector2 point, float maxDistance, uint layers, int group, out ChipmunkNearestPointQueryInfo info);
	
	/// Return only information about the nearest shape to the query point.
	/// Shapes are filtered using the group and layers in the same way as collisions.
	public static ChipmunkShape NearestPointQueryNearest(Vector2 point, float maxDistance, uint layers, string group, out ChipmunkNearestPointQueryInfo info){
		var handle = manager._space._handle;
		return ChipmunkShape._FromHandle(cpSpaceNearestPointQueryNearest(handle, point, maxDistance, layers, ChipmunkBinding.InternString(group), out info));
	}
	
	/// Calls NearestPointQueryNearest() with all layers an no group.
	public static ChipmunkShape NearestPointQueryNearest(Vector2 point, float maxDistance, out ChipmunkNearestPointQueryInfo info){
		return NearestPointQueryNearest(point, maxDistance, ~(uint)0, "", out info);
	}
	
	//Mark: Segment Queries
	
	/// Delegate type to use with segment queries.
	/// Parameters passed are the shape, the fraction along the query segment and the normal of the intersection with the shape.
	public delegate void SegmentQueryDelegate(ChipmunkShape shape, float fraction, Vector2 normal);
	
	[MethodImplAttribute(MethodImplOptions.InternalCall)]
	private static extern void _SegmentQuery(IntPtr handle, Vector2 start, Vector2 end, uint layers, int group, SegmentQueryDelegate del);
	
	/// Find all shapes overlapping the segment with the given start and end points.
	/// Shapes are filtered using the group and layers in the same way as collisions.
	public static void SegmentQuery(Vector2 start, Vector2 end, uint layers, string group, SegmentQueryDelegate del){
		var handle = manager._space._handle;
		_SegmentQuery(handle, start, end, layers, ChipmunkBinding.InternString(group), del);
	}
	
	/// Calls SegmentQuery() with all layers and no group.
	public static void SegmentQuery(Vector2 start, Vector2 end, SegmentQueryDelegate del){
		SegmentQuery(start, end, ~(uint)0, "", del);
	}
	
	[DllImport(CP.IMPORT)] private static extern IntPtr
	cpSpaceSegmentQueryFirst(IntPtr handle, Vector2 start, Vector2 end, uint layers, int group, out ChipmunkSegmentQueryInfo info);
	
	/// Return only the first shape struck by the segment query as it goes from start to end.
	/// Shapes are filtered using the group and layers in the same way as collisions.
	public static ChipmunkShape SegmentQueryFirst(Vector2 start, Vector2 end, uint layers, string group, out ChipmunkSegmentQueryInfo info){
		IntPtr handle = manager._space._handle;
		return ChipmunkShape._FromHandle(cpSpaceSegmentQueryFirst(handle, start, end, layers, ChipmunkBinding.InternString(group), out info));
	}
	
	/// Calls SegmentQueryFirst() with all layers and no group.
	public static ChipmunkShape SegmentQueryFirst(Vector2 start, Vector2 end, out ChipmunkSegmentQueryInfo info){
		return SegmentQueryFirst(start, end, ~(uint)0, "", out info);
	}
}

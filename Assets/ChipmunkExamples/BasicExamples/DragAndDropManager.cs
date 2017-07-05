// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class DragAndDropManager : MonoBehaviour {
	
	// People have fat fingers, so we want to give a little radius to our touch. 
	// This makes it much easier to grab small objects.
	float fingerThickness = 0.3f;
	
	// Handle input by moving a body around to match the cursor (or touch)
	public ChipmunkBody mouseBody;
	// Then connect the mouse to whatever you clicked on.
	private ChipmunkPivotJoint mouseConstraint;
	
	private ChipmunkShape _grabbedShape;
	public ChipmunkShape grabbedShape{
		get{
			return _grabbedShape;
		}
		set{
			if(value != _grabbedShape){
				// Add gameplay actions here when you grab a new shape.
				_grabbedShape = value;			
			}
		}
	}
	
	public Vector2 GetPosition(Vector2 mouseOrTouchPosition){	
		// We want to transform this screen position "mouseOrTouchPosition" to world coordinates.
		// However, you might be using orthographic or perspective cameras
		// or even a perspective camera not perpendicular to the plane of action.
			
		// So manually cast a ray to the plane of action.
		Ray ray = Camera.mainCamera.ScreenPointToRay(mouseOrTouchPosition);
		Vector3 worldPos = CastRayToPlane(ray);

		return worldPos;
	}
	
	public Vector3 CastRayToPlane(Ray ray){
		float distance = ray.origin.z/ray.direction.z;
		Vector3 zeroCross = ray.origin - ray.direction*distance;
		return zeroCross;
	}
	
	protected void FixedUpdate(){
		mouseBody.position = GetPosition(Input.mousePosition);
	}
	
	protected void Update(){
		// Works for the mouse or a single touch. If doing multitouch, replace with with a loop over Input.touches
		// and adapt accordingly
		if(Input.GetMouseButtonDown(0)){
			
			//Debug.Log ("mouse: "+ Input.mousePosition + " position: " + GetPosition(Input.mousePosition));
			
			ChipmunkNearestPointQueryInfo info;
			Chipmunk.NearestPointQueryNearest(mouseBody.position, fingerThickness, out info);
			//Debug.Log ("Grabbed shape: " + info.shape);

			if(info.shape != null){
				
				if(info.shape.body == null){
					// We clicked on a static shape. You can't drag those!
					return;	
				}
				
				grabbedShape = info.shape;
				
				mouseConstraint = mouseBody.gameObject.AddComponent(typeof(ChipmunkPivotJoint)) as ChipmunkPivotJoint;
				mouseConstraint.bodyB = grabbedShape.body;
				mouseConstraint.pivot = Vector2.zero;
				
				// high but not infinite. You can push heavy things, but not force yourself through things.
				mouseConstraint.maxForce = 1e3f;
				
				// 60f = the approximate intended frame rate
				mouseConstraint.errorBias = Mathf.Pow(1.0f - 0.15f, 60f);
			}
		}
		
		if(Input.GetMouseButtonUp(0)){
			// remove mouse constraint.
			Destroy (mouseConstraint);
		}
	}
	
	/*
	 * Unused in this example, but useful when you're casting a 
	 * ray against an arbitrary plane.
	 * 

	protected Vector3 CheckRay(Ray ray, Transform target){
		var origin = target.InverseTransformPoint(ray.origin);
		var direction = target.InverseTransformDirection(ray.direction);
		
		Vector3 zeroCross = origin - direction*(origin.z/direction.z);
		return zeroCross;
	}
	*/

}

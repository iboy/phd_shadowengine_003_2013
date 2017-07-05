// Ian Grant
// Display touch points
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchTracker : MonoBehaviour {
	
	
	public GameObject[] TouchPointSprites;
	public GameObject[] TouchPointControllers;
	public bool displayTouchPointsSwitch = true;


	private Vector2[] touchPosition;

	private Vector3 mousePosition;



	public float touchOverlayZ 	=  	6f;
	public float trackSpeed 	= 	0.1f;


	public float 	spring 		=	50.0f;
	public float  	damper 		= 	5.0f;
	public float  	drag 		= 	10.0f;
	public float  	angularDrag = 	5.0f;
	public float  	distance 	= 	0.2f;
	public bool 	attachTouchDragsToCenterOfMass = false;
	public Camera mainCamera;
	public float perspectiveMultiplier = 1.0f;

	//static List<GameObject> controllerTouchObjects = new List<GameObject>();
	private SpringJoint springJoint;
	//int controllerTouchObjectsSize;

	void Start() {

		foreach (GameObject touchGraphic in TouchPointSprites) {
			touchGraphic.SetActive(false);
		}


	}


	void Update() {
	

		// Track a single touch as a direction control.
		if (Input.touchCount > 0)
		{
			//Touch touch = Input.GetTouch(0);
			foreach (Touch touch in Input.touches) {


				if (displayTouchPointsSwitch) {
					DisplayTouchPoints(touch);
				}
				//RaycastHit hit;

				TouchToController(touch);

				//DragrigidbodyWithTouch(touch);
				//fingerCount++;
			}
		}

	}
	
	
	void DisplayTouchPoints ( Touch touch ) {



		//Debug.Log ("Handle Touch: "+touch.fingerId+" Began");

		switch (touch.phase) {

		case TouchPhase.Began:

			//Debug.Log ("Handle Touch: "+touch.fingerId+" Began");
			TouchPointSprites[touch.fingerId].SetActive(true);
			break;

		case TouchPhase.Moved:
			//Debug.Log ("Handle Touch: "+touch.fingerId+" Moved");

			Vector3 touchDeltaPosition = touch.position; // remember get touch is zero indexed.
			touchDeltaPosition = Camera.main.ScreenToWorldPoint(touchDeltaPosition);
			TouchPointSprites[touch.fingerId].transform.position = new Vector3(touchDeltaPosition.x*perspectiveMultiplier, touchDeltaPosition.y*perspectiveMultiplier, touchOverlayZ-2);
			break;
			

		case TouchPhase.Ended:
			//Debug.Log ("Handle Touch: "+touch.fingerId+" Ended");
			TouchPointSprites[touch.fingerId].SetActive(false);
			break;
		}
	}


	void TouchToController ( Touch touch ) {
		
		
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
		
		//bool dragit = false; // flag to signal we're ready to drag...
		RaycastHit hit = new RaycastHit();



		//Debug.Log ("Handle Touch: "+touch.fingerId+" Began");
		
		switch (touch.phase) {
			
		case TouchPhase.Began:
		
			//Debug.Log ("Handle Touch: "+touch.fingerId+" Began");
			TouchPointControllers[touch.fingerId].SetActive(true);

//			int humanTouchCount = touch.fingerId+1;
//			string rigidbodyName = "Rigidbody dragger for Touch "+humanTouchCount.ToString();
			
			//GameObject go = new GameObject (rigidbodyName);
		
			
			//hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				//Debug.Log ("Touch: "+touch.fingerId+"  hit"+hit.collider.name);
				
				
				
				
				if (hit.rigidbody) { // is it a rigidbody
					
					if(!hit.rigidbody.isKinematic) {  // is it NOT set to isKinematic? i.e not to move
						
						
						
						// float oldAngularDrag;
						
						//Debug.Log ("Touch: "+touch.fingerId+"  hit"+hit.collider.name);
						
						//dragit = true;
						

						
						
						
						//Rigidbody body = controllerTouchObjects[touch.fingerId].AddComponent ("Rigidbody") as Rigidbody;
						//	 springJoint = TouchPointControllers[touch.fingerId].AddComponent ("SpringJoint") as SpringJoint;
						//body.isKinematic = true;

						springJoint = TouchPointControllers[touch.fingerId].GetComponent<SpringJoint>();
						springJoint.transform.position = hit.point;
						
						// check where the anchor is supposed to go
						if (attachTouchDragsToCenterOfMass) {
							
							// check these values
							Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
							anchor = springJoint.transform.InverseTransformPoint(anchor);
							//anchor = springJoint.transform.TransformPoint(anchor);
							springJoint.anchor = anchor;
							
						} else {
							springJoint.anchor = Vector3.zero;
						}
						
						// set spring settings
						springJoint.spring = spring;
						springJoint.damper = damper;
						springJoint.maxDistance = distance;
						springJoint.connectedBody = hit.rigidbody;
						
						//StartCoroutine (DragObject( hit.distance, touch, dragit));
						
					} // end of kinematic test
					
				} // end of rigid body test
				
			} // end of first hit test
			
			else {
				
				//if (springJoint.connectedBody) { Destroy(springJoint.gameObject); } // destroys the object if it was connected to a kinematic rigidbody
				
			}


			break;
			
		case TouchPhase.Moved:
			//Debug.Log ("Handle Touch: "+touch.fingerId+" Moved");
			
			Vector3 touchDeltaPosition = touch.position; // remember get touch is zero indexed.
			touchDeltaPosition = Camera.main.ScreenToWorldPoint(touchDeltaPosition);
			TouchPointControllers[touch.fingerId].transform.position = new Vector3(touchDeltaPosition.x*perspectiveMultiplier, touchDeltaPosition.y*perspectiveMultiplier, TouchPointControllers[touch.fingerId].transform.position.z);
			break;
			
			
		case TouchPhase.Ended:
			//Debug.Log ("Handle Touch: "+touch.fingerId+" Ended");
			TouchPointControllers[touch.fingerId].SetActive(false);
			if (springJoint) {
				springJoint.connectedBody = null;
			}
			break;
		}
	}

	//IEnumerator DragObject(float distance, Touch touch, bool dragit) {
	//
	//	//Debug.Log ("Here we go!");
	//
	//
	//	float oldDrag = springJoint.connectedBody.drag;
	//	float oldAngularDrag = springJoint.connectedBody.angularDrag;
	//	springJoint.connectedBody.drag = drag;
	//	springJoint.connectedBody.angularDrag = angularDrag;
	//
	//	while (touch.phase != TouchPhase.Stationary || touch.phase != TouchPhase.Ended ) // controls when we exit the co-routine.
	//	{
	//
	//		//Debug.Log("We're in the coroutine");
	//
	//		Vector3 touchWorldPosition = touch.position;
	//		touchWorldPosition = mainCamera.ScreenToWorldPoint(touchWorldPosition);
	//		Ray ray = mainCamera.ScreenPointToRay(touchWorldPosition);
	//
	//		//touchWorldPosition = Camera.main.ScreenToWorldPoint(touchWorldPosition);
	//		//springJoint.transform.position = ray.GetPoint(distance);
	//
	//		//springJoint.transform.position = touchWorldPosition;
	//		//Debug.Log("Touch X "+ touchWorldPosition.x +"Touch Y "+ touchWorldPosition.y +"Touch Z "+ touchWorldPosition.z);
	//		//instead of moving the springjoint - move the gameobject
	//
	//
	//		//TouchPointSprites[touch.fingerId].transform.position = new Vector3(touchDeltaPosition.x, touchDeltaPosition.y, touchOverlayZ);
	//
	//		yield return null;
	//
	//	}
	//	if (springJoint.connectedBody)
	//	{
	//		springJoint.connectedBody.drag = oldDrag;
	//		springJoint.connectedBody.angularDrag = oldAngularDrag;
	//		springJoint.connectedBody = null;
	//		Debug.Log("Stopped moving in the coroutine.");
	//		//dragit = false;
	//		//if (go) { Destroy(go); }
	//
	//
	//
	//	}
	//
	//}

	
	
	
	
//	void DragrigidbodyWithTouch ( Touch touch ) {
//	
//		// this should run for every touch so somehow get a new id on the game object for every one!
//
//		//int humanTouchCount = touch.fingerId+1;
//		//string rigidbodyName = "Rigidbody dragger for Touch "+humanTouchCount.ToString();
//	
//		Ray ray = Camera.main.ScreenPointToRay(touch.position);
//	
//		bool dragit = false; // flag to signal we're ready to drag...
//		RaycastHit hit = new RaycastHit();
//		//GameObject go;
//		switch (touch.phase) {
//	
//		case TouchPhase.Began:
//			//RaycastHit hit = new RaycastHit();
//			//Debug.Log ("Handle Touch: "+touch.fingerId+" Began");
//			int humanTouchCount = touch.fingerId+1;
//			string rigidbodyName = "Rigidbody dragger for Touch "+humanTouchCount.ToString();
//
//			//GameObject go = new GameObject (rigidbodyName);
//			controllerTouchObjects.Insert (touch.fingerId, new GameObject (rigidbodyName));
//
//	
//			controllerTouchObjectsSize = controllerTouchObjects.Count;
//			Debug.Log("In TOUCH BEGAN: "+controllerTouchObjectsSize);
//
//
//
//			//hit = new RaycastHit();
//			if (Physics.Raycast(ray, out hit)) {
//				//Debug.Log ("Touch: "+touch.fingerId+"  hit"+hit.collider.name);
//
//
//			
//
//			if (hit.rigidbody) { // is it a rigidbody
//
//				if(!hit.rigidbody.isKinematic) {  // is it NOT set to isKinematic? i.e not to move
//		
//
//				
//					float oldAngularDrag;
//
//					//Debug.Log ("Touch: "+touch.fingerId+"  hit"+hit.collider.name);
//
//					dragit = true;
//					
//					//if (!springJoint) {
//
//
//
//							Rigidbody body = controllerTouchObjects[touch.fingerId].AddComponent ("Rigidbody") as Rigidbody;
//							SpringJoint springJoint = controllerTouchObjects[touch.fingerId].AddComponent ("SpringJoint") as SpringJoint;
//							body.isKinematic = true;
//					//	}
//
//						springJoint.transform.position = hit.point;
//
//					// check where the anchor is supposed to go
//					if (attachTouchDragsToCenterOfMass) {
//						
//							// check these values
//							Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
//							anchor = springJoint.transform.InverseTransformPoint(anchor);
//							//anchor = springJoint.transform.TransformPoint(anchor);
//							springJoint.anchor = anchor;
//
//					} else {
//							springJoint.anchor = Vector3.zero;
//					}
//
//					// set spring settings
//						springJoint.spring = spring;
//						springJoint.damper = damper;
//						springJoint.maxDistance = distance;
//						springJoint.connectedBody = hit.rigidbody;
//
//					//StartCoroutine (DragObject( hit.distance, touch, dragit));
//
//					} // end of kinematic test
//			
//				} // end of rigid body test
//
//			} // end of first hit test
//
//			else {
//
//				//if (springJoint.connectedBody) { Destroy(springJoint.gameObject); } // destroys the object if it was connected to a kinematic rigidbody
//
//			}
//
//
//			break;
//		case TouchPhase.Moved:
//			if (dragit == true) {
//			dragit = true;
//			Vector3 touchWorldPosition = touch.position;
//			touchWorldPosition = mainCamera.ScreenToWorldPoint(touchWorldPosition);
//			//Ray ray = mainCamera.ScreenPointToRay(touchWorldPosition);
//			//Debug.Log("Touch X "+ touchWorldPosition.x +"Touch Y "+ touchWorldPosition.y);
//
//			
//				float oldDrag = springJoint.connectedBody.drag;
//				float oldAngularDrag = springJoint.connectedBody.angularDrag;
//				springJoint.connectedBody.drag = drag;
//				springJoint.connectedBody.angularDrag = angularDrag;
//
//			// go isn't in scope...
//			// find it - this works
//				springJoint.gameObject.transform.position = new Vector3(touchWorldPosition.x, touchWorldPosition.y, touchOverlayZ);
//
//			//Vector3 touchWorldPosition = touch.position;
//			//touchWorldPosition = mainCamera.ScreenToWorldPoint(touchWorldPosition);
//			//Ray ray = mainCamera.ScreenPointToRay(touchWorldPosition);
//			}
//
//			break;
//		case TouchPhase.Stationary:
//			break;
//
//		case TouchPhase.Ended:
//
//
//			if (dragit == true) {
//				if (springJoint.connectedBody) {
//
//					float oldDrag = springJoint.connectedBody.drag;
//					float oldAngularDrag = springJoint.connectedBody.angularDrag;
//					springJoint.connectedBody.drag = oldDrag;
//					springJoint.connectedBody.angularDrag = oldAngularDrag;
//					springJoint.connectedBody = null;
//					//Destroy(springJoint.gameObject);
//					//Debug.Log("Stopped moving in the coroutine.");
//					//dragit = false;
//					//if (go) { Destroy(go); }
//				
//				
//				}
//			//if (go) { Destroy(go); }
//			//if (dragit == true) { Destroy(springJoint.gameObject); dragit = false;}
//			}
//
//			controllerTouchObjects.RemoveAt (touch.fingerId);
//			controllerTouchObjectsSize = controllerTouchObjects.Count;
//			Debug.Log("In TOUCH END: "+controllerTouchObjectsSize);
//			break;
//		} // end of switch
//
//
//
//	}
	
	
}
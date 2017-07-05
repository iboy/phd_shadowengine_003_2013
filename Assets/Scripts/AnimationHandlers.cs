using UnityEngine;
using System.Collections;

public class AnimationHandlers : MonoBehaviour {
	
	// can this list of objects be created in another way?
	// programmatically in an array or hash of all touchable / dragable / objects
	//
	public GameObject object1; // bird head
	public GameObject object2; // bird body
	public GameObject object3; // snake head
	public GameObject object4; // snake tail
	public GameObject object3c; // snake head controller
	public GameObject object4c; // snake tail controller
	public GameObject object5; // snake root
	public GameObject object6;
	public float multiplier =1;
	private bool myTouch = false;
	private float x1;
	private float y1;
	private float z1;
	private float x2;
	private float y2;
	private float z2;
	private float x3;
	private float y3;
	private float z3;
	private float x4;
	private float y4;
	private float z4;
	private bool object1Exists = true;
	private bool object2Exists = true;
	private bool object3Exists = true;
	private bool object4Exists = true;
	private bool Pad2PuppetController1Touch1 = false;
	private bool Pad2PuppetController1Touch2 = false;
	private bool Pad2PuppetController1Touch3 = false;
	private bool Pad2PuppetController2Touch1 = false;
	private bool Pad2PuppetController2Touch2 = false;
	// Use this for initialization
	void Start () {
	
	
	}
	
	// Update is called once per frame

	

// Move Object 1	
	// this is called from the OSC listener. Could it be made generic?
	// Principle is to separate the OSC logic from the animation control and from the object being controlled.
	// However, if the OSC control object has a single use - could this be more confusing and too complicated?

	public void MoveObject1(bool Pad2PuppetController1TouchFlag,  float xx1, float yy1, float zz1) {
		
		//check touch is true
		// there is no release event so do we have to create one?
		if (Pad2PuppetController1TouchFlag == true) {
		  	myTouch = true;
			Pad2PuppetController1Touch1 = true;
			//Debug.Log("touch is triggered");
		
		// move object1 using the incoming touch values
		if (object1 !=null) {
				// be sure to set some flags so fixed update doesn't try to transform an object that isn't active.


					//Debug.Log("Hello from AnimationHandlers - function DoFunctionInAnotherScript()");
					//Debug.Log ("Function DoFunctionInAnotherScript(): X="+x1);
					//Debug.Log ("Function DoFunctionInAnotherScript(): Y="+y1);
					//Debug.Log ("Function DoFunctionInAnotherScript(): Z="+z1);
				
					// send coords to the dragable script on the object we wish to drag
					// should this logic be in 'Dragable.cs'?
					
				if (yy1 < 0.0f) { yy1 = 0.2f ; }
				x1 = xx1*multiplier;
				y1 = yy1*multiplier;
				z1 = zz1*multiplier;

				object1Exists = true; // this needs to be controlled by the PREFAB INSTANTIATING ROUTINES

				//try to move the transform to fixed update or update - expose the coords
					//object1.transform.localPosition = new Vector3 (x1*multiplier, y1*multiplier, z1*multiplier);
			}
			
		} else {
			
			// this probably means the Touch event has been released?	
			Debug.Log("I think the bird doesn't exist");
			myTouch = false; 
			Pad2PuppetController1TouchFlag = false;

			Debug.Log(myTouch); 
			Pad2PuppetController1Touch1 = false;
			// call the equivalent
		
		}
		

	}

	
// Move Object 2	
	// this is called from the OSC listener. Could it be made generic?
	// Principle is to separate the OSC logic from the animation control and from the object being controlled.
	// However, if the OSC control object has a single use - could this be more confusing and too complicated?

	public void MoveObject2(bool Pad2PuppetController1TouchFlag,  float xx2, float yy2, float zz2) {
		
		//check touch is true
		// there is no release event so do we have to create one?
		if (Pad2PuppetController1TouchFlag == true) {
		  	myTouch = true;
			Pad2PuppetController1Touch2 = true;
			//Debug.Log("touch is triggered");
		
		// move object2 using the incoming touch values
		if (object2 !=null) {
				// be sure to set some flags so fixed update doesn't try to transform an object that isn't active.

					//Debug.Log("Hello from AnimationHandlers - function DoFunctionInAnotherScript()");
					//Debug.Log ("Function DoFunctionInAnotherScript(): X="+x1);
					//Debug.Log ("Function DoFunctionInAnotherScript(): Y="+y1);
					//Debug.Log ("Function DoFunctionInAnotherScript(): Z="+z1);
				
					// send coords to the dragable script on the object we wish to drag
					// should this logic be in 'Dragable.cs'?
						
				x2 = xx2*multiplier;
				y2 = yy2*multiplier;
				z2 = zz2*multiplier;
				//object2.transform.localPosition = new Vector3 (x2*multiplier, y2*multiplier, z2*multiplier);
			}
			
		} else {
			
			// this probably means the Touch event has been released?	
			
			myTouch = false; 
			Pad2PuppetController1TouchFlag = false; 
			Debug.Log(myTouch); 
			Pad2PuppetController1Touch2 = false;
			// call the equivalent
		
		}
		

	}

	// Move Object 2	
	// this is called from the OSC listener. Could it be made generic?
	// Principle is to separate the OSC logic from the animation control and from the object being controlled.
	// However, if the OSC control object has a single use - could this be more confusing and too complicated?
	
	public void MoveObjectDragonHead(bool Pad2PuppetController2Touch1Flag,  float xx3, float yy3, float zz3) {
		
		//check touch is true
		// there is no release event so do we have to create one?
		if (Pad2PuppetController2Touch1Flag == true) {
			myTouch = true;
			Pad2PuppetController2Touch1 = true;

			//Debug.Log("touch is triggered");
			
			// move object2 using the incoming touch values
			if (object3 !=null) {
				// be sure to set some flags so fixed update doesn't try to transform an object that isn't active.

				//Debug.Log("Hello from AnimationHandlers - function DoFunctionInAnotherScript()");
				//Debug.Log ("Function DoFunctionInAnotherScript(): X="+x1);
				//Debug.Log ("Function DoFunctionInAnotherScript(): Y="+y1);
				//Debug.Log ("Function DoFunctionInAnotherScript(): Z="+z1);
				
				// send coords to the dragable script on the object we wish to drag
				// should this logic be in 'Dragable.cs'?
				
				x3 = xx3*multiplier;
				y3 = yy3*multiplier;
				z3 = zz3*multiplier;
				object3.transform.localPosition = new Vector3 (x3, y3, z3);

			}
			
		} else {
			
			// this probably means the Touch event has been released?	
			/*
			myTouch = false; 
			Pad2PuppetController2Touch1Flag = false; 
			Debug.Log("Touch 1 (head) has been released" );
			object3.rigidbody2D.isKinematic = false; // TODO remove this from here when the instantiation business is working
			// call the equivalent
			Pad2PuppetController2Touch1 = false;
			*/
		}
		

	}
	
	public void MoveObjectDragonTail(bool Pad2PuppetController2Touch2Flag,  float xx4, float yy4, float zz4) {
		
		//check touch is true
		// there is no release event so do we have to create one?
		if (object4 !=null) {
		
			if (Pad2PuppetController2Touch2Flag == true) { // TODO count flags
				myTouch = true;
				Pad2PuppetController2Touch2 = true;
			

				// be sure to set some flags so fixed update doesn't try to transform an object that isn't active.

				//Debug.Log("Hello from AnimationHandlers - function DoFunctionInAnotherScript()");
				//Debug.Log ("Function DoFunctionInAnotherScript(): X="+x1);
				//Debug.Log ("Function DoFunctionInAnotherScript(): Y="+y1);
				//Debug.Log ("Function DoFunctionInAnotherScript(): Z="+z1);
				
				// send coords to the dragable script on the object we wish to drag
				// should this logic be in 'Dragable.cs'?
				
				x4 = xx4*multiplier;
				y4 = yy4*multiplier;
				z4 = zz4*multiplier;

				object4.transform.localPosition = new Vector3 (x4, y4, z4);
			} else {
			
			// this probably means the Touch event has been released?	
			
			myTouch = false; 
			Pad2PuppetController2Touch2Flag = false; 
			Debug.Log("Touch 2 (tail) has been released" ); 
			object4.rigidbody2D.isKinematic = false; // TODO remove this from here when the instantiation business is working
			// call the equivalent
			Pad2PuppetController2Touch2 = false;

		}

		}
		

	}

// LISTEN FOR ROTATION
	
	//void rotateObjectThrough180() {
	
	//myObject.transform.rotation.eulerAngles = new Vector3(0,180,0);
	//object1.transform.Rotate(Vector3.up * 180, Space.World);
	//}
		
	void FixedUpdate() {
	if (object3 != null) {
		if (myTouch == true) {

			//if (Pad2PuppetController1Touch1) {object1.transform.localPosition = new Vector3 (x1*multiplier, y1*multiplier, z1*multiplier);}
			//if (Pad2PuppetController1Touch2) {object2.transform.localPosition = new Vector3 (x2*multiplier, y2*multiplier, z2*multiplier);}

			// move the dragon // this should be done with torque and force or by attaching another drag-spring

		//		if (Pad2PuppetController2Touch1) {object3.transform.localPosition = new Vector3 (x3, y3, z3);} 
			//	if (Pad2PuppetController2Touch2) {object4.transform.localPosition = new Vector3 (x4*multiplier, y4*multiplier, z4*multiplier);} 

			//Debug.Log("myTouch = "+myTouch+ ". This is the fixed update of the animation handler class");	
		} else { 
			//Debug.Log ("myTouch == false");
			//myTouch = false; 
			//Debug.Log("myTouch = "+myTouch+ ". This is the fixed update of the animation handler class");

			
				//object3.rigidbody2D.isKinematic = false; //Debug.Log ("Running every physics frame");

				//object4.rigidbody2D.isKinematic = false; //Debug.Log ("Running every physics frame");



			}
		}
		
	}

	public void SetObject1(GameObject go) {
		
		object1 = go;
		
	}
	
	
	public void SetObject2(GameObject go) {
		
		object2 = go;
		
	}

	public void SetObject5(GameObject go) {
		
		object3 = go;
		
	}
	
	public void SetObject5c(GameObject go) {
		
		object3c = go;
		
	}

	public void SetObject6(GameObject go) {
		
		object4 = go;
		
	}

	public void SetObject6c(GameObject go) {
		
		object4c = go;
		
	}

	public void SetObject7(GameObject go) {
		
		object5 = go;
		
	}
	
}

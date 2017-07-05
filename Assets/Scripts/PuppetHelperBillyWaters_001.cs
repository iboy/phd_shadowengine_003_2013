using UnityEngine;
using System.Collections;

public class PuppetHelperBillyWaters_001 : MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	public GameObject MyRoot;
	public GameObject Part000;
	public GameObject Part001;
	public GameObject Part002;
	public GameObject Part003;
	public GameObject Part004;
	public GameObject Part005;
	public GameObject Part006;
	public GameObject Part007;
	public GameObject Part008;
	public GameObject Part009;
	public GameObject Part010;
	public GameObject ControllerStart;
	public GameObject ControllerEnd;

	public Transform ControllerOneHangState;
	public Transform ControllerTwoHangState;

	public bool is2D;

	public bool Rotated;

	// Use this for initialization
	private PuppetHelperBillyWaters_001 genericHelper;
	Vector3 myRotationNow;
	Vector3 myRotationTarget;
	bool facingLeft;
	bool rotating;
	public bool toggleHangState = false;

	// state
	Vector3 cachePosController1; 			
	Vector3 cachePosController2;	


	void Awake () {
		Debug.Log ("An instance of GenericHelper has been create: AWAKE");
		// I guess the instance isn't around at 'awake' time
		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");
		
		//go.GetComponent<UnityOSCListener>().HelloWorld("HelloFromTheNewBird");
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject5(Part010);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject6(Part000);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject5c(ControllerStart);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject6c(ControllerEnd);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject7(MyRoot);
		
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject5(Part010);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject6(Part000);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject5c(ControllerStart);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject6c(ControllerEnd);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject7(MyRoot);

		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_Dragon(MyRoot);


		if (is2D) {
			Part001.collider2D.transform.parent = Part001.transform; // Bug in Unity3D
			Part002.collider2D.transform.parent = Part002.transform; // Bug in Unity3D
			Part003.collider2D.transform.parent = Part003.transform; // Bug in Unity3D
			Part004.collider2D.transform.parent = Part004.transform; // Bug in Unity3D
			Part005.collider2D.transform.parent = Part005.transform; // Bug in Unity3D
			Part006.collider2D.transform.parent = Part006.transform; // Bug in Unity3D
			Part007.collider2D.transform.parent = Part007.transform; // Bug in Unity3D
			Part008.collider2D.transform.parent = Part008.transform; // Bug in Unity3D
			Part009.collider2D.transform.parent = Part009.transform; // Bug in Unity3D
			Part000.collider2D.transform.parent = Part000.transform; // Bug in Unity3D
			Part010.collider2D.transform.parent = Part010.transform; // Bug in Unity3D
		} else {
			if (Part001.collider) { Part001.collider.transform.parent = Part001.transform; } // Bug in Unity3D 
			if (Part002.collider) { Part002.collider.transform.parent = Part002.transform; } // Bug in Unity3D
			if (Part003.collider) { Part003.collider.transform.parent = Part003.transform; } // Bug in Unity3D
			if (Part004.collider) { Part004.collider.transform.parent = Part004.transform; } // Bug in Unity3D
			if (Part005.collider) { Part005.collider.transform.parent = Part005.transform; } // Bug in Unity3D
			if (Part006.collider) { Part006.collider.transform.parent = Part006.transform; } // Bug in Unity3D
			if (Part007.collider) { Part007.collider.transform.parent = Part007.transform; } // Bug in Unity3D
			if (Part008.collider) { Part008.collider.transform.parent = Part008.transform; } // Bug in Unity3D
			if (Part009.collider) { Part009.collider.transform.parent = Part009.transform; } // Bug in Unity3D
			if (Part000.collider) { Part000.collider.transform.parent = Part000.transform; } // Bug in Unity3D
			if (Part010.collider) { Part010.collider.transform.parent = Part010.transform; } // Bug in Unity3D

		}


	}
	
	void Start () {
		Debug.Log ("An instance of GenericHelper has been created: START");


		genericHelper = (PuppetHelperBillyWaters_001)FindObjectOfType(typeof(PuppetHelperBillyWaters_001));


		
	}




	// IsKinetic Toggles and Controls
	public void MakeKinematicTrueHeadBody() {

		if (is2D) {

		Part010.rigidbody2D.isKinematic     = true;
		Part000.rigidbody2D.isKinematic 	= true;


		} else {

			Part010.rigidbody.isKinematic     = true;
			Part000.rigidbody.isKinematic 	= true;
		}
	}	
	
	public void MakeKinematicFalseHeadBody() {

		if (is2D) {
		Part010.rigidbody2D.isKinematic     = false;
		Part000.rigidbody2D.isKinematic 	= false;
		} else {
			Part010.rigidbody.isKinematic     = false;
			Part000.rigidbody.isKinematic 	= false;
			
		}

	}
	
	public void MakeKinematicTrueBody() {

		if (is2D) {

			Part000.rigidbody2D.isKinematic = true;
		
		} else {

			Part000.rigidbody2D.isKinematic = true;

		}


	}	
	
	public void MakeKinematicFalseBody() {

			if (is2D) {

		Part000.rigidbody2D.isKinematic = false;

		} else {

			Part000.rigidbody.isKinematic = false;

		}


	}
	
	public void MakeKinematicTrueHead() {

				if (is2D) {
		Part010.rigidbody2D.isKinematic      = true;

		} else {

			Part010.rigidbody.isKinematic      = true;

		}


	}
	
	public void MakeKinematicFalseHead() {

		if (is2D) {
		
			Part010.rigidbody2D.isKinematic      = false;	
		
		} else {
			
			Part010.rigidbody.isKinematic      = false;
			
		}


	}	
	
	public void MakeKinematicFalseAll() {

	if (is2D == true) {

			Part000.rigidbody2D.isKinematic = false;
			Part001.rigidbody2D.isKinematic     = false;
			Part002.rigidbody2D.isKinematic     = false;
			Part003.rigidbody2D.isKinematic     = false;
			Part004.rigidbody2D.isKinematic 	= false;
			Part005.rigidbody2D.isKinematic 	= false;
			Part006.rigidbody2D.isKinematic 	= false;
			Part007.rigidbody2D.isKinematic 	= false;
			Part008.rigidbody2D.isKinematic 	= false;
			Part009.rigidbody2D.isKinematic 	= false;
			Part010.rigidbody2D.isKinematic = false;

	} else {

			Part000.rigidbody.isKinematic = false;
			Part001.rigidbody.isKinematic     = false;
			Part002.rigidbody.isKinematic     = false;
			Part003.rigidbody.isKinematic     = false;
			Part004.rigidbody.isKinematic 	= false;
			Part005.rigidbody.isKinematic 	= false;
			Part006.rigidbody.isKinematic 	= false;
			Part007.rigidbody.isKinematic 	= false;
			Part008.rigidbody.isKinematic 	= false;
			Part009.rigidbody.isKinematic 	= false;
			Part010.rigidbody.isKinematic = false;

		}

	}

		
		
	public void MakeKinematicTrueAll() {

	if (is2D) {
			Part000.rigidbody2D.isKinematic = true;
			Part001.rigidbody2D.isKinematic     = true;
			Part002.rigidbody2D.isKinematic     = true;
			Part003.rigidbody2D.isKinematic     = true;
			Part004.rigidbody2D.isKinematic 	= true;
			Part005.rigidbody2D.isKinematic 	= true;
			Part006.rigidbody2D.isKinematic 	= true;
			Part007.rigidbody2D.isKinematic 	= true;
			Part008.rigidbody2D.isKinematic 	= true;
			Part009.rigidbody2D.isKinematic 	= true;
			Part010.rigidbody2D.isKinematic = true;
		} else {
			Part000.rigidbody.isKinematic = true;
			Part001.rigidbody.isKinematic     = true;
			Part002.rigidbody.isKinematic     = true;
			Part003.rigidbody.isKinematic     = true;
			Part004.rigidbody.isKinematic 	= true;
			Part005.rigidbody.isKinematic 	= true;
			Part006.rigidbody.isKinematic 	= true;
			Part007.rigidbody.isKinematic 	= true;
			Part008.rigidbody.isKinematic 	= true;
			Part009.rigidbody.isKinematic 	= true;
			Part010.rigidbody.isKinematic = true;



		}


		
	}
	
	public void ToggleIsKinematicAll() {
		if (is2D) {
					
		//MyRoot.rigidbody2D.isKinematic    = !MyRoot.rigidbody2D.isKinematic;
			Part000.rigidbody2D.isKinematic = !Part000.rigidbody2D.isKinematic;
			Part001.rigidbody2D.isKinematic     = !Part001.rigidbody2D.isKinematic;
			Part002.rigidbody2D.isKinematic     = !Part002.rigidbody2D.isKinematic;
			Part003.rigidbody2D.isKinematic     = !Part003.rigidbody2D.isKinematic;
			Part004.rigidbody2D.isKinematic 	= !Part004.rigidbody2D.isKinematic;
			Part005.rigidbody2D.isKinematic 	= !Part005.rigidbody2D.isKinematic;
			Part006.rigidbody2D.isKinematic 	= !Part006.rigidbody2D.isKinematic;
			Part007.rigidbody2D.isKinematic 	= !Part007.rigidbody2D.isKinematic;
			Part008.rigidbody2D.isKinematic 	= !Part008.rigidbody2D.isKinematic;
			Part009.rigidbody2D.isKinematic 	= !Part009.rigidbody2D.isKinematic;
			Part010.rigidbody2D.isKinematic 	= !Part010.rigidbody2D.isKinematic;

		} else {

			Part000.rigidbody.isKinematic = !Part000.rigidbody.isKinematic;
			Part001.rigidbody.isKinematic     = !Part001.rigidbody.isKinematic;
			Part002.rigidbody.isKinematic     = !Part002.rigidbody.isKinematic;
			Part003.rigidbody.isKinematic     = !Part003.rigidbody.isKinematic;
			Part004.rigidbody.isKinematic 	= !Part004.rigidbody.isKinematic;
			Part005.rigidbody.isKinematic 	= !Part005.rigidbody.isKinematic;
			Part006.rigidbody.isKinematic 	= !Part006.rigidbody.isKinematic;
			Part007.rigidbody.isKinematic 	= !Part007.rigidbody.isKinematic;
			Part008.rigidbody.isKinematic 	= !Part008.rigidbody.isKinematic;
			Part009.rigidbody.isKinematic 	= !Part009.rigidbody.isKinematic;
			Part010.rigidbody.isKinematic 	= !Part010.rigidbody.isKinematic;


		}



	}
	
	public void ToggleHang() {
		
		// decide what hanging the bird does
		//if (Part010.rigidbody2D.isKinematic) { ToggleIsKinematicAll(); }
		if (toggleHangState == false) {
			cachePosController1 			= ControllerStart.transform.position;
			cachePosController2		= ControllerEnd.transform.position;	

			
			
			SetDefaultScale();
			//ToggleIsKinematicAll();

			// TODO externalise the hang state positions into the control
			ControllerStart.transform.position 		=new Vector3 (4.168327f, 5.389643f+3.0f, 0.04231771f);
			ControllerEnd.transform.position 		=new Vector3 (-1.007799f, 4.153904f+3.0f, 0.04231833f);

			//ToggleIsKinematicAll();
			toggleHangState = true;
			
			
		} else {
			
			SetDefaultScale();
			//ToggleIsKinematicAll();
			ControllerStart.transform.position 	 	= cachePosController1;	
			ControllerEnd.transform.position 	 	= cachePosController2;
			toggleHangState = !toggleHangState;
			//ToggleIsKinematicAll();
		}

		
	}
	
	public void SetScale (float sliderValue) {
			
		
		// Thought processes
		// Anchor positions of joints don't scale with the scale of the object
		// solution 1 add game object nulls at the transform points, parent then sensibly and re-set anchor points during / after a scale
		// Toggle IsKinectic functions switch all rigidbodies.kinetic to false
		
		// problems seems to work for one joint but explode others
		// should I scale the root game object (parent of all the rigid bodies);
			
		//MakeKinematicTrueAll();
			
		Vector3 scaleVec = new Vector3 (sliderValue, sliderValue, 1);
			//Root.transform.localScale = scaleVec;

			
			//transform.localScale = new Vector3 (sliderValue, sliderValue, 1);
		//Root.transform.localScale = scaleVec;
		Part000.transform.localScale = scaleVec;
		Part001.transform.localScale = scaleVec;
		Part002.transform.localScale = scaleVec;
		Part003.transform.localScale = scaleVec;
		Part004.transform.localScale = scaleVec;
		Part005.transform.localScale = scaleVec;
		Part006.transform.localScale = scaleVec;
		Part007.transform.localScale = scaleVec;
		Part008.transform.localScale = scaleVec;
		Part009.transform.localScale = scaleVec;
		Part010.transform.localScale = scaleVec;
		Part000.transform.localScale = scaleVec;

		
		// return to the control of physics
		//MakeKinematicFalseAll();
	}
		
	public void SetDefaultScale () {
			
			MakeKinematicTrueAll();
		//MyRoot.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
		Part000.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part001.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part002.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part003.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part004.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part005.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part006.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part007.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part008.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part009.transform.localScale = new Vector3 (1f, 1f, 1f);
		Part010.transform.localScale = new Vector3 (1f, 1f, 1f);
			

		MakeKinematicFalseAll();
		
	}

	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Part010.GetComponent(SpriteRenderer);
		Color myColor = new Color(0f,0f,0f,1.0f);
		SpriteRenderer headRenderer = Part010.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer Part009Renderer = Part009.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part009Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part008Renderer = Part008.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part008Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part007Renderer = Part007.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part007Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part006Renderer = Part006.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part006Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part005Renderer = Part005.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part005Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part004Renderer = Part004.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part004Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part003Renderer = Part003.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part003Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part002Renderer = Part002.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part002Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part001Renderer = Part001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part001Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part000Renderer = Part000.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part000Renderer.color = myColor; // Set to opaque black
		//Part010.SpriteRenderer.color = new Color(1,1,1,1);


	}

	
	public void normalColorMode() {
		
		//SpriteRenderer myrenderer = Part010.GetComponent(SpriteRenderer);
		Color myColor = new Color(1f,1f,1f,.9f);
		SpriteRenderer headRenderer = Part010.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer Part009Renderer = Part009.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part009Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part008Renderer = Part008.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part008Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part007Renderer = Part007.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part007Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part006Renderer = Part006.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part006Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part005Renderer = Part005.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part005Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part004Renderer = Part004.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part004Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part003Renderer = Part003.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part003Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part002Renderer = Part002.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part002Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part001Renderer = Part001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part001Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Part000Renderer = Part000.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Part000Renderer.color = myColor; // Set to opaque black
		//Part010.SpriteRenderer.color = new Color(1,1,1,1);
		
	}

	public void EndScale (float sliderValue) {
			
			MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		

		
		
	}





	public void Rotate(bool whichWay) {
		
		// 
		if (is2D) {

			Debug.Log("Dragon Helper: 2D rotate (flip)");
			if (whichWay == true) {
				
				Debug.Log("We're in Dragon Unity  Helper - Rotate: "+whichWay);
				
				SetColliders(false);
				//MakeKinematicTrueHead();
				MakeKinematicTrueAll();
				//MakeKinematicTrueHead();

				// lookup current scale
				Vector3 myScale = Part010.transform.localScale;

				if (Part000) { Part000.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part001) { Part001.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part002) { Part002.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part003) { Part003.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part004) { Part004.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part005) { Part005.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part006) { Part006.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part007) { Part007.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part008) { Part008.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part009) { Part009.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Part010){ Part010.transform.localScale =  new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }

				MakeKinematicFalseAll();

				//iTween.RotateTo(Part010, iTween.Hash("y", 180.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject, "onComplete", "resetPreRotationState"));
				//MyRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);
				
				
				
			} else {
				//ToggleIsKinematicAll();
				//MyRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
				Debug.Log("trying to rotate back the other way");
				MakeKinematicTrueAll();
				//MakeKinematicTrueHead();
				iTween.RotateTo(Part010, iTween.Hash("y", 0.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));
				//whichWay = true;
				//ToggleIsKinematicAll();
				
			}


		} else {




		if (whichWay == true) {
			
			Debug.Log("We're in Dragon Unity  Helper - Rotate "+whichWay);

			SetColliders(false);
			//MakeKinematicTrueHead();
			MakeKinematicTrueAll();
			//MakeKinematicTrueHead();
			iTween.RotateTo(Part010, iTween.Hash("y", 180.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject, "onComplete", "resetPreRotationState"));
			//MyRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);


			
		} else {
			//ToggleIsKinematicAll();
			//MyRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
			Debug.Log("trying to rotate back the other way");
			MakeKinematicTrueAll();
			//MakeKinematicTrueHead();
			iTween.RotateTo(Part010, iTween.Hash("y", 0.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));
			//whichWay = true;
			//ToggleIsKinematicAll();
			
		}

		} // end of is2D


		
	}

	public void resetPreRotationState() {

	
		//MakeKinematicFalseAll();
		SetColliders(true);
		//MakeKinematicFalseHead();
		MakeKinematicFalseAll();
		Debug.Log("Tween callback called PuppetHelperDragon resetPreRotationState()");
		
	}



	Camera FindCamera ()
	{
		if (camera)
			return camera;
		else
			return Camera.main;
	}


	public void MoveObjectDragonTail(bool Pad2PuppetController2Touch1Flag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerEnd.rigidbody2D.isKinematic = true;
		ControllerEnd.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerEnd.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectDragonHead(bool Pad2PuppetController2Touch1Flag,  float xx3, float yy3, float zz3) {


		ControllerStart.transform.position = new Vector3(xx3, yy3, zz3);
	

	}
	


	void DisableColliders() {
		
		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (Part000) { Part000.collider2D.enabled = false; Debug.Log("In disableColliders(). Setting Part000 to off"); }
			if (Part001) { Part001.collider2D.enabled = false; }
			if (Part002) { Part002.collider2D.enabled = false; }
			if (Part003) { Part003.collider2D.enabled = false; }
			if (Part004) { Part004.collider2D.enabled = false; }
			if (Part005) { Part005.collider2D.enabled = false; }
			if (Part006) { Part006.collider2D.enabled = false; }
			if (Part007) { Part007.collider2D.enabled = false; }
			if (Part008) { Part008.collider2D.enabled = false; }
			if (Part009) { Part009.collider2D.enabled = false; }
			if (Part010){ Part010.collider2D.enabled = false; }
			
		} else {
			
			Debug.Log("disableColliders() - I'm a 3D object with 2D rigidbodies");
			if (Part000) { Part000.collider.enabled = false; Debug.Log("In disableColliders(). Setting Part000 to off"); }
			if (Part001) { Part001.collider.enabled = false; }
			if (Part002) { Part002.collider.enabled = false; }
			if (Part003) { Part003.collider.enabled = false; }
			if (Part004) { Part004.collider.enabled = false; }
			if (Part005) { Part005.collider.enabled = false; }
			if (Part006) { Part006.collider.enabled = false; }
			if (Part007) { Part007.collider.enabled = false; }
			if (Part008) { Part008.collider.enabled = false; }
			if (Part009) { Part009.collider.enabled = false; }
			if (Part010){ Part010.collider.enabled = false; }
			
		}
	}


	void SetColliders(bool toggleState) {
		
		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (Part000) { Part000.collider2D.enabled = toggleState; Debug.Log("In toggleeColliders(). Setting Part000 to "+toggleState); }
			if (Part001) { Part001.collider2D.enabled = toggleState; }
			if (Part002) { Part002.collider2D.enabled = toggleState; }
			if (Part003) { Part003.collider2D.enabled = toggleState; }
			if (Part004) { Part004.collider2D.enabled = toggleState; }
			if (Part005) { Part005.collider2D.enabled = toggleState; }
			if (Part006) { Part006.collider2D.enabled = toggleState; }
			if (Part007) { Part007.collider2D.enabled = toggleState; }
			if (Part008) { Part008.collider2D.enabled = toggleState; }
			if (Part009) { Part009.collider2D.enabled = toggleState; }
			if (Part010){ Part010.collider2D.enabled = toggleState; }
			
		} else {
			
			Debug.Log("disableColliders() - I'm a 3D object with 2D rigidbodies");
			if (Part000 && Part000.collider ) { Part000.collider.enabled = toggleState; Debug.Log("In toggleColliders(). Setting Part000 to "+ toggleState); }
			if (Part001 && Part000.collider) { Part001.collider.enabled = toggleState; }
			if (Part002 && Part002.collider) { Part002.collider.enabled = toggleState; }
			if (Part003 && Part003.collider) { Part003.collider.enabled = toggleState; }
			if (Part004 && Part004.collider) { Part004.collider.enabled = toggleState; }
			if (Part005 && Part005.collider) { Part005.collider.enabled = toggleState; }
			if (Part006 && Part006.collider) { Part006.collider.enabled = toggleState; }
			if (Part007 && Part007.collider) { Part007.collider.enabled = toggleState; }
			if (Part008 && Part008.collider) { Part008.collider.enabled = toggleState; }
			if (Part009 && Part009.collider) { Part009.collider.enabled = toggleState; }
			if (Part010 && Part010.collider){ Part010.collider.enabled = toggleState; }
			
		}
	}

	void FixedUpdate(){


		// this is checking the public var for testing rotation on the UI
		if (Rotated) {
			Debug.Log("Dragon is rotated");
			Rotate(true);
			facingLeft = true;
			Rotated = false;


} else {

			if (facingLeft == true && !Rotated) {

				Rotate(false);
				facingLeft = false;
			}


		}

}
}
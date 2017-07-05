using UnityEngine;
using System.Collections;

public class PuppetHelperDragon_001 : MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	public GameObject DragonRoot;
	public GameObject Dragon000Tail;
	public GameObject Dragon001;
	public GameObject Dragon002;
	public GameObject Dragon003;
	public GameObject Dragon004;
	public GameObject Dragon005;
	public GameObject Dragon006;
	public GameObject Dragon007;
	public GameObject Dragon008;
	public GameObject Dragon009;
	public GameObject Dragon010Head;
	public GameObject ControllerHead;
	public GameObject ControllerTail;

	public bool is2D;

	public bool Rotated;

	// Use this for initialization
	private PuppetHelperDragon_001 dragonHelper;
	Vector3 dragonRotationNow;
	Vector3 dragonRotationTarget;
	bool facingLeft;
	bool rotating;
	public bool toggleHangState = false;


	// attempting dragable method on movement
	private SpringJoint2D springJoint;
	public float distance = 0.2f;
	public float damper = 0.5f; // damping ration in SpringJoint2D (0.0.- 1.0)
	public float frequency = 8.0f;
	public float drag = 10.0f; // this doesn't exist on 2D Spring...
	public float angularDrag = 5.0f;
	//var distance = 0.2;
	public bool attachToCenterOfMass = false;
	private bool headMoving = false;

	// state
	Vector3 cachePosController1; 			
	Vector3 cachePosController2;	


	void Awake () {
		Debug.Log ("An instance of DragonHelper has been create: AWAKE");
		// I guess the instance isn't around at 'awake' time
		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");
		
		//go.GetComponent<UnityOSCListener>().HelloWorld("HelloFromTheNewBird");
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject5(Dragon010Head);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject6(Dragon000Tail);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject5c(ControllerHead);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject6c(ControllerTail);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject7(DragonRoot);
		
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject5(Dragon010Head);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject6(Dragon000Tail);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject5c(ControllerHead);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject6c(ControllerTail);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject7(DragonRoot);

		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_Dragon(DragonRoot);


		if (is2D) {
			Dragon001.collider2D.transform.parent = Dragon001.transform; // Bug in Unity3D
			Dragon002.collider2D.transform.parent = Dragon002.transform; // Bug in Unity3D
			Dragon003.collider2D.transform.parent = Dragon003.transform; // Bug in Unity3D
			Dragon004.collider2D.transform.parent = Dragon004.transform; // Bug in Unity3D
			Dragon005.collider2D.transform.parent = Dragon005.transform; // Bug in Unity3D
			Dragon006.collider2D.transform.parent = Dragon006.transform; // Bug in Unity3D
			Dragon007.collider2D.transform.parent = Dragon007.transform; // Bug in Unity3D
			Dragon008.collider2D.transform.parent = Dragon008.transform; // Bug in Unity3D
			Dragon009.collider2D.transform.parent = Dragon009.transform; // Bug in Unity3D
			Dragon000Tail.collider2D.transform.parent = Dragon000Tail.transform; // Bug in Unity3D
			Dragon010Head.collider2D.transform.parent = Dragon010Head.transform; // Bug in Unity3D
		} else {
			if (Dragon001.collider) { Dragon001.collider.transform.parent = Dragon001.transform; } // Bug in Unity3D 
			if (Dragon002.collider) { Dragon002.collider.transform.parent = Dragon002.transform; } // Bug in Unity3D
			if (Dragon003.collider) { Dragon003.collider.transform.parent = Dragon003.transform; } // Bug in Unity3D
			if (Dragon004.collider) { Dragon004.collider.transform.parent = Dragon004.transform; } // Bug in Unity3D
			if (Dragon005.collider) { Dragon005.collider.transform.parent = Dragon005.transform; } // Bug in Unity3D
			if (Dragon006.collider) { Dragon006.collider.transform.parent = Dragon006.transform; } // Bug in Unity3D
			if (Dragon007.collider) { Dragon007.collider.transform.parent = Dragon007.transform; } // Bug in Unity3D
			if (Dragon008.collider) { Dragon008.collider.transform.parent = Dragon008.transform; } // Bug in Unity3D
			if (Dragon009.collider) { Dragon009.collider.transform.parent = Dragon009.transform; } // Bug in Unity3D
			if (Dragon000Tail.collider) { Dragon000Tail.collider.transform.parent = Dragon000Tail.transform; } // Bug in Unity3D
			if (Dragon010Head.collider) { Dragon010Head.collider.transform.parent = Dragon010Head.transform; } // Bug in Unity3D

		}
//Root = this.Root;
//IIMBird001Head = this.IIMBird001Head;
//IIMBird001Body = this.IIMBird001Body;
//IIMBird001Neck1 = this.IIMBird001Neck1;
//IIMBird001Neck2 = this.IIMBird001Neck2;
//IIMBird001Neck3 = this.IIMBird001Neck3;
//IIMBird001LLegUpper = this.IIMBird001LLegUpper;
//IIMBird001LLegLower = this.IIMBird001LLegLower;
//IIMBird001RLegUpper = this.IIMBird001RLegUpper;
//IIMBird001RLegLower = this.IIMBird001RLegLower;
//IIMBird001BodyNeckJointAnchor = this.IIMBird001BodyNeckJointAnchor;

	}
	
	void Start () {
		Debug.Log ("An instance of DragonHelper has been created: START");
		//Root = this.Root;
		//IIMBird001Head = this.IIMBird001Head;
		//IIMBird001Body = this.IIMBird001Body;
		
		dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));

		// this is for the physics rotate to target script 
		//---------------------------------
		//targetAngle = transform.eulerAngles.y; // get the current angle just for start
		//curAngle = targetAngle;
		//--------------------------------

		//var bodyToNeck3HingeJoint = IIMBird001Body.GetComponent(typeof(HingeJoint)) as HingeJoint;
		//Destroy(bodyToNeck3HingeJoint);
		
		//HingeJoint joint = IIMBird001Body.AddComponent<HingeJoint>();
		
		//joint.anchor = new Vector3(0.91f,0.48f,0f);
		//joint.connectedBody = IIMBird001Neck3.rigidbody;
		//JointLimits mylimits = joint.limits;
		//mylimits.min = -30;
		//mylimits.max =  30;
		//joint.limits = mylimits;
		
		// Replace Existing Hinge Joint (body to neck 3)
		
		//IIMBird001Body.AddComponent("HingeJoint");

		//Joint foo = IIMBird001Body.GetComponent<HingeJoint>();
		//IIMBird001Body.hingeJoint.anchor = new Vector3(0.91f,0.48f,0f);
		//var bodyToNeck3HingeJoint2 = IIMBird001Body.GetComponent(typeof(HingeJoint)) as HingeJoint;
		
	 	//print(foo.anchor.x);
		
		//joint.connectedBody = IIMBird001Neck3.rigidbody;
		
		//bodyToNeck3HingeJoint2.anchor = new Vector3(0.91f,0.48f,0f);
		//bodyToNeck3HingeJoint2.axis= new Vector3(0f,0f,1f);
		//JointLimits mylimits = bodyToNeck3HingeJoint.limits;
		//mylimits.min =-30f;
		//mylimits.max = 30f;
		//bodyToNeck3HingeJoint.useLimits = true;
		
		
	}




	// IsKinetic Toggles and Controls
	public void MakeKinematicTrueHeadBody() {

		if (is2D) {

		Dragon010Head.rigidbody2D.isKinematic     = true;
		Dragon000Tail.rigidbody2D.isKinematic 	= true;


		} else {

			Dragon010Head.rigidbody.isKinematic     = true;
			Dragon000Tail.rigidbody.isKinematic 	= true;
		}
	}	
	
	public void MakeKinematicFalseHeadBody() {

		if (is2D) {
		Dragon010Head.rigidbody2D.isKinematic     = false;
		Dragon000Tail.rigidbody2D.isKinematic 	= false;
		} else {
			Dragon010Head.rigidbody.isKinematic     = false;
			Dragon000Tail.rigidbody.isKinematic 	= false;
			
		}

	}
	
	public void MakeKinematicTrueBody() {

		if (is2D) {

			Dragon000Tail.rigidbody2D.isKinematic = true;
		
		} else {

			Dragon000Tail.rigidbody2D.isKinematic = true;

		}


	}	
	
	public void MakeKinematicFalseBody() {

			if (is2D) {

		Dragon000Tail.rigidbody2D.isKinematic = false;

		} else {

			Dragon000Tail.rigidbody.isKinematic = false;

		}


	}
	
	public void MakeKinematicTrueHead() {

				if (is2D) {
		Dragon010Head.rigidbody2D.isKinematic      = true;

		} else {

			Dragon010Head.rigidbody.isKinematic      = true;

		}


	}
	
	public void MakeKinematicFalseHead() {

		if (is2D) {
		
			Dragon010Head.rigidbody2D.isKinematic      = false;	
		
		} else {
			
			Dragon010Head.rigidbody.isKinematic      = false;
			
		}


	}	
	
	public void MakeKinematicFalseAll() {

	if (is2D == true) {

			Dragon000Tail.rigidbody2D.isKinematic = false;
			Dragon001.rigidbody2D.isKinematic     = false;
			Dragon002.rigidbody2D.isKinematic     = false;
			Dragon003.rigidbody2D.isKinematic     = false;
			Dragon004.rigidbody2D.isKinematic 	= false;
			Dragon005.rigidbody2D.isKinematic 	= false;
			Dragon006.rigidbody2D.isKinematic 	= false;
			Dragon007.rigidbody2D.isKinematic 	= false;
			Dragon008.rigidbody2D.isKinematic 	= false;
			Dragon009.rigidbody2D.isKinematic 	= false;
			Dragon010Head.rigidbody2D.isKinematic = false;

	} else {

			Dragon000Tail.rigidbody.isKinematic = false;
			Dragon001.rigidbody.isKinematic     = false;
			Dragon002.rigidbody.isKinematic     = false;
			Dragon003.rigidbody.isKinematic     = false;
			Dragon004.rigidbody.isKinematic 	= false;
			Dragon005.rigidbody.isKinematic 	= false;
			Dragon006.rigidbody.isKinematic 	= false;
			Dragon007.rigidbody.isKinematic 	= false;
			Dragon008.rigidbody.isKinematic 	= false;
			Dragon009.rigidbody.isKinematic 	= false;
			Dragon010Head.rigidbody.isKinematic = false;

		}

	}

		
		
	public void MakeKinematicTrueAll() {

	if (is2D) {
			Dragon000Tail.rigidbody2D.isKinematic = true;
			Dragon001.rigidbody2D.isKinematic     = true;
			Dragon002.rigidbody2D.isKinematic     = true;
			Dragon003.rigidbody2D.isKinematic     = true;
			Dragon004.rigidbody2D.isKinematic 	= true;
			Dragon005.rigidbody2D.isKinematic 	= true;
			Dragon006.rigidbody2D.isKinematic 	= true;
			Dragon007.rigidbody2D.isKinematic 	= true;
			Dragon008.rigidbody2D.isKinematic 	= true;
			Dragon009.rigidbody2D.isKinematic 	= true;
			Dragon010Head.rigidbody2D.isKinematic = true;
		} else {
			Dragon000Tail.rigidbody.isKinematic = true;
			Dragon001.rigidbody.isKinematic     = true;
			Dragon002.rigidbody.isKinematic     = true;
			Dragon003.rigidbody.isKinematic     = true;
			Dragon004.rigidbody.isKinematic 	= true;
			Dragon005.rigidbody.isKinematic 	= true;
			Dragon006.rigidbody.isKinematic 	= true;
			Dragon007.rigidbody.isKinematic 	= true;
			Dragon008.rigidbody.isKinematic 	= true;
			Dragon009.rigidbody.isKinematic 	= true;
			Dragon010Head.rigidbody.isKinematic = true;



		}


		
	}
	
	public void ToggleIsKinematicAll() {
		if (is2D) {
					
		//DragonRoot.rigidbody2D.isKinematic    = !DragonRoot.rigidbody2D.isKinematic;
			Dragon000Tail.rigidbody2D.isKinematic = !Dragon000Tail.rigidbody2D.isKinematic;
			Dragon001.rigidbody2D.isKinematic     = !Dragon001.rigidbody2D.isKinematic;
			Dragon002.rigidbody2D.isKinematic     = !Dragon002.rigidbody2D.isKinematic;
			Dragon003.rigidbody2D.isKinematic     = !Dragon003.rigidbody2D.isKinematic;
			Dragon004.rigidbody2D.isKinematic 	= !Dragon004.rigidbody2D.isKinematic;
			Dragon005.rigidbody2D.isKinematic 	= !Dragon005.rigidbody2D.isKinematic;
			Dragon006.rigidbody2D.isKinematic 	= !Dragon006.rigidbody2D.isKinematic;
			Dragon007.rigidbody2D.isKinematic 	= !Dragon007.rigidbody2D.isKinematic;
			Dragon008.rigidbody2D.isKinematic 	= !Dragon008.rigidbody2D.isKinematic;
			Dragon009.rigidbody2D.isKinematic 	= !Dragon009.rigidbody2D.isKinematic;
			Dragon010Head.rigidbody2D.isKinematic 	= !Dragon010Head.rigidbody2D.isKinematic;

		} else {

			Dragon000Tail.rigidbody.isKinematic = !Dragon000Tail.rigidbody.isKinematic;
			Dragon001.rigidbody.isKinematic     = !Dragon001.rigidbody.isKinematic;
			Dragon002.rigidbody.isKinematic     = !Dragon002.rigidbody.isKinematic;
			Dragon003.rigidbody.isKinematic     = !Dragon003.rigidbody.isKinematic;
			Dragon004.rigidbody.isKinematic 	= !Dragon004.rigidbody.isKinematic;
			Dragon005.rigidbody.isKinematic 	= !Dragon005.rigidbody.isKinematic;
			Dragon006.rigidbody.isKinematic 	= !Dragon006.rigidbody.isKinematic;
			Dragon007.rigidbody.isKinematic 	= !Dragon007.rigidbody.isKinematic;
			Dragon008.rigidbody.isKinematic 	= !Dragon008.rigidbody.isKinematic;
			Dragon009.rigidbody.isKinematic 	= !Dragon009.rigidbody.isKinematic;
			Dragon010Head.rigidbody.isKinematic 	= !Dragon010Head.rigidbody.isKinematic;


		}



	}
	
	public void ToggleHang() {
		
		// decide what hanging the bird does
		//if (Dragon010Head.rigidbody2D.isKinematic) { ToggleIsKinematicAll(); }
		if (toggleHangState == false) {
			cachePosController1 			= ControllerHead.transform.position;
			cachePosController2		= ControllerTail.transform.position;	

			
			
			SetDefaultScale();
			//ToggleIsKinematicAll();
			ControllerHead.transform.position 		=new Vector3 (4.168327f, 5.389643f+3.0f, 0.04231771f);
			ControllerTail.transform.position 		=new Vector3 (-1.007799f, 4.153904f+3.0f, 0.04231833f);

			//ToggleIsKinematicAll();
			toggleHangState = true;
			
			
		} else {
			
			SetDefaultScale();
			//ToggleIsKinematicAll();
			ControllerHead.transform.position 	 	= cachePosController1;	
			ControllerTail.transform.position 	 	= cachePosController2;
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
			
			
        	//Configjoint.anchor = new Vector3(0.91f,0.48f,0f);  known anchor point when object is scale = 1		
			//Configjoint.anchor = Vector3.Scale(new Vector3 (scaleVec.x,scaleVec.y,scaleVec.z), new Vector3(0.91f,0.48f,0f));
			//bodyToNeck3HingeJoint.
			
		
		//joint.anchor = Vector3.Scale(new Vector3 (scaleVec.x,scaleVec.y,scaleVec.z), new Vector3(0.91f,0.48f,0f));
		
			//Configjoint = IIMBird001Body.GetComponent(ConfigurableJoint);
			//Configjoint.  (1.0,1.0,1.0);
			
			//transform.localScale = new Vector3 (sliderValue, sliderValue, 1);
		//Root.transform.localScale = scaleVec;
		Dragon000Tail.transform.localScale = scaleVec;
		Dragon001.transform.localScale = scaleVec;
		Dragon002.transform.localScale = scaleVec;
		Dragon003.transform.localScale = scaleVec;
		Dragon004.transform.localScale = scaleVec;
		Dragon005.transform.localScale = scaleVec;
		Dragon006.transform.localScale = scaleVec;
		Dragon007.transform.localScale = scaleVec;
		Dragon008.transform.localScale = scaleVec;
		Dragon009.transform.localScale = scaleVec;
		Dragon010Head.transform.localScale = scaleVec;
		Dragon000Tail.transform.localScale = scaleVec;

		
		// return to the control of physics
		//MakeKinematicFalseAll();
	}
		
	public void SetDefaultScale () {
			
			MakeKinematicTrueAll();
		//DragonRoot.transform.localScale = new Vector3 (0.6f, 0.6f, 0.6f);
		Dragon000Tail.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon001.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon002.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon003.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon004.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon005.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon006.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon007.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon008.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon009.transform.localScale = new Vector3 (1f, 1f, 1f);
		Dragon010Head.transform.localScale = new Vector3 (1f, 1f, 1f);
			

		MakeKinematicFalseAll();
		
	}

	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(0f,0f,0f,1.0f);
		SpriteRenderer headRenderer = Dragon010Head.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon009Renderer = Dragon009.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon009Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon008Renderer = Dragon008.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon008Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon007Renderer = Dragon007.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon007Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon006Renderer = Dragon006.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon006Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon005Renderer = Dragon005.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon005Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon004Renderer = Dragon004.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon004Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon003Renderer = Dragon003.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon003Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon002Renderer = Dragon002.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon002Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon001Renderer = Dragon001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon001Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon000TailRenderer = Dragon000Tail.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon000TailRenderer.color = myColor; // Set to opaque black
		//Dragon010Head.SpriteRenderer.color = new Color(1,1,1,1);


	}

	
	public void normalColorMode() {
		
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(1f,1f,1f,.9f);
		SpriteRenderer headRenderer = Dragon010Head.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon009Renderer = Dragon009.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon009Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon008Renderer = Dragon008.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon008Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon007Renderer = Dragon007.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon007Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon006Renderer = Dragon006.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon006Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon005Renderer = Dragon005.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon005Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon004Renderer = Dragon004.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon004Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon003Renderer = Dragon003.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon003Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon002Renderer = Dragon002.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon002Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon001Renderer = Dragon001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon001Renderer.color = myColor; // Set to opaque black
		SpriteRenderer Dragon000TailRenderer = Dragon000Tail.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		Dragon000TailRenderer.color = myColor; // Set to opaque black
		//Dragon010Head.SpriteRenderer.color = new Color(1,1,1,1);
		
	}

	public void EndScale (float sliderValue) {
			
			MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		
		// decide what rotating the bird does
		
		
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
				Vector3 myScale = Dragon010Head.transform.localScale;

				if (Dragon000Tail) { Dragon000Tail.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon001) { Dragon001.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon002) { Dragon002.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon003) { Dragon003.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon004) { Dragon004.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon005) { Dragon005.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon006) { Dragon006.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon007) { Dragon007.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon008) { Dragon008.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon009) { Dragon009.transform.localScale = new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }
				if (Dragon010Head){ Dragon010Head.transform.localScale =  new Vector3(myScale.x * -1.0f,myScale.y * 1.0f,myScale.z * 1.0f); }

				MakeKinematicFalseAll();

				//iTween.RotateTo(Dragon010Head, iTween.Hash("y", 180.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject, "onComplete", "resetPreRotationState"));
				//DragonRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);
				
				
				
			} else {
				//ToggleIsKinematicAll();
				//DragonRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
				Debug.Log("trying to rotate back the other way");
				MakeKinematicTrueAll();
				//MakeKinematicTrueHead();
				iTween.RotateTo(Dragon010Head, iTween.Hash("y", 0.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));
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
			iTween.RotateTo(Dragon010Head, iTween.Hash("y", 180.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject, "onComplete", "resetPreRotationState"));
			//DragonRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);


			
		} else {
			//ToggleIsKinematicAll();
			//DragonRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
			Debug.Log("trying to rotate back the other way");
			MakeKinematicTrueAll();
			//MakeKinematicTrueHead();
			iTween.RotateTo(Dragon010Head, iTween.Hash("y", 0.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));
			//whichWay = true;
			//ToggleIsKinematicAll();
			
		}

		} // end of is2D

		/*
		if (whichWay == true) {
			Debug.Log("We're in Dragon Helper - Rotate Clockwise "+whichWay);
			dragonRotationNow = DragonRoot.transform.localRotation.eulerAngles;
			Debug.Log(dragonRotationNow.y);
			//dragonRotation.x = dragonRotation.y = 0.0f;
			dragonRotationNow.y = 180.0f;

			// variables is for the physics rotate to target script 
			//---------------------------------
			//targetAngle = 180.0f;

			rotating = true;

			
			//DragonRoot.transform.eulerAngles = dragonRotationNow;
		} else {
			Debug.Log("We're in Dragon Helper - Rotate Anti-Clockwise True");
			//object7.transform.eulerAngles.z = 0.0f;
			dragonRotationNow = DragonRoot.transform.localRotation.eulerAngles;
			dragonRotationTarget = DragonRoot.transform.localRotation.eulerAngles;
			Debug.Log(dragonRotationNow.y);
			//dragonRotation.x = dragonRotation.y = 0.0f;
			dragonRotationNow.y = 0.0f;
			//d
			// model for slerping - does this need to be in Update()
			// object7.transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, Time.time * speed);
			rotating = true;

			//targetAngle = -180.0f;

			//DragonRoot.transform.eulerAngles = dragonRotationNow;
		}
		*/
		
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
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerTail.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectDragonHead(bool Pad2PuppetController2Touch1Flag,  float xx3, float yy3, float zz3) {


		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerHead.rigidbody2D.isKinematic = true;
		ControllerHead.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerHead.rigidbody2D.isKinematic = false;


/*
		Vector3 myPosition = new Vector3 (xx3, yy3, zz3);
		Camera mainCamera = FindCamera ();
		int layerMask = 1 << 8;
		RaycastHit2D hit = Physics2D.Raycast (mainCamera.ScreenToWorldPoint (myPosition), Vector2.zero, Mathf.Infinity, layerMask);

		if (!springJoint) {
			GameObject go = new GameObject ("Rigidbody2D Dragger");
			Rigidbody2D body = go.AddComponent ("Rigidbody2D") as Rigidbody2D;
			springJoint = go.AddComponent ("SpringJoint2D") as SpringJoint2D;
			
			body.isKinematic = true;
		}
		
		springJoint.transform.position = new Vector2(xx3 , yy3);
		
		
		if (attachToCenterOfMass) {
			
			Debug.Log ("Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics - it will be added in a future release.");
			// Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics yet - it will be added in a future release.
			
			//Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position; in c# might be Vector2?
			
			//anchor = springJoint.transform.InverseTransformPoint(anchor);
			//springJoint.anchor = anchor;
		} else {
			
			//springJoint.anchor = Vector3.zero;
		}
		
		springJoint.distance = distance; // there is no distance in SpringJoint2D
		springJoint.dampingRatio = damper;// there is no damper in SpringJoint2D but there is a dampingRatio
		//springJoint.maxDistance = distance;  // there is no MaxDistance in the SpringJoint2D - but there is a 'distance' field 
		//	see http://docs.unity3d.com/Documentation/ScriptReference/SpringJoint2D.html
		//springJoint.maxDistance = distance;
		springJoint.connectedBody = hit.rigidbody;
		
		
		// maybe check if the 'fraction' is normalised. See http://docs.unity3d.com/Documentation/ScriptReference/RaycastHit2D-fraction.html
		//StartCoroutine ("DragObject", hit.fraction);
		StartCoroutine (DragObject(hit.fraction, myPosition));


		*/


	}
	/*

	IEnumerator DragObject (float distance, Vector3 myPosition)
	{	
		
		float oldDrag = springJoint.connectedBody.drag;
		float oldAngularDrag = springJoint.connectedBody.angularDrag;
		
		springJoint.connectedBody.drag = drag;
		springJoint.connectedBody.angularDrag = angularDrag;
		
		Camera mainCamera = FindCamera ();
		
		while (headMoving) {
			Ray ray = mainCamera.ScreenPointToRay (myPosition);
			springJoint.transform.position = ray.GetPoint (distance);
			yield return null;
		}
		
		
		
		if (springJoint.connectedBody) {	
			springJoint.connectedBody.drag = oldDrag;
			springJoint.connectedBody.angularDrag = oldAngularDrag;
			springJoint.connectedBody = null;
		}
		
	}
*/


	void DisableColliders() {
		
		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (Dragon000Tail) { Dragon000Tail.collider2D.enabled = false; Debug.Log("In disableColliders(). Setting Dragon000Tail to off"); }
			if (Dragon001) { Dragon001.collider2D.enabled = false; }
			if (Dragon002) { Dragon002.collider2D.enabled = false; }
			if (Dragon003) { Dragon003.collider2D.enabled = false; }
			if (Dragon004) { Dragon004.collider2D.enabled = false; }
			if (Dragon005) { Dragon005.collider2D.enabled = false; }
			if (Dragon006) { Dragon006.collider2D.enabled = false; }
			if (Dragon007) { Dragon007.collider2D.enabled = false; }
			if (Dragon008) { Dragon008.collider2D.enabled = false; }
			if (Dragon009) { Dragon009.collider2D.enabled = false; }
			if (Dragon010Head){ Dragon010Head.collider2D.enabled = false; }
			
		} else {
			
			Debug.Log("disableColliders() - I'm a 3D object with 2D rigidbodies");
			if (Dragon000Tail) { Dragon000Tail.collider.enabled = false; Debug.Log("In disableColliders(). Setting Dragon000Tail to off"); }
			if (Dragon001) { Dragon001.collider.enabled = false; }
			if (Dragon002) { Dragon002.collider.enabled = false; }
			if (Dragon003) { Dragon003.collider.enabled = false; }
			if (Dragon004) { Dragon004.collider.enabled = false; }
			if (Dragon005) { Dragon005.collider.enabled = false; }
			if (Dragon006) { Dragon006.collider.enabled = false; }
			if (Dragon007) { Dragon007.collider.enabled = false; }
			if (Dragon008) { Dragon008.collider.enabled = false; }
			if (Dragon009) { Dragon009.collider.enabled = false; }
			if (Dragon010Head){ Dragon010Head.collider.enabled = false; }
			
		}
	}


	void SetColliders(bool toggleState) {
		
		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (Dragon000Tail) { Dragon000Tail.collider2D.enabled = toggleState; Debug.Log("In toggleeColliders(). Setting Dragon000Tail to "+toggleState); }
			if (Dragon001) { Dragon001.collider2D.enabled = toggleState; }
			if (Dragon002) { Dragon002.collider2D.enabled = toggleState; }
			if (Dragon003) { Dragon003.collider2D.enabled = toggleState; }
			if (Dragon004) { Dragon004.collider2D.enabled = toggleState; }
			if (Dragon005) { Dragon005.collider2D.enabled = toggleState; }
			if (Dragon006) { Dragon006.collider2D.enabled = toggleState; }
			if (Dragon007) { Dragon007.collider2D.enabled = toggleState; }
			if (Dragon008) { Dragon008.collider2D.enabled = toggleState; }
			if (Dragon009) { Dragon009.collider2D.enabled = toggleState; }
			if (Dragon010Head){ Dragon010Head.collider2D.enabled = toggleState; }
			
		} else {
			
			Debug.Log("disableColliders() - I'm a 3D object with 2D rigidbodies");
			if (Dragon000Tail && Dragon000Tail.collider ) { Dragon000Tail.collider.enabled = toggleState; Debug.Log("In toggleColliders(). Setting Dragon000Tail to "+ toggleState); }
			if (Dragon001 && Dragon000Tail.collider) { Dragon001.collider.enabled = toggleState; }
			if (Dragon002 && Dragon002.collider) { Dragon002.collider.enabled = toggleState; }
			if (Dragon003 && Dragon003.collider) { Dragon003.collider.enabled = toggleState; }
			if (Dragon004 && Dragon004.collider) { Dragon004.collider.enabled = toggleState; }
			if (Dragon005 && Dragon005.collider) { Dragon005.collider.enabled = toggleState; }
			if (Dragon006 && Dragon006.collider) { Dragon006.collider.enabled = toggleState; }
			if (Dragon007 && Dragon007.collider) { Dragon007.collider.enabled = toggleState; }
			if (Dragon008 && Dragon008.collider) { Dragon008.collider.enabled = toggleState; }
			if (Dragon009 && Dragon009.collider) { Dragon009.collider.enabled = toggleState; }
			if (Dragon010Head && Dragon010Head.collider){ Dragon010Head.collider.enabled = toggleState; }
			
		}
	}

	void FixedUpdate(){



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
		//if (rotating) {


			//MakeKinematicTrueAll();
			//DragonRoot.transform.eulerAngles = dragonRotationNow;

			// variables is for the physics rotate to target script 
			//---------------------------------
			/*
			float error = targetAngle - curAngle; // generate the error signal
			float diff = (error - lastError)/ Time.deltaTime; // calculate differential error
			lastError = error;
			// calculate the acceleration:
			accel = error * pGain + diff * dGain;
			// limit it to the max acceleration
			accel = Mathf.Clamp(accel, -maxAccel, maxAccel);
			// apply accel to angular speed:
			angSpeed += accel * Time.deltaTime; 
			// limit max angular speed
			angSpeed = Mathf.Clamp(angSpeed, -maxASpeed, maxASpeed);
			curAngle += angSpeed * Time.deltaTime; // apply the rotation to the angle...
			// and make the object follow the angle (must be modulo 360)
			DragonRoot.transform.rotation = Quaternion.Euler(0, curAngle%360, 0);
			//Dragon010Head.rigidbody2D.transform.rotation = Quaternion.Euler(0, curAngle%360, 0); 
			// variables is for the physics rotate to target script 

			//---------------------------------

		} else {

			rotating = false;
			//MakeKinematicFalseAll();
	}
	}
*/
}
}
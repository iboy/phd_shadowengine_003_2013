using UnityEngine;
using System.Collections;

public class PuppetHelperIIM_Bird_Uni2D : MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	// IIM_Bird_Uni2D accepts 3 touches: [1] head, [2] body then [3] beak aim
	public GameObject BirdRoot;
	public GameObject BirdHead;
	public GameObject BirdBody;
	public GameObject BirdNeck1;
	public GameObject BirdNeck2;
	public GameObject BirdNeck3;
	public GameObject BirdLeg_L_Upper;
	public GameObject BirdLeg_L_Lower;
	public GameObject BirdLeg_R_Upper;
	public GameObject BirdLeg_R_Lower;
	public GameObject BirdBeakAnchor;
	public GameObject ControllerHead;
	public GameObject ControllerBody;
	public GameObject ControllerBeak;
	public GameObject ControllerLeftKnee;
	public GameObject ControllerRightKnee;

	public bool is2D;

	public bool Rotated;

	public Transform ControllerOneHangState;
	public Transform ControllerTwoHangState;
	public Transform ControllerThreeHangState;
	public Transform ControllerFourHangState;
	public Transform ControllerFiveHangState;

	// Use this for initialization
	private PuppetHelperIIM_Bird_Uni2D birdUni2DHelper;
	Vector3 birdRotationNow;
	Vector3 birdRotationTarget;
	bool facingLeft;
	bool rotating;
	public bool toggleHangState = false;

	// state
	Vector3 cachePosControllerBeak; 			
	Vector3 cachePosControllerBody;		
	Vector3 cachePosControllerHead;		
	Vector3 cachePosControllerRightKnee;	
	Vector3 cachePosControllerLeftKnee;	
	Vector3 cachePosRoot;

	// more state - trying to cache position of each element at start / awake
	Transform cachePosHead;
	Transform cachePosBody;
	Transform cachePosNeck1;
	Transform cachePosNeck2;
	Transform cachePosNeck3;
	Transform cachePosUpperLeftLeg;
	Transform cachePosLowerLeftLeg;
	Transform cachePosUpperRightLeg;
	Transform cachePosLowerRightLeg;


	// this is convoluted but it may help scaling the object 
	// these are all the anchor and connected anchor locations created as transforms
	public Transform pointPair1a;
	public Transform pointPair1b;
	public Transform pointPair2a;
	public Transform pointPair2b;
	public Transform pointPair3a;
	public Transform pointPair3b;
	public Transform pointPair4a;
	public Transform pointPair4b;
	public Transform pointPair5a;
	public Transform pointPair5b;
	public Transform pointPair6a;
	public Transform pointPair6b;
	public Transform pointPair7a;
	public Transform pointPair7b;



	// this is to simplify testing controls
	bool toggleController1 = true;
	bool toggleController2 = true;
	bool toggleController3 = true;
	bool toggleController4 = true;
	bool toggleController5 = true;
	int controllerKeyCount = 0;

	// the color changing 
	Color tintColor;



	void Awake () {
		Debug.Log ("An instance of BirdUni2DHelper has been create: AWAKE");
		// I guess the instance isn't around at 'awake' time
		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");
		tintColor = BirdBody.renderer.material.GetColor ("_TintColor");
		//tintColor = BirdBody.renderer.material.GetColor ("_Color");
		//monochromeColorMode();

		ControllerHead.SetActive(toggleController1);
		ControllerBody.SetActive(toggleController2);
		ControllerBeak.SetActive(toggleController3);
		ControllerLeftKnee.SetActive(toggleController4);
		ControllerRightKnee.SetActive(toggleController5);



		// this is maybe unnecessary - try to not manage complex references across classes. Encapsulate.
		/*
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


*/
		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_IIMBirdUni(BirdRoot);

		if (is2D) {
			BirdRoot.collider2D.transform.parent = BirdRoot.transform; // Bug in Unity3D
			BirdHead.collider2D.transform.parent = BirdHead.transform; // Bug in Unity3D
			BirdBody.collider2D.transform.parent = BirdBody.transform; // Bug in Unity3D
			BirdNeck1.collider2D.transform.parent = BirdNeck1.transform; // Bug in Unity3D
			BirdNeck2.collider2D.transform.parent = BirdNeck2.transform; // Bug in Unity3D
			BirdNeck3.collider2D.transform.parent = BirdNeck1.transform; // Bug in Unity3D
			BirdLeg_L_Upper.collider2D.transform.parent = BirdLeg_L_Upper.transform; // Bug in Unity3D
			BirdLeg_L_Lower.collider2D.transform.parent = BirdLeg_L_Lower.transform; // Bug in Unity3D
			BirdLeg_R_Upper.collider2D.transform.parent = BirdLeg_R_Upper.transform; // Bug in Unity3D
			BirdLeg_R_Lower.collider2D.transform.parent = BirdLeg_R_Lower.transform; // Bug in Unity3D
			BirdBeakAnchor.collider2D.transform.parent = BirdBeakAnchor.transform; // Bug in Unity3D
			ControllerHead.collider2D.transform.parent = ControllerHead.transform; // Bug in Unity3D
			ControllerBody.collider2D.transform.parent = ControllerBody.transform; // Bug in Unity3D
			ControllerLeftKnee.collider2D.transform.parent = ControllerLeftKnee.transform; // Bug in Unity3D
			ControllerRightKnee.collider2D.transform.parent = ControllerRightKnee.transform; // Bug in Unity3D
		} else {

			if (BirdRoot.collider) {BirdRoot.collider.transform.parent =BirdRoot.collider.transform; }
			if (BirdHead.collider) {BirdHead.collider.transform.parent =BirdHead.collider.transform; }
			if (BirdBody.collider) {BirdBody.collider.transform.parent =BirdBody.collider.transform; }
			if (BirdNeck1.collider) {BirdNeck1.collider.transform.parent=BirdNeck1.collider.transform;}
			if (BirdNeck2.collider) {BirdNeck2.collider.transform.parent=BirdNeck2.collider.transform;}
			if (BirdNeck3.collider) {BirdNeck3.collider.transform.parent=BirdNeck3.collider.transform;}
			if (BirdLeg_L_Upper.collider){ BirdLeg_L_Upper.collider.transform.parent =BirdLeg_L_Upper.collider.transform; }
			if (BirdLeg_L_Lower.collider){ BirdLeg_L_Lower.collider.transform.parent =BirdLeg_L_Lower.collider.transform; }
			if (BirdLeg_R_Upper.collider){ BirdLeg_R_Upper.collider.transform.parent =BirdLeg_R_Upper.collider.transform; }
			if (BirdLeg_R_Lower.collider){ BirdLeg_R_Lower.collider.transform.parent =BirdLeg_R_Lower.collider.transform; }
			if (BirdBeakAnchor.collider) { BirdBeakAnchor.collider.transform.parent =BirdBeakAnchor.collider.transform; }
			if (ControllerHead.collider) { ControllerHead.collider.transform.parent =ControllerHead.collider.transform; }
			if (ControllerBody.collider) { ControllerBody.collider.transform.parent =ControllerBody.collider.transform; }
			if (ControllerLeftKnee.collider) { ControllerLeftKnee.collider.transform.parent =ControllerLeftKnee.collider.transform;}
			if (ControllerRightKnee.collider){ ControllerRightKnee.collider.transform.parent =ControllerRightKnee.collider.transform;}
			
		}

		// try adding joints at awake time

		//BirdLeg_L_Upper.AddComponent<HingeJoint>();

		//BirdLeg_L_Upper.hingeJoint.axis = new Vector3(0,0,1);

		//BirdLeg_L_Upper.hingeJoint.connectedBody = BirdBody.rigidbody;

		//BirdLeg_L_Upper.hingeJoint.autoConfigureConnectedAnchor = false;
		//BirdLeg_L_Upper.hingeJoint.connectedAnchor = new Vector3(pointPair1a.localPosition.x,pointPair1a.localPosition.y);


	}
	public void SayHello() {
		Debug.Log("Hello from PuppetHelper - called by keyboard helper");

	}
	void Start () {
		Debug.Log ("An instance of birdUni2DHelper has been created: START");

		
		birdUni2DHelper = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));

		cachePosHead = BirdHead.transform;
		cachePosBody = BirdBody.transform;
		cachePosNeck1 = BirdNeck1.transform;
		cachePosNeck2 = BirdNeck2.transform;
		cachePosNeck3 = BirdNeck3.transform;
		cachePosUpperLeftLeg = BirdLeg_L_Upper.transform;
		cachePosLowerLeftLeg = BirdLeg_L_Lower.transform;
		cachePosUpperRightLeg = BirdLeg_R_Upper.transform;
		cachePosLowerRightLeg = BirdLeg_L_Lower.transform;


	

		DisableColliders();
	}
	
	// IsKinetic Toggles and Controls
	public void MakeKinematicTrueHeadBody() {
		BirdHead.rigidbody.isKinematic    = true;
		BirdBody.rigidbody.isKinematic    = true;
	}	
	
	public void MakeKinematicFalseHeadBody() {
		BirdHead.rigidbody.isKinematic    = false;
		BirdBody.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueBody() {
		BirdBody.rigidbody.isKinematic    = true;
	}	
	
	public void MakeKinematicFalseBody() {
		BirdBody.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueHead() {
		BirdHead.rigidbody.isKinematic    = true;
	}
	
	public void MakeKinematicFalseHead() {
		BirdHead.rigidbody.isKinematic    = false;
	}	
	
	public void MakeKinematicFalseAll() {
		BirdHead.rigidbody.isKinematic   	 	= false;
		BirdNeck1.rigidbody.isKinematic    	= false;
		BirdNeck2.rigidbody.isKinematic    	= false;
		BirdNeck3.rigidbody.isKinematic    	= false;
		BirdBody.rigidbody.isKinematic    	= false;
		BirdLeg_L_Upper.rigidbody.isKinematic = false;
		BirdLeg_L_Lower.rigidbody.isKinematic = false;
		BirdLeg_R_Upper.rigidbody.isKinematic = false;
		BirdLeg_R_Lower.rigidbody.isKinematic = false;
	}
		
		
	public void MakeKinematicTrueAll() {

		BirdHead.rigidbody.isKinematic   	 	= true;
		BirdNeck1.rigidbody.isKinematic    	= true;
		BirdNeck2.rigidbody.isKinematic    	= true;
		BirdNeck3.rigidbody.isKinematic    	= true;
		BirdBody.rigidbody.isKinematic    	= true;
		BirdLeg_L_Upper.rigidbody.isKinematic = true;
		BirdLeg_L_Lower.rigidbody.isKinematic = true;
		BirdLeg_R_Upper.rigidbody.isKinematic = true;
		BirdLeg_R_Lower.rigidbody.isKinematic = true;
	}
	
	public void ToggleIsKinematicAll() {
		

		BirdHead.rigidbody.isKinematic   	 	=!BirdHead.rigidbody.isKinematic;   	 	
		BirdNeck1.rigidbody.isKinematic    		=!BirdNeck1.rigidbody.isKinematic;    	
		BirdNeck2.rigidbody.isKinematic    		=!BirdNeck2.rigidbody.isKinematic;   	
		BirdNeck3.rigidbody.isKinematic    		=!BirdNeck3.rigidbody.isKinematic;    	
		BirdBody.rigidbody.isKinematic    		=!BirdBody.rigidbody.isKinematic;    	
		BirdLeg_L_Upper.rigidbody.isKinematic 	=!BirdLeg_L_Upper.rigidbody.isKinematic; 
		BirdLeg_L_Lower.rigidbody.isKinematic 	=!BirdLeg_L_Lower.rigidbody.isKinematic; 
		BirdLeg_R_Upper.rigidbody.isKinematic 	=!BirdLeg_R_Upper.rigidbody.isKinematic; 
		BirdLeg_R_Lower.rigidbody.isKinematic 	=!BirdLeg_R_Lower.rigidbody.isKinematic; 


	}
	
	public void ToggleHang() {
		
		// decide what hanging does
		// decide what resetting the bird does

		// Cache Controller Defaults
		// then hang
		// then, if toggle, reapply last cached settings
		if (toggleHangState == false) {

			cachePosRoot = BirdRoot.transform.position;
			cachePosControllerBeak 			= ControllerBeak.transform.position;
			cachePosControllerHead		= ControllerHead.transform.position;	
			cachePosControllerBody		= ControllerBody.transform.position;	
			cachePosControllerLeftKnee	= ControllerLeftKnee.transform.position; 
			cachePosControllerRightKnee	= ControllerRightKnee.transform.position; 
			
			
			//SetDefaultScale();
			//MakeKinematicTrueAll();
			//DisableColliders();

			iTween.MoveTo(BirdRoot, iTween.Hash("y", cachePosRoot.y+8.0f, "easeType", "easeInOutExpo", "loopType", "none", "time", 0.8f));

			
			//ControllerBeak.transform.localPosition 		= ControllerOneHangState.position;
			//ControllerHead.transform.localPosition 		= ControllerTwoHangState.position;
			//ControllerBody.transform.localPosition 		= ControllerThreeHangState.position;
			//ControllerLeftKnee.transform.localPosition 	= ControllerFourHangState.position;
			//ControllerRightKnee.transform.localPosition 	= ControllerFiveHangState.position;
			//MakeKinematicFalseAll();
			//SetColliders(true);
			toggleHangState = true;


		} else {

		//SetDefaultScale();
			//ToggleIsKinematicAll();

			iTween.MoveTo(BirdRoot, iTween.Hash("y", cachePosRoot.y, "easeType", "easeInOutExpo", "loopType", "none", "time", 1.0f));

			//iTween.MoveTo(BirdRoot, iTween.Hash("y", 6.0f, "easeType", "easeInOutExpo", "loopType", "none", "time", 0.8f));
			//ControllerBeak.transform.position 	 	= cachePosControllerBeak;	
			//ControllerHead.transform.position 	 	= cachePosControllerHead;
			//ControllerBody.transform.position 	 	= cachePosControllerBody;	
			//ControllerLeftKnee.transform.position  	= cachePosControllerLeftKnee;
			//ControllerRightKnee.transform.position  	= cachePosControllerRightKnee;

			//SetColliders(true);
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
			
		MakeKinematicTrueAll();


			
		Vector3 scaleVec = new Vector3 (sliderValue, sliderValue, 1); // -1 as the bird puppet is flipped


		// TODO Try this
		// cache all transforms
		// do scale
		// restore positions? 
		// restore broken hingejoints

		//cachePosBody = BirdBody.transform;
		//cachePosUpperLeftLeg = BirdLeg_L_Upper.transform;
		//cachePosLowerLeftLeg = BirdLeg_L_Lower.transform;
		//
		//Debug.Log ("Object: BirdBody: Position X:"+cachePosBody.localPosition.x+" and Rotation Z"+cachePosBody.transform.localEulerAngles.z);
		//
		//BirdBody.transform.localScale =scaleVec;




		/*
		((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;

		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
*/
		BirdRoot.transform.localScale = scaleVec;
		//BirdHead.transform.localScale =scaleVec;
		//BirdBody.transform.localScale =scaleVec;
		//BirdNeck1.transform.localScale =scaleVec;
		//BirdNeck2.transform.localScale =scaleVec;
		//BirdNeck3.transform.localScale =scaleVec;
		//BirdLeg_L_Upper.transform.localScale =scaleVec;
		//BirdLeg_L_Lower.transform.localScale =scaleVec;
		//BirdLeg_R_Upper.transform.localScale =scaleVec;
		//BirdLeg_R_Lower.transform.localScale =scaleVec;


		//BirdRoot.transform.localScale =scaleVec;
		//BirdHead.transform.localScale =scaleVec;
		//BirdBody.transform.localScale =scaleVec;
		//BirdNeck1.transform.localScale =scaleVec;
		//BirdNeck2.transform.localScale =scaleVec;
		//BirdNeck3.transform.localScale =scaleVec;
		//BirdLeg_L_Upper.transform.localScale =scaleVec;
		//BirdLeg_L_Lower.transform.localScale =scaleVec;
		//BirdLeg_R_Upper.transform.localScale =scaleVec;
		//BirdLeg_R_Lower.transform.localScale =scaleVec;
		/*
		Dragon000Tail.transform.localScale = scaleVec;z
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
*/
		
		// return to the control of physics
		MakeKinematicFalseAll();
	}
		
	public void SetDefaultScale () {
			
		// MakeKinematicTrueAll();
		Vector3 defaultScale =new Vector3 (-1, 1, 1);
		// e.g. DragonRoot.transform.localScale = new Vector3 (1, 1, 1);
		// MakeKinematicFalseAll();
		BirdRoot.transform.localScale =defaultScale;
		//((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		//((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		
	}

	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);

		Color myColor = new Color(0f,0f,0f,.5f);

		/*
		 * Process for Uni2D
		 */

		//SpriteRenderer headRenderer = BirdHead.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		//headRenderer.color = myColor; // Set to opaque black

		BirdHead.renderer.material.SetColor ("_TintColor", myColor);
		BirdBody.renderer.material.SetColor ("_TintColor", myColor);
		BirdNeck1.renderer.material.SetColor ("_TintColor", myColor);
		BirdNeck2.renderer.material.SetColor ("_TintColor", myColor);
		BirdNeck3.renderer.material.SetColor ("_TintColor", myColor);
		BirdLeg_L_Upper.renderer.material.SetColor ("_TintColor", myColor);
		BirdLeg_R_Upper.renderer.material.SetColor ("_TintColor", myColor);
		BirdLeg_L_Lower.renderer.material.SetColor ("_TintColor", myColor);
		BirdLeg_R_Lower.renderer.material.SetColor ("_TintColor", myColor);

		//((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		/*
		BirdHead..material.SetColor("_TintColor", myColor);
		BirdBody.renderer.material.SetColor("_TintColor", myColor);
		BirdNeck1.renderer.material.SetColor("_TintColor", myColor);
		BirdNeck2.renderer.material.SetColor("_TintColor", myColor);
		BirdNeck3.renderer.material.SetColor("_TintColor", myColor);
		BirdLeg_L_Upper.renderer.material.SetColor("_TintColor", myColor);
		BirdLeg_R_Upper.renderer.material.SetColor("_TintColor", myColor);
		BirdLeg_L_Lower.renderer.material.SetColor("_TintColor", myColor);
		BirdLeg_R_Lower.renderer.material.SetColor("_TintColor", myColor);
*/
		/*
		 * Process for Unity 2D Sprites
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
*/

	}

	
	public void normalColorMode() {
		
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		//Color myColor = new Color(.5f,.5f,.5f,.9f);
	

		BirdHead.renderer.material.SetColor ("_TintColor", tintColor);
		BirdBody.renderer.material.SetColor ("_TintColor", tintColor);
		BirdNeck1.renderer.material.SetColor ("_TintColor", tintColor);
		BirdNeck2.renderer.material.SetColor ("_TintColor", tintColor);
		BirdNeck3.renderer.material.SetColor ("_TintColor", tintColor);
		BirdLeg_L_Upper.renderer.material.SetColor ("_TintColor", tintColor);
		BirdLeg_R_Upper.renderer.material.SetColor ("_TintColor", tintColor);
		BirdLeg_L_Lower.renderer.material.SetColor ("_TintColor", tintColor);
		BirdLeg_R_Lower.renderer.material.SetColor ("_TintColor", tintColor);
		//((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;

		/*
		 * Process for Unity 2D Sprites
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

*/
		
	}

	public void EndScale (float sliderValue) {
			
			MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		
		// decide what resetting the bird does
		//SetDefaultScale();
		//MakeKinematicTrueAll();
		ControllerBeak.transform.position =cachePosControllerBeak;
		ControllerHead.transform.position =cachePosControllerHead;
		ControllerBody.transform.position =cachePosControllerBody;
		ControllerLeftKnee.transform.position =cachePosControllerLeftKnee;
		ControllerRightKnee.transform.position =cachePosControllerRightKnee;

		//BirdHead.transform.rotation       = cachePosHead.rotation;
		//BirdBody.transform.rotation        = cachePosBody.rotation;
		//BirdNeck1.transform.rotation       = cachePosNeck1.rotation;
		//BirdNeck2.transform.rotation       = cachePosNeck2.rotation;
		//BirdNeck3.transform.rotation       = cachePosNeck3.rotation;
		//BirdLeg_L_Upper.transform.rotation = cachePosUpperLeftLeg.rotation;
		//BirdLeg_L_Lower.transform.rotation = cachePosLowerLeftLeg.rotation;
		//BirdLeg_R_Upper.transform.rotation = cachePosUpperRightLeg.rotation;
		//BirdLeg_R_Lower.transform.rotation = cachePosLowerRightLeg.rotation;

		//BirdHead.transform.localPosition       = cachePosHead.localPosition;
		//BirdBody.transform.localPosition        = cachePosBody.localPosition;
		//BirdNeck1.transform.localPosition       = cachePosNeck1.localPosition;
		//BirdNeck2.transform.localPosition       = cachePosNeck2.localPosition;
		//BirdNeck3.transform.localPosition       = cachePosNeck3.localPosition;
		//BirdLeg_L_Upper.transform.localPosition = cachePosUpperLeftLeg.localPosition;
		//BirdLeg_L_Lower.transform.localPosition = cachePosLowerLeftLeg.localPosition;
		//BirdLeg_R_Upper.transform.localPosition = cachePosUpperRightLeg.localPosition;
		//BirdLeg_R_Lower.transform.localPosition = cachePosLowerRightLeg.localPosition;
		
		

		//MakeKinematicFalseAll();
	}
	
	public void Rotate(bool whichWay) {
		
		// note this nearly works! Except the parent, root object does not move along with the physics movement
		// so when the itween rotates - it rotates around the old origin - offset by the distance the bird had moved...


		if (whichWay == true) {

			//Debug.Log("We're in Bird Uni 2D  Helper - Rotate Clockwise "+whichWay);
			//MakeKinematicTrueAll();
			//DisableColliders();
			//localRotation = Quaternion.Euler(10, 0, 0) * Quaternion.Euler(0, 180, 0);
		
			// e.g. iTween.RotateTo(Dragon010Head, iTween.Hash("y", 0.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.4f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));

			iTween.RotateTo(BirdRoot, iTween.Hash("y", 180.0f, "islocal", true, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.6f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));

			//iTween.RotateTo(BirdRoot,iTween.Hash("rotation",new Vector3(0, 180.0f, 0),iTween.EaseType.easeInOutExpo,"time",1.3f));
			//BirdRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);
			//	BirdRoot.transform.Rotate(0, 180.0f, 0, Space.Self);
			//BirdRoot.animation.Play("RotateClockwise", PlayMode.StopAll);
			whichWay=false;
			//ToggleIsKinematicAll();


		} else {
			//MakeKinematicTrueAll();
			//DisableColliders();
			// can I iTween here
			iTween.RotateTo(BirdRoot, iTween.Hash("y", 0.0f, "islocal", true, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", 0.6f, "onCompleteTarget", gameObject,"onComplete", "resetPreRotationState"));

			//BirdRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
			//BirdRoot.transform.Rotate(0, -180.0f, 0, Space.Self);
			//Debug.Log("We're in Bird Uni 2D Helper - Rotate Anti-Clockwise True");
			//BirdRoot.animation.Play("RotateCounterClockwise", PlayMode.StopAll);
			whichWay = true;
			//ToggleIsKinematicAll();

		}
		
		
	}

	public void resetPreRotationState() {
		
		
		//MakeKinematicFalseAll();
		//SetColliders(true);
		//MakeKinematicFalseHead();
		//MakeKinematicFalseAll();
		Debug.Log("Tween callback called PuppetHelperIIMBird resetPreRotationState()");
		
	}


		void DisableColliders() {

		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (BirdHead) { BirdHead.collider2D.enabled = false; Debug.Log("In disableColliders(). Setting BirdHead to off"); }
			//if (BirdBody) { BirdBody.collider2D.enabled = false; }
			if (BirdNeck1) { BirdNeck1.collider2D.enabled = false; }
			if (BirdNeck2) { BirdNeck2.collider2D.enabled = false; }
			if (BirdNeck3) { BirdNeck3.collider2D.enabled = false; }
			if (BirdLeg_L_Upper) { BirdLeg_L_Upper.collider2D.enabled = false; }
			if (BirdLeg_L_Lower) { BirdLeg_L_Lower.collider2D.enabled = false; }
			if (BirdLeg_R_Upper) { BirdLeg_R_Upper.collider2D.enabled = false; }
			if (BirdLeg_R_Lower) { BirdLeg_R_Lower.collider2D.enabled = false; }

			
		} else {
			
			Debug.Log("disableColliders() - I'm a 3D object with 2D rigidbodies");
			if (BirdHead) { BirdHead.collider.enabled = false; Debug.Log("In disableColliders(). Setting BirdHead to off"); }
			//if (BirdBody) { BirdBody.collider.enabled = false; }
			if (BirdNeck1) { BirdNeck1.collider.enabled = false; }
			if (BirdNeck2) { BirdNeck2.collider.enabled = false; }
			if (BirdNeck3) { BirdNeck3.collider.enabled = false; }
			if (BirdLeg_L_Upper) { BirdLeg_L_Upper.collider.enabled = false; }
			if (BirdLeg_L_Lower) { BirdLeg_L_Lower.collider.enabled = false; }
			if (BirdLeg_R_Upper) { BirdLeg_R_Upper.collider.enabled = false; }
			if (BirdLeg_R_Lower) { BirdLeg_R_Lower.collider.enabled = false; }

			
		}
	}
	
	
	void SetColliders(bool toggleState) {

		if (is2D) {
			
			Debug.Log("SetColliders(toggleState) - I'm actually a 2D object with 2D rigidbodies");
			if (BirdHead) { BirdHead.collider2D.enabled = toggleState; Debug.Log("In toggleColliders(). Setting BirdHead to "+toggleState); }
//			if (BirdBody) { BirdBody.collider2D.enabled = toggleState; }
			if (BirdNeck1) { BirdNeck1.collider2D.enabled = toggleState; }
			if (BirdNeck2) { BirdNeck2.collider2D.enabled = toggleState; }
			if (BirdNeck3) { BirdNeck3.collider2D.enabled = toggleState; }
			if (BirdLeg_L_Upper) { BirdLeg_L_Upper.collider2D.enabled = toggleState; }
			if (BirdLeg_L_Lower) { BirdLeg_L_Lower.collider2D.enabled = toggleState; }
			if (BirdLeg_R_Upper) { BirdLeg_R_Upper.collider2D.enabled = toggleState; }
			
			
		} else {
			
			Debug.Log("SetColliders(toggleState) - I'm a 3D object with 2D rigidbodies");
			if (BirdHead) { BirdHead.collider.enabled = toggleState; Debug.Log("In toggleColliders(). Setting BirdHead to "+toggleState); }
//			if (BirdBody) { BirdBody.collider.enabled = toggleState; }
			if (BirdNeck1) { BirdNeck1.collider.enabled = toggleState; }
			if (BirdNeck2) { BirdNeck2.collider.enabled = toggleState; }
			if (BirdNeck3) { BirdNeck3.collider.enabled = toggleState; }
			if (BirdLeg_L_Upper) { BirdLeg_L_Upper.collider.enabled = toggleState; }
			if (BirdLeg_L_Lower) { BirdLeg_L_Lower.collider.enabled = toggleState; }
			if (BirdLeg_R_Upper) { BirdLeg_R_Upper.collider.enabled = toggleState; }
			
			
		}



	}

	public void MoveObjectBirdUni2DBody(bool Pad2PuppetController2TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerBody.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectBirdUni2DBeak(bool Pad2PuppetController2TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerBeak.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectBirdUni2DHead(bool Pad2PuppetController2TouchFlag,  float xx3, float yy3, float zz3) {


		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerHead.rigidbody2D.isKinematic = true;
		ControllerHead.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerHead.rigidbody2D.isKinematic = false;

	}


	public void MoveObjectBirdUni2DLeftKnee(bool Pad2PuppetController2TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerLeftKnee.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectBirdUni2DRightKnee(bool Pad2PuppetController2TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerRightKnee.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}


	// Toggle control objects
	public void ToggleControllerObject1 () {

		if (toggleController1==false){
			ControllerHead.SetActive(true); toggleController1 = true;
		} else {
			ControllerHead.SetActive(false); toggleController1=false;
		}


	}

	public void SetControllerObject1 (bool value) {
		
		
		ControllerHead.SetActive(value);
		
		
		
	}


	
	public void SetControllerObject2 (bool value) {
		
		
		ControllerBody.SetActive(value);
		
		
		
	}


	public void SetControllerObject3 (bool value) {
		
		
		ControllerBeak.SetActive(value);

		
	}

	
	public void SetControllerObject4 (bool value) {
		
		
		ControllerLeftKnee.SetActive(value);
		
		
		
	}

	public void SetControllerObject5 (bool value) {
		
		
		ControllerRightKnee.SetActive(value);
		
		
		
	}

	public void ToggleControllerObject2 () {
		
		if (toggleController2==false){
			ControllerBody.SetActive(true); toggleController2 = true;
		} else {
			ControllerBody.SetActive(false); toggleController2=false;
		}

		
	}
	

	public void ToggleControllerObject4 () {
		
		if (toggleController4==false){
			ControllerLeftKnee.SetActive(true); toggleController4 = true;
		} else {
			ControllerLeftKnee.SetActive(false); toggleController4=false;
		}

		
	}

	public void ToggleControllerObject5 () {
		
		if (toggleController5==false){
			ControllerRightKnee.SetActive(true); toggleController5 = true;
		} else {
			ControllerRightKnee.SetActive(false); toggleController5=false;
		}
		

		
	}


	void FixedUpdate(){


		if (Input.GetKeyUp(KeyCode.Backslash)) {
		// should all movement updates go here?

			// this logic toggles controller objects in a defined sequence
			// note if the physics object moves when the controller is disabled
			// the controller will not follow along
			// TODO options: 1: turn off rendering and toggle influence
			// 2: know the offset from the controlled object and reposition on activation. Preferred solution
			// EXPERIMENT: group the controllers

			controllerKeyCount++;
			if (controllerKeyCount == 1) {
				Debug.Log("Key Backslash key is pressed 1"); 
				SetControllerObject1(true);
				SetControllerObject2(true);
				SetControllerObject3(true);
				SetControllerObject4(false);
				SetControllerObject5(false);

				
			}
			if (controllerKeyCount == 2) { Debug.Log("Key Backslash key is pressed 2"); 
				SetControllerObject1(true);
				SetControllerObject2(true);
				SetControllerObject3(true);
				SetControllerObject4(true);
				SetControllerObject5(true);
			
			
			
			}
			if (controllerKeyCount == 3) { Debug.Log("Key Backslash key is pressed 3");  
				SetControllerObject1(true);
				SetControllerObject2(false);
				SetControllerObject3(false);
				SetControllerObject4(false);
				SetControllerObject5(false);

			
			}
			if (controllerKeyCount == 4) { Debug.Log("Key Backslash key is pressed 4"); controllerKeyCount = 1;
				SetControllerObject1(true);
				SetControllerObject2(true);
				SetControllerObject3(false);
				SetControllerObject4(false);
				SetControllerObject5(false);
			}


	}
	}
	
}
using UnityEngine;
using System.Collections;

public class PuppetHelperIIM_Karagoz_As_Horse: MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	// IIM_Bird_Uni2D accepts 3 touches: [1] head, [2] body then [3] beak aim
	public GameObject Root;
	public GameObject Head;
	public GameObject Body;
	
	public GameObject ControllerHead;
	public GameObject ControllerBody;
	public GameObject ControllerMisc;

	public UnityOSCListener mainOSCListener;

	bool monochromeState;
	
	// Use this for initialization
//	private PuppetHelper my2DHelper;
	Vector3 thisRotationNow;
	Vector3 thisRotationTarget;
	bool facingLeft;
	bool rotating;
	bool toggleHangState = false;
	
	// state
	Vector3 cachePosControllerHead; 			
	Vector3 cachePosControllerBody;		
	Vector3 cachePosControllerMisc;	

	
	void OnEnable () {
		monochromeState = mainOSCListener.getMonochromeState();
		Debug.Log("Monochrome State = "+monochromeState);
		
		if (monochromeState) { monochromeColorMode(); } else { normalColorMode(); }

	}
	
	
	void Awake () {
		Debug.Log ("An instance of PuppetHelper has been created for the object: ");
		// I guess the instance isn't around at 'awake' time
		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");
		
	
		// uncomment and add function to keyboard helper for this object with the same function name below
//		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_NAME_PUPPET_HERE(Root);


		// TODO implement a flag to check if 

	
		
	}

	public void SayHello() {
		// testing 
		Debug.Log("Hello from PuppetHelper - called by keyboard helper");
		
	}
	void Start () {
		Debug.Log ("An instance of my2DHelper has been created: START");
		monochromeState = mainOSCListener.getMonochromeState();
		Debug.Log("Monochrome State = "+monochromeState);

		
//		my2DHelper = (PuppetHelper)FindObjectOfType(typeof(PuppetHelper));
		
		
	}
	
	// IsKinetic Toggles and Controls
	// Delete / Add as required by object. There is probably a single call that could do this
	public void MakeKinematicTrueHeadBody() {
		Head.rigidbody.isKinematic    = true;
		Body.rigidbody.isKinematic    = true;
	}	
	
	public void MakeKinematicFalseHeadBody() {
		Head.rigidbody.isKinematic    = false;
		Body.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueBody() {
		Body.rigidbody.isKinematic    = true;
	}	
	
	public void MakeKinematicFalseBody() {
		Body.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueHead() {
		Head.rigidbody.isKinematic    = true;
	}
	
	public void MakeKinematicFalseHead() {
		Head.rigidbody.isKinematic    = false;
	}	
	
	public void MakeKinematicFalseAll() {
		Head.rigidbody.isKinematic   	 	= false;
		Body.rigidbody.isKinematic    		= false;

	}
	
	
	public void MakeKinematicTrueAll() {
		
		Head.rigidbody.isKinematic   	 	= true;
		Body.rigidbody.isKinematic    	= true;

	}
	
	public void ToggleIsKinematicAll() {
		
		
		Head.rigidbody.isKinematic   	 	=!Head.rigidbody.isKinematic;   	 	
		Body.rigidbody.isKinematic    		=!Body.rigidbody.isKinematic;    	

		
	}
	
	public void ToggleHang() {
		
		// decide what hanging does
		// decide what resetting the bird does
		
		// Cache Controller Defaults
		// then hang
		// then, if toggle, reapply last cached settings
		if (toggleHangState == false) {

			cachePosControllerHead		= ControllerHead.transform.position;	
			cachePosControllerBody		= ControllerBody.transform.position;	

			
			SetDefaultScale();
			ControllerHead.transform.position 		=new Vector3 (1.826007f+1.807446f, 3.96988f+6.0f, -1.163427f);
			ControllerBody.transform.position 		=new Vector3 (-6.717631f+1.807446f, 2.901467f+6.0f, -1.163427f);
			toggleHangState = true;
			
			
		} else {
			
			SetDefaultScale();
			ControllerHead.transform.position 	 	= cachePosControllerHead;
			ControllerBody.transform.position 	 	= cachePosControllerBody;	
			toggleHangState = !toggleHangState;
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


		/*
		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Neck.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Body.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Leg_L_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Leg_L_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Leg_R_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)Leg_R_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
*/
		Head.transform.localScale = scaleVec;
		Body.transform.localScale = scaleVec;


		/*
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
*/
		
		// return to the control of physics
		//MakeKinematicFalseAll();
	}
	
	public void SetDefaultScale () {
		
		// MakeKinematicTrueAll();
		Vector3 defaultScale =new Vector3 (1, 1, 1);
		// e.g. DragonRoot.transform.localScale = new Vector3 (1, 1, 1);
		// MakeKinematicFalseAll();

		Head.transform.localScale = new Vector3(1,1,1);
		Body.transform.localScale = new Vector3(1,1,1);


		
	}
	
	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(0f,0f,0f,1f);
		
		/*
		 * Process for Uni2D
		 */
		
		//SpriteRenderer headRenderer = Head.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		//headRenderer.color = myColor; // Set to opaque black
		
		//((Uni2DSprite)Head.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Body.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Neck.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Leg_L_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Leg_R_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Leg_L_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		//((Uni2DSprite)Leg_R_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		/*
		Head..material.SetColor("_TintColor", myColor);
		Body.renderer.material.SetColor("_TintColor", myColor);
		BirdNeck1.renderer.material.SetColor("_TintColor", myColor);
		BirdNeck2.renderer.material.SetColor("_TintColor", myColor);
		Neck.renderer.material.SetColor("_TintColor", myColor);
		Leg_L_Upper.renderer.material.SetColor("_TintColor", myColor);
		Leg_R_Upper.renderer.material.SetColor("_TintColor", myColor);
		Leg_L_Lower.renderer.material.SetColor("_TintColor", myColor);
		Leg_R_Lower.renderer.material.SetColor("_TintColor", myColor);
*/

		// Process for Unity 2D Sprites
		SpriteRenderer headRenderer = Head.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer bodyRenderer = Body.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		bodyRenderer.color = myColor; // Set to opaque black

		//Dragon010Head.SpriteRenderer.color = new Color(1,1,1,1);
		
	}
	
	
	public void normalColorMode() {
		
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(1f,1f,1f,.9f);

		

		// Process for Unity 2D Sprites
		SpriteRenderer headRenderer = Head.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		headRenderer.color = myColor; // Set to opaque black
		SpriteRenderer bodyRenderer = Body.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		bodyRenderer.color = myColor; // Set to opaque black

		//Dragon010Head.SpriteRenderer.color = new Color(1,1,1,1);


		
	}
	
	public void EndScale (float sliderValue) {
		
		MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		
		// decide what resetting the puppet does
		SetDefaultScale();

		ControllerHead.transform.position 		=new Vector3 (1.826007f+1.807446f, 3.96988f, -1.163427f);
		ControllerBody.transform.position 		=new Vector3 (-6.717631f+1.807446f, 2.901467f, -1.163427f);
		//ControllerMisc.transform.position =new Vector3 (-2.886333f+1.807446f, 3.30211f, -1.163427f);

		
	}
	
	public void Rotate(bool whichWay) {
		
		// 
		
		
		if (whichWay == true) {


			// this doesn't work - use iTween!

			//Debug.Log("We're in Bird Uni 2D  Helper - Rotate Clockwise "+whichWay);
			ToggleIsKinematicAll();
			//localRotation = Quaternion.Euler(10, 0, 0) * Quaternion.Euler(0, 180, 0);



			Root.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);
			//	BirdRoot.transform.Rotate(0, 180.0f, 0, Space.Self);
			//BirdRoot.animation.Play("RotateClockwise", PlayMode.StopAll);
			whichWay=false;
			ToggleIsKinematicAll();
			
			
		} else {
			ToggleIsKinematicAll();
			Root.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
			//BirdRoot.transform.Rotate(0, -180.0f, 0, Space.Self);
			//Debug.Log("We're in Bird Uni 2D Helper - Rotate Anti-Clockwise True");
			//BirdRoot.animation.Play("RotateCounterClockwise", PlayMode.StopAll);
			whichWay = true;
			ToggleIsKinematicAll();
			
		}
		
		
	}
	
	////////////
	// Move and Control Scripts
	////////////
	/// 
	/// 
	public void MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Body(bool Pad2PuppetController2TouchFlag,  float x, float y, float z) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerBody.transform.position = new Vector3(x, y, z);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	
	public void MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Head(bool Pad2PuppetController2TouchFlag,  float x, float y, float z) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerHead.rigidbody2D.isKinematic = true;
		ControllerHead.transform.position = new Vector3(x, y, z);
		//ControllerHead.rigidbody2D.isKinematic = false;
		
	}
	

	
	void FixedUpdate(){
		
		// should all movement updates go here?
	}
	
	
}
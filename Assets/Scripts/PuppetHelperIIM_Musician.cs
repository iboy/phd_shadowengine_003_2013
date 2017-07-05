using UnityEngine;
using System.Collections;

public class PuppetHelperIIM_Musician : MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	// IIM_Bird_Uni2D accepts 3 touches: [1] head, [2] body then [3] beak
	public GameObject MusicianRoot;
	public GameObject MusicianBody;
	public GameObject MusicianLeg_L;
	public GameObject MusicianLeg_R;
	public GameObject ControllerHead;
	public GameObject ControllerBack;



	// Use this for initialization
	private PuppetHelperIIM_Musician musicianUni2DHelper;
	Vector3 birdRotationNow;
	Vector3 birdRotationTarget;
	bool facingLeft;
	bool rotating;
	bool toggleHangState = false;

	// state

	Vector3 cachePosControllerBack;		
	Vector3 cachePosControllerHead;		






	void Awake () {
		Debug.Log ("An instance of Musician has been created: AWAKE");
		// I guess the instance isn't around at 'awake' time
		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");
		

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
		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_IIMMusician(MusicianRoot);
		

	}
	
	void Start () {
		Debug.Log ("An instance of musicianUni2DHelper has been created: START");

		
		musicianUni2DHelper = (PuppetHelperIIM_Musician)FindObjectOfType(typeof(PuppetHelperIIM_Musician));

		
	}
	
	// IsKinetic Toggles and Controls
	public void MakeKinematicTrueHeadBody() {
		MusicianBody.rigidbody.isKinematic    = true;

	}	
	
	public void MakeKinematicFalseHeadBody() {
		MusicianBody.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueBody() {
		MusicianBody.rigidbody.isKinematic    = true;
	}	
	
	public void MakeKinematicFalseBody() {
		MusicianBody.rigidbody.isKinematic    = false;
	}
	
	public void MakeKinematicTrueHead() {

	}
	
	public void MakeKinematicFalseHead() {

	}	
	
	public void MakeKinematicFalseAll() {
		MusicianBody.rigidbody.isKinematic    = false;
		MusicianLeg_L.rigidbody.isKinematic    = false;
		MusicianLeg_R.rigidbody.isKinematic    = false;

	}
		
		
	public void MakeKinematicTrueAll() {

		MusicianBody.rigidbody.isKinematic    = true;
		MusicianLeg_L.rigidbody.isKinematic    = true;
		MusicianLeg_R.rigidbody.isKinematic    = true;
	}
	
	public void ToggleIsKinematicAll() {
		

		MusicianLeg_R.rigidbody.isKinematic = !MusicianLeg_R.rigidbody.isKinematic;
		MusicianLeg_L.rigidbody.isKinematic = !MusicianLeg_L.rigidbody.isKinematic;
		MusicianBody.rigidbody.isKinematic = !MusicianBody.rigidbody.isKinematic;

		
	}
	
	public void ToggleHang() {
		
		// decide what hanging does
		// decide what resetting the bird does

		// Cache Controller Defaults
		// then hang
		// then, if toggle, reapply last cached settings
		if (toggleHangState == false) {
			cachePosControllerHead		= ControllerHead.transform.position;	
			cachePosControllerBack		= ControllerBack.transform.position;	

			
			
			SetDefaultScale();

			ControllerHead.transform.position 		=new Vector3 (-2.182693f+1.807446f, 3.812082f+6.0f, -1.163427f);
			ControllerBack.transform.position 		=new Vector3 (-0.6826942f+1.807446f, 2.262082f+6.0f, -1.163427f);

			toggleHangState = true;


		} else {

		SetDefaultScale();

			ControllerHead.transform.position 	 	= cachePosControllerHead;
			ControllerBack.transform.position 	 	= cachePosControllerBack;	

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
		//((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		/*
		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
		((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =scaleVec;
*/
		
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
		/*
		((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).transform.localScale =defaultScale;
		*/
		
	}

	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(0f,0f,0f,1f);

		/*
		 * Process for Uni2D


		//SpriteRenderer headRenderer = BirdHead.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		//headRenderer.color = myColor; // Set to opaque black

		((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
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

		// * Process for Unity 2D Sprites
		SpriteRenderer MusicianBody2 = MusicianBody.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianBody2.color = myColor; // Set to opaque black
		SpriteRenderer MusicianLegR = MusicianLeg_R.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianLegR.color = myColor; // Set to opaque black
		SpriteRenderer MusicianLegL = MusicianLeg_L.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianLegL.color = myColor; // Set to opaque black


	}

	
	public void normalColorMode() {
		
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
		Color myColor = new Color(1f,1f,1f,.9f);

		/*((Uni2DSprite)BirdHead.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdBody.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck1.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck2.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdNeck3.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_L_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_R_Upper.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_L_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
		((Uni2DSprite)BirdLeg_R_Lower.GetComponent(typeof(Uni2DSprite))).VertexColor =myColor;
*/
		/*
		 * Process for Unity 2D Sprites
		 * */
		SpriteRenderer MusicianBody2 = MusicianBody.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianBody2.color = myColor; // Set to opaque black
		SpriteRenderer MusicianLegR = MusicianLeg_R.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianLegR.color = myColor; // Set to opaque black
		SpriteRenderer MusicianLegL = MusicianLeg_L.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		MusicianLegL.color = myColor; // Set to opaque black
		//Dragon010Head.SpriteRenderer.color = new Color(1,1,1,1);

		
	}

	public void EndScale (float sliderValue) {
			
			MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		
		// decide what resetting the bird does
		SetDefaultScale();
		ControllerHead.transform.position =new Vector3 (-2.182693f+1.807446f, 3.812082f, -1.163427f);
		ControllerBack.transform.position =new Vector3 (-0.6826942f+1.807446f, 2.262082f, -1.163427f);

	}
	
	public void Rotate(bool whichWay) {
		
		// 


		if (whichWay == true) {

			//Debug.Log("We're in Bird Uni 2D  Helper - Rotate Clockwise "+whichWay);
			ToggleIsKinematicAll();
			//localRotation = Quaternion.Euler(10, 0, 0) * Quaternion.Euler(0, 180, 0);
		
			MusicianRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 180.0f, 0);
			//	BirdRoot.transform.Rotate(0, 180.0f, 0, Space.Self);
			//BirdRoot.animation.Play("RotateClockwise", PlayMode.StopAll);
			whichWay=false;
			ToggleIsKinematicAll();


		} else {
			ToggleIsKinematicAll();
			MusicianRoot.transform.rotation = Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, 0, 0);
			//BirdRoot.transform.Rotate(0, -180.0f, 0, Space.Self);
			//Debug.Log("We're in Bird Uni 2D Helper - Rotate Anti-Clockwise True");
			//BirdRoot.animation.Play("RotateCounterClockwise", PlayMode.StopAll);
			whichWay = true;
			ToggleIsKinematicAll();

		}
		
		
	}



	public void MoveObjectMusicianHead(bool Pad2PuppetController4TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerHead.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}

	public void MoveObjectMusicianBack(bool Pad2PuppetController4TouchFlag,  float xx3, float yy3, float zz3) {
		
		
		// latest: try to move the controller object
		// nice experiment different configurations of the iSKinematic property of the 'controllers'
		//ControllerTail.rigidbody2D.isKinematic = true;
		ControllerBack.transform.position = new Vector3(xx3, yy3, zz3);
		//ControllerTail.rigidbody2D.isKinematic = false;
	}








	void FixedUpdate(){

		// should all movement updates go here?
	}

	
}
using UnityEngine;
using System.Collections;

public class PuppetHelperIIM_Bird_001 : MonoBehaviour {
	
	
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs
	public GameObject Root;
	public GameObject IIMBird001Head;
	public GameObject IIMBird001Neck1;
	public GameObject IIMBird001Neck2;
	public GameObject IIMBird001Neck3;
	public GameObject IIMBird001Body;
	public GameObject IIMBird001LLegUpper;
	public GameObject IIMBird001LLegLower;
	public GameObject IIMBird001RLegUpper;
	public GameObject IIMBird001RLegLower;
	public GameObject IIMBird001BodyNeckJointAnchor;
	public GameObject IIMBird001BodyLLegJointAnchor;
	public GameObject IIMBird001BodyRLegJointAnchor;
	public GameObject IIMBird001LLegLowerJointAnchor;
	public GameObject IIMBird001RLegLowerJointAnchor;
	public GameObject IIMBird001Neck1JointAnchor;
	public GameObject IIMBird001Neck2JointAnchor;
	public GameObject IIMBird001HeadJointAnchor;
	//private UnityOSCListener UnityOSCController;

	// Use this for initialization

	
	void Awake () {
		//Debug.Log ("An instance of IIM_Bird has been created: AWAKE method");
		// I guess the instance isn't around at 'awake' time

		GameObject _AnimationOSCController = GameObject.Find("_AnimationOSCController");
		GameObject _AnimationHandlers = GameObject.Find("_AnimationHandlers");
		GameObject _KeyboardController = GameObject.Find("_KeyboardController");

		//go.GetComponent<UnityOSCListener>().HelloWorld("HelloFromTheNewBird");
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject1(IIMBird001Head);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject2(IIMBird001Body);
		_AnimationOSCController.GetComponent<UnityOSCListener>().SetObject3(Root);

		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject1(IIMBird001Head);
		_AnimationHandlers.GetComponent<AnimationHandlers>().SetObject2(IIMBird001Body);

		_KeyboardController.GetComponent<KeyboardController>().SetPuppet_IIM_Bird_001(Root);

		//UnityOSCListener theOSCListener = (UnityOSCListener)AnimationOSCController.GetComponent(typeof(UnityOSCListener));

		//theOSCListener.object1 = this.IIMBird001Head;
		
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


	public void doIExist(string whereFrom){

		Debug.Log("PuppetBird Controller: exist I do!: from "+whereFrom);
	}
	void Start () {
	Debug.Log ("An instance of IIM_Bird has been created: START");
	}
	
	// IsKinetic Toggles and Controls
	public void MakeKinematicTrueHeadBody() {
		IIMBird001Head.rigidbody.isKinematic      = true;
		IIMBird001Body.rigidbody.isKinematic      = true;
	}	
	
	public void MakeKinematicFalseHeadBody() {
		IIMBird001Head.rigidbody.isKinematic      = false;
		IIMBird001Body.rigidbody.isKinematic      = false;
	}
	
	public void MakeKinematicTrueBody() {
		IIMBird001Body.rigidbody.isKinematic      = true;
	}	
	
	public void MakeKinematicFalseBody() {
		IIMBird001Body.rigidbody.isKinematic      = false;
	}
	
	public void UseGravityTrueHead() {
		IIMBird001Head.rigidbody.useGravity      = true;
	}

	public void UseGravityFalseHead() {
		IIMBird001Head.rigidbody.useGravity      = false;
	}

	public void MakeKinematicTrueHead() {
		IIMBird001Head.rigidbody.isKinematic      = true;
	}	
	public void MakeKinematicFalseHead() {
		IIMBird001Head.rigidbody.isKinematic      = false;	
	}	
	
	public void MakeKinematicFalseAll() {
		IIMBird001Head.rigidbody.isKinematic      = false;
		IIMBird001Neck1.rigidbody.isKinematic     = false;
		IIMBird001Neck3.rigidbody.isKinematic     = false;
		IIMBird001Neck3.rigidbody.isKinematic     = false;
		IIMBird001Body.rigidbody.isKinematic      = false;
		IIMBird001LLegUpper.rigidbody.isKinematic = false;
		IIMBird001LLegLower.rigidbody.isKinematic = false;
		IIMBird001RLegUpper.rigidbody.isKinematic = false;
		IIMBird001RLegLower.rigidbody.isKinematic = false;
	}
		
		
	public void MakeKinematicTrueAll() {
		IIMBird001Head.rigidbody.isKinematic      = true;
		IIMBird001Neck1.rigidbody.isKinematic     = true;
		IIMBird001Neck3.rigidbody.isKinematic     = true;
		IIMBird001Neck3.rigidbody.isKinematic     = true;
		IIMBird001Body.rigidbody.isKinematic      = true;
		IIMBird001LLegUpper.rigidbody.isKinematic = true;
		IIMBird001LLegLower.rigidbody.isKinematic = true;
		IIMBird001RLegUpper.rigidbody.isKinematic = true;
		IIMBird001RLegLower.rigidbody.isKinematic = true;
		
	}
	
	public void ToggleIsKinematicAll() {
		
		IIMBird001Head.rigidbody.isKinematic      = !IIMBird001Head.rigidbody.isKinematic;
		IIMBird001Neck1.rigidbody.isKinematic     = !IIMBird001Neck1.rigidbody.isKinematic;
		IIMBird001Neck3.rigidbody.isKinematic     = !IIMBird001Neck3.rigidbody.isKinematic;
		IIMBird001Neck3.rigidbody.isKinematic     = !IIMBird001Neck3.rigidbody.isKinematic;
		IIMBird001Body.rigidbody.isKinematic      = !IIMBird001Body.rigidbody.isKinematic;
		IIMBird001LLegUpper.rigidbody.isKinematic = !IIMBird001LLegUpper.rigidbody.isKinematic;
		IIMBird001LLegLower.rigidbody.isKinematic = !IIMBird001LLegLower.rigidbody.isKinematic;
		IIMBird001RLegUpper.rigidbody.isKinematic = !IIMBird001RLegUpper.rigidbody.isKinematic;
		IIMBird001RLegLower.rigidbody.isKinematic = !IIMBird001RLegLower.rigidbody.isKinematic;
	}
	
	public void ToggleHang() {
		
		// decide what hanging the bird does
		if (IIMBird001Head.rigidbody.isKinematic) { ToggleIsKinematicAll(); }
		
		
	}
	
	public void SetScale (float sliderValue) {
			
		
		// Thought processes
		// Anchor positions of joints don't scale with the scale of the object
		// solution 1 add game object nulls at the transform points, parent then sensibly and re-set anchor points during / after a scale
		// Toggle IsKinectic functions switch all rigidbodies.kinetic to false
		
		// problems seems to work for one joint but explode others
		// should I scale the root game object (parent of all the rigid bodies);
			MakeKinematicTrueAll();
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
			IIMBird001Head.transform.localScale = scaleVec;
			IIMBird001Neck1.transform.localScale = scaleVec;
			IIMBird001Neck2.transform.localScale = scaleVec;
			IIMBird001Neck3.transform.localScale = scaleVec;
			IIMBird001Body.transform.localScale = scaleVec;
			IIMBird001LLegUpper.transform.localScale = scaleVec;
			IIMBird001LLegLower.transform.localScale = scaleVec;
			IIMBird001RLegUpper.transform.localScale = scaleVec;
			IIMBird001RLegLower.transform.localScale = scaleVec;

		// remove attempt at replacing the joints
		//HingeJoint neck1HeadJoint = IIMBird001Neck1.GetComponent<HingeJoint>(); //HingeJoint // neck1 is connect to the head
		//neck1HeadJoint.anchor = new Vector3(IIMBird001HeadJointAnchor.transform.localPosition.x,IIMBird001HeadJointAnchor.transform.localPosition.y,IIMBird001HeadJointAnchor.transform.localPosition.z);
		
		
		
		//HingeJoint neck2Neck1Joint = IIMBird001Neck2.GetComponent<HingeJoint>(); //HingeJoint
		//neck2Neck1Joint.anchor = new Vector3(IIMBird001Neck1JointAnchor.transform.localPosition.x,IIMBird001Neck1JointAnchor.transform.localPosition.y,IIMBird001Neck1JointAnchor.transform.localPosition.z);
		
		//HingeJoint neck3Neck2Joint = IIMBird001Neck3.GetComponent<HingeJoint>(); // Hingejoint
		//neck3Neck2Joint.anchor = new Vector3(IIMBird001Neck2JointAnchor.transform.localPosition.x,IIMBird001Neck2JointAnchor.transform.localPosition.y,IIMBird001Neck2JointAnchor.transform.localPosition.z);
		
		
		//HingeJoint bodyNeck3Joint = IIMBird001Neck3.GetComponent<HingeJoint>(); // Hingejoint
		//bodyNeck3Joint.anchor = new Vector3(IIMBird001BodyNeckJointAnchor.transform.localPosition.x,IIMBird001BodyNeckJointAnchor.transform.localPosition.y,IIMBird001BodyNeckJointAnchor.transform.localPosition.z);
		
		
		//bodyneckjoint.connectedBody = null; // test and see if we need to remove the connected body to minimise physics explosions
		//bodyneckjoint.connectedBody = IIMBird001Neck3.rigidbody;// test and see if we need to remove the connected body to minimise physics explosions
		//bodyNeck3Joint.anchor = new Vector3(IIMBird001BodyNeckJointAnchor.transform.localPosition.x,IIMBird001BodyNeckJointAnchor.transform.localPosition.y,IIMBird001BodyNeckJointAnchor.transform.localPosition.z);
		
			
		
		/*
		 * 
IIMBird001HeadJointAnchor;  -> 
IIMBird001Neck1JointAnchor; -> head
IIMBird001Neck2JointAnchor; -> neck 1
IIMBird001BodyNeckJointAnchor; - body - neck3
IIMBird001BodyLLegJointAnchor;  - body
IIMBird001LLegLowerJointAnchor;

IIMBird001BodyRLegJointAnchor;
IIMBird001RLegLowerJointAnchor;



*/
		
		
		
		// return to the control of physics
		MakeKinematicFalseAll();
	}
		
	public void SetDefaultScale () {
			
			MakeKinematicTrueAll();
			IIMBird001Head.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001Neck1.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001Neck3.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001Neck3.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001Body.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001LLegUpper.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001LLegLower.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001RLegUpper.transform.localScale = new Vector3 (1, 1, 1);
			IIMBird001RLegLower.transform.localScale = new Vector3 (1, 1, 1);
			MakeKinematicFalseAll();
		
	}
		
	public void EndScale (float sliderValue) {
			
			MakeKinematicFalseAll();
		
	}
	
	public void Reset() {
		
		// decide what resetting the bird does
		
		
	}
	
	public void Rotate() {
		
		// decide what rotating the bird does

		// currently it triggers an animation on the bird but that happend on a fixed axis independent of the figure... so it always rotates aroudn the origin
		
	}
}

using UnityEngine;
using System.Collections;

public class SceneryAnimationTweenIn : MonoBehaviour {
	//Define Enum
	public enum SetEntryDirections{		FromTop, 	 FromBottom,   FromLeft, FromRight};

	public enum SetEasingTypes{ 		spring, easeInQuad,  easeOutQuad,  easeInOutQuad, 
										easeInCubic, easeOutCubic, easeInOutCubic, 
										easeInQuart, easeOutQuart, easeInOutQuart, 
										easeInQuint, easeOutQuint, easeInOutQuint, 
										easeInSine,	 easeOutSine, easeInOutSine, 
										easeInExpo,  easeOutExpo, easeInOutExpo, 
										easeInCirc,  easeOutCirc, easeInOutCirc, 
										linear, 	  	  easeInBounce, 
										easeOutBounce, easeInOutBounce, easeInBack, 
										easeOutBack, easeInOutBack, easeInElastic, 
										easeOutElastic, easeInOutElastic			};

	
	//This is what you need to show in the inspector.
	public SetEntryDirections SetEntryDirection;
	public SetEasingTypes SetEasingType;
	// Use this for initialization
	//Hashtable ht = new Hashtable();
	bool onStage = false;
	public float inTriggerDelay = .1f;
	public float outTriggerDelay = .1f;
	public float inRotationSpeed = 1.0f;
	public float outRotationSpeed = 1.0f;
	public float randomDelayFactor = 1.0f;
	public float randomSpeedFactor = 1.0f;
	public string triggerKey = "z";
	string axis;
	float startAngle;
	float endAngle;
	string myEasingType = "spring";

	//public GameObject root;
	//public GameObject collider1;
	//public GameObject collider2;
	//public GameObject collider3;
	//public GameObject collider4;
	//public GameObject collider5;
	//public GameObject collider6;
	//public GameObject collider7;
	//public GameObject collider8;
	//public GameObject collider9;
	//public GameObject collider10;
	void Awake () {
		
		//ht.Add("y",4);
		//int myInt = SetEntryDirections.SetEntryDirection;
		randomDelayFactor = Random.Range(.5F, 1.5F);
		randomSpeedFactor = Random.Range(.5F, 1.5F);


		switch (SetEasingType) 
		{
		
		case SetEasingTypes.spring:
			myEasingType = "spring";
			break;
			
		case SetEasingTypes.easeInQuad:
			myEasingType = "easeInQuad";
			break;
			
		case SetEasingTypes.easeOutQuad:
			myEasingType = "easeOutQuad";
			break;
			
		case SetEasingTypes.easeInOutQuad:
			myEasingType = "easeInOutQuad";
			break;
			
		case SetEasingTypes.easeInCubic:
			myEasingType = "easeInCubic";
			break;
			
		case SetEasingTypes.easeOutCubic:
			myEasingType = "easeOutCubic";
			break;
			
		case SetEasingTypes.easeInOutCubic:
			myEasingType = "easeInOutCubic";
			break;
			
		case SetEasingTypes.easeInQuart:
			myEasingType = "easeInQuart";
			break;
			
		case SetEasingTypes.easeOutQuart:
			myEasingType = "easeOutQuart";
			break;
			
		case SetEasingTypes.easeInOutQuart:
			myEasingType = "easeInOutQuart";
			break;
			
		case SetEasingTypes.easeInQuint:
			myEasingType = "easeInQuint";
			break;
			
		case SetEasingTypes.easeOutQuint:
			myEasingType = "easeOutQuint";
			break;
			
		case SetEasingTypes.easeInOutQuint:
			myEasingType = "easeInOutQuint";
			break;
			
		case SetEasingTypes.easeInSine:
			myEasingType = "easeInSine";
			break;
			
		case SetEasingTypes.easeOutSine:
			myEasingType = "easeOutSine";
			break;
			
		case SetEasingTypes.easeInOutSine:
			myEasingType = "easeInOutSine";
			break;
			
		case SetEasingTypes.easeInExpo:
			myEasingType = "easeInExpo";
			break;
			
		case SetEasingTypes.easeOutExpo:
			myEasingType = "easeOutExpo";
			break;
			
		case SetEasingTypes.easeInOutExpo:
			myEasingType = "easeInOutExpo";
			break;
			
		case SetEasingTypes.easeInCirc:
			myEasingType = "easeInCirc";
			break;
			
		case SetEasingTypes.easeOutCirc:
			myEasingType = "easeOutCirc";
			break;
			
		case SetEasingTypes.easeInOutCirc:
			myEasingType = "easeInOutCirc";
			break;
			
		case SetEasingTypes.linear:
			myEasingType = "linear";
			break;
			
		case SetEasingTypes.easeInBounce:
			myEasingType = "easeInBounce";
			break;
			
		case SetEasingTypes.easeOutBounce:
			myEasingType = "easeOutBounce";
			break;
			
		case SetEasingTypes.easeInOutBounce:
			myEasingType = "easeInOutBounce";
			break;
			
		case SetEasingTypes.easeInBack:
			myEasingType = "easeInBack";
			break;
			
		case SetEasingTypes.easeOutBack:
			myEasingType = "easeOutBack";
			break;
			
		case SetEasingTypes.easeInOutBack:
			myEasingType = "easeInOutBack";
			break;
			
		case SetEasingTypes.easeInElastic:
			myEasingType = "easeInElastic";
			break;
			
		case SetEasingTypes.easeOutElastic:
			myEasingType = "easeOutElastic";
			break;
			
		case SetEasingTypes.easeInOutElastic:
			myEasingType = "easeInOutElastic";
				break;
		}


		switch (SetEntryDirection) 
		{
		case SetEntryDirections.FromTop:
 			
			axis = "x";
			startAngle = 180.0f;
			endAngle = 90.0f;
			//Debug.Log("FromTop: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;

		case SetEntryDirections.FromBottom:

			axis = "x";
			startAngle = 0.0f;
			endAngle = 90.0f;
			//Debug.Log("FromBottom: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;
		case SetEntryDirections.FromLeft:

			axis = "y";
			startAngle = 0f;
			endAngle = -90.0f;
			//Debug.Log("FromLeft: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;
		case SetEntryDirections.FromRight:
			axis = "y";
			startAngle = 180f;
			endAngle = 270f;
			//Debug.Log("FromRight: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);

			break;
		}

		
	}
	
	void FixedUpdate () {
		
		if (onStage == false) {
			
			if (Input.GetKeyUp(triggerKey) && (onStage == false)) {
				
				//disableColliders();
				//setIsKinematicTrue();
				//iTween.MoveTo(gameObject, iTween.Hash("x", .4f,"y", 2.5f, "z", 0, "easeType", "easeInOutExpo", "loopType", "none", "delay", .5, "time", 1.8f));

				iTween.RotateTo(gameObject, iTween.Hash(axis, startAngle, "easeType", myEasingType, "loopType", "none", "delay", inTriggerDelay*randomDelayFactor, "time", inRotationSpeed*randomSpeedFactor));

				randomDelayFactor = Random.Range(.7F, 1.7F);
				randomSpeedFactor = Random.Range(.7F, 1.7F);

				//	iTween.RotateTo(gameObject, iTween.Hash("x", -90.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", inTriggerDelay, "time", inRotationSpeed));

				//iTween.ShakeScale(root, iTween.Hash("amount", new Vector3(.02f,.02f,.02f), "delay", 0.2f, "time", 2.0f));
				//iTween.ScaleTo(root, iTween.Hash("x", 2f,"y", 2f, "z", 1f, "time", 1.0f, "onComplete", "setIsKinematicFalse", "onCompleteTarget", collider1));
				//setIsKinematicFalse();
				
				onStage = true;
			}
		} else {
			
			if (Input.GetKeyUp(triggerKey) && (onStage == true)) {
				//onStage = false;
				//disableColliders();
				//iTween.ShakeScale(root, iTween.Hash("amount", new Vector3(.01f,.02f,.01f), "delay", 2.4f, "time", 2.0f));
				iTween.RotateTo(gameObject, iTween.Hash(axis, endAngle, "easeType", myEasingType, "loopType", "none", "delay", outTriggerDelay*randomDelayFactor, "time",outRotationSpeed*randomSpeedFactor));
				//iTween.MoveTo(gameObject, iTween.Hash("x", -7f, "y", 4.22074f, "z", 0, "easeType", "easeInOutExpo", "loopType", "none", "delay", 2.0f, "time", 1.4f));

				randomDelayFactor = Random.Range(.7F, 1.7F);
				randomSpeedFactor = Random.Range(.7F, 1.7F);

				onStage = false;
			}
			
		}


		// I want this code to only execute in the editor - to test look and feel
#if UNITY_EDITOR
		
		switch (SetEasingType) 
		{
			
		case SetEasingTypes.spring:
			myEasingType = "spring";
			break;
			
		case SetEasingTypes.easeInQuad:
			myEasingType = "easeInQuad";
			break;
			
		case SetEasingTypes.easeOutQuad:
			myEasingType = "easeOutQuad";
			break;
			
		case SetEasingTypes.easeInOutQuad:
			myEasingType = "easeInOutQuad";
			break;
			
		case SetEasingTypes.easeInCubic:
			myEasingType = "easeInCubic";
			break;
			
		case SetEasingTypes.easeOutCubic:
			myEasingType = "easeOutCubic";
			break;
			
		case SetEasingTypes.easeInOutCubic:
			myEasingType = "easeInOutCubic";
			break;
			
		case SetEasingTypes.easeInQuart:
			myEasingType = "easeInQuart";
			break;
			
		case SetEasingTypes.easeOutQuart:
			myEasingType = "easeOutQuart";
			break;
			
		case SetEasingTypes.easeInOutQuart:
			myEasingType = "easeInOutQuart";
			break;
			
		case SetEasingTypes.easeInQuint:
			myEasingType = "easeInQuint";
			break;
			
		case SetEasingTypes.easeOutQuint:
			myEasingType = "easeOutQuint";
			break;
			
		case SetEasingTypes.easeInOutQuint:
			myEasingType = "easeInOutQuint";
			break;
			
		case SetEasingTypes.easeInSine:
			myEasingType = "easeInSine";
			break;
			
		case SetEasingTypes.easeOutSine:
			myEasingType = "easeOutSine";
			break;
			
		case SetEasingTypes.easeInOutSine:
			myEasingType = "easeInOutSine";
			break;
			
		case SetEasingTypes.easeInExpo:
			myEasingType = "easeInExpo";
			break;
			
		case SetEasingTypes.easeOutExpo:
			myEasingType = "easeOutExpo";
			break;
			
		case SetEasingTypes.easeInOutExpo:
			myEasingType = "easeInOutExpo";
			break;
			
		case SetEasingTypes.easeInCirc:
			myEasingType = "easeInCirc";
			break;
			
		case SetEasingTypes.easeOutCirc:
			myEasingType = "easeOutCirc";
			break;
			
		case SetEasingTypes.easeInOutCirc:
			myEasingType = "easeInOutCirc";
			break;
			
		case SetEasingTypes.linear:
			myEasingType = "linear";
			break;
			
		case SetEasingTypes.easeInBounce:
			myEasingType = "easeInBounce";
			break;
			
		case SetEasingTypes.easeOutBounce:
			myEasingType = "easeOutBounce";
			break;
			
		case SetEasingTypes.easeInOutBounce:
			myEasingType = "easeInOutBounce";
			break;
			
		case SetEasingTypes.easeInBack:
			myEasingType = "easeInBack";
			break;
			
		case SetEasingTypes.easeOutBack:
			myEasingType = "easeOutBack";
			break;
			
		case SetEasingTypes.easeInOutBack:
			myEasingType = "easeInOutBack";
			break;
			
		case SetEasingTypes.easeInElastic:
			myEasingType = "easeInElastic";
			break;
			
		case SetEasingTypes.easeOutElastic:
			myEasingType = "easeOutElastic";
			break;
			
		case SetEasingTypes.easeInOutElastic:
			myEasingType = "easeInOutElastic";
			break;
		}
		
		
		switch (SetEntryDirection) 
		{
		case SetEntryDirections.FromTop:
			
			axis = "x";
			startAngle = 180.0f;
			endAngle = 90.0f;
			//Debug.Log("FromTop: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;
			
		case SetEntryDirections.FromBottom:
			
			axis = "x";
			startAngle = 0.0f;
			endAngle = 90.0f;
			//Debug.Log("FromBottom: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;
		case SetEntryDirections.FromLeft:
			
			axis = "y";
			startAngle = 0f;
			endAngle = -90.0f;
			//Debug.Log("FromLeft: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			break;
		case SetEntryDirections.FromRight:
			axis = "y";
			startAngle = 180f;
			endAngle = 270f;
			//Debug.Log("FromRight: StartAngle= "+startAngle+"; EndAngle="+endAngle+"; Axis="+axis);
			
			break;
		}

#endif

	}
	
	//void setIsKinematicTrue() {
	//	Debug.Log("Setting isKinematic to True");
	//	if (collider2) { collider2.rigidbody.isKinematic = true; }
	//	if (collider3) { collider3.rigidbody.isKinematic = true; }
	//	if (collider4) { collider4.rigidbody.isKinematic = true; }
	//	if (collider5) { collider5.rigidbody.isKinematic = true; }
	//	if (collider6) { collider6.rigidbody.isKinematic = true; }
	//	if (collider7) { collider7.rigidbody.isKinematic = true; }
	//	if (collider8) { collider8.rigidbody.isKinematic = true; }
	//	if (collider9) { collider9.rigidbody.isKinematic = true; }
	//	if (collider10) { collider10.rigidbody.isKinematic = true; }
	//	
	//}
	//
	//void setIsKinematicFalse() {
	//	//Debug.Log("In function: setIsKinematicFalse()");
	//	//collider2.rigidbody.WakeUp();
	//	if (collider2) { collider2.rigidbody.isKinematic = false; }
	//	//Debug.Log("Setting isKinematic on collider 2 to false");
	//	if (collider3) { collider3.rigidbody.isKinematic = false; }
	//	if (collider4) { collider4.rigidbody.isKinematic = false; }
	//	if (collider5) { collider5.rigidbody.isKinematic = false; }
	//	if (collider6) { collider6.rigidbody.isKinematic = false; }
	//	if (collider7) { collider7.rigidbody.isKinematic = false; }
	//	if (collider8) { collider8.rigidbody.isKinematic = false; }
	//	if (collider9) { collider9.rigidbody.isKinematic = false; }
	//	if (collider10) { collider10.rigidbody.isKinematic = false; }
	//	
	//}
	//void disableColliders() {
	//	
	//	if (collider1) { collider1.collider.enabled = false; Debug.Log("In disableColliders(). Setting collider1 to off"); }
	//	if (collider2) { collider2.collider.enabled = false; }
	//	if (collider3) { collider3.collider.enabled = false; }
	//	if (collider4) { collider4.collider.enabled = false; }
	//	if (collider5) { collider5.collider.enabled = false; }
	//	if (collider6) { collider6.collider.enabled = false; }
	//	if (collider7) { collider7.collider.enabled = false; }
	//	if (collider8) { collider8.collider.enabled = false; }
	//	if (collider9) { collider9.collider.enabled = false; }
	//	if (collider10){ collider10.collider.enabled = false; }
	//}
	//void completedEntryAndRotate() {
	//	
	//	//Debug.Log("Completed Entry and Rotate");
	//	if (collider1) { collider1.collider.enabled = true; 
	//		//Debug.Log("Setting collider1 to on"); 
	//	}
	//	if (collider2) { collider2.collider.enabled = true; }
	//	if (collider3) { collider3.collider.enabled = true; }
	//	if (collider4) { collider4.collider.enabled = true; }
	//	if (collider5) { collider5.collider.enabled = true; }
	//	if (collider6) { collider6.collider.enabled = true; }
	//	if (collider7) { collider7.collider.enabled = true; }
	//	if (collider8) { collider8.collider.enabled = true; }
	//	if (collider9) { collider9.collider.enabled = true; }
	//	if (collider10){ collider10.collider.enabled = true; }
	//	//setIsKinematicFalse();
	//	//onStage = true;
	//}
	//
	//void completedExitAndRotate() {
	//	
	//	Debug.Log("Completed Exit and Rotate");
	//	if (collider1) { collider1.collider.enabled = true; }
	//	if (collider2) { collider2.collider.enabled = true; }
	//	if (collider3) { collider3.collider.enabled = true; }
	//	if (collider4) { collider4.collider.enabled = true; }
	//	if (collider5) { collider5.collider.enabled = true; }
	//	if (collider6) { collider6.collider.enabled = true; }
	//	if (collider7) { collider7.collider.enabled = true; }
	//	if (collider8) { collider8.collider.enabled = true; }
	//	if (collider9) { collider9.collider.enabled = true; }
	//	if (collider10){ collider10.collider.enabled = true; }
	//	onStage = false;
	//}
	
}

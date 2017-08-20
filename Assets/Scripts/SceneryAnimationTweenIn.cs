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

		onStage = false;
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
	
	void Update () {
		

			
			

		if (Input.GetKeyDown(triggerKey)) {

			if (onStage == false) {

				iTween.RotateTo(gameObject, iTween.Hash(axis, startAngle, "easeType", myEasingType, "loopType", "none", "delay", inTriggerDelay*randomDelayFactor, "time", inRotationSpeed*randomSpeedFactor));
				randomDelayFactor = Random.Range(.7F, 1.7F);
				randomSpeedFactor = Random.Range(.7F, 1.7F);
				onStage = true;
				Debug.Log ("Trigger key pressed - onstage = "+onStage+" startAngle ="+startAngle);

		
			} else if (onStage == true) {

				iTween.RotateTo(gameObject, iTween.Hash(axis, endAngle, "easeType", myEasingType, "loopType", "none", "delay", outTriggerDelay*randomDelayFactor, "time",outRotationSpeed*randomSpeedFactor));
				randomDelayFactor = Random.Range(.7F, 1.7F);
				randomSpeedFactor = Random.Range(.7F, 1.7F);
				onStage = false;
				Debug.Log ("Trigger key pressed - onstage = "+onStage+" endAngle ="+endAngle);


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

	
}

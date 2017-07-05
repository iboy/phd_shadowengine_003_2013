using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using OSC.NET;

public class UnityOSCListener : MonoBehaviour  {
	// all public references to objects or parts of objects need to be set with reference to 
	// instantiated prefabs. This now happens in each PuppetHelperScript and in Anination Handlers.
	public GameObject object1; //Head (bird)
	public GameObject object2; //Body (bird)
	public GameObject object3; //IIM_Bird_001 (root)
	public GameObject object4;
	public GameObject object5; //DragonLink 010 Head
	public GameObject object6; //DragonLink 000 Tail
	public GameObject object5c; //DragonLink Head controller
	public GameObject object6c; //DragonLink Tailcontroller


	public GameObject object7;

	public GameObject object8; //Musician Body
	public GameObject object8c; //Musician Head controller
	public GameObject object8cc; //Musician Backcontroller

	public GameObject Puppet1;
	public GameObject Puppet2;
	public GameObject Puppet3;
	public GameObject Puppet4;
	public GameObject Puppet5;

	public GameObject ControllerMrsMartin1;
	public GameObject ControllerMrsMartin2;
	

	
	
	public GameObject LemurObject1; // Sphere
	public GameObject LemurObject2;
	public GameObject LemurObject3;
	public GameObject LemurObject4;

	public Camera myCamera;
	public float multiplier = 6;

	// reference to Bird Puppet Helper for scripting
	private PuppetHelperIIM_Bird_001 birdHelper; // this gets set on instancing the prefab? How does the reference get here?
	private PuppetHelperDragon_001 dragonHelper;
	private PuppetHelperIIM_Bird_Uni2D birdHelperUni2D;
	private PuppetHelperIIM_Musician musicianHelper;
	private PuppetHelperIIM_Karagoz_As_Horse karagozAsHorseHelper;

	private PuppetHelperMrsMartin mrsMartinHelper;


	// Add helper objects here name of class, variable name
	

	private GameObject IIM_Bird_001;
	//dragon rotation

	// Scenography and Misc Objects 
	public Light mainLight; // Directional Light (currently the only light in the scene)
	public ColorPicker myColorPicker; //_ColorPicker (ColorPicker)
	public Texture2D colorPickerTex; //colorpicker_texture
	int screenWidth = Screen.width;
	int screenHeight = Screen.height;
	public GameObject iris;
	public GameObject fadeFX;
	
	//OSCtransmitter Business
	private bool connected = false;
	private bool connectedController002 = false;
	public int port = 9000; //TouchOSC default listening port 9000
	public int port002 = 9001; //TouchOSC default listening port 9000
	public string SendToIPAddress = "127.0.0.1"; //
	public string SendToIPAddress002 = "127.0.0.1"; //
	private OSCTransmitter transmitter; 
	private OSCTransmitter transmitterController002; 
	UnityOSCTransmitter myOSCTransmitter;
	UnityOSCTransmitter myOSCTransmitterController002;
	private OSCMessage myMessage;
	private OSCPacket myPacket;
	private OSCMessage myMessageController002;
	private OSCPacket myPacketController002;
	
	//InstanceChecker
	int birdInstanceCount = 0;

	Vector3 myVec3;
	AnimationHandlers myAnimationHandler;
	GameObject getMoveable;
	
	Vector3 firstTouch;
	Vector3 secondTouch;
	Vector3 thirdTouch;
	//Dragable theBirdsHeadDragable;
	
	
	
	// effects states and defaults
	bool blurState = false;
	float blurSize = 0.0f;
	bool motionBlurState = false;
	float motionBlurAmount = 0.0f;
	public bool monochromeToggle = false;

	// rotating object
	bool isRotated = false;

	// keyboard
	bool shift = false;
	int shiftCount;
	string[] captionPresets;
	bool lKeyUp = false;


	// DEBUGGING
	
	public bool debug = true;


	public bool getMonochromeState(){

		return monochromeToggle;


	}

	
	public  void setMonochromeState(bool monochromeToggleState){
		
		monochromeToggle = monochromeToggleState;

		if (monochromeToggleState == true) {
		myMessage = new OSCMessage("/3/FXMonochromeToggle", 1);
		transmitter.Send(myMessage);
		} else {

			myMessage = new OSCMessage("/3/FXMonochromeToggle", 0);
			transmitter.Send(myMessage);
		}

		
		
	}


	void Start() {
		
		// there's a nicer way without the 'find'
		GameObject myNetworkPresets = GameObject.Find("__NetworkPresets");

		// Controller 001
		SendToIPAddress = myNetworkPresets.GetComponent<NetworkPresets>().SendToIPAddress;
		port = myNetworkPresets.GetComponent<NetworkPresets>().Port;

		// Controller 002
		SendToIPAddress002 = myNetworkPresets.GetComponent<NetworkPresets>().SendToIPAddress002;
		port002 = myNetworkPresets.GetComponent<NetworkPresets>().PortController002;
/* SET DEFAULTS ON OSC INTERFACE  TODO THIS SHOULD BE REFACTORED INTO A CONTROLLER*/
		// create a transmitter to sync the UI

		// TODO It would be nice to count the number of connect devices
		// and iterate through them sending the same init data to each 

		transmitter = new OSCTransmitter(SendToIPAddress, port);

	//  try
	//  {
	//  	// Attempt to do everything in one line.
	//  	transmitter.Connect();
	//  	connected = true;
	//  	// Do something with bullet
	//  }
	//  catch(Exception e)
	//  {
	//  	// Something went wrong, so lets get information about it.
	//  	Debug.Log(e.ToString()+"Hello from the exception in UnityOSCListener.cs");
	//  	// Do something knowing the bullet isn't on screen.
	//  }

		transmitter.Connect();
		connected = true;

		//myMessage = new OSCMessage("/4" ); // switch to the fourth tab in TouchOSC
		//transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/CameraZoomOrtho", 2.61f ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/FXSunShaftsIntensity", 4.0f ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/FXVignetteSize", 5.08f ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/FXVignetteBlur", 1.0f ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/FXVignetteChromatic", 1.98f ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/DOFToggle", 0 ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/CameraTypeToggle/1/2", 0 ); 
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/FXMotionBlur", motionBlurAmount ); 
		transmitter.Send(myMessage);

		myMessage = new OSCMessage("/3/IrisLocationXYPad/x", 0.0f);
		transmitter.Send(myMessage);
		myMessage = new OSCMessage("/3/IrisLocationXYPad/y", 0.0f);
		transmitter.Send(myMessage);

		myMessage = new OSCMessage("/3/IrisTransitionSpeed", 0.2f);
		transmitter.Send(myMessage);
		irisDiameter(.2f);

		myMessage = new OSCMessage("/3/ManualFadeSlider/x", 0.0f);
		transmitter.Send(myMessage);


		myMessage = new OSCMessage("/2/Pad2Ctrl2Scale", 1.0f);
		transmitter.Send(myMessage);


		// Test setup a second transmitter to a second controller
		transmitterController002 = new OSCTransmitter(SendToIPAddress002, port002);
		transmitterController002.Connect();
		connectedController002 = true;


		// set all object scales to 1

		// init captions list
		initCaptionList();

		if (object1  == null) {

			Debug.Log("Bird Head is not Defined, probably because it doesn't exist.");

		}




	}
	
void Awake() {
		if (debug == true) {Debug.Log("We're in the AWAKE method");}
		
		// the code below lets this script know of the Animation Handlers Class
		// I think there can only be one instance of AnimationHandlers on a game object
		// in the scene/project
		myAnimationHandler = (AnimationHandlers)FindObjectOfType(typeof(AnimationHandlers));
		
		
		
		// create bird helper instance - provides convenience functions to rotate, toggleKinematic, hang, reset etc. 
		// This should happen on INSTANTIATION
		//birdHelper = (PuppetHelperIIM_Bird_001)FindObjectOfType(typeof(PuppetHelperIIM_Bird_001));

		// create dragon helper instance - provides convenience functions to rotate, toggleKinematic, hang, reset etc. 
		// This should happen on INSTANTIATION
		// dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
			
			
			
	}
	
	public void OSCMessageReceived(OSC.NET.OSCMessage message){	
		
		string address = message.Address;
		ArrayList args = message.Values;
		
		
		
		if (debug == true) {Debug.Log(address);}
		
		
		if (address == "/1/Pad2PuppetController1" ) {
			if (debug == true) {Debug.Log ("Lemur activated!");}
		}
		
		
		foreach( var item in args){
		if (debug == true) {Debug.Log(item);}

// ADDRESS SLIDER1
			
			if (address == "/1/Slider1") {
			
				if (debug == true) {Debug.Log (args[0]);}
				float SliderY1 = (float) args[0];
				//construct a mouse touch / touch point
				// Input.Touch point is a vec2
				// Input.Mouse is a vec3
				
				object1.transform.position = new Vector3(transform.position.x,SliderY1, transform.position.z);


			}

			
//-----------------PAGE 1 AND 2------CONTROLLER 1----------------------------//
// ADDRESS Pad2PuppetController1 FirstTouch			
			
			if (address == "/1/Pad2PuppetController1/1" || address == "/2/Pad2PuppetController1/1") {
			
			bool Pad2PuppetController1TouchFlag = true;
			// This is a problem: how do we manipulate the rigid body character with these values?
			// Where do we rescale the coordinates to the view / screen or world?
				
			// Construct a mouse touch / touch point
			// Input.Touch point is a vec2
			// Input.Mouse is a vec3
			// Then send to animationHandler or to the object helper?
				
			// TOUCHOSC INVERTS THIS
			// TEST FOR THE LEMUR
			float x1 = (float) args[1]   * multiplier;  
			float y1 = (float) args[0]  * multiplier;
			float z1 = 0.05f; // z of head in prefab
			
			// send first touch x y z to second and third touch scripts
			firstTouch = new Vector3(x1,y1,z1);
			
			//myVec3 = new Vector3(x1,y1, z1);
			
				
				if (object1  == null) {
					
					//Debug.Log("From first touch area: Bird Head is not Defined, probably because it doesn't exist.");
					//object1 = new GameObject("Bird Null");
					return;
				} else {
					// the bird exists
					if (debug == true) {Debug.Log ("IIM_Bird_001 Instance Exists");}
					//birdHelper = (PuppetHelperIIM_Bird_001)FindObjectOfType(typeof(PuppetHelperIIM_Bird_001));
					birdHelper = GameObject.Find("IIM_Bird_001").GetComponent<PuppetHelperIIM_Bird_001>();
				

				
			// send values directly to the dragable script...
		
			// do a check to see if the target transform is really far from the point of first touch
			// ideally - tween the object to the touch position
					

			// send Touch Values to the animation handler...
			


				

			// where the hell should this be set? 
					
				//if (object1  != null) {
				//birdHelper = GameObject.Find("IIM_Bird_001").GetComponent<PuppetHelperIIM_Bird_001>();

				
					//birdHelper.doIExist("from touch 1");

			//birdHelper.ToggleIsKinematicAll();    // it is important to track the calls to isKinematic and useGravity
			//birdHelper.MakeKinematicFalseHead();
			//	birdHelper.UseGravityFalseHead();
			
			
				//}

					//birdHelper.MakeKinematicTrueHeadBody();
				if (debug == true) {Debug.Log("Is Rotated = "+isRotated);}
			
				if (isRotated == false) { 

						myAnimationHandler.MoveObject1(Pad2PuppetController1TouchFlag, x1,y1,z1); 
					}
				
				else {
					
						myAnimationHandler.MoveObject1(Pad2PuppetController1TouchFlag, x1*-1,y1,z1);
				}
			
			//birdHelper.ToggleIsKinematicAll();
			//birdHelper.MakeKinematicTrueHead();
			//birdHelper.UseGravityTrueHead();
			// send the touch and coordinates to the dragable instance on the object we wish to control
			
			
			
			// Weird Flip - iPad in Landscape - Y comes in before X from TOUCH OSC. I've swopped the arguments
				
			//print ("1 fingers: doing the multi xy thing");
			//print ("1x = "+args[0]);
			//print ("1y = "+args[1]);

				
			if (debug == true) {Debug.Log("One Touch Detected: x ="+x1+" y ="+y1+" z ="+z1);}
			
			//object1.transform.position = new Vector3(x1,y1, transform.position.z);
				
				
		
			// try some scaling of the control values (-1 to 1) to the Unity Screen Size / World coords.
			
			//float finalX1 = x1 * (Screen.width) ;
			//float finalY1 = y1 * (Screen.height) ;
			
			
				
			//object1.rigidbody.isKinematic = true;
			//myVec3 = Camera.main.ScreenToWorldPoint(new Vector3(x1,y1, transform.position.z));
			//object1.transform.position = 	Camera.main.ScreenToWorldPoint(new Vector3(x1,y1, transform.position.z));
			//object1.transform.position = new Vector3(x1,y1, transform.position.z);
			//object1.rigidbody.isKinematic = false;
			if (debug == true) { Debug.Log("One Touch Detected: x ="+myVec3.x+" y ="+myVec3.y);}
				}
			//print ("Width:"+ Screen.width+ "Height: "+ Screen.height);
			
			//} else { //object1.rigidbody.isKinematic = false; 
				//bool Pad2PuppetController1TouchFlag = false;
				//object1.GetComponent<Dragable>().OnMoveObject(Pad2PuppetController1TouchFlag,0f,0f,0f);
				//object1.GetComponent<Dragable>().OnStopMoveObject();
				
				
					//birdHelper.UseGravityTrueHead();
					//birdHelper.MakeKinematicFalseHead();


				//}
			}
			
// ADDRESS Pad2PuppetController1 SECONDTouch
			
			if (address == "/1/Pad2PuppetController1/2" || address == "/2/Pad2PuppetController1/2") {
			
				if (object2 != null) {
				bool Pad2PuppetController1TouchFlag = true;
			//print ("2 fingers: doing the multi xy thing");
			//print ("2x = "+args[0]);
			//print ("2y = "+args[1]);
			float x1 = (float) args[1] * multiplier;
			float y1 = (float) args[0] * multiplier;
					float z1 =  0.05f;
			secondTouch = new Vector3(x1,y1, 0.05f);
			// do a check to see if the target transform is really far from the point of first touch
			// ideally - tween the object to the touch position
			// this is a LERP e.g. http://answers.unity3d.com/questions/63060/vector3lerp-works-outside-of-update.html
			// transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
			
			object2.rigidbody.isKinematic = true;
			object2.rigidbody.useGravity = false; // this lends a lot of control
			//object2.transform.position = new Vector3(x1,y1, transform.position.z);
				
				
				
			birdHelper.ToggleIsKinematicAll();
			myAnimationHandler.MoveObject2(Pad2PuppetController1TouchFlag, x1,y1,z1);
			birdHelper.ToggleIsKinematicAll();
			
			}

			}

// ADDRESS Pad2PuppetController1 THIRDTouch
			
			if (address == "/1/Pad2PuppetController1/3" || address == "/2/Pad2PuppetController1/3") {
			
				if (object1!=null) {
				bool Pad2PuppetController1TouchFlag = true;
			//print ("2 fingers: doing the multi xy thing");
			//print ("2x = "+args[0]);
			//print ("2y = "+args[1]);
			float x1 = (float) args[1] * multiplier;;
			float y1 = (float) args[0] * multiplier;;
					float z1 =  0.05f;
			thirdTouch = new Vector3(x1,y1,0.0f);
				
				
			// in principle this works as an twist gesture rotating the head object	
			Quaternion newRotation = Quaternion.LookRotation(thirdTouch - firstTouch, new Vector3(0, 0, 1));
			newRotation.x = 0.0f;
			newRotation.y = 0.0f;
			object1.rigidbody.isKinematic = true;
				object1.rigidbody.useGravity = false;
			object1.transform.rotation = Quaternion.Slerp(object1.transform.rotation, newRotation, Time.deltaTime * 70);
			//object1.rigidbody.isKinematic = false;
				
				// this line works out an angle from two vector point // check this is rotated round Z
			//object1.transform.rotation = Quaternion.LookRotation(thirdTouch - firstTouch, Vector3.forward);
			//Vector3 relativePos = thirdTouch - firstTouch;
        	//Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.forward);
        	//object1.transform.rotation = rotation;
				
			//	object1.rigidbody.isKinematic = true;
			//object1.transform.Rotate(0f,0f, 1f ); // this depends on +ve or -ve Y	
			//	object1.rigidbody.isKinematic = false;
			//object1.rigidbody.isKinematic = false;
			//object1.rigidbody.useGravity = false;
			//object2.transform.position = new Vector3(x1,y1, transform.position.z);
				
				
				
			//birdHelper.ToggleIsKinematicAll();
			//myAnimationHandler.MoveObject2(Pad2PuppetController1TouchFlag, x1,y1,z1);
			//birdHelper.ToggleIsKinematicAll();
			
				}
				
			}
			
// Pad2Ctrl1Rotate
			
			if (address == "/1/Pad2Ctrl1Rotate" || address == "/2/Pad2Ctrl1Rotate") {

				float value1 = (float) args[0];
				
				// here we have the option to use the trigger button to
				// rotate clockwise and anti-clockwise depending on where it is touch (top / bottom half)

				//bool isRotated = false;
				// cache blur settings 
				//
				// this blur is going to be global - not just the object that is rotating - as such it is an experiment
				Camera.main.gameObject.GetComponent<Blur>().enabled =  true;
				Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				Camera.main.gameObject.GetComponent<Blur>().blurSize =  3.0f;
				//float motionBlurAmount = Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount;
				// this ble
				
				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Rotate Pressed. Value="+value1);}
					
					
					if (value1 > .5){
						
						
						//object3.animation.Play();
						//object4.rigidbody.isKinematic=true;
						
						birdHelper.MakeKinematicTrueAll();
						
						if (isRotated == false){ 
							
							// if there is one object or a decent object by object blur exists
							// do the blur here! Don't do it like this as it interupts if any blur already exists and turns it off.
					
						
						
						
							object3.animation.Play("RotateClockwise", PlayMode.StopAll);
							
							isRotated = true;
						} else { 
							
						
						
							object3.animation.Play("RotateCounterClockwise", PlayMode.StopAll); 
							isRotated = false;
							
						}
						birdHelper.MakeKinematicFalseAll();
						
					} else {
				
						//birdHelper.ToggleIsKinematicAll();
						birdHelper.MakeKinematicTrueAll();
						if (isRotated == true){
						
							object3.animation.Play("RotateCounterClockwise", PlayMode.StopAll);
							isRotated = false;
						} else { 
						
							object3.animation.Play("RotateClockwise", PlayMode.StopAll); 
							isRotated = true;
						}
						birdHelper.MakeKinematicFalseAll();
						//birdHelper.ToggleIsKinematicAll();
						 
					} 
					
				}
			
				if (value1 == 0)  {
			
				if (debug == true) {Debug.Log ("Pad2Ctrl1Rotate Released. Value="+value1);}
					
					
				Camera.main.gameObject.GetComponent<Blur>().enabled =  blurState;
				Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
				Camera.main.gameObject.GetComponent<Blur>().blurSize =  blurSize;

					
				}
	
		}
			
			
		
			
// /1/Pad2Ctrl1Hang and /2/Pad2Ctrl1Hang
			
			if (address == "/1/Pad2Ctrl1Hang" || address == "/2/Pad2Ctrl1Hang") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Hang 1 or 2 Pressed. Value="+value1);}
					if (value1 > .5){
						if (debug == true) {Debug.Log("Hang Object");}
					} else {
						
						if (debug == true) {Debug.Log("Hang Object");}
					}
					
				}
			
				if (value1 == 0)  {
			
				if (debug == true) {Debug.Log ("/1/Pad2Ctrl1Hang or  /2/Pad2Ctrl1Hang Released. Value="+value1);}
					
					
				}
	
		}
			
// /1/Pad2Ctrl1Reset and /2/Pad2Ctrl1Reset 
			
			if (address == "/1/Pad2Ctrl1Reset" || address == "/2/Pad2Ctrl1Reset") {

				float value1 = (float) args[0];
				

				if (value1 == 1) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Reset 1 or 2 Pressed. Value="+value1);}
					//var clone : GameObject = Instantiate( myObj, transform.position, Quaternion.identity );
					//clone.name = "ThisExactNameHere";
					
					GameObject go = Instantiate(Resources.Load("IIM_Bird_001")) as GameObject; 
					go.name = "IIM_Bird_001";

					//birdHelper = (PuppetHelperIIM_Bird_001)FindObjectOfType(typeof(PuppetHelperIIM_Bird_001));
					birdInstanceCount = 1;
					birdHelper = GameObject.Find("IIM_Bird_001").GetComponent<PuppetHelperIIM_Bird_001>();
					birdHelper.doIExist("from RESET1");
				}
			
				if (value1 == 0)  {
			
				if (debug == true) {Debug.Log ("Pad2Ctrl1Reset 1 or 2 Released. Value="+value1);}
					
				}
	
		}
			
// /1/Pad2Ctrl1Scale and /2/Pad2Ctrl1Scale
			if (address == "/1/Pad2Ctrl1Scale" || address == "/2/Pad2Ctrl1Scale" ) {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Scale Active. Value="+value1);}
					
					if (object1 == null) { return; } else {
						birdHelper.SetScale(value1); }
					
//					Puppet1.transform.localScale += new Vector3(value1, value1, 1);
				}
			
				if (value1 == 0)  {
			
				if (debug == true) {Debug.Log ("Pad2Ctrl1Scale Released. Value="+value1);}
					//birdHelper.MakeKinematicFalseAll();
					
				}
	
		}


/*			

/1/Pad2Ctrl1Scale
/2/Pad2Ctrl1Scale
	

*/
			if (address == "/2/Pad2Ctrl2Scale") {
				
				float value1 = (float) args[0];
				
				
				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Scale Active. Value="+value1);}
					
					if (object5 == null) {
						return;
					} else {
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						dragonHelper.SetScale(value1); 
					}
					

				}
				
				if (value1 == 0)  {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl1Scale Released. Value="+value1);}
					//birdHelper.MakeKinematicFalseAll();
					
				}
				
			}
/*
/2/Pad2Ctrl2Scale
/2/Pad2Ctrl3Scale
/2/Pad2Ctrl4Scale
*/			

//-----------------PAGE 2 CONTROLLER 2----------------------------//

			
// ADDRESS Pad2PuppetController2 FirstTouch		// EXPERIMENT DATA FROM ACCELAROMTER	
			
			if (address == "/2/Pad2PuppetController2/1") {

				if (object5 != null) {
				bool Pad2PuppetController2TouchFlag = true;
				// This is a problem: how do we manipulate the rigid body character with these values?
				// Where do we rescale the coordinates to the view / screen or world?
				
				//construct a mouse touch / touch point
				// Input.Touch point is a vec2
				// Input.Mouse is a vec3
				// Then send to animationHandler
				
				// TOUCHOSC INVERTS THIS
				// TEST FOR THE LEMUR
				float x1 = (float) args[1] * multiplier;  
				float y1 = (float) args[0]  * multiplier;
				//float z1 = object5.transform.position.z;
				float z1 = 0.0f;
				
				// send first touch x y z to second and third touch scripts
				firstTouch = new Vector3(x1,y1,z1);
				


					// this is the place to adjust / map the touch values to screen space
					// -1 to 1 seems okay - but y is off centre


				//myVec3 = new Vector3(x1,y1, z1);
				
				//GameObject go = GameObject.Find("Head");
				
				// send values directly to the dragable script...
				
				// do a check to see if the target transform is really far from the point of first touch
				// ideally - tween the object to the touch position
				
				
				//object1.GetComponent<Dragable>().OnMoveObject(Pad2PuppetController1TouchFlag,x1,y1,z1);
				
				// send Touch Values to the animation handler...
				//birdHelper.ToggleIsKinematicAll();    // it is important to track the calls to isKinematic and useGravity
				//birdHelper.MakeKinematicFalseHead();

				//object5.rigidbody2D.isKinematic = true;

				//object6.rigidbody2D.isKinematic = true; // stops tail jumping when using the latest methods.

				
					if (y1 < -4.0f) { y1 = -3.8f;} // check floor
				
				if (debug == true) {Debug.Log("Is Rotated = "+isRotated);}
				

				if (isRotated == false) { 
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						// the y adjustment is hard offsetting the TOUCHOSC minicontroller so centre. 
						// The controller is set to -1 : 1
						// TODO could do calculation
						dragonHelper.MoveObjectDragonHead(Pad2PuppetController2TouchFlag, x1,y1+1.5f,z1); } 
					else {
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						// the y adjustment is hard offsetting the TOUCHOSC minicontroller so centre. 
						// TODO could do calculation
						dragonHelper.MoveObjectDragonHead(Pad2PuppetController2TouchFlag, x1*-1,y1+1.5f,z1);
					}
				//birdHelper.MakeKinematicFalseHeadBody();
				//birdHelper.ToggleIsKinematicAll();
				//birdHelper.MakeKinematicTrueHead();
				//object1.rigidbody.useGravity = true;
				// send the touch and coordinates to the dragable instance on the object we wish to control
				
			

				
				// Weird Flip - iPad in Landscape - Y comes in before X from TOUCH OSC. I've swopped the arguments
			
				
				
				if (debug == true) {Debug.Log("One Touch Detected: x ="+x1+" y ="+y1+" z ="+z1);}
				
				//object1.transform.position = new Vector3(x1,y1, transform.position.z);
				
				
				
				// try some scaling of the control values (-1 to 1) to the Unity Screen Size / World coords.
				
				//float finalX1 = x1 * (Screen.width) ;
				//float finalY1 = y1 * (Screen.height) ;
				
				
				
				//object1.rigidbody.isKinematic = true;
				//myVec3 = Camera.main.ScreenToWorldPoint(new Vector3(x1,y1, transform.position.z));
				//object1.transform.position = 	Camera.main.ScreenToWorldPoint(new Vector3(x1,y1, transform.position.z));
				//object1.transform.position = new Vector3(x1,y1, transform.position.z);
				//object1.rigidbody.isKinematic = false;
				if (debug == true) { Debug.Log("One Touch Detected: x ="+myVec3.x+" y ="+myVec3.y);}
				
				//print ("Width:"+ Screen.width+ "Height: "+ Screen.height);
				
			} else { //object1.rigidbody.isKinematic = false; 
				//bool Pad2PuppetController1TouchFlag = false;
				//object1.GetComponent<Dragable>().OnMoveObject(Pad2PuppetController1TouchFlag,0f,0f,0f);
				//object1.GetComponent<Dragable>().OnStopMoveObject();
				
				
				//object5.rigidbody.useGravity = true;
	

				}
				if (object5!=null){
					//object5.rigidbody2D.isKinematic = false;
					//object6.rigidbody2D.isKinematic = false;
				}

			} // object4 head and therefore bird doesn't exist


			// Checking Touch Status Pad 2
			// this is a feature of Touch OSC - can Lemur do the same thing
			/*if (address == "/2/Pad2PuppetController2/1/z") {
				
				float touchOneStatusPadOne = (float) args[0];
				if (touchOneStatusPadOne == 1) {
					//Debug.Log ("Pad 2 Touch 1 Started");
					if (object5 != null) { //object5.rigidbody2D.isKinematic = true;
					}
				}
				if (touchOneStatusPadOne == 0) {
					Debug.Log ("Pad 2 Touch  1 Ended");
					if (object5 != null) { //object5.rigidbody2D.isKinematic = false;
					}
				}
			}
			if (address == "/2/Pad2PuppetController2/2/z") {
					
					float touchTwoStatusPadTwo = (float) args[0];
					if (touchTwoStatusPadTwo == 1) {
						//Debug.Log ("Pad 2 Touch 2 Started");
						if (object6 != null) { //object6.rigidbody2D.isKinematic = true;
					}
					}
					if (touchTwoStatusPadTwo == 0) {
						//Debug.Log ("Pad 2 Touch  2 Ended");
						if (object6 != null) { //object6.rigidbody2D.isKinematic = false;
					}
					}


					//if (debug == true) { Debug.Log("Pad Two Touch Two Status: "+touchTwoStatusPadTwo);}
			}

*/

			// ADDRESS Pad2PuppetController2 SecondTouch




			if (address == "/2/Pad2PuppetController2/2") {

				if (object6 != null) {
				bool Pad2PuppetController2Touch2Flag = true;
			//print ("2 fingers: doing the multi xy thing");
			//print ("2x = "+args[0]);
			//print ("2y = "+args[1]);
			float x2 = (float) args[1] * multiplier;
			float y2 = (float) args[0] * multiplier;

			float z2 = 0.0f;
			//float z2 = object6.transform.position.z;
			
			//object2.rigidbody.isKinematic = true;	
			//object2.transform.position = new Vector3(x2,y2, transform.position.z);
				//object6.rigidbody2D.isKinematic = true;
				if (y2 < -4.0f) { y2 = -3.8f;}
				
					if (debug == true) {Debug.Log("/2/Pad2PuppetController2/2: X = "+x2+" Y = "+y2);}
				

					if (isRotated == false) { dragonHelper.MoveObjectDragonTail(Pad2PuppetController2Touch2Flag, x2,y2+1.5f,z2); }
				
				else {
					
						dragonHelper.MoveObjectDragonTail(Pad2PuppetController2Touch2Flag, x2*-1,y2+1.5f,z2);
				}
				
				
				
				} 
				
			} 
			
			
// Pad2Ctrl2Rotate  DRAGON ROTATE
			
			if (address == "/2/Pad2Ctrl2Rotate") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl2Rotate Pressed. Value="+value1);}
					if (value1 > .5){
						if (debug == true) {Debug.Log("Rotate ClockWise");}
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						dragonHelper.Rotate(true);




					} else {
						
						if (debug == true) {Debug.Log("Rotate Counter-ClockWise");}
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						dragonHelper.Rotate(false);

					}
					
				}
			
				if (value1 == 0)  {
			
				if (debug == true) {Debug.Log ("Pad2Ctrl2Rotate Released. Value="+value1);}
					
					
				}
	
		}
			
			
		
			
// /2/Pad2Ctrl2Hang
			
			if (address == "/2/Pad2Ctrl2Hang") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					Debug.Log ("/2/Pad2Ctrl2Hang Pressed. Value="+value1);
					if (value1 > .5){
						Debug.Log("Hang Object");
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						dragonHelper.ToggleHang();
					} else {
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));
						dragonHelper.ToggleHang();
						Debug.Log("Hang Object");
					}
					
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("/2/Pad2Ctrl2Hang Released. Value="+value1);
					
					
				}
	
		}
			
// /2/Pad2Ctrl2Reset
			
			if (address == "/2/Pad2Ctrl2Reset" ) {

				float value1 = (float) args[0];
				

				if (value1 == 1) {
					
					Debug.Log ("/2/Pad2Ctrl2Reset2 Pressed. Value="+value1);
					GameObject go = Instantiate(Resources.Load("Dragon_Greek")) as GameObject; 
					go.name = "Dragon_Greek";
		
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("Pad2Ctrl2Reset Released. Value="+value1);
					
				}
	
		}
			
			
			
//---------------------------------------------------------------------------//

//-----------------PAGE 2 CONTROLLER 3----------------------------//

			
// ADDRESS Pad2PuppetController3 FirstTouch			
			
			if (address == "/2/Pad2PuppetController3/1" || address == "/7/Pad2PuppetController3/1" ) {
			bool Pad2PuppetController3Touch1Flag = true;
				
			// construct a mouse touch / touch point
			// Input.Touch point is a vec2
			// Input.Mouse is a vec3
			// Then send to animationHandler
			// Weird Flip - iPad in Landscape - Y comes in before X from TOUCH OSC. I've swopped the arguments
			
			float x1 = (float) args[1]   * multiplier;  
			float y1 = (float) args[0]  * multiplier;
			float z1 = 0.0f;
				//birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
				//birdHelperUni2D.ToggleControllerObject1();
				
			// send Touch Values to the animation handler...
			
				if (y1 < -4.0f) { y1 = -3.8f;}
				
				if (debug == true) {Debug.Log("/2/Pad2PuppetController3/1: X = "+x1+" Y = "+y1);}
				
				
				if (isRotated == false) { 

					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DHead(Pad2PuppetController3Touch1Flag, x1,y1+1.5f,z1); }
				
				else {
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DHead(Pad2PuppetController3Touch1Flag, x1*-1,y1+1.5f,z1);
				}



		
			} 
			
// ADDRESS Pad2PuppetController3 SecondTouch
			
			if (address == "/2/Pad2PuppetController3/2"  || address == "/7/Pad2PuppetController3/2" ) {
				
			
				float x2 = (float) args[1] * multiplier;;
				float y2 = (float) args[0] * multiplier;;
				float z2 = 0.0f;
				bool Pad2PuppetController3Touch2Flag = true;



				// send Touch Values to the animation handler...
				
				if (y2 < -4.0f) { y2 = -3.8f;} // constrain something to the floor Might not be necessary

				if (debug == true) {Debug.Log("/2/Pad2PuppetController3/2: X = "+x2+" Y = "+y2);}
			
				
				if (isRotated == false) { 
					
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DBody(Pad2PuppetController3Touch2Flag, x2,y2+1.5f,z2); }
				
				else {
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DBody(Pad2PuppetController3Touch2Flag, x2*-1,y2+1.5f,z2);
				}


				
			} else {
			
				
				
			}
// ADDRESS Pad2PuppetController3 ThirdTouch
			
			if (address == "/2/Pad2PuppetController3/3"  || address == "/7/Pad2PuppetController3/3" ) {
				
				
				float x3 = (float) args[1] * multiplier;;
				float y3 = (float) args[0] * multiplier;;
				float z3 = 0.0f;
				bool Pad2PuppetController3Touch3Flag = true;
				//birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
				//birdHelperUni2D.setControllerObject3(true);
				
				// send Touch Values to the animation handler...
				
				if (y3 < -4.0f) { y3 = -3.8f;} // constrain something to the floor Might not be necessary
				
				if (debug == true) {Debug.Log("/2/Pad2PuppetController3/3: X = "+x3+" Y = "+y3);}
				
				
				if (isRotated == false) { 
					
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					//birdHelperUni2D.setControllerObject3(true);

					birdHelperUni2D.MoveObjectBirdUni2DBeak(Pad2PuppetController3Touch3Flag, x3,y3+1.5f,z3); }
				
				else {
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					//birdHelperUni2D.setControllerObject3(true);

					birdHelperUni2D.MoveObjectBirdUni2DBeak(Pad2PuppetController3Touch3Flag, x3*-1,y3+1.5f,z3);
				}
				
				
				
			} else {
				
				
				
			}	





// EXPERIMENT 2 iPad Setup using fixed OSC address from second iPad therefore a varied TouchOSC controller patch

			// ADDRESS Pad1PuppetController3 FirstTouch
			
			if (address == "/7/Pad1PuppetController3/1"   ) {
				
				// TODO in the multi ipad context be careful that these local variables don't clash
				float x1 = (float) args[1] * multiplier;;
				float y1 = (float) args[0] * multiplier;;
				float z1 = 0.0f;
				bool Pad1PuppetController3Touch1Flag = true;
				
				
				
				// send Touch Values to the animation handler...
				
				if (y1 < -4.0f) { y1 = -3.8f;} // constrain something to the floor Might not be necessary
				
				if (debug == true) {Debug.Log("/7/Pad1PuppetController3/1: X = "+x1+" Y = "+y1);}
				
				
				if (isRotated == false) { 
					
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DLeftKnee(Pad1PuppetController3Touch1Flag, x1,y1+1.5f,z1); }
				
				else {
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DLeftKnee(Pad1PuppetController3Touch1Flag, x1*-1,y1+1.5f,z1);
				}
				
				
				
			} 

			// ADDRESS Pad1PuppetController3 SecondTouch
			
			if (address == "/7/Pad1PuppetController3/2"  ) {
				
				// TODO in the multi ipad context be careful that these local variables don't clash
				float x2 = (float) args[1] * multiplier;;
				float y2 = (float) args[0] * multiplier;;
				float z2 = 0.0f;
				bool Pad1PuppetController3Touch2Flag = true;
				
				
				
				// send Touch Values to the animation handler...
				
				if (y2 < -4.0f) { y2 = -3.8f;} // constrain something to the floor Might not be necessary
				
				if (debug == true) {Debug.Log("/7/Pad1PuppetController3/2: X = "+x2+" Y = "+y2);}
				
				
				if (isRotated == false) { 
					
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DRightKnee(Pad1PuppetController3Touch2Flag, x2,y2+1.5f,z2); }
				
				else {
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.MoveObjectBirdUni2DRightKnee(Pad1PuppetController3Touch2Flag, x2*-1,y2+1.5f,z2);
				}
				
				
				
			} 


// End of second iPad experiment

			// Pad2Ctrl3Rotate
			
			if (address == "/7/Pad2Ctrl3Rotate" || address == "/2/Pad2Ctrl3Rotate") {
				
				float value1 = (float) args[0];
				
				
				if (value1 > 0) {
					
					Debug.Log ("Pad2Ctrl3Rotate Pressed. Value="+value1);
					if (value1 > .5){
						//Debug.Log("Rotate ClockWise");
						birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
						birdHelperUni2D.Rotate(true);


					} else {
						
						Debug.Log("Rotate Counter-ClockWise");
						birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
						birdHelperUni2D.Rotate(false);
					}
					
				}
				
				if (value1 == 0)  {
					
					Debug.Log ("Pad2Ctrl3Rotate Released. Value="+value1);
					
					
				}
				
			}




			if (address == "/7/Pad2Ctrl3Scale" || address == "/2/Pad2Ctrl3Scale") {
				// The UNI2D scale doesn't work - like the Unity 3D structures - it is using the same physics solver.

				float value1 = (float) args[0];
				
				
				if (value1 > 0) {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl3Scale Active. Value="+value1);}
					

					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.SetScale(value1); 

					
					
				}
				
				if (value1 == 0)  {
					
					if (debug == true) {Debug.Log ("Pad2Ctrl3Scale Released. Value="+value1);}

					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.SetDefaultScale(); 
					
				}
				
			}


			if (address == "/7/Pad2Ctrl3Reset" || address == "/2/Pad2Ctrl3Reset" ) {
				
				float value1 = (float) args[0];
				
				
				if (value1 == 1) {
					
					Debug.Log ("/7/Pad2Ctrl3Reset Pressed. Value="+value1);
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.Reset();

					
				}
			}

			if (address == "/7/Pad2Ctrl3Hang"||address == "/2/Pad2Ctrl3Hang" ) {
				
				float value1 = (float) args[0];
				
				
				if (value1 == 1) {
					
					Debug.Log ("/7/Pad2Ctrl3Hang Pressed. Value="+value1);
					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					birdHelperUni2D.ToggleHang();
					
					
				}
			}
// Pad2Ctrl3Rotate
			
			if (address == "/2/Pad2Ctrl3Rotate") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					Debug.Log ("Pad2Ctrl3Rotate Pressed. Value="+value1);
					if (value1 > .5){
						Debug.Log("Rotate ClockWise");
					} else {
						
						Debug.Log("Rotate Counter-ClockWise");
					}
					
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("Pad2Ctrl3Rotate Released. Value="+value1);
					
					
				}
	
		}
			
			
		
			
// /2/Pad2Ctrl3Hang
			
			if (address == "/2/Pad2Ctrl3Hang") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					Debug.Log ("/2/Pad2Ctrl3Hang Pressed. Value="+value1);
					if (value1 > .5){
						Debug.Log("Hang Object");
					} else {
						
						Debug.Log("Hang Object");
					}
					
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("/2/Pad2Ctrl3Hang Released. Value="+value1);
					
					
				}
	
		}
			
// /2/Pad2Ctrl3Reset
			
			if (address == "/2/Pad2Ctrl3Reset" ) {

				float value1 = (float) args[0];
				

				if (value1 == 1) {
					
					Debug.Log ("/2/Pad2Ctrl3Reset Pressed. Value="+value1);
		
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("Pad2Ctrl3Reset Released. Value="+value1);
					
				}
	
		}
			
			
			
//---------------------------------------------------------------------------//

			
//-----------------PAGE 2 CONTROLLER 4----------------------------//

			
// ADDRESS Pad2PuppetController4 FirstTouch			
			
			if (address == "/2/Pad2PuppetController4/1") {
			// bool Pad2PuppetController1TouchFlag = true;
				
			// construct a mouse touch / touch point
			// Input.Touch point is a vec2
			// Input.Mouse is a vec3
			// Then send to animationHandler
			// Weird Flip - iPad in Landscape - Y comes in before X from TOUCH OSC. I've swopped the arguments
			
			float x1 = (float) args[1]   * multiplier;  
			float y1 = (float) args[0]  * multiplier;




				float z1 = 0.0f;
				bool Pad1PuppetController4Touch1Flag = true;
				
				
				
				// send Touch Values to the animation handler...
				
				if (y1 < -4.0f) { y1 = -3.8f;} // constrain something to the floor Might not be necessary
				
				if (debug == true) {Debug.Log("/7/Pad1PuppetController3/2: X = "+x1+" Y = "+y1);}
				
				
				if (isRotated == false) { 

					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));
					karagozAsHorseHelper.MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Head(Pad1PuppetController4Touch1Flag, x1,y1+1.5f,z1); }
				
				else {
					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));

					karagozAsHorseHelper.MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Head(Pad1PuppetController4Touch1Flag, x1*-1,y1+1.5f,z1);
				}
			//float z1 = object1.transform.position.z;
  			//Debug.Log("/2/Pad2PuppetController4/1: X = "+x1+" Y = "+y1);
				
			// send Touch Values to the animation handler...
			// 	myAnimationHandler.MoveObject1(Pad2PuppetController1TouchFlag, x1,y1,z1);
			
			// send the touch and coordinates to the dragable instance on the object we wish to control
		
			} else { //object1.rigidbody.isKinematic = false; 
			}
			
// ADDRESS Pad2PuppetController4 SecondTouch
			
			if (address == "/2/Pad2PuppetController4/2") {
				
			//print ("2 fingers: doing the multi xy thing");
			//print ("2x = "+args[0]);
			//print ("2y = "+args[1]);
			float x2 = (float) args[1] * multiplier;;
			float y2 = (float) args[0] * multiplier;;
				float z2 = 0.0f;
				bool Pad1PuppetController4Touch2Flag = true;
				
				
				
				// send Touch Values to the animation handler...
				
				if (y2 < -4.0f) { y2 = -3.8f;} // constrain something to the floor Might not be necessary
				
				if (debug == true) {Debug.Log("/7/Pad1PuppetController3/2: X = "+x2+" Y = "+y2);}
				
				
				if (isRotated == false) { 
					
					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));
					karagozAsHorseHelper.MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Body(Pad1PuppetController4Touch2Flag, x2,y2+1.5f,z2);
				
				}
				
				else {
					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));
					karagozAsHorseHelper.MoveObject_PuppetHelperIIM_Karagoz_As_Horse_Body(Pad1PuppetController4Touch2Flag, x2*-1f,y2+1.5f,z2);
				}
			//object2.rigidbody.isKinematic = true;	
			//object2.transform.position = new Vector3(x2,y2, transform.position.z);
			
				Debug.Log("/2/Pad2PuppetController4/2: X = "+x2+" Y = "+y2);
				
				
			} else {
			
				
				
			}
			
			
// /2/Pad2Ctrl4Rotate
			
			if (address == "/2/Pad2Ctrl4Rotate") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					Debug.Log ("Pad2Ctrl4Rotate Pressed. Value="+value1);
					if (value1 > .5){
						Debug.Log("Rotate ClockWise");
					} else {
						
						Debug.Log("Rotate Counter-ClockWise");
					}
					
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("Pad2Ctrl4Rotate Released. Value="+value1);
					
					
				}
	
		}
			
			
		
			
// /2/Pad2Ctrl4Hang
			
			if (address == "/2/Pad2Ctrl4Hang") {

				float value1 = (float) args[0];
				

				if (value1 > 0) {
					
					Debug.Log ("/2/Pad2Ctrl4Hang Pressed. Value="+value1);
					if (value1 > .5){
						Debug.Log("Hang Object");

						musicianHelper = (PuppetHelperIIM_Musician)FindObjectOfType(typeof(PuppetHelperIIM_Musician));
						musicianHelper.ToggleHang();

					} else {
						
						musicianHelper = (PuppetHelperIIM_Musician)FindObjectOfType(typeof(PuppetHelperIIM_Musician));
						musicianHelper.ToggleHang();
					}
					
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("/2/Pad2Ctrl4Hang Released. Value="+value1);
					
					
				}
	
		}
			
// /2/Pad2Ctrl4Reset
			
			if (address == "/2/Pad2Ctrl4Reset" ) {

				float value1 = (float) args[0];
				

				if (value1 == 1) {
					
					Debug.Log ("/2/Pad2Ctrl4Reset Pressed. Value="+value1);
		
				}
			
				if (value1 == 0)  {
			
				Debug.Log ("Pad2Ctrl4Reset Released. Value="+value1);
					
				}
	
		}
			
			
			
//---------------------------------------------------------------------------//
// ******* CINEMATIC CONTROLLERS ****** //
			
	
// IRIS CENTROID CONTROLLERS -- this could be an XY pad except the touch and release is desirable interaction

// x1y1
			if (address == "/3/IrisInCentroid/1/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x1y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x1y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					//placeRefObjectAtScreenCoordinates(0,0);
					moveIrisPlane(0,0);
					
				}
			}
// x2y1
			if (address == "/3/IrisInCentroid/1/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					//placeRefObjectAtScreenCoordinates(1,0);
					moveIrisPlane(1,0);
				}
			}

// x3y1
			if (address == "/3/IrisInCentroid/1/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					//placeRefObjectAtScreenCoordinates(2,0);
					moveIrisPlane(2,0);
				}
			}

// x4y1
			if (address == "/3/IrisInCentroid/1/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,0);
				}
			}
		

// x5y1
			if (address == "/3/IrisInCentroid/1/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,0);
				}
			}
			
// x6y1
			if (address == "/3/IrisInCentroid/1/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,0);
				}
			}
			
// x7y1
			if (address == "/3/IrisInCentroid/1/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,0);
				}
			}		
			
// x8y1
			if (address == "/3/IrisInCentroid/1/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,0);
				}
			}
			
// x9y1
			if (address == "/3/IrisInCentroid/1/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,0);
				}
			}		

// x10y1
			if (address == "/3/IrisInCentroid/1/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,0);
				}
			}

// x11y1
			if (address == "/3/IrisInCentroid/1/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,0);
				}
			}
			
// x12y1
			if (address == "/3/IrisInCentroid/1/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,0);
				}
			}

// x13y1
			if (address == "/3/IrisInCentroid/1/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,0);
				}
			}
			
// x14y1
			if (address == "/3/IrisInCentroid/1/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,0);
				}
			}
			


//******************************************//
// x1y2
			if (address == "/3/IrisInCentroid/2/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,1);
					
				}
			}
// x2y2
			if (address == "/3/IrisInCentroid/2/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,1);
						
				}
			}

// x3y2
			if (address == "/3/IrisInCentroid/2/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,1);
				}
			}

// x4y2
			if (address == "/3/IrisInCentroid/2/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,1);
				}
			}
		

// x5y2
			if (address == "/3/IrisInCentroid/2/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,1);
				}
			}
			
// x6y2
			if (address == "/3/IrisInCentroid/2/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,1);
				}
			}
			
// x7y2
			if (address == "/3/IrisInCentroid/2/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,1);
				}
			}		
			
// x8y2
			if (address == "/3/IrisInCentroid/2/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,1);
				}
			}
			
// x9y2
			if (address == "/3/IrisInCentroid/2/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,1);
				}
			}		

// x10y2
			if (address == "/3/IrisInCentroid/2/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,1);
				}
			}

// x11y2
			if (address == "/3/IrisInCentroid/2/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,1);
				}
			}
			
// x12y2
			if (address == "/3/IrisInCentroid/2/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,1);
				}
			}

// x13y2
			if (address == "/3/IrisInCentroid/2/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,1);
				}
			}
			
// x14y2
			if (address == "/3/IrisInCentroid/2/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,1);
				}
			}	
		
//******************************************//
// x1y3
			if (address == "/3/IrisInCentroid/3/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,2);
					
				}
			}
// x2y3
			if (address == "/3/IrisInCentroid/3/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,2);
						
				}
			}

// x3y3
			if (address == "/3/IrisInCentroid/3/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,2);
				}
			}

// x4y3
			if (address == "/3/IrisInCentroid/3/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,2);
				}
			}
		

// x5y3
			if (address == "/3/IrisInCentroid/3/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,2);
				}
			}
			
// x6y3
			if (address == "/3/IrisInCentroid/3/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,2);
				}
			}
			
// x7y3
			if (address == "/3/IrisInCentroid/3/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,2);
				}
			}		
			
// x8y3
			if (address == "/3/IrisInCentroid/3/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,2);
				}
			}
			
// x9y3
			if (address == "/3/IrisInCentroid/3/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,2);
				}
			}		

// x10y3
			if (address == "/3/IrisInCentroid/3/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,2);
				}
			}

// x11y3
			if (address == "/3/IrisInCentroid/3/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,2);
				}
			}
			
// x12y3
			if (address == "/3/IrisInCentroid/3/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,2);
				}
			}

// x13y3
			if (address == "/3/IrisInCentroid/3/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,2);
				}
			}
			
// x14y3
			if (address == "/3/IrisInCentroid/3/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,2);
				}
			}
			
//******************************************//
// x1y4
			if (address == "/3/IrisInCentroid/4/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,3);
					
				}
			}
// x2y4
			if (address == "/3/IrisInCentroid/4/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,3);
						
				}
			}

// x3y4
			if (address == "/3/IrisInCentroid/4/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,3);
				}
			}

// x4y4
			if (address == "/3/IrisInCentroid/4/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,3);
				}
			}
		

// x5y4
			if (address == "/3/IrisInCentroid/4/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,3);
				}
			}
			
// x6y4
			if (address == "/3/IrisInCentroid/4/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,3);
				}
			}
			
// x7y4
			if (address == "/3/IrisInCentroid/4/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,3);
				}
			}		
			
// x8y4
			if (address == "/3/IrisInCentroid/4/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,3);
				}
			}
			
// x9y4
			if (address == "/3/IrisInCentroid/4/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,3);
				}
			}		

// x10y4
			if (address == "/3/IrisInCentroid/4/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,3);
				}
			}

// x11y4
			if (address == "/3/IrisInCentroid/4/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,3);
				}
			}
			
// x12y4
			if (address == "/3/IrisInCentroid/4/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,3);
				}
			}

// x13y4
			if (address == "/3/IrisInCentroid/4/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,3);
				}
			}
			
// x14y4
			if (address == "/3/IrisInCentroid/4/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,3);
				}
			}			
//******************************************//
// x1y5
			if (address == "/3/IrisInCentroid/5/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,4);
					
				}
			}
// x2y5
			if (address == "/3/IrisInCentroid/5/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,4);
						
				}
			}

// x3y5
			if (address == "/3/IrisInCentroid/5/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,4);
				}
			}

// x4y5
			if (address == "/3/IrisInCentroid/5/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,4);
				}
			}
		

// x5y5
			if (address == "/3/IrisInCentroid/5/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,4);
				}
			}
			
// x6y5
			if (address == "/3/IrisInCentroid/5/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,4);
				}
			}
			
// x7y5
			if (address == "/3/IrisInCentroid/5/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,4);
				}
			}		
			
// x8y5
			if (address == "/3/IrisInCentroid/5/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,4);
				}
			}
			
// x9y5
			if (address == "/3/IrisInCentroid/5/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,4);
				}
			}		

// x10y5
			if (address == "/3/IrisInCentroid/5/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,4);
				}
			}

// x11y5
			if (address == "/3/IrisInCentroid/5/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,4);
				}
			}
			
// x12y5
			if (address == "/3/IrisInCentroid/5/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,4);
				}
			}

// x13y5
			if (address == "/3/IrisInCentroid/5/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,4);
				}
			}
			
// x14y5
			if (address == "/3/IrisInCentroid/5/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,4);
				}
			}
			
//******************************************//
// x1y6
			if (address == "/3/IrisInCentroid/6/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,5);
					
				}
			}
// x2y6
			if (address == "/3/IrisInCentroid/6/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,5);
						
				}
			}

// x3y6
			if (address == "/3/IrisInCentroid/6/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,5);
				}
			}

// x4y6
			if (address == "/3/IrisInCentroid/6/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,5);
				}
			}
		

// x5y6
			if (address == "/3/IrisInCentroid/6/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,5);
				}
			}
			
// x6y6
			if (address == "/3/IrisInCentroid/6/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,5);
				}
			}
			
// x7y6
			if (address == "/3/IrisInCentroid/6/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,5);
				}
			}		
			
// x8y6
			if (address == "/3/IrisInCentroid/6/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,5);
				}
			}
			
// x9y6
			if (address == "/3/IrisInCentroid/6/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,5);
				}
			}		

// x10y6
			if (address == "/3/IrisInCentroid/6/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,5);
				}
			}

// x11y6
			if (address == "/3/IrisInCentroid/6/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,5);
				}
			}
			
// x12y6
			if (address == "/3/IrisInCentroid/6/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,5);
				}
			}

// x13y6
			if (address == "/3/IrisInCentroid/6/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,5);
				}
			}
			
// x14y6
			if (address == "/3/IrisInCentroid/6/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,5);
				}
			}
//******************************************//
// x1y7
			if (address == "/3/IrisInCentroid/7/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,6);
					
				}
			}
// x2y7
			if (address == "/3/IrisInCentroid/7/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,6);
						
				}
			}

// x3y7
			if (address == "/3/IrisInCentroid/7/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,6);
				}
			}

// x4y7
			if (address == "/3/IrisInCentroid/7/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,6);
				}
			}
		

// x5y7
			if (address == "/3/IrisInCentroid/7/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,6);
				}
			}
			
// x6y7
			if (address == "/3/IrisInCentroid/7/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,6);
				}
			}
			
// x7y7
			if (address == "/3/IrisInCentroid/7/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,6);
				}
			}		
			
// x8y7
			if (address == "/3/IrisInCentroid/7/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,6);
				}
			}
			
// x9y7
			if (address == "/3/IrisInCentroid/7/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,6);
				}
			}		

// x10y7
			if (address == "/3/IrisInCentroid/7/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,6);
				}
			}

// x11y7
			if (address == "/3/IrisInCentroid/7/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,6);
				}
			}
			
// x12y7
			if (address == "/3/IrisInCentroid/7/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,6);
				}
			}

// x13y7
			if (address == "/3/IrisInCentroid/7/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,6);
				}
			}
			
// x14y7
			if (address == "/3/IrisInCentroid/7/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,6);
				}
			}
			
//******************************************//
// x1y8
			if (address == "/3/IrisInCentroid/8/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,7);
					
				}
			}
// x2y8
			if (address == "/3/IrisInCentroid/8/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,7);
						
				}
			}

// x3y8
			if (address == "/3/IrisInCentroid/8/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,7);
				}
			}

// x4y8
			if (address == "/3/IrisInCentroid/8/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,7);
				}
			}
		

// x5y8
			if (address == "/3/IrisInCentroid/8/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,7);
				}
			}
			
// x6y8
			if (address == "/3/IrisInCentroid/8/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,7);
				}
			}
			
// x7y8
			if (address == "/3/IrisInCentroid/8/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,7);
				}
			}		
			
// x8y8
			if (address == "/3/IrisInCentroid/8/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,7);
				}
			}
			
// x9y8
			if (address == "/3/IrisInCentroid/8/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,7);
				}
			}		

// x10y8
			if (address == "/3/IrisInCentroid/8/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,7);
				}
			}

// x11y8
			if (address == "/3/IrisInCentroid/8/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					//Debug.Log ("/3/IrisInCentroid button (x11y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,7);
				}
			}
			
// x12y8
			if (address == "/3/IrisInCentroid/8/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,7);
				}
			}

// x13y8
			if (address == "/3/IrisInCentroid/8/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,7);
				}
			}
			
// x14y8
			if (address == "/3/IrisInCentroid/8/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,7);
				}
			}
			

//******************************************//
// x1y9
			if (address == "/3/IrisInCentroid/9/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,8);
					
				}
			}
// x2y9
			if (address == "/3/IrisInCentroid/9/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,8);
						
				}
			}

// x3y9
			if (address == "/3/IrisInCentroid/9/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,8);
				}
			}

// x4y9
			if (address == "/3/IrisInCentroid/9/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,8);
				}
			}
		

// x5y9
			if (address == "/3/IrisInCentroid/9/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,8);
				}
			}
			
// x6y9
			if (address == "/3/IrisInCentroid/9/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,8);
				}
			}
			
// x7y9
			if (address == "/3/IrisInCentroid/9/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,8);
				}
			}		
			
// x8y9
			if (address == "/3/IrisInCentroid/9/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,8);
				}
			}
			
// x9y9
			if (address == "/3/IrisInCentroid/9/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,8);
				}
			}		

// x10y9
			if (address == "/3/IrisInCentroid/9/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,8);
				}
			}

// x11y9
			if (address == "/3/IrisInCentroid/9/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,8);
				}
			}
			
// x12y9
			if (address == "/3/IrisInCentroid/9/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,8);
				}
			}

// x13y9
			if (address == "/3/IrisInCentroid/9/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x13y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x13y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,8);
				}
			}
			
// x14y9
			if (address == "/3/IrisInCentroid/9/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x14y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x14y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,8);
				}
			}
//******************************************//
// x1y10
			if (address == "/3/IrisInCentroid/10/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,9);
					
				}
			}
// x2y10
			if (address == "/3/IrisInCentroid/10/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x2y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x2y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,9);
						
				}
			}

// x3y10
			if (address == "/3/IrisInCentroid/10/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x3y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x3y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,9);
				}
			}

// x4y10
			if (address == "/3/IrisInCentroid/10/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x4y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x4y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,9);
				}
			}
		

// x5y10
			if (address == "/3/IrisInCentroid/10/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x5y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x5y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,9);
				}
			}
			
// x6y10
			if (address == "/3/IrisInCentroid/10/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x6y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x6y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,9);
				}
			}
			
// x7y10
			if (address == "/3/IrisInCentroid/10/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x7y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x7y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,9);
				}
			}		
			
// x8y10
			if (address == "/3/IrisInCentroid/10/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x8y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x8y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,9);
				}
			}
			
// x9y10
			if (address == "/3/IrisInCentroid/10/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x9y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x9y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,9);
				}
			}		

// x10y10
			if (address == "/3/IrisInCentroid/10/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x10y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x10y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,9);
				}
			}

// x11y10
			if (address == "/3/IrisInCentroid/10/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x11y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x11y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,9);
				}
			}
			
// x12y10
			if (address == "/3/IrisInCentroid/10/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisInCentroid button (x12y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisInCentroid button (x12y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,9);
				}
			}

// x13y10
			if (address == "/3/IrisInCentroid/10/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					//Debug.Log ("/3/IrisInCentroid button (x13y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					//Debug.Log ("/3/IrisInCentroid button (x13y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,9);
				}
			}
			
// x14y10
			if (address == "/3/IrisInCentroid/10/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					//Debug.Log ("/3/IrisInCentroid button (x14y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					//Debug.Log ("/3/IrisInCentroid button (x14y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,9);
				}
			}			

// *************************************************************************************//
			
// IRIS CENTROID CONTROLLER OUT



// x1y1
			if (address == "/3/IrisOutCentroid/1/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x1y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x1y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,0);
					
				}
			}
// x2y1
			if (address == "/3/IrisOutCentroid/1/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,0);
						
				}
			}

// x3y1
			if (address == "/3/IrisOutCentroid/1/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,0);
				}
			}

// x4y1
			if (address == "/3/IrisOutCentroid/1/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,0);
				}
			}
		

// x5y1
			if (address == "/3/IrisOutCentroid/1/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,0);
				}
			}
			
// x6y1
			if (address == "/3/IrisOutCentroid/1/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,0);
				}
			}
			
// x7y1
			if (address == "/3/IrisOutCentroid/1/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,0);
				}
			}		
			
// x8y1
			if (address == "/3/IrisOutCentroid/1/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,0);
				}
			}
			
// x9y1
			if (address == "/3/IrisOutCentroid/1/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,0);
				}
			}		

// x10y1
			if (address == "/3/IrisOutCentroid/1/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,0);
				}
			}

// x11y1
			if (address == "/3/IrisOutCentroid/1/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,0);
				}
			}
			
// x12y1
			if (address == "/3/IrisOutCentroid/1/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,0);
				}
			}

// x13y1
			if (address == "/3/IrisOutCentroid/1/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,0);
				}
			}
			
// x14y1
			if (address == "/3/IrisOutCentroid/1/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y1)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y1) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,0);
				}
			}
			


//******************************************//
// x1y2
			if (address == "/3/IrisOutCentroid/2/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,1);
					
				}
			}
// x2y2
			if (address == "/3/IrisOutCentroid/2/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,1);
						
				}
			}

// x3y2
			if (address == "/3/IrisOutCentroid/2/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,1);
				}
			}

// x4y2
			if (address == "/3/IrisOutCentroid/2/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,1);
				}
			}
		

// x5y2
			if (address == "/3/IrisOutCentroid/2/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,1);
				}
			}
			
// x6y2
			if (address == "/3/IrisOutCentroid/2/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,1);
				}
			}
			
// x7y2
			if (address == "/3/IrisOutCentroid/2/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,1);
				}
			}		
			
// x8y2
			if (address == "/3/IrisOutCentroid/2/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,1);
				}
			}
			
// x9y2
			if (address == "/3/IrisOutCentroid/2/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,1);
				}
			}		

// x10y2
			if (address == "/3/IrisOutCentroid/2/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,1);
				}
			}

// x11y2
			if (address == "/3/IrisOutCentroid/2/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,1);
				}
			}
			
// x12y2
			if (address == "/3/IrisOutCentroid/2/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,1);
				}
			}

// x13y2
			if (address == "/3/IrisOutCentroid/2/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,1);
				}
			}
			
// x14y2
			if (address == "/3/IrisOutCentroid/2/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y2)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y2) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,1);
				}
			}	
		
//******************************************//
// x1y3
			if (address == "/3/IrisOutCentroid/3/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,2);
					
				}
			}
// x2y3
			if (address == "/3/IrisOutCentroid/3/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,2);
						
				}
			}

// x3y3
			if (address == "/3/IrisOutCentroid/3/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,2);
				}
			}

// x4y3
			if (address == "/3/IrisOutCentroid/3/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,2);
				}
			}
		

// x5y3
			if (address == "/3/IrisOutCentroid/3/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,2);
				}
			}
			
// x6y3
			if (address == "/3/IrisOutCentroid/3/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,2);
				}
			}
			
// x7y3
			if (address == "/3/IrisOutCentroid/3/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,2);
				}
			}		
			
// x8y3
			if (address == "/3/IrisOutCentroid/3/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,2);
				}
			}
			
// x9y3
			if (address == "/3/IrisOutCentroid/3/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,2);
				}
			}		

// x10y3
			if (address == "/3/IrisOutCentroid/3/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,2);
				}
			}

// x11y3
			if (address == "/3/IrisOutCentroid/3/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,2);
				}
			}
			
// x12y3
			if (address == "/3/IrisOutCentroid/3/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,2);
				}
			}

// x13y3
			if (address == "/3/IrisOutCentroid/3/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,2);
				}
			}
			
// x14y3
			if (address == "/3/IrisOutCentroid/3/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y3)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y3) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,2);
				}
			}
			
//******************************************//
// x1y4
			if (address == "/3/IrisOutCentroid/4/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,3);
					
				}
			}
// x2y4
			if (address == "/3/IrisOutCentroid/4/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,3);
						
				}
			}

// x3y4
			if (address == "/3/IrisOutCentroid/4/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,3);
				}
			}

// x4y4
			if (address == "/3/IrisOutCentroid/4/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,3);
				}
			}
		

// x5y4
			if (address == "/3/IrisOutCentroid/4/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,3);
				}
			}
			
// x6y4
			if (address == "/3/IrisOutCentroid/4/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,3);
				}
			}
			
// x7y4
			if (address == "/3/IrisOutCentroid/4/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,3);
				}
			}		
			
// x8y4
			if (address == "/3/IrisOutCentroid/4/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,3);
				}
			}
			
// x9y4
			if (address == "/3/IrisOutCentroid/4/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,3);
				}
			}		

// x10y4
			if (address == "/3/IrisOutCentroid/4/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,3);
				}
			}

// x11y4
			if (address == "/3/IrisOutCentroid/4/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,3);
				}
			}
			
// x12y4
			if (address == "/3/IrisOutCentroid/4/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,3);
				}
			}

// x13y4
			if (address == "/3/IrisOutCentroid/4/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,3);
				}
			}
			
// x14y4
			if (address == "/3/IrisOutCentroid/4/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y4)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y4) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,3);
				}
			}			
//******************************************//
// x1y5
			if (address == "/3/IrisOutCentroid/5/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,4);
					
				}
			}
// x2y5
			if (address == "/3/IrisOutCentroid/5/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,4);
						
				}
			}

// x3y5
			if (address == "/3/IrisOutCentroid/5/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,4);
				}
			}

// x4y5
			if (address == "/3/IrisOutCentroid/5/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,4);
				}
			}
		

// x5y5
			if (address == "/3/IrisOutCentroid/5/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,4);
				}
			}
			
// x6y5
			if (address == "/3/IrisOutCentroid/5/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,4);
				}
			}
			
// x7y5
			if (address == "/3/IrisOutCentroid/5/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,4);
				}
			}		
			
// x8y5
			if (address == "/3/IrisOutCentroid/5/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,4);
				}
			}
			
// x9y5
			if (address == "/3/IrisOutCentroid/5/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,4);
				}
			}		

// x10y5
			if (address == "/3/IrisOutCentroid/5/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,4);
				}
			}

// x11y5
			if (address == "/3/IrisOutCentroid/5/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,4);
				}
			}
			
// x12y5
			if (address == "/3/IrisOutCentroid/5/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,4);
				}
			}

// x13y5
			if (address == "/3/IrisOutCentroid/5/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,4);
				}
			}
			
// x14y5
			if (address == "/3/IrisOutCentroid/5/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y5)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y5) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,4);
				}
			}
			
//******************************************//
// x1y6
			if (address == "/3/IrisOutCentroid/6/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,5);
					
				}
			}
// x2y6
			if (address == "/3/IrisOutCentroid/6/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,5);
						
				}
			}

// x3y6
			if (address == "/3/IrisOutCentroid/6/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,5);
				}
			}

// x4y6
			if (address == "/3/IrisOutCentroid/6/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,5);
				}
			}
		

// x5y6
			if (address == "/3/IrisOutCentroid/6/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,5);
				}
			}
			
// x6y6
			if (address == "/3/IrisOutCentroid/6/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,5);
				}
			}
			
// x7y6
			if (address == "/3/IrisOutCentroid/6/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,5);
				}
			}		
			
// x8y6
			if (address == "/3/IrisOutCentroid/6/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,5);
				}
			}
			
// x9y6
			if (address == "/3/IrisOutCentroid/6/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,5);
				}
			}		

// x10y6
			if (address == "/3/IrisOutCentroid/6/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,5);
				}
			}

// x11y6
			if (address == "/3/IrisOutCentroid/6/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,5);
				}
			}
			
// x12y6
			if (address == "/3/IrisOutCentroid/6/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,5);
				}
			}

// x13y6
			if (address == "/3/IrisOutCentroid/6/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,5);
				}
			}
			
// x14y6
			if (address == "/3/IrisOutCentroid/6/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y6)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y6) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,5);
				}
			}
//******************************************//
// x1y7
			if (address == "/3/IrisOutCentroid/7/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,6);
					
				}
			}
// x2y7
			if (address == "/3/IrisOutCentroid/7/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,6);
						
				}
			}

// x3y7
			if (address == "/3/IrisOutCentroid/7/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,6);
				}
			}

// x4y7
			if (address == "/3/IrisOutCentroid/7/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,6);
				}
			}
		

// x5y7
			if (address == "/3/IrisOutCentroid/7/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,6);
				}
			}
			
// x6y7
			if (address == "/3/IrisOutCentroid/7/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,6);
				}
			}
			
// x7y7
			if (address == "/3/IrisOutCentroid/7/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,6);
				}
			}		
			
// x8y7
			if (address == "/3/IrisOutCentroid/7/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,6);
				}
			}
			
// x9y7
			if (address == "/3/IrisOutCentroid/7/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,6);
				}
			}		

// x10y7
			if (address == "/3/IrisOutCentroid/7/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,6);
				}
			}

// x11y7
			if (address == "/3/IrisOutCentroid/7/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,6);
				}
			}
			
// x12y7
			if (address == "/3/IrisOutCentroid/7/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,6);
				}
			}

// x13y7
			if (address == "/3/IrisOutCentroid/7/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,6);
				}
			}
			
// x14y7
			if (address == "/3/IrisOutCentroid/7/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y7)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y7) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,6);
				}
			}
			
//******************************************//
// x1y8
			if (address == "/3/IrisOutCentroid/8/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,7);
					
				}
			}
// x2y8
			if (address == "/3/IrisOutCentroid/8/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,7);
						
				}
			}

// x3y8
			if (address == "/3/IrisOutCentroid/8/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,7);
				}
			}

// x4y8
			if (address == "/3/IrisOutCentroid/8/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,7);
				}
			}
		

// x5y8
			if (address == "/3/IrisOutCentroid/8/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,7);
				}
			}
			
// x6y8
			if (address == "/3/IrisOutCentroid/8/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,7);
				}
			}
			
// x7y8
			if (address == "/3/IrisOutCentroid/8/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,7);
				}
			}		
			
// x8y8
			if (address == "/3/IrisOutCentroid/8/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,7);
				}
			}
			
// x9y8
			if (address == "/3/IrisOutCentroid/8/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,7);
				}
			}		

// x10y8
			if (address == "/3/IrisOutCentroid/8/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,7);
				}
			}

// x11y8
			if (address == "/3/IrisOutCentroid/8/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,7);
				}
			}
			
// x12y8
			if (address == "/3/IrisOutCentroid/8/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,7);
				}
			}

// x13y8
			if (address == "/3/IrisOutCentroid/8/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,7);
				}
			}
			
// x14y8
			if (address == "/3/IrisOutCentroid/8/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y8)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y8) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,7);
				}
			}
			

//******************************************//
// x1y9
			if (address == "/3/IrisOutCentroid/9/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,8);
					
				}
			}
// x2y9
			if (address == "/3/IrisOutCentroid/9/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,8);
						
				}
			}

// x3y9
			if (address == "/3/IrisOutCentroid/9/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,8);
				}
			}

// x4y9
			if (address == "/3/IrisOutCentroid/9/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,8);
				}
			}
		

// x5y9
			if (address == "/3/IrisOutCentroid/9/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,8);
				}
			}
			
// x6y9
			if (address == "/3/IrisOutCentroid/9/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,8);
				}
			}
			
// x7y9
			if (address == "/3/IrisOutCentroid/9/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,8);
				}
			}		
			
// x8y9
			if (address == "/3/IrisOutCentroid/9/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,8);
				}
			}
			
// x9y9
			if (address == "/3/IrisOutCentroid/9/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,8);
				}
			}		

// x10y9
			if (address == "/3/IrisOutCentroid/9/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,8);
				}
			}

// x11y9
			if (address == "/3/IrisOutCentroid/9/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,8);
				}
			}
			
// x12y9
			if (address == "/3/IrisOutCentroid/9/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,8);
				}
			}

// x13y9
			if (address == "/3/IrisOutCentroid/9/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x13y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x13y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,8);
				}
			}
			
// x14y9
			if (address == "/3/IrisOutCentroid/9/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x14y9)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x14y9) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,8);
				}
			}
//******************************************//
// x1y10
			if (address == "/3/IrisOutCentroid/10/1" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(0,9);
					
				}
			}
// x2y10
			if (address == "/3/IrisOutCentroid/10/2" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x2y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x2y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(1,9);
						
				}
			}

// x3y10
			if (address == "/3/IrisOutCentroid/10/3" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x3y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x3y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(2,9);
				}
			}

// x4y10
			if (address == "/3/IrisOutCentroid/10/4" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x4y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x4y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(3,9);
				}
			}
		

// x5y10
			if (address == "/3/IrisOutCentroid/10/5" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x5y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x5y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(4,9);
				}
			}
			
// x6y10
			if (address == "/3/IrisOutCentroid/10/6" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x6y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x6y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(5,9);
				}
			}
			
// x7y10
			if (address == "/3/IrisOutCentroid/10/7" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x7y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x7y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(6,9);
				}
			}		
			
// x8y10
			if (address == "/3/IrisOutCentroid/10/8" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x8y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x8y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(7,9);
				}
			}
			
// x9y10
			if (address == "/3/IrisOutCentroid/10/9" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x9y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x9y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(8,9);
				}
			}		

// x10y10
			if (address == "/3/IrisOutCentroid/10/10" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x10y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x10y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(9,9);
				}
			}

// x11y10
			if (address == "/3/IrisOutCentroid/10/11" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x11y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x11y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(10,9);
				}
			}
			
// x12y10
			if (address == "/3/IrisOutCentroid/10/12" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					Debug.Log ("/3/IrisOutCentroid button (x12y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					Debug.Log ("/3/IrisOutCentroid button (x12y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(11,9);
				}
			}

// x13y10
			if (address == "/3/IrisOutCentroid/10/13" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					//Debug.Log ("/3/IrisOutCentroid button (x13y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					//Debug.Log ("/3/IrisOutCentroid button (x13y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(12,9);
				}
			}
			
// x14y10
			if (address == "/3/IrisOutCentroid/10/14" ) {

				float value1 = (float) args[0];
				if (value1 >=1) {
					//Debug.Log ("/3/IrisOutCentroid button (x14y10)  Pressed. Value="+value1);
				}
				if (value1 == 0)  {
					//Debug.Log ("/3/IrisOutCentroid button (x14y10) Released. Value="+value1);
					// trigger function here pass in coordinates
					moveIrisPlane(13,9);
				}
			}			

// END OF IRIS OUT ARRAYS*****************************************************************//			

// Here's an alternative Iris Position XY

			if (address == "/3/IrisLocationXYPad")

			{
				float irisX1 = (float) args[1] * multiplier;
				float irisY1 = (float) args[0] * multiplier;

				moveIrisPlaneFloat(irisX1,irisY1+2.0f);
			}

//  IrisTransitionSpeed SLIDER
			
			if (address == "/3/IrisTransitionSpeed") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0.0001) {
					//Debug.Log ("/3/IrisTransitionSpeed Value="+sliderValue);
					iris.SetActive(true);
					irisDiameter(sliderValue);
				}
				if (sliderValue <0.0001)  {
					//Debug.Log ("/3/IrisTransitionSpeed SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					iris.SetActive(true);
					irisDiameter(0.0f);

					// One could reset the iris plane here but the Control UI needs to update too

					myMessage = new OSCMessage("/3/IrisLocationXYPad/x", 0.0f);
					transmitter.Send(myMessage);
					myMessage = new OSCMessage("/3/IrisLocationXYPad/y", 0.0f);
					transmitter.Send(myMessage);
					moveIrisPlaneFloat(0.0f, 2.0f);

				}
				
			}
// FadeToggle*************************************************************************************//			
			

			if (address == "/3/FadeToggle" ) {

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FadeToggle TRUE");
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FadeToggle FALSE");
				}
			}		


//  FXSunShaftsIntensity
			
			if (address == "/3/FXSunShaftsIntensity") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/FXSunShaftsIntensity Value="+sliderValue);
					// TODO Set FXSunShaftsToggle to true with an OSC Message
					myMessage = new OSCMessage("/3/FXSunShaftsToggle", 1 ); // switch to the fourth tab in TouchOSC
					transmitter.Send(myMessage);
					
					Camera.main.gameObject.GetComponent<SunShafts>().enabled = true;
					Camera.main.gameObject.GetComponent<SunShafts>().sunShaftIntensity = sliderValue;
					
					// Set the Shafts Caster to a prominent part of the active puppet [hardwired here to the body of the bird]
					// You could set it to an animated object to simulate randomish shadows on the screen


					// TODO create a way to address a key component of the main moving object... and make the effect below
					//Camera.main.gameObject.GetComponent<SunShafts>().sunTransform = object4.transform;


					// it would be great to control the color of the shafts.
					//Color col = myColorPicker.PickColorFromCoords(colorPickerTex, 10,10);
					//Debug.Log (col);
					//	Camera.main.gameObject.GetComponent<SunShafts>().sunColor = col;
					
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXSunShaftsIntensity SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					Camera.main.gameObject.GetComponent<DepthOfField34>().enabled = false;
				}
				
			}

			
//  FXSunShaftsToggle *************************************************************************************//	
			
	
			
			if (address == "/3/FXSunShaftsToggle" ) {

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXSunShaftsToggle TRUE");
					Camera.main.gameObject.GetComponent<SunShafts>().enabled = true;
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXSunShaftsToggle FALSE");
					Camera.main.gameObject.GetComponent<SunShafts>().enabled = false;
				}
			}	
			
//  FXShaftColorPickerXY *************************************************************************************//	
				
			if (address == "/3/FXShaftColorPickerXY") {
			
				
				float xValue = (float) args[0];
				float yValue = (float) args[1];
				
				
				Debug.Log ("/3/FXShaftColorPickerXY XValue = "+xValue+ "Y Value = "+yValue);
				// TODO Set FXSunShaftsToggle to true with an OSC Message
				// TODO I've uncommented these to let one set the color without previewing that color - e.g. automatically turning on the effect.
				//myMessage = new OSCMessage("/3/FXSunShaftsToggle", 1 ); 
				//transmitter.Send(myMessage);
				//Camera.main.gameObject.GetComponent<SunShafts>().enabled = true;
				
					
				// Set the Shafts Caster to a prominent part of the active puppet [hardwired here to the body of the bird]
				// You could set it to an animated object to simulate randomish shadows on the screen

				// TODO create a way to address a key component of the main moving object... and make the effect below
				//Camera.main.gameObject.GetComponent<SunShafts>().sunTransform = object4.transform;
					
				// it would be great to control the color of the shafts. Here it is (!):
				// it would be great to included the image into the color picker in TouchOSC

				//TODO set a default for the color picker
				Color col = myColorPicker.PickColorFromCoords(colorPickerTex, xValue,yValue);
				Debug.Log (col);
				Camera.main.gameObject.GetComponent<SunShafts>().sunColor = col;
					
				
			}
/*
 * Vignetting - 
 *						defaults	range
 * 1. Amount=				 5.08		0-100
 * 2. Blurred Corners=		 1			0-12	
 * 3. Chromatic Aberration= 1.98		-80-80
 * 
 * 
*/
			
//  /3/FXVignetteSize
			
			if (address == "/3/FXVignetteSize") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/FXVignetteSize Value="+sliderValue);
					Camera.main.gameObject.GetComponent<Vignetting>().intensity = sliderValue;
					
					
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXVignetteSize SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					//Camera.main.gameObject.GetComponent<Vignetting>().enabled = false;
				}
				
			}
			
//  /3/FXVBlurredCorners
			
			if (address == "/3/FXVignetteBlur") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/FXVignetteBlur Value="+sliderValue);
					Camera.main.gameObject.GetComponent<Vignetting>().blur = sliderValue;
					
					
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXVignetteBlur SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					//Camera.main.gameObject.GetComponent<Vignetting>().enabled = false;
				}
				
			}			
//  /3/FXVignetteChromatic
			if (address == "/3/FXVignetteChromatic") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/FXVignetteChromatic Value="+sliderValue);
					Camera.main.gameObject.GetComponent<Vignetting>().chromaticAberration = sliderValue;
					
					
				}
				
				if (sliderValue <0) {
					Debug.Log ("/3/FXVignetteChromatic Value="+sliderValue);
					Camera.main.gameObject.GetComponent<Vignetting>().chromaticAberration = sliderValue;
					
					
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXVignetteChromatic SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					//Camera.main.gameObject.GetComponent<Vignetting>().enabled = false;
				}
				
			}
			
//  /3/FXVignetteSetDefaults *************************************************************************************//	
			
	
			
			if (address == "/3/FXVignetteSetDefaults" ) {

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXVignetteSetDefaults TRUE");
					Camera.main.gameObject.GetComponent<Vignetting>().intensity = 5.08f;
					Camera.main.gameObject.GetComponent<Vignetting>().blur = 1.0f;
					Camera.main.gameObject.GetComponent<Vignetting>().chromaticAberration = 1.98f;
					
					myMessage = new OSCMessage("/3/FXVignetteSize", 5.08f ); 
					transmitter.Send(myMessage);
					myMessage = new OSCMessage("/3/FXVignetteBlur", 1.0f ); 
					transmitter.Send(myMessage);
					myMessage = new OSCMessage("/3/FXVignetteChromatic", 1.98f ); 
					transmitter.Send(myMessage);
					
					
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXVignetteSetDefaults FALSE");
					Camera.main.gameObject.GetComponent<SunShafts>().enabled = false;
				}
			}	
			
//  FXInvertColors *************************************************************************************//	
			
	
			
			if (address == "/3/FXInvertColors" ) {

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXInvertColors TRUE");
					Camera.main.gameObject.GetComponent<InvertColors>().enabled = true;
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXInvertColors FALSE");
					Camera.main.gameObject.GetComponent<InvertColors>().enabled = false;
				}
			}
			

			
// DOFToggle*************************************************************************************//	
			
	
			
			if (address == "/3/DOFToggle" ) {

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/DOFToggle TRUE");
					Camera.main.gameObject.GetComponent<DepthOfField34>().enabled = true;
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/DOFToggle FALSE");
				}
			}		

// DOFFocalDistance*************************************************************************************//	
			
			if (address == "/3/DOFFocalDistance") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/DOFFocalDistance Value="+sliderValue);
					Camera.main.gameObject.GetComponent<DepthOfField34>().enabled = true;
					Camera.main.gameObject.GetComponent<DepthOfField34>().focalZDistance = sliderValue;
					// toggle the UI
					myMessage = new OSCMessage("/3/DOFToggle", 1 ); // switch to the fourth tab in TouchOSC
					transmitter.Send(myMessage);
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/DOFFocalDistance SLIDER OFF Value="+sliderValue); // use slider value = 0 as a toggle
					Camera.main.gameObject.GetComponent<DepthOfField34>().enabled = false;
					// toggle the UI
					myMessage = new OSCMessage("/3/DOFToggle", 0 ); // switch to the fourth tab in TouchOSC
					transmitter.Send(myMessage);
				}
				
			}
			

// CameraTypeToggle*************************************************************************************//	
			
	
			
			if (address == "/3/CameraTypeToggle/1/1" ) { // button 1/1 is the bottom one in the radio group (perspective)

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/CameraTypeToggle1/1/1 Perpective Camera TRUE");
					Debug.Log ("/3/CameraTypeToggle1/1/2 Ortho Camera FALSE");
					Camera.main.orthographic = false;
				}
				if (value1 == 0)  {
					Debug.Log ("/3/CameraTypeToggle/1/1 Perpective Camera FALSE");
					Debug.Log ("/3/CameraTypeToggle/1/2 Ortho Camera TRUE");
					Camera.main.orthographic = true;
				}
			}	
			

// CameraZoomOrtho*************************************************************************************//	
			
			if (address == "/3/CameraZoomOrtho") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/CameraZoomOrtho Value="+sliderValue);
					Camera.main.orthographicSize = sliderValue;
					
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/CameraZoomOrtho SLIDER OFF Value="+sliderValue); // you can use slider value = 0 as a toggle, if relevant.
					//Camera.main.orthographicSize = 2.61f; // a sensible default
				}
				
			}			

			
// CameraZoomPerspective*************************************************************************************//	
			
			if (address == "/3/CameraZoomPerspective") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					Debug.Log ("/3/CameraZoomPerspective Value="+sliderValue);
					Camera.main.fieldOfView = sliderValue;
				}
				if (sliderValue == 0)  {
					Debug.Log ("/3/CameraZoomPerspective SLIDER OFF Value="+sliderValue); // you can use slider value = 0 as a toggle, if relevant.
					//Camera.main.fieldOfView = 74f; // a sensible default
				}
				
			}	
// CamOrthoDefaultSize*************************************************************************************//		
			
			if (address == "/3/CamOrthoDefaultSize" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/CamOrthoDefaultSize Set=2.61");
					Camera.main.orthographicSize = 2.61f;
					// DONE call a function to transmit this value to the UI
					myMessage = new OSCMessage("/3/CameraZoomOrtho", 2.61f ); 
					transmitter.Send(myMessage);
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/CamOrthoDefaultSize RELEASED");
				}
			}	
			
			
// CamPerspectiveDefaultFOV*************************************************************************************//		
			
			if (address == "/3/CamPerspectiveDefaultFOV" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/CamPerspectiveDefaultFOV Set=74.0f");
					Camera.main.fieldOfView = 74.0f;
				}
				if (value1 == 0)  {
					Debug.Log ("/3/CamPerspectiveDefaultFOV RELEASED");
				}
			}	
			
			
	
// FXGreyScaleToggle*************************************************************************************//		
			
			if (address == "/3/FXGreyScaleToggle" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXGreyScaleToggle TRUE");
					Camera.main.gameObject.GetComponent<GrayscaleEffect>().enabled = true;
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXGreyScaleToggle FALSE");
					Camera.main.gameObject.GetComponent<GrayscaleEffect>().enabled = false;
				}
			}	


// FXMonochromeToggle*************************************************************************************//		
			
			// TODO this is temporary. Should swop a 'lighting state' with a very simple one (i.e. no lights).
			// Currently we disable the main and only light.
			if (address == "/3/FXMonochromeToggle" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXMonochromeToggle TRUE");
					monochromeToggle = true;
					mainLight.enabled = false;

					if (object5!=null) { 
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));

						if (dragonHelper) {
						dragonHelper.monochromeColorMode();}

					}

					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));
					
					if (karagozAsHorseHelper) {
						karagozAsHorseHelper.monochromeColorMode();}


					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					if (birdHelperUni2D) {birdHelperUni2D.monochromeColorMode();}



					// add other puppets to turn here... or create an array of GOs to search through...

				}


				if (value1 == 0)  {
					Debug.Log ("/3/FXMonochromeToggle FALSE");
					monochromeToggle = false;
					mainLight.enabled = true;
					if (object5!=null) { 
						dragonHelper = (PuppetHelperDragon_001)FindObjectOfType(typeof(PuppetHelperDragon_001));

						if (dragonHelper) {
						dragonHelper.normalColorMode();
						}
				}

					birdHelperUni2D = (PuppetHelperIIM_Bird_Uni2D)FindObjectOfType(typeof(PuppetHelperIIM_Bird_Uni2D));
					if (birdHelperUni2D) {birdHelperUni2D.normalColorMode();}

					karagozAsHorseHelper = (PuppetHelperIIM_Karagoz_As_Horse)FindObjectOfType(typeof(PuppetHelperIIM_Karagoz_As_Horse));

					if (karagozAsHorseHelper) {
						karagozAsHorseHelper.normalColorMode();}



				}
			}	
			
// FXNoiseToggle*************************************************************************************//		
			
			// TODO this simple switches between two states - experiment to see if any finer control would be helpful
			/*General Grain / Default Gentle Noise
			Intensity Multiplier = .4
			General  = .2
			Softness = 0 */
			/* Soft Big Movement (like underwater / caustic reflections) 
			Intensity Multiplier = .62
			General  = .97 
			Softness = .98 */
			
			if (address == "/3/FXNoiseToggle" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXNoiseToggle TRUE");
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().intensityMultiplier = .62f;
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().generalIntensity = 0.97f;
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().softness = 0.98f;
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXNoiseToggle FALSE");
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().intensityMultiplier = .4f;
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().generalIntensity = 0.2f;
					Camera.main.gameObject.GetComponent<NoiseAndGrain>().softness = 0.0f;
					
					
				}
			}			


// FXSepiaToggle*************************************************************************************//	
			if (address == "/3/FXSepiaToggle" ) { 

				float value1 = (float) args[0];
				if (value1 ==1) {
					Debug.Log ("/3/FXSepiaToggle TRUE");
					Camera.main.gameObject.GetComponent<SepiaToneEffect>().enabled = true;
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/FXSepiaToggle FALSE");
					Camera.main.gameObject.GetComponent<SepiaToneEffect>().enabled = false;
				}
			}	

			
// FXBlur*************************************************************************************//	
			
			/*
			 * Blur
			1. Blur Size - if it increases programmatically increase iteration count.
			Iteration Count  			>=
				1						2.95
				2						3.65
				3						5.5
				4						6
			 // these make sense in the inspector - but does not create smooth transitions between iteration counts. Also if the iterations are up the 0 state is still 
			 //	very blurred and makes a snappy transition.
			 */
			
			if (address == "/3/FXBlur") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					
					
						//Debug.Log ("/3/FXBlur Value="+sliderValue);
						Camera.main.gameObject.GetComponent<Blur>().enabled =  true;
						Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
						Camera.main.gameObject.GetComponent<Blur>().blurSize =  sliderValue;
					blurState=true;
					blurSize = sliderValue;
						
				}
				
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXBlur SLIDER OFF Value="+sliderValue); // you can use slider value = 0 as a toggle, if relevant.
					
					Camera.main.gameObject.GetComponent<Blur>().enabled =  false;
					Camera.main.gameObject.GetComponent<Blur>().blurIterations =  1;
					Camera.main.gameObject.GetComponent<Blur>().blurSize =  0.0f;
					blurState = false;
					blurSize = sliderValue;
					
					
				}
				
				
				
			}			

			
/* FXMotionBlur ******************* */
			
			if (address == "/3/FXMotionBlur") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					
					
						//Debug.Log ("/3/FXBlur Value="+sliderValue);
						Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  true;
						Camera.main.gameObject.GetComponent<MotionBlur>().blurAmount =  sliderValue;
					motionBlurState = true;
					motionBlurAmount = sliderValue;
					
						
				}
				
				if (sliderValue == 0)  {
					Debug.Log ("/3/FXMotionBlur SLIDER OFF Value="+sliderValue); // you can use slider value = 0 as a toggle, if relevant.
					Camera.main.gameObject.GetComponent<MotionBlur>().enabled =  false;
					motionBlurState = false;
					motionBlurAmount = sliderValue;
					
					
					
				}
				
				
				
			}	
			
			
			
// ManualFadeSlider*************************************************************************************//	
			// TODO the tone mapper is a useful fade - but the lighting conditions need to be matched to the output of the filter - pre-fade.
			// TODO Change with an alternative fading system
			if (address == "/3/ManualFadeSlider") {
			
				
				float sliderValue = (float) args[0];
				
				if (sliderValue >0) {
					//Debug.Log ("/3/ManualFadeSlider Value="+sliderValue);
					
					// Tonemapping isn't a good way to do this - we have burn and other rendering artefacts


					Color color = fadeFX.renderer.material.color;
					color.a = sliderValue;
					fadeFX.renderer.material.color = color;
					// Tonemapping isn't a good way to do this - we have burn and other rendering artefacts
					//Camera.main.gameObject.GetComponent<Tonemapping>().type = Tonemapping.TonemapperType.AdaptiveReinhardAutoWhite;
					//Camera.main.gameObject.GetComponent<Tonemapping>().enabled =  true; // not necessary if the filter is enabled by default.
					//Camera.main.gameObject.GetComponent<Tonemapping>().middleGrey = sliderValue;
					
					
					
					
				}
				if (sliderValue == 0)  {
					//Debug.Log ("/3/CameraZoomPerspective SLIDER OFF Value="+sliderValue); // you can use slider value = 0 as a toggle, if relevant.
					//Camera.main.fieldOfView = 74f; // a sensible default
					//Camera.main.gameObject.GetComponent<Tonemapping>().enabled =  true; // not necessary if the filter is enabled by default.
				}
				
			}	
	
// AutoFade*************************************************************************************//	
			if (address == "/3/AutoFade" ) { 

				float value1 = (float) args[0];
				if (value1 >0) {
					Debug.Log ("/3/AutoFade TRUE");
					//Camera.main.gameObject.GetComponent<SepiaToneEffect>().enabled = true;
					
				}
				if (value1 == 0)  {
					Debug.Log ("/3/AutoFade FALSE");
					//Camera.main.gameObject.GetComponent<SepiaToneEffect>().enabled = false;
				}
			}			







// New Move Controllers


			//-----------------PAGE 1 AND 2------CONTROLLER 1----------------------------//
			// ADDRESS Pad2PuppetControllerO4 FirstTouch			
			
			if (address == "/7/Pad2PuppetControllerO4/1") {
				
				bool Pad2PuppetControllerO4Touch1Flag = true;
				// This is a problem: how do we manipulate the rigid body character with these values?
				// Where do we rescale the coordinates to the view / screen or world?
				
				// Construct a mouse touch / touch point
				// Input.Touch point is a vec2
				// Input.Mouse is a vec3
				// Then send to animationHandler or to the object helper?
				
				// TOUCHOSC INVERTS THIS
				// TEST FOR THE LEMUR
				float x1 = (float) args[1]   * multiplier;  
				float y1 = (float) args[0]  * multiplier;
				float z1 = 0.0f; // z of head in prefab
				
					// send first touch x y z to second and third touch scripts
				//the first touch variable should be unique? Could clash with multiple controllers
				firstTouch = new Vector3(x1,y1,z1);
				
				//myVec3 = new Vector3(x1,y1, z1);
				
				
				if (ControllerMrsMartin1  == null) {
					
					//Debug.Log("From first touch area: Controller is not Defined, probably because it doesn't exist.");

					return;
				} else {
					// the object exists
					if (debug == true) {Debug.Log ("Mrs Martin Instance Exists");}

					mrsMartinHelper = GameObject.Find("BW_Mrs_Martin").GetComponent<PuppetHelperMrsMartin>();
					
					
					

					if (debug == true) {Debug.Log("Is Rotated = "+isRotated);}
					
					if (isRotated == false) { 
						
						myAnimationHandler.MoveObject1(Pad2PuppetControllerO4Touch1Flag, x1,y1,z1); 
					}
					
					else {
						
						myAnimationHandler.MoveObject1(Pad2PuppetControllerO4Touch1Flag, x1*-1,y1,z1);
					}

					
					
					if (debug == true) {Debug.Log("One Touch Detected: x ="+x1+" y ="+y1+" z ="+z1);}
					
			
					if (debug == true) { Debug.Log("One Touch Detected: x ="+myVec3.x+" y ="+myVec3.y);}
				}
	

			}

			
// Lemur Test*************************************************************************************//
			
			if (address == "/1/Pad2PuppetController1") {
				
				Debug.Log (address);
				Debug.Log ("Argument[0] ="+args[0]);
				Debug.Log ("Argument[1] ="+args[1]);
				Debug.Log ("Argument[2] ="+args[2]);
				Debug.Log ("Argument[3] ="+args[3]);
				Debug.Log ("Argument[4] ="+args[4]);
				//string param = (string) args[0];
				//float valueX1 = (float) args[0];
				//float valueX2 = (float) args[1];
				//float valueY1 = 0;
				//float valueY2 = 0;
				//Debug.Log ("Lemur Test: Ball 0 = x = "+valueX1);
				//Debug.Log ("Lemur Test: Ball 1 = x = "+valueX2);
			
			
				
				//float valueY1 = (float) args[0];
				//float valueY2 = (float) args[1];
				//Debug.Log ("Lemur Test: Ball 0 = y = "+valueY1);
				//Debug.Log ("Lemur Test: Ball 1 = y = "+valueY2);
			
				
			//LemurObject1.transform.localPosition = new Vector3 (valueX1, valueY1, 0);
			}
	// LEMUR KEYBOARD
			if (address=="/IpadAsciiKeyboard/IpadAlphaKeyboard/IpadShiftLeft/Pads/x") {
				float value1 = (float) args[0];
				if (value1 == 1f) {
					shiftCount += 1;
					Debug.Log (shiftCount); }



			}
	if (address == "/IpadAsciiKeyboard/IpadAlphaKeyboard/IpadAsciiKey111/Pads/x") {
				if (shiftCount % 2 == 0) {
					Debug.Log ("a"); } else {Debug.Log ("A");}	
				addToCaptionString(0, "a");
				
			

	}

				
			

	
	
			
			
		} // end of for loop over OSC listeners
		
	} // end of OSC listeners

	public void addToCaptionString(int textPreset, string letter) {

	// need an identifier for what string we are setting
	// maybe on the keyboard interface we store presets for strings
	// so if preset = 0 store in X
	// for now set up an array with 20 storage spaces


	//HINTS ON ARRAYS - ARRAY LISTS
	//	ArrayList myArrayList = new ArrayList();    // declaration
	//	myArrayList.Add(anItem);                    // add an item to the end of the array
	//	myArrayList[i] = newValue;                  // change the value stored at position i
	//	TheType thisItem = (TheType) myArray[i];    // retrieve an item from position i
	//	myArray.RemoveAt(i);                        // remove an item from position i
	//	var howBig = myArray.Count;                 // get the number of items in the ArrayList

	//	BUILT IN ARRAYS
	//	TheType myArray = new TheType[lengthOfArray];  // declaration
	//	int[] myNumbers = new int[10];                 // declaration example using ints
	//	GameObject[] enemies = new GameObject[16];       // declaration example using GameObjects
	//	int howBig = myArray.Length;               // get the length of the array
	//	myArray[i] = newValue;                     // set a value at position i
	//	TheType thisValue = myArray[i];            // get a value from position i
	//	System.Array.Resize(ref myArray, size);    //resize array
	//	
	//	string[] weapons = new string[] { "Sword", "Knife", "Gun" };
	//
	//	string caption0 = "Hello world";
	//	captionPresets.Add (caption0);

	}

	public void initCaptionList(){

		string[] captionPresets = new string[12];
	
		captionPresets[0] = ("This is caption one!");
		captionPresets[1] = ("Hello World!");
		captionPresets[2] = ("Hello World!");
		captionPresets[3] = ("Hello World!");
		captionPresets[4] = ("Hello World!");
		captionPresets[5] = ("Hello World!");
		captionPresets[6] = ("Hello World!");
		captionPresets[7] = ("Hello World!");
		captionPresets[8] = ("Hello World!");
		captionPresets[9] = ("Hello World!");
		captionPresets[10] = ("Hello World!");
		captionPresets[11] = ("Hello World!");
		int captionArraySize = captionPresets.Length;
		//Debug.Log("Caption list is "+captionArraySize+" item(s) long");
		//Debug.Log("Caption Item [0] is:"+captionPresets[0]);



	}

	public void irisDiameter(float irisDiameter){

		iris.renderer.sharedMaterial.SetFloat( "_Radius", irisDiameter );
	}

	public void moveIrisPlane(int xPos, int yPos){



		int myXDivision =  screenWidth / 13; // 13 is the x size of the multipush array
		int myYDivision = screenHeight / 10; // 10 is the y size of the multipush array
		int myYOffset = myYDivision / 2;
		int myXPos = (myXDivision*xPos); // centre of division
		int myYPos = (myYDivision*yPos)+myYOffset; // centre of division REMOVED / 2 
		//int myStartX = 0;
		// this defines the bottom left hand corner of the visible screen (in world space)

		Vector3 p = myCamera.ScreenToWorldPoint(new Vector3(myXPos, myYPos, -2.1f));

		iris.transform.position = new Vector3(p.x, p.y, -2.1f);
	}


	public void moveIrisPlaneFloat(float xPos, float yPos){

		iris.transform.position = new Vector3(xPos,yPos,-2.1f);
		
		
		
	}

	public void placeRefObjectAtScreenCoordinates(int xPositionMultiplier, int yPositionMultiplier) {
		int myXDivision =  screenWidth / 13; // 13 is the x size of the multipush array
		int myYDivision = screenHeight / 10; // 10 is the y size of the multipush array
		int myYOffset = myYDivision / 2;
		int myXPos = (myXDivision*xPositionMultiplier); // centre of division
		int myYPos = (myYDivision*yPositionMultiplier)+myYOffset; // centre of division REMOVED / 2 
		//int myStartX = 0;
		// this defines the bottom left hand corner of the visible screen (in world space)
		Vector3 p = myCamera.ScreenToWorldPoint(new Vector3(myXPos, myYPos, 0));
			


		// test the maths with some spheres - for fun make them rigidbodies!			
		GameObject sphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		sphere2.AddComponent<Rigidbody>();	
		sphere2.transform.position = new Vector3(p.x,p.y,0);
		//sphere2.transform.position = new Vector3(p.x+Random.Range(-0.1F, 0.1F),p.y,0);
		sphere2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		// Assigns a material named "Assets/Resources/DEV_Orange" to the object.
		Material newMat = Resources.Load("Red", typeof(Material)) as Material;
		sphere2.renderer.material = newMat;	
		
		//Debug.Log("Posiition ("+xPositionMultiplier+", " + yPositionMultiplier+") - centroid = ("+p.x+","+p.y+")");	
	}

	public void HelloWorld(string message) {

		Debug.Log("A message from: "+message+". Message Sent to UnityOSCListener");

	}


	// these setters allow for an object to be created and talked to by the control system. There is
	// a better way to allow dynamic control to be assigned. I think I need a big hash table mapping objects, controlInfo etc.
	// or a UI that assigns objects to controllers. Currently this is hardcoded

	// bird head
	public void SetObject1(GameObject go) {
		
		object1 = go;
		
	}

	// bird body
	public void SetObject2(GameObject go) {

		object2 = go;

	}

	// bird root
	public void SetObject3(GameObject go) {
		
		object3 = go;
		
	}
	//dragon head
	public void SetObject5(GameObject go) {
		
		object5 = go;
		
	}
	
	//dragon tail
	public void SetObject6(GameObject go) {
		
		object6 = go;
		
	}

	//dragon head controller
	public void SetObject5c(GameObject go) {
		
		object5c = go;
		
	}
	
	//dragon tail controller
	public void SetObject6c(GameObject go) {
		
		object6c = go;
		
	}

	//dragon root
	public void SetObject7(GameObject go) {
		
		object7 = go;
		
	}
	
	void Update() {
		if (Input.GetKeyUp("l")) {


			GameObject go = Instantiate(Resources.Load("IIM_Bird_001")) as GameObject; 
			go.name = "IIM_Bird_001";
			
			//birdHelper = (PuppetHelperIIM_Bird_001)FindObjectOfType(typeof(PuppetHelperIIM_Bird_001));
			birdInstanceCount = 1;
			birdHelper = GameObject.Find("IIM_Bird_001").GetComponent<PuppetHelperIIM_Bird_001>();
			birdHelper.doIExist("from RESET1");

		}

		 

	}
	void FixedUpdate()
			
		{
		 	//Debug.Log("We're in fixed update");
			
		

		}	
	
}

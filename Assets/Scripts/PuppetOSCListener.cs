using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OSC.NET;

public class PuppetOSCListener : MonoBehaviour {
	public float multiplier = 6; // TODO this should only exist in one place!
	Vector3 firstTouch;
	Vector3 secondTouch;
	Vector3 thirdTouch;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OSCMessageReceived(OSC.NET.OSCMessage message){	
		// do I need to "subscribe" to any messages? or make this instance know about the connection receiver	?
		Debug.Log("this is the new puppet specific controller! OSCMessageReceived. I hope this gets instantiated with the prefab.");
		string address = message.Address;
		ArrayList args = message.Values;

		foreach( var item in args){

// ADDRESS SLIDER1
		
				if (address == "/1/Pad2PuppetController1/1" || address == "/2/Pad2PuppetController1/1") {
					
				Debug.Log("this is the new puppet specific controller!. I hope this gets instantiated with the prefab.");
				bool Pad2PuppetController1TouchFlag = true;
				// This is a problem: how do we manipulate the rigid body character with these values?
				// Where do we rescale the coordinates to the view / screen or world?
				
				//construct a mouse touch / touch point
				// Input.Touch point is a vec2
				// Input.Mouse is a vec3
				// Then send to animationHandler
				
				// TOUCHOSC INVERTS THIS
				// TEST FOR THE LEMUR
				float x1 = (float) args[1]   * multiplier;  
				float y1 = (float) args[0]  * multiplier;
				float z1 = 0.05f; // z of head in prefab
				
				// send first touch x y z to second and third touch scripts
				firstTouch = new Vector3(x1,y1,z1);
					


			}
		}

	}
}

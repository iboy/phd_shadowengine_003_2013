using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using OSC.NET;


// Each iPad may need a unique TRANSMITTER class.

public class UnityOSCTransmitter : MonoBehaviour {
	
	private bool connected = false;
	public int port = 9000; //TouchOSC default listening port
	public string SendToIPAddress = "127.0.0.1"; 
	private OSCTransmitter transmitter;
	
	private OSCMessage myMessage;
	private OSCPacket myPacket;
	//private NetworkPresets myNetworkPresets;
	
	
	// Use this for initialization
	
	void Start () {
		// get IP values from single setting source
		GameObject myNetworkPresets = GameObject.Find("__NetworkPresets");
		SendToIPAddress = myNetworkPresets.GetComponent<NetworkPresets>().SendToIPAddress;
		port = myNetworkPresets.GetComponent<NetworkPresets>().Port;

		Debug.Log("IP Address: "+SendToIPAddress+" Outgoing OSC Port: "+port);

		transmitter = new OSCTransmitter(SendToIPAddress, port);
		//myPacket.

			// Attempt to do everything in one line.
			transmitter.Connect();
			connected = true;
			// Do something with bullet

			// Something went wrong, so lets get information about it.
		//	Debug.Log(e.ToString()+"hello from the exception in UnityOSCTransmitter.cs");
			// Do something knowing the bullet isn't on screen.



		
		// let's set some toggles to default values
		// Start Camera.main is ORTHO 
		Camera.main.orthographic = true;
		Camera.main.orthographicSize = 2.61f;
		myMessage = new OSCMessage("/3/CameraTypeToggle/2/1", 1 ); // switch to the third tab in TouchOSC
		transmitter.Send(myMessage);
		Debug.Log("Camera set to orthographic. Scale = 2.61");

		// just testing sending messages - this should go elsewhere or in some preset manager.
		//myMessage = new OSCMessage("/3"); // switch to the third tab in TouchOSC
		//transmitter.Send(myMessage);
		// just testing sending messages - this should go elsewhere or in some preset manager.
		//myMessage = new OSCMessage("/3/FadeToggle", 1);
		//transmitter.Send(myMessage);
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnApplicationQuit(){
		disconnect();
		
	}
	
	
	public void disconnect() {
      	if (transmitter!=null){
      		 transmitter.Close();
      	}
      	
       	transmitter = null;
		connected = false;
	}
	
	public bool isConnected() { return connected; }
	
}

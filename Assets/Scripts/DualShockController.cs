using UnityEngine;
using System.Collections;

public class DualShockController : MonoBehaviour {
	public float speed = 10.0F;
	public float rotationSpeed = 100.0F;
	bool ButtonXDown;
	bool ButtonXRelease;
	bool ButtonODown;
	bool ButtonORelease;
	bool ButtonTriangleDown;
	bool ButtonTriangleRelease;
	bool ButtonSquareDown;
	bool ButtonSquareRelease;
	bool ButtonSelectDown;
	bool ButtonSelectRelease;
	bool ButtonStartDown;
	bool ButtonStartRelease;
	bool ButtonL2Release;
	bool ButtonL2Down;
	float leftTranslation;
	float leftRotation;
	float rightTranslation;
	float rightRotation;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float leftAnalogueButton = Input.GetAxis("L2");
		if (Mathf.Abs(leftAnalogueButton)>0.01f) { // this corrects noise from the non-moving stick
			//leftTranslation = moveYLeft;
			Debug.Log("L2 Button Pressed = "+leftAnalogueButton);

		}

		if (Input.GetButtonDown("L2")){
			//float L2Value = Input.GetAxis("L2");
			ButtonL2Release = false;
			ButtonL2Down = true;
			Debug.Log("L2 Button Pressed = "+ButtonL2Down);
		} 
		
		if (Input.GetButtonUp("L2")){
			//float L2Value = Input.GetAxis("L2");
			ButtonL2Release = true;
			ButtonL2Down = false;
			Debug.Log("L2 Button Released = "+ButtonL2Release);
		} 

		// Select button joystick id 0
		if (Input.GetButtonDown("Select")){
			ButtonSelectRelease = false;
			ButtonSelectDown = true;
			Debug.Log("Select Button Pressed = "+ButtonSelectDown);
		} 

		if (Input.GetButtonUp("Select")){
			ButtonSelectRelease = true;
			ButtonSelectDown = false;
			Debug.Log("Select Button Released = "+ButtonSelectRelease);
		} 

		// Start button joystick id 3
		if (Input.GetButtonDown("Start")){
			ButtonStartRelease = false;
			ButtonStartDown = true;
			Debug.Log("Start Button Pressed = "+ButtonStartDown);
		} 
		
		if (Input.GetButtonUp("Start")){
			ButtonStartRelease = true;
			ButtonStartDown = false;
			Debug.Log("Start Button Released = "+ButtonStartRelease);
		} 

		// triangle button joystick id 12
		if (Input.GetButtonDown("Button Triangle")){
			ButtonTriangleRelease = false;
			ButtonTriangleDown = true;
			Debug.Log("Jump (Button Triangle) Pressed = "+ButtonTriangleDown);
		} 
		
		if (Input.GetButtonUp("Button Triangle")){
			ButtonTriangleRelease = true;
			ButtonTriangleDown = false;
			Debug.Log("Jump (Button Triangle) Released = "+ButtonTriangleRelease);
		} 
		

		// O button joystick id 13
		if (Input.GetButtonDown("Button O")){
			ButtonORelease = false;
			ButtonODown = true;
			Debug.Log("Jump (Button O) Pressed = "+ButtonODown);
		} 
		
		if (Input.GetButtonUp("Button O")){
			ButtonORelease = true;
			ButtonODown = false;
			Debug.Log("Jump (Button O) Released = "+ButtonORelease);
		} 


		// X button joystick id 14
		if (Input.GetButtonDown("Button X")){
			ButtonXRelease = false;
			ButtonXDown = true;
			Debug.Log("Jump (Button X) Pressed = "+ButtonXDown);
		} 
		
		if (Input.GetButtonUp("Button X")){
			ButtonXRelease = true;
			ButtonXDown = false;
			Debug.Log("Jump (Button X) Released = "+ButtonXRelease);
		} 

		// square button joystick id 15
		if (Input.GetButtonDown("Button Square")){
			ButtonSquareRelease = false;
			ButtonSquareDown = true;
			Debug.Log("Jump (Button Square) Pressed = "+ButtonSquareDown);
		} 
		
		if (Input.GetButtonUp("Button Square")){
			ButtonSquareRelease = true;
			ButtonSquareDown = false;
			Debug.Log("Jump (Button Square) Released = "+ButtonSquareRelease);
		} 



		// left joystick movement
		float moveYLeft = Input.GetAxis("Move Y");
		if (Mathf.Abs(moveYLeft)>0.01f) { // this corrects noise from the non-moving stick
			leftTranslation = moveYLeft;
			Debug.Log("Translation = "+leftTranslation);
		}

		float moveXLeft = Input.GetAxis("Move X");
		if (Mathf.Abs(moveXLeft)>0.01f) { // this corrects noise from the non-moving stick
			leftRotation = moveXLeft;
			Debug.Log("Rotation = "+leftRotation);
		}

		// right joystick movement
		float moveYRight = Input.GetAxis("Move Y Right");
		if (Mathf.Abs(moveYRight)>0.01f) { // this corrects noise from the non-moving stick
			rightTranslation = moveYRight;
			Debug.Log("Translation = "+rightTranslation);
		}
		
		float moveXRight = Input.GetAxis("Move X Right");
		if (Mathf.Abs(moveXRight)>0.01f) { // this corrects noise from the non-moving stick
			rightRotation = moveXRight;
			Debug.Log("Rotation = "+rightRotation);
		}


	
	}
}

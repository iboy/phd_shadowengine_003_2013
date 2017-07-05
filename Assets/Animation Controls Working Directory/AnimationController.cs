using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	
	
	public GameObject myObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		
		if(Input.GetKey(KeyCode.W))
		{
			Debug.Log("W Pressed");
			myObject.rigidbody.AddForce(Vector3.up * 60);
		}
		
		
		if(Input.GetKey(KeyCode.RightArrow))
		{
			Debug.Log("Right Arrow Pressed");
			myObject.rigidbody.AddForce(-1,0,0 * 60);
		}
		
	}
}

	using UnityEngine;
using System.Collections;

public class AddTorque3DToHorse : MonoBehaviour {

	public float torqueAmount = 100000f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		//Debug.Log("In Fixed Update");
		if (Input.GetKeyDown("a")) {
		//float h = torqueAmount * Time.deltaTime;
		//float v = Input.GetAxis("Vertical") * torqueAmount * Time.deltaTime;
			Debug.Log("A is pressed");
		
			transform.Rotate(0, 0, -3.0f);

			//rigidbody.AddTorque(transform.up * h);
		//rigidbody2D.AddTorque(transform.right * v);
		}

		if (Input.GetKeyDown("s")) {
			//float h = -torqueAmount * Time.deltaTime;
			//float v = Input.GetAxis("Vertical") * torqueAmount * Time.deltaTime;
			Debug.Log("s is pressed");
			//rigidbody.AddTorque(transform.up * h);
			//rigidbody2D.AddTorque(transform.right * v);
		}

	}
}

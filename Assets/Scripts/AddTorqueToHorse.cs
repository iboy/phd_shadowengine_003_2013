	using UnityEngine;
using System.Collections;

public class AddTorqueToHorse : MonoBehaviour {

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
			Debug.Log("a is pressed");

			transform.Rotate(Vector3.up,Time.deltaTime, Space.World);
			//transform.Rotate(,)
			//transform.Rotate((1,0,0), 30f);
			//rigidbody2D.AddTorque(h);
		//rigidbody2D.AddTorque(transform.right * v);
		}

		if (Input.GetKeyDown("s")) {
			float h = -torqueAmount * Time.deltaTime;
			//float v = Input.GetAxis("Vertical") * torqueAmount * Time.deltaTime;
			Debug.Log("s is pressed");
			rigidbody2D.AddTorque(h);
			//rigidbody2D.AddTorque(transform.right * v);
		}

	}
}

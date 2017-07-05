using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	
	public GameObject object1; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void Ctrl1Object1(float x1, float y1) {
		
	//Debug.Log("Hello from Ctrl1Object1!");
	object1.transform.position = new Vector3(x1,y1, transform.position.z);
		
	}
}

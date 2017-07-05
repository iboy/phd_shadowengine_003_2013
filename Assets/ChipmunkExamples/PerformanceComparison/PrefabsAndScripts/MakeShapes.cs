// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections.Generic;

// Simple test script that just creates a bunch of objects, tracks and moves them.
// Designed to keep objects moving, preventing them from sleeping, and offering a realistic
// number of changing contact points for a very active simulation.
public class MakeShapes : MonoBehaviour {
	
	public int maxShapes = 100;
	private List<ChipmunkBody> bodies = new List<ChipmunkBody>();
	private List<Transform> transforms = new List<Transform>();
	public float threshold = -20f;
	
	public GameObject prefab;
	public BoxCollider spawnHere;
	
	private Vector2 RandomPos(){
		Vector3 min = collider.bounds.min;
		Vector3 max = collider.bounds.max;
		return new Vector2(Mathf.Lerp(min.x, max.x, Random.value), Mathf.Lerp(min.y, max.y, Random.value));
	}
	
	private void FixedUpdate(){
		foreach(ChipmunkBody body in bodies){
			Vector2 p = body.position;
			if(p.y < threshold) body.position = RandomPos();
		}
		foreach(Transform physX in transforms){
			if(physX.position.y < threshold) physX.position = RandomPos();
		}
	}
	
	private void AddBox(){
		var go = Instantiate(prefab, RandomPos(), Quaternion.identity) as GameObject;
		
		ChipmunkBody body = go.GetComponent<ChipmunkBody>();
		if(body != null){
			bodies.Add(body);
		}else{
			transforms.Add (go.transform);	
		}
	}
	
	private void OnGUI(){
		this.useGUILayout = false;
		
		// There are many ways of measuring the framerate. We chose this method rather than counting the number of frames in a second
		// because it gives you a good idea of individual "frame length", and will accurately reflect "stuttering" or situations
		// where you get some long frames and some short frames. 
				
		GUI.Label(new Rect(Screen.width - 200, Screen.height - 60, 200, 20), string.Format("Framerate (1/dt): {0,0:0.000}", (1f/Time.deltaTime) ) );
		GUI.Label(new Rect(Screen.width - 200, Screen.height - 40, 200, 20), "Object Count: " + bodies.Count);
		
		if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 80, 200, 20), "Add 10")){
			for(int i=0; i<10; i++) AddBox();
		}
	}
		
}

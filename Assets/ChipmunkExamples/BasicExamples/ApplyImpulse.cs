// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class ApplyImpulse : MonoBehaviour {
	
	public Vector2 impulse = Vector2.up;
	
	public void Start(){
		
		GetComponent<ChipmunkBody>().ApplyImpulse(impulse, Vector2.zero);
	}
}

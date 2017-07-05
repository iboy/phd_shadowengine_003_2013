// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class DestroyOnDelay : MonoBehaviour {
	void Start(){
		// get rid of particle systems after a fixed time.
		Destroy (this.gameObject, 2.0f);	
	}
}
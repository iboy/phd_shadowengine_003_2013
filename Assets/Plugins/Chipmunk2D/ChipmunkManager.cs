// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

/// Internal Chipmunk class used to implement the update loop.
public class ChipmunkManager : MonoBehaviour {
	public ChipmunkSpace _space;
	
	protected void OnDestroy(){
		Chipmunk._manager = null;
	}
	
	protected void FixedUpdate(){
		_space._Step(Time.fixedDeltaTime);	
		
		foreach(ChipmunkBody b in _space.bodies){
			b.transform.position = (Vector3) b.position + (Vector3.forward * b._savedZ);
			b.transform.rotation = Quaternion.AngleAxis(b.angle*Mathf.Rad2Deg, Vector3.forward);
		}
	}
}

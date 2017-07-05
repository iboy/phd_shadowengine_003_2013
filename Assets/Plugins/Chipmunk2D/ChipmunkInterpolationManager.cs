// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// Internal Chipmunk class used to implement rigid body interpolation and extrapolation.
public class ChipmunkInterpolationManager : MonoBehaviour {
	protected List<ChipmunkBody> bodies = new List<ChipmunkBody>();
	
	public static void _Register(ChipmunkBody body){
		if(Chipmunk._interpolationManager){
			Chipmunk._interpolationManager.RegisterHelper(body);
		}
	}
	
	protected void RegisterHelper(ChipmunkBody body){
		if(!body.enabled || body._interpolationMode == ChipmunkBodyInterpolationMode.None){
			// Remove disabled or non-interpolating bodies
			bodies.Remove(body);
			this.enabled = (bodies.Count > 0);
		} else if(!bodies.Contains(body)){
			// Add enabled, interpolating ones.
			bodies.Add(body);
			this.enabled = true;
		}
	}
	
	protected void OnDestroy(){
		Chipmunk._interpolationManager = null;
	}
	
	protected void Update(){
		float dt = Time.time - Time.fixedTime;
		
		foreach(ChipmunkBody b in bodies){
			var mode = b._interpolationMode;
			
			if(mode == ChipmunkBodyInterpolationMode.Extrapolate){
				b.transform.position = (Vector3)(b.position + b.velocity*dt) + (Vector3.forward * b._savedZ);
				b.transform.rotation = Quaternion.AngleAxis((b.angle + b.angularVelocity*dt)*Mathf.Rad2Deg, Vector3.forward);
			}
		}
	}
}

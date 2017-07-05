// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class CrushDetector : MonoBehaviour {
	public float crushForceThreshold = 0f;
	public bool includeFrictionForces = false;
	
	protected ChipmunkBody body;
	protected void Start(){
		body = GetComponent<ChipmunkBody>();
	}
	
	protected void FixedUpdate(){
		float magnitudeSum = 0f;
		Vector2 vectorSum = Vector2.zero;
		
		body.EachArbiter(delegate(ChipmunkArbiter arbiter){
			Vector2 j = (includeFrictionForces ? arbiter.impulseWithFriction : arbiter.impulse);
			magnitudeSum += j.magnitude;
			vectorSum += j;
		});
		
		float crushForce = (magnitudeSum - vectorSum.magnitude)*Time.fixedDeltaTime;
		if(crushForce > crushForceThreshold){
			SendMessage("OnCrush", crushForce, SendMessageOptions.DontRequireReceiver);
		}
	}
}

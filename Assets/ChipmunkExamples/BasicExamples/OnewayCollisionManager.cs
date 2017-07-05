// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class OnewayCollisionManager : ChipmunkCollisionManager {

	protected void Start(){
		// Turning down the timestep can smooth things out significantly.
		// Chipmunk is also pretty fast so you don't need to worry about the performance so much.
		// Not really necessary, but helps in several subtle ways.
		Time.fixedDeltaTime = 1f/180f;
	}
	
	protected bool ChipmunkPreSolve_ball_oneway(ChipmunkArbiter arbiter)
	{
		if(arbiter.GetNormal(0).y > -0.7f){
			arbiter.Ignore();
			return false;
		}
		
		return true;
	}
	
}

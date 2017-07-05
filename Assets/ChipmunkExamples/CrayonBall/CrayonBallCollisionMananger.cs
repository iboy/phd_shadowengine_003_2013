// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class CrayonBallCollisionMananger : ChipmunkCollisionManager {
	private void MarkCollision(ChipmunkArbiter arbiter){
		ChipmunkShape a, b;
		arbiter.GetShapes(out a, out b);
		a.GetComponent<Ball>().parent = b.GetComponent<Ball>();
		
		Debug.DrawLine(a.transform.position, b.transform.position, Color.white);
	}
	
	public bool ChipmunkPreSolve_red_red(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
	
	protected bool ChipmunkPreSolve_green_green(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
	
	protected bool ChipmunkPreSolve_blue_blue(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
	
	protected bool ChipmunkPreSolve_yellow_yellow(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
	
	protected bool ChipmunkPreSolve_magenta_magenta(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
	
	protected bool ChipmunkPreSolve_cyan_cyan(ChipmunkArbiter arbiter){
		MarkCollision(arbiter);
		return true;
	}
}

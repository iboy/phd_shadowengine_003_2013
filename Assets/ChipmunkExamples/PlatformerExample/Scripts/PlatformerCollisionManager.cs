// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class PlatformerCollisionManager : ChipmunkCollisionManager {
	protected void Start(){
		// Turning down the timestep can smooth things out significantly.
		// Chipmunk is also pretty fast so you don't need to worry about the performance so much.
		// Not really necessary, but helps in several subtle ways.
		Time.fixedDeltaTime = 1f/180f;
		Chipmunk.gravity = new Vector2(0f, -100f);
	}
	
	protected bool ChipmunkPreSolve_player_oneway(ChipmunkArbiter arbiter)
	{
		// If we're pressing the down arrow key.
		if (Input.GetAxis("Vertical") < -0.5f) {
			// Fall through the floor
			arbiter.Ignore();
			return false;
		}


		if(arbiter.GetNormal(0).y > -0.7f){
			arbiter.Ignore();
			return false;
		}
		
		return true;
	}
	
	protected bool ChipmunkBegin_player_bumpable(ChipmunkArbiter arbiter){
		if(arbiter.GetNormal(0).y > 0.9f){
			ChipmunkShape player, bonusBlock;
			arbiter.GetShapes(out player, out bonusBlock);
			
			bonusBlock.SendMessage("Bump");
		}
		
		return true;
	}
	
	protected bool ChipmunkBegin_player_pit(ChipmunkArbiter arbiter){
		ChipmunkShape player, pit;
		arbiter.GetShapes(out player, out pit);
		
		player.SendMessage("OnFellInPit");
		
		return true;
	}
	
	protected bool ChipmunkBegin_player_jumpshroom(ChipmunkArbiter arbiter){
		ChipmunkShape player, shroom;
		arbiter.GetShapes(out player, out shroom);
		
		if(arbiter.GetNormal(0).y < -0.9f){
			ChipmunkBody body = player.GetComponent<ChipmunkBody>();
			body.velocity = new Vector2(body.velocity.x, 50f);			
			return false;
		} else {
			return true;
		}
	}
	
	// -----------
	
	float FLUID_DENSITY = 1f;
	float FLUID_DRAG = 3f;
	
	protected float Cross(Vector2 v1, Vector2 v2){
		return v1.x*v2.y - v1.y*v2.x;
	}
	
	protected float AreaForPoly(int count, Vector2[] verts){
		float area = 0.0f;
		for(int i=0; i<count; i++){
			area += Cross(verts[i], verts[(i+1)%count]);
		}
		
		return -area/2.0f;
	}
	
	protected Vector2 CentroidForPoly(int count, Vector2[] verts){
		float sum = 0f;
		Vector2 vsum = Vector2.zero;
		
		for(int i=0; i<count; i++){
			Vector2 v1 = verts[i];
			Vector2 v2 = verts[(i+1)%count];
			float cross = Cross(v1, v2);
			
			sum += cross;
			vsum = vsum + (v1 + v2)*cross;
		}
		
		return vsum/(3f*sum);
	}
	
	protected float KScalarBody(ChipmunkBody body, Vector2 r, Vector2 n){
		float rcn = Cross(r, n);
		return 1f/body.mass + rcn*rcn/body.moment;
	}
	
	protected Vector2 NormalizeSafe(Vector2 v){
		return v/(v.magnitude + float.MinValue);
	}
	
	protected float MomentForPoly(float m, int numVerts, Vector2[] verts, Vector2 offset){
		float sum1 = 0.0f;
		float sum2 = 0.0f;
		for(int i=0; i<numVerts; i++){
			Vector2 v1 = verts[i] +offset;
			Vector2 v2 = verts[(i+1)%numVerts] + offset;
			
			float a = Cross(v2, v1);
			float b = Vector2.Dot(v1, v1) + Vector2.Dot(v1, v2) + Vector2.Dot(v2, v2);
			
			sum1 += a*b;
			sum2 += a;
		}
		
		return (m*sum1)/(6.0f*sum2);
	}
	
	protected void ApplyImpulse(ChipmunkBody body, Vector2 j, Vector2 r){
		body.velocity += j/body.mass;
		body.angularVelocity += Cross(r, j)/body.moment;
	}
	
	protected bool ChipmunkPreSolve_water_crate(ChipmunkArbiter arbiter){
		ChipmunkShape water, poly;
		arbiter.GetShapes(out water, out poly);
		ChipmunkBody body = poly.body;
		
		// Sanity check
		if(water._handle == IntPtr.Zero || poly._handle == IntPtr.Zero){
			Debug.LogError("Invalid shape references. This is likely be a Chipmunk2D bug.");
			return false;
		}
		
		// Get the top of the water sensor bounding box to use as the water level.
		// Chipmunk bounding boxes aren't exposed by ChipmunkShape yet.
		// They are rarely useful, though this makes a pretty good case for it.
		float level = ChipmunkBinding._cpShapeGetBB(water._handle).t;
		
		// Clip the polygon against the water level
		int count = ChipmunkBinding.cpPolyShapeGetNumVerts(poly._handle);
		int clippedCount = 0;
		Vector2[] clipped = new Vector2[count + 1];
	
		for(int i=0, j=count-1; i<count; j=i, i++){
			Vector2 a = ChipmunkBinding._cpBodyLocal2World(body._handle, ChipmunkBinding.cpPolyShapeGetVert(poly._handle, j));
			Vector2 b = ChipmunkBinding._cpBodyLocal2World(body._handle, ChipmunkBinding.cpPolyShapeGetVert(poly._handle, i));
			
			if(a.y < level){
				clipped[clippedCount] = a;
				clippedCount++;
			}
			
			float a_level = a.y - level;
			float b_level = b.y - level;
			
			if(a_level*b_level < 0.0f){
				float t = Mathf.Abs(a_level)/(Mathf.Abs(a_level) + Mathf.Abs(b_level));
				
				clipped[clippedCount] = Vector2.Lerp(a, b, t);
				clippedCount++;
			}
		}
		
		// Calculate buoyancy from the clipped polygon area
		float clippedArea = AreaForPoly(clippedCount, clipped);
		float displacedMass = clippedArea*FLUID_DENSITY;
		Vector2 centroid = CentroidForPoly(clippedCount, clipped);
		Vector2 r = centroid - body.position;
		
		for(int i=0, j=clippedCount-1; i<clippedCount; j=i, i++){
			Vector2 a = clipped[i];
			Vector2 b = clipped[j];
			Debug.DrawLine(a, b, Color.green);
		}
//		ChipmunkDebugDrawPolygon(clippedCount, clipped, RGBAColor(0, 0, 1, 1), RGBAColor(0, 0, 1, 0.1f));
//		ChipmunkDebugDrawPoints(5, 1, &centroid, RGBAColor(0, 0, 1, 1));
		
		float dt = Time.fixedDeltaTime;
		Vector2 g = Chipmunk.gravity;
		
		// Apply the buoyancy force as an impulse.
		ApplyImpulse(body, g*(-displacedMass*dt), r);
		
		// Apply linear damping for the fluid drag.
		Vector2 v_centroid = body.velocity + (new Vector2(-r.y, r.x))*body.angularVelocity;
		float k = KScalarBody(body, r, NormalizeSafe(v_centroid));
		float damping = clippedArea*FLUID_DRAG*FLUID_DENSITY;
		float v_coef = Mathf.Exp(-damping*dt*k); // linear drag
	//	float v_coef = 1.0/(1.0 + damping*dt*cpvlength(v_centroid)*k); // quadratic drag
		ApplyImpulse(body, (v_centroid*v_coef - v_centroid)/k, r);
		
		// Apply angular damping for the fluid drag.
		float w_damping = MomentForPoly(FLUID_DRAG*FLUID_DENSITY*clippedArea, clippedCount, clipped, -body.position);
		body.angularVelocity *= Mathf.Exp(-w_damping*dt/body.moment);
		
		return false;
	}
}

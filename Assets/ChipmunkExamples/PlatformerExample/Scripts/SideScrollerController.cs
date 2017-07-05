// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class SideScrollerController : MonoBehaviour {
	protected ChipmunkBody body;
	protected ChipmunkShape shape;
	
	public VirtualControls virtualControls;
	
	protected void Start(){
		body = GetComponent<ChipmunkBody>();
		shape = GetComponent<ChipmunkShape>();
		
		if(body == null){
			Debug.LogError("Your SideScrollerController is not configured properly! Add a body!");	
		}
	}
	
	// Player is touching a surface that is pointed upwards.
	protected bool _grounded = false;
	public bool grounded {
		get { return (_grounded || remainingJumpLeniency > 0f); }
	}
	
	// How long the player has not been grounded.
	public float airTime { get; protected set; }
	
	// Send an "OnLand" message if landing from a jump longer than this.
	public float landThreshold = 0.25f;
	
	// Player is grounded on a surface they won't slide down.
	public bool wellGrounded { get; protected set; }
	
	protected Vector2 groundNormal;
	protected float groundPenetration;
	protected ChipmunkBody groundBody;
	protected ChipmunkShape groundShape;
	protected Vector2 groundImpulse;
	protected Vector2 recentGroundVelocity = Vector2.zero;
	
	public Vector2 groundPenetrationOffset {
		get { return groundNormal*(groundPenetration); }
	}
	
	public Vector2 groundVelocity {
		get { return (grounded ? body.velocity - recentGroundVelocity : Vector2.zero); }
	}
	
	protected void UpdateGrounding(){
		bool wasGrounded = _grounded;
		
		// Reset the grounding values to defaults
		_grounded = false;
		groundNormal = new Vector2(0f, -1f);
		groundPenetration = 0f;
		groundBody = null;
		groundShape = null;
		groundImpulse = Vector2.zero;
		
		// Find the best (most upright) normal of something you are standing on.
		body.EachArbiter(delegate(ChipmunkArbiter arbiter){
			ChipmunkShape player, ground; arbiter.GetShapes(out player, out ground);
			Vector2 n = -arbiter.GetNormal(0);
			
			// Magic thershold here to detect if you hit your head or not.
			if(n.y < -0.7f){
				// Bumped your head, disable further jumping.
				remainingAirJumps = 0;
				remainingBoost = 0f;
			} else if(n.y > groundNormal.y){
				_grounded = true;
				groundNormal = n;
				groundPenetration = -arbiter.GetDepth(0);
				groundBody = ground.body;
				groundShape = ground;
			}
			
			groundImpulse += arbiter.impulseWithFriction;
		});
		
		// If the player just landed from a significant jump, send a message.
		if(_grounded && !wasGrounded && airTime > landThreshold){
			SendMessage("OnLand");
		}
		
		// Increment airTime if the player is not grounded.
		airTime = (!_grounded ? airTime + Time.deltaTime : 0f);
		
		// To be well grounded, the slope you are standing on needs to less than the amount of friction
		float friction = _grounded ? shape.friction * groundShape.friction : 0f;
		wellGrounded = grounded && Mathf.Abs(groundNormal.x/groundNormal.y) < friction;
		if(wellGrounded){
			recentGroundVelocity = (groundBody != null ? groundBody.velocity : Vector2.zero);
			remainingAirJumps = maxAirJumps;
			remainingJumpLeniency = jumpLeniency;
		}
	}
	
	public float walkSpeed = 8f;
	public float runSpeed = 12;
	public float walkAccelTime = 0.05f;
	public float airAccelTime = 0.25f;
	public float fallSpeed = 5f;
	
	protected float walkAccel {
		get { return walkSpeed/walkAccelTime; }
	}
	
	protected float airAccel {
		get { return walkSpeed/airAccelTime; }
	}
	
	public int maxAirJumps = 1;
	public float jumpLeniency = 0.05f;
	public float jumpHeight = 1.0f;
	public float jumpBoostHeight = 2.0f;
	
	protected float remainingJumpLeniency = 0f;
	protected int remainingAirJumps = 0;
	protected bool lastJumpKeyState = false;
	protected float remainingBoost = 0f;
	
	public string jumpButton = "Jump";
	protected bool jumpInput {
		get {
			// Check both the virtual controls and the input buttons.
			return (
				(virtualControls && virtualControls.jump)
				|| Input.GetButton(jumpButton)
			);
		}
	}
	
	public string runButton = "Fire1";
	protected bool runInput {
		get { return Input.GetButton(runButton); }
	}
	
	public string directionAxis = "Horizontal";
	protected float directionInput {
		get {
			// Check both the virtual controls and the input buttons.
			return (virtualControls ? virtualControls.direction : 0f) + Input.GetAxis(directionAxis);
		}
	}
	
	protected void FixedUpdate(){
		float dt = Time.fixedDeltaTime;
		Vector2 v = body.velocity;
		Vector2 f = Vector2.zero;
		
		UpdateGrounding();
		
		// Target horizontal velocity used by air/ground control
		float target_vx = (runInput ? runSpeed : walkSpeed)*directionInput;
		
		// Update the surface velocity and friction
		Vector2 surface_v = new Vector2(target_vx, 0f);
		shape.surfaceVelocity = surface_v;
		if(_grounded){
			shape.friction = -walkAccel/Chipmunk.gravity.y;
		} else {
			shape.friction = 0f;
		}
		
		// Apply air control if not grounded
		if(!_grounded){
			float max = airAccel;
			float accel_x = (target_vx + recentGroundVelocity.x - v.x)/dt;
			f.x = Mathf.Clamp(accel_x, -max, max)*body.mass;
//			v.x += Mathf.MoveTowards(v.x, target_vx + recentGroundVelocity.x, playerAirAccel*dt);
		}
		
		// If the jump key was just pressed this frame, jump!
		bool jumpState = jumpInput;
		bool jump = (jumpState && !lastJumpKeyState);
		//Input.GetButton(jumpButton)
		if(jump && (wellGrounded || remainingAirJumps > 0 || remainingJumpLeniency > 0f)){
			float jump_v = Mathf.Sqrt(-2f*jumpHeight*Chipmunk.gravity.y);
			remainingBoost = jumpBoostHeight/jump_v;
			
			// Apply the jump to the velocity.
			v.y = recentGroundVelocity.y + jump_v;
			
			
			// Check if it was an air jump.
			if(!wellGrounded && (remainingJumpLeniency <= 0f)) {
				remainingAirJumps--;
				
				// Force the jump direction for air jumps.
				// Otherwise difficult to air jump to a block directly over you.
				v.x = directionInput*walkSpeed;
				SendMessage("OnAirJump");
			}else{
				SendMessage("OnJump");
			}
		} else if(!jumpState){
			remainingBoost = 0f;
		}
		
		// Apply jump boosting.
		if(jumpState && remainingBoost > 0f){
			f -= body.mass*(Vector2)Chipmunk.gravity;
		}
				
		remainingJumpLeniency -= dt;
		remainingBoost -= dt;
		lastJumpKeyState = jumpState;
		
		// Clamp off the falling velocity.
		v.y = Mathf.Clamp(v.y, -fallSpeed, Mathf.Infinity);
		
		body.velocity = v;
		body.force = f;
	}
}

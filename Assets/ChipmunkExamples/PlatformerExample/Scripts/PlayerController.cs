// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	protected Vector2 startPos;
	
	public Texture2D[] walkFrames;
	public Texture2D jumpFrame;
	public float animationCoef = 1f;
	public float animationThreshold = 0.1f;
	
	protected float walkFrame = 0;
	protected ChipmunkBody body;
	protected SideScrollerController controller;
	
	public GameObject sprite;
	protected Vector3 spriteScale;
	
	public AudioClip deathSound;
	public AudioClip jumpSound;
	public AudioClip landSound;
	
	public GameObject JumpEffectPrefab;
	public Transform JumpEffectLocation;
	
	protected void Start(){
		body = GetComponent<ChipmunkBody>();
		controller = GetComponent<SideScrollerController>();
		
		if(body == null){
			Debug.LogError("Your PlayerController is not configured properly! Add a body!");	
		}

		
		startPos = body.position;
		spriteScale = sprite.transform.localScale;
	}
	
	protected void Update(){
		if(controller.grounded){
			float ground_velocity = Mathf.Abs(controller.groundVelocity.x);
			if(ground_velocity < animationThreshold){
				walkFrame = 0f;
			} else {
				walkFrame += animationCoef*ground_velocity;
			}
			
			int i = (int)walkFrame;
			sprite.renderer.material.mainTexture = walkFrames[i%walkFrames.Length];
		} else {
			sprite.renderer.material.mainTexture = jumpFrame;
		}
		
		float velocity = body.velocity.x;
		if(Mathf.Abs(velocity) > animationThreshold){
			Vector3 scale = spriteScale;
			scale.x *= Mathf.Sign(velocity);
			sprite.transform.localScale = scale;
		}
	}
	
	public bool fixRenderOverlap = true;
	protected void LateUpdate(){
		// Add the groundPenetrationOffset to the position to force the graphics to not overlap.
		if(fixRenderOverlap) transform.position += (Vector3)controller.groundPenetrationOffset;
	}
	
	public void Reset(){
		body.position = transform.position = startPos;
		body.velocity = Vector2.zero;
	}
	
	protected void OnFellInPit(){
		Debug.Log("Fell in pit");
		Reset();
		
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
	}
	
	protected void OnCrush(float crushForce){
		Debug.Log("Crush force: " + crushForce);
		Reset();
		
		AudioSource.PlayClipAtPoint(deathSound, transform.position);
	}
	
	protected void OnJump(){
		if(JumpEffectPrefab){
			Instantiate(JumpEffectPrefab, JumpEffectLocation.position, Quaternion.identity);	
		}
		AudioSource.PlayClipAtPoint(jumpSound, transform.position);
	}
	
		
	protected void OnAirJump(){
		if(JumpEffectPrefab){
			// Could use a different effect for air jumps.
			Instantiate(JumpEffectPrefab, JumpEffectLocation.position, Quaternion.identity);	
		}
		// higher pitch for air jumps
		PlayClipAtPosition(jumpSound, transform.position, 1f, 1.8f);
	}
	
	protected void OnLand(){
		AudioSource.PlayClipAtPoint(landSound, transform.position);
	}
	
	
	public static AudioSource PlayClipAtPosition(AudioClip clip, Vector3 position, float volume, float pitch)
	{ 
		GameObject newClip = new GameObject(clip.name + " Instantiation");
		newClip.AddComponent(typeof(AudioSource));
		newClip.audio.clip = clip;
		newClip.transform.position = position;
		newClip.audio.volume = volume;
		newClip.audio.pitch = pitch;
		newClip.audio.Play();
		Object.DontDestroyOnLoad(newClip);
		Object.Destroy(newClip, clip.length / pitch + 0.2f);
		return newClip.audio;
	}
}

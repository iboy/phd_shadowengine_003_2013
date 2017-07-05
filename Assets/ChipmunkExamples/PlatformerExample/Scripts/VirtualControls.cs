// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class VirtualControls : MonoBehaviour {
	public Transform leftButton, rightButton, jumpButton;
	
	[HideInInspector]
	public float direction;
	[HideInInspector]
	public bool jump;
	
	protected void Update(){
		direction = 0f;
		jump = false;
		
		
		if(Input.GetMouseButton(0) && Input.touchCount == 0){
			ProcessTouch(Input.mousePosition);
		}
		
		foreach(var touch in Input.touches) ProcessTouch(touch.position);
	}
	
	protected void ProcessTouch(Vector3 point){
		var ray = Camera.mainCamera.ScreenPointToRay(point);
		direction += CheckDirection(ray);
		jump = jump || CheckJump(ray);
	}
	
	protected float CheckDirection(Ray ray){
		float l = (CheckHit(ray, this.leftButton ) ? 1f : 0f);
		float r = (CheckHit(ray, this.rightButton) ? 1f : 0f);
		
		return (r - l);
	}
	
	private bool CheckJump(Ray ray){
		return CheckHit(ray, this.jumpButton);
	}
	
	protected bool CheckHit(Ray ray, Transform target){
		var origin = target.InverseTransformPoint(ray.origin);
		var direction = target.InverseTransformDirection(ray.direction);
		
		Vector3 zeroCross = origin - direction*(origin.z/direction.z);
		return (zeroCross.magnitude < 0.5);
	}
}

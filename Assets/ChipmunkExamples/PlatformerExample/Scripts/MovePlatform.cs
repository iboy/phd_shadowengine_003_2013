// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class MovePlatform : MonoBehaviour {
	protected Vector2 startPos;
	protected ChipmunkBody body;
	
	protected void Start(){
		body = GetComponent<ChipmunkBody>();
		startPos = body.position;
	}
	
	public Vector2 offset = new Vector2(10f, 10f);
	public float duration = 5f;
	
	protected void FixedUpdate(){
		float alpha = 1f - Mathf.Abs(2f*Time.time/duration%2f - 1f);
		body.position = startPos + offset*Mathf.SmoothStep(0f, 1f, alpha);
	}
	
	protected void Update(){
	}
}

// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrayonBallLogic : MonoBehaviour {
	public static List<Ball> balls = new List<Ball>();
	
	public int max = 100;
	public GameObject ballPrefab;
	
	protected void FixedUpdate(){
		// Find balls that are in groups 4 or more
		foreach(Ball ball in balls){
			if(ball.root.count >= 4) Destroy(ball.gameObject, 1f);
		}
		
		// Reset the graph
		foreach(Ball ball in balls) ball.ResetGraphNode();
	}
	
	protected void Update(){
		if(balls.Count < max){
			var pos = new Vector3(Random.Range(-1.7f, 1.7f), 2.8f);
			Instantiate(ballPrefab, pos, Quaternion.identity);
		}
	}
}

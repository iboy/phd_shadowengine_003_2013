// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class BonusBlock : MonoBehaviour {
	public GameObject coinPrefab;
	public float popHeight = 3f;
	
	public AudioClip soundEffect;
	
	protected IEnumerator CoinCoro(){
		float g = -Physics.gravity.y;
		float v = Mathf.Sqrt(2f*g*popHeight);
		float duration = 2f*v/g;
		
		Vector3 startPos = transform.position + Vector3.forward;
		GameObject go = (GameObject)Instantiate(coinPrefab, startPos, Quaternion.identity);
		Destroy(go, duration);
		
		float start = Time.time;
		for(float t = 0f; t < duration; t = Time.time - start){
			go.transform.position = startPos + Vector3.up*(v*t - g*t*t/2f);
			
			yield return null;
		}
	}
	
	public void Bump(){
		StartCoroutine(CoinCoro());
		
		AudioSource.PlayClipAtPoint(soundEffect, transform.position);
	}
}

// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class SetLayerOfChildren : MonoBehaviour {

	public int layer;
	
	public void Start(){
		foreach(ChipmunkShape shape in this.gameObject.GetComponentsInChildren<ChipmunkShape>()){
			shape.layers = (uint) layer;
		}
	}
	
}

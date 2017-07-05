// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;

public class SetupRenderSettings : MonoBehaviour {
	
	public bool forceFogOnWhenPlay = true;
	
	public void Start(){
		// turn fog on, but only when we hit play... otherwise you can't see anything in the scene view :(
	    if(forceFogOnWhenPlay) RenderSettings.fog = true;
	}
}

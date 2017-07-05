using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Animation frame event params
[Serializable]
public class Uni2DAnimationFrameInfos
{
	// Use this to pass a string during the animation event 
	public string stringInfo;
		
	// Use this to pass int during the animation event 
	public int intInfo;
	
	// Use this to pass a float during the animation event 
	public float floatInfo;
	
	// Use this to pass a custom object during the animation event
	public UnityEngine.Object objectInfo;
	
	// Is this event params is different from an event params
	public bool IsDifferentFrom(Uni2DAnimationFrameInfos a_rOtherEventParams)
	{
		return stringInfo != a_rOtherEventParams.stringInfo
			|| intInfo != a_rOtherEventParams.intInfo
			|| floatInfo != a_rOtherEventParams.floatInfo
			|| objectInfo != a_rOtherEventParams.objectInfo;
	}
	
	// Copy this event params to an other event params
	public void CopyTo(Uni2DAnimationFrameInfos a_rOtherEventParams)
	{
		a_rOtherEventParams.stringInfo = stringInfo;
		a_rOtherEventParams.intInfo = intInfo;
		a_rOtherEventParams.floatInfo = floatInfo;
		a_rOtherEventParams.objectInfo = objectInfo;
	}
}
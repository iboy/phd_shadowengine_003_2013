using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Animation event
public class Uni2DAnimationEvent
{
	// The sprite animation that fired the event
	public Uni2DSpriteAnimation spriteAnimation;
	
	// The index of the clip that fired the event
	public int clipIndex;
	
	// the clip that fired the event
	public Uni2DAnimationClip clip;
	
	// Constructor
	public Uni2DAnimationEvent()
	{
	}
	
	// Constructor
	public Uni2DAnimationEvent(Uni2DSpriteAnimation a_rSpriteAnimation, int a_iClipIndex, Uni2DAnimationClip a_rClip)
	{
		spriteAnimation = a_rSpriteAnimation;
		clipIndex = a_iClipIndex;
		clip = a_rClip;
	}
}
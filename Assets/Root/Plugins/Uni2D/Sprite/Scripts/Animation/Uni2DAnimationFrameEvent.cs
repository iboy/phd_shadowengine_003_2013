using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Animation frame event
public class Uni2DAnimationFrameEvent : Uni2DAnimationEvent
{
	// The frame index that fired the event
	public int frameIndex;
	
	// The frame that fired the event
	public Uni2DAnimationFrame frame;
	
	// Constructor
	public Uni2DAnimationFrameEvent()
	{
	}
	
	// Constructor
	public Uni2DAnimationFrameEvent(Uni2DSpriteAnimation a_rSpriteAnimation, int a_iClipIndex, Uni2DAnimationClip a_rClip, int a_iFrameIndex, Uni2DAnimationFrame a_rFrame)
	{
		spriteAnimation = a_rSpriteAnimation;
		clipIndex = a_iClipIndex;
		clip = a_rClip;
		frameIndex = a_iFrameIndex;
		frame = a_rFrame;
	}
}
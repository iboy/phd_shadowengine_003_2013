using UnityEngine;
using System.Collections;

/* Uni2DAnimationPlayer
 * 
 * Handles animation playback.
 * 
 */
public class Uni2DAnimationPlayer
{
	// Animation end handler
	public delegate void AnimationEndEventHandler( Uni2DAnimationPlayer a_rAnimationPlayer, Uni2DAnimationClip a_rAnimationClip );
	
	// Animation frame event handler
	public delegate void AnimationFrameEventHandler( Uni2DAnimationPlayer a_rAnimationPlayer );
	
	// Animation on new frame event handler
	public delegate void AnimationNewFrameEventHandler( Uni2DAnimationPlayer a_rAnimationPlayer );

	// Animation on active event handler
	//public delegate void AnimationOnActiveEventHandler( );

	// Animation on inactive event handler
	public delegate void AnimationInactiveEventHandler( Uni2DAnimationPlayer a_rAnimationPlayer );
	
	// Handler called on animation end (i.e. NormalizedTime == 1.0f in forward play(positive speed) or NormalizedTime == 0.0f in backward play (negative speed))
	// Called at each loop end for Loop and PingPong wrap mode
	public AnimationEndEventHandler onAnimationEndEvent;
	
	// Handler called at the begining of each frame where the "trigger event" checkbox is checked
	public AnimationFrameEventHandler onAnimationFrameEvent;

	// Handler called on new frame
	public AnimationNewFrameEventHandler onAnimationNewFrameEvent;

	// Handler called when animation becomes active
	//public AnimationOnActiveEventHandler onAnimationActiveEvent;

	// Handler called when animation becomes inactive
	public AnimationInactiveEventHandler onAnimationInactiveEvent;
	
	// Is the animation enable?
	private bool m_bEnabled;

	// Is the animation active? (i.e. animation isn't over and still handling animation playback)
	private bool m_bActived;
	
	// Is the animation is paused
	private bool m_bPaused;
	
	// Animation play speed
	private float m_fAnimationSpeed = 1.0f;
	
	// Animation time
	private float m_fAnimationTime = 0.0f;
	
	// Current frame
	private Uni2DAnimationFrame m_rCurrentFrame;
	
	// Current clip frame
	private int m_iCurrentFrameIndex = 0;
	
	// Current clip
	private Uni2DAnimationClip m_rCurrentClip;
	
	// The wrap mode
	private Uni2DAnimationClip.WrapMode m_eWrapMode;
	
	// The frame rate
	private float m_fFrameRate;
	
	// Is animation on End?
	private bool m_bIsAnimationOnEnd;

	// Clip played
	public Uni2DAnimationClip Clip
	{
		get
		{
			return m_rCurrentClip;
		}
	}

	public bool Enabled
	{
		get
		{
			return m_bEnabled;
		}
	}

	// Is animation active?
	public bool Active
	{
		get
		{
			return m_bActived;
		}
	}
	
	// WrapMode
	public Uni2DAnimationClip.WrapMode WrapMode
	{
		get
		{
			return m_eWrapMode;
		}
		
		set
		{
			if(m_eWrapMode != value)
			{
				Uni2DAnimationClip.WrapMode eWrapModePrevious = m_eWrapMode;
				float fFrameShowTimeElapsed = Mathf.Abs(Time - ComputeAnimationTimeFromFrame());
				m_eWrapMode = value;
				OnWrapModeChange(eWrapModePrevious, fFrameShowTimeElapsed);
			}
		}
	}
	
	// Framerate
	public float FrameRate
	{
		get
		{
			return m_fFrameRate;
		}
		
		set
		{
			if(m_fFrameRate != value)
			{
				float fNormalizedTimePrevious = NormalizedTime;
				float fFrameRatePrevious = m_fFrameRate;
				m_fFrameRate = value;
				OnFrameRateChange(fFrameRatePrevious, fNormalizedTimePrevious);
			}
		}
	}
	
	// FrameCount
	public int FrameCount
	{
		get
		{
			if(m_rCurrentClip == null)
			{
				return 0;
			}
			else
			{
				if(m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
				{
					int iFrameCount = m_rCurrentClip.FrameCount;
					if(iFrameCount <= 0)
					{
						return 0;
					}
					else if(iFrameCount == 1)
					{
						return 1;	
					}
					else if(iFrameCount == 2)
					{
						return 2;
					}
					else
					{
						return iFrameCount * 2 - 2;
					}
				}
				else
				{
					return m_rCurrentClip.FrameCount;
				}
			}
		}
	}
	
	
	// Frame
	public int FrameIndex
	{
		get
		{
			return m_iCurrentFrameIndex;
		}
		
		set
		{
			// Set frame
			int iLastClipFrame = m_iCurrentFrameIndex;
			m_iCurrentFrameIndex = value;
			ConstraintFrame();
			
			// Set animation time
			m_fAnimationTime = ComputeAnimationTimeFromFrame();
			bool bReachTheEnd = HasAnimationReachTheEnd();
			
			// New frame?
			if(iLastClipFrame != m_iCurrentFrameIndex )
			{
				OnNewFrame( );
			}
			
			// If we reach the end
			if(bReachTheEnd)
			{
				OnClipEnd();
			}
		}
	}
	
	// Frame Name
	public string FrameName
	{
		get
		{
			if(m_rCurrentFrame == null)
			{
				return "";
			}
			else
			{
				return m_rCurrentFrame.name;
			}
		}
		
		set
		{
			if(m_rCurrentClip != null)
			{
				int iFrameIndex;
				if(m_rCurrentClip.TryGetFrameIndex(value, out iFrameIndex))
				{
					FrameIndex = iFrameIndex;
				}
			}
		}
	}
	
	// Frame
	public Uni2DAnimationFrame Frame
	{
		get
		{
			return m_rCurrentFrame;
		}
	}
	
	// Time
	public float Time
	{
		get
		{
			return m_fAnimationTime;
		}
		
		set
		{
			// Set animation time
			m_fAnimationTime = value;
			bool bReachTheEnd = HasAnimationReachTheEnd();
			ConstraintAnimationTime();
			
			// Set frame
			int iLastClipFrame = m_iCurrentFrameIndex;
			m_iCurrentFrameIndex = ComputeCurrentClipFrameFromTime( );
			if(iLastClipFrame != m_iCurrentFrameIndex )
			{
				OnNewFrame( );
			}
			
			// If we reach the end
			if(bReachTheEnd)
			{
				OnClipEnd();
			}
		}
	}
	
	// NormalizedTime
	public float NormalizedTime
	{
		get
		{
			if(m_rCurrentClip == null)
			{
				return 0.0f;
			}
			else
			{
				float fAnimationLength = Length;
				if(fAnimationLength == 0.0f)
				{
					return 0.0f;
				}
				else
				{
					return m_fAnimationTime/fAnimationLength;
				}
			}
		}
		
		set
		{
			Time = value * Length;
		}
	}
	
	// Speed
	public float Speed
	{
		get
		{
			return m_fAnimationSpeed;
		}
		
		set
		{
			m_fAnimationSpeed = value;
		}
	}
	
	// Length
	public float Length
	{
		get
		{
			if(m_rCurrentClip == null)
			{
				return 0.0f;
			}
			else
			{
				float fFrameRate = FrameRate;
				if(fFrameRate == 0.0f)
				{
					return 1.0f;
				}
				else
				{
					return Mathf.Abs(FrameCount/fFrameRate);
				}
			}
		}
	}
	
	// Current clip name
	public string Name
	{
		get
		{
			if(m_rCurrentClip == null)
			{
				return "NoClip";
			}
			else
			{
				return m_rCurrentClip.name;
			}
		}
	}
	
	// Paused
	public bool Paused
	{
		get
		{
			return m_bPaused;
		}
		
		set
		{
			m_bPaused = value;
		}
	}
	
	// Play the current clip from the beginning
	public void Play()
	{
		Play( m_rCurrentClip );
	}
	
	// Stop playing the current clip
	public void Stop( bool a_bResetToMainFrame = true )
	{
		m_bEnabled = false;
		m_bPaused  = false;
		
		if(a_bResetToMainFrame)
		{
			//m_rSprite.ResetToMainFrame();
			m_bActived = false;

			m_rCurrentClip       = null;
			m_iCurrentFrameIndex = 0;
			m_fAnimationTime     = 0;

			// Notify animation inactivity
			RaiseAnimationInactiveEvent( );
		}
	}
	
	// Play the clip from the beginning
	public void Play( Uni2DAnimationClip a_rAnimationClip )
	{
		// Get the clip
		m_rCurrentClip = a_rAnimationClip;
		
		// Reset
		
		// Time
		m_fAnimationTime = 0.0f;
		m_iCurrentFrameIndex = 0;
		
		// Play mode
		m_eWrapMode  = Uni2DAnimationClip.WrapMode.Loop;
		m_fFrameRate = Uni2DAnimationClip.defaultFrameRate;
		if(m_rCurrentClip != null)
		{
			m_eWrapMode  = m_rCurrentClip.wrapMode;
			m_fFrameRate = m_rCurrentClip.frameRate;
		}
		
		// Play
		m_bEnabled = true;
		m_bPaused  = false;
		m_bActived = true;
		
		//RaiseAnimationActiveEvent( );
		
		// On New Frame
		OnNewFrame();
	}
	
	// Update
	public void Update(float a_fDeltaTime)
	{
		if(m_bEnabled && m_bPaused == false && FrameRate != 0 && Speed != 0.0f)
		{
			// Last frame
			int iLastClipFrame = m_iCurrentFrameIndex;
			
			// Advance animation time
			m_fAnimationTime += m_fAnimationSpeed * a_fDeltaTime;
			
			// Check if we reached the end
			bool bHasAnimationReachTheEnd = HasAnimationReachTheEnd();
			
			// Constraint the time
			ConstraintAnimationTime();
			
			// Compute current clip frame
			m_iCurrentFrameIndex = ComputeCurrentClipFrameFromTime();
			
			// If the frame has changed
			if(iLastClipFrame != m_iCurrentFrameIndex)
			{
				OnNewFrame();
			}
			
			// If we reach the end
			if(bHasAnimationReachTheEnd)
			{
				OnClipEnd();
			}
		}
	}
	
	// Compute current clip frame
	private int ComputeCurrentClipFrameFromTime()
	{
		int iFrameCount = FrameCount;
		if(iFrameCount == 0)
		{
			return 0;		
		}
		else if(m_fAnimationTime == Length)
		{
			if(FrameRate < 0.0f)
			{
				return 0;
			}
			else
			{
				if(m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
				{
					return 0;
				}
				else
				{
					return iFrameCount - 1;
				}
			}
		}
		else
		{	
			float fFrameRate = FrameRate;
			float fFrameRateAbs = Mathf.Abs(fFrameRate);
			
			int iFrame;
			if(fFrameRateAbs == 0.0f)
			{
				iFrame = (int)(m_fAnimationTime * iFrameCount);
			}
			else
			{
				iFrame = (int)(m_fAnimationTime * fFrameRateAbs);
			}
			
			if(fFrameRate < 0.0f)
			{
				if(m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
				{
					iFrame = (int)Mathf.Repeat(m_rCurrentClip.FrameCount - 1 - iFrame, iFrameCount);
				}
				else
				{
					iFrame = iFrameCount - 1 - iFrame;
				}
			}

			return (int)Mathf.Repeat(iFrame, iFrameCount);
		}
	}
	
	// Compute animation time
	private float ComputeAnimationTimeFromFrame()
	{
		int iFrameCount = FrameCount;
		if(iFrameCount == 0)
		{
			return 0.0f;			
		}
		else
		{
			float fFrameRate = FrameRate;
			if(fFrameRate == 0.0f)
			{
				if(iFrameCount == 0)
				{
					return 0;
				}
				else
				{
					return (float)m_iCurrentFrameIndex / (float)iFrameCount;
				}
			}
			else
			{
				int iFrame = m_iCurrentFrameIndex;
				if(fFrameRate < 0.0f)
				{
					if(m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
					{
						iFrame = (int)Mathf.Repeat(m_rCurrentClip.FrameCount - 1 - m_iCurrentFrameIndex, iFrameCount);
					}
					else
					{
						iFrame = iFrameCount - 1 - m_iCurrentFrameIndex;
					}
				}
				
				return iFrame/Mathf.Abs(fFrameRate);
			}
		}
	}
	
	// Constraint animation time
	private void ConstraintAnimationTime()
	{
		float fLength = Length;
		if(fLength <= 0.0f)
		{
			m_fAnimationTime = 0.0f;
		}
		else
		{
			switch(m_eWrapMode)
			{
				case Uni2DAnimationClip.WrapMode.Loop:
				case Uni2DAnimationClip.WrapMode.PingPong:
				{
					if(m_fAnimationTime != Length)
					{
						m_fAnimationTime = Mathf.Repeat(m_fAnimationTime, Length);
					}
				}
				break;
				
				case Uni2DAnimationClip.WrapMode.Once:
				{
					m_fAnimationTime = Mathf.Clamp(m_fAnimationTime, 0.0f, Length);
				}
				break;
				
				case Uni2DAnimationClip.WrapMode.ClampForever:
				{
					m_fAnimationTime = Mathf.Clamp(m_fAnimationTime, 0.0f, Length);
				}
				break;
			}
		}
	}
	
	// Constraint frame
	private void ConstraintFrame()
	{
		switch(m_eWrapMode)
		{
			case Uni2DAnimationClip.WrapMode.Loop:
			case Uni2DAnimationClip.WrapMode.PingPong:
			{
				m_iCurrentFrameIndex = (int)Mathf.Repeat(m_iCurrentFrameIndex, FrameCount);
			}
			break;
			
			case Uni2DAnimationClip.WrapMode.Once:
			{
				m_iCurrentFrameIndex = Mathf.Clamp(m_iCurrentFrameIndex, 0, FrameCount);
			}
			break;
			
			case Uni2DAnimationClip.WrapMode.ClampForever:
			{
				m_iCurrentFrameIndex = Mathf.Clamp(m_iCurrentFrameIndex, 0, FrameCount - 1);
			}
			break;
		}
	}
	
	// On new frame
	private void OnNewFrame()
	{
		if( m_rCurrentClip != null)
		{
			int iFrameCount = m_rCurrentClip.FrameCount;
			int iFrame;
			if(iFrameCount > 0)
			{
				if(iFrameCount == 1)
				{
					iFrame = 0;
				}
				else
				{
				 	iFrame = (int)Mathf.PingPong(m_iCurrentFrameIndex, iFrameCount - 1);
				}
			
				m_rCurrentFrame = m_rCurrentClip.frames[iFrame];
				//m_rSprite.SetFrame(m_rCurrentFrame);

				RaiseAnimationNewFrameEvent();

				RaiseAnimationFrameEvent();
			}
			else
			{
				m_rCurrentFrame = null;
			}
		}
	}
	
	// Has animation reached the end?
	private bool HasAnimationReachTheEnd()
	{
		bool bWasOnEnd = m_bIsAnimationOnEnd;
		m_bIsAnimationOnEnd = Speed >= 0.0f && m_fAnimationTime >= Length || Speed <= 0.0f && m_fAnimationTime <= 0;
		
		return bWasOnEnd == false && m_bIsAnimationOnEnd;
	}
	
	// On clip end
	private void OnClipEnd()
	{
		Uni2DAnimationClip rPreviousClip = m_rCurrentClip;
		switch(m_eWrapMode)
		{
			case Uni2DAnimationClip.WrapMode.Loop:
			{
			}
			break;
			
			case Uni2DAnimationClip.WrapMode.PingPong:
			{
			}
			break;
			
			case Uni2DAnimationClip.WrapMode.Once:
			{
				Stop(true);
			}
			break;
			
			case Uni2DAnimationClip.WrapMode.ClampForever:
			{
			}
			break;
		}
		RaiseAnimationEndEvent( rPreviousClip );
	}
	
	// On anim length change
	private void OnWrapModeChange(Uni2DAnimationClip.WrapMode a_eWrapModePrevious, float a_fFrameShowTimeElapsed)
	{
		if(a_eWrapModePrevious == Uni2DAnimationClip.WrapMode.PingPong 
			|| m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
		{
			if(a_eWrapModePrevious == Uni2DAnimationClip.WrapMode.PingPong)
			{
				m_iCurrentFrameIndex = (int)Mathf.PingPong(m_iCurrentFrameIndex, FrameCount - 1);
			}
			
			// Compute the time the image has been show until then
			m_fAnimationTime = ComputeAnimationTimeFromFrame() + a_fFrameShowTimeElapsed; 
		}
	}
	
	// On frame rate change
	private void OnFrameRateChange(float a_fFrameRatePrevious, float a_fNormalizedTimePrevious)
	{
		float fFrameRate = FrameRate;
		if((a_fFrameRatePrevious < 0.0f && fFrameRate >= 0.0f)
			|| (fFrameRate < 0.0f && a_fFrameRatePrevious >= 0.0f))
		{
			if(m_eWrapMode == Uni2DAnimationClip.WrapMode.PingPong)
			{
				// Take a shifter complementary
				float fShift = ComputeReverseShift();
				float fShiftedNormalizedTime = 1.0f - a_fNormalizedTimePrevious + fShift;
				if(fShiftedNormalizedTime != 1.0f)
				{
					fShiftedNormalizedTime = Mathf.Repeat(fShiftedNormalizedTime, 1.0f);
				}
				NormalizedTime = fShiftedNormalizedTime;
			}
			else
			{
				// Take the complementary
				NormalizedTime = 1.0f - a_fNormalizedTimePrevious;
			}
		}
		else
		{
			// keep the same normalized time
			NormalizedTime = a_fNormalizedTimePrevious;
		}
	}
	
	// Compute ping pong frame co
	private float ComputePingPongFrameCount()
	{
		int iFrameCount = m_rCurrentClip.FrameCount;
		if(iFrameCount <= 0)
		{
			return 0;
		}
		else if(iFrameCount == 1)
		{
			return 1;	
		}
		else if(iFrameCount == 2)
		{
			return 2;
		}
		else
		{
			return iFrameCount * 2 - 2;
		}
	}
	
	// Compute the reverse origin shift
	private float ComputeReverseShift()
	{
		int iFrameCount = FrameCount;
		if(iFrameCount == 0)
		{
			return 0.0f;
		}
		else
		{
			return (float)m_rCurrentClip.FrameCount/(float)iFrameCount;
		}
	}
	
	// Raise animation end event
	private void RaiseAnimationEndEvent( Uni2DAnimationClip a_rAnimationClip )
	{
		if( onAnimationEndEvent != null )
		{
			onAnimationEndEvent( this, a_rAnimationClip );
		}
	}
	
	// Raise animation frame event
	private void RaiseAnimationFrameEvent( )
	{
		if( m_rCurrentFrame != null && m_rCurrentFrame.triggerEvent && onAnimationFrameEvent != null )
		{
			onAnimationFrameEvent( this );
		}
	}

	private void RaiseAnimationInactiveEvent( )
	{
		if( onAnimationInactiveEvent != null )
		{
			onAnimationInactiveEvent( this );
		}
	}

	private void RaiseAnimationNewFrameEvent( )
	{
		if( onAnimationNewFrameEvent != null )
		{
			onAnimationNewFrameEvent( this );
		}
	}
}

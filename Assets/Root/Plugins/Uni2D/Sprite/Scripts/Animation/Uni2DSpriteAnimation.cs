using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

// Uni2D sprite animation
[System.Serializable]
public class Uni2DSpriteAnimation
{	
	// Animation end handler
	public delegate void AnimationEndEventHandler(Uni2DAnimationEvent a_rAnimationEvent);
	
	// Animation frame event handler
	public delegate void AnimationFrameEventHandler(Uni2DAnimationFrameEvent a_rAnimationFrameEvent);
	
	// Handler called on animation end (i.e. NormalizedTime == 1.0f in forward play(positive speed) or NormalizedTime == 0.0f in backward play (negative speed))
	// Called at each loop end for Loop and PingPong wrap mode
	public AnimationEndEventHandler onAnimationEndEvent;
	
	// Handler called at the begining of each frame where the "trigger event" checkbox is checked
	public AnimationFrameEventHandler onAnimationFrameEvent;

	// Animation player
	private Uni2DAnimationPlayer m_rAnimationPlayer = new Uni2DAnimationPlayer( );

	// Animation clips
	[SerializeField]
	private Uni2DAnimationClip[ ] m_rAnimationClips;
	
	// Clip index
	[SerializeField]
	private int m_iStartClipIndex;
	
	// Play automatically
	public bool playAutomatically = true;
	
	// Current clip index
	private int m_iCurrentClipIndex = -1;
	
	// The sprite
	private Uni2DSprite m_rSprite;
	
	// Clip index by name
	private Dictionary<string, int> m_oClipIndexByName;		
	
	// Sprite
	public Uni2DSprite Sprite
	{
		get
		{
			return m_rSprite;	
		}
	}
	
	// WrapMode
	public Uni2DAnimationClip.WrapMode WrapMode
	{
		get
		{
			return m_rAnimationPlayer.WrapMode;
		}
		
		set
		{
			m_rAnimationPlayer.WrapMode = value;
		}
	}
	
	// Framerate
	public float FrameRate
	{
		get
		{
			return m_rAnimationPlayer.FrameRate;
		}
		
		set
		{
			m_rAnimationPlayer.FrameRate = value;
		}
	}
	
	// Frame Count
	public int FrameCount
	{
		get
		{
			return m_rAnimationPlayer.FrameCount;
		}
	}
	
	
	// Frame Index
	public int FrameIndex
	{
		get
		{
			return m_rAnimationPlayer.FrameIndex;
		}
		
		set
		{
			m_rAnimationPlayer.FrameIndex = value;
		}
	}
	
	// Frame Name
	public string FrameName
	{
		get
		{
			return m_rAnimationPlayer.FrameName;
		}
		
		set
		{
			m_rAnimationPlayer.FrameName = value;
		}
	}
	
	// Frame
	public Uni2DAnimationFrame Frame
	{
		get
		{
			return m_rAnimationPlayer.Frame;
		}
	}
	
	// Time
	public float Time
	{
		get
		{
			return m_rAnimationPlayer.Time;
		}
		
		set
		{
			// Set animation time
			m_rAnimationPlayer.Time = value;
		}
	}
	
	// NormalizedTime
	public float NormalizedTime
	{
		get
		{
			return m_rAnimationPlayer.NormalizedTime;
		}
		
		set
		{
			m_rAnimationPlayer.NormalizedTime = value;
		}
	}
	
	// Speed
	public float Speed
	{
		get
		{
			return m_rAnimationPlayer.Speed;
		}
		
		set
		{
			m_rAnimationPlayer.Speed = value;
		}
	}
	
	// Length
	public float Length
	{
		get
		{
			return m_rAnimationPlayer.Length;
		}
	}
	
	// Current clip name
	public string Name
	{
		get
		{
			return m_rAnimationPlayer.Name;
		}
	}
	
	// Current clip index
	// return -1 if no clip is playing
	public int CurrentClipIndex
	{
		get
		{
			return m_iCurrentClipIndex;
		}
	}

	// Clip played
	public Uni2DAnimationClip Clip
	{
		get
		{
			return m_rAnimationPlayer.Clip;
		}
	}
	
	// Number of clip
	public int ClipCount
	{
		get
		{
			return m_rAnimationClips != null ? m_rAnimationClips.Length : 0;
		}
	}
	
	// Paused
	public bool Paused
	{
		get
		{
			return m_rAnimationPlayer.Paused;
		}
		
		set
		{
			m_rAnimationPlayer.Paused = value;
		}
	}
	
	// Is Playing
	public bool IsPlaying
	{
		get
		{
			return m_rAnimationPlayer.Active;
		}	
	}
	
	// Play the current clip from the beginning
	public void Play()
	{
		Play(m_iCurrentClipIndex);
	}
	
	// Stop playing the current clip
	public void Stop(bool a_bResetToMainFrame = true)
	{
		m_rAnimationPlayer.Stop( a_bResetToMainFrame );
	}
	
	// Pause
	public void Pause()
	{
		Paused = true;
	}
	
	// Resume
	public void Resume()
	{
		Paused = false;
	}
	
	// Play the clip from the beginning by name
	// Return false if the clip doesn't exist
	public bool Play(string a_oClipName)
	{
		int iClipIndex = GetClipIndexByName(a_oClipName);
		return Play(iClipIndex);
	}
	
	// Play the clip from the beginning
	// Return false if the clip doesn't exist
	public bool Play(int a_iClipIndex)
	{
		if(IsValidClipIndex(a_iClipIndex))
		{
			// Get the clip
			m_iCurrentClipIndex = a_iClipIndex;

			// Play
			m_rAnimationPlayer.Play(m_rAnimationClips[a_iClipIndex]);
			
			return true;
		}
		else
		{
			return false;
		}
	}
	
	// Get clip by index
	public Uni2DAnimationClip GetClipByIndex(int a_iClipIndex)
	{
		if(IsValidClipIndex(a_iClipIndex))
		{
			return m_rAnimationClips[a_iClipIndex];
		}
		else
		{
			return null;
		}
	}
	
	// Get clip by index
	public Uni2DAnimationClip GetClipByName(string a_oClipName)
	{
		return GetClipByIndex(GetClipIndexByName(a_oClipName));
	}
	
	// Get clip index by name
	public int GetClipIndexByName(string a_oClipName)
	{
		Dictionary<string, int> oClipIndexByName = GetClipIndexByName();
		int iClipIndex;
		if(oClipIndexByName.TryGetValue(a_oClipName, out iClipIndex))
		{
			return iClipIndex;
		}
		else
		{
			return -1;
		}
	}
	
	// Start
	public void Start(Uni2DSprite a_rSprite)
	{
		m_rSprite = a_rSprite;
		m_iCurrentClipIndex = m_iStartClipIndex;

		// Setup animation player
		m_rAnimationPlayer.onAnimationFrameEvent    += RaiseAnimationFrameEvent;
		m_rAnimationPlayer.onAnimationNewFrameEvent += RaiseAnimationNewFrameEvent;	// New frame event
		m_rAnimationPlayer.onAnimationEndEvent      += RaiseAnimationEndEvent;
		m_rAnimationPlayer.onAnimationInactiveEvent += RaiseAnimationInactiveEvent;

		if( playAutomatically )
		{
			this.Play( );
		}
	}
	
	// Update
	public void Update(float a_fDeltaTime)
	{
		m_rAnimationPlayer.Update( a_fDeltaTime );
	}
		
	// Is valid clip index
	private bool IsValidClipIndex(int a_iClipIndex)
	{
		return m_rAnimationClips != null && a_iClipIndex >= 0 && a_iClipIndex < m_rAnimationClips.Length;
	}

	// On new frame
	private void RaiseAnimationNewFrameEvent( Uni2DAnimationPlayer a_rAnimationPlayer )	// On New Frame
	{
		m_rSprite.SetFrame( m_rAnimationPlayer.Frame );
	}
	
	// Raise animation end event
	private void RaiseAnimationEndEvent( Uni2DAnimationPlayer a_rAnimationPlayer, Uni2DAnimationClip a_rAnimationClip )
	{
		if( onAnimationEndEvent != null )
		{
			onAnimationEndEvent( new Uni2DAnimationEvent( this, this.GetClipIndexByName( a_rAnimationClip.name ), a_rAnimationClip ) );
		}
	}
	
	// Raise animation frame event
	private void RaiseAnimationFrameEvent( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		if( onAnimationFrameEvent != null )
		{
			onAnimationFrameEvent(new Uni2DAnimationFrameEvent(this, m_iCurrentClipIndex, m_rAnimationClips[ m_iCurrentClipIndex ] , a_rAnimationPlayer.FrameIndex, a_rAnimationPlayer.Frame ) );
		}
	}

	// Releases the sprite when animation is inactive (the sprite is no longer animated)
	private void RaiseAnimationInactiveEvent( Uni2DAnimationPlayer a_rAnimationPlayer )
	{
		m_rSprite.ResetToMainFrame( );
	}
	
	// Get Clip Index By Name
	private Dictionary<string, int> GetClipIndexByName()
	{
		if(m_oClipIndexByName == null)
		{
			BuildClipIndexByName();
		}
		return m_oClipIndexByName;
	}
	
	// Build Clip Index By Name
	private void BuildClipIndexByName()
	{
		m_oClipIndexByName = new Dictionary<string, int>();
		int iClipIndex = 0;
		foreach(Uni2DAnimationClip rClip in m_rAnimationClips)
		{
			if(rClip != null)
			{
				if(m_oClipIndexByName.ContainsKey(rClip.name) == false)
				{
					m_oClipIndexByName.Add(rClip.name, iClipIndex);
				}
			}
			iClipIndex++;
		}
	}

#if UNITY_EDITOR
	public Uni2DAnimationClip[] Clips
	{
		get
		{
			return m_rAnimationClips;
		}
	}

	public int StartClipIndex
	{
		get
		{
			return m_iStartClipIndex;
		}
		set
		{
			if( IsValidClipIndex( value ) )
			{
				m_iStartClipIndex = value;
			}
		}
	}

	// Swaps 2 clips specified by their index
	public void SwapClips( int a_iClipIndexA, int a_iClipIndexB )
	{
		if( a_iClipIndexA != a_iClipIndexB && this.IsValidClipIndex( a_iClipIndexA ) && this.IsValidClipIndex( a_iClipIndexB ) )
		{
			// Update start clip index
			if( a_iClipIndexA == m_iStartClipIndex )
			{
				m_iStartClipIndex = a_iClipIndexB;
			}
			else if( a_iClipIndexB == m_iStartClipIndex )
			{
				m_iStartClipIndex = a_iClipIndexA;
			}

			// Swap clips
			Uni2DAnimationClip rTmp = m_rAnimationClips[ a_iClipIndexA ];
			m_rAnimationClips[ a_iClipIndexA ] = m_rAnimationClips[ a_iClipIndexB ];
			m_rAnimationClips[ a_iClipIndexB ] = rTmp;
		}
	}

	// Add a clip to the list
	public void AddClip( Uni2DAnimationClip a_rAnimationClip )
	{
		if( a_rAnimationClip != null )
		{
			int iClipCount = this.ClipCount;
	
			// Prevent to add a clip twice
			for( int iClipIndex = 0; iClipIndex < iClipCount; ++iClipIndex )
			{
				if( a_rAnimationClip == m_rAnimationClips[ iClipIndex ] )
				{
					return;
				}
			}
	
			Uni2DAnimationClip[ ] oAnimationClips = new Uni2DAnimationClip[ iClipCount + 1 ];
	
			if( m_rAnimationClips != null )
			{
				m_rAnimationClips.CopyTo( oAnimationClips, 0 );
			}
	
			oAnimationClips[ iClipCount ] = a_rAnimationClip;
			m_rAnimationClips = oAnimationClips;
		}
	}

	// Remove a clip specified by the given index
	public void RemoveClip( int a_iClipToRemoveIndex )
	{
		if( this.IsValidClipIndex( a_iClipToRemoveIndex ) )
		{
			if( m_iStartClipIndex == a_iClipToRemoveIndex )
			{
				//playAutomatically = false;
				m_iStartClipIndex = 0;
			}

			int iClipCount = this.ClipCount;
			Uni2DAnimationClip[ ] oAnimationClips = new Uni2DAnimationClip[ iClipCount - 1 ];

			for( int iClipIndex = 0; iClipIndex < a_iClipToRemoveIndex; ++iClipIndex )
			{
				oAnimationClips[ iClipIndex ] = m_rAnimationClips[ iClipIndex ];
			}

			for( int iClipIndex = a_iClipToRemoveIndex + 1; iClipIndex < iClipCount; ++iClipIndex )
			{
				oAnimationClips[ iClipIndex - 1 ] = m_rAnimationClips[ iClipIndex ];
			}

			m_rAnimationClips = oAnimationClips;
		}
	}

	// Deletes null animation clip references (which appear after deleting a clip resource)
	public void CleanDeletedAnimationClips( )
	{
		Uni2DAnimationClip rStartAnimationClip = playAutomatically
			? m_rAnimationClips[ m_iStartClipIndex ]
			: null;

		// LINQ: Select only non null clips
		m_rAnimationClips = m_rAnimationClips.Where( x => x != null ).ToArray( );

		if( rStartAnimationClip != null )
		{
			m_iStartClipIndex = ArrayUtility.IndexOf<Uni2DAnimationClip>( m_rAnimationClips, rStartAnimationClip );
		}
		else
		{
			// Turn off auto play if clip array is empty
			playAutomatically = ( m_rAnimationClips.Length > 0 ? playAutomatically : false );
			m_iStartClipIndex = 0;
		}
	}
#endif
}
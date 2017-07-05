using UnityEngine;
using System.Collections;

[AddComponentMenu("Uni2D/Samples/SpriteAnimationViewer")]
// Sprite Animation Viewer
public class SpriteAnimationViewer : MonoBehaviour 
{
	// The sprite
	public Uni2DSprite sprite;
	
	// The control screen area center
	private Vector2 m_f2ControlScreenArea_Center = new Vector2(0.5f, 0.65f);
	
	// The control screen area size
	private Vector2 m_f2ControlScreenArea_Size = new Vector2(0.9f, 0.2f);
	
	// Awake
	private void Awake()
	{
		sprite.spriteAnimation.Play();
	}
	
	// On GUI
	private void OnGUI()
	{
		if(sprite == null || sprite.spriteAnimation.ClipCount <= 0)
		{
			return;	
		}
		
		GUILayout.BeginArea(new Rect((m_f2ControlScreenArea_Center.x - m_f2ControlScreenArea_Size.x * 0.5f) * Screen.width,
			(m_f2ControlScreenArea_Center.y - m_f2ControlScreenArea_Size.y * 0.5f) * Screen.width,
			m_f2ControlScreenArea_Size.x * Screen.width, m_f2ControlScreenArea_Size.y * Screen.width));
		{
			// Clip selector
			GUILayout.Label("Clip : " + sprite.spriteAnimation.Name + " (" + sprite.spriteAnimation.CurrentClipIndex + ")");
			GUILayout.BeginHorizontal();
			{
				if(GUILayout.Button("Prev"))
				{
					sprite.spriteAnimation.Play((sprite.spriteAnimation.CurrentClipIndex + sprite.spriteAnimation.ClipCount - 1) % sprite.spriteAnimation.ClipCount);
				}
				
				if(GUILayout.Button("Next"))
				{
					sprite.spriteAnimation.Play((sprite.spriteAnimation.CurrentClipIndex + 1) % sprite.spriteAnimation.ClipCount);
				}
			}
			GUILayout.EndHorizontal();
			
			// Speed slider
			GUILayout.Label("Speed : " + sprite.spriteAnimation.Speed.ToString("0.00"));
			sprite.spriteAnimation.Speed = GUILayout.HorizontalSlider(sprite.spriteAnimation.Speed, -2.0f, 2.0f);
			
			// Playback control			
			DisplayPlaybackControls();
			
		}
		GUILayout.EndArea();
	}
	
	// Display Playback controls
	private void DisplayPlaybackControls()
	{
		// Frame Slider
		GUILayout.Label( "Frame " + ( sprite.spriteAnimation.FrameIndex + 1 ) + "/" + sprite.spriteAnimation.FrameCount);
		float fNormalizedTime;
		GUI.changed = false;
		fNormalizedTime = GUILayout.HorizontalSlider(sprite.spriteAnimation.NormalizedTime, 0.0f, 1.0f);
		if(GUI.changed)
		{
			sprite.spriteAnimation.Paused = true;
			sprite.spriteAnimation.NormalizedTime = fNormalizedTime;
		}
		
		GUILayout.BeginHorizontal( );
		{
			// Rewind to first frame |<<
			// Unity 4 doesn't handle these chars very well, so...
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if( GUILayout.Button( "\u2503\u25c0\u25c0") )
#else
			if( GUILayout.Button( "\u2503\u25c1\u25c1") )
#endif
			{
				sprite.spriteAnimation.Paused = true;
				sprite.spriteAnimation.FrameIndex = 0;
			}

			// Previous frame <|
			// Unity 4 doesn't handle these chars very well, so...
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			if( GUILayout.Button( "\u25c0\u2503") )
#else
			if( GUILayout.Button( "\u25c1\u2503") )
#endif
			{
				sprite.spriteAnimation.Paused = true;
				--sprite.spriteAnimation.FrameIndex;
			}

			// Play / pause
			// Is paused or not playing?
			if( sprite.spriteAnimation.Paused || sprite.spriteAnimation.IsPlaying == false )
			{
				// Display play button >
				if( GUILayout.Button( "\u25B6") )
				{
					if( sprite.spriteAnimation.Paused )
					{
						sprite.spriteAnimation.Paused = false;
					}
					else
					{
						sprite.spriteAnimation.Play( );
					}
				}
			}	// Playing
			else if( GUILayout.Button( "\u2590\u2590" ) )	// Display pause button ||
			{
				sprite.spriteAnimation.Paused = true;
			}

			// Next frame |>
			if( GUILayout.Button( "\u2503\u25B6" ) )
			{
				sprite.spriteAnimation.Paused = true;
				++sprite.spriteAnimation.FrameIndex;
			}

			// Go to last frame >>|
			if( GUILayout.Button( "\u25B6\u25B6\u2503" ) )
			{
				sprite.spriteAnimation.Paused = true;
				sprite.spriteAnimation.FrameIndex = sprite.spriteAnimation.FrameCount - 1;
			}
		}
		GUILayout.EndHorizontal( );
	}
}

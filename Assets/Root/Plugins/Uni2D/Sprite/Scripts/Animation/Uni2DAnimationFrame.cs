using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
// Animation frame
public class Uni2DAnimationFrame
{
	private const int mc_iNoTextureWidth  = 0;
	private const int mc_iNoTextureHeight = 0;
	
	// Name
	public string name;
	
	// Texture container
	public Texture2DContainer textureContainer;
	
	// Optionnal atlas containing the texture to use for the frame
	public Uni2DTextureAtlas atlas;
	
	// Trigger event
	// Set this to true if you want to be notified when this frame is played
	// During an animation
	public bool triggerEvent;
	
	// Frame infos passed along with the frame event
	public Uni2DAnimationFrameInfos frameInfos = new Uni2DAnimationFrameInfos();
	
	// The texture width
	[SerializeField]
	[HideInInspector]
	private int m_iTextureWidth = 1024;
	
	// The texture height
	[SerializeField]
	[HideInInspector]
	private int m_iTextureHeight = 1024;
	
	// Texture width
	public int TextureWidth
	{
		get
		{
			return m_iTextureWidth;	
		}
	}
	
	// Texture height
	public int TextureHeight
	{
		get
		{
			return m_iTextureHeight;	
		}
	}
	
	// Generate infos
	public void GenerateInfos( )
	{
		if( textureContainer != null )
		{
			// Ensure the frame is/is not referencing the texture if it has/hasn't have to
			// regarding to atlas setting
			if( textureContainer.IsKeepingTextureReference ^ atlas == null )	// XOR
			{
				// Keep reference if not atlasing; don't keep it if atlasing
				textureContainer = new Texture2DContainer( textureContainer.GUID, atlas == null );
			}

			if( textureContainer.Texture != null )
			{
				m_iTextureWidth  = textureContainer.Texture.width;
				m_iTextureHeight = textureContainer.Texture.height;
			}
		}
		else
		{
			m_iTextureWidth  = mc_iNoTextureWidth;
			m_iTextureHeight = mc_iNoTextureHeight;
		}
	}
	
	// Is this frame is different from an other frame
	public bool IsDifferentFrom( Uni2DAnimationFrame a_rOtherFrame )
	{
		return textureContainer != a_rOtherFrame.textureContainer
			|| name     != a_rOtherFrame.name
			|| atlas            != a_rOtherFrame.atlas
			|| triggerEvent     != a_rOtherFrame.triggerEvent
			|| frameInfos.IsDifferentFrom( a_rOtherFrame.frameInfos );
	}

	// Default constructor
	public Uni2DAnimationFrame( )
	{
		textureContainer = new Texture2DContainer( null );
	}

	// Copy Constructor
	public Uni2DAnimationFrame(Uni2DAnimationFrame a_rFrameSource)
	{
		name = a_rFrameSource.name;
		atlas            = a_rFrameSource.atlas;
		triggerEvent     = a_rFrameSource.triggerEvent;
		a_rFrameSource.frameInfos.CopyTo( frameInfos );

		textureContainer = new Texture2DContainer( a_rFrameSource.textureContainer.GUID, atlas == null );
	}
}
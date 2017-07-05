using UnityEngine;
using System.Collections;

/*
 * Uni2DPixelPerfectCamera
 * 
 * This component forces a camera component to use
 * an orthographic projection and a 1:1 pixel/screen ratio (a.k.a "pixel perfect")
 * 
 * The pixelPerfect value can be used to enable or disable the pixel perfect feature at runtime.
 * 
 */
[AddComponentMenu( "Uni2D/Pixel Perfect Camera" )]
[RequireComponent( typeof( Camera ) )]
[ExecuteInEditMode( )]
public class Uni2DPixelPerfectCamera : MonoBehaviour
{
	// Is pixel perfect enabled?
	public bool pixelPerfect = true;

	// Cache: camera & transform
	private Camera m_rCamera = null;
	private Transform m_rTransform = null;

	// Computes the orthographic size value to use
	// to have pixel perfect textures 
	public float pixelPerfectOrthographicSize
	{
		get
		{
			return Screen.height * 0.5f * Uni2DSpriteUtils.mc_fSpriteUnitToUnity;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		// Cache transform & camera components
		m_rTransform = this.transform;
		m_rCamera    = this.GetComponent<Camera>( );

		// Init pixel perfect cam
		this.CheckPixelPerfectSettings( );
	}
	
	// LateUpdate is called once per frame
	void LateUpdate ()
	{
		this.CheckPixelPerfectSettings( );
	}

	// Checks camera settings and apply good ones
	// if needed and pixel perfect enabled
	private void CheckPixelPerfectSettings( )
	{
		// If pixel perfect enabled...
		if( pixelPerfect )
		{
			// Check camera settings
			float fPixelPerfectOrthoSize = this.pixelPerfectOrthographicSize;
			if( m_rCamera.orthographic == false || m_rCamera.orthographicSize != fPixelPerfectOrthoSize )
			{
				m_rCamera.orthographic = true;
				m_rCamera.orthographicSize = fPixelPerfectOrthoSize;
			}
	
			// Check camera rotation
			Vector3 f3EulerAngles = m_rTransform.eulerAngles;
	
			// X / Y rotation axises forbidden in pixel perfect
			if( f3EulerAngles.x != 0.0f || f3EulerAngles.y != 0.0f )
			{
				f3EulerAngles.x = 0.0f;
				f3EulerAngles.y = 0.0f;
				m_rTransform.eulerAngles = f3EulerAngles;
			}
		}
	}
}

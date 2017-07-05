using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * Texture2DContainer
 * 
 * Contains the GUID of a texture and provides a transparent access to the associated Texture2D object.
 * It avoids to make direct references to Texture2D objects (which make Unity to adding them when building, even if not needed).
 * 
 * Editor use only.
 * 
 */
[System.Serializable]
public class Texture2DContainer
{
	// The texture GUID
	[SerializeField]
	[HideInInspector]
	private string m_oTextureGUID = null;

	// The texture instance
	[SerializeField]
	[HideInInspector]
	private Texture2D m_rTexture = null;

	// Container accessors
	public string GUID
	{
		get
		{
			return m_oTextureGUID;
		}
	}

	public Texture2D Texture
	{
		// In editor, returns a Texture2D object from the asset database via the texture GUID
		// Null if not found or not a texture

		// At runtime, returns the texture reference (asset database not available)
		get
		{
			#if UNITY_EDITOR
			return m_rTexture != null ? m_rTexture : Uni2DEditorUtils.GetAssetFromUnityGUID<Texture2D>( m_oTextureGUID );
			#else
			return m_rTexture;
			#endif
		}
	}

	// Returns true if this container has a direct reference to the texture
	public bool IsKeepingTextureReference
	{
		get
		{
			return m_rTexture != null;
		}
	}

	public Texture2DContainer( Texture2DContainer a_rTextureContainer )
	{
		if( a_rTextureContainer != null )
		{
			m_oTextureGUID = a_rTextureContainer.m_oTextureGUID;
			m_rTexture     = a_rTextureContainer.m_rTexture;
		}
	}

	public Texture2DContainer( string a_rTextureGUID, bool a_bKeepTextureReference )
	{
		m_oTextureGUID = a_rTextureGUID;

		if( a_bKeepTextureReference )
		{
			m_rTexture = this.Texture;
		}
	}
	
#if UNITY_EDITOR
	public Texture2DContainer( Texture2D a_rTexture, bool a_bKeepTextureReference )
	{
		m_oTextureGUID = Uni2DEditorUtils.GetUnityAssetGUID( a_rTexture );

		if( a_bKeepTextureReference )
		{
			m_rTexture = a_rTexture;
		}
	}
#endif

	public static implicit operator Texture2D( Texture2DContainer a_rTextureContainer )
	{
		return !System.Object.ReferenceEquals( a_rTextureContainer, null ) ? a_rTextureContainer.Texture : null;
	}

	public override bool Equals( object a_rObject )
	{
		return this.Equals( a_rObject as Texture2DContainer );
	}

	public bool Equals( Texture2DContainer a_rAnotherContainer )
	{
		return !System.Object.ReferenceEquals( a_rAnotherContainer, null )
			&& string.Equals( m_oTextureGUID, a_rAnotherContainer.m_oTextureGUID )
			&& m_rTexture == a_rAnotherContainer.m_rTexture;
	}

	public override int GetHashCode( )
	{
		return m_oTextureGUID.GetHashCode( );
	}

	public static bool operator == ( Texture2DContainer a_rAContainer, Texture2DContainer a_rAnotherContainer )
	{
		if( System.Object.ReferenceEquals( a_rAContainer, a_rAnotherContainer ) )
		{
			return true;
		}
		else if( System.Object.ReferenceEquals( a_rAContainer, null ) || System.Object.ReferenceEquals( a_rAnotherContainer, null ) )
		{
			return false;
		}
		else
		{
			return a_rAContainer.Equals( a_rAnotherContainer );	
		}
	}

	public static bool operator != ( Texture2DContainer a_rAContainer, Texture2DContainer a_rAnotherContainer )
	{
		return !( a_rAContainer == a_rAnotherContainer );
	}
}
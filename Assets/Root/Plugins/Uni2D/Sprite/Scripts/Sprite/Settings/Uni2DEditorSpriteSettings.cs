using UnityEngine;

using SpriteRenderMesh = Uni2DSprite.SpriteRenderMesh;
using PivotType        = Uni2DSprite.PivotType;
using PhysicsMode       = Uni2DSprite.PhysicsMode;
using CollisionType    = Uni2DSprite.CollisionType;

/*
 * Uni2DEditorSpriteSettings
 * 
 * Contains all the settings of an Uni2DSprite.
 * Useful to extract, copy or check the parameters of an Uni2DSprite.
 * 
 * Editor use only.
 * 
 */
[System.Serializable]
public class Uni2DEditorSpriteSettings
{
	
	///// Texture /////
	// The sprite texture used to build the mesh and used by the quad game object
	public Texture2DContainer textureContainer = null;	// Non-interactive parameter, essential
	// The texture atlas
	public Uni2DTextureAtlas atlas = null;	// Interactive parameter
	
	// The shared material
	public Material sharedMaterial = null;

	///// Sprite /////
	// Sprite/physic mesh pivot type
	// Scale of the mesh and quad
	public float spriteScale = 1.0f;	// Interactive parameter

	// The render mesh used by the sprite
	public SpriteRenderMesh renderMesh = SpriteRenderMesh.Quad;	// Non-interactive parameter

	// if sprite mesh mode == Mesh, should it use
	// the same settings as physics?
	public bool usePhysicBuildSettings = false;

	public PivotType pivotType = PivotType.MiddleCenter;	// Interactive parameter
	// Sprite/physic mesh pivot coords
	public Vector2 pivotCustomCoords = Vector2.zero;	// Interactive parameter
	// Sprite quad mesh vertex color
	public Color vertexColor = Color.white;			// Interactive parameter

	///// Physic: none by default /////
	// The sprite physic modes
	public PhysicsMode physicsMode     = PhysicsMode.NoPhysics;	// Non-interactive parameter
	// The collision type modes
	public CollisionType collisionType = CollisionType.Convex;	// Non-interactive parameter
	// Set the collider as kinematic?
	public bool isKinematic            = false;					// Interactive parameter

	///// Polygonization /////
	// The alpha threshold the sprite contour will be extracted and polygonized
	public float alphaCutOff            = 0.75f;	// Non-interactive parameter, essential
	// Depth of the mesh to build
	public float extrusionDepth         = 0.5f;		// Interactive parameter, essential
	// The accuracy the sprite contour is simplified and polygonized 
	public float polygonizationAccuracy = 5.0f;		// Non-interactive parameter, essential
	// Whether or not holes is taken in account
	public bool polygonizeHoles         = false;	// Non-interactive parameter


	///// Sprite render mesh polygonization /////
	public float renderMeshAlphaCutOff            = 0.75f;
	public float renderMeshPolygonizationAccuracy = 5.0f;
	public bool renderMeshPolygonizeHoles 		  = false;

	// The grid horizontal/vertical sub-divs
	public int renderMeshGridHorizontalSubDivs = 5;	// Interactive
	public int renderMeshGridVerticalSubDivs   = 5;	// Interactive
	
	///// Skeletal smooth binding settings /////
	public SkinQuality skinQuality    = SkinQuality.Bone4;
	public float boneInfluenceFalloff = 4.0f;

	///// Min / max values /////
	public const float ScaleMin                  = 1e-1f;	// No max limit
	public const float ExtrusionMin              = 1e-1f;	// No max limit
	public const float AlphaCutOffMin            = 0.0f;
	public const float AlphaCutOffMax            = 1.0f;
	public const float PolygonizationAccuracyMin = 1.0f;
	public const float PolygonizationAccuracyMax = 256.0f;
	public const float BoneInfluenceFalloffMin   = 0.01f;	// No max limit
	public const int RenderMeshGridSubDivsMin    = 0;
	public const int RenderMeshGridSubDivsMax    = 253;		// Mesh vertex buffer is 16 bits. Grid vertex count is (HSub + 2) * (VSub + 2), so capping to 253

	// Computes the sprite width according to texture width and scale factor
	// Returns 0.0f is no texture assigned
	public float SpriteWidth
	{
		get
		{
			if( textureContainer != null )
			{
				Texture2D rTexture = textureContainer.Texture;
				if( rTexture != null )
				{
					return rTexture.width * this.ScaleFactor;
				}
			}

			return 0.0f;
		}
	}

	// Computes the sprite height according to texture height and scale factor
	// Returns 0.0f if no texture assigned
	public float SpriteHeight
	{
		get
		{
			if( textureContainer != null )
			{
				Texture2D rTexture = textureContainer.Texture;
				if( rTexture != null )
				{
					return rTexture.height * this.ScaleFactor;
				}
			}

			return 0.0f;
		}
	}

	// Returns the scale factor used by Uni2D
	// Computed from the given sprite scale and a Uni2D-defined unit constant
	public float ScaleFactor
	{
		get
		{
			return spriteScale * Uni2DSpriteUtils.mc_fSpriteUnitToUnity;
		}
	}

	public Vector2 ScaledPivotCoords
	{
		get
		{
			return textureContainer != null && textureContainer.Texture != null
			? this.ScaleFactor * Uni2DSpriteUtils.ComputePivotCoords( textureContainer.Texture.width, this.textureContainer.Texture.height, this.pivotType, this.pivotCustomCoords )
			: Vector2.zero;
		}
	}

	public Vector2 PivotCoords
	{
		get
		{
			return textureContainer != null
				? Uni2DSpriteUtils.ComputePivotCoords( this.textureContainer.Texture, this.pivotType, this.pivotCustomCoords )
				: Vector2.zero;
		}
	}

	public int BoneInfluence
	{
		get
		{
			switch( skinQuality )
			{
				case SkinQuality.Bone1:	return 1;
				case SkinQuality.Bone2:	return 2;
				case SkinQuality.Bone4:	return 4;
				
				case SkinQuality.Auto:
				default:
				{
					switch( QualitySettings.blendWeights )
					{
						default:
						case BlendWeights.OneBone:		return 1;
						case BlendWeights.TwoBones:		return 2;
						case BlendWeights.FourBones:	return 4;
					}
				}
			}
		}
	}
	
#if UNITY_EDITOR
	
	// Default constructor
	public Uni2DEditorSpriteSettings( )
	{
		// Nothing.
	}
	
	// Creates a new sprite settings with the given texture.
	// If the a_bPhysic argument is set to true, the settings will
	// depict a physical sprite
	public Uni2DEditorSpriteSettings( Texture2D a_rTexture2D, bool a_bPhysic = false )
	{
		this.textureContainer = new Texture2DContainer( a_rTexture2D, true );
		
		atlas = Uni2DEditorUtils.FindFirstTextureAtlas( textureContainer.GUID );
		
		// Set physic mode accordingly to given argument
		this.physicsMode = a_bPhysic
			? PhysicsMode.Dynamic
			: PhysicsMode.NoPhysics;
	}

	// Deep copy constructor
	public Uni2DEditorSpriteSettings( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		this.textureContainer       = new Texture2DContainer( a_rSpriteSettings.textureContainer );	// Deep copy
		this.atlas                  = a_rSpriteSettings.atlas;
		this.sharedMaterial			= a_rSpriteSettings.sharedMaterial;
		
		this.renderMesh             = a_rSpriteSettings.renderMesh;
		this.usePhysicBuildSettings = a_rSpriteSettings.usePhysicBuildSettings;
		
		this.spriteScale            = a_rSpriteSettings.spriteScale;
		this.pivotType				= a_rSpriteSettings.pivotType;
		this.pivotCustomCoords		= a_rSpriteSettings.pivotCustomCoords;
		this.vertexColor            = a_rSpriteSettings.vertexColor;
		this.physicsMode            = a_rSpriteSettings.physicsMode;
		this.collisionType          = a_rSpriteSettings.collisionType;
		this.isKinematic            = a_rSpriteSettings.isKinematic;
	
		this.renderMeshAlphaCutOff            = a_rSpriteSettings.renderMeshAlphaCutOff;
		this.renderMeshPolygonizationAccuracy = a_rSpriteSettings.renderMeshPolygonizationAccuracy;
		this.renderMeshPolygonizeHoles = a_rSpriteSettings.renderMeshPolygonizeHoles;
	
		this.alphaCutOff            = a_rSpriteSettings.alphaCutOff;
		this.extrusionDepth         = a_rSpriteSettings.extrusionDepth;
		this.polygonizationAccuracy = a_rSpriteSettings.polygonizationAccuracy;
		this.polygonizeHoles        = a_rSpriteSettings.polygonizeHoles;

		this.renderMeshGridHorizontalSubDivs = a_rSpriteSettings.renderMeshGridHorizontalSubDivs;
		this.renderMeshGridVerticalSubDivs   = a_rSpriteSettings.renderMeshGridVerticalSubDivs;

		this.skinQuality          = a_rSpriteSettings.skinQuality;
		this.boneInfluenceFalloff = a_rSpriteSettings.boneInfluenceFalloff;
	}
	
	// Checks if the most essential settings are valid
	public bool AreSettingsValid( )
	{
		return textureContainer != null
			&& textureContainer.Texture != null
			&& Uni2DEditorSpriteSettings.ScaleMin     <= spriteScale
			&& Uni2DEditorSpriteSettings.ExtrusionMin <= extrusionDepth
			
			// Alpha cut-off min / max
			&& Uni2DEditorSpriteSettings.AlphaCutOffMin <= alphaCutOff
				&& alphaCutOff <= Uni2DEditorSpriteSettings.AlphaCutOffMax

			// Polygonization min / max
			&& Uni2DEditorSpriteSettings.PolygonizationAccuracyMin <= polygonizationAccuracy
				&& polygonizationAccuracy <= Uni2DEditorSpriteSettings.PolygonizationAccuracyMax

			// Grid sub-divs min/max
			&& Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMin <= renderMeshGridHorizontalSubDivs
			&& Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMin <= renderMeshGridVerticalSubDivs
				&& renderMeshGridHorizontalSubDivs <= Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMax
				&& renderMeshGridVerticalSubDivs   <= Uni2DEditorSpriteSettings.RenderMeshGridSubDivsMax

			// Bone influence
			&& Uni2DEditorSpriteSettings.BoneInfluenceFalloffMin <= boneInfluenceFalloff;
	}

	// Returns true if and only if a_rSpriteParameters is not null and if interactive parameters are equal
	public bool InteractiveParametersEquals( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		return a_rSpriteSettings           != null
			&& this.usePhysicBuildSettings == a_rSpriteSettings.usePhysicBuildSettings
			&& this.pivotType              == a_rSpriteSettings.pivotType
			&& this.pivotCustomCoords      == a_rSpriteSettings.pivotCustomCoords
			&& this.spriteScale            == a_rSpriteSettings.spriteScale
			&& this.extrusionDepth         == a_rSpriteSettings.extrusionDepth
			&& this.atlas                  == a_rSpriteSettings.atlas
			&& this.sharedMaterial		   == a_rSpriteSettings.sharedMaterial
			&& this.vertexColor            == a_rSpriteSettings.vertexColor
			&& this.isKinematic            == a_rSpriteSettings.isKinematic
			&& this.skinQuality            == a_rSpriteSettings.skinQuality
			&& this.boneInfluenceFalloff   == a_rSpriteSettings.boneInfluenceFalloff;
	}

	// Returns true if and only if a_rSpriteSettings is not null and if non-interactive parameters are equal
	public bool NonInteractiveParametersEquals( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		return a_rSpriteSettings                     != null
			&& this.physicsMode                      == a_rSpriteSettings.physicsMode
			&& this.collisionType                    == a_rSpriteSettings.collisionType
			&& this.alphaCutOff                      == a_rSpriteSettings.alphaCutOff
			&& this.polygonizationAccuracy           == a_rSpriteSettings.polygonizationAccuracy
			&& this.polygonizeHoles                  == a_rSpriteSettings.polygonizeHoles
			&& this.textureContainer                 == a_rSpriteSettings.textureContainer
			&& this.renderMesh                       == a_rSpriteSettings.renderMesh
			&& this.renderMeshAlphaCutOff            == a_rSpriteSettings.renderMeshAlphaCutOff
			&& this.renderMeshPolygonizationAccuracy == a_rSpriteSettings.renderMeshPolygonizationAccuracy
			&& this.renderMeshPolygonizeHoles == a_rSpriteSettings.renderMeshPolygonizeHoles
			&& this.renderMeshGridHorizontalSubDivs  == a_rSpriteSettings.renderMeshGridHorizontalSubDivs
			&& this.renderMeshGridVerticalSubDivs    == a_rSpriteSettings.renderMeshGridVerticalSubDivs;
	}

//	// Returns true if the given settings imply to rebuild the sprite quad
//	public bool DoNewSettingsImplyToRebuildQuad( Uni2DEditorSpriteSettings a_rSpriteSettings )
//	{
//		return this.InteractiveParametersEquals( a_rSpriteSettings ) == false
//			|| this.spriteTexture != a_rSpriteSettings.spriteTexture;
//	}

	// Returns true if the given settings imply to rebuild the sprite physic
	public bool DoNewSettingsImplyToRebuildPhysics( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		return this.NonInteractiveParametersEquals( a_rSpriteSettings ) == false
			|| this.pivotType         != a_rSpriteSettings.pivotType
			|| ( this.pivotType       == PivotType.Custom
					&& this.pivotCustomCoords != a_rSpriteSettings.pivotCustomCoords )
			|| this.spriteScale       != a_rSpriteSettings.spriteScale;
	}

	public bool IsAtlasInvalid( )
	{
		return atlas != null && atlas.Contains( textureContainer.GUID ) == false;
	}
	
	public bool ShouldUseAtlas( )
	{
		return atlas != null && atlas.Contains( textureContainer.GUID );
	}

	public bool ShouldUseAtlas( string a_rTextureGUID )
	{
		return atlas != null && atlas.Contains( a_rTextureGUID );
	}

	public bool ShouldUseAtlas( Texture2D a_rTextureToCheck )
	{
		return atlas != null && atlas.Contains( a_rTextureToCheck );
	}

	// Checks equality between 2 Uni2DEditorSpriteSettings instances.
	// True if a_rObject is not null, is an instance of Uni2DEditorSpriteSettings
	// and if members are equal
	public override bool Equals( System.Object a_rObject )
	{
		return this.Equals( a_rObject as Uni2DEditorSpriteSettings );
	}

	// Same as above but with an Uni2DEditorSpriteSettings argument.
	public bool Equals( Uni2DEditorSpriteSettings a_rSpriteSettings )
	{
		return this.InteractiveParametersEquals( a_rSpriteSettings )
			&& this.NonInteractiveParametersEquals( a_rSpriteSettings );
	}

	// Avoid warnings
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}
#endif
}

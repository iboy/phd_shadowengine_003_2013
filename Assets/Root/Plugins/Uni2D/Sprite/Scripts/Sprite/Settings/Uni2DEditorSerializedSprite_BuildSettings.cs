#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

using SpriteRenderMesh = Uni2DSprite.SpriteRenderMesh;
using PhysicsMode    = Uni2DSprite.PhysicsMode;
using CollisionType = Uni2DSprite.CollisionType;

public class Uni2DEditorSerializedSprite_BuildSettings
{
	private SerializedObject m_rSerializedSprite;

	// Physic
	public SerializedSetting<PhysicsMode> serializedPhysicsMode       = null;
	public SerializedSetting<CollisionType> serializedCollisionType  = null;

	// Polygonization
	public SerializedSetting<float> serializedAlphaCutOff            = null;
	public SerializedSetting<float> serializedPolygonizationAccuracy = null;
	public SerializedSetting<bool> serializedPolygonizeHoles         = null;

	public SerializedSetting<bool> serializedUsePhysicBuildSettings = null;
	public SerializedSetting<SpriteRenderMesh> serializedRenderMesh = null;

	public SerializedSetting<float> serializedRenderMeshAlphaCutOff            = null;
	public SerializedSetting<float> serializedRenderMeshPolygonizationAccuracy = null;
	public SerializedSetting<bool> serializedRenderMeshPolygonizeHoles = null;

	public SerializedSetting<bool> serializedIsPhysicsDirty = null;

	public SerializedSetting<int> serializedRenderMeshGridHorizontalSubDivs = null;
	public SerializedSetting<int> serializedRenderMeshGridVerticalSubDivs   = null;

	public SerializedObject SerializedSprite
	{
		get
		{
			return m_rSerializedSprite;
		}
	}

	public Uni2DEditorSerializedSprite_BuildSettings( SerializedObject a_rSerializedSprite )
	{
		if( a_rSerializedSprite != null )
		{
			m_rSerializedSprite     = a_rSerializedSprite;
		
			serializedPhysicsMode    = new SerializedSetting<PhysicsMode>(    a_rSerializedSprite, "m_rSpriteSettings.physicsMode" );
			serializedCollisionType = new SerializedSetting<CollisionType>( a_rSerializedSprite, "m_rSpriteSettings.collisionType" );

			serializedAlphaCutOff            = new SerializedSetting<float>( a_rSerializedSprite, "m_rSpriteSettings.alphaCutOff" );
			serializedPolygonizationAccuracy = new SerializedSetting<float>( a_rSerializedSprite, "m_rSpriteSettings.polygonizationAccuracy" );
			serializedPolygonizeHoles        = new SerializedSetting<bool>(  a_rSerializedSprite, "m_rSpriteSettings.polygonizeHoles" );

			serializedRenderMesh = new SerializedSetting<SpriteRenderMesh>( a_rSerializedSprite, "m_rSpriteSettings.renderMesh" );
			serializedUsePhysicBuildSettings = new SerializedSetting<bool>( a_rSerializedSprite, "m_rSpriteSettings.usePhysicBuildSettings" );

			serializedRenderMeshAlphaCutOff = new SerializedSetting<float>( a_rSerializedSprite, "m_rSpriteSettings.renderMeshAlphaCutOff" );
			serializedRenderMeshPolygonizationAccuracy = new SerializedSetting<float>( a_rSerializedSprite, "m_rSpriteSettings.renderMeshPolygonizationAccuracy" );
			serializedRenderMeshPolygonizeHoles = new SerializedSetting<bool>( a_rSerializedSprite, "m_rSpriteSettings.renderMeshPolygonizeHoles" );

			serializedIsPhysicsDirty = new SerializedSetting<bool>( a_rSerializedSprite, "isPhysicsDirty" );

			serializedRenderMeshGridHorizontalSubDivs = new SerializedSetting<int>( a_rSerializedSprite, "m_rSpriteSettings.renderMeshGridHorizontalSubDivs" );
			serializedRenderMeshGridVerticalSubDivs   = new SerializedSetting<int>( a_rSerializedSprite, "m_rSpriteSettings.renderMeshGridVerticalSubDivs" );
		}
	}

	public void ApplyValues( )
	{
		serializedPhysicsMode.Apply( );
		serializedCollisionType.Apply( );

		serializedAlphaCutOff.Apply( );
		serializedPolygonizationAccuracy.Apply( );
		serializedPolygonizeHoles.Apply( );

		serializedRenderMesh.Apply( );
		serializedUsePhysicBuildSettings.Apply( );

		serializedRenderMeshAlphaCutOff.Apply( );
		serializedRenderMeshPolygonizationAccuracy.Apply( );
		serializedRenderMeshPolygonizeHoles.Apply( );

		serializedIsPhysicsDirty.Apply( );

		serializedRenderMeshGridHorizontalSubDivs.Apply( );
		serializedRenderMeshGridVerticalSubDivs.Apply( );
	}

	public bool AreValuesModified( )
	{
		return serializedPhysicsMode.IsDefined
			|| serializedCollisionType.IsDefined
			|| serializedAlphaCutOff.IsDefined
			|| serializedPolygonizationAccuracy.IsDefined
			|| serializedPolygonizeHoles.IsDefined
			|| serializedRenderMesh.IsDefined
			|| serializedUsePhysicBuildSettings.IsDefined
			|| serializedRenderMeshAlphaCutOff.IsDefined
			|| serializedRenderMeshPolygonizationAccuracy.IsDefined
			|| serializedRenderMeshPolygonizeHoles.IsDefined
			|| serializedIsPhysicsDirty.IsDefined
			|| serializedRenderMeshGridHorizontalSubDivs.IsDefined
			|| serializedRenderMeshGridVerticalSubDivs.IsDefined;
	}

	public void RevertValues( )
	{
		serializedPhysicsMode.Revert( );
		serializedCollisionType.Revert( );

		serializedAlphaCutOff.Revert( );
		serializedPolygonizationAccuracy.Revert( );
		serializedPolygonizeHoles.Revert( );

		serializedRenderMesh.Revert( );
		serializedUsePhysicBuildSettings.Revert( );	
	
		serializedRenderMeshAlphaCutOff.Revert( );
		serializedRenderMeshPolygonizationAccuracy.Revert( );
		serializedRenderMeshPolygonizeHoles.Revert( );

		serializedIsPhysicsDirty.Revert( );

		serializedRenderMeshGridHorizontalSubDivs.Revert( );
		serializedRenderMeshGridVerticalSubDivs.Revert( );

	}

	public void Generate( )
	{
		foreach( Object rTargetObject in m_rSerializedSprite.targetObjects )
		{
			Uni2DSprite rTargetSprite = (Uni2DSprite) rTargetObject;

			rTargetSprite.Regenerate( );
		}
	}
}
#endif

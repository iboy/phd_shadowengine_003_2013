using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.IO;
using System.Linq;

using BoneEditMode = Uni2DEditorSmoothBindingUtils.BoneEditMode;
#endif

[AddComponentMenu( "Uni2D/Sprite/Uni2DSprite" )]
[ExecuteInEditMode( )]
public class Uni2DSprite : MonoBehaviour 
{	
	public enum SpriteRenderMesh
	{
		Quad,			// Classic quad
		TextureToMesh,	// Hull
		Grid			// Parametrized grid
	};

	public enum PhysicsMode
	{
		NoPhysics,
		Static,
		Dynamic,
	};
	
	public enum CollisionType
	{
		Convex,
		Concave,
		Compound,
	};

	public enum PivotType
	{
		Custom,
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	// The sprite settings
	[HideInInspector]
	[SerializeField]
	private Uni2DEditorSpriteSettings m_rSpriteSettings = new Uni2DEditorSpriteSettings( );

	// The sprite generated data after being built
	[HideInInspector]
	[SerializeField]
	private Uni2DEditorSpriteData m_rSpriteData = new Uni2DEditorSpriteData( );

	private Uni2DSpriteRuntimeData m_rSpriteRuntimeData = new Uni2DSpriteRuntimeData( );
	
	private Mesh m_rEditableRenderMesh;
	
	// The editable render mesh
	// The first call create a copy of the render mesh that can be modified
	public Mesh EditableRenderMesh
	{
		get
		{
			if(Application.isPlaying)
			{
				if(m_rEditableRenderMesh == null)
				{
					MeshFilter rMeshFilter = GetComponent<MeshFilter>();
					m_rEditableRenderMesh = (rMeshFilter!=null)?rMeshFilter.mesh:null;
				}
				return m_rEditableRenderMesh;
			}
			else
			{
				return null;
			}
		}
	}
	
	public Uni2DEditorSpriteSettings SpriteSettings
	{
		get
		{
			return m_rSpriteSettings;
		}
	}

	public Uni2DEditorSpriteData SpriteData
	{
		get
		{
			return m_rSpriteData;
		}
	}

	public Uni2DSpriteRuntimeData RuntimeData
	{
		get
		{
			return m_rSpriteRuntimeData;
		}
	}

	public Color32 VertexColor
	{
		get
		{
			return m_rSpriteSettings.vertexColor;
		}
		set
		{
			Mesh rInstanciatedMesh = null;
			MeshFilter rMeshFilter = this.GetComponent<MeshFilter>( );
			if( rMeshFilter != null )
			{
				rInstanciatedMesh = rMeshFilter.mesh;

				if( rInstanciatedMesh != null )
				{
					Uni2DSpriteUtils.UpdateMeshVertexColor( rInstanciatedMesh, value );
					m_rSpriteSettings.vertexColor = value;

					SkinnedMeshRenderer rSkinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>( );
					if( rSkinnedMeshRenderer != null && rSkinnedMeshRenderer.sharedMesh != rMeshFilter.sharedMesh )
					{
						rSkinnedMeshRenderer.sharedMesh = rMeshFilter.sharedMesh;
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	public Uni2DSmoothBindingBone[ ] Bones
	{
		get
		{
			Uni2DSmoothBindingBone[ ] oChildBones = this.GetComponentsInChildren<Uni2DSmoothBindingBone>( false );
			List<Uni2DSmoothBindingBone> oBones = new List<Uni2DSmoothBindingBone>( oChildBones.Length );
			foreach( Uni2DSmoothBindingBone rBone in oChildBones )
			{
				if( rBone.Sprite == this )
				{
					oBones.Add( rBone );
				}
			}

			return oBones.ToArray( );
		}
	}
#endif

	// Up to date sprite mesh state: if the texture associated with this sprite mesh has been modified,
	// the sprite mesh of this texture may not correspond and needs to be rebuild
	public bool isPhysicsDirty = false;
	
	// Atlas generation ID
	public string atlasGenerationID = "";
	
	// Texture import guid
	public string m_oTextureImportGUID = "";
	
	// The animation
	public Uni2DSpriteAnimation spriteAnimation = new Uni2DSpriteAnimation( );
	
	// Current frame texture Width
	private int m_iCurrentFrameTextureWidth = -1;
	
	// Current frame texture Height
	private int m_iCurrentFrameTextureHeight = -1;
	
	// Frame atlas
	private Texture2DContainer m_rCurrentFrameTextureContainer;
	
	// Frame atlas
	private Uni2DTextureAtlas m_rCurrentFrameAtlas;
	
	// the duplicata marker
	[SerializeField]
	[HideInInspector]
	private DuplicataMarker m_oDuplicataMarker;

	// Creation in progress
	private static bool ms_bCreationInProgress;

	// Set main frame
	public void ResetToMainFrame( )
	{
		SetSpriteFrame( m_rSpriteSettings.textureContainer,
			m_rSpriteSettings.atlas,
			(int) m_rSpriteData.spriteWidth,
			(int) m_rSpriteData.spriteHeight );

		// When resetting animation, reset sprite material and delete
		// the cloned material (runtime data)
		if( renderer.sharedMaterial == m_rSpriteRuntimeData.animationMaterial )
		{
			renderer.sharedMaterial = m_rSpriteData.renderMeshMaterial;
		}
		Material.Destroy( m_rSpriteRuntimeData.animationMaterial );

	}
	
	// Set the frame draw by the sprite
	public void SetFrame(Uni2DAnimationFrame a_rFrame)
	{
		SetSpriteFrame(a_rFrame.textureContainer, a_rFrame.atlas, a_rFrame.TextureWidth, a_rFrame.TextureHeight);
	}
	
	// Set the frame draw by the sprite
	private void SetSpriteFrame(Texture2DContainer a_rTextureContainer, Uni2DTextureAtlas a_rTextureAtlas, int a_iWidth, int a_iHeight)
	{
		bool a_bUpdateMesh = false;
		if(m_iCurrentFrameTextureWidth != a_iWidth || m_iCurrentFrameTextureHeight != a_iHeight )
		{
			m_iCurrentFrameTextureWidth  = a_iWidth;
			m_iCurrentFrameTextureHeight = a_iHeight;
			a_bUpdateMesh = true;
		}
		
		bool a_bUpdateUV = false;
		if(m_rCurrentFrameAtlas != a_rTextureAtlas || (m_rCurrentFrameAtlas != null && m_rCurrentFrameTextureContainer != a_rTextureContainer))
		{
			m_rCurrentFrameTextureContainer = a_rTextureContainer;
			m_rCurrentFrameAtlas            = a_rTextureAtlas;
			a_bUpdateUV = true;
		}
		
		Uni2DSpriteUtils.SetSpriteFrame(this, a_rTextureContainer, a_rTextureAtlas, a_iWidth, a_iHeight, a_bUpdateMesh, a_bUpdateUV);
	}
	
	// Awake
	private void Awake()
	{
#if UNITY_EDITOR
		if( Application.isPlaying )
#endif
		{
			m_rCurrentFrameAtlas            = m_rSpriteSettings.atlas;
			m_rCurrentFrameTextureContainer = m_rSpriteSettings.textureContainer;
			m_iCurrentFrameTextureWidth     = (int)m_rSpriteData.spriteWidth;
			m_iCurrentFrameTextureHeight    = (int)m_rSpriteData.spriteHeight;
			spriteAnimation.Start( this );
		}
		
#if UNITY_EDITOR
		// Is a prefab instance
		else if( IsADuplicate( ) )
		{
			//Debug.Log("Duplicate");
			Uni2DEditorResourceCopyUtils.DuplicateResources( this );
		}
#endif

		// Create the duplicata marker if needed
		if(m_oDuplicataMarker == null || m_oDuplicataMarker.IsADuplicate(this))
		{
			m_oDuplicataMarker = ScriptableObject.CreateInstance<DuplicataMarker>();
			m_oDuplicataMarker.Create(this);
		}
	}
	
	// On Destroy
	private void OnDestroy()
	{
		if(m_oDuplicataMarker != null && m_oDuplicataMarker.IsADuplicate(this) == false)
		{
			DestroyImmediate(m_oDuplicataMarker);
		}
		
		if(m_rEditableRenderMesh != null)
		{
			Destroy(m_rEditableRenderMesh);
		}
	}

	// Update
	private void Update( )
	{
#if UNITY_EDITOR
		if(Application.isPlaying)
#endif
		{
			spriteAnimation.Update(Time.deltaTime);
		}
		
#if UNITY_EDITOR
		else if(AnimationUtility.InAnimationMode() == false)
		{
			CheckIfAtlasDeleted( );

			CheckIfMaterialChange( );

			CheckIfTextureChange( );

			CheckIfAnimationClipsDeleted( );
			
			if( PrefabUtility.GetPrefabObject( gameObject ) != null )
			{
				//Debug.Log( "Break" );
				this.BreakResourcesConnection( );
			}
			else if( this.NeedToBeRebuild( ) )
			{	
				//Debug.Log("Rebuild");
				this.Regenerate( true );
			}
			else if( m_rSpriteData.HasSharedResources( ) )
			{
				//Debug.Log("Break prefab resources connection");
				this.Regenerate( true );	// Force the whole regeneration to break connection
			}
			else if( this.HasAtlasBeenRefreshed( ) )
			{
				//Debug.LogWarning("Update UVs");
				this.UpdateUvs( );
			}
			else if( m_rSpriteSettings.IsAtlasInvalid( ) )
			{
				//Debug.LogWarning( "Atlas invalid");
				this.SetDefaultAtlas( );
				this.UpdateUvs( );
			}

			if( this.GetComponent<SkinnedMeshRenderer>( ) != null
				&& Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode == Uni2DEditorSmoothBindingUtils.BoneEditMode.Posing
				&& this.transform == Selection.activeTransform )
			{
				this.UpdatePosing( );
			}
		}
#endif
	}
	
#if UNITY_EDITOR
	
	// Create: use this when creating an Uni2DSprite component in edit mode
	public static Uni2DSprite Create(GameObject a_oSpriteGameObject)
	{
		ms_bCreationInProgress = true;
		Uni2DSprite rSprite = a_oSpriteGameObject.AddComponent<Uni2DSprite>();
		ms_bCreationInProgress = false;
		
		return rSprite;
	}
	
	private void CheckIfAtlasDeleted( )
	{
		if( m_rSpriteSettings.atlas == null && !string.IsNullOrEmpty( atlasGenerationID ) )
		{
			this.RegenerateInteractiveData( );
		}
	}

	private void CheckIfMaterialChange( )
	{
		Material rCurrentMaterial = renderer != null ? renderer.sharedMaterial : null;
		Texture2D rCurrentTexture = ( rCurrentMaterial != null ? rCurrentMaterial.mainTexture : null ) as Texture2D;

		Texture2D rSpriteTexture = m_rSpriteSettings.textureContainer;
		
		Material rUsedMaterial = m_rSpriteSettings.atlas != null ? m_rSpriteSettings.atlas.GetMaterial( rSpriteTexture ) : null;
		if(rUsedMaterial == null)
		{
			rUsedMaterial = m_rSpriteData.renderMeshMaterial;
			if(rUsedMaterial == null)
			{
				rUsedMaterial = m_rSpriteData.generatedMaterial;
			}
		}

		// If no material
		// or not sprite's/atlas' material
		// or not sprite's/atlas' texture
		// or texture contained by current atlas (assuming it's not sprite's/atlas' texture)...
		if( (DragAndDrop.visualMode == DragAndDropVisualMode.None || DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)	// If dragging material from editor GUI to the sprite, don't do anything before the drag is over
			&& ( rUsedMaterial != rCurrentMaterial
			|| ( rSpriteTexture != rCurrentTexture && m_rSpriteSettings.atlas == null) ) )
		{
			OnMaterialChange( );
		}
	}

	private void CheckIfAnimationClipsDeleted( )
	{
		for( int iClipIndex = 0, iClipCount = this.spriteAnimation.ClipCount; iClipIndex < iClipCount; ++iClipIndex )
		{
			if( this.spriteAnimation.Clips[ iClipIndex ] == null )
			{
				this.spriteAnimation.CleanDeletedAnimationClips( );
				break;
			}
		}
	}

	// Sets the sprite settings according to the new material (assuming it's new)
	private void OnMaterialChange( )
	{
		Material rNewMaterial = renderer.sharedMaterial;

		if( rNewMaterial != null )
		{
			Texture rNewTexture = rNewMaterial.mainTexture;
	
			if(EditorUtility.IsPersistent(rNewMaterial))
			{
				m_rSpriteSettings.sharedMaterial = rNewMaterial;
			}
			m_rSpriteData.renderMeshMaterial = rNewMaterial;
	
			if(rNewTexture == null)
			{
				rNewTexture = m_rSpriteSettings.textureContainer;
				rNewMaterial.mainTexture = rNewTexture;
			}
			
			if( rNewTexture != null )
			{
				m_rSpriteSettings.atlas = null;
				
				// Material & texture not null =>
				// Take in account this material, its texture and regen the sprite mesh
				m_rSpriteSettings.textureContainer = new Texture2DContainer( (Texture2D) rNewTexture, false );
				
				this.Regenerate( true );
				return;
			}
		}

		// No material and/or no texture => regenerate/reset it with the current texture
		Uni2DEditorSpriteBuilderUtils.GenerateSpriteMatFromSettings( m_rSpriteSettings, this );
	}
	
	// On a texture change
	private void OnTextureChange(string a_oNewTextureImportGUID)
	{
		//Debug.Log("On texture change: " + this.gameObject.name );

		Texture2D rSpriteTexture = m_rSpriteSettings.textureContainer;

		// Update sprite size
		if( rSpriteTexture != null)
		{
			string oTexturePath = AssetDatabase.GetAssetPath( rSpriteTexture.GetInstanceID( ) );
			TextureImporter rTextureImporter = TextureImporter.GetAtPath( oTexturePath ) as TextureImporter;
			if(rTextureImporter != null)
			{	
				TextureImporterSettings oTextureImporterSettings = Uni2DEditorSpriteBuilderUtils.TextureProcessingBegin(rTextureImporter);
		
				this.UpdateAccordinglyToTextureChange( a_oNewTextureImportGUID );
				//Uni2DEditorUtilsSpriteBuilder.DoUpdateAllSceneSpritesAccordinglyToTextureChange( rSpriteTexture, a_oNewTextureImportGUID);
				
				Uni2DEditorSpriteBuilderUtils.TextureProcessingEnd(rTextureImporter, oTextureImporterSettings);
		
				EditorUtility.UnloadUnusedAssets( );
			}
		}
	}
	
	// Update for a texture change
	public void UpdateAccordinglyToTextureChange( string a_oNewTextureImportGUID )
	{
		//Debug.Log( "UpdateAccordinglyTo... current GUID: " + m_oTextureImportGUID + " / new:" + a_oNewTextureImportGUID, this.gameObject );
		if( m_oTextureImportGUID == a_oNewTextureImportGUID )
		{
			return;
		}

		// Regenerate sprite mesh
		Uni2DEditorSpriteBuilderUtils.GenerateSpriteMeshFromSettings( m_rSpriteSettings, this );

		// Texture change end
		m_oTextureImportGUID = a_oNewTextureImportGUID;
		if( m_rSpriteSettings.physicsMode != PhysicsMode.NoPhysics )
		{
			isPhysicsDirty = true;
		}
		
		EditorUtility.SetDirty( this.gameObject );
	}
	
	// Rebuild
	public void Regenerate( bool a_bForce = false )
	{
		if( EditorUtility.IsPersistent( this ) )
		{
			Uni2DEditorSpriteBuilderUtils.UpdateSpriteInResource( this, a_bForce );
		}
		else
		{
			// Force update/rebuild
			Uni2DEditorSpriteBuilderUtils.GenerateSpriteFromSettings( m_rSpriteSettings, this, a_bForce );
		}
		
		EditorUtility.SetDirty( this.gameObject );
 	}

	public void RegenerateInteractiveData( )
	{
		if( EditorUtility.IsPersistent( this ) )
		{
			Uni2DEditorSpriteBuilderUtils.UpdateSpriteInResource( this );
		}
		else
		{
			Uni2DEditorSpriteBuilderUtils.RegenerateInteractiveDataFromSettings( this );
		}
		EditorUtility.SetDirty( this.gameObject );
	}

	// Rebuild mesh in a batch
	// Useful when some sprites share the same texture (only one texture import)
	public void RebuildInABatch( bool a_bForce = false )
	{
		// RebuildInABatch purpose is only to reduce the time of
		// the awful-texture-imports-juggling currently performed to read the true texture dimensions.
		if( EditorUtility.IsPersistent( this ) )
		{
			Uni2DEditorSpriteBuilderUtils.UpdateSpriteInResourceInABatch( this );
		}
		else
		{
			Uni2DEditorSpriteBuilderUtils.GenerateSpriteFromSettings( m_rSpriteSettings, this, a_bForce, true );
		}
		EditorUtility.SetDirty( this.gameObject );
	}
	
	// Rebuild mesh
	public void UpdateUvs()
	{
		Material rSpriteMaterial = m_rSpriteData.renderMeshMaterial;

		// Handle the case where the atlas doesn't contains the texture anymore
		if( m_rSpriteSettings.ShouldUseAtlas( m_rSpriteSettings.textureContainer.GUID ) )
		{
			rSpriteMaterial   = m_rSpriteSettings.atlas.GetMaterial( m_rSpriteSettings.textureContainer.GUID );
			atlasGenerationID = m_rSpriteSettings.atlas.generationId;
		}
		else
		{
			m_rSpriteSettings.atlas = null;
			rSpriteMaterial = m_rSpriteData.renderMeshMaterial;
			atlasGenerationID = "";
		}

		// Update UVs
		Mesh rSpriteMesh = m_rSpriteData.renderMesh;
		rSpriteMesh.uv = Uni2DSpriteUtils.BuildUVs( m_rSpriteSettings.textureContainer,
			m_rSpriteData.renderMeshUVs,
			m_rSpriteSettings.atlas );

		// Update material
		Renderer rSpriteRender = this.renderer;
		if( rSpriteRender != null )
		{
			rSpriteRender.sharedMaterial = rSpriteMaterial;
		}

		EditorUtility.SetDirty(this);
	}
	
	// Save sprite as part of a prefab
	public void SaveSpriteAsPartOfAPrefab(string a_rPrefabResourcesFolderPath, string a_oPrefabResourcesFolderPath_Absolute, string a_oPrefabResourcesSubFolderName)
	{
		if( NeedToBeSaved( ) )
		{
			Regenerate( true );
		}
		SaveResources(a_rPrefabResourcesFolderPath, a_oPrefabResourcesFolderPath_Absolute, a_oPrefabResourcesSubFolderName);
	}
	
	// Need to be saved?
	// Used to check if the sprite is a prefab and losts all its data
	// (Unity sets all non-serialized data references to null when creating a prefab)
	public bool NeedToBeSaved()
	{
		return m_rSpriteData.renderMesh == null
			|| m_rSpriteData.renderMeshMaterial == null
			|| 
			(m_rSpriteData.meshCollidersList == null || m_rSpriteData.meshCollidersList.Contains(null))
			||
			(m_rSpriteData.meshColliderComponentsList == null || m_rSpriteData.meshColliderComponentsList.Contains(null));
	}
	
	// Before prefab post process
	public bool BeforePrefabPostProcess()
	{
		if(NeedToBeSaved())
		{
			return true;
		}
		else
		{
			// Check if all the sprite resources belong to the good folder
			if(IsResourcesInTheCorrectFolder() == false)
			{
				// Nullify the quad mesh to force to resave the prefab
				m_rSpriteData.renderMesh = null;
				
				return true;
			}
		}
		
		return false;
	}
	
	// Before prefab post process
	private bool IsResourcesInTheCorrectFolder()
	{
		string oResourceDirectory = Uni2DEditorSpriteBuilderUtils.GetPrefabResourcesDirectoryPathLocal(this.gameObject);
		if(AssetDatabase.GetAssetPath(m_rSpriteData.renderMesh).Contains(oResourceDirectory) == false)
		{
			return false;	
		}
		
		//if(AssetDatabase.GetAssetPath(m_rSpriteData.renderMeshMaterial).Contains(oResourceDirectory) == false)
		//{
		//	return false;	
		//}
		
		foreach(Mesh rMeshCollider in m_rSpriteData.meshCollidersList)
		{
			if(AssetDatabase.GetAssetPath(rMeshCollider).Contains(oResourceDirectory) == false)
			{
				return false;	
			}
		}
		
		return true;
	}
	
	// After build
	public void AfterBuild()
	{
		m_oTextureImportGUID = Uni2DEditorUtils.GetTextureImportGUID( m_rSpriteSettings.textureContainer );
	}
	
	// Check if dirty
	private void CheckIfTextureChange()
	{
		Texture2D rSpriteTexture = m_rSpriteSettings.textureContainer;
		if( rSpriteTexture != null)
		{
	 		string oTextureImportGUID = Uni2DEditorUtils.GetTextureImportGUID( rSpriteTexture );
			if(oTextureImportGUID != m_oTextureImportGUID)
			{	
				// On texture change
				OnTextureChange(oTextureImportGUID);
			}
		}
	}
	
	// Is a duplicate
	private bool IsADuplicate()
	{
		return ms_bCreationInProgress == false && (m_oDuplicataMarker != null && m_oDuplicataMarker.IsADuplicate(this));
	}
	
	// Save Mesh resources
	private void BreakResourcesConnection()
	{
		// Replace the prefab default material by a copy of the source instance (this sprite from wich the prefab has just been create) if the material is not persistent
		Material rGeneratedMaterialInstance = SpriteData.generatedMaterial;
		if(rGeneratedMaterialInstance != null)
		{
			Uni2DSprite rSpritePrefab = PrefabUtility.GetPrefabParent(this) as Uni2DSprite;
			Material rPrefabMaterial = rSpritePrefab.SpriteData.generatedMaterial;
			
			// If prefab material isn't created yet wait for it
			if(rPrefabMaterial == null)
			{
				return;
			}
			
			rPrefabMaterial.shader = rGeneratedMaterialInstance.shader;
			rPrefabMaterial.CopyPropertiesFromMaterial(rGeneratedMaterialInstance);
		}
		
		//Debug.Log("Break Resources");
		Uni2DEditorResourceCopyUtils.DuplicateResources(this , true);
		
		PrefabUtility.DisconnectPrefabInstance(gameObject);
	}
	
	// Save Mesh resources
	private void SaveResources(string a_oPrefabResourcesFolderPath, string a_oPrefabResourcesFolderPath_Absolute, string a_oPrefabResourcesSubFolderName)
	{
		// Save created asset names to clean the other assets next
		HashSet<string> oCreatedAssetPaths = new HashSet<string>();
		
		Mesh rSpriteMesh = m_rSpriteData.renderMesh;
		Material rGeneratedMaterial = m_rSpriteData.generatedMaterial;
		
		string oPrefabResourcesSubFolderPath = a_oPrefabResourcesFolderPath + "/" + a_oPrefabResourcesSubFolderName;
		string oPrefabResourcesSubFolderPath_Absolute = a_oPrefabResourcesFolderPath_Absolute + "/" + a_oPrefabResourcesSubFolderName;
		if(Directory.Exists(oPrefabResourcesSubFolderPath_Absolute) == false)
		{
			// Create folder
			string oPrefabResourcesSubFolderPathGUID = AssetDatabase.CreateFolder(a_oPrefabResourcesFolderPath, a_oPrefabResourcesSubFolderName);
			oPrefabResourcesSubFolderPath = AssetDatabase.GUIDToAssetPath(oPrefabResourcesSubFolderPathGUID);
		}
		oPrefabResourcesSubFolderPath += "/";
		
		// Import assets to database
		//AssetDatabase.StartAssetEditing();
		
		// Sprite mesh
		string oSpriteMeshAssetPath = oPrefabResourcesSubFolderPath + rSpriteMesh.name + ".asset";

		if( AssetDatabase.Contains( rSpriteMesh ) )
		{
			AssetDatabase.CopyAsset( AssetDatabase.GetAssetPath( rSpriteMesh ), oSpriteMeshAssetPath );
		}
		else
		{
			AssetDatabase.CreateAsset( rSpriteMesh, oSpriteMeshAssetPath );
			AssetDatabase.ImportAsset( oSpriteMeshAssetPath, ImportAssetOptions.ForceSynchronousImport );
		}
		oCreatedAssetPaths.Add(oSpriteMeshAssetPath);

		// Generated material
		if(rGeneratedMaterial != null)
		{
			string oGeneratedMaterialAssetPath = oPrefabResourcesSubFolderPath + rGeneratedMaterial.name + ".mat";
			if( AssetDatabase.Contains( rGeneratedMaterial ) )
			{
				AssetDatabase.CopyAsset( AssetDatabase.GetAssetPath( rGeneratedMaterial ), oGeneratedMaterialAssetPath );
			}
			else
			{
				AssetDatabase.CreateAsset( rGeneratedMaterial, oGeneratedMaterialAssetPath );
				AssetDatabase.ImportAsset( oGeneratedMaterialAssetPath, ImportAssetOptions.ForceSynchronousImport );
			}
			oCreatedAssetPaths.Add(oGeneratedMaterialAssetPath);
		}

		try
		{
			if( m_rSpriteData.meshCollidersList != null && m_rSpriteData.meshCollidersList.Count > 0 )
			{
				// Mesh collider(s)
				string oMeshColliderName = m_rSpriteData.meshCollidersList[ 0 ].name;
				string oMeshColliderAssetPath = oPrefabResourcesSubFolderPath + oMeshColliderName + ".asset";
					
				Mesh rFirstColliderMesh = m_rSpriteData.meshCollidersList[0];
				
				if( AssetDatabase.Contains( rFirstColliderMesh ) )
				{
					AssetDatabase.CopyAsset( AssetDatabase.GetAssetPath( rFirstColliderMesh ), oMeshColliderAssetPath );
				}
				else
				{
					AssetDatabase.CreateAsset( rFirstColliderMesh, oMeshColliderAssetPath );
					AssetDatabase.ImportAsset( oMeshColliderAssetPath, ImportAssetOptions.ForceSynchronousImport );
				}
				
				oCreatedAssetPaths.Add(oMeshColliderAssetPath);
				List<Mesh> rMeshCollidersList = m_rSpriteData.meshCollidersList;
				
				for( int iMeshIndex = 1, iMeshCount = rMeshCollidersList.Count; iMeshIndex < iMeshCount; ++iMeshIndex )
				{
					Mesh rMeshCollider = rMeshCollidersList[ iMeshIndex ];
					//rMeshCollider.name = "mesh_Collider_" + oGameObjectName + "_" + iMeshIndex;
					AssetDatabase.AddObjectToAsset( rMeshCollider, rMeshCollidersList[ 0 ] );
					AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( rMeshCollider ), ImportAssetOptions.ForceSynchronousImport );
				}
			}
		}
		finally
		{
			//AssetDatabase.StopAssetEditing( );
		}
		
		// Remove all the unused assets in the folder
		string[] oFiles = Directory.GetFiles(oPrefabResourcesSubFolderPath_Absolute);
		string[] oDirectory = Directory.GetDirectories(oPrefabResourcesSubFolderPath_Absolute);
		List<string> oAssets = new List<string>();
		oAssets.AddRange(oFiles);
		oAssets.AddRange(oDirectory);
		foreach(string oAssetPath in oAssets)
		{
			if(oCreatedAssetPaths.Contains(oAssetPath) == false)
			{
				string oAssetName = oAssetPath.Replace(oPrefabResourcesSubFolderPath_Absolute + "/", "");
				AssetDatabase.DeleteAsset(oPrefabResourcesSubFolderPath + "/" + oAssetName);
			}
		}
	}
	
	// Reset 
	private void Reset()
	{
		EditorUtility.SetDirty(this);
	}
	
	// Need to be rebuild?
	private bool NeedToBeRebuild()
	{
		return isPhysicsDirty || m_rSpriteData.AreDataGenerated( m_rSpriteSettings.physicsMode ) == false;
	}
	
	// Has atlas been refreshed ?
	private bool HasAtlasBeenRefreshed()
	{
		return ( m_rSpriteSettings.atlas != null && atlasGenerationID != m_rSpriteSettings.atlas.generationId );
	}
	
	// Set default atlas
	public void SetDefaultAtlas()
	{
		m_rSpriteSettings.atlas = Uni2DEditorUtils.FindFirstTextureAtlas( m_rSpriteSettings.textureContainer.GUID );
	}

	///// Skeletal animation /////

	public void UpdatePosing( bool a_bCleanSkinning = false )
	{
		Uni2DSmoothBindingBone[ ] rBones = this.Bones;
		int iBoneCount = rBones.Length;

		MeshFilter rMeshFilterComponent                   = this.GetComponent<MeshFilter>( );
		MeshRenderer rMeshRendererComponent               = this.GetComponent<MeshRenderer>( );
		SkinnedMeshRenderer rSkinnedMeshRendererComponent = this.GetComponent<SkinnedMeshRenderer>( );

		// The skinned mesh
		Mesh rSkinnedMesh = SpriteData.renderMesh;

		if( rMeshFilterComponent == null )
		{
			rMeshFilterComponent = this.gameObject.AddComponent<MeshFilter>( );
		}

		// Bones are attached to the sprite: need to update its posing / bindpose / bone weights
		// and ensure the sprite has the required components to be rendered as a skinned mesh
		if( iBoneCount > 0 )
		{
			// Create skinned mesh renderer component if none exists
			if( rSkinnedMeshRendererComponent == null )
			{
				rSkinnedMeshRendererComponent = this.gameObject.AddComponent<SkinnedMeshRenderer>( );
			}

			// If a mesh renderer component exists, copy its shared materials to the skinned mesh renderer component
			// then destroy it to have only one (skinned) mesh renderer
			if( rMeshRendererComponent != null )
			{
				rSkinnedMeshRendererComponent.sharedMaterials = rMeshRendererComponent.sharedMaterials;
				DestroyImmediate( rMeshRendererComponent );
			}

			// Force skinned mesh renderer component to be enabled
			rSkinnedMeshRendererComponent.enabled = true;
			
			// Skinned vertices
			int iSkinnedVertexCount = rSkinnedMesh.vertexCount;
			Vector3[ ] rSkinnedMeshVertices = rSkinnedMesh.vertices;

			Transform[ ] oBoneTransforms = new Transform[ iBoneCount ];
			Matrix4x4[ ] oBindPoses      = new Matrix4x4[ iBoneCount ];
			BoneWeight[ ] oBoneWeights   = new BoneWeight[ iSkinnedVertexCount ];
	
			// Create poses
			Matrix4x4 rLocalToWorldMatrix = this.transform.localToWorldMatrix;
			Dictionary<Uni2DSmoothBindingBone,int> oBoneIndexDict = new Dictionary<Uni2DSmoothBindingBone, int>( iBoneCount );

			int iBoneIndex = 0;
			foreach( Uni2DSmoothBindingBone rBone in rBones )
			{
				Transform rBoneTransform      = rBone.transform;
				oBoneTransforms[ iBoneIndex ] = rBoneTransform;
				oBindPoses[ iBoneIndex ]      = rBoneTransform.worldToLocalMatrix * rLocalToWorldMatrix;
				
				oBoneIndexDict.Add( rBone, iBoneIndex );
				++iBoneIndex;
			}
	
			// Compute bone weights for each vertex
			for( int iSkinnedVertexIndex = 0; iSkinnedVertexIndex < iSkinnedVertexCount; ++iSkinnedVertexIndex )
			{
				oBoneWeights[ iSkinnedVertexIndex ] = this.ComputeBoneWeight( rSkinnedMeshVertices[ iSkinnedVertexIndex ], oBoneIndexDict );
			}
	
			// Set bindposes and weights
			rSkinnedMesh.boneWeights = oBoneWeights;
			rSkinnedMesh.bindposes   = oBindPoses;
	
			// Set bones and skinned mesh
			rSkinnedMeshRendererComponent.bones       = oBoneTransforms;
			rSkinnedMeshRendererComponent.sharedMesh  = rSkinnedMesh;
			rSkinnedMeshRendererComponent.localBounds = rSkinnedMesh.bounds;
			rSkinnedMeshRendererComponent.quality     = this.SpriteSettings.skinQuality;

			rMeshFilterComponent.sharedMesh = rSkinnedMesh;

			EditorUtility.SetDirty( rSkinnedMesh );
			EditorUtility.SetDirty( rSkinnedMeshRendererComponent );
			EditorUtility.SetDirty( rMeshFilterComponent );
			EditorUtility.SetDirty( this.gameObject );
		}
		else if( a_bCleanSkinning && rSkinnedMeshRendererComponent != null )	// In this case, revert sprite to a "classic" (unskinned) rendering
		{

			if( rMeshRendererComponent == null )
			{
				rMeshRendererComponent = this.gameObject.AddComponent<MeshRenderer>( );
			}

			rSkinnedMesh.boneWeights = null;
			rSkinnedMesh.bindposes   = null;

			rMeshFilterComponent.sharedMesh        = rSkinnedMesh;
			rMeshRendererComponent.sharedMaterials = rSkinnedMeshRendererComponent.sharedMaterials;

			// Remove useless skinned mesh component
			DestroyImmediate( rSkinnedMeshRendererComponent );
			
			EditorUtility.SetDirty( rSkinnedMesh );
			EditorUtility.SetDirty( rMeshFilterComponent );
			EditorUtility.SetDirty( rMeshRendererComponent );
			EditorUtility.SetDirty( this.gameObject );
		}
	}

	private BoneWeight ComputeBoneWeight( Vector3 a_f3VertexLocalCoords, Dictionary<Uni2DSmoothBindingBone,int> a_rBonesIndexes )
	{
		Dictionary<Uni2DSmoothBindingBone, float> rBonesInfluences = this.GetBonesInfluences( a_f3VertexLocalCoords, a_rBonesIndexes.Keys );
		float[ ] rWeights = rBonesInfluences.Values.ToArray( );
		Uni2DSmoothBindingBone[ ] rBones = rBonesInfluences.Keys.ToArray( );

		float fInvInfluenceSum = 1.0f / rBonesInfluences.Sum( x => x.Value );

		int iBoneCount = rBones.Length;
		BoneWeight oBoneWeight = new BoneWeight( );

		// Bone 1
		if( iBoneCount > 0 )
		{
			oBoneWeight.weight0 = rWeights[ 0 ] * fInvInfluenceSum;
			oBoneWeight.boneIndex0 = a_rBonesIndexes[ rBones[ 0 ] ];

			// Bone 2
			if( iBoneCount > 1 )
			{
				oBoneWeight.weight1 = rWeights[ 1 ] * fInvInfluenceSum;
				oBoneWeight.boneIndex1 = a_rBonesIndexes[ rBones[ 1 ] ];
		
				// Bone 3
				if( iBoneCount > 2 )
				{
					oBoneWeight.weight2 = rWeights[ 2 ] * fInvInfluenceSum;
					oBoneWeight.boneIndex2 = a_rBonesIndexes[ rBones[ 2 ] ];
		
					// Bone 4
					if( iBoneCount > 3 )
					{
						oBoneWeight.weight3 = rWeights[ 3 ] * fInvInfluenceSum;
						oBoneWeight.boneIndex3 = a_rBonesIndexes[ rBones[ 3 ] ];
					}
				}
			}
		}

		return oBoneWeight;
	}

	private Dictionary<Uni2DSmoothBindingBone,float> GetBonesInfluences( Vector3 a_f3VertexCoords, ICollection<Uni2DSmoothBindingBone> a_rBones )
	{
		// Get all transforms (i.e. bones) except the root itself
		int iBoneCount = a_rBones.Count;
		Dictionary<Uni2DSmoothBindingBone,float> oBoneDistancesDict = new Dictionary<Uni2DSmoothBindingBone, float>( iBoneCount );
		
		foreach( Uni2DSmoothBindingBone rBone in a_rBones )
		{
			if( rBone.HasInfluence )
			{
				oBoneDistancesDict.Add( rBone, this.ComputeDistanceToBone( rBone, a_f3VertexCoords ) );
			}
		}

		// Order by distance (ascending), take the 4 best results and build a transform->distance dict
		return oBoneDistancesDict.OrderBy( x => x.Value )
			.Take( 4 )
			.ToDictionary( x => x.Key, x => Uni2DEditorSmoothBindingUtils.BoneInfluenceDistanceFunc( x.Value, this.SpriteSettings.boneInfluenceFalloff ) );
	}

	private float ComputeDistanceToBone( Uni2DSmoothBindingBone a_rBone, Vector3 a_f3VertexCoords )
	{
		Vector3 f3LocalRootPosition = this.transform.InverseTransformPoint( a_rBone.transform.position );
		float fMinDistance = Vector3.Distance( f3LocalRootPosition, a_f3VertexCoords );
		
		foreach( Uni2DSmoothBindingBone rBoneChild in a_rBone.Children )
		{
			float fDistance = Uni2DMathUtils.DistancePointToLineSegment( a_f3VertexCoords, f3LocalRootPosition, this.transform.InverseTransformPoint( rBoneChild.transform.position ) );
			fMinDistance = Mathf.Min( fMinDistance, fDistance );
		}
		
		return fMinDistance;
	}
	
	void OnDrawGizmos( )
	{
		Bounds rBounds = this.renderer.bounds;
		float fLimit = 0.01f * Vector3.Distance( rBounds.min, rBounds.max );

		if( Selection.activeTransform != this.transform )
		{
			this.DrawBoneGizmos( Uni2DEditorPreferences.UnselectedBoneGizmoColor,
				Uni2DEditorPreferences.ActiveBoneGizmoColor,
				Uni2DEditorPreferences.UnselectedRootBoneGizmoColor,
				Uni2DEditorPreferences.ActiveRootBoneGizmoColor,
				Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode == BoneEditMode.Posing,
				fLimit );
		}
		else if( Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode == BoneEditMode.None )
		{
			this.DrawBoneGizmos( Uni2DEditorPreferences.SelectedBoneGizmoColor,
				Uni2DEditorPreferences.ActiveBoneGizmoColor,
				Uni2DEditorPreferences.SelectedRootBoneGizmoColor,
				Uni2DEditorPreferences.ActiveRootBoneGizmoColor,
				false,
				fLimit );
		}
		else
		{
			this.DrawBoneGizmos( Uni2DEditorPreferences.EditableBoneGizmoColor,
				Uni2DEditorPreferences.ActiveBoneGizmoColor,
				Uni2DEditorPreferences.EditableRootBoneGizmoColor,
				Uni2DEditorPreferences.ActiveRootBoneGizmoColor,
				Uni2DEditorSmoothBindingGUI.CurrentBoneEditMode == BoneEditMode.Posing,
				fLimit );
		}
	}

	private void DrawBoneGizmos( Color a_rBoneColor, Color a_rActiveBoneColor, Color a_rRootBoneColor, Color a_rActiveRootBoneColor, bool a_bHideBoneSpheres, float a_fBoneGizmoSizeLimit )
	{
		float fSphereSize;
		float fLimitFactor;

		foreach( Uni2DSmoothBindingBone rBone in this.Bones )
		{
			bool bIsActiveBone = ( rBone == Uni2DEditorSmoothBindingGUI.activeBone );
			Uni2DSmoothBindingBone rBoneParent = rBone.Parent;

			Transform rBoneTransform = rBone.transform;
			Vector3 f3BonePos = rBoneTransform.position;

			if( rBoneParent != null && rBone.IsFakeRootBone == false )
			{
				Gizmos.color = bIsActiveBone ? a_rActiveBoneColor : a_rBoneColor;
				this.DrawBone( rBoneParent.transform, rBoneTransform, a_fBoneGizmoSizeLimit );
			}

			if( a_bHideBoneSpheres == false )
			{	
				if( rBoneParent == null )
				{
					Gizmos.color = bIsActiveBone ? a_rActiveRootBoneColor : a_rRootBoneColor;
					fSphereSize  = 0.2f;
					fLimitFactor = 1.0f;
				}
				else
				{
					Gizmos.color = bIsActiveBone ? a_rActiveBoneColor : a_rBoneColor;
					fSphereSize  = 0.1f;
					fLimitFactor = 0.5f;
				}

				Gizmos.DrawSphere( f3BonePos,
					Mathf.Clamp( HandleUtility.GetHandleSize( f3BonePos ) * fSphereSize,
					0.0f,
					a_fBoneGizmoSizeLimit * fLimitFactor ) );
			}
		}
	}

	private void DrawBone( Transform a_rFrom, Transform a_rTo, float a_fSizeLimit )
	{
		Vector3 f3BoneStart = a_rFrom.position;
		Vector3 f3BoneEnd   = a_rTo.position;

		Quaternion oRot = Quaternion.FromToRotation( a_rFrom.forward, f3BoneEnd - f3BoneStart );

		float fSizeFactor = Mathf.Clamp( HandleUtility.GetHandleSize( a_rFrom.position ) * 0.2f, 0.0f, a_fSizeLimit );
		Vector3 f3LocalUp    = fSizeFactor * ( oRot * a_rFrom.up );
		Vector3 f3LocalRight = fSizeFactor * ( oRot * a_rFrom.right );

		// Start [
		Gizmos.DrawLine( f3BoneStart + f3LocalUp, f3BoneStart + f3LocalRight );
		Gizmos.DrawLine( f3BoneStart + f3LocalRight, f3BoneStart - f3LocalUp );
		Gizmos.DrawLine( f3BoneStart - f3LocalUp, f3BoneStart - f3LocalRight );
		Gizmos.DrawLine( f3BoneStart - f3LocalRight, f3BoneStart + f3LocalUp );

		// Start [ -> end >
		Gizmos.DrawLine( f3BoneStart + f3LocalUp, f3BoneEnd );
		Gizmos.DrawLine( f3BoneStart + f3LocalRight, f3BoneEnd );
		Gizmos.DrawLine( f3BoneStart - f3LocalUp, f3BoneEnd );
		Gizmos.DrawLine( f3BoneStart - f3LocalRight, f3BoneEnd );
	}

	public void RestorePosePosition( )
	{
		Transform rSpriteTransform = this.transform;

		if( rSpriteTransform != null )	// Can be null if an inspector is exiting because of sprite deletion
		{
			// Bones
			Uni2DSmoothBindingBone[ ] rBones = this.Bones;
			
			// Restore rotations and scales
			for( int iBoneIndex = 0, iBoneCount = rBones.Length; iBoneIndex < iBoneCount; ++iBoneIndex )
			{
				Transform rBoneTransform = rBones[ iBoneIndex ].transform;
				rBoneTransform.localRotation = Quaternion.identity;
				rBoneTransform.localScale = Vector3.one;
			}
	
			// Restore roots positions
			SkinnedMeshRenderer rSkinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>( );
			if( rSkinnedMeshRenderer != null )
			{
				Mesh rSkinnedMesh = rSkinnedMeshRenderer.sharedMesh;
				if( rSkinnedMesh == null )
				{
					MeshFilter rMeshFilterComponent = this.GetComponent<MeshFilter>( );
					rSkinnedMesh = rMeshFilterComponent.sharedMesh;
					rSkinnedMeshRenderer.sharedMesh = rSkinnedMesh;
				}
		
				Matrix4x4[ ] rBindPoses = rSkinnedMesh.bindposes;
				Transform[ ] rBoneTransforms = rSkinnedMeshRenderer.bones;
				
				if( rBindPoses != null && rBoneTransforms != null )
				{
					int iCount = Mathf.Min( rBindPoses.Length, rBoneTransforms.Length );
					for( int iBoneTransformIndex = 0; iBoneTransformIndex < iCount; ++iBoneTransformIndex )
					{
						Transform rBone = rBoneTransforms[ iBoneTransformIndex ];
						if( rBone != null && rBone.parent == rSpriteTransform )
						{
							rBone.localPosition = - rBindPoses[ iBoneTransformIndex ].MultiplyPoint( Vector3.zero );
						}
					}
				}
			}
		}
	}

#endif
}
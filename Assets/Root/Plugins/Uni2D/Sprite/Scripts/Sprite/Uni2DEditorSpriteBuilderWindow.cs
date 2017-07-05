#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Linq;

using PivotType     = Uni2DSprite.PivotType;
using PhysicsMode   = Uni2DSprite.PhysicsMode;
using CollisionType = Uni2DSprite.CollisionType;

public class Uni2DEditorSpriteBuilderWindow : EditorWindow
{
	private const string mc_oTextureAtlasDefaultName = "TextureAtlas";
//	private const string mc_oAnimationLibraryDefaultName = "AnimationLibrary";
	private const string mc_oAnimationClipDefaultName = "AnimationClip";
	private const string mc_oPixelPerfectCameraDefaultName = "PixelPerfectCamera";
	
	// Editor window settings
	private const string mc_oWindowTitle    = "Sprite Editor";
	private const bool mc_bWindowUtility    = false;	// Floating window?
	private const bool mc_bWindowTakeFocus  = false;
	private const float mc_fWindowMinWidth  = 250.0f;
	private const float mc_fWindowMinHeight = 65.0f;
	
	// section
	private const string mc_oGUILabelBuildSection = "Builder";
	private const string mc_oGUILabelLibrarySection = "Library";
	
	// Editor window GUI
	private const string mc_oGUILabelCreateSprite = "Create Sprite";
	private const string mc_oGUILabelCreatePhysicSprite = "Create Physic Sprite";
	private const string mc_oGUILabelCreateAnimationClip = "Create Animation Clip";
	private const string mc_oGUILabelSaveSprite = "Save as Prefab";

	// TODO: remove
	// Default sprite parameters
	private const PhysicsMode m_ePhysicModeSprite = PhysicsMode.NoPhysics;
	private const CollisionType m_eCollisionTypeSprite = CollisionType.Convex;
	private const bool m_bIsKinematicSprite = false;
	private const float m_fScaleSprite = 1.0f;
	
	// Default physic sprite parameters
	private const PhysicsMode m_ePhysicModePhysicSprite = PhysicsMode.Dynamic;
	private const CollisionType m_eCollisionTypePhysicSprite = CollisionType.Convex;
	private const bool m_bIsKinematicPhysicSprite = false;
	private const float m_fAlphaCutOffPhysicSprite = 0.75f;
	private const float m_fPolygonizationAccuracyPhysicSprite = 5.0f;
	private const bool m_bPolygonizeHolesPhysicSprite = false;
	private const float m_fExtrusionDepthPhysicSprite = 0.5f;

	private Object[ ] m_rSelectedObjects = null;
	
	// Library
	
	// Editor window GUI
	private const string mc_oGUILabelFilter = "Filter";
	private const string mc_oGUILabelUpdate = "Update Physic";

	[MenuItem( "Uni2D/" + mc_oWindowTitle )]
	public static void CreateEditorWindow( )
	{
		Uni2DEditorSpriteBuilderWindow oEditorWindow = EditorWindow.GetWindow<Uni2DEditorSpriteBuilderWindow>( mc_bWindowUtility, mc_oWindowTitle, mc_bWindowTakeFocus );
		oEditorWindow.minSize = new Vector2( mc_fWindowMinWidth, mc_fWindowMinHeight );
	}
	
	[MenuItem("Assets/Create/Uni2D/Texture Atlas")]
	[MenuItem("Uni2D/Create/Texture Atlas")]
	static void DoCreateTextureAtlas()
    {
		// Get the selected path
		string oNewPrefabPath = Uni2DEditorUtils.GenerateNewPrefabLocalPath(mc_oTextureAtlasDefaultName);
		
		// Create model
        GameObject oPrefabModel = new GameObject();
        oPrefabModel.AddComponent<Uni2DTextureAtlas>();
		
		// Save it as a prefab
		PrefabUtility.CreatePrefab(oNewPrefabPath, oPrefabModel);
		
		// Destroy model
        GameObject.DestroyImmediate(oPrefabModel);
	}

	// TODO
	[MenuItem("Assets/Create/Uni2D/Animation Clip")]
	[MenuItem("Uni2D/Create/Animation Clip")]
	static void DoCreateAnimationClip( )
    {
		Uni2DEditorSpriteBuilderWindow.CreateAnimationClip( null );
	}

	/*
	[MenuItem("Assets/Create/Uni2D/Animation Library")]
    static void DoCreateAnimationLibraryAssetsMenu()
    {
		DoCreateAnimationLibrary();
	}
	
	[MenuItem("Uni2D/Create/Animation Library")]
    static void DoCreateAnimationLibraryMainMenu()
    {
		DoCreateAnimationLibrary();
	}
	
	static void DoCreateAnimationLibrary()
    {
		// Get the selected path
		string oNewPrefabPath = Uni2DEditorUtils.GenerateNewPrefabLocalPath(mc_oAnimationLibraryDefaultName);
		
		// Create model
        GameObject oPrefabModel = new GameObject();
        oPrefabModel.AddComponent<Uni2DAnimationLibrary>();
		
		// Save it as a prefab
		PrefabUtility.CreatePrefab(oNewPrefabPath, oPrefabModel);
		
		// Destroy model
        GameObject.DestroyImmediate(oPrefabModel);
	}
	*/

	[MenuItem( "Assets/Create/Uni2D/Pixel Perfect Camera" )]
	static void DoCreatePixelPerfectCameraAssetsMenu( )
	{
		string oNewPrefabPath = Uni2DEditorUtils.GenerateNewPrefabLocalPath( mc_oPixelPerfectCameraDefaultName );

		GameObject oPrefabModel = new GameObject( );
		oPrefabModel.AddComponent<Camera>( );
		oPrefabModel.AddComponent<Uni2DPixelPerfectCamera>( );

		PrefabUtility.CreatePrefab( oNewPrefabPath, oPrefabModel );
		GameObject.DestroyImmediate( oPrefabModel );
	}

	[MenuItem( "Uni2D/Create/Pixel Perfect Camera" )]
	static void DoCreatePixelPerfectCameraMainMenu( )
	{
		GameObject oGameObject = new GameObject( mc_oPixelPerfectCameraDefaultName );
		oGameObject.AddComponent<Camera>( );
		oGameObject.AddComponent<Uni2DPixelPerfectCamera>( );

		oGameObject.transform.position = SceneView.currentDrawingSceneView.pivot;
	}

	private void OnEnable( )
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		UpdateSelection();
		this.Repaint( );
	}

	private void OnDestroy( )
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}
	
	private void OnSelectionChange( )
	{
		// Default Unity selection mode
		UpdateSelection();
		this.Repaint( );
	}
	
	private void UpdateSelection()
	{	
		m_rSelectedObjects = Selection.GetFiltered( typeof( Texture2D ), SelectionMode.Assets );
	}
	
	private void OnGUI( )
	{
		UpdateSelection();
		DisplayBuilder();
		//EditorGUILayout.Separator();
		//DisplayLibrary();
	}
	
	private void OnSceneGUI(SceneView a_rSceneView)
    {
       if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
	    {
			List<Texture2D> oDraggedTextures = new List<Texture2D>();
	        foreach(Object rObject in DragAndDrop.objectReferences)
			{
				if(rObject is Texture2D)
				{
					oDraggedTextures.Add(rObject as Texture2D);
				}
			}
			
			if(oDraggedTextures.Count > 0)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			
				if(Event.current.type == EventType.DragPerform)
				{	
					DragAndDrop.AcceptDrag();					
					
					// Drop
					// Update editor selection
					Selection.objects = this.CreateSpritesFromDragAndDrop( oDraggedTextures, ComputeDropPositionWorld( a_rSceneView ), Event.current.alt, Event.current.shift );
				}
	
				Event.current.Use( );
			}
	    }
    }
	
	// Compute the drop position world
	private Vector3 ComputeDropPositionWorld(SceneView a_rSceneView)
	{
		// compute mouse position on the world y=0 plane
		float fOffsetY = 30.0f;
        Ray mouseRay = a_rSceneView.camera.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y - fOffsetY, 0.0f));
		
        float t = -mouseRay.origin.z / mouseRay.direction.z;
        Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
        mouseWorldPos.z = 0.0f;

       	return mouseWorldPos;
	}
	
	// Compute the drop position world
	// TODO: investigate null exception bug when creating from another scene view (Asset Server,...)
	private Vector3 ComputeCreateFromButtonPositionWorld(SceneView a_rSceneView)
	{
		// compute mouse position on the world y=0 plane
		float fOffsetY = 30.0f;
        Ray mouseRay = a_rSceneView.camera.ScreenPointToRay(new Vector3(Screen.width  * 0.5f, Screen.width * 0.5f - fOffsetY, 0.0f));
		
        float t = -mouseRay.origin.z / mouseRay.direction.z;
        Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
        mouseWorldPos.z = 0.0f;

       	return mouseWorldPos;
	}
	
	private void DisplayBuilder( )
	{
		bool bCreateSprite = false;
		bool bCreatePhysicSprite = false;
		bool bCreateAnimationClip = false;

		EditorGUILayout.BeginHorizontal( );
		{
			EditorGUI.BeginDisabledGroup( m_rSelectedObjects == null || m_rSelectedObjects.Length == 0 );
			{
				bCreateSprite = GUILayout.Button( mc_oGUILabelCreateSprite );
				bCreatePhysicSprite = GUILayout.Button( mc_oGUILabelCreatePhysicSprite );
				bCreateAnimationClip = GUILayout.Button( mc_oGUILabelCreateAnimationClip );
			}
			EditorGUI.EndDisabledGroup( );
		}
		EditorGUILayout.EndHorizontal( );

		if( bCreateSprite == true || bCreatePhysicSprite == true )
		{
			if( m_rSelectedObjects != null )
			{
				List<GameObject> oCreatedGameObjects = new List<GameObject>();
				foreach( Object rObject in m_rSelectedObjects )
				{
					Texture2D rTexture = (Texture2D) rObject;
					
					// Create sprite
					GameObject oSpriteMeshGameObject = CreateSprite(rTexture, ComputeCreateFromButtonPositionWorld(SceneView.lastActiveSceneView), bCreatePhysicSprite);
					
					oCreatedGameObjects.Add(oSpriteMeshGameObject);
				}
				
				// Update editor selection
			 	Selection.objects = oCreatedGameObjects.ToArray();			
				EditorUtility.UnloadUnusedAssets( );
			}
		}
		else if( bCreateAnimationClip )
		{
			List<Texture2D> oTexturesList = null;

			if( m_rSelectedObjects != null )
			{
				// Object -> Texture2D cast
				oTexturesList = m_rSelectedObjects.Cast<Texture2D>( ).ToList( );
			}

			Uni2DAnimationClip rAnimationClip = Uni2DEditorSpriteBuilderWindow.CreateAnimationClip( oTexturesList );
			Selection.objects = new Object[ 1 ]{ rAnimationClip.gameObject };
		}
	}

	public static Uni2DAnimationClip CreateAnimationClip( List<Texture2D> a_rTexturesList, string a_rAnimationClipName = null, string a_rAnimationClipPath = null )
	{
		// Create a new animation clip prefab
		GameObject oModel = new GameObject( );
		Uni2DAnimationClip oAnimationClipModel = oModel.AddComponent<Uni2DAnimationClip>( );

		// Path to save prefab
		string oPrefabPath;

		if( a_rTexturesList != null && a_rTexturesList.Count > 0 )
		{
			// Sort by name
			IOrderedEnumerable<Texture2D> rOrderedTexturesEnumerable = a_rTexturesList.OrderBy( x => x.name );
			Texture2D rFirstTexture = rOrderedTexturesEnumerable.First( );			

			// Create frames
			foreach( Texture2D rTexture in rOrderedTexturesEnumerable )
			{
				Uni2DAnimationFrame oAnimationFrame = new Uni2DAnimationFrame( );
				oAnimationFrame.textureContainer = new Texture2DContainer( rTexture, true );
				oAnimationClipModel.frames.Add( oAnimationFrame );
			}
		
			// Apply
			oAnimationClipModel.ApplySettings( Uni2DAnimationClip.AnimationClipRegeneration.RegenerateAnimationClipOnly );
			oPrefabPath = ( a_rAnimationClipPath == null
					? Uni2DEditorUtils.GetLocalAssetFolderPath( rFirstTexture )
					: a_rAnimationClipPath )
				
				+ ( a_rAnimationClipName == null
					? ( "AnimationClip_" + rFirstTexture.name )
					: a_rAnimationClipName )
				
				+ ".prefab";
			// Make prefab path unique
			oPrefabPath = AssetDatabase.GenerateUniqueAssetPath( oPrefabPath );
		}
		else
		{
			// Unique prefab path
			string oClipName = ( a_rAnimationClipName == null ? mc_oAnimationClipDefaultName : a_rAnimationClipName );
			oPrefabPath = ( a_rAnimationClipPath == null
				? Uni2DEditorUtils.GenerateNewPrefabLocalPath( oClipName )
				: AssetDatabase.GenerateUniqueAssetPath( a_rAnimationClipPath + oClipName ) );
		}

		// Save it as a prefab
		GameObject oPrefab = PrefabUtility.CreatePrefab( oPrefabPath, oModel );

		// Destroy model
		GameObject.DestroyImmediate( oModel );

		// Return prefab animation clip component
		return oPrefab.GetComponent<Uni2DAnimationClip>( );
	}

	private GameObject[ ] CreateSpritesFromDragAndDrop( List<Texture2D> a_rTexturesList, Vector3 a_f3Position, bool a_bPhysic, bool a_bCreateAnimationClip )
	{
		GameObject[ ] oGeneratedSprites;

		// Create an animation clip from selected textures
		if( a_bCreateAnimationClip )
		{
			oGeneratedSprites = new GameObject[ 1 ];

			// Create clip
			Uni2DAnimationClip rAnimationClip = CreateAnimationClip( a_rTexturesList );

			// Create a sprite from first animation frame
			GameObject rSpriteGameObject = this.CreateSprite( rAnimationClip.frames[ 0 ].textureContainer, a_f3Position, a_bPhysic );

			// Add clip to sprite animation
			Uni2DSpriteAnimation rSpriteAnimation = rSpriteGameObject.GetComponent<Uni2DSprite>( ).spriteAnimation;
			rSpriteAnimation.AddClip( rAnimationClip );

			// Set auto play
			rSpriteAnimation.playAutomatically = true;
			rSpriteAnimation.StartClipIndex = 0;

			// Add to generated sprites
			oGeneratedSprites[ 0 ] = rSpriteGameObject;
		}
		else
		{
			int iTextureCount = a_rTexturesList.Count;
			oGeneratedSprites = new GameObject[ iTextureCount ];

			for( int iTextureIndex = 0; iTextureIndex < iTextureCount; ++iTextureIndex )
			{
				oGeneratedSprites[ iTextureIndex ] = this.CreateSprite( a_rTexturesList[ iTextureIndex ], a_f3Position, a_bPhysic );
			}
		}

		return oGeneratedSprites;
	}
	
	// NEW
	private GameObject CreateSprite( Texture2D a_rTexture, bool a_bPhysic )
	{
		// Get default Uni2DSprite settings, according to a_bPhysic argument
		return Uni2DEditorSpriteBuilderUtils.GenerateSpriteFromSettings( new Uni2DEditorSpriteSettings( a_rTexture, a_bPhysic ) );
	}

	// NEW
	private GameObject CreateSprite( Texture2D a_rTexture, Vector3 a_f3CreationPosition, bool a_bPhysic )
	{
		// Create sprite game object
		GameObject rSpriteGameObject = CreateSprite( a_rTexture, a_bPhysic );

		// Set creation position
		rSpriteGameObject.transform.position = a_f3CreationPosition;

		return rSpriteGameObject;
	}
}
#endif
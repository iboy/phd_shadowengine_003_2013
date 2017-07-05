#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

/*
 * Uni2DEditorUtilsBuildingProgressBar
 * 
 * An util static class to create sweat progress bars while Uni2D is building sprites.
 * 
 * Editor use only.
 * 
 */
public static class Uni2DEditorUtilsBuildingProgressBar
{
	// Progress bar window title
	public const string progressBarWindowTitle = "Uni2D building progress";

	// Number of steps needed to build an Uni2D sprite
	public const float buildStepsCount = 4.0f;

	// Number of sprite which would be processed (used to compute progression)
	private static float ms_fSpriteToBuildCount = 1.0f;
	
	// Current sprite
	private static float ms_fProcessedSpriteCount = 0.0f;
	private static float ms_fProcessedTextureImportCount = 0.0f;
	private static float ms_fProcessedTextureReimportCount = 0.0f;
	
	// Current build step
	private static float ms_fProcessedBuildStep = 0.0f;

	private static bool ms_bHasBeenInit = false;

	private static float Progress
	{
		get
		{
			float fInvCount = 1.0f / ms_fSpriteToBuildCount;
			float fImports = 0.05f * ms_fProcessedTextureImportCount + 0.05f * ms_fProcessedTextureReimportCount;	// imports = 5 + 5 = 10%
			float fSprites = 0.9f * ms_fProcessedSpriteCount + ( 0.9f * ms_fProcessedBuildStep ) / buildStepsCount; // sprites = 90%

			// ( 10% imports + 90% sprites ) / sprite count
			return fImports * fInvCount + fSprites * fInvCount;
		}
	}

	public static void CreateBuildingProgressBar( int a_iSpriteCount )
	{
		ClearBuildingProgressBar( );

		ms_fSpriteToBuildCount = a_iSpriteCount;

		ms_fProcessedSpriteCount = 0.0f;
		ms_fProcessedBuildStep   = 0.0f;
		ms_fProcessedTextureImportCount   = 0.0f;
		ms_fProcessedTextureReimportCount = 0.0f;

		ms_bHasBeenInit = true;

		DisplayProgressBar( );
	}

	public static void AddProcessedTextureImport( )
	{
		if( ms_bHasBeenInit )
		{
			++ms_fProcessedTextureImportCount;
			DisplayProgressBar( );
		}
	}

	public static void AddProcessedTextureReimport( )
	{
		if( ms_bHasBeenInit )
		{
			++ms_fProcessedTextureReimportCount;
			DisplayProgressBar( );
		}
	}

	public static void AddProcessedSpriteBuildStep( )
	{
		if( ms_bHasBeenInit )
		{
			++ms_fProcessedBuildStep;
			DisplayProgressBar( );
		}
	}

	public static void AddProcessedSprite( )
	{
		if( ms_bHasBeenInit )
		{
			++ms_fProcessedSpriteCount;
			ms_fProcessedBuildStep = 0.0f;

			DisplayProgressBar( );
		}
	}

	private static void DisplayProgressBar( )
	{
		EditorUtility.DisplayProgressBar( progressBarWindowTitle, "Processing...", Uni2DEditorUtilsBuildingProgressBar.Progress );
	}

	public static void ClearBuildingProgressBar( )
	{
		ms_bHasBeenInit = false;

		EditorUtility.ClearProgressBar( );
		
		ms_fSpriteToBuildCount = 1.0f;

		ms_fProcessedSpriteCount = 0.0f;
		ms_fProcessedBuildStep   = 0.0f;
		ms_fProcessedTextureImportCount   = 0.0f;
		ms_fProcessedTextureReimportCount = 0.0f;
	}
}
#endif
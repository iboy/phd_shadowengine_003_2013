#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class Uni2DEditorShapeExtractionUtils
{

	// Distinguish completely transparent pixels from significant pixel by
	// "binarizing" the texture via a bit array.
	// false if a pixel is not significant (= transparent), true otherwise
	public static BinarizedImage BinarizeTexture( Texture2D a_rTextureToFilter, float a_fAlphaCutOff )
	{
		if( a_rTextureToFilter == null )
		{
			return null;
		}

		// float to byte
		byte iAlphaCutOff32 = (byte) ( a_fAlphaCutOff * 255.0f );

		// Retrieve texture pixels (in 32bits format, faster)
		// Array is flattened / pixels laid left to right, bottom to top
		Color32[ ] oTexturePixels = a_rTextureToFilter.GetPixels32( );
		int iPixelCount = oTexturePixels.Length;

		// Create (padded) sprite shape pixels array (bit array)
		BinarizedImage oBinarizedTexture = new BinarizedImage( a_rTextureToFilter.width, a_rTextureToFilter.height, false );

		// Parse all pixels
		for( int iPixelIndex = 0; iPixelIndex < iPixelCount; ++iPixelIndex )
		{
			Color32 f4Pixel = oTexturePixels[ iPixelIndex ];
			oBinarizedTexture.SetUnpaddedPixel( iPixelIndex, ( f4Pixel.a >= iAlphaCutOff32 ) );
		}

		// Fill 1px holes
		//oBinarizedTexture.FillInsulatedHoles( );
		return oBinarizedTexture;
	}


	// Double the width and height of a binarized image
	public static BinarizedImage ResizeImage( BinarizedImage a_rBinarizedImage )
	{
		int iImageHeight = a_rBinarizedImage.Height;
		int iImageWidth  = a_rBinarizedImage.Width;

		int iResizedImageHeight = 2 * iImageHeight;
		int iResizedImageWidth  = 2 * iImageWidth;

		BinarizedImage oResizedBinarizedImage = new BinarizedImage( iResizedImageWidth, iResizedImageHeight, false );

		// Upsampling
		// Copy original pixels to resized sprite pixels array
		for( int iResizedY = 0; iResizedY < iResizedImageHeight; ++iResizedY )
		{
			for( int iResizedX = 0; iResizedX < iResizedImageWidth; ++iResizedX )
			{
				// Euclidian div
				int iOriginalX = iResizedX / 2;
				int iOriginalY = iResizedY / 2;
				int iOriginalIndex = a_rBinarizedImage.Coords2PixelIndex( iOriginalX, iOriginalY );
				int iResizedIndex = oResizedBinarizedImage.Coords2PixelIndex( iResizedX, iResizedY );

				// Pixel copy
				oResizedBinarizedImage[ iResizedIndex ] = a_rBinarizedImage[ iOriginalIndex ];
			}
		}

		return oResizedBinarizedImage;
	}
}
#endif
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Uni2DEditorContourExtractionUtils
{
	public static void CombinedContourLabeling( BinarizedImage a_rBinarizedImage, bool a_bPolygonizeHoles, out List<Contour> a_rOuterContours, out List<Contour> a_rInnerContours )
	{
		a_rOuterContours = new List<Contour>( );
		a_rInnerContours = new List<Contour>( );

		int[ ] oLabelMap = new int[ a_rBinarizedImage.PixelCount ];	// init as 0
		int iRegionCounter = 0;

		for( int iY = 0; iY < a_rBinarizedImage.Height; ++iY )
		{
			int iLabel = 0;

			for( int iX = 0; iX < a_rBinarizedImage.Width; ++iX )
			{
				int iPixelIndex = a_rBinarizedImage.Coords2PixelIndex( iX, iY );

				if( a_rBinarizedImage[ iPixelIndex ] == true )	// Foreground
				{
					if( iLabel != 0 )	// Continue inside region
					{
						oLabelMap[ iPixelIndex ] = iLabel;
					}
					else
					{
						iLabel = oLabelMap[ iPixelIndex ];
						if( iLabel == 0 )	// New outer contour
						{
							++iRegionCounter;
							iLabel = iRegionCounter;
							Contour rOuterContour = TraceContour( iPixelIndex, NeighborDirection.DirectionRight, iLabel, a_rBinarizedImage, oLabelMap );
							a_rOuterContours.Add( rOuterContour );
							oLabelMap[ iPixelIndex ] = iLabel;
						}
					}
				}
				else // Background
				{
					if( iLabel != 0 )
					{
						if( a_bPolygonizeHoles == true )
						{
							if( oLabelMap[ iPixelIndex ] == 0 )	// New inner contour
							{
								iPixelIndex = a_rBinarizedImage.Coords2PixelIndex( iX - 1, iY );
								Contour rInnerContour = TraceContour( iPixelIndex, NeighborDirection.DirectionUpRight, iLabel, a_rBinarizedImage, oLabelMap );
								a_rInnerContours.Add( rInnerContour );
							}
							iLabel = 0;
						}
						else if( oLabelMap[ iPixelIndex ] != -1 )
						{
							oLabelMap[ iPixelIndex ] = iLabel;
						}
						else
						{
							iLabel = 0;
						}
					}
				}
			}
		}
	}

	public static Contour TraceContour( int a_iStartingPixelIndex, NeighborDirection a_eStartingDirection, int a_iContourLabel, BinarizedImage a_rBinarizedImage, int[ ] a_rLabelMap )
	{
		int iPixelIndexTrace;
		NeighborDirection eDirectionNext = a_eStartingDirection;
		FindNextPoint( a_iStartingPixelIndex, a_eStartingDirection, a_rBinarizedImage, a_rLabelMap, out iPixelIndexTrace, out eDirectionNext );
		Contour oContour = new Contour( a_iContourLabel );
		oContour.AddFirst( a_rBinarizedImage.PixelIndex2Coords( iPixelIndexTrace ) );
		int iPreviousPixelIndex = a_iStartingPixelIndex;
		int iCurrentPixelIndex = iPixelIndexTrace;
		bool bDone = ( a_iStartingPixelIndex == iPixelIndexTrace );

		// Choose a bias factor
		// The bias factor points to exterior if tracing an outer contour
		// The bias factor points to interior if tracing an inner contour
		float fOrthoBiasFactor;
		if( a_eStartingDirection == NeighborDirection.DirectionUpRight )	// inner contour
		{
			fOrthoBiasFactor = -0.2f;
		}
		else // outer contour
		{
			fOrthoBiasFactor = 0.2f;
		}
		
		while( bDone == false )
		{
			a_rLabelMap[ iCurrentPixelIndex ] = a_iContourLabel;
			NeighborDirection eDirectionSearch = (NeighborDirection) ( ( (int) eDirectionNext + 6 ) % 8 );
			int iNextPixelIndex;
			FindNextPoint( iCurrentPixelIndex, eDirectionSearch, a_rBinarizedImage, a_rLabelMap, out iNextPixelIndex, out eDirectionNext );

			iPreviousPixelIndex = iCurrentPixelIndex;
			iCurrentPixelIndex = iNextPixelIndex;
			bDone = ( iPreviousPixelIndex == a_iStartingPixelIndex && iCurrentPixelIndex == iPixelIndexTrace );

			if( bDone == false )
			{
				// Apply some bias to inner and outer contours to avoid them overlap
				// Use the orthogonal vector to direction
				NeighborDirection eOrthoBiasDirection = (NeighborDirection) ( ( (int) eDirectionNext + 6 ) % 8 ); // == direction - 2 % 8 but easier to compute (always positive)
				Vector2 f2Bias = fOrthoBiasFactor * BinarizedImage.GetDirectionVector( eOrthoBiasDirection );

				// Add bias to pixel pos
				Vector2 f2BiasedPos = f2Bias + a_rBinarizedImage.PixelIndex2Coords( iNextPixelIndex );

				// Add biased pos to contour
				oContour.AddFirst( f2BiasedPos );
			}
		}

		return oContour;	
	}

	public static void FindNextPoint( int a_iStartingPixelIndex, NeighborDirection a_eStartingDirection, BinarizedImage a_rBinarizedImage, int[ ] a_rLabelMap, out int a_iNextPixelIndex, out NeighborDirection a_eNextDirection )
	{
		a_eNextDirection = a_eStartingDirection;

		// Search in all directions except initial direction (=> 7)
		for( int iSearchIteration = 0; iSearchIteration < 7; ++iSearchIteration )
		{
			a_iNextPixelIndex = a_rBinarizedImage.GetNeighborPixelIndex( a_iStartingPixelIndex, a_eNextDirection );

			if( a_rBinarizedImage[ a_iNextPixelIndex ] == false )	// Background pixel, mark it as visited
			{
				a_rLabelMap[ a_iNextPixelIndex ] = -1;	// mark as visited
				a_eNextDirection = (NeighborDirection) ( ( (int) a_eNextDirection + 1 ) % 8 );	// Next direction...
			}
			else // Non background pixel
			{
				return;
			}
		}
		a_iNextPixelIndex = a_iStartingPixelIndex;	// return starting index
	}
}
#endif
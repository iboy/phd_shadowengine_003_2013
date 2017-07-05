/*
 	Based on the Public Domain MaxRectsBinPack.cpp source by Jukka Jylänki
 	https://github.com/juj/RectangleBinPack/
 
 	Ported to C# by Sven Magnus
 	This version is also public domain - do whatever you want with it.
*/
using UnityEngine;

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;

using EAtlasSize = Uni2DTextureAtlas.AtlasSize;
#endif

// Packed rect
[System.Serializable]
public class PackedRect
{
	public Rect rect;
	public bool isFlipped;
	
	public PackedRect( Rect a_rRect, bool a_bIsFlipped )
	{
		rect = a_rRect;
		isFlipped = a_bIsFlipped;
	}
}

#if UNITY_EDITOR
// Max rect bin packer
public class Uni2DEditorTextureAtlasPacker
{
	public int binWidth = 0;
	public int binHeight = 0;
	public int rectPadding = 0;
	public bool allowRotations;

	private Dictionary<int,PackedRect> m_oUsedRectanglesDict = new Dictionary<int,PackedRect>( );
	private List<Rect> m_oFreeRectanglesDict = new List<Rect>( );

	public Dictionary<int, PackedRect> UsedRectangles
	{
		get
		{
			return new Dictionary<int, PackedRect>( this.m_oUsedRectanglesDict );
		}
	}
	
	public enum FreeRectChoiceHeuristic
	{
		RectBestShortSideFit,	///< -BSSF: Positions the rectangle against the short side of a free rectangle into which it fits the best.
		RectBestLongSideFit,	///< -BLSF: Positions the rectangle against the long side of a free rectangle into which it fits the best.
		RectBestAreaFit,		///< -BAF: Positions the rectangle into the smallest free rect into which it fits.
		RectBottomLeftRule,		///< -BL: Does the Tetris placement.
		RectContactPointRule	///< -CP: Choosest the placement where the rectangle touches other rects as much as possible.
	};
 
	public Uni2DEditorTextureAtlasPacker( int width, int height, int padding = 0, bool rotations = true )
	{
		Init( width, height, padding, rotations );
	}

	// Returns a rough atlas size estimation, based on the sum of covered area of given rects.
	// It should only be used as a good start to find the optimal atlas size, not *THE* optimal size by itself.
	public static void EstimateMinAtlasSize( IEnumerable<Rect> a_rRectsToPackList, int a_iMaxAtlasSize, int a_iPadding, out int a_iAtlasWidthSize, out int a_iAtlasHeightSize )
	{
		float fTotalArea = 0.0f;
		float fPadding = (float) a_iPadding;

		float fMinWidthRequired  = 0.0f;
		float fMinHeightRequired = 0.0f;

		int iRectCount = 0;

		// Compute total area
		foreach( Rect rRect in a_rRectsToPackList )
		{
			float fPaddedWidth  = rRect.width  + fPadding;
			float fPaddedHeight = rRect.height + fPadding;

			fTotalArea += fPaddedWidth * fPaddedHeight;

			// fMinWidthRequired always >= fMinHeightRequired
			fMinWidthRequired  = Mathf.Max( fMinWidthRequired, Mathf.Max( fPaddedWidth, fPaddedHeight ) );
			fMinHeightRequired = Mathf.Max( fMinHeightRequired, Mathf.Min( fPaddedWidth, fPaddedHeight ) );
			++iRectCount;
		}
		int iMinPOTWidthRequired  = Mathf.Min( a_iMaxAtlasSize, Mathf.NextPowerOfTwo( Mathf.CeilToInt( fMinWidthRequired ) ) );
		int iMinPOTHeightRequired = Mathf.Min( a_iMaxAtlasSize, Mathf.NextPowerOfTwo( Mathf.CeilToInt( fMinHeightRequired ) ) );

		// Use closest POT as a good initial value
		int iWidthPOT  = Mathf.Min( a_iMaxAtlasSize, Mathf.ClosestPowerOfTwo( Mathf.CeilToInt( Mathf.Sqrt( fTotalArea ) ) ) );
		int iHeightPOT = Mathf.Min( a_iMaxAtlasSize, Mathf.ClosestPowerOfTwo( Mathf.CeilToInt( fTotalArea / (float) iWidthPOT ) ) );

		if( iHeightPOT > iWidthPOT )
		{
			// Swap to ensure WidthPOT always >= HeightPOT
			int iTmp   = iHeightPOT;
			iHeightPOT = iWidthPOT;
			iWidthPOT  = iTmp;
		}		

		// If closest POT size doesn't meet the min. size requirements...
		if( iMinPOTWidthRequired > iWidthPOT || iMinPOTHeightRequired > iHeightPOT )
		{
			iHeightPOT = Mathf.Min( a_iMaxAtlasSize, Mathf.ClosestPowerOfTwo( Mathf.CeilToInt( fTotalArea / (float) iMinPOTWidthRequired ) ) );
	
			a_iAtlasWidthSize  = Mathf.Max( iMinPOTWidthRequired, Mathf.Min( iWidthPOT, iMinPOTWidthRequired ) );
			a_iAtlasHeightSize = Mathf.Max( iMinPOTHeightRequired, Mathf.Min( iHeightPOT, iMinPOTHeightRequired ) );
		}
		else // min width/height required are smaller than the computed POT sizes
		{
			a_iAtlasWidthSize  = iWidthPOT;
			a_iAtlasHeightSize = iHeightPOT;
		}
	}
 
	public void Init( int width, int height, int padding = 0, bool rotations = true )
	{
		binWidth = width;
		binHeight = height;
		rectPadding = padding;
		allowRotations = rotations;
 
		Rect oInitialRect = new Rect();
		oInitialRect.x = 0;
		oInitialRect.y = 0;
		oInitialRect.width  = width;
		oInitialRect.height = height;
 
		m_oUsedRectanglesDict.Clear( );

		m_oFreeRectanglesDict.Clear( );
		m_oFreeRectanglesDict.Add( oInitialRect );
	}

	// For online packing use only.
	// Not useful here.
	/*
	public Rect Insert( int width, int height, FreeRectChoiceHeuristic method )
	{
		Rect newNode = new Rect();
		int score1 = 0; // Unused in this function. We don't need to know the score after finding the position.
		int score2 = 0;
		switch( method )
		{
			case FreeRectChoiceHeuristic.RectBestShortSideFit:	newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2); break;
			case FreeRectChoiceHeuristic.RectBottomLeftRule:	newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2); break;
			case FreeRectChoiceHeuristic.RectContactPointRule:	newNode = FindPositionForNewNodeContactPoint(width, height, ref score1); break;
			case FreeRectChoiceHeuristic.RectBestLongSideFit:	newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1); break;
			case FreeRectChoiceHeuristic.RectBestAreaFit:		newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2); break;
		}
 
		if( newNode.height == 0 )
		{
			return newNode;
		}
 
		int numRectanglesToProcess = freeRectangles.Count;
		for( int i = 0; i < numRectanglesToProcess; ++i )
		{
			if( SplitFreeNode( freeRectangles[ i ], ref newNode ) )
			{
				freeRectangles.RemoveAt(i);
				--i;
				--numRectanglesToProcess;
			}
		}
 
		PruneFreeList();
 
		usedRectangles.Add(newNode);
		return newNode;
	}*/
 
	// Returns true if all rects have been inserted, false otherwise
	public bool Insert( Dictionary<int,Rect> a_rRectsDict, FreeRectChoiceHeuristic a_ePackingMethod )
	{
		int iRectCount = a_rRectsDict.Count;
 
		while( iRectCount > 0 )
		{
			int iBestScore1 = int.MaxValue;
			int iBestScore2 = int.MaxValue;

			int iBestRectIndex  = -1;
			bool bBestIsFlipped = false;
			Rect oBestNode      = new Rect();
 
			foreach( KeyValuePair<int,Rect> rIndexedRect in a_rRectsDict )
			{
				int iScore1 = 0;
				int iScore2 = 0;
				bool bIsFlipped = false;

				// Score padded rect
				// Get a new padded (and probably flipped) rect
				Rect newNode = ScoreRect( (int) ( rIndexedRect.Value.width + rectPadding ),
					(int) ( rIndexedRect.Value.height + rectPadding ),
					a_ePackingMethod,
					ref iScore1,
					ref iScore2,
					ref bIsFlipped );
 
				if( iScore1 < iBestScore1 || ( iScore1 == iBestScore1 && iScore2 < iBestScore2 ) )
				{
					iBestScore1    = iScore1;
					iBestScore2    = iScore2;
					oBestNode      = newNode;
					iBestRectIndex = rIndexedRect.Key;
					bBestIsFlipped = bIsFlipped;
				}
			}
 
			if( iBestRectIndex == -1 )
			{
				return false;
			}
 
			PlaceRect( iBestRectIndex, oBestNode, bBestIsFlipped );
			a_rRectsDict.Remove( iBestRectIndex );

			--iRectCount;
		}

		return true;	// iRectCount = 0
	}
 
	void PlaceRect( int a_iIndex, Rect a_rNode, bool bIsFlipped )
	{
		int numRectanglesToProcess = m_oFreeRectanglesDict.Count;
		for( int i = 0; i < numRectanglesToProcess; ++i )
		{
			if( SplitFreeNode(m_oFreeRectanglesDict[ i ], ref a_rNode ) )
			{
				m_oFreeRectanglesDict.RemoveAt( i );
				--i;
				--numRectanglesToProcess;
			}
		}
 
		PruneFreeList( );
 
		// Unpad rect
		a_rNode.width  -= rectPadding;
		a_rNode.height -= rectPadding;

		// Add to used rectangles
		m_oUsedRectanglesDict.Add( a_iIndex, new PackedRect( a_rNode, bIsFlipped ) );
	}
 
	Rect ScoreRect(int width, int height, FreeRectChoiceHeuristic method, ref int score1, ref int score2, ref bool a_bIsFlipped )
	{
		Rect newNode = new Rect();
		score1 = int.MaxValue;
		score2 = int.MaxValue;
		switch(method)
		{
			case FreeRectChoiceHeuristic.RectBestShortSideFit: newNode = FindPositionForNewNodeBestShortSideFit(width, height, ref score1, ref score2, ref a_bIsFlipped); break;
			case FreeRectChoiceHeuristic.RectBottomLeftRule: newNode = FindPositionForNewNodeBottomLeft(width, height, ref score1, ref score2, ref a_bIsFlipped); break;
			case FreeRectChoiceHeuristic.RectContactPointRule: newNode = FindPositionForNewNodeContactPoint(width, height, ref score1, ref a_bIsFlipped); 
				score1 = -score1; // Reverse since we are minimizing, but for contact point score bigger is better.
				break;
			case FreeRectChoiceHeuristic.RectBestLongSideFit: newNode = FindPositionForNewNodeBestLongSideFit(width, height, ref score2, ref score1, ref a_bIsFlipped); break;
			case FreeRectChoiceHeuristic.RectBestAreaFit: newNode = FindPositionForNewNodeBestAreaFit(width, height, ref score1, ref score2, ref a_bIsFlipped); break;
		}
 
		// Cannot fit the current rectangle.
		if (newNode.height == 0)
		{
			score1 = int.MaxValue;
			score2 = int.MaxValue;
		}
 
		return newNode;
	}
 
	/// Computes the ratio of used surface area.
	public float Occupancy()
	{
		ulong usedSurfaceArea = 0UL;
		foreach( PackedRect rPackedRect in m_oUsedRectanglesDict.Values )
		{
			usedSurfaceArea += (ulong) ( rPackedRect.rect.width * rPackedRect.rect.height );
		}
 
		return (float)usedSurfaceArea / (binWidth * binHeight);
	}
 
	Rect FindPositionForNewNodeBottomLeft(int width, int height, ref int bestY, ref int bestX, ref bool a_bIsFlipped )
	{
		Rect bestNode = new Rect();
		//memset(bestNode, 0, sizeof(Rect));
 
		bestY = int.MaxValue;
 
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (m_oFreeRectanglesDict[i].width >= width && m_oFreeRectanglesDict[i].height >= height)
			{
				int topSideY = (int)m_oFreeRectanglesDict[i].y + height;
				if (topSideY < bestY || (topSideY == bestY && m_oFreeRectanglesDict[i].x < bestX))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestY = topSideY;
					bestX = (int)m_oFreeRectanglesDict[i].x;
					a_bIsFlipped = false;
				}
			}
			if (allowRotations && m_oFreeRectanglesDict[i].width >= height && m_oFreeRectanglesDict[i].height >= width)
			{
				int topSideY = (int)m_oFreeRectanglesDict[i].y + width;
				if (topSideY < bestY || (topSideY == bestY && m_oFreeRectanglesDict[i].x < bestX))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestY = topSideY;
					bestX = (int)m_oFreeRectanglesDict[i].x;
					a_bIsFlipped = true;
				}
			}
		}
		return bestNode;
	}
 
	Rect FindPositionForNewNodeBestShortSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit, ref bool a_bIsFlipped ) 
	{
		Rect bestNode = new Rect();
		//memset(&bestNode, 0, sizeof(Rect));
 
		bestShortSideFit = int.MaxValue;
 
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (m_oFreeRectanglesDict[i].width >= width && m_oFreeRectanglesDict[i].height >= height)
			{
				int leftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - width);
				int leftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
 
				if (shortSideFit < bestShortSideFit || (shortSideFit == bestShortSideFit && longSideFit < bestLongSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
					a_bIsFlipped = false;
				}
			}
 
			if (allowRotations && m_oFreeRectanglesDict[i].width >= height && m_oFreeRectanglesDict[i].height >= width)
			{
				int flippedLeftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - height);
				int flippedLeftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - width);
				int flippedShortSideFit = Mathf.Min(flippedLeftoverHoriz, flippedLeftoverVert);
				int flippedLongSideFit = Mathf.Max(flippedLeftoverHoriz, flippedLeftoverVert);
 
				if (flippedShortSideFit < bestShortSideFit || (flippedShortSideFit == bestShortSideFit && flippedLongSideFit < bestLongSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = flippedShortSideFit;
					bestLongSideFit = flippedLongSideFit;
					a_bIsFlipped = true;
				}
			}
		}
		return bestNode;
	}
 
	Rect FindPositionForNewNodeBestLongSideFit(int width, int height, ref int bestShortSideFit, ref int bestLongSideFit, ref bool a_bIsFlipped )
	{
		Rect bestNode = new Rect();
		//memset(&bestNode, 0, sizeof(Rect));
 
		bestLongSideFit = int.MaxValue;
 
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (m_oFreeRectanglesDict[i].width >= width && m_oFreeRectanglesDict[i].height >= height)
			{
				int leftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - width);
				int leftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
 
				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
					a_bIsFlipped = false;
				}
			}
 
			if (allowRotations && m_oFreeRectanglesDict[i].width >= height && m_oFreeRectanglesDict[i].height >= width)
			{
				int leftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - height);
				int leftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - width);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
				int longSideFit = Mathf.Max(leftoverHoriz, leftoverVert);
 
				if (longSideFit < bestLongSideFit || (longSideFit == bestLongSideFit && shortSideFit < bestShortSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = shortSideFit;
					bestLongSideFit = longSideFit;
					a_bIsFlipped = true;
				}
			}
		}
		return bestNode;
	}
 
	Rect FindPositionForNewNodeBestAreaFit(int width, int height, ref int bestAreaFit, ref int bestShortSideFit, ref bool a_bIsFlipped )
	{
		Rect bestNode = new Rect();
		//memset(&bestNode, 0, sizeof(Rect));
 
		bestAreaFit = int.MaxValue;
 
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			int areaFit = (int)m_oFreeRectanglesDict[i].width * (int)m_oFreeRectanglesDict[i].height - width * height;
 
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (m_oFreeRectanglesDict[i].width >= width && m_oFreeRectanglesDict[i].height >= height)
			{
				int leftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - width);
				int leftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - height);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
 
				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestShortSideFit = shortSideFit;
					bestAreaFit = areaFit;
					a_bIsFlipped = false;
				}
			}
 
			if (allowRotations && m_oFreeRectanglesDict[i].width >= height && m_oFreeRectanglesDict[i].height >= width)
			{
				int leftoverHoriz = Mathf.Abs((int)m_oFreeRectanglesDict[i].width - height);
				int leftoverVert = Mathf.Abs((int)m_oFreeRectanglesDict[i].height - width);
				int shortSideFit = Mathf.Min(leftoverHoriz, leftoverVert);
 
				if (areaFit < bestAreaFit || (areaFit == bestAreaFit && shortSideFit < bestShortSideFit))
				{
					bestNode.x = m_oFreeRectanglesDict[i].x;
					bestNode.y = m_oFreeRectanglesDict[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestShortSideFit = shortSideFit;
					bestAreaFit = areaFit;
					a_bIsFlipped = true;
				}
			}
		}
		return bestNode;
	}
 
	/// Returns 0 if the two intervals i1 and i2 are disjoint, or the length of their overlap otherwise.
	int CommonIntervalLength(int i1start, int i1end, int i2start, int i2end)
	{
		if (i1end < i2start || i2end < i1start)
		{
			return 0;
		}

		return Mathf.Min(i1end, i2end) - Mathf.Max(i1start, i2start);
	}
 
	int ContactPointScoreNode(int x, int y, int width, int height)
	{
		int score = 0;
 
		if (x == 0 || x + width == binWidth)
		{
			score += height;
		}

		if (y == 0 || y + height == binHeight)
		{
			score += width;
		}
 
		foreach( PackedRect rPackedRect in m_oUsedRectanglesDict.Values )
		{
			Rect oPackedRect = rPackedRect.rect;
			if ( oPackedRect.x == x + width || oPackedRect.x + oPackedRect.width == x)
			{
				score += CommonIntervalLength( (int) oPackedRect.y, (int) oPackedRect.y + (int) oPackedRect.height, y, y + height);
			}
			if ( oPackedRect.y == y + height || oPackedRect.y + oPackedRect.height == y )
			{
				score += CommonIntervalLength( (int) oPackedRect.x, (int) oPackedRect.x + (int) oPackedRect.width, x, x + width );
			}
		}
		return score;
	}
 
	Rect FindPositionForNewNodeContactPoint(int width, int height, ref int bestContactScore, ref bool a_bIsFlipped )
	{
		Rect bestNode = new Rect();
		//memset(&bestNode, 0, sizeof(Rect));
 
		bestContactScore = -1;
 
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			// Try to place the rectangle in upright (non-flipped) orientation.
			if (m_oFreeRectanglesDict[i].width >= width && m_oFreeRectanglesDict[i].height >= height) 
			{
				int score = ContactPointScoreNode((int)m_oFreeRectanglesDict[i].x, (int)m_oFreeRectanglesDict[i].y, width, height);
				if (score > bestContactScore)
				{
					bestNode.x = (int)m_oFreeRectanglesDict[i].x;
					bestNode.y = (int)m_oFreeRectanglesDict[i].y;
					bestNode.width = width;
					bestNode.height = height;
					bestContactScore = score;
					a_bIsFlipped = false;
				}
			}
			if (allowRotations && m_oFreeRectanglesDict[i].width >= height && m_oFreeRectanglesDict[i].height >= width)
			{
				int score = ContactPointScoreNode((int)m_oFreeRectanglesDict[i].x, (int)m_oFreeRectanglesDict[i].y, height, width);
				if (score > bestContactScore)
				{
					bestNode.x = (int)m_oFreeRectanglesDict[i].x;
					bestNode.y = (int)m_oFreeRectanglesDict[i].y;
					bestNode.width = height;
					bestNode.height = width;
					bestContactScore = score;
					a_bIsFlipped = true;
				}
			}
		}
		return bestNode;
	}
 
	bool SplitFreeNode(Rect freeNode, ref Rect usedNode)
	{
		// Test with SAT if the rectangles even intersect.
		if (usedNode.x >= freeNode.x + freeNode.width || usedNode.x + usedNode.width <= freeNode.x ||
			usedNode.y >= freeNode.y + freeNode.height || usedNode.y + usedNode.height <= freeNode.y)
		{
			return false;
		}
 
		if (usedNode.x < freeNode.x + freeNode.width && usedNode.x + usedNode.width > freeNode.x)
		{
			// New node at the top side of the used node.
			if (usedNode.y > freeNode.y && usedNode.y < freeNode.y + freeNode.height) 
			{
				Rect newNode = freeNode;
				newNode.height = usedNode.y - newNode.y;
				m_oFreeRectanglesDict.Add(newNode);
			}
 
			// New node at the bottom side of the used node.
			if (usedNode.y + usedNode.height < freeNode.y + freeNode.height) 
			{
				Rect newNode = freeNode;
				newNode.y = usedNode.y + usedNode.height;
				newNode.height = freeNode.y + freeNode.height - (usedNode.y + usedNode.height);
				m_oFreeRectanglesDict.Add(newNode);
			}
		}
 
		if (usedNode.y < freeNode.y + freeNode.height && usedNode.y + usedNode.height > freeNode.y)
		{
			// New node at the left side of the used node.
			if (usedNode.x > freeNode.x && usedNode.x < freeNode.x + freeNode.width)
			{
				Rect newNode = freeNode;
				newNode.width = usedNode.x - newNode.x;
				m_oFreeRectanglesDict.Add(newNode);
			}
 
			// New node at the right side of the used node.
			if (usedNode.x + usedNode.width < freeNode.x + freeNode.width)
			{
				Rect newNode = freeNode;
				newNode.x = usedNode.x + usedNode.width;
				newNode.width = freeNode.x + freeNode.width - (usedNode.x + usedNode.width);
				m_oFreeRectanglesDict.Add(newNode);
			}
		}
 
		return true;
	}
 
	void PruneFreeList()
	{
		for(int i = 0; i < m_oFreeRectanglesDict.Count; ++i)
		{
			for(int j = i+1; j < m_oFreeRectanglesDict.Count; ++j)
			{
				if (IsContainedIn(m_oFreeRectanglesDict[i], m_oFreeRectanglesDict[j]))
				{
					m_oFreeRectanglesDict.RemoveAt(i);
					--i;
					break;
				}
				if (IsContainedIn(m_oFreeRectanglesDict[j], m_oFreeRectanglesDict[i]))
				{
					m_oFreeRectanglesDict.RemoveAt(j);
					--j;
				}
			}
		}
	}
 
	bool IsContainedIn(Rect a, Rect b)
	{
		return a.x >= b.x && a.y >= b.y 
			&& a.x+a.width <= b.x+b.width 
			&& a.y+a.height <= b.y+b.height;
	}
 
}

#endif
#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Enum used to select a neighbor node/pixel
public enum NeighborDirection
{
	DirectionRight,
	DirectionUpRight,
	DirectionUp,
	DirectionUpLeft,
	DirectionLeft,
	DirectionDownLeft,
	DirectionDown,
	DirectionDownRight
}

/* Class maintaining a pixel array of a sprite shape.
 * Because this struct is focused on the shape of a sprite,
 * the pixels have only 2 values: true (belong to shape) or false (don't belong to shape).
 * 
 * The pixels must be accessed via the overriden [ ] operator because
 * the struct maintains a flatten bit array of length ( width + 2 ) * ( height + 2 )
 * with inaccessible blank boundary rows / columns, so the index is recomputed.
 */
public class BinarizedImage
{
	// The (boolean) pixel array
	private BitArray m_oPixelsBitArray;

	// The sprite width / height
	private int m_iWidth;
	private int m_iHeight;

	private static int[,] ms_iDirectionDeltas = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };

	// Default constructor
	// Create a new flatten bit array of length ( width + 2 ) * ( height + 2 )
	// with a_bDefaultValue as default value
	public BinarizedImage( int a_iWidth, int a_iHeight, bool a_bDefaultValue )
	{
		m_iWidth = a_iWidth;
		m_iHeight = a_iHeight;
		m_oPixelsBitArray = new BitArray( this.PixelCount, a_bDefaultValue );
	}

	// Accessor to pixels
	public bool this[ int a_iIndex ]
	{
		get
		{
			return m_oPixelsBitArray[ a_iIndex ];
		}
		set
		{
			m_oPixelsBitArray[ a_iIndex ] = value;
		}
	}

	// Sprite height
	public int Height
	{
		get
		{
			return m_iHeight;
		}
	}

	// Sprite width
	public int Width
	{
		get
		{
			return m_iWidth;
		}
	}

	public int PaddedWidth
	{
		get
		{
			return m_iWidth + 2;
		}
	}

	public int PaddedHeight
	{
		get
		{
			return m_iHeight + 2;
		}
	}

	public int PixelCount
	{
		get
		{
			return ( this.PaddedWidth ) * ( this.PaddedHeight );
		}
	}

	// Transform unpadded 2D (X,Y) coords into 1D flatten + padded array index
	public int Coords2PixelIndex( int a_iX, int a_iY )
	{
		return a_iX + 1 + ( a_iY + 1 ) * ( this.PaddedWidth );
	}

	public int Coords2PixelIndex( Vector2 a_f2Coords )
	{
		return ((int) a_f2Coords.x) + 1 + ( ((int) a_f2Coords.y) + 1 ) * ( this.PaddedWidth );
	}

	// Transform padded 1D flattened pixel array index into unpadded 2D coords
	public Vector2 PixelIndex2Coords( int a_iInternalPixelIndex )
	{
		return new Vector2( ( a_iInternalPixelIndex % ( this.PaddedWidth ) ) - 1, ( a_iInternalPixelIndex / ( this.PaddedWidth ) ) - 1 );
	}

	// Transform a pixel index used via accessors into pixel index used internally by the bit array
	private int Index2InternalIndex( int a_iPixelIndex )
	{
		return ( ( a_iPixelIndex / m_iWidth ) + 1 ) * ( this.PaddedWidth ) + ( ( a_iPixelIndex % m_iWidth ) + 1 );
	}

	// Transform a pixel index used internally by the bit array into pixel index via accessors
	/*private int InternalIndex2Index( int a_iInternalPixelIndex )
	{
		int iInternalWidth = m_iWidth + 2;
		return ( ( a_iInternalPixelIndex / iInternalWidth ) - 1 ) * m_iWidth + ( ( a_iInternalPixelIndex % iInternalWidth ) - 1 );
	}*/

	// Return true if the pixel is wedged i.e. its 4-neighbors of the pixel are all significants
	public bool IsPixelWedged( int a_iPixelIndex )
	{
		return m_oPixelsBitArray[ a_iPixelIndex + 1            ] == true
			&& m_oPixelsBitArray[ a_iPixelIndex - 1            ] == true
			&& m_oPixelsBitArray[ a_iPixelIndex - this.PaddedWidth ] == true
			&& m_oPixelsBitArray[ a_iPixelIndex + this.PaddedWidth ] == true;
	}

	public static Vector2 GetDirectionVector( NeighborDirection a_eDirection )
	{
		int iDirectionValue = (int) a_eDirection;
		return new Vector2( ms_iDirectionDeltas[ iDirectionValue, 0 ], ms_iDirectionDeltas[ iDirectionValue, 1 ] );
	}
	
	public int GetNeighborPixelIndex( int a_iPixelIndex, NeighborDirection a_eDirection )
	{
		int iDirectionValue = (int) a_eDirection;
		return a_iPixelIndex + ms_iDirectionDeltas[ iDirectionValue, 0 ] + ms_iDirectionDeltas[ iDirectionValue, 1 ] * ( this.PaddedWidth );
	}

	public void SetUnpaddedPixel( int a_iPixelIndex, bool a_bPixelValue )
	{
		int a_iInternalPixelIndex = this.Index2InternalIndex( a_iPixelIndex );
		m_oPixelsBitArray[ a_iInternalPixelIndex ] = a_bPixelValue;
	}

	// Fill wedged white 1px holes
	public void FillInsulatedHoles( )
	{
		for( int iY = 0; iY < m_iHeight; ++iY )
		{
			for( int iX = 0; iX < m_iWidth; ++iX )
			{
				int iPixelIndex = this.Coords2PixelIndex( iX, iY );
				if( m_oPixelsBitArray[ iPixelIndex ] == false && this.IsPixelWedged( iPixelIndex ) == true )
				{
					m_oPixelsBitArray[ iPixelIndex ] = true;
				}
			}
		}
	}
}
#endif
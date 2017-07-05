#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Uni2DEditorPolygonTriangulationUtils
{
	// Returns a flat 3D polygonized mesh from 2D outer/inner contours
	public static Mesh PolygonizeContours( List<Contour> a_rDominantContoursList, float a_fScale, Vector3 a_f3PivotPoint, float a_fWidth, float a_fHeight )
	{
		// The mesh to build
		Mesh oCombinedMesh = new Mesh( );

		int iContourCount = a_rDominantContoursList.Count; //a_rDominantContoursList.Count;

		// Step 2: Ear clip outer contour
		CombineInstance[ ] oCombineMeshInstance = new CombineInstance[ iContourCount ];

		for( int iContourIndex = 0; iContourIndex < iContourCount; ++iContourIndex )
		{
			Vector3[ ] oVerticesArray;
			int[ ] oTrianglesArray;
			Vector2[ ] oUVs;
			Mesh oSubMesh = new Mesh( );

			EarClipping( a_rDominantContoursList[ iContourIndex ], a_fScale, a_f3PivotPoint, a_fWidth, a_fHeight, out oVerticesArray, out oTrianglesArray, out oUVs );

			oSubMesh.vertices  = oVerticesArray;
			oSubMesh.uv        = oUVs;
			oSubMesh.triangles = oTrianglesArray;
	
			oCombineMeshInstance[ iContourIndex ].mesh = oSubMesh;
		}

		// Step 3: Combine every sub meshes (merge, no transform)
		oCombinedMesh.CombineMeshes( oCombineMeshInstance, true, false );

		// Return!
		return oCombinedMesh;		
	}

	// Polygonizes a flat parametrized grid mesh from a binarized image
	public static Mesh PolygonizeGrid( BinarizedImage a_rBinarizedImage, float a_fScale, Vector3 a_f3PivotPoint, int a_iHorizontalSubDivs, int a_iVerticalSubDivs )
	{
		Mesh oGridPlaneMesh = new Mesh( );

		// Texture dimensions
		int iTextureWidth  = a_rBinarizedImage.Width;
		int iTextureHeight = a_rBinarizedImage.Height;

		int iGridSubDivPerRow    = ( a_iVerticalSubDivs + 1 );
		int iGridSubDivPerColumn = ( a_iHorizontalSubDivs + 1 );
		int iGridSubDivCount     = iGridSubDivPerRow * iGridSubDivPerColumn;

		int iGridVertexCount = ( a_iHorizontalSubDivs + 2 ) * ( a_iVerticalSubDivs + 2 );
		
		// Grid div dimensions
		float fGridSubDivPixelWidth  = ( (float) iTextureWidth  ) / (float) iGridSubDivPerRow;
		float fGridSubDivPixelHeight = ( (float) iTextureHeight ) / (float) iGridSubDivPerColumn;

		// Vertex pos -> index dictionary: stores the index associated to a given vertex
		int iCurrentVertexIndex = 0;
		Dictionary<Vector3,int> oVertexPosIndexDict = new Dictionary<Vector3, int>( iGridVertexCount );

		// Mesh data
		List<int> oGridPlaneTriangleList = new List<int>( 6 * iGridVertexCount );	// quad = 2 tris, 1 tri = 3 vertices => 2 * 3
		//List<Vector2> oGridPlaneUVList   = new List<Vector2>( iGridVertexCount );

		// Iterate grid divs
		for( int iGridSubDivIndex = 0; iGridSubDivIndex < iGridSubDivCount; ++iGridSubDivIndex )
		{
			// ( X, Y ) grid div pos from grid div index (in grid div space)
			int iGridSubDivX = iGridSubDivIndex % iGridSubDivPerRow;
			int iGridSubDivY = iGridSubDivIndex / iGridSubDivPerRow;
			
			// Compute the pixel bounds for this subdivision
			int iStartX = Mathf.FloorToInt(iGridSubDivX * fGridSubDivPixelWidth);
			int iStartY = Mathf.FloorToInt(iGridSubDivY * fGridSubDivPixelHeight);
			
			int iEndX = Mathf.CeilToInt((iGridSubDivX + 1) * fGridSubDivPixelWidth);
			int iEndY = Mathf.CeilToInt((iGridSubDivY + 1) * fGridSubDivPixelHeight);
			
			int iWidth = iEndX - iStartX;
			int iHeight = iEndY - iStartY;
			
			// Set grid sub div as empty while no significant pixel found
			bool bEmptyGridSubDiv = true;
			for( int iY = 0; bEmptyGridSubDiv && iY < iHeight; ++iY )
			{
				for( int iX = 0; bEmptyGridSubDiv && iX < iWidth; ++iX )
				{
					int iPixelIndex = a_rBinarizedImage.Coords2PixelIndex( iX + iStartX, iY + iStartY );
					//int iPixelIndex = Mathf.FloorToInt( iGridSubDivY * fGridSubDivPixelHeight ) * iTextureWidth + Mathf.FloorToInt( iGridSubDivX * fGridSubDivPixelWidth ) + iY * iTextureWidth + iX;
					//bEmptyGridSubDiv = a_rBinarizedImage[ iPixelIndex ];

					// At least one pixel is significant => need to create the whole grid div
					if( a_rBinarizedImage[ iPixelIndex ] )
					{
						/*
						 * Grid div
						 * 
						 *	       ...       ...
						 *	        |         |
						 *	... --- TL ------- TR --- ...
						 *	        |   T2  / |
						 *	        |     /   |
						 *	        |   /     |
						 *	        | /   T1  |
						 *	... --- BL ------- BR --- ...
						 * 	        |         |
						 * 	       ...       ...
						 */
						Vector3 f3BottomLeftVertex  = new Vector3(         iGridSubDivX * fGridSubDivPixelWidth,         iGridSubDivY * fGridSubDivPixelHeight, 0.0f );	// BL
						Vector3 f3BottomRightVertex = new Vector3( ( iGridSubDivX + 1 ) * fGridSubDivPixelWidth,         iGridSubDivY * fGridSubDivPixelHeight, 0.0f );	// BR
						Vector3 f3TopRightVertex    = new Vector3( ( iGridSubDivX + 1 ) * fGridSubDivPixelWidth, ( iGridSubDivY + 1 ) * fGridSubDivPixelHeight, 0.0f );	// TR
						Vector3 f3TopLeftVertex     = new Vector3(         iGridSubDivX * fGridSubDivPixelWidth, ( iGridSubDivY + 1 ) * fGridSubDivPixelHeight, 0.0f );	// TL
						
						int iBottomLeftVertexIndex;
						int iBottomRightVertexIndex;
						int iTopRightVertexIndex;
						int iTopLeftVertexIndex;
						
						// For each grid div vertex, query its index if already created (the vertex might be shared with other grid divs).
						// If not, create it.
						if( oVertexPosIndexDict.TryGetValue( f3BottomLeftVertex, out iBottomLeftVertexIndex ) == false )
						{
							iBottomLeftVertexIndex = iCurrentVertexIndex++;
							oVertexPosIndexDict.Add( f3BottomLeftVertex, iBottomLeftVertexIndex );
							//oGridPlaneUVList.Add( new Vector2( f3BottomLeftVertex.x / (float) iTextureWidth, f3BottomLeftVertex.y / (float) iTextureHeight ) );
						}
						
						if( oVertexPosIndexDict.TryGetValue( f3BottomRightVertex, out iBottomRightVertexIndex ) == false )
						{
							iBottomRightVertexIndex = iCurrentVertexIndex++;
							oVertexPosIndexDict.Add( f3BottomRightVertex, iBottomRightVertexIndex );
							//oGridPlaneUVList.Add( new Vector2( f3BottomRightVertex.x / (float) iTextureWidth, f3BottomRightVertex.y / (float) iTextureHeight ) );
						}
						
						if( oVertexPosIndexDict.TryGetValue( f3TopRightVertex, out iTopRightVertexIndex ) == false )
						{
							iTopRightVertexIndex = iCurrentVertexIndex++;
							oVertexPosIndexDict.Add( f3TopRightVertex, iTopRightVertexIndex );
							//oGridPlaneUVList.Add( new Vector2( f3TopRightVertex.x / (float) iTextureWidth, f3TopRightVertex.y / (float) iTextureHeight ) );
						}
						
						if( oVertexPosIndexDict.TryGetValue( f3TopLeftVertex, out iTopLeftVertexIndex ) == false )
						{
							iTopLeftVertexIndex = iCurrentVertexIndex++;
							oVertexPosIndexDict.Add( f3TopLeftVertex, iTopLeftVertexIndex );
							//oGridPlaneUVList.Add( new Vector2( f3TopLeftVertex.x / (float) iTextureWidth, f3TopLeftVertex.y / (float) iTextureHeight ) );
						}
						
						// Create grid div triangles
						// First triangle (T1)
						oGridPlaneTriangleList.Add( iBottomRightVertexIndex );	// BR
						oGridPlaneTriangleList.Add( iBottomLeftVertexIndex );	// BL
						oGridPlaneTriangleList.Add( iTopRightVertexIndex );		// TR
						
						// Second triangle (T2)
						oGridPlaneTriangleList.Add( iBottomLeftVertexIndex );	// BL
						oGridPlaneTriangleList.Add( iTopLeftVertexIndex );		// TL
						oGridPlaneTriangleList.Add( iTopRightVertexIndex );		// TR

						// Set grid sub div as not empty
						bEmptyGridSubDiv = false;
					}
				}
			}
		}

		// Apply pivot + compute UVs
		int iVertexCount = oVertexPosIndexDict.Count;
		Vector2[ ] oGridPlaneUVs      = new Vector2[ iVertexCount ];
		Vector3[ ] oGridPlaneVertices = new Vector3[ iVertexCount ];
		Vector2 f2Dimensions          = new Vector2( 1.0f / (float) iTextureWidth, 1.0f / (float) iTextureHeight );

		foreach( KeyValuePair<Vector3,int> rVertexPosIndexPair in oVertexPosIndexDict )
		{
			Vector3 f3VertexPos = rVertexPosIndexPair.Key;
			int iIndex = rVertexPosIndexPair.Value;

			oGridPlaneUVs[ iIndex ]      = new Vector2( f3VertexPos.x * f2Dimensions.x, f3VertexPos.y * f2Dimensions.y );
			oGridPlaneVertices[ iIndex ] = ( f3VertexPos - a_f3PivotPoint ) * a_fScale;
		}
		
		oGridPlaneMesh.vertices  = oGridPlaneVertices;
		oGridPlaneMesh.uv        = oGridPlaneUVs;
		oGridPlaneMesh.triangles = oGridPlaneTriangleList.ToArray( );	// LINQ

		return oGridPlaneMesh;
	}

	// Returns an extruded 3D polygonized mesh from 2D outer/inner contours
	public static Mesh PolygonizeAndExtrudeContours( List<Contour> a_rDominantContoursList, float a_fExtrusionDepth, float a_fScale, Vector3 a_f3PivotPoint )
	{
		// The mesh to build
		Mesh oCombinedMesh = new Mesh( );

		int iContourCount = a_rDominantContoursList.Count; //a_rDominantContoursList.Count;

		// Step 2: Ear clip outer contour
		CombineInstance[ ] oCombineMeshInstance = new CombineInstance[ iContourCount ];

		for( int iContourIndex = 0; iContourIndex < iContourCount; ++iContourIndex )
		{
			Vector3[ ] oVerticesArray;
			int[ ] oTrianglesArray;
			Vector2[ ] oUVs;

			EarClipping( a_rDominantContoursList[ iContourIndex ], a_fScale, a_f3PivotPoint, 1.0f, 1.0f, out oVerticesArray, out oTrianglesArray, out oUVs );

			oCombineMeshInstance[ iContourIndex ].mesh = BuildExtrudedMeshFromPolygonizedContours( oVerticesArray, oTrianglesArray, a_fExtrusionDepth ); // EarClippingSubMesh( a_rDominantContoursList[ iContourIndex ] );
		}

		// Step 3: Combine every sub meshes (merge, no transform)
		oCombinedMesh.CombineMeshes( oCombineMeshInstance, true, false );

		// Return!
		return oCombinedMesh;
	}

	// Returns a list of extruded 3D triangles meshes from 2D outer/inner contours
	public static List<Mesh> CompoundPolygonizeAndExtrudeContours( List<Contour> a_rDominantContoursList, float a_fExtrusionDepth, float a_fScale, Vector3 a_f3PivotPoint )
	{
		List<Mesh> oMeshesList = new List<Mesh>( );

		foreach( Contour rDominantContour in a_rDominantContoursList )
		{
			Vector3[ ] oVerticesArray;
			int[ ] oTrianglesArray;
			Vector2[ ] oUVs;

			EarClipping( rDominantContour, a_fScale, a_f3PivotPoint, 1.0f, 1.0f, out oVerticesArray, out oTrianglesArray, out oUVs );
			List<Mesh> rExtrudedTriangleMeshesList = BuildExtrudedTrianglesFromPolygonizedContours( oVerticesArray, oTrianglesArray, a_fExtrusionDepth );
			oMeshesList.AddRange( rExtrudedTriangleMeshesList );
		}

		return oMeshesList;
	}
	
	// Returns a polygonized mesh from a 2D outer contour
	private static void EarClipping( Contour a_rDominantOuterContour, float a_fScale, Vector3 a_f3PivotPoint, float a_fWidth, float a_fHeight, out Vector3[ ] a_rVerticesArray, out int[ ] a_rTrianglesArray, out Vector2[ ] a_rUVs )
	{
		// Sum of all contours count
		int iVerticesCount = a_rDominantOuterContour.Count;

		// Mesh vertices array
		a_rVerticesArray = new Vector3[ iVerticesCount ];

		// Mesh UVs array
		a_rUVs           = new Vector2[ iVerticesCount ];

		// Vertex indexes lists array (used by ear clipping algorithm)
		CircularLinkedList<int> oVertexIndexesList = new CircularLinkedList<int>( );

		// Build contour vertex index circular list
		// Store every Vector3 into mesh vertices array
		// Store corresponding index into the circular list
		int iVertexIndex = 0;
		foreach( Vector2 f2OuterContourVertex in a_rDominantOuterContour.Vertices )
		{
			a_rVerticesArray[ iVertexIndex ] = f2OuterContourVertex;
			oVertexIndexesList.AddLast( iVertexIndex );

			++iVertexIndex;
		}

		// Build reflex/convex vertices lists
		LinkedList<int> rReflexVertexIndexesList;
		LinkedList<int> rConvexVertexIndexesList;
		BuildReflexConvexVertexIndexesLists( a_rVerticesArray, oVertexIndexesList, out rReflexVertexIndexesList, out rConvexVertexIndexesList );

		// Triangles for this contour
		List<int> oTrianglesList = new List<int>( 3 * iVerticesCount );

		// Build ear tips list
		CircularLinkedList<int> rEarTipVertexIndexesList = BuildEarTipVerticesList( a_rVerticesArray, oVertexIndexesList, rReflexVertexIndexesList, rConvexVertexIndexesList );

		// Remove the ear tips one by one!
		while( rEarTipVertexIndexesList.Count > 0 && oVertexIndexesList.Count > 2 )
		{

			CircularLinkedListNode<int> rEarTipNode = rEarTipVertexIndexesList.First;

			// Ear tip index
			int iEarTipVertexIndex = rEarTipNode.Value;

			// Ear vertex indexes
			CircularLinkedListNode<int> rContourVertexNode                 = oVertexIndexesList.Find( iEarTipVertexIndex );
			CircularLinkedListNode<int> rPreviousAdjacentContourVertexNode = rContourVertexNode.Previous;
			CircularLinkedListNode<int> rNextAdjacentContourVertexNode     = rContourVertexNode.Next;

			int iPreviousAjdacentContourVertexIndex = rPreviousAdjacentContourVertexNode.Value;
			int iNextAdjacentContourVertexIndex     = rNextAdjacentContourVertexNode.Value;
		
			// Add the ear triangle to our triangles list
			oTrianglesList.Add( iPreviousAjdacentContourVertexIndex );
			oTrianglesList.Add( iEarTipVertexIndex );
			oTrianglesList.Add( iNextAdjacentContourVertexIndex );

			// Remove the ear tip from vertices / convex / ear lists
			oVertexIndexesList.Remove( iEarTipVertexIndex );
			rConvexVertexIndexesList.Remove( iEarTipVertexIndex );

			// Adjacent n-1 vertex
			// if was convex => remain convex, can possibly an ear
			// if was an ear => can possibly not remain an ear
			// if was reflex => can possibly convex and possibly an ear
			if( rReflexVertexIndexesList.Contains( iPreviousAjdacentContourVertexIndex ) )
			{
				CircularLinkedListNode<int> rPreviousPreviousAdjacentContourVertexNode = rPreviousAdjacentContourVertexNode.Previous;

				Vector3 f3AdjacentContourVertex         = a_rVerticesArray[ rPreviousAdjacentContourVertexNode.Value ];
				Vector3 f3PreviousAdjacentContourVertex = a_rVerticesArray[ rPreviousPreviousAdjacentContourVertexNode.Value ];
				Vector3 f3NextAdjacentContourVertex     = a_rVerticesArray[ rPreviousAdjacentContourVertexNode.Next.Value ];

				if( IsReflexVertex( f3AdjacentContourVertex, f3PreviousAdjacentContourVertex, f3NextAdjacentContourVertex ) == false )
				{
					rReflexVertexIndexesList.Remove( iPreviousAjdacentContourVertexIndex );
					rConvexVertexIndexesList.AddFirst( iPreviousAjdacentContourVertexIndex );
				}
			}

			// Adjacent n+1 vertex
			// if was convex => remain convex, can possibly an ear
			// if was an ear => can possibly not remain an ear
			// if was reflex => can possibly convex and possibly an ear
			if( rReflexVertexIndexesList.Contains( iNextAdjacentContourVertexIndex ) )
			{
				CircularLinkedListNode<int> rNextNextAdjacentContourVertexNode = rNextAdjacentContourVertexNode.Next;

				Vector3 f3AdjacentContourVertex         = a_rVerticesArray[ rNextAdjacentContourVertexNode.Value ];
				Vector3 f3PreviousAdjacentContourVertex = a_rVerticesArray[ rNextAdjacentContourVertexNode.Previous.Value ];
				Vector3 f3NextAdjacentContourVertex     = a_rVerticesArray[ rNextNextAdjacentContourVertexNode.Value ];

				if( IsReflexVertex( f3AdjacentContourVertex, f3PreviousAdjacentContourVertex, f3NextAdjacentContourVertex ) == false )
				{
					rReflexVertexIndexesList.Remove( iNextAdjacentContourVertexIndex );
					rConvexVertexIndexesList.AddFirst( iNextAdjacentContourVertexIndex );
				}
			}

			if( rConvexVertexIndexesList.Contains( iPreviousAjdacentContourVertexIndex ) )
			{
				if( IsEarTip( a_rVerticesArray, iPreviousAjdacentContourVertexIndex, oVertexIndexesList, rReflexVertexIndexesList ) )
				{
					if( rEarTipVertexIndexesList.Contains( iPreviousAjdacentContourVertexIndex ) == false )
					{
						rEarTipVertexIndexesList.AddLast( iPreviousAjdacentContourVertexIndex );
					}
				}
				else
				{
					rEarTipVertexIndexesList.Remove( iPreviousAjdacentContourVertexIndex );
				}
			}

			if( rConvexVertexIndexesList.Contains( iNextAdjacentContourVertexIndex ) )
			{
				if( IsEarTip( a_rVerticesArray, iNextAdjacentContourVertexIndex, oVertexIndexesList, rReflexVertexIndexesList ) )
				{
					if( rEarTipVertexIndexesList.Contains( iNextAdjacentContourVertexIndex ) == false )
					{
						rEarTipVertexIndexesList.AddFirst( iNextAdjacentContourVertexIndex );
					}
				}
				else
				{
					rEarTipVertexIndexesList.Remove( iNextAdjacentContourVertexIndex );
				}
			}

			rEarTipVertexIndexesList.Remove( iEarTipVertexIndex );
		}

		// Create UVs, rescale vertices, apply pivot
		Vector2 f2Dimensions = new Vector2( 1.0f / a_fWidth, 1.0f / a_fHeight );
		for( iVertexIndex = 0; iVertexIndex < iVerticesCount; ++iVertexIndex )
		{
			Vector3 f3VertexPos = a_rVerticesArray[ iVertexIndex ];

			//a_rUVs[ iVertexIndex ] = Vector2.Scale( f3VertexPos, f2Dimensions );
			a_rUVs[ iVertexIndex ]           = new Vector2( f3VertexPos.x * f2Dimensions.x, f3VertexPos.y * f2Dimensions.y );
			a_rVerticesArray[ iVertexIndex ] = ( f3VertexPos - a_f3PivotPoint ) * a_fScale;
		}

		a_rTrianglesArray = oTrianglesList.ToArray( );
	}

	// Builds indexes lists of reflex and convex vertex
	private static void BuildReflexConvexVertexIndexesLists( Vector3[ ] a_rContourVerticesArray, CircularLinkedList<int> a_rContourVertexIndexesList, out LinkedList<int> a_rReflexVertexIndexesList, out LinkedList<int> a_rConvexVertexIndexesList )
	{
		LinkedList<int> oReflexVertexIndexesList = new LinkedList<int>( );
		LinkedList<int> oConvexVertexIndexesList = new LinkedList<int>( );

		// Iterate contour vertices
		CircularLinkedListNode<int> rContourNode = a_rContourVertexIndexesList.First;
		do
		{
			int iContourVertexIndex         = rContourNode.Value;
			Vector3 f3ContourVertex         = a_rContourVerticesArray[ iContourVertexIndex ];
			Vector3 f3PreviousContourVertex = a_rContourVerticesArray[ rContourNode.Previous.Value ];
			Vector3 f3NextContourVertex     = a_rContourVerticesArray[ rContourNode.Next.Value ];

			// Sorting reflex / convex vertices
			// Reflex vertex forms a triangle with an angle >= 180°
			if( IsReflexVertex( f3ContourVertex, f3PreviousContourVertex, f3NextContourVertex ) == true )
			{
				oReflexVertexIndexesList.AddLast( iContourVertexIndex );
			}
			else // angle < 180° => Convex vertex
			{
				oConvexVertexIndexesList.AddLast( iContourVertexIndex );
			}

			rContourNode = rContourNode.Next;
		}
		while( rContourNode != a_rContourVertexIndexesList.First );

		// Transmit results
		a_rReflexVertexIndexesList = oReflexVertexIndexesList;
		a_rConvexVertexIndexesList = oConvexVertexIndexesList;
	}

	// Finds a pair of inner contour vertex / outer contour vertex that are mutually visible
	public static Contour InsertInnerContourIntoOuterContour( Contour a_rOuterContour, Contour a_rInnerContour )
	{
		// Look for the inner vertex of maximum x-value
		Vector2 f2InnerContourVertexMax = Vector2.one * int.MinValue;
		CircularLinkedListNode<Vector2> rMutualVisibleInnerContourVertexNode = null;

		CircularLinkedList<Vector2> rInnerContourVertexList = a_rInnerContour.Vertices;
		CircularLinkedListNode<Vector2> rInnerContourVertexNode = rInnerContourVertexList.First;
		
		do
		{
			// x-value
			Vector2 f2InnerContourVertex = rInnerContourVertexNode.Value;

			// New max x found
			if( f2InnerContourVertexMax.x < f2InnerContourVertex.x )
			{
				f2InnerContourVertexMax = f2InnerContourVertex;
				rMutualVisibleInnerContourVertexNode = rInnerContourVertexNode;
			}

			// Go to next vertex
			rInnerContourVertexNode = rInnerContourVertexNode.Next;
		}
		while( rInnerContourVertexNode != rInnerContourVertexList.First );

		// Visibility ray
		Ray oInnerVertexVisibilityRay = new Ray( f2InnerContourVertexMax, Vector3.right );
		float fClosestDistance = int.MaxValue;
		Vector2 f2ClosestOuterEdgeStart = Vector2.zero;
		Vector2 f2ClosestOuterEdgeEnd = Vector2.zero;

		Contour rOuterCutContour = new Contour( a_rOuterContour.Region );
		rOuterCutContour.AddLast( a_rOuterContour.Vertices );

		CircularLinkedList<Vector2> rOuterCutContourVertexList = rOuterCutContour.Vertices;
		CircularLinkedListNode<Vector2> rOuterContourVertexEdgeStart = null;

		// Raycast from the inner contour vertex to every edge
		CircularLinkedListNode<Vector2> rOuterContourVertexNode = rOuterCutContourVertexList.First;
		do
		{
			// Construct outer edge from current and next outer contour vertices
			Vector2 f2OuterEdgeStart = rOuterContourVertexNode.Value;
			Vector2 f2OuterEdgeEnd = rOuterContourVertexNode.Next.Value;
			Vector2 f2OuterEdge = f2OuterEdgeEnd - f2OuterEdgeStart;

			// Orthogonal vector to edge (pointing to polygon interior)
			Vector2 f2OuterEdgeNormal = Uni2DMathUtils.PerpVector2( f2OuterEdge );

			// Vector from edge start to inner vertex
			Vector2 f2OuterEdgeStartToInnerVertex = f2InnerContourVertexMax - f2OuterEdgeStart;

			// If the inner vertex is on the left of the edge (interior),
			// test if there's any intersection
			if( Vector2.Dot( f2OuterEdgeStartToInnerVertex, f2OuterEdgeNormal ) >= 0.0f )
			{
				float fDistanceT;

				// If visibility intersects outer edge... 
				if( Uni2DMathUtils.Raycast2DSegment( oInnerVertexVisibilityRay, f2OuterEdgeStart, f2OuterEdgeEnd, out fDistanceT ) == true )
				{
					// Is it the closest intersection we found?
					if( fClosestDistance > fDistanceT )
					{
						fClosestDistance = fDistanceT;
						rOuterContourVertexEdgeStart = rOuterContourVertexNode;

						f2ClosestOuterEdgeStart = f2OuterEdgeStart;
						f2ClosestOuterEdgeEnd = f2OuterEdgeEnd;
					}
				}
			}

			// Go to next edge
			rOuterContourVertexNode = rOuterContourVertexNode.Next;
		}
		while( rOuterContourVertexNode != rOuterCutContourVertexList.First );

		// Take the vertex of maximum x-value from the closest intersected edge
		Vector2 f2ClosestVisibleOuterContourVertex;
		CircularLinkedListNode<Vector2> rMutualVisibleOuterContourVertexNode;
		if( f2ClosestOuterEdgeStart.x < f2ClosestOuterEdgeEnd.x )
		{
			f2ClosestVisibleOuterContourVertex = f2ClosestOuterEdgeEnd;
			rMutualVisibleOuterContourVertexNode = rOuterContourVertexEdgeStart.Next;
		}
		else
		{
			f2ClosestVisibleOuterContourVertex = f2ClosestOuterEdgeStart;
			rMutualVisibleOuterContourVertexNode = rOuterContourVertexEdgeStart;
		}

		// Looking for points inside the triangle defined by inner vertex, intersection point an closest outer vertex
		// If a point is inside this triangle, at least one is a reflex vertex
		// The closest reflex vertex which minimises the angle this-vertex/inner vertex/intersection vertex
		// would be choosen as the mutual visible vertex
		Vector3 f3IntersectionPoint = oInnerVertexVisibilityRay.GetPoint( fClosestDistance );
		Vector2 f2InnerContourVertexToIntersectionPoint = new Vector2( f3IntersectionPoint.x, f3IntersectionPoint.y ) - f2InnerContourVertexMax;
		Vector2 f2NormalizedInnerContourVertexToIntersectionPoint = f2InnerContourVertexToIntersectionPoint.normalized;

		float fMaxDotAngle = float.MinValue;
		float fMinDistance = float.MaxValue;
		rOuterContourVertexNode = rOuterCutContourVertexList.First;
		do
		{
			Vector2 f2OuterContourVertex = rOuterContourVertexNode.Value;

			// if vertex not part of triangle
			if( f2OuterContourVertex != f2ClosestVisibleOuterContourVertex )
			{
				// if vertex is inside triangle...
				if( Uni2DMathUtils.IsPointInsideTriangle( f2InnerContourVertexMax, f3IntersectionPoint, f2ClosestVisibleOuterContourVertex, f2OuterContourVertex ) == true )
				{
					// if vertex is reflex
					Vector2 f2PreviousOuterContourVertex = rOuterContourVertexNode.Previous.Value;
					Vector2 f2NextOuterContourVertex = rOuterContourVertexNode.Next.Value;
	
					if( IsReflexVertex( f2OuterContourVertex, f2PreviousOuterContourVertex, f2NextOuterContourVertex ) == true )
					{
						// Use dot product as distance
						Vector2 f2InnerContourVertexToReflexVertex = f2OuterContourVertex - f2InnerContourVertexMax;

						// INFO: f2NormalizedInnerContourVertexToIntersectionPoint == Vector3.right (if everything is right)
						float fDistance = Vector2.Dot( f2NormalizedInnerContourVertexToIntersectionPoint, f2InnerContourVertexToReflexVertex );
						float fDotAngle = Vector2.Dot( f2NormalizedInnerContourVertexToIntersectionPoint, f2InnerContourVertexToReflexVertex.normalized );

						// New mutual visible vertex if angle smaller (e.g. dot angle larger) than min or equal and closer
						if( fDotAngle > fMaxDotAngle || ( fDotAngle == fMaxDotAngle && fDistance < fMinDistance ) )
						{
							fMaxDotAngle = fDotAngle;
							fMinDistance = fDistance;
							rMutualVisibleOuterContourVertexNode = rOuterContourVertexNode;
						}
					}
				}
			}

			// Go to next vertex
			rOuterContourVertexNode = rOuterContourVertexNode.Next;
		}
		while( rOuterContourVertexNode != rOuterCutContourVertexList.First );

		// Insert now the cut into the polygon
		// The cut starts from the outer contour mutual visible vertex to the inner vertex
		CircularLinkedListNode<Vector2> rOuterContourVertexNodeToInsertBefore = rMutualVisibleOuterContourVertexNode.Next;

		// Loop over the inner contour starting from the inner contour vertex...
		rInnerContourVertexNode = rMutualVisibleInnerContourVertexNode;
		do
		{
			// ... add the inner contour vertex before the outer contour vertex after the cut
			rOuterCutContourVertexList.AddBefore( rOuterContourVertexNodeToInsertBefore, rInnerContourVertexNode.Value );
			rInnerContourVertexNode = rInnerContourVertexNode.Next;
		}
		while( rInnerContourVertexNode != rMutualVisibleInnerContourVertexNode );

		// Close the cut by doubling the inner and outer contour vertices
		rOuterCutContourVertexList.AddBefore( rOuterContourVertexNodeToInsertBefore, rMutualVisibleInnerContourVertexNode.Value );
		rOuterCutContourVertexList.AddBefore( rOuterContourVertexNodeToInsertBefore, rMutualVisibleOuterContourVertexNode.Value );

		return rOuterCutContour;
	}

	// Returns true if the vertex I is a reflex vertex, i.e. the angle JÎH is >= 180°
	private static bool IsReflexVertex( Vector3 a_f3VertexI, Vector3 a_f3VertexJ, Vector3 a_f3VertexH )
	{
		Vector3 f3SegmentJI = a_f3VertexI - a_f3VertexJ;
		Vector3 f3SegmentIH = a_f3VertexH - a_f3VertexI;
		Vector3 f3JINormal  = new Vector3( f3SegmentJI.y, - f3SegmentJI.x, 0 );

		return Vector3.Dot( f3SegmentIH, f3JINormal ) < 0.0f;
	}

	// Builds and return a list of vertex indexes that are ear tips.
	private static CircularLinkedList<int> BuildEarTipVerticesList( Vector3[ ] a_rMeshVertices, CircularLinkedList<int> a_rOuterContourVertexIndexesList, LinkedList<int> a_rReflexVertexIndexesList, LinkedList<int> a_rConvexVertexIndexesList )
	{
		CircularLinkedList<int> oEarTipVertexIndexesList = new CircularLinkedList<int>( );

		// Iterate convex vertices
		for( LinkedListNode<int> rConvexIndexNode = a_rConvexVertexIndexesList.First; rConvexIndexNode != null; rConvexIndexNode = rConvexIndexNode.Next )
		{
			// The convex vertex index
			int iConvexContourVertexIndex = rConvexIndexNode.Value;

			// Is the convex vertex is an ear tip?
			if( IsEarTip( a_rMeshVertices, iConvexContourVertexIndex, a_rOuterContourVertexIndexesList, a_rReflexVertexIndexesList ) == true )
			{
				// Yes: adds it to the list
				oEarTipVertexIndexesList.AddLast( iConvexContourVertexIndex );
			}
		}

		// Return the ear tip list
		return oEarTipVertexIndexesList;
	}

	// Returns true if the specified convex vertex is an ear tip
	private static bool IsEarTip( Vector3[ ] a_rMeshVertices, int a_iEarTipConvexVertexIndex, CircularLinkedList<int> a_rContourVertexIndexesList, LinkedList<int> a_rReflexVertexIndexesList )
	{
		CircularLinkedListNode<int> rContourVertexNode = a_rContourVertexIndexesList.Find( a_iEarTipConvexVertexIndex );

		int iPreviousContourVertexIndex = rContourVertexNode.Previous.Value;
		int iNextContourVertexIndex = rContourVertexNode.Next.Value;

		// Retrieve previous (i-1) / current (i) / next (i+1) vertices to form the triangle < Vi-1, Vi, Vi+1 >
		Vector3 f3ConvexContourVertex   = a_rMeshVertices[ a_iEarTipConvexVertexIndex ];
		Vector3 f3PreviousContourVertex = a_rMeshVertices[ iPreviousContourVertexIndex ];
		Vector3 f3NextContourVertex     = a_rMeshVertices[ iNextContourVertexIndex ];

		// Look for an inner point into the triangle formed by the 3 vertices
		// Only need to look over the reflex vertices.
		for( LinkedListNode<int> rReflexIndexNode = a_rReflexVertexIndexesList.First; rReflexIndexNode != null; rReflexIndexNode = rReflexIndexNode.Next )
		{
			// Retrieve the reflex vertex
			int iReflexContourVertexIndex = rReflexIndexNode.Value;

			// Is the point inside the triangle?
			// (Exclude the triangle points themselves)
			Vector3 f3ReflexContourVertex = a_rMeshVertices[ iReflexContourVertexIndex ];
			if( f3ReflexContourVertex != f3PreviousContourVertex && f3ReflexContourVertex != f3ConvexContourVertex && f3ReflexContourVertex != f3NextContourVertex )
			{
				if( Uni2DMathUtils.IsPointInsideTriangle( f3PreviousContourVertex, f3ConvexContourVertex, f3NextContourVertex, f3ReflexContourVertex ) == true )
				{
					// Point is inside triangle: not an ear tip
					return false;
				}
			}
		}

		// No point inside the triangle: ear tip found!
		return true;
	}

	// Builds a circular linked list with a_iIndexesCount index, from 0 to a_iIndexesCount - 1
	private static CircularLinkedList<int> BuildContourVertexIndexesList( int a_iIndexesCount, int a_iIndexOffset )
	{
		CircularLinkedList<int> oVertexIndexesList = new CircularLinkedList<int>( );

		for( int iIndex = 0; iIndex < a_iIndexesCount; ++iIndex )
		{
			oVertexIndexesList.AddLast( iIndex + a_iIndexesCount );
		}

		return oVertexIndexesList;
	}

	private static List<Mesh> BuildExtrudedTrianglesFromPolygonizedContours( Vector3[ ] a_rMeshVerticesArray, int[ ] a_rTriangleVertexIndexesArray, float a_fExtrusionDepth )
	{
		int iTriangleVertexIndexCount     = a_rTriangleVertexIndexesArray.Length;
		Vector3[ ] oTriangleVerticesArray = new Vector3[ 3 ];
		List<Mesh> oTriangleMeshesList    = new List<Mesh>( iTriangleVertexIndexCount / 3 );

		int[ ] oTrianglesVertexIndexesArray = new int[ 3 ]{ 0, 1, 2 };

		for( int iTriangleVertexIndex = 0; iTriangleVertexIndex < iTriangleVertexIndexCount; iTriangleVertexIndex += 3 )
		{
			int iTriangleVertexIndexA = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex     ];
			int iTriangleVertexIndexB = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 1 ];
			int iTriangleVertexIndexC = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 2 ];

			oTriangleVerticesArray[ 0 ] = a_rMeshVerticesArray[ iTriangleVertexIndexA ];
			oTriangleVerticesArray[ 1 ] = a_rMeshVerticesArray[ iTriangleVertexIndexB ];
			oTriangleVerticesArray[ 2 ] = a_rMeshVerticesArray[ iTriangleVertexIndexC ];

			Mesh rTriangleMesh = BuildExtrudedMeshFromPolygonizedContours( oTriangleVerticesArray, oTrianglesVertexIndexesArray, a_fExtrusionDepth );
			oTriangleMeshesList.Add( rTriangleMesh );
		}

		return oTriangleMeshesList;
	}

	private static Mesh BuildExtrudedMeshFromPolygonizedContours( Vector3[ ] a_rMeshVerticesArray, int[ ] a_rTriangleVertexIndexesArray, float a_fExtrusionDepth )
	{
		// Copy mesh vertices
		int iTriangleVertexIndexesCount   = a_rTriangleVertexIndexesArray.Length;
		int iVerticesCount                = a_rMeshVerticesArray.Length;
		int iExtrudedVerticesCount        = 2 * iVerticesCount;

		Vector3[ ] oExtrudedVerticesArray = new Vector3[ iExtrudedVerticesCount ];
		//a_rMeshVerticesArray.CopyTo( oExtrudedVerticesArray, 0 );
		
		// Create the extrusion direction (half backward, half forward)
		Vector3 f3ExtrusionDepthDirection = a_fExtrusionDepth * 0.5f * Vector3.forward;

		// Build extruded vertices
		for( int iVertexIndex = 0; iVertexIndex < iVerticesCount; ++iVertexIndex )
		{
			Vector3 f3Vertex = a_rMeshVerticesArray[ iVertexIndex ];

			oExtrudedVerticesArray[ iVertexIndex                  ] = f3Vertex - f3ExtrusionDepthDirection;
			oExtrudedVerticesArray[ iVertexIndex + iVerticesCount ] = f3Vertex + f3ExtrusionDepthDirection;
		}

		// Copy mesh triangles
		// Copy original triangle vertex indexes
		int[ ] oExtrudedTriangleVertexIndexesArray = new int[ 2 * iTriangleVertexIndexesCount + 6 * iVerticesCount ];

		for( int iTriangleVertexIndex = 0; iTriangleVertexIndex < iTriangleVertexIndexesCount; iTriangleVertexIndex += 3 )
		{
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex     ] = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex     ];
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex + 1 ] = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 1 ];
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex + 2 ] = a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 2 ];
		}

		// Build extruded triangle vertex indexes
		// CW to CCW
		for( int iTriangleVertexIndex = 0; iTriangleVertexIndex < iTriangleVertexIndexesCount; iTriangleVertexIndex += 3 )
		{				
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex + iTriangleVertexIndexesCount     ] = iVerticesCount + a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 1 ];
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex + iTriangleVertexIndexesCount + 1 ] = iVerticesCount + a_rTriangleVertexIndexesArray[ iTriangleVertexIndex     ];
			oExtrudedTriangleVertexIndexesArray[ iTriangleVertexIndex + iTriangleVertexIndexesCount + 2 ] = iVerticesCount + a_rTriangleVertexIndexesArray[ iTriangleVertexIndex + 2 ];
		}

		// Build jointure band
		int iJointureBandIndex = iTriangleVertexIndexesCount * 2;
		for( int iTriangleVertexIndex = 0; iTriangleVertexIndex < iVerticesCount; ++iTriangleVertexIndex )
		{
			int iCurrentIndex = 6 * iTriangleVertexIndex + iJointureBandIndex;
			int iNextContourTriangleVertexIndex = ( iTriangleVertexIndex + 1 ) % iVerticesCount;

			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex     ] = iTriangleVertexIndex;	// nA
			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex + 1 ] = iTriangleVertexIndex + iVerticesCount;	// nB
			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex + 2 ] = iNextContourTriangleVertexIndex;	// nA+1

			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex + 3 ] = iNextContourTriangleVertexIndex;	// nA+1
			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex + 4 ] = iTriangleVertexIndex + iVerticesCount;	// nB
			oExtrudedTriangleVertexIndexesArray[ iCurrentIndex + 5 ] = iNextContourTriangleVertexIndex + iVerticesCount;	// nB+1
		}

		// Build mesh
		Mesh oExtrudedMesh = new Mesh( );
		oExtrudedMesh.vertices  = oExtrudedVerticesArray;
		oExtrudedMesh.uv        = new Vector2[ iExtrudedVerticesCount ];	// Dummy UVs to prevent info messages...
		oExtrudedMesh.triangles = oExtrudedTriangleVertexIndexesArray;

		return oExtrudedMesh;
	}
}
#endif
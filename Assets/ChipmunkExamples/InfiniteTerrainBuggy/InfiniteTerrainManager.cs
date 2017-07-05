// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InfiniteTerrainManager : MonoBehaviour {
	
	public GameObject trackThis;
	public GameObject terrainChunkPrefab;
	
	public int SEGMENTS_PER_GROUP = 10;
	
	public int loadGroupsOnEitherSide = 2;
	public int unloadGroupsFurtherThan = 3;
	public float lengthPerSegment = 2f;
	
	public Dictionary<float, Transform> loadedGroups = new Dictionary<float, Transform>();
	public float scaleOctave1X = 1f;
	public float scaleOctave2X = 1f;
	public float scaleOctave1Y = 1f;
	public float scaleOctave2Y = 1f;
	
	public void Update(){
		
		// find the current location of the player, and make sure we have three groups of segments present.
		Vector3 playerPos = trackThis.transform.position;
		
		float len = SEGMENTS_PER_GROUP * lengthPerSegment;
		float groupStartX = Mathf.Floor(playerPos.x / len) * len;
		
		// Load a few groups on either side of the player, if they aren't loaded.
		for(int i = -loadGroupsOnEitherSide; i < loadGroupsOnEitherSide; i++){	
			float x = groupStartX + i * len;
			if(!loadedGroups.ContainsKey(x)){	
				loadedGroups[x] = CreateNewGeometryAt(x);
			}
		}
		
		List<float> removeThese = new List<float>();
		// Now unload groups that are too far away:
		foreach(var key in loadedGroups.Keys){
			if(Mathf.Abs(key - playerPos.x) > unloadGroupsFurtherThan * len){
				Destroy(loadedGroups[key].gameObject);
				removeThese.Add(key);
			}
		}
		
		// And remove them from the list.
		foreach(float key in removeThese){
			loadedGroups.Remove(key);	
		}
		
	}
	
	public float GetY(float x){
		return scaleOctave1Y * Mathf.PerlinNoise(x * scaleOctave1X, 0f) +  scaleOctave2Y * Mathf.PerlinNoise(x * scaleOctave2X, 0f);
	}
	
	public Transform CreateNewGeometryAt(float startX){
		GameObject holder = new GameObject("Terrain_" + startX);
		holder.transform.parent = this.transform;
		
		for(int i = 0; i < SEGMENTS_PER_GROUP; i++){
			// Calculating the start and end points is pretty easy using Unity's built in perlin noise:
			float x = startX + i*lengthPerSegment;
			Vector3 startPoint = new Vector2(x, GetY(x));
			Vector3 endPoint = new Vector2(x + lengthPerSegment, GetY(x + lengthPerSegment));
			
			// We can draw a line pretty simply to see our terrain:
			Debug.DrawLine(startPoint, endPoint, i % 2 == 1 ? Color.blue : Color.red, 10f);
			
			// But we want to rotate and resize default Unity cubes to match these endpoints, so we need some math...
			// Segment length is used as the scale.
			Vector3 delta = endPoint - startPoint;
			float segmentLength = delta.magnitude;
			
			// find the rotation:
			Quaternion rot = Quaternion.FromToRotation(Vector3.right, delta);
			
			// determine center position of segment:
			Vector3 center = startPoint + delta / 2f;
			
			GameObject segment = Instantiate(terrainChunkPrefab, center, rot ) as GameObject;
			segment.transform.localScale = new Vector3(segmentLength, 1f, 1f);
			Chipmunk.UpdatedTransform(segment);
			
			// That's all we need to do! If you want, you can edit the Chipmunk Shape like this:
			ChipmunkSegmentShape shape = segment.GetComponent<ChipmunkSegmentShape>();
			shape.friction = 1.5f;
			// and anything else you might want to do with the shape...
			
			// Parent the new object.
			segment.transform.parent = holder.transform;
		}
		
		
		return holder.transform;

		
	}
	
	
}

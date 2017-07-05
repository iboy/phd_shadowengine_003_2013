using UnityEngine;
using System.Collections;


[
 RequireComponent(typeof(PolygonCollider2D))
 ]
public class CharacterPhysicsColliderFix : MonoBehaviour
{
	protected Vector2[] originalPoints;

	PolygonCollider2D polygonCollider2D;
	
	void Start ()
	{
		polygonCollider2D=GetComponent<PolygonCollider2D> ();
		
		originalPoints=new Vector2[polygonCollider2D.points.Length];
		
		polygonCollider2D.points.CopyTo (originalPoints,0);
	}
	
	void Update()
	{
		FixColliderRotation ();
	}
	
	public void FixColliderRotation()
	{
		Vector2[] new_t=new Vector2[originalPoints.Length];
		
		for(int i=0;i<originalPoints.Length;i++)
		{
			//new_t[i]=VectorConverter.ToVector2(transform.localRotation*VectorConverter.ToVector3(originalPoints[i]));
			new_t[i]=(Vector2)(transform.localRotation*(Vector3)originalPoints[i]);
			//(Vector3)originalPoints[i]
		}
		
		polygonCollider2D.points = new_t;
	}
}
using UnityEngine;
using System.Collections;
using Vectrosity;

public class VectrosityTest : MonoBehaviour {

	public Material lineMaterial;
	//public Color lineColor = new Color(0.0f, 0.0f, 0.0f, 1f);
	 Camera myCam;
	Vector2[] linePoints1, linePoints2, linePoints3, linePoints4, linePoints5, linePoints6, linePoints7;
	VectorLine myLine1, myLine2, myLine3, myLine4, myLine5, myLine6, myLine7;
	public Transform pointPair1a;
	public Transform pointPair1b;
	public Transform pointPair2a;
	public Transform pointPair2b;
	public Transform pointPair3a;
	public Transform pointPair3b;
	public Transform pointPair4a;
	public Transform pointPair4b;
	public Transform pointPair5a;
	public Transform pointPair5b;
	public Transform pointPair6a;
	public Transform pointPair6b;
	public Transform pointPair7a;
	public Transform pointPair7b;


	// Use this for initialization
	void Start () {

		linePoints1 = new Vector2[2]; // JS & C#
		//linePoints1 = new Vector2[2]; // JS & C#
		linePoints2 = new Vector2[2]; // JS & C#
		linePoints3 = new Vector2[2]; // JS & C#
		linePoints4 = new Vector2[2]; // JS & C#
		linePoints5 = new Vector2[2]; // JS & C#
		linePoints6 = new Vector2[2]; // JS & C#
		linePoints7 = new Vector2[2]; // JS & C#


		linePoints1[0] =  new Vector2(pointPair1a.position.x,  pointPair1a.position.y);
		linePoints1[1] =  new Vector2(pointPair1b.position.x,  pointPair1b.position.y);
		linePoints2[0] =  new Vector2(pointPair2a.position.x,  pointPair2a.position.y);
		linePoints2[1] =  new Vector2(pointPair2b.position.x,  pointPair2b.position.y);
		linePoints3[0] =  new Vector2(pointPair3a.position.x,  pointPair3a.position.y);
		linePoints3[1] =  new Vector2(pointPair3b.position.x,  pointPair3b.position.y);
		linePoints4[0] =  new Vector2(pointPair4a.position.x,  pointPair4a.position.y);
		linePoints4[1] =  new Vector2(pointPair4b.position.x,  pointPair4b.position.y);
		linePoints5[0] =  new Vector2(pointPair5a.position.x,  pointPair5a.position.y);
		linePoints5[1] =  new Vector2(pointPair5b.position.x,  pointPair5b.position.y);
		linePoints6[0] = new  Vector2(pointPair6a.position.x, pointPair6a.position.y);
		linePoints6[1] = new  Vector2(pointPair6b.position.x, pointPair6b.position.y);
		linePoints7[0] = new  Vector2(pointPair7a.position.x, pointPair7a.position.y);
		linePoints7[1] = new  Vector2(pointPair7b.position.x, pointPair7b.position.y);

		myLine1 = new VectorLine("BirdLine1", linePoints1, lineMaterial, .03f); // C#
		myLine2 = new VectorLine("BirdLine2", linePoints2, lineMaterial, .03f); // C#
		myLine3 = new VectorLine("BirdLine3", linePoints3, lineMaterial, .03f); // C#
		myLine4 = new VectorLine("BirdLine4", linePoints4, lineMaterial, .03f); // C#
		myLine5 = new VectorLine("BirdLine5", linePoints5, lineMaterial, .03f); // C#
		myLine6 = new VectorLine("BirdLine6", linePoints6, lineMaterial, .03f); // C#
		myLine7 = new VectorLine("BirdLine7", linePoints7, lineMaterial, .03f); // C#

		myCam = VectorLine.SetCamera();
		myCam.isOrthoGraphic = true;
		myCam.transform.position = new Vector3(0f,2.2f,-2.66f);
		myCam.nearClipPlane = 0.3f;
		myCam.farClipPlane = 1000.0f;
		myCam.orthographicSize = 2.61f;

		// this is needed to move a 
		//myLine.drawTransform = pointPair1a;
		//myLine.drawTransform = pointPair1b;
		//myLine3.drawTransform = pointPair2a;
		//myLine4.drawTransform = pointPair2b;
		//myLine5.drawTransform = pointPair3a;
		//myLine6.drawTransform = pointPair3b;
		//myLine7.drawTransform = pointPair4a;

		myLine1.Draw();
		myLine2.Draw();
		myLine3.Draw();
		myLine4.Draw();
		myLine5.Draw();
		myLine6.Draw();
		myLine7.Draw();

	}
	// Update is called once per frame


	void FixedUpdate () {
		myLine1.points2[0] = new Vector2(pointPair1a.position.x, pointPair1a.position.y);
		myLine1.points2[1] = new Vector2(pointPair1b.position.x, pointPair1b.position.y);
		myLine1.Draw();

		myLine2.points2[0] = new Vector2(pointPair2a.position.x, pointPair2a.position.y);
		myLine2.points2[1] = new Vector2(pointPair2b.position.x, pointPair2b.position.y);
		myLine2.Draw();

		myLine3.points2[0] = new Vector2(pointPair3a.position.x, pointPair3a.position.y);
		myLine3.points2[1] = new Vector2(pointPair3b.position.x, pointPair3b.position.y);
		myLine3.Draw();

		
		myLine4.points2[0] = new Vector2(pointPair4a.position.x, pointPair4a.position.y);
		myLine4.points2[1] = new Vector2(pointPair4b.position.x, pointPair4b.position.y);
		myLine4.Draw();

		myLine5.points2[0] = new Vector2(pointPair5a.position.x, pointPair5a.position.y);
		myLine5.points2[1] = new Vector2(pointPair5b.position.x, pointPair5b.position.y);
		myLine5.Draw();

		myLine6.points2[0] = new Vector2(pointPair6a.position.x, pointPair6a.position.y);
		myLine6.points2[1] = new Vector2(pointPair6b.position.x, pointPair6b.position.y);
		myLine6.Draw();

		myLine7.points2[0] = new Vector2(pointPair7a.position.x, pointPair7a.position.y);
		myLine7.points2[1] = new Vector2(pointPair7b.position.x, pointPair7b.position.y);
		myLine7.Draw();
		//myLine2.Draw();
		//myLine3.Draw();
		//myLine4.Draw();
		//myLine5.Draw();
		//myLine6.Draw();
		//myLine7.Draw();


	}
}

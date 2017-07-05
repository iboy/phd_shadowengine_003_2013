using UnityEngine;
using System.Collections;

public class FrameController : MonoBehaviour {

	public GameObject theFrame;
	public string frameImageName = "Frames.001";
	string lastFrameImageName;
	string leadingZero ="0";
	int count = 1;
	//string myListFrameImageName;
	//string texture;

	// Use this for initialization
	void Start () {
	
		//myLastFrameImageName = frameImageName;

		lastFrameImageName = frameImageName;
		theFrame.renderer.material.mainTexture = Resources.Load<Texture2D>("Frames/"+frameImageName);
		
		

		
		//theFrame.renderer.material.mainTexture = texture;
		//Debug.Log(tex == null); //prints true

		//theFrame.renderer.material.mainTexture=texture;
		
		//theFrame.renderer.material.mainTexture = tex;

	
		//theFrame.renderer.material.mainTexture = inputTexture;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.UpArrow) && count <=36) {
			count++;
			setFrameByIndex(count);



		}

		if (Input.GetKeyDown(KeyCode.DownArrow) && count >1) {
			count--;
			setFrameByIndex(count);
		}
		
		if (lastFrameImageName != frameImageName) {
			theFrame.renderer.material.mainTexture = Resources.Load<Texture2D>("Frames/"+frameImageName);
			lastFrameImageName = frameImageName;
		}
	
	}


	public void setFrameByIndex(int index) {
		if (index >=10) { leadingZero = ""; } else { leadingZero = "0";}
		frameImageName = "Frames.0"+leadingZero+index;
		theFrame.renderer.material.mainTexture = Resources.Load<Texture2D>("Frames/"+frameImageName);
		lastFrameImageName = frameImageName;
		

	}




}

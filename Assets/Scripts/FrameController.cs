using UnityEngine;
using System.Collections;

public class FrameController : MonoBehaviour {

	public GameObject theFrame;
	public string frameImageName = "Frames.001";
	public int totalNumberofFrames = 36; 
	string lastFrameImageName;
	string leadingZero ="0";
	int count = 1;

	void Start () {
	
		lastFrameImageName = frameImageName;
		theFrame.renderer.material.mainTexture = Resources.Load<Texture2D>("Frames/"+frameImageName);

	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.UpArrow) && count <=totalNumberofFrames) {
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

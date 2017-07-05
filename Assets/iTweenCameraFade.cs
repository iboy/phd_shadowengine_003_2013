using UnityEngine;
using System.Collections;

public class iTweenCameraFade : MonoBehaviour{
	//The song in this project is copyright Pixelplacement 1998 Bob Berkebile (also the developer of iTween) do not use it for any commercial work - I included this file to help you mess around and test iTween only.
	
	public Texture2D cameraTexture;
	public float fadeOutDuration = 2.0f;
	public float fadeInDuration = 2.0f;
	bool faded;
	
	void Start(){
		iTween.CameraFadeAdd(cameraTexture,200);
	}
	
//	void OnGUI(){
//		if(!faded){
//			if(GUI.Button(new Rect(75,151,100,50),"Fade Out")){
//				iTween.CameraFadeTo(1,2);
//				//iTween.AudioTo(gameObject,0,0,2);
//				faded=true;
//			}
//		}else{
//			if(GUI.Button(new Rect(75,151,100,50),"Fade In")){
//				iTween.CameraFadeTo(0,2);
//				//iTween.AudioTo(gameObject,1,1,2);
//				faded=false;
//			}	
//		}
//	}


	void Update() {



		if (!faded) {

				if (Input.GetKeyDown("return")) {
					Debug.Log("Return is pressed and fading out");
					iTween.CameraFadeTo(1,fadeOutDuration);
					//iTween.AudioTo(gameObject,0,0,2);
					faded=true;
			}
		} else {

				if (Input.GetKeyDown("return")) {
					Debug.Log("Return is pressed and fading in");
					iTween.CameraFadeTo(0,fadeInDuration);
					//iTween.AudioTo(gameObject,1,1,2);
					faded=false;
				}
		}
	}

}

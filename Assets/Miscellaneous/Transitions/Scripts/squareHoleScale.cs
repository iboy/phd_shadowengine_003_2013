using UnityEngine;
using System.Collections;

public class squareHoleScale : MonoBehaviour {
	public GameObject myObject;
	public float hSliderValueScaleLevel = 1.0F;
	
	
    void OnGUI() {
        hSliderValueScaleLevel = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValueScaleLevel, -20.0F, 20.0F); // scale level

	 	myObject.transform.localScale = new Vector3(hSliderValueScaleLevel, 1.0F, hSliderValueScaleLevel);
	
		//Screen.width
		//Screen.height
    }
}



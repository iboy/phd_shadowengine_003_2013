using UnityEngine;
using System.Collections;

public class gui_fade : MonoBehaviour {
	public GameObject myObject;
	public float hSliderValueFadeLevel = 1.0F;
	
	
    void OnGUI() {
        hSliderValueFadeLevel = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValueFadeLevel, 0.0F, 1.0F); // fade level
		
		Color color = myObject.renderer.material.color;
		color.a = hSliderValueFadeLevel;
		myObject.renderer.material.color = color;
	
		
    }
}


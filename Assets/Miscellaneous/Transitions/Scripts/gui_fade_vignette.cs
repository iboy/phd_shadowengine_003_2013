using UnityEngine;
using System.Collections;

public class gui_fade_vignette : MonoBehaviour {
	public GameObject myObject;
	// global fade
	public float hSliderValueFadeLevel = 1.0F;
	// aperture
	public float hSliderValueRadius = 0.1F;
	public float hSliderValueHoleBlur = 240.0F;
	
	
    void OnGUI() {
        hSliderValueFadeLevel = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValueFadeLevel, 0.0F, 1.0F); // fade level
		
		// fade
		Color color = myObject.renderer.material.color;
		color.a = hSliderValueFadeLevel;
		myObject.renderer.material.color = color;
	
		// iris
		hSliderValueRadius = GUI.HorizontalSlider(new Rect(25, 50, 100, 30), hSliderValueRadius, 0.0F, 1.0F); // size of the hole
		hSliderValueHoleBlur = GUI.HorizontalSlider(new Rect(25, 75, 100, 30), hSliderValueHoleBlur, 0.5F, 240.0F); // edge feather / blue of the hole
		
		myObject.renderer.sharedMaterial.SetFloat( "_Radius", hSliderValueRadius );
		myObject.renderer.sharedMaterial.SetFloat( "_Shape", hSliderValueHoleBlur );
		
		
    }
}


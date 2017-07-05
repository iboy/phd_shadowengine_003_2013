using UnityEngine;
using System.Collections;

public class gui_iris : MonoBehaviour {
	public GameObject myObject;
	public float hSliderValueRadius = 0.1F;
	public float hSliderValueHoleBlur = 240.0F;
	
    void OnGUI() {
        hSliderValueRadius = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), hSliderValueRadius, 0.0F, .2F); // size of the hole
		hSliderValueHoleBlur = GUI.HorizontalSlider(new Rect(25, 55, 100, 30), hSliderValueHoleBlur, 0.5F, 240.0F); // edge feather / blue of the hole
		
		myObject.renderer.sharedMaterial.SetFloat( "_Radius", hSliderValueRadius );
		myObject.renderer.sharedMaterial.SetFloat( "_Shape", hSliderValueHoleBlur );
    }
}
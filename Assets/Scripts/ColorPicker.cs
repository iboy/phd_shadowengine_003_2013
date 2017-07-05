using UnityEngine;

using System;

using System.Collections;

 

public class ColorPicker : MonoBehaviour {

    public Texture2D colorPickerTex;

    public int ImageWidth = 400;

    public int ImageHeight = 400;

 
// convert this from onGUI to looking up under a XY Pad.
	
	public Color PickColorFromCoords (Texture2D myTexture, float xPos, float yPos) {
		
		//Vector2 pickpos = new Vector2 (100.0f, 100.0f);
		//inverted X and Y here - weird XY Pad error from TouchOSC
		Vector2 pickpos = new Vector2 (yPos, xPos);

		int aaa = Convert.ToInt32(pickpos.x);

		int bbb = Convert.ToInt32(pickpos.y);
		//colorPickerTex = Resources.Load("colorpicker_texture", typeof(Texture2D));
		//Color col = myTexture.GetPixel(aaa,41-bbb);
		Color[] colarray = myTexture.GetPixels(aaa,bbb,1,1);
		Color col = colarray[0];
		Debug.Log(col);
		
		return col;
		
	}
	
    


}

using UnityEngine;
using System.Collections;

public class ObjectHelperKiosk_001 : MonoBehaviour {
	public GameObject Kiosk_Object_001;
	public bool switchRenderMode =false;
	bool flag = false;
	// Use this for initialization



	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (switchRenderMode)
		{
			monochromeColorMode();
		} else { normalColorMode(); }
	}

	public void monochromeColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
			if (flag == true) { return; }
		Color myColor = new Color(0f,0f,0f,1f);
		SpriteRenderer objectRenderer = Kiosk_Object_001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		objectRenderer.color = myColor; // Set to opaque black
			flag = true;
	}

	public void normalColorMode() {
		//SpriteRenderer myrenderer = Dragon010Head.GetComponent(SpriteRenderer);
			if (flag == false) { return; }
		Color myColor = new Color(1f,1f,1f,1f);
		SpriteRenderer objectRenderer = Kiosk_Object_001.GetComponent<SpriteRenderer>();//Get the renderer via GetComponent or have it cached previously
		objectRenderer.color = myColor; // Set to opaque black
		flag = false;
	}
}

using UnityEngine;
using System.Collections;

public class SwitchMaterialsToSilhouette : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void ToggleMaterialsSilhouette () {
		

        camera.CopyFrom(Camera.main);
  
	}
}


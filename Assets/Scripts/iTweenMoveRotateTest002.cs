using UnityEngine;
using System.Collections;

public class iTweenMoveRotateTest002 : MonoBehaviour {

	// Use this for initialization
	//Hashtable ht = new Hashtable();
	bool onStage = false;
	public GameObject root;
	public GameObject collider1;
	public GameObject collider2;
	public GameObject collider3;
	public GameObject collider4;
	public GameObject collider5;
	public GameObject collider6;
	public GameObject collider7;
	public GameObject collider8;
	public GameObject collider9;
	public GameObject collider10;
	public bool is2D;
	public string triggerKey = "z";

	void Awake () {

		//ht.Add("y",4);

	}

	void FixedUpdate () {

		if (onStage == false) {

			if (Input.GetKeyUp(triggerKey) && (onStage == false)) {

				disableColliders();
				//setIsKinematicTrue();

				// the move
				//iTween.MoveTo(gameObject, iTween.Hash("x", .4f,"y", 2.5f, "z", 0, "easeType", "easeInOutExpo", "loopType", "none", "delay", .5, "time", 1.8f));

				// the rotate
				iTween.RotateTo(gameObject, iTween.Hash("y", 180.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time", .6f, "onComplete", "completedEntryAndRotate"));
				iTween.ShakeScale(root, iTween.Hash("amount", new Vector3(.02f,.02f,.02f), "delay", 0.2f, "time", 2.0f));
				//iTween.ScaleTo(root, iTween.Hash("x", 2f,"y", 2f, "z", 1f, "time", 1.0f, "onComplete", "setIsKinematicFalse", "onCompleteTarget", collider1));
				//setIsKinematicFalse();

				onStage = true;
			}
		} else {
			// this move the puppet off-stage
			if (Input.GetKeyUp(triggerKey) && (onStage == true)) {
				//onStage = false;
				disableColliders();
				//setIsKinematicTrue();
				iTween.ShakeScale(root, iTween.Hash("amount", new Vector3(.01f,.02f,.01f), "delay", 0.0f, "time", 1.0f));

				//the rotate
				iTween.RotateTo(gameObject, iTween.Hash("y", 360.0f, "easeType", "easeInOutExpo", "loopType", "none", "delay", 0.0f, "time",.6f));

				//iTween.ScaleTo(root, iTween.Hash("x", 1f,"y", 1f, "z", 1f, "time", 1.0f, "onComplete", "setIsKinematicTrue", "onCompleteTarget", collider1));
				//the move
				//iTween.MoveTo(gameObject, iTween.Hash("x", -7f, "y", 4.22074f, "z", 0, "easeType", "easeInOutExpo", "loopType", "none", "delay", 2.0f, "time", 1.4f, "onComplete", "completedExitAndRotate"));
				//setIsKinematicFalse();

				onStage = false;
			}

		}
	}

	void setIsKinematicTrue() {


		Debug.Log("Setting isKinematic to True");

		if (is2D) {

			Debug.Log("I'm actually a 2D object");
			//if (collider1) { collider1.rigidbody.isKinematic = true; } // what out for the first object...
			if (collider2) { collider2.rigidbody2D.isKinematic = true; }
			if (collider3) { collider3.rigidbody2D.isKinematic = true; }
			if (collider4) { collider4.rigidbody2D.isKinematic = true; }
			if (collider5) { collider5.rigidbody2D.isKinematic = true; }
			if (collider6) { collider6.rigidbody2D.isKinematic = true; }
			if (collider7) { collider7.rigidbody2D.isKinematic = true; }
			if (collider8) { collider8.rigidbody2D.isKinematic = true; }
			if (collider9) { collider9.rigidbody2D.isKinematic = true; }
			if (collider10) { collider10.rigidbody2D.isKinematic = true; }

		} else {
			//if (collider1) { collider1.rigidbody.isKinematic = true; }  // what out for the first object...
			if (collider2) { collider2.rigidbody.isKinematic = true; }
			if (collider3) { collider3.rigidbody.isKinematic = true; }
			if (collider4) { collider4.rigidbody.isKinematic = true; }
			if (collider5) { collider5.rigidbody.isKinematic = true; }
			if (collider6) { collider6.rigidbody.isKinematic = true; }
			if (collider7) { collider7.rigidbody.isKinematic = true; }
			if (collider8) { collider8.rigidbody.isKinematic = true; }
			if (collider9) { collider9.rigidbody.isKinematic = true; }
			if (collider10) { collider10.rigidbody.isKinematic = true; }
			}
	}

	void setIsKinematicFalse() {
		//Debug.Log("In function: setIsKinematicFalse()");
		//collider2.rigidbody.WakeUp();

		if (is2D) {
			
			Debug.Log("setIsKinematicFalse() - I'm actually a 2D object with 2D rigidbodies");

			//if (collider1) { collider1.rigidbody2D.isKinematic = false; } // watch out where the root object is
			if (collider2) { collider2.rigidbody2D.isKinematic = false; }
			//Debug.Log("Setting isKinematic on collider 2 to false");
			if (collider3) { collider3.rigidbody2D.isKinematic = false; }
			if (collider4) { collider4.rigidbody2D.isKinematic = false; }
			if (collider5) { collider5.rigidbody2D.isKinematic = false; }
			if (collider6) { collider6.rigidbody2D.isKinematic = false; }
			if (collider7) { collider7.rigidbody2D.isKinematic = false; }
			if (collider8) { collider8.rigidbody2D.isKinematic = false; }
			if (collider9) { collider9.rigidbody2D.isKinematic = false; }
			if (collider10) { collider10.rigidbody2D.isKinematic = false; }
			
		} else {

			//if (collider1) { collider1.rigidbody.isKinematic = false; } // watch out where the root object is
			if (collider2) { collider2.rigidbody.isKinematic = false; }
			//Debug.Log("Setting isKinematic on collider 2 to false");
			if (collider3) { collider3.rigidbody.isKinematic = false; }
			if (collider4) { collider4.rigidbody.isKinematic = false; }
			if (collider5) { collider5.rigidbody.isKinematic = false; }
			if (collider6) { collider6.rigidbody.isKinematic = false; }
			if (collider7) { collider7.rigidbody.isKinematic = false; }
			if (collider8) { collider8.rigidbody.isKinematic = false; }
			if (collider9) { collider9.rigidbody.isKinematic = false; }
			if (collider10) { collider10.rigidbody.isKinematic = false; }
		}
			}
	void disableColliders() {

		if (is2D) {
			
			Debug.Log("disableColliders() - I'm actually a 2D object with 2D rigidbodies");
			if (collider1) { collider1.collider2D.enabled = false; Debug.Log("In disableColliders(). Setting collider1 to off"); }
			if (collider2) { collider2.collider2D.enabled = false; }
			if (collider3) { collider3.collider2D.enabled = false; }
			if (collider4) { collider4.collider2D.enabled = false; }
			if (collider5) { collider5.collider2D.enabled = false; }
			if (collider6) { collider6.collider2D.enabled = false; }
			if (collider7) { collider7.collider2D.enabled = false; }
			if (collider8) { collider8.collider2D.enabled = false; }
			if (collider9) { collider9.collider2D.enabled = false; }
			if (collider10){ collider10.collider2D.enabled = false; }
			
		} else {

			Debug.Log("disableColliders() - I'm a 3D object with rigidbodies");
		if (collider1) { collider1.collider.enabled = false; Debug.Log("In disableColliders(). Setting collider1 to off"); }
		if (collider2) { collider2.collider.enabled = false; }
		if (collider3) { collider3.collider.enabled = false; }
		if (collider4) { collider4.collider.enabled = false; }
		if (collider5) { collider5.collider.enabled = false; }
		if (collider6) { collider6.collider.enabled = false; }
		if (collider7) { collider7.collider.enabled = false; }
		if (collider8) { collider8.collider.enabled = false; }
		if (collider9) { collider9.collider.enabled = false; }
		if (collider10){ collider10.collider.enabled = false; }

		}
	}
	void completedEntryAndRotate() {
		if (is2D) {
			if (collider1) { collider1.collider2D.enabled = true;  //Debug.Log("Setting collider1 to on"); 
			}
			if (collider2) { collider2.collider2D.enabled = true; }
			if (collider3) { collider3.collider2D.enabled = true; }
			if (collider4) { collider4.collider2D.enabled = true; }
			if (collider5) { collider5.collider2D.enabled = true; }
			if (collider6) { collider6.collider2D.enabled = true; }
			if (collider7) { collider7.collider2D.enabled = true; }
			if (collider8) { collider8.collider2D.enabled = true; }
			if (collider9) { collider9.collider2D.enabled = true; }
			if (collider10){ collider10.collider2D.enabled = true; }



		} else {
		//Debug.Log("Completed Entry and Rotate");
		if (collider1) { collider1.collider.enabled = true; 
			//Debug.Log("Setting collider1 to on"); 
		}
		if (collider2) { collider2.collider.enabled = true; }
		if (collider3) { collider3.collider.enabled = true; }
		if (collider4) { collider4.collider.enabled = true; }
		if (collider5) { collider5.collider.enabled = true; }
		if (collider6) { collider6.collider.enabled = true; }
		if (collider7) { collider7.collider.enabled = true; }
		if (collider8) { collider8.collider.enabled = true; }
		if (collider9) { collider9.collider.enabled = true; }
		if (collider10){ collider10.collider.enabled = true; }
		//setIsKinematicFalse();
		//onStage = true;
	}
	}

	void completedExitAndRotate() {
		
		Debug.Log("Completed Exit and Rotate");

		if (is2D) {
			// if it is a 2d sprite / 2d rigidbody
			if (collider1) { collider1.collider2D.enabled = true; }
			if (collider2) { collider2.collider2D.enabled = true; }
			if (collider3) { collider3.collider2D.enabled = true; }
			if (collider4) { collider4.collider2D.enabled = true; }
			if (collider5) { collider5.collider2D.enabled = true; }
			if (collider6) { collider6.collider2D.enabled = true; }
			if (collider7) { collider7.collider2D.enabled = true; }
			if (collider8) { collider8.collider2D.enabled = true; }
			if (collider9) { collider9.collider2D.enabled = true; }
			if (collider10){ collider10.collider2D.enabled = true; }
			onStage = false;


		} else {

		if (collider1) { collider1.collider.enabled = true; }
		if (collider2) { collider2.collider.enabled = true; }
		if (collider3) { collider3.collider.enabled = true; }
		if (collider4) { collider4.collider.enabled = true; }
		if (collider5) { collider5.collider.enabled = true; }
		if (collider6) { collider6.collider.enabled = true; }
		if (collider7) { collider7.collider.enabled = true; }
		if (collider8) { collider8.collider.enabled = true; }
		if (collider9) { collider9.collider.enabled = true; }
		if (collider10){ collider10.collider.enabled = true; }
		onStage = false;

		}
	}

}

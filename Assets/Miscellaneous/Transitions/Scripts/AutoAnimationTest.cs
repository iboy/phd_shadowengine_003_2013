using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animation))]

public class AutoAnimationTest : MonoBehaviour {
	
	public GameObject myObject;
	
	// Use this for initialization
    void Start() {
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 4, 5);
		AnimationCurve curve2 = AnimationCurve.Linear(0, 2, 4, 2);
        AnimationClip clip = new AnimationClip();
        clip.SetCurve("", typeof(Transform), "localPosition.z", curve);
		clip.SetCurve("", typeof(Transform), "localPosition.x", curve2);
        animation.AddClip(clip, "test");
        animation.Play("test");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}



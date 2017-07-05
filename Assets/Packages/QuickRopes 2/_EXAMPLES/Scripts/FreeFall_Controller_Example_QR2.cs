using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FreeFall_Controller_Example_QR2 : MonoBehaviour 
{
    QuickRope2 rope;

    void Start()
    {
        rope = QuickRope2.Create(Vector3.zero, new Vector3(0, -10, 0), BasicRopeTypes.Mesh);
        rope.enablePhysics = true;
        rope.enableRopeController = true;
        rope.ApplyRopeSettings();

        rope.rigidbody.isKinematic = true;
    }

    void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rope.FreeFallMode = (rope.FreeFallMode == false);
        }
	}

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "Press SPACE to toggle freefall mode!");
        GUI.Label(new Rect(10, 40, 300, 30), "Press ARROW UP to shorten rope!");
        GUI.Label(new Rect(10, 70, 300, 30), "Press ARROW DOWN to lengthen rope!");

        GUI.Label(new Rect(10, 110, 150, 30), "FreeFall: " + rope.FreeFallMode);
        GUI.Label(new Rect(10, 140, 150, 30), "Max Length: " + rope.maxRopeLength.ToString("00"));
        rope.maxRopeLength = GUI.HorizontalSlider(new Rect(150, 150, 150, 30), rope.maxRopeLength, 5, 50);
    }
}

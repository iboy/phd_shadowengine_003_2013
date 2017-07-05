using UnityEngine;
using System.Collections;

public class SinMovement_QR2 : MonoBehaviour 
{
	void Update () 
    {
        transform.position = new Vector3(Mathf.Sin(Time.time * 0.5f) * 5f, 0, 0);
    }
}

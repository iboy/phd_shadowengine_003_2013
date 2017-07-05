using UnityEngine;
using System.Collections;

public class CraneControl_QR2 : MonoBehaviour 
{
    public Transform craneRotationObject = null;
    public Transform trollyObject = null;
    public Transform trollyInStop = null;
    public Transform trollyOutStop = null;

    private float rotationVelocity = 0;
    public float rotationAccel = 3;
    public float maxRotationVelocity = 7;
    public float rotationDampening = 0.985f;

    private float trollyVelocity = 0;
    public float trollyAccel = 3;
    public float maxTrollyVelocity = 7;
    public float trollyDampening = 0.985f;
    private float trollyPosition = 0.5f;

	void Update () 
    {
        if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            rotationVelocity += rotationAccel * Time.deltaTime;
        } 
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rotationVelocity -= rotationAccel * Time.deltaTime;
        }
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rotationVelocity *= rotationDampening;
        }
        rotationVelocity = Mathf.Clamp(rotationVelocity, -maxRotationVelocity, maxRotationVelocity);
        craneRotationObject.Rotate(0, rotationVelocity * Time.deltaTime, 0);

        // Trolly
        if (!Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
        {
            trollyVelocity += trollyAccel * 0.1f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            trollyVelocity -= trollyAccel * 0.1f * Time.deltaTime;
        }
        else if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            trollyVelocity *= trollyDampening;
        }
        trollyVelocity = Mathf.Clamp(trollyVelocity, -maxTrollyVelocity * 0.1f, maxTrollyVelocity * 0.1f);
        trollyPosition += trollyVelocity * Time.deltaTime;
        trollyPosition = Mathf.Clamp(trollyPosition, 0f, 1f);
        if (trollyPosition == 0 || trollyPosition == 1)
            trollyVelocity = 0;
        
        trollyObject.transform.position = trollyInStop.position + ((trollyOutStop.position - trollyInStop.position) * trollyPosition);
	}
}

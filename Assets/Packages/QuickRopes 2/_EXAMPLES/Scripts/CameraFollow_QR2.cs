using UnityEngine;
using System.Collections;

public class CameraFollow_QR2 : MonoBehaviour 
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, 0);
    public float lerpValue = 5.0f;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z + offset.z), Time.deltaTime * lerpValue);
    }
}

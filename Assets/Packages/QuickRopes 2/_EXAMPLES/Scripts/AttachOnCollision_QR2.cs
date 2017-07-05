using UnityEngine;
using System.Collections;

public class AttachOnCollision_QR2 : MonoBehaviour 
{
    public QuickRope2 rope;
    public string connectableTag = "Player";
    public KeyCode detachKey = KeyCode.Space;
    public GUIText HUD = null;
    private GameObject attachedObject = null;
    private bool isConnected = false;

    void Update()
    {
        if (isConnected && Input.GetKeyDown(detachKey))
        {
            rope.DetachObject(attachedObject);
            isConnected = false;

            if (HUD)
                HUD.text = "Hook Disconnected";
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (isConnected || (col.gameObject.tag != connectableTag))
            return;

        isConnected = true;
        attachedObject = col.gameObject;
        rope.AttachObject(attachedObject, rope.Joints.Count - 1, false);

        if (HUD)
            HUD.text = "Hook Connected";
    }
}

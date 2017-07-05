//----------------------------------------------
//                Joint2DGizmos
//         Copyright © 2013  Illogika
//----------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Joint2DGizmo : MonoBehaviour {

#if UNITY_EDITOR
	protected Color COLOR_ANCHOR = Color.red;
	protected Color COLOR_SPRING = Color.magenta;
	protected Color COLOR_FREE_MOVEMENT = Color.cyan;
	protected Color COLOR_CONNECTED_ANCHOR_LINK = Color.yellow;

	protected Color COLOR_UNSELECTED = Color.white;
	protected Color COLOR_SELECTED = Color.green;

	protected void DrawFreeMovementZone(Vector3 anchor, float distance)
	{
		Handles.color = COLOR_FREE_MOVEMENT;
		Handles.DrawWireDisc(new Vector3(transform.position.x + anchor.x * transform.lossyScale.x,
		                                 transform.position.y + anchor.y * transform.lossyScale.y,
		                                 transform.position.z - 1),
		                     Vector3.forward,
		                     distance);
	}

	protected void DrawAnchorLeadingLine(Vector3 anchor)
	{
		Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + anchor.x * transform.lossyScale.x, transform.position.y + anchor.y * transform.lossyScale.y, transform.position.z - 1));
	}

	protected void DrawLinkBetweenAnchors(Rigidbody2D connectedBody, Vector3 anchor, Vector3 connectedAnchor)
	{
		Gizmos.DrawLine(new Vector3(transform.position.x + anchor.x * transform.lossyScale.x, transform.position.y + anchor.y * transform.lossyScale.y, transform.position.z - 1),
		                new Vector3(connectedBody.transform.position.x + connectedAnchor.x * connectedBody.transform.lossyScale.x, connectedBody.transform.position.y + connectedAnchor.y * connectedBody.transform.lossyScale.y, connectedBody.transform.position.z - 1));
	}

	protected void DrawLinkBetweenBodyAndAnchor(Rigidbody2D connectedBody, Vector3 anchor)
	{
		Gizmos.DrawLine(new Vector3(connectedBody.transform.position.x, connectedBody.transform.position.y, connectedBody.transform.position.z - 1),
		                new Vector3(connectedBody.transform.position.x + anchor.x * connectedBody.transform.lossyScale.x, connectedBody.transform.position.y + anchor.y * connectedBody.transform.lossyScale.y, connectedBody.transform.position.z - 1));
	}

	protected void DrawAnchor(Vector3 anchor)
	{
		Gizmos.color = COLOR_ANCHOR;
		DrawTransformAnchor(transform, anchor);
	}

	protected void DrawTransformAnchor(Transform reference, Vector3 anchor)
	{
		float px = reference.position.x + anchor.x * reference.lossyScale.x;
		float py = reference.position.y + anchor.y * reference.lossyScale.y;
		float pz = reference.position.z - 1;
		
		Gizmos.DrawLine(new Vector3(px - 0.1f, py, pz),
		                new Vector3(px + 0.1f, py, pz));
		Gizmos.DrawLine(new Vector3(px, py - 0.1f, pz),
		                new Vector3(px, py + 0.1f, pz));
	}

	protected void DrawConnectedAnchor(Rigidbody2D connectedBody, Vector3 connectedAnchor)
	{
		Gizmos.color = COLOR_ANCHOR;

		if(connectedBody != null)
		{
			DrawTransformAnchor(connectedBody.transform, connectedAnchor);
		}
	}
#endif
}

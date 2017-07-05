//----------------------------------------------
//                Joint2DGizmos
//         Copyright © 2013  Illogika
//----------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(DistanceJoint2D))]
public class DistanceJoint2DGizmo : Joint2DGizmo {

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		DoDrawGizmos(COLOR_UNSELECTED);
	}
	
	void OnDrawGizmosSelected()
	{
		DoDrawGizmos(COLOR_SELECTED, true);
	}
	
	void DoDrawGizmos(Color color, bool drawConnectorLines = false)
	{
		DistanceJoint2D[] joints = gameObject.GetComponents<DistanceJoint2D>();
		
		foreach(DistanceJoint2D joint in joints)
		{
			Gizmos.color = color;

			DrawAnchorLeadingLine(transform.rotation * joint.anchor);

			if(joint.connectedBody != null)
			{
				DrawFreeMovementZone(transform.rotation * joint.anchor, joint.distance);

				Gizmos.color = COLOR_ANCHOR;
				DrawLinkBetweenAnchors(joint.connectedBody, transform.rotation * joint.anchor, joint.connectedAnchor);

				Gizmos.color = COLOR_CONNECTED_ANCHOR_LINK;
				DrawLinkBetweenBodyAndAnchor(joint.connectedBody, joint.connectedAnchor);

				if(drawConnectorLines)
				{
					Gizmos.color = COLOR_CONNECTED_ANCHOR_LINK;
					if (Application.isPlaying)
					{
						Gizmos.DrawLine(joint.connectedBody.transform.position, joint.connectedBody.transform.position + transform.rotation * joint.connectedAnchor);
					}
					else
					{
						DrawLinkBetweenBodyAndAnchor(joint.connectedBody, joint.connectedAnchor);
					}
				}
			}

			DrawAnchor(transform.rotation * joint.anchor);
			DrawConnectedAnchor(joint.connectedBody, joint.connectedAnchor);
		}
	}
#endif
}

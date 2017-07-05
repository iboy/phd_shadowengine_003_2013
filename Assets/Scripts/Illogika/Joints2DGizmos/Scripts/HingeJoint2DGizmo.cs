//----------------------------------------------
//                Joint2DGizmos
//         Copyright © 2013  Illogika
//----------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[RequireComponent(typeof(HingeJoint2D))]
public class HingeJoint2DGizmo : Joint2DGizmo {

#if UNITY_EDITOR
	private Dictionary<HingeJoint2D,Vector3> cachedAngleStarts = new Dictionary<HingeJoint2D,Vector3>();
	private Dictionary<HingeJoint2D,Quaternion> cachedRotationStarts = new Dictionary<HingeJoint2D,Quaternion>();
	private Dictionary<HingeJoint2D,Quaternion> cachedOriginalRotations = new Dictionary<HingeJoint2D,Quaternion>();

	void Awake()
	{
		HingeJoint2D[] joints = gameObject.GetComponents<HingeJoint2D>();

		foreach(HingeJoint2D joint in joints)
		{
			if(joint.connectedBody != null)
			{
				cachedRotationStarts.Add(joint, joint.connectedBody.transform.rotation);
			}
			cachedAngleStarts.Add(joint, new Vector3(joint.connectedAnchor.normalized.x, joint.connectedAnchor.normalized.y));
			cachedOriginalRotations.Add(joint, transform.rotation);
		}
	}

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
		HingeJoint2D[] joints = gameObject.GetComponents<HingeJoint2D>();
		
		foreach(HingeJoint2D joint in joints)
		{
			Handles.color = color;

			Vector3 anchor = new Vector3(joint.anchor.x, joint.anchor.y, 0);
			anchor = joint.transform.rotation * anchor;

			// Draw the rotation Arc
			if(joint.connectedBody != null)
			{
				float radius = new Vector3(joint.connectedAnchor.x * joint.connectedBody.transform.lossyScale.x, joint.connectedAnchor.y * joint.connectedBody.transform.lossyScale.y, 0).magnitude;

				Handles.DrawWireArc(new Vector3(transform.position.x + anchor.x * transform.lossyScale.x,
				                                transform.position.y + anchor.y * transform.lossyScale.y,
				                                transform.position.z - 1),
				                    Vector3.forward,
				                    (Application.isPlaying ? cachedRotationStarts[joint] * Quaternion.Euler(transform.rotation.eulerAngles - cachedOriginalRotations[joint].eulerAngles) : joint.connectedBody.transform.rotation) * Quaternion.Euler(0,0,joint.limits.min + transform.rotation.z) * (Application.isPlaying ? cachedAngleStarts[joint] : new Vector3(joint.connectedAnchor.normalized.x, joint.connectedAnchor.normalized.y)),
				                    joint.useLimits ? joint.limits.min - joint.limits.max : 360,
				                    radius);


				// Add the motor's direction if used.
				if(joint.useMotor && !Mathf.Approximately(joint.motor.motorSpeed, 0)) 
					Handles.ConeCap(0, (transform.position + new Vector3(anchor.x * transform.lossyScale.x, anchor.y * transform.lossyScale.y + radius, 0)), Quaternion.Euler(0,joint. motor.motorSpeed > 0 ? -90 : 90,0), 0.5f);

				if(drawConnectorLines)
				{
					Gizmos.color = COLOR_CONNECTED_ANCHOR_LINK;
					
					if (Application.isPlaying)
					{
						Gizmos.DrawLine(joint.connectedBody.transform.position, joint.transform.position + anchor);
					}
					else 
					{
						DrawLinkBetweenBodyAndAnchor(joint.connectedBody, joint.connectedBody.transform.rotation * joint.connectedAnchor);
					}
				}
			}

			// Draw the link between the two anchors


			DrawAnchor(anchor);
			if (!Application.isPlaying)
				DrawConnectedAnchor(joint.connectedBody, joint.connectedBody.transform.rotation * joint.connectedAnchor);
		}
	}

#endif
}

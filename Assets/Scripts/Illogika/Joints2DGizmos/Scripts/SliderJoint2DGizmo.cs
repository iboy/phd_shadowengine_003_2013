//----------------------------------------------
//                Joint2DGizmos
//         Copyright © 2013  Illogika
//----------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[RequireComponent(typeof(SliderJoint2D))]
public class SliderJoint2DGizmo : Joint2DGizmo {

#if UNITY_EDITOR
	private Dictionary<SliderJoint2D,Quaternion> cachedParentRotationStarts = new Dictionary<SliderJoint2D,Quaternion>();
	private Dictionary<SliderJoint2D,Quaternion> cachedSliderRotationStarts = new Dictionary<SliderJoint2D,Quaternion>();

	void Awake()
	{
		SliderJoint2D[] joints = gameObject.GetComponents<SliderJoint2D>();
		
		foreach(SliderJoint2D joint in joints)
		{
			cachedParentRotationStarts.Add(joint, transform.parent.rotation);
			cachedSliderRotationStarts.Add(joint, transform.localRotation);
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
		SliderJoint2D[] joints = gameObject.GetComponents<SliderJoint2D>();
		
		foreach(SliderJoint2D joint in joints)
		{
			Gizmos.color = color;

			DrawAnchorLeadingLine(joint.anchor);
			/*
			if(Application.isPlaying && cachedSliderRotationStarts[joint] != transform.localRotation)
			{
				cachedSliderRotationStarts[joint] = transform.localRotation;
				cachedParentRotationStarts[joint] = transform.parent.rotation;
			}
*/
			Quaternion rotation = Application.isPlaying ? Quaternion.Euler( transform.parent.rotation.eulerAngles - cachedParentRotationStarts[joint].eulerAngles ) : Quaternion.identity;

			// Draw the slider
			Vector3 minEnd = transform.position + new Vector3(joint.anchor.x * transform.lossyScale.x, joint.anchor.y * transform.lossyScale.y, 0) + (rotation * Quaternion.Euler(0,0,joint.angle + 180) * Vector3.up) * (joint.useLimits ? Mathf.Min (joint.limits.min, joint.limits.max) : 100000);
			Vector3 maxEnd = transform.position + new Vector3(joint.anchor.x * transform.lossyScale.x, joint.anchor.y * transform.lossyScale.y, 0) + (rotation * Quaternion.Euler(0,0,joint.angle + 180) * Vector3.up) * (joint.useLimits ? Mathf.Max (joint.limits.min, joint.limits.max) : -100000);
			Gizmos.DrawLine(minEnd, maxEnd);
	
			if(drawConnectorLines && joint.connectedBody != null)
			{
				Gizmos.color = COLOR_CONNECTED_ANCHOR_LINK;
				if (Application.isPlaying)
				{
					Gizmos.DrawLine(joint.connectedBody.transform.position, joint.connectedBody.transform.position + joint.connectedBody.transform.rotation * joint.connectedAnchor);
				}
				else
				{
					DrawLinkBetweenBodyAndAnchor(joint.connectedBody, joint.connectedBody.transform.rotation * joint.connectedAnchor);
				}
			}

			Handles.color = color;

			// Add the motor's direction if used.
			if(joint.useMotor && !Mathf.Approximately(joint.motor.motorSpeed, 0) && (!joint.useLimits || Mathf.Abs(joint.limits.max - joint.limits.min) != 0) )
				Handles.ConeCap(0, minEnd - (joint.useLimits ? Vector3.zero : (maxEnd - minEnd).normalized) + (maxEnd - minEnd) * 0.5f, Quaternion.LookRotation((maxEnd - minEnd) * (joint.motor.motorSpeed > 0 ?  -1: 1)) , 0.5f);

			Gizmos.color = COLOR_ANCHOR;
			DrawAnchor(joint.anchor);
			DrawConnectedAnchor(joint.connectedBody, joint.connectedBody.transform.rotation * joint.connectedAnchor);
		}
	}
#endif
}

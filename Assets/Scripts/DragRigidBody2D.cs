// Conversion of standard DragRigidbody.js to DragRigidbody2D.cs
// Ian Grant v001
using UnityEngine;
using System.Collections;

public class DragRigidBody2D : MonoBehaviour
{

// Class Variables
		public float distance = 0.2f;
		public float damper = 0.5f; // damping ration in SpringJoint2D (0.0.- 1.0)
		public float frequency = 8.0f;
		public float drag = 1.0f; // this doesn't exist on 2D Spring...
		public float angularDrag = 5.0f;
		//var distance = 0.2;
		public bool attachToCenterOfMass = false;
		private SpringJoint2D springJoint;
	

// Update
		void Update ()
		{
	

				if (!Input.GetMouseButtonDown (0))
						return;
		
				Camera mainCamera = FindCamera ();
				int layerMask = 1 << 8;
				RaycastHit2D hit = Physics2D.Raycast (mainCamera.ScreenToWorldPoint (Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
				Debug.Log ("Layermask: " + LayerMask.LayerToName (8));
				// I have proxy collider objects (empty gameobjects with a 2D Collider) as a child of a 3D rigidbody - simulating collisions between 2D and 3D objects
				// I therefore set any 'touchable' object to layer 8 and use the layerMask above for all touchable items

				if (hit.collider != null && hit.rigidbody.isKinematic == true) {
						return;
				} 

				if (hit.collider != null && hit.rigidbody.isKinematic == false) {
			
			
						if (!springJoint) {
								GameObject go = new GameObject ("Rigidbody2D Dragger");
								Rigidbody2D body = go.AddComponent ("Rigidbody2D") as Rigidbody2D;
								springJoint = go.AddComponent ("SpringJoint2D") as SpringJoint2D;
				
								body.isKinematic = true;
						}

						springJoint.transform.position = hit.point;


						if (attachToCenterOfMass) {
			
								Debug.Log ("Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics - it will be added in a future release.");
								// Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics yet - it will be added in a future release.
				
								//Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position; in c# might be Vector2?
				
								//anchor = springJoint.transform.InverseTransformPoint(anchor);
								//springJoint.anchor = anchor;
						} else {
				
								//springJoint.anchor = Vector3.zero;
						}

						springJoint.distance = distance; // there is no distance in SpringJoint2D
						springJoint.dampingRatio = damper;// there is no damper in SpringJoint2D but there is a dampingRatio
						//springJoint.maxDistance = distance;  // there is no MaxDistance in the SpringJoint2D - but there is a 'distance' field 
						//	see http://docs.unity3d.com/Documentation/ScriptReference/SpringJoint2D.html
						//springJoint.maxDistance = distance;
						springJoint.connectedBody = hit.rigidbody;
						Vector3 localPoint = transform.InverseTransformPoint (hit.point);
						springJoint.connectedAnchor = localPoint;
			
				
						// maybe check if the 'fraction' is normalised. See http://docs.unity3d.com/Documentation/ScriptReference/RaycastHit2D-fraction.html
						StartCoroutine ("DragObject", hit.fraction);



				} // end of hit true condition

		} // end of update


		IEnumerator DragObject (float distance)
		{	
		
				float oldDrag = springJoint.connectedBody.drag;
				float oldAngularDrag = springJoint.connectedBody.angularDrag;

				springJoint.connectedBody.drag = drag;
				springJoint.connectedBody.angularDrag = angularDrag;

				Camera mainCamera = FindCamera ();

				while (Input.GetMouseButton (0)) {
						Ray ray = mainCamera.ScreenPointToRay (Input.mousePosition);
						springJoint.transform.position = ray.GetPoint (distance);
						yield return null;
				}
		
		
		
				if (springJoint.connectedBody) {	
						springJoint.connectedBody.drag = oldDrag;
						springJoint.connectedBody.angularDrag = oldAngularDrag;
						springJoint.connectedBody = null;
				}

		}

		Camera FindCamera ()
		{
				if (camera)
						return camera;
				else
						return Camera.main;
		}
}

using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour {

		public float gizmoSize = .01f;
		public Color gizmoColor = Color.grey;
		
		void OnDrawGizmos() {
		
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere(transform.position, gizmoSize);
		}
		
	

}
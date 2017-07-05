using UnityEngine;

using System.Collections;



public class DynamicDOF : MonoBehaviour {
	
	
	
	public Transform Origin;
	
	public Transform target;
	
	public DepthOfField34 DOF34;
	
	
	
	void Update () {
		
		
		
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); // Construct a ray from the current mouse coordinates
			
		//Ray ray = new Ray(Origin.position, Origin.forward);
		
		RaycastHit hit = new RaycastHit ();
		
		
		
		if (Physics.Raycast (ray, out hit, Mathf.Infinity))
			
		{
			
			DOF34.objectFocus = target;
			
			target.transform.position = hit.point;
			Debug.Log("We've hit an object and changed the DOF focus...");
			
		}
		
		else
			
		{
			
			DOF34.objectFocus = null;
			Debug.Log("We've not hit an object and not changed the DOF focus...");
			
		}
		
		
		
	}
	
}
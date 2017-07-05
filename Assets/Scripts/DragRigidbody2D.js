// Conversion of standard DragRigidbody.js to DragRigidbody2D.js
// Ian Grant v001

var distance =0.2;
var damper = 0.5; // damping ration in SpringJoint2D (0.0.- 1.0)
var frequency = 8.0;
var drag = 1.0; // this doesn't exist on 2D Spring...
var angularDrag = 5.0;
//var distance = 0.2;
var attachToCenterOfMass = false;


private var springJoint : SpringJoint2D;

function Update ()
{
	// Make sure the user pressed the mouse down
	if (!Input.GetMouseButtonDown (0))
		return;

	var mainCamera = FindCamera();
	var layerMask = 1 << 8;
	var hit : RaycastHit2D = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
	Debug.Log("Layermask: "+LayerMask.LayerToName(8));
	// I have proxy collider objects (empty gameobjects with a 2D Collider) as a child of a 3D rigidbody - simulating collisions between 2D and 3D objects
	// I therefore set any 'touchable' object to layer 8 and use the layerMask above for all touchable items
	
    if (hit.collider != null  && hit.rigidbody.isKinematic==true)
        {
            return;
        } 
        
    if (hit.collider != null  && hit.rigidbody.isKinematic==false) {
    	

		if (!springJoint)
		{
			var go = new GameObject("Rigidbody2D Dragger");
			var body : Rigidbody2D = go.AddComponent ("Rigidbody2D") as Rigidbody2D;
			springJoint = go.AddComponent ("SpringJoint2D");
			
			body.isKinematic = true;
		}
		
		springJoint.transform.position = hit.point;
		
		if (attachToCenterOfMass)
		{
			Debug.Log("Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics - it will be added in a future release.");
			// Currently 'centerOfMass' isn't reported for 2D physics like 3D Physics yet - it will be added in a future release.
			
			//var anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
			
			//anchor = springJoint.transform.InverseTransformPoint(anchor);
			//springJoint.anchor = anchor;
		} else{
		
			//springJoint.anchor = Vector3.zero;
			
		}
		
		springJoint.distance = distance; // there is no distance in SpringJoint2D
		springJoint.dampingRatio = damper;// there is no damper in SpringJoint2D but there is a dampingRatio
		//springJoint.maxDistance = distance;  // there is no MaxDistance in the SpringJoint2D - but there is a 'distance' field 
											//	see http://docs.unity3d.com/Documentation/ScriptReference/SpringJoint2D.html
		//springJoint.maxDistance = distance;
		springJoint.connectedBody = hit.rigidbody;
		
		
		// maybe check if the 'fraction' is normalised. See http://docs.unity3d.com/Documentation/ScriptReference/RaycastHit2D-fraction.html
		StartCoroutine ("DragObject", hit.fraction);
		
		} // end of hit true condition
		
	} // end of update

function DragObject (distance : float)
{	

	var oldDrag = springJoint.connectedBody.drag;
	var oldAngularDrag = springJoint.connectedBody.angularDrag;
	springJoint.connectedBody.drag = drag;
	springJoint.connectedBody.angularDrag = angularDrag;
	var mainCamera = FindCamera();
	while (Input.GetMouseButton (0))
	{
		var ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		springJoint.transform.position = ray.GetPoint(distance);
		yield;
	}
	
	
	
	if (springJoint.connectedBody)
	{	
		springJoint.connectedBody.drag = oldDrag;
		springJoint.connectedBody.angularDrag = oldAngularDrag;
		springJoint.connectedBody = null;
	}
}

function FindCamera ()
{
	if (camera)
		return camera;
	else
		return Camera.main;
}
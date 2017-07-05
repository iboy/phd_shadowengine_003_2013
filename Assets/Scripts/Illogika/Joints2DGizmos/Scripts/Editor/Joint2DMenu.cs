//----------------------------------------------
//                Joint2DGizmos
//         Copyright © 2013  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;

public static class Joint2DMenu{

	[AddComponentMenu("Physics 2D/Spring Joint 2D and Gizmo")]
	public static void AddSpringJoint() 
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			Undo.AddComponent<SpringJoint2DGizmo>(go);
		}
	}

	[AddComponentMenu("Physics 2D/Distance Joint 2D and Gizmo")]
	public static void AddDistanceJoint()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			Undo.AddComponent<DistanceJoint2DGizmo>(go);
		}
	}

	[AddComponentMenu("Physics 2D/Hinge Joint 2D and Gizmo")]
	public static void AddHingeJoint()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			Undo.AddComponent<HingeJoint2DGizmo>(go);
		}
	}

	[AddComponentMenu("Physics 2D/Slider Joint 2D and Gizmo")]
	public static void AddSliderJoint()
	{
		foreach(GameObject go in Selection.gameObjects)
		{
			Undo.AddComponent<SliderJoint2DGizmo>(go);
		}
	}
}

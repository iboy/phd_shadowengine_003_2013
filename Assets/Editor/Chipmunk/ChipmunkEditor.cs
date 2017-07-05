// Copyright 2013 Howling Moon Software. All rights reserved.
// See http://chipmunk2d.net/legal.php for more information.

using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class ChipmunkMenus : ScriptableObject
{
	
	// Can be changed if you prefer your Chipmunk stuff in it's own top-level menu.
	const string CHIPMUNK_ROOT_MENU = "Component/Chipmunk2D/";
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Chipmunk2D Unity Docs", false, 8000)]
	public static void Docs1() {
		Application.OpenURL("http://chipmunk-physics.net/documentation.php");
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Chipmunk2D General Docs", false, 8001)]
	public static void Docs2() {
		Application.OpenURL("http://chipmunk-physics.net/documentation.php");
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Chipmunk2D Forum", false, 8002)]
	public static void Docs3() {
		Application.OpenURL("http://chipmunk-physics.net/forum/");
	}
	
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Body", false, 1100)]
	public static void AddBody() {
		AddComponents<ChipmunkBody>();
	}

	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Circle Shape", false, 1120)]
	public static void AddCircleShape() {
		AddComponents<ChipmunkCircleShape>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Segment Shape", false, 1121)]
	public static void AddSegmentShape() {
		AddComponents<ChipmunkSegmentShape>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Box Shape", false, 1122)]
	public static void AddBoxShape() {
		AddComponents<ChipmunkBoxShape>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Poly Shape", false, 1123)]
	public static void AddPolyShape() {
		AddComponents<ChipmunkPolyShape>();
	}
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Damped Rotary Spring", false, 1220)]
	public static void AddDampedRotarySpring() {
		AddComponents<ChipmunkDampedRotarySpring>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Damped Spring", false, 1221)]
	public static void AddDampedSpring() {
		AddComponents<ChipmunkDampedSpring>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Gear Joint", false, 1222)]
	public static void AddGearJoint() {
		AddComponents<ChipmunkGearJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Groove Joint", false, 1223)]
	public static void AddGrooveJoint() {
		AddComponents<ChipmunkGrooveJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Pin Joint", false, 1224)]
	public static void AddPinJoint() {
		AddComponents<ChipmunkPinJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Pivot Joint", false, 1225)]
	public static void AddPivotJoint() {
		AddComponents<ChipmunkPivotJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Ratchet Joint", false, 1226)]
	public static void AddRatchetJoint() {
		AddComponents<ChipmunkRatchetJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Rotary Limit Joint", false, 1227)]
	public static void AddRotaryLimitJoint() {
		AddComponents<ChipmunkRotaryLimitJoint>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Simple Motor", false, 1228)]
	public static void AddSimpleMotor() {
		AddComponents<ChipmunkSimpleMotor>();
	}
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Slide Joint", false, 1229)]
	public static void AddSlideJoint() {
		AddComponents<ChipmunkSlideJoint>();
	}
	
	public static void AddComponents<T>() where T : Component{
		Undo.RegisterSceneUndo("add " + typeof(T).Name);
		
		foreach(GameObject go in Selection.gameObjects){
			go.AddComponent<T>();
		}
	}
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Body", true, 1100)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Circle Shape", true, 1120)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Segment Shape", true, 1121)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Box Shape", true, 1122)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Poly Shape", true, 1123)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Damped Rotary Spring", true, 1220)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Damped Spring", true, 1221)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Gear Joint", true, 1222)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Groove Joint", true, 1223)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Pin Joint", true, 1224)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Pivot Joint", true, 1225)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Ratchet Joint", true, 1226)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Rotary Limit Joint", true, 1227)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Simple Motor", true, 1228)]
	[MenuItem (CHIPMUNK_ROOT_MENU + "Add Slide Joint", true, 1229)]
//	[MenuItem (CHIPMUNK_ROOT_MENU + "Replace PhysX in selection with Chipmunk", true, 1210)]

	static bool ValidateAddChipmunkObjects() {
			Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep | SelectionMode.Editable | SelectionMode.ExcludePrefab);
			return (IS_SETUP && objs != null && objs.Length > 0);
	}
	
	public static void ReplaceComponent<T, N>(GameObject go) where T : Component where N : Component {
		if(go.GetComponent<T>()){
			if(go.GetComponent<N>() == null){
				go.AddComponent<N>();
			}
			DestroyImmediate(go.GetComponent<T>());
		}
	}
	
//	[MenuItem ("Chipmunk2D/Replace PhysX in selection with Chipmunk", false, 1210)]
//	public static void ReplacePhysXEverywhere() {
//		foreach(GameObject go in Selection.gameObjects){
//			ReplaceComponent<BoxCollider, ChipmunkBoxShape>(go);
//		}
//	}
	
	protected const string DLL = "Assets/Plugins/chipmunk.dll";
	protected const string DYLIB = "Assets/Plugins/libChipmunk.dylib";
	protected const string BUNDLE = "Assets/Plugins/chipmunk.bundle";
	
	protected static bool IS_SETUP = false;
	
	static ChipmunkMenus() {
		IS_SETUP = !(
			!UnityEditorInternal.InternalEditorUtility.HasPro() &&
			File.Exists(DLL) && File.Exists(DYLIB) && Directory.Exists(BUNDLE)
		);
		if(!IS_SETUP) {
		Debug.LogError(
			"You are running Unity Free and have not run the Chipmunk setup. " +
			"Please run the " + CHIPMUNK_ROOT_MENU + "Setup Chipmunk2D for Unity Free " + "menu."
		);
		}
	}
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Setup Chipmunk2D for Unity Free", false, 10000)]
	public static void InstallChipmunkFree(){
		string msg = "Note that due to license restrictions in Unity Free, you cannot make Mac or Windows builds using Chipmunk2D. Android and iOS builds are fine.";
		EditorUtility.DisplayDialog("Setting up Chipmunk2D to use with Unity Free", msg, "Ok");
		
		File.Move(DLL, "chipmunk.dll");
		File.Move(DYLIB, "libChipmunk.dylib");
		Directory.Delete(BUNDLE, true);
		
		EditorApplication.OpenProject(Directory.GetCurrentDirectory());
	}
	
	[MenuItem (CHIPMUNK_ROOT_MENU + "Setup Chipmunk2D for Unity Free", true, 10000)]
	public static bool ValidateInstall(){
		return !IS_SETUP;
	}
}

[CanEditMultipleObjects]
public class ChipmunkEditor : Editor {
	protected Vector2 CircleHandle(Vector3 pos){
		float size = HandleUtility.GetHandleSize(pos)*0.2f;
		return Handles.FreeMoveHandle(pos, Quaternion.identity, size, Vector3.zero, Handles.CircleCap);
	}
	
	protected Vector2 DotHandle(Vector3 pos){
		float size = HandleUtility.GetHandleSize(pos)*0.05f;
		return Handles.FreeMoveHandle(pos, Quaternion.identity, size, Vector3.zero, Handles.DotCap);
	}
	
	protected void SetupUndo(string message){
		Undo.SetSnapshotTarget(target, message);
		if(Input.GetMouseButtonDown(0)){
			Undo.CreateSnapshot();
			Undo.RegisterSnapshot();
		}
	}
}

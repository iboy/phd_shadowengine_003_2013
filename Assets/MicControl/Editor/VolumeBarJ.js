#pragma strict



@CustomEditor (MicControl)
class VolumeBarJ extends Editor  {

var ListenToMic = Selection.activeGameObject;
		
		
/////////////////////////////////////////////////////////////////////////////////////////////////		
		function OnInspectorGUI() {
var micInputValue=MicControl.loudness;
ProgressBar (micInputValue, "Loudness");


EditorUtility.SetDirty(target);
		
	// Show default inspector property editor
	DrawDefaultInspector ();
	}

	
	
		// Custom GUILayout progress bar.
	function ProgressBar (value : float, label : String) {
		// Get a rect for the progress bar using the same margins as a textfield:
		var rect : Rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, value, label);
		EditorGUILayout.Space ();
	}
	
}
using UnityEditor;
using UnityEngine;

/*
Batch Active Toggle
http://wiki.unity3d.com/index.php/BatchActiveToggle
Desi Quintans (CowfaceGames.com), 27 Jan 2014.
 
Press Ctrl + Shift + A to toggle active status of selected game objects.
This is a non-recursive activation toggle.
*/

public class BatchActiveToggle : Editor
{
	[MenuItem("GameObject/Toggle Active %#a")]
	static void BatchToggleActive ()
	{
		foreach (Transform t in Selection.transforms)
		{
			// foreach didn't like working on a GameObject array, so I have to get the transforms as the first step.
			GameObject go = t.gameObject;
			string undoText;
			
			if (go.activeSelf)
			{
				undoText = "Deactivate";
			}
			else
			{
				undoText = "Activate";
			}
			
			Undo.RecordObject(go, undoText + " " + go.name);
			go.SetActive (!go.activeSelf);
			Debug.Log(undoText.TrimEnd('e') + "ing " + go.name + ".");
		}
	}
}
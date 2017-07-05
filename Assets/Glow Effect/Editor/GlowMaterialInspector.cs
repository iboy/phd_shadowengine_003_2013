using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

// per shader keywords not available prior to 4.1.
#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1)
public class GlowMaterialInspector : MaterialEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!isVisible)
            return;

        Material targetMat = target as Material;
        if (targetMat.shaderKeywords == null || targetMat.shaderKeywords.Length == 0) {
            targetMat.shaderKeywords = new string[] { "GLOWEFFECT_USE_MAINTEX_OFF", "GLOWEFFECT_USE_GLOWTEX_OFF", "GLOWEFFECT_USE_GLOWCOLOR_OFF", 
                                                      "GLOWEFFECT_USE_VERTEXCOLOR_OFF", "GLOWEFFECT_MULTIPLY_COLOR_OFF" };
            EditorUtility.SetDirty(targetMat);
        }

        showToggleGUI("Glow using Main Texture", "GLOWEFFECT_USE_MAINTEX");
        showToggleGUI("Glow using Glow Texture", "GLOWEFFECT_USE_GLOWTEX");
        showToggleGUI("Glow using Glow Color", "GLOWEFFECT_USE_GLOWCOLOR");
        showToggleGUI("Glow using Vertex Color", "GLOWEFFECT_USE_VERTEXCOLOR");
        showToggleGUI("Multiply Glow by Glow Color Multiplyer", "GLOWEFFECT_MULTIPLY_COLOR");
    }

    private void showToggleGUI(string friendlyName, string keyword)
    {
        Material targetMat = target as Material;
        List<string> shaderKeywords = targetMat.shaderKeywords.OfType<string>().ToList();
        int index = -1;
        for (int i = 0; i < shaderKeywords.Count; ++i) {
            if (shaderKeywords[i].Contains(keyword)) {
                index = i;
                break;
            }
        }
        bool keywordEnabled = shaderKeywords.Contains(keyword);
        EditorGUI.BeginChangeCheck();
        keywordEnabled = EditorGUILayout.Toggle(friendlyName, keywordEnabled);
        if (EditorGUI.EndChangeCheck()) {
            if (keywordEnabled) {
                shaderKeywords[index] = keyword;
            } else {
                shaderKeywords[index] = string.Format("{0}_OFF",keyword);
            }
            targetMat.shaderKeywords = shaderKeywords.ToArray();
            EditorUtility.SetDirty(targetMat);
        }
    }
}
#endif
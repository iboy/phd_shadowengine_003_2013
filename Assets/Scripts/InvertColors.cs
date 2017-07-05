using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Invert Colors")]


public class InvertColors : ImageEffectBase {
	
	
	private Material mat;


	new void Start () {
		mat = new Material (
			"Shader \"Hidden/Invert\" {" +
			"SubShader {" +
			"	Pass {" +
			"		ZTest Always Cull Off ZWrite Off" +
			"		SetTexture [_RenderTex] { combine one-texture }" +
			"	}" +
			"}" +
			"}"
		);
	}


	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		
		RenderTexture.active = destination;
		source.SetGlobalShaderProperty ("_RenderTex");
		GL.PushMatrix ();
		GL.LoadOrtho ();
		// activate the first pass (in this case we know it is the only pass)
		mat.SetPass (0);
		// draw a quad
		GL.Begin (GL.QUADS);
		GL.TexCoord2 (0f, 0f); GL.Vertex3 (0f, 0f, 0.1f);
		GL.TexCoord2 (1f, 0f); GL.Vertex3 (1f, 0f, 0.1f);
		GL.TexCoord2 (1f, 1f); GL.Vertex3 (1f, 1f, 0.1f);
		GL.TexCoord2 (0f, 1f); GL.Vertex3 (0f, 1f, 0.1f);
		GL.End ();
		GL.PopMatrix ();
		
		
	}

}

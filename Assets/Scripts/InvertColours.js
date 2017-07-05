@script ExecuteInEditMode()
@script AddComponentMenu("Image Effects/Color Adjustments/Invert Colours");

private var mat : Material;

	function Start () {
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

function OnRenderImage (source : RenderTexture, dest : RenderTexture) {
		RenderTexture.active = dest;
		source.SetGlobalShaderProperty ("_RenderTex");
		GL.PushMatrix ();
		GL.LoadOrtho ();
		// activate the first pass (in this case we know it is the only pass)
		mat.SetPass (0);
		// draw a quad
		GL.Begin (GL.QUADS);
		GL.TexCoord2 (0, 0); GL.Vertex3 (0, 0, 0.1);
		GL.TexCoord2 (1, 0); GL.Vertex3 (1, 0, 0.1);
		GL.TexCoord2 (1, 1); GL.Vertex3 (1, 1, 0.1);
		GL.TexCoord2 (0, 1); GL.Vertex3 (0, 1, 0.1);
		GL.End ();
		GL.PopMatrix ();
	}
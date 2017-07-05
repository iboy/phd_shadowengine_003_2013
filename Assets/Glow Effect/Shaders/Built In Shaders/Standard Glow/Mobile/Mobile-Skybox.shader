// Simplified Skybox shader. Differences from regular Skybox one:
// - no tint color

Shader "Mobile/Skybox (Glow)" {
Properties {
	_FrontTex ("Front (+Z)", 2D) = "white" {}
	_BackTex ("Back (-Z)", 2D) = "white" {}
	_LeftTex ("Left (+X)", 2D) = "white" {}
	_RightTex ("Right (-X)", 2D) = "white" {}
	_UpTex ("Up (+Y)", 2D) = "white" {}
	_DownTex ("down (-Y)", 2D) = "white" {}
	_GlowTex ("Glow Texture", 2D) = "white" {}
	_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
	_GlowColorMult ("Glow Color Multiplier", Color) = (1, 1, 1, 1)
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	Pass {
		SetTexture [_FrontTex] { combine texture }
	}
	Pass {
		SetTexture [_BackTex]  { combine texture }
	}
	Pass {
		SetTexture [_LeftTex]  { combine texture }
	}
	Pass {
		SetTexture [_RightTex] { combine texture }
	}
	Pass {
		SetTexture [_UpTex]    { combine texture }
	}
	Pass {
		SetTexture [_DownTex]  { combine texture }
	}
}
}

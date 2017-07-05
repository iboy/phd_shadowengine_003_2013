// Simplified Skybox shader. Differences from regular Skybox one:
// - no tint color
// - no Alpha Channel

Shader "Mobile/Skybox (No Alpha)" {
Properties {
	_FrontTex ("Front (+Z)", 2D) = "white" {}
	_BackTex ("Back (-Z)", 2D) = "white" {}
	_LeftTex ("Left (+X)", 2D) = "white" {}
	_RightTex ("Right (-X)", 2D) = "white" {}
	_UpTex ("Up (+Y)", 2D) = "white" {}
	_DownTex ("down (-Y)", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" }
	Cull Off ZWrite Off Fog { Mode Off }
	Pass {
		SetTexture [_FrontTex] { constantColor (1, 1, 1, 0) combine texture * constant }
	}
	Pass {
		SetTexture [_BackTex]  { constantColor (1, 1, 1, 0) combine texture * constant }
	}
	Pass {
		SetTexture [_LeftTex]  { constantColor (1, 1, 1, 0) combine texture * constant }
	}
	Pass {
		SetTexture [_RightTex] { constantColor (1, 1, 1, 0) combine texture * constant }
	}
	Pass {
		SetTexture [_UpTex]    { constantColor (1, 1, 1, 0) combine texture * constant }
	}
	Pass {
		SetTexture [_DownTex]  { constantColor (1, 1, 1, 0) combine texture * constant }
	}
}
}

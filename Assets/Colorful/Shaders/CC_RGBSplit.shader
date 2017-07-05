Shader "Hidden/CC_RGBSplit"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_rgbShiftAmount ("RGB Shift Amount", Float) = 0.0
		_rgbShiftAngleSin ("RGB Shift Angle (Sin)", Float) = 0.0
		_rgbShiftAngleCos ("RGB Shift Angle (Cos)", Float) = 0.0
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

				#pragma vertex vert_img
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				half _rgbShiftAmount;
				half _rgbShiftAngleCos;
				half _rgbShiftAngleSin;

				fixed4 frag(v2f_img i):COLOR
				{
					half2 coords = i.uv;
					half  d = distance(coords, half2(0.5, 0.5));
					half  amount = _rgbShiftAmount * d * 2;
					half2 offset = amount * half2(_rgbShiftAngleCos, _rgbShiftAngleSin);
					half  cr  = tex2D(_MainTex, coords + offset).r;
					half2 cga = tex2D(_MainTex, coords).ga;
					half  cb  = tex2D(_MainTex, coords - offset).b;

					// Stupid hack to make it work with d3d9 (CG compiler bug ?)
					return half4(cr + 0.0000001, cga.x + 0.0000002, cb + 0.0000003, cga.y);
				}

			ENDCG
		}
	}

	FallBack off
}

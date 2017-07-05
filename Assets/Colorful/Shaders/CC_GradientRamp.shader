Shader "Hidden/CC_GradientRamp"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RampTex ("Ramp (RGB)", 2D) = "white" {}
		_amount ("Amount", Range(0.0, 1.0)) = 1.0
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
				#include "Colorful.cginc"

				sampler2D _MainTex;
				sampler2D _RampTex;
				half _amount;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = tex2D(_MainTex, i.uv);
					half2 lum = luminance(color).rr;
					half4 result = tex2D(_RampTex, lum);
					return lerp(color, result, _amount);
				}

			ENDCG
		}
	}

	FallBack off
}

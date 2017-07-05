Shader "Hidden/CC_Posterize"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_levels ("Levels", Range(2, 255)) = 4
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
				half _levels;

				fixed4 frag(v2f_img i):COLOR
				{
					half4 color = tex2D(_MainTex, i.uv);
					half4 lvls = half4(_levels, _levels, _levels, _levels);
					color = floor(color * lvls) / lvls;
					return color;
				}

			ENDCG
		}
	}

	FallBack off
}


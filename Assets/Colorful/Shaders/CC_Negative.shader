Shader "Hidden/CC_Negative"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
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

				sampler2D _MainTex;
				half _amount;

				fixed4 frag(v2f_img i):COLOR
				{
					fixed4 oc = tex2D(_MainTex, i.uv);
					fixed4 nc = fixed4(1, 1, 1, 1) - oc;
					nc.a = 1.0;
					return lerp(oc, nc, _amount);
				}

			ENDCG
		}
	}

	FallBack off
}

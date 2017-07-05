Shader "Self-Illumin/Diffuse (Glow)" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_Illum ("Illumin (A)", 2D) = "white" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
	_GlowTex ("Glow Texture", 2D) = "white" {}
	_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
	_GlowColorMult ("Glow Color Multiplier", Color) = (1, 1, 1, 1)
}
SubShader {
	Tags { "RenderType"="Glow" }
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _Illum;
fixed4 _Color;

struct Input {
	float2 uv_MainTex;
	float2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
	o.Alpha = c.a;
}
ENDCG
} 
FallBack "Self-Illumin/VertexLit (Glow)"
}

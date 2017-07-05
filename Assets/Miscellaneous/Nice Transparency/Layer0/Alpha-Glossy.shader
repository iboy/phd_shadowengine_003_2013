Shader "Nice Transparency/Layer0/Specular" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 0)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
	_BackFalloff("Back Falloff",Range(1,5)) = 1
	_FrontFalloff("Front Falloff",Range(0,5)) = 0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 300
Cull Front
CGPROGRAM
#pragma surface surf BlinnPhong alpha

sampler2D _MainTex;
fixed4 _Color;
half _Shininess;
half _BackFalloff;

struct Input {
	float2 uv_MainTex;
	half3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Normal = float3(0,0,-1);
	o.Specular = _Shininess;
	o.Alpha *=pow(saturate(dot(normalize(IN.viewDir),half3(0,0,-1))),_BackFalloff);
}
ENDCG
Cull Back
Pass 
{
	ZWrite On
	ColorMask 0
}
CGPROGRAM
#pragma surface surf BlinnPhong alpha

sampler2D _MainTex;
fixed4 _Color;
half _Shininess;
half _FrontFalloff;

struct Input {
	float2 uv_MainTex;
	half3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Normal = float3(0,0,1);
	o.Specular = _Shininess;
	o.Alpha *=pow(saturate(dot(normalize(IN.viewDir),half3(0,0,1))),_FrontFalloff);
}
ENDCG
}

Fallback "Transparent/VertexLit"
}

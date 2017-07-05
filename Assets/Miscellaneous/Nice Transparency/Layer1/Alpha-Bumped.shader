Shader "Nice Transparency/Layer1/Bumped Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BackFalloff("Back Falloff",Range(1,5)) = 1
	_FrontFalloff("Front Falloff",Range(0,5)) = 0
}

SubShader {
	Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 300

Cull Front
CGPROGRAM
#pragma surface surf Lambert alpha

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _BackFalloff;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	half3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	o.Normal.z *= -1;
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
#pragma surface surf Lambert alpha

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _FrontFalloff;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
	half3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	o.Alpha *=pow(saturate(dot(normalize(IN.viewDir),half3(0,0,1))),_FrontFalloff);
}
ENDCG
}

FallBack "Transparent/Diffuse"
}
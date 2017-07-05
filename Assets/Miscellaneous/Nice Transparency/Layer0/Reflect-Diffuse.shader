Shader "Nice Transparency/Layer0/Reflective/Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_BackFalloff("Back Falloff",Range(1,5)) = 1
	_FrontFalloff("Front Falloff",Range(0,5)) = 0
}
SubShader {
	LOD 200
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Cull Front
CGPROGRAM
#pragma surface surf Lambert alpha
#pragma target 3.0

sampler2D _MainTex;
samplerCUBE _Cube;

fixed4 _Color;
fixed4 _ReflectColor;
half _BackFalloff;

struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	half3 viewDir;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Normal = float3(0,0,-1);
	fixed4 reflcol = texCUBE (_Cube, WorldReflectionVector(IN,o.Normal));
	reflcol *= tex.a;
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	o.Alpha = c.a+reflcol.a*_ReflectColor.a;
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
#pragma target 3.0

sampler2D _MainTex;
samplerCUBE _Cube;

fixed4 _Color;
fixed4 _ReflectColor;
half _FrontFalloff;

struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	half3 viewDir;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Normal = float3(0,0,1);
	fixed4 reflcol = texCUBE (_Cube, WorldReflectionVector(IN,o.Normal));
	reflcol *= tex.a;
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	o.Alpha = c.a+reflcol.a*_ReflectColor.a;
	o.Alpha *=pow(saturate(dot(normalize(IN.viewDir),half3(0,0,1))),_FrontFalloff);
}
ENDCG
}
	
FallBack "Reflective/VertexLit"
} 

// Does not do anything in 3.x
Shader "Legacy Shaders/Diffuse Fast (No Alpha)" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}
Fallback "VertexLit (No Alpha)"
}
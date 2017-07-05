// Does not do anything in 3.x
Shader "Legacy Shaders/Diffuse Fast (Glow)" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_GlowTex ("Glow Texture", 2D) = "white" {}
	_GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
	_GlowColorMult ("Glow Color Multiplier", Color) = (1, 1, 1, 1)
}
Fallback "VertexLit (Glow)"
}
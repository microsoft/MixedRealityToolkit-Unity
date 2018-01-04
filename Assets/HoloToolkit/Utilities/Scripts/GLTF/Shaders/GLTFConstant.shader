Shader "GLTF/GLTFConstant" {
	Properties {
		_AmbientFactor ("Ambient Factor", Color) = (1,1,1,1)
		_EmissionColor("Emission Factor", Color) = (1,1,1,1)
		_LightmapFactor("Lightmap Factor", Color) = (1,1,1,1)
		_EmissionMap("Emission (RGB)", 2D) = "white" {}
		_LightMap("Lightmap (RGB)", 2D) = "white" {}
		_EmissionUV ("Emission UV Index", Int) = 0
		_LightUV("Lightmap UV Index", Int) = 0

		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}
	SubShader {
		Pass {
			Tags { "PerformanceChecks"="False" }
			LOD 200
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_Cull]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature VERTEX_COLOR_ON
			#pragma shader_feature EMISSION_MAP_ON
			#pragma shader_feature LIGHTMAP_ON
			#pragma shader_feature _ALPHATEST_ON

			#include "GLTFConstant.cginc"
			ENDCG
		}
	}
}

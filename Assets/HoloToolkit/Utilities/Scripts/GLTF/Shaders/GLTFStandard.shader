// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GLTF/GLTFStandard" {

	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		_Metallic("Metallic", Range(0,1)) = 0.0
		_Roughness("Roughness", Range(0,1)) = 0.5
		_MetallicRoughnessMap("Metallic Roughness", 2D) = "black" {}

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_OcclusionMap("Occlusion", 2D) = "white" {}
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0

		_EmissionColor("Color", Color) = (1,1,1,0)
		_EmissionMap("Emission", 2D) = "black" {}

		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0
	}

	CGINCLUDE
	#define UNITY_SETUP_BRDF_INPUT MetallicSetup
	ENDCG

	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry" "PerformanceChecks"="False" }
		LOD 300

		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_Cull]

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#define _NORMALMAP 1
			#define _EMISSION 1
			#define _METALLICGLOSSMAP 1
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertBase
			#pragma fragment fragBase

			#include "GLTFStandardInput.cginc"

#if !SHADER_API_GLES
			#include "UnityStandardCoreForward.cginc"
#endif
			ENDCG
		}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Cull [_Cull]
			Blend [_SrcBlend] One
			Fog { Color (0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------

			#define _NORMALMAP 1
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#define _METALLICGLOSSMAP 1

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd

			#include "GLTFStandardInput.cginc"

#if !SHADER_API_GLES
			#include "UnityStandardCoreForward.cginc"
#endif
			ENDCG
		}
		// ------------------------------------------------------------------
		//  Shadow rendering pass
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma target 3.0

			// -------------------------------------


			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma multi_compile_shadowcaster

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
	}

	SubShader 
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" "PerformanceChecks" = "False" }

		Cull [_Cull]
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		LOD 200

		Pass 
		{
			Name "ForwardBaseMobile"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			// Mobile Shader
			#pragma target 2.0
			// Vertex Colors
			#pragma multi_compile _ VERTEX_COLOR_ON
			// Occlusion packed in red channel of MetallicRoughnessMap
			#pragma multi_compile _ OCC_METAL_ROUGH_ON
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#include "GLTFMobileCommon.cginc"
			#pragma vertex gltfMobileVert
			#pragma fragment gltfMobileFrag
			ENDCG
		}
	}

	SubShader 
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" "PerformanceChecks" = "False" }

		Cull [_Cull]
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		LOD 100

		Pass 
		{
			Name "ForwardBaseVertexLit"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			// Vertex Lit Shader
			#pragma target 2.0
			// Vertex Colors
			#pragma multi_compile _ VERTEX_COLOR_ON
			// Occlusion packed in red channel of MetallicRoughnessMap
			#pragma multi_compile _ OCC_METAL_ROUGH_ON
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#include "GLTFVertexLitCommon.cginc"

			#pragma vertex gltfVertexFunc
			#pragma fragment gltfFragFunc

			ENDCG
		}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" "PerformanceChecks" = "False" }

		Cull[_Cull]
		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]
		LOD 50

		Pass
		{
			Name "ForwardBaseUnlit"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			// unlit 
			#pragma target 2.0
			// Vertex Colors
			#pragma multi_compile _ VERTEX_COLOR_ON
			// Occlusion packed in red channel of MetallicRoughnessMap
			#pragma multi_compile _ OCC_METAL_ROUGH_ON
			#pragma multi_compile _ _ALPHATEST_ON _ALPHABLEND_ON
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#include "GLTFVertexLitCommon.cginc"

			#pragma vertex gltfVertexUnlit
			#pragma fragment gltfFragUnlit

			ENDCG
		}
	}
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "MixedRealityToolkit/Obsolete/Fast Configurable 2 Sided"
{
    Properties
    {
		_Mode("Rendering Mode", Float) = 0.0

		[Toggle] _UseVertexColor("Vertex Color Enabled?", Float) = 0
		[Toggle] _UseMainColor("Main Color Enabled?", Float) = 0
		_Color("Main Color", Color) = (1,1,1,1)
		[Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0
		[NoScaleOffset]_MainTex("Main Texture", 2D) = "red" {}

		[Toggle] _UseOcclusionMap("Occlusion/Detail Texture Enabled?", Float) = 0
		[NoScaleOffset]_OcclusionMap("Occlusion/Detail Texture", 2D) = "blue" {}

		[Toggle] _UseAmbient("Ambient Lighting Enabled?", Float) = 1
		[Toggle] _UseDiffuse("Diffuse Lighting Enabled?", Float) = 1

		[Toggle] _SpecularHighlights("Specular Lighting Enabled?", Float) = 0
		[Toggle] _Shade4("Use additional lighting data? (Expensive!)", Float) = 0

		[Toggle] _ForcePerPixel("Light per-pixel (always on if a map is set)", Float) = 0

		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		[PowerSlider(5.0)]_Specular("Specular (Specular Power)", Range(1.0, 100.0)) = 10.0
		[Toggle] _UseSpecularMap("Use Specular Map? (per-pixel)", Float) = 0
		[NoScaleOffset]_SpecularMap("Specular Map", 2D) = "white" {}

		_Gloss("Gloss (Specular Scale)", Range(0.1, 10.0)) = 1.0
		[Toggle] _UseGlossMap("Use Gloss Map? (per-pixel)", Float) = 0
		[NoScaleOffset]_GlossMap("Gloss Map", 2D) = "white" {}

		[Toggle] _UseBumpMap("Normal Map Enabled? (per-pixel)", Float) = 0
		[NoScaleOffset][Normal] _BumpMap("Normal Map", 2D) = "bump" {}

		[Toggle] _UseReflections("Reflections Enabled?", Float) = 0
		[NoScaleOffset]_CubeMap("CubeMap", Cube) = "" {}
		_ReflectionScale("Reflection Scale", Range(0.01, 3.0)) = 2.0
		[Toggle]_CalibrationSpaceReflections("Reflect in calibration space?", Float) = 0

		[Toggle] _UseRimLighting("Rim Lighting Enabled?", Float) = 0
		[PowerSlider(.6)]_RimPower("Power", Range(0.1, 1.0)) = 0.7
		_RimColor("Color", Color) = (1,1,1,1)

		[Toggle] _UseEmissionColor("Emission Color Enabled?", Float) = 0
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		[Toggle] _UseEmissionMap("Emission Map Enabled?", Float) = 0
		[NoScaleOffset] _EmissionMap("Emission Map", 2D) = "blue" {}

		_TextureScaleOffset("Texture Scale (XY) and Offset (ZW)", Vector) = (1, 1, 0, 0)

		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"

		[Toggle] _AlphaTest("Alpha test enabled?", Float) = 0
		_Cutoff("Alpha Cutoff", Range(-0.1, 1.0)) = -0.1

		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
		[Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"
		[Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100
        Blend[_SrcBlend][_DstBlend]
        BlendOp[_BlendOp]
        ZTest[_ZTest]
        ZWrite[_ZWrite]

        Pass
        {
            Name "FRONT"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ColorMask[_ColorWriteMask]

            CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				//compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
				#pragma multi_compile_fwdbase

				//expands to several variants to handle different fog types
				#pragma multi_compile_fog

				//We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
				#pragma target 5.0
				#pragma only_renderers d3d11

				//shader features are only compiled if a material uses them
				#pragma shader_feature _USEVERTEXCOLOR_ON
				#pragma shader_feature _USEMAINCOLOR_ON
				#pragma shader_feature _USEMAINTEX_ON
				#pragma shader_feature _USESOCCLUSIONMAP_ON
				#pragma shader_feature _USEBUMPMAP_ON
				#pragma shader_feature _USEAMBIENT_ON
				#pragma shader_feature _USEDIFFUSE_ON
				#pragma shader_feature _USESPECULAR_ON
				#pragma shader_feature _USEGLOSSMAP_ON
				#pragma shader_feature _SHADE4_ON
				#pragma shader_feature _USEREFLECTIONS_ON
				#pragma shader_feature _USERIMLIGHTING_ON
				#pragma shader_feature _USEEMISSIONCOLOR_ON
				#pragma shader_feature _USEEMISSIONTEX_ON
				#pragma shader_feature _ALPHATEST_ON

				//scale and offset will apply to all
				#pragma shader_feature _MainTex_SCALE_ON
				#pragma shader_feature _MainTex_OFFSET_ON

				//may be set from script so generate both paths
				#pragma multi_compile __ _NEAR_PLANE_FADE_ON

				#include "FastConfigurable.cginc"			
            ENDCG
        }

        Pass
        {
            Name "BACK"
            Tags{ "LightMode" = "ForwardBase" }
            Cull Front
            ColorMask[_ColorWriteMask]

            CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				//compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
				#pragma multi_compile_fwdbase

				//expands to several variants to handle different fog types
				#pragma multi_compile_fog

				//We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
				#pragma target 5.0
				#pragma only_renderers d3d11

				//shader features are only compiled if a material uses them
				#pragma shader_feature _USEMAINCOLOR_ON
				#pragma shader_feature _USEMAINTEX_ON
				#pragma shader_feature _USESOCCLUSIONMAP_ON
				#pragma shader_feature _USEBUMPMAP_ON
				#pragma shader_feature _USEAMBIENT_ON
				#pragma shader_feature _USEDIFFUSE_ON
				#pragma shader_feature _USESPECULAR_ON
				#pragma shader_feature _USEGLOSSMAP_ON
				#pragma shader_feature _SHADE4_ON
				#pragma shader_feature _USEEMISSIONCOLOR_ON
				#pragma shader_feature _USEEMISSIONTEX_ON

				//scale and offset will apply to all
				#pragma shader_feature _MainTex_SCALE_ON
				#pragma shader_feature _MainTex_OFFSET_ON

				//may be set from script so generate both paths
				#pragma multi_compile __ _NEAR_PLANE_FADE_ON

				#define FLIP_NORMALS 1
				#include "FastConfigurable.cginc"
            ENDCG
        }
    } 

	CustomEditor "HoloToolkit.Unity.FastConfigurable2SidedGUI"
    Fallback "VertexLit" //for shadows
}
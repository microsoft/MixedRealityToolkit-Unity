// Very fast shader that uses the Unity lighting model.
// Compiles down to only performing the operations you're actually using.
// Uses material property drawers rather than a custom editor for ease of maintenance.

Shader "HoloToolkit/Lambertian Configurable Transparent"
{
    Properties
    {
        [Header(Main Color)]
        [Toggle] _UseColor("Enabled?", Float) = 0
        _Color("Main Color", Color) = (1,1,1,1)
        [Space(20)]

        [Header(Base(RGB))]
        [Toggle] _UseMainTex("Enabled?", Float) = 1
        _MainTex("Base (RGB)", 2D) = "white" {}
        [Space(20)]

        // Uses UV scale, etc from main texture
        [Header(Normalmap)]
        [Toggle] _UseBumpMap("Enabled?", Float) = 0
        [NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
        [Space(20)]

        // Uses UV scale, etc from main texture
        [Header(Emission(RGB))]
        [Toggle] _UseEmissionTex("Enabled?", Float) = 0
        [NoScaleOffset] _EmissionTex("Emission (RGB)", 2D) = "white" {}
        [Space(20)]

        [Header(Blend State)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
        [Space(20)]

        [Header(Other)]
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
        [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1.0 //"On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "PerformanceChecks" = "False" }
        Blend[_SrcBlend][_DstBlend]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Cull[_Cull]
        ColorMask[_ColorWriteMask]
        LOD 300

        CGPROGRAM
        // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
        #pragma target 5.0
        #pragma only_renderers d3d11

        #pragma surface surf Lambert vertex:vert alpha:fade

        #pragma shader_feature _USECOLOR_ON
        #pragma shader_feature _USEMAINTEX_ON
        #pragma shader_feature _USEBUMPMAP_ON
        #pragma shader_feature _USEEMISSIONTEX_ON
        #pragma shader_feature _NEAR_PLANE_FADE_ON

        #include "HoloToolkitCommon.cginc"
        #include "LambertianConfigurable.cginc"

        ENDCG
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#if _USEMAINTEX_ON
    UNITY_DECLARE_TEX2D(_MainTex);
#endif

#if _USECOLOR_ON
    float4 _Color;
#endif

#if _USEEMISSIONTEX_ON
    UNITY_DECLARE_TEX2D(_EmissionTex);
#endif

#if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
    float4 _MainTex_ST;
#endif

uniform float4x4 _SlicingWorldToLocal;
uniform float4 _SlicingLocalToCm;
float4 CutPlane;

struct appdata_t
{
    float4 vertex : POSITION;
    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON || !defined(LIGHTMAP_OFF)
        float2 texcoord : TEXCOORD0;
    #endif
    float3 normal : NORMAL;
};

struct v2f_surf
{
    float4 pos : SV_POSITION;
    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
        float2 pack0 : TEXCOORD0;
    #endif
    #ifndef LIGHTMAP_OFF
        float2 lmap : TEXCOORD1;
    #else
        float3 vlight : TEXCOORD1;
    #endif
    float4 worldPos : TEXCOORD2;
    LIGHTING_COORDS(3, 4)
    UNITY_FOG_COORDS(5)
};

float gridifyXZ(float3 worldPos)
{
    // Calculate the value in the next pixel:
    float3 nextPos = floor(worldPos + abs(ddx(worldPos)) + abs(ddy(worldPos)));
    float3 curPos = floor(worldPos);

    // Take the difference:
    float3 gridOn = saturate(nextPos - curPos);
    return max(gridOn.x, gridOn.z);
}

//really point inside corner check
float PointToPlaneAngle(float3 worldPos)
{
    float3 sl = mul(_SlicingWorldToLocal, float4(worldPos.xyz, 1)).xyz;
    return min(min(sl.x, sl.y), -sl.z);
}

float PointToPlaneDistance(float3 worldPos)
{
    return -dot(worldPos, CutPlane.xyz) + CutPlane.w;
}

inline float3 LightingLambertVS(float3 normal, float3 lightDir)
{
    float diff = max(0, dot(normal, lightDir));
    return _LightColor0.rgb * diff;
}

v2f_surf vert(appdata_t v)
{
    v2f_surf o;
    UNITY_INITIALIZE_OUTPUT(v2f_surf, o);

    o.pos = UnityObjectToClipPos(v.vertex);

    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
        o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    #endif

    #ifndef LIGHTMAP_OFF
        o.lmap.xy = v.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #else
        float3 worldN = UnityObjectToWorldNormal(v.normal);
        o.vlight = ShadeSH9(float4(worldN, 1.0));
        o.vlight += LightingLambertVS(worldN, _WorldSpaceLightPos0.xyz);
    #endif

    o.worldPos = mul(unity_ObjectToWorld, v.vertex);

    TRANSFER_VERTEX_TO_FRAGMENT(o);
    UNITY_TRANSFER_FOG(o, o.pos);
    return o;
}

float4 LambertianPixel(v2f_surf IN)
{
    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
        float2 uv_MainTex = IN.pack0.xy;
    #endif

    float4 surfaceColor;
    #if _USEMAINTEX_ON
        surfaceColor = UNITY_SAMPLE_TEX2D(_MainTex, uv_MainTex);
    #else
        surfaceColor = 1;
    #endif

    #if _USECOLOR_ON
        surfaceColor *= _Color;
    #endif

    float atten = LIGHT_ATTENUATION(IN);
    float4 finalColor = 0;

    #ifdef LIGHTMAP_OFF
        finalColor.rgb = surfaceColor.rgb * IN.vlight * atten;
    #else
        float3 lm = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
        #ifdef SHADOWS_SCREEN
            finalColor.rgb = surfaceColor.rgb * min(lm, atten * 2);
        #else
            finalColor.rgb = surfaceColor.rgb * lm;
        #endif
    #endif

    finalColor.a = surfaceColor.a;

    #ifdef _USEEMISSIONTEX_ON
        finalColor.rgb += UNITY_SAMPLE_TEX2D(_EmissionTex, uv_MainTex);
    #endif

    return finalColor;
}

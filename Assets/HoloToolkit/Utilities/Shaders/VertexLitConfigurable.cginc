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

struct appdata_t
{
    float4 vertex   : POSITION;
    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
    float3 normal : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
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
    LIGHTING_COORDS(2, 3)
    UNITY_FOG_COORDS(4)
    #if _NEAR_PLANE_FADE_ON
        float fade : TEXCOORD5;
    #endif
    UNITY_VERTEX_OUTPUT_STEREO
};

inline float3 LightingLambertVS(float3 normal, float3 lightDir)
{
    float diff = max(0, dot(normal, lightDir));
    return _LightColor0.rgb * diff;
}

v2f_surf vert(appdata_t v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    v2f_surf o;
    UNITY_INITIALIZE_OUTPUT(v2f_surf, o);

    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

    #if _USEMAINTEX_ON || _USEEMISSIONTEX_ON
        o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    #endif

    #ifndef LIGHTMAP_OFF
        o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    #else
        float3 worldN = UnityObjectToWorldNormal(v.normal);
        o.vlight = ShadeSH9(float4(worldN, 1.0));
        o.vlight += LightingLambertVS(worldN, _WorldSpaceLightPos0.xyz);
    #endif
    
    #if _NEAR_PLANE_FADE_ON
        o.fade = ComputeNearPlaneFadeLinear(v.vertex);
    #endif

    TRANSFER_VERTEX_TO_FRAGMENT(o);
    UNITY_TRANSFER_FOG(o, o.pos);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    return o;
}

float4 frag(v2f_surf IN) : SV_Target
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

    #if _NEAR_PLANE_FADE_ON
        finalColor.rgb *= IN.fade;
    #endif

    UNITY_APPLY_FOG(IN.fogCoord, finalColor);

    return finalColor;
}
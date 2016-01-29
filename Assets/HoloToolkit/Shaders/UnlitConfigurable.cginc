#include "UnityCG.cginc"

#if _USEMAINTEX_ON
    UNITY_DECLARE_TEX2D(_MainTex);
    float4 _MainTex_ST;
#endif

#if _USECOLOR_ON
    float4 _Color;
#endif

struct appdata_t
{
    float4 vertex : POSITION;
    #if _USEMAINTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
    #if defined (SHADER_API_D3D11) && defined (VRINSTANCINGEXT_ON)
        uint instId : SV_InstanceID;
    #endif				
};

struct v2f
{
    float4 vertex : SV_POSITION;
    #if _USEMAINTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
        UNITY_FOG_COORDS(1)
    #if defined (SHADER_API_D3D11) && defined (VRINSTANCINGEXT_ON)
        uint renderTargetIndex: SV_RenderTargetArrayIndex;
    #endif
};

v2f vert(appdata_t v)
{
    v2f o;
    #if defined (SHADER_API_D3D11) && defined (VRINSTANCINGEXT_ON)
        o.vertex = mul(UNITY_MATRIX_MVP_STEREO[v.instId], v.vertex);
        o.renderTargetIndex = v.instId;
    #else
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
    #endif

    #if _USEMAINTEX_ON
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
    #endif

    UNITY_TRANSFER_FOG(o, o.vertex);
    return o;
}

float4 frag(v2f i) : SV_Target
{
    float4 c;

    #if _USEMAINTEX_ON
        c = UNITY_SAMPLE_TEX2D(_MainTex, i.texcoord);
    #else
        c = 1;
    #endif

    #if _USECOLOR_ON
        c *= _Color;
    #endif

    UNITY_APPLY_FOG(i.fogCoord, c);
    return c;
}
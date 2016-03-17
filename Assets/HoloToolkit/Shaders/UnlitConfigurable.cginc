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
};

struct v2f
{
    float4 vertex : SV_POSITION;
    #if _USEMAINTEX_ON
        float2 texcoord : TEXCOORD0;
    #endif
    UNITY_FOG_COORDS(1)
	#if _NEAR_PLANE_FADE_ON
		float fade : TEXCOORD2;
	#endif	
};

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

    #if _USEMAINTEX_ON
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
    #endif
	
	#if _NEAR_PLANE_FADE_ON
		o.fade = ComputeNearPlaneFadeLinear(v.vertex);
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
	
	#if _NEAR_PLANE_FADE_ON
		c.rgb *= i.fade;
	#endif
	
    return c;
}
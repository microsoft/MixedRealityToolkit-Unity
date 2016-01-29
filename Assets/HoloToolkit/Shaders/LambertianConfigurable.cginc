#if _USEMAINTEX_ON
    UNITY_DECLARE_TEX2D(_MainTex);
#endif

#if _USECOLOR_ON		
    float4 _Color;
#endif

#if _USEBUMPMAP_ON
    UNITY_DECLARE_TEX2D(_BumpMap);
#endif

#if _USEEMISSIONTEX_ON
    UNITY_DECLARE_TEX2D(_EmissionTex);
#endif

struct Input
{
    //will get compiled out if not touched
    float2 uv_MainTex;
};

void surf(Input IN, inout SurfaceOutput o)
{
    float4 c;

    #if _USEMAINTEX_ON
        c = UNITY_SAMPLE_TEX2D(_MainTex, IN.uv_MainTex);
    #else
        c = 1;
    #endif

    #if _USECOLOR_ON
        c *= _Color;
    #endif

    o.Albedo = c.rgb;
    o.Alpha = c.a;

    #if _USEBUMPMAP_ON
        o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.uv_MainTex));
    #endif

    #if _USEEMISSIONTEX_ON
        o.Emission = UNITY_SAMPLE_TEX2D(_EmissionTex, IN.uv_MainTex);
    #endif
}
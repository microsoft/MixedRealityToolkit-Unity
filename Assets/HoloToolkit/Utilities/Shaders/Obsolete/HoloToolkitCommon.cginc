#include "UnityCG.cginc"

#ifndef HOLOTOOLKIT_COMMON
#define HOLOTOOLKIT_COMMON

float4 _NearPlaneFadeDistance;

// Helper function for focal plane fading
// Could instead be done non-linear in projected space for speed
inline float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -(UnityObjectToViewPos(vertex).z);
    return saturate(mad(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x));
}

// Diffuse lighting
inline float3 HoloTKLightingLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    return lightCol * max(0, dot(normal, lightDir));
}

// Specular lighting
inline float3 HoloTKLightingBlinnPhong(float3 normal, float3 lightDir, float lightCol, float3 viewDir, float specularPower, float specularScale, float3 specularColor)
{
    float3 h = normalize(lightDir + viewDir);	
    float nh = max(0, dot(normal, h));
    float spec = pow(nh, specularPower) * specularScale;
    
    return lightCol * specularColor * spec;
}

#endif //HOLOTOOLKIT_COMMON
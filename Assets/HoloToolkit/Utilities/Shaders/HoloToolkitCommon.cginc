#ifndef HOLOTOOLKIT_COMMON
#define HOLOTOOLKIT_COMMON

// Helper function for focal plane fading
float4 _NearPlaneFadeDistance;

float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -(mul(UNITY_MATRIX_MV, vertex).z);
    return saturate(mad(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x));
}

inline float3 HoloTKLightingLambertian(float3 normal, float3 lightDir, float3 lightCol)
{
    float diff = max(0, dot(normal, lightDir));
    return lightCol * diff;
}

inline float3 HoloTKLightingBlinnPhong(float3 normal, float3 lightDir, float lightCol, float3 viewDir, float specularAmount, float glossAmount, float3 specularColor)
{
    float3 h = normalize(lightDir + viewDir);
    float nh = max(0, dot(normal, h));
    float spec = pow(nh, specularAmount*128.0) * glossAmount;

    return lightCol * specularColor * spec;
}

#endif //HOLOTOOLKIT_COMMON
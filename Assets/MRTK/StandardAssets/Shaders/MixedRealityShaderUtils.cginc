// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#ifndef MRTK_SHADER_UTILS
#define MRTK_SHADER_UTILS

#if defined(_CLIPPING_PLANE)
inline float PointVsPlane(float3 worldPosition, float4 plane)
{
    float3 planePosition = plane.xyz * plane.w;
    return dot(worldPosition - planePosition, plane.xyz);
}
#endif

#if defined(_CLIPPING_SPHERE)
inline float PointVsSphere(float3 worldPosition, float4 sphere)
{
    return distance(worldPosition, sphere.xyz) - sphere.w;
}
#endif

#if defined(_CLIPPING_BOX)
inline float PointVsBox(float3 worldPosition, float3 boxSize, float4x4 boxInverseTransform)
{
    float3 distance = abs(mul(boxInverseTransform, float4(worldPosition, 1.0))) - boxSize;
    return length(max(distance, 0.0)) + min(max(distance.x, max(distance.y, distance.z)), 0.0);
}
#endif


#endif
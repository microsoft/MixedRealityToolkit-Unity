// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#ifndef HOLOTOOLKIT_COMMON
#define HOLOTOOLKIT_COMMON

float4 _NearPlaneFadeDistance;

// Helper function for focal plane fading
// Could instead be done non-linear in projected space for speed
inline float ComputeNearPlaneFadeLinear(float4 vertex)
{
    float distToCamera = -UnityObjectToViewPos(vertex).z;
    return saturate(mad(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x));
}

#endif //HOLOTOOLKIT_COMMON
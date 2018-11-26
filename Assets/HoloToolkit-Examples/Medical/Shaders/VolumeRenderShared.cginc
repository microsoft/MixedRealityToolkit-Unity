// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#include "math.cginc"

float4 _VolBufferSize;
float4 _VolTextureFactor;
float4x4 _WorldToVolume;
sampler3D _VolTex;

float4 SlabMin; //xyz are offsets of respective slab near planes.  w is unused.
float4 SlabMax; //xyz are offsets of respective slab far planes.  w is unused.

struct vertexInput
{
    float4 vertex : POSITION;
};

struct vertexOutputFull
{
    float4 pos : SV_POSITION;
    float3 posVolSpace : TEXCOORD0;
    nointerpolation float3 cameraPosVolSpace : TEXCOORD2;
};

float4 SampleVolTex(sampler3D tex, float3 coord)
{
    return tex3D(tex, coord * _VolTextureFactor.xyz);
}

float4 AlphaBlendPremultiplied(float4 dst, float4 src)
{
    return src + dst * (1.0 - src.a);
}

float4 AlphaBlendNonPremultiplied(float4 dst, float4 src)
{
    float4 res = (src * src.a) + (dst - dst * src.a);
    res.a = src.a + (dst.a - dst.a*src.a);
    return res;
}

vertexOutputFull vertFull(vertexInput input)
{
    vertexOutputFull output;
    UNITY_INITIALIZE_OUTPUT(vertexOutputFull, output);

    float4 posWorldSpace = mul(unity_ObjectToWorld, input.vertex);
    output.posVolSpace = mul(_WorldToVolume, posWorldSpace);

    //TODO: this should come down in a constant buffer
    output.cameraPosVolSpace = mul(_WorldToVolume, float4(_WorldSpaceCameraPos, 1));

    output.pos = UnityObjectToClipPos(input.vertex);

    return output;
}
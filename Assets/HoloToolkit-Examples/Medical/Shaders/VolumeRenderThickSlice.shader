// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Serra/Volume Renderer Thick Slice" 
{
	Properties
	{
		_VolTex("Volume Texture", 3D) = "white" {}
	}

    SubShader
    {
        Tags { "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        ZTest Less

        Pass
        {
            CGPROGRAM

            #pragma target 5.0
            #pragma only_renderers d3d11

            #include "UnityCG.cginc"
            #include "VolumeRenderShared.cginc"
            #include "PlaneCut.cginc"

            #pragma vertex vertFull  
            #pragma fragment frag 

            float4 volTraceShallow(float3 front, float3 cameraPosVolSpace)
            {
                //max loops of 20 seems sufficient to fill in volume, need better interpolation instead
                float startOffset = Random(front.xy);

                const float sliceDepth = .15;
                float3 toVolume = front - cameraPosVolSpace;
                float3 back = front + (normalize(toVolume) * sliceDepth);

                //TODO: shouldn't bother with subvoxels
                const int maxLoops = 20; 
 
                // walking back to front
                float stepDelta = 1.0 / maxLoops;
                float4 curColor = float4(0, 0, 0, 0);

                float currentRayDistance = 1.0 - (stepDelta * startOffset);

                float3 unitCoord = saturate(lerp(front, back, currentRayDistance));
                float3 unitCoordI1 = saturate(lerp(front, back, currentRayDistance - stepDelta));
                float3 unitCoordDelta = unitCoordI1 - unitCoord;

                [unroll]
                for (int i = 0; i < maxLoops; ++i)
                {
                    float4 sampleCol = SampleVolTex(_VolTex, unitCoord);
                    curColor = AlphaBlendPremultiplied(curColor, sampleCol);
                    unitCoord += unitCoordDelta;
                }

                return curColor;
            }

            float4 frag(vertexOutputFull input) : COLOR
            {
				const float3 SlabOffset = float3(0.02, 0.02, 0.02);               
                clip(input.posVolSpace - SlabMin + SlabOffset);
                clip(SlabMax - input.posVolSpace + SlabOffset);

                float4 volColor = volTraceShallow(input.posVolSpace, input.cameraPosVolSpace);

                if (!any(volColor.rgb))
                {
                    clip(-1);
                }

                volColor.a = 1.0;
                return volColor;
             }

            ENDCG
        }
    }
    Fallback "Diffuse"
}

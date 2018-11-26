// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Serra/Volume Renderer Slice" 
{
	Properties
	{
		_VolTex("Volume Texture", 3D) = "white" {}
	}

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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

            float4 frag(vertexOutputFull input) : COLOR
            {
				const float3 SlabOffset = float3(0.02, 0.02, 0.02);
                clip(input.posVolSpace - SlabMin + SlabOffset);
                clip(SlabMax - input.posVolSpace + SlabOffset);

                float4 volColor = SampleVolTex(_VolTex, saturate(input.posVolSpace));

				if (!any(volColor.rgb))
				{
					clip(-1);
				}

                //scale up brightness since we're not add blending through volume
                volColor.rgb *= 1.75;
                volColor.a = 1.0;

		        return volColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}

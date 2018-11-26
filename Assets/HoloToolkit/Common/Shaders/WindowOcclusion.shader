// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

///
/// Simple occlusion shader that can be used to hide other objects.
/// This prevents other objects from being rendered by drawing invisible 'opaque' pixels to the depth buffer.
///
Shader "MixedRealityToolkit/WindowOcclusion"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }

        Pass
        {
            ColorMask 0 // Color will not be rendered.

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_base v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}
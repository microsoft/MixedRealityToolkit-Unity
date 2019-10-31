// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

Shader "Mixed Reality Toolkit/Depth Buffer Viewer"
{
    Properties
    {
        _DepthTex("Texture", 2D) = "black" {}
        _MainTex("Base (RGB)", 2D) = "green" {}
    }

    SubShader
    {
        Pass
        {
        CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            sampler2D _DepthTex;

            float4 frag(v2f_img i) : COLOR
            {
                /*
                float4 white = float4(1.0f, 1.0f, 1.0f, 1.0f);
                float4 clear = float4(0.0f, 0.0f, 0.0f, 1.0f);
                float4 black = float4(0.0f, 0.0f, 0.0f, 0.0f);

                float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_DepthTex, i.uv));
                float4 c = tex2D(_MainTex, i.uv);

                if (!all(c == clear))
                {
                    if (d == 0)
                    {
                        return float4(1, 0, 0, 1);
                    }
                }

                return c;
                */
                // LKG
                return Linear01Depth(SAMPLE_DEPTH_TEXTURE(_DepthTex, i.uv));
            }
        ENDCG
        }
    }
}
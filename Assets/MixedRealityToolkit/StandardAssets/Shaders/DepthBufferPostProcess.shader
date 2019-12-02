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
                return Linear01Depth(SAMPLE_DEPTH_TEXTURE(_DepthTex, i.uv));
            }
        ENDCG
        }
    }
}
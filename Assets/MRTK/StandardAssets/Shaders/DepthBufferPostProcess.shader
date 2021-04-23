// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// NOTE: MRTK Shaders are versioned via the MRTK.Shaders.sentinel file.
// When making changes to any shader's source file, the value in the sentinel _must_ be incremented.

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
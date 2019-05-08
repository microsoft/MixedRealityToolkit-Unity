// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/BGRToRGB"
{
    Properties
    {
        _FlipTex("Texture", 2D) = "white" {}
        _AlphaScale("AlaphScale", float) = 1
        _YFlip("YFlip", int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FlipTex;
            float _AlphaScale;
            float _YFlip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.y = (_YFlip * (1 - i.uv.y)) + ((1 - _YFlip) * i.uv.y);
                fixed4 col = tex2D(_FlipTex, i.uv);
                col.a *= _AlphaScale;
                return col.bgra;
            }
            ENDCG
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/HoloAlpha"
{
    Properties
    {
        _BackTex("BackTex", 2D) = "white" {}
        _FronTex("FrontTex", 2D) = "white" {}
        _Alpha("Alpha", float) = 0.9
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

            sampler2D _BackTex;
            sampler2D _FrontTex;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 backCol = tex2D(_BackTex, i.uv);
                fixed4 frontCol = tex2D(_FrontTex, i.uv);
            
                fixed4 composite = backCol * (1 - _Alpha) + frontCol * _Alpha;
                composite.a = 1.0f;
                return composite;
            }
            ENDCG
        }
    }
}

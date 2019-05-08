// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/Downsample"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightOffset("HeightOffset", Float) = 0
        _WidthOffset("WidthOffset", Float) = 0
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

            sampler2D _MainTex;
            float HeightOffset;
            float WidthOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                const float epsilon = .00001f;

                fixed4 col1 = tex2D(_MainTex, i.uv + float2(WidthOffset, HeightOffset));
                fixed4 col2 = tex2D(_MainTex, i.uv + float2(-WidthOffset, HeightOffset));
                fixed4 col3 = tex2D(_MainTex, i.uv + float2(WidthOffset, -HeightOffset));
                fixed4 col4 = tex2D(_MainTex, i.uv + float2(-WidthOffset, -HeightOffset));

                // don't need the fancy alpha calculations now that we start rendering by splating the real world into the background
                //float weight = col1.a + col2.a + col3.a + col4.a + epsilon;
                //fixed4 col = ((col1 * col1.a) + (col2 * col2.a) + (col3 * col3.a) + (col4 * col4.a)) / weight;
                //col.a = (weight - epsilon) / 4;

                return (col1 + col2 + col3 + col4) / 4;
            }
            ENDCG
        }
    }
}

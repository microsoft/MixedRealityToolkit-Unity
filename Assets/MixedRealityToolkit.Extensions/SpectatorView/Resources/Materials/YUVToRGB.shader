// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/YUVToRGB"
{
    Properties
    {
        _YUVTex ("Texture", 2D) = "white" {}
        _Width("Width", int) = 0
        _Height("Height", int) = 0
        _AlphaScale("AlaphScale", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma exclude_renderers d3d11_9x
            
            #include "UnityCG.cginc"
            #include "YUVHelper.cginc"

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

            sampler2D _YUVTex;
            int _Width;
            int _Height;
            float _AlphaScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.y = 1 - i.uv.y;

                uint index = (int)floor(((int)floor(_Height * i.uv.y) * _Width) + floor(_Width * i.uv.x));
                uint yuvIndex = index / 2;

                fixed2 uv;
                uv.x = ((yuvIndex / 4.0f) % (_Width / 4.0f)) / (_Width / 4.0f);
                uv.y = ((yuvIndex / 4.0f) / (_Width / 4.0f)) / _Height;

                fixed4 yuvPixel = tex2D(_YUVTex, uv);
                int val = 0;
                if (index % 2 != 0)
                {
                    val = 1;
                }
                fixed4 backCol = GetRGBA(yuvPixel, val);

                backCol.a *= _AlphaScale;

                return backCol.rgba;
            }
            ENDCG
        }
    }
}

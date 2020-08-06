// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Hidden/ChannelPacker"
{
    Properties
    {
        _MetallicMap("Metallic Map", 2D) = "black" {}
        _MetallicMapChannel("Metallic Map Channel", Int) = 0 // Red.
        _MetallicUniform("Metallic Uniform", Float) = -0.01
        _OcclusionMap("Occlusion Map", 2D) = "white" {}
        _OcclusionMapChannel("Occlusion Map Channel", Int) = 1 // Green.
        _OcclusionUniform("Occlusion Uniform", Float) = -0.01
        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionMapChannel("Emission Map Channel", Int) = 4 // RGBAverage.
        _EmissionUniform("Emission Uniform", Float) = -0.01
        _SmoothnessMap("Smoothness Map", 2D) = "gray" {}
        _SmoothnessMapChannel("Smoothness Map Channel", Int) = 3 // Alpha.
        _SmoothnessUniform("Smoothness Uniform", Float) = -0.01
    }
    SubShader
    {
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MetallicMap;
            int _MetallicMapChannel;
            float _MetallicUniform;
            sampler2D _OcclusionMap;
            int _OcclusionMapChannel;
            float _OcclusionUniform;
            sampler2D _EmissionMap;
            int _EmissionMapChannel;
            float _EmissionUniform;
            sampler2D _SmoothnessMap;
            int _SmoothnessMapChannel;
            float _SmoothnessUniform;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 ToGrayScale(fixed4 color)
            {
                return color.r * 0.21 + color.g * 0.71 + color.b * 0.08;
            }

            fixed Sample(fixed4 color, int channel, float uniformValue)
            {
                if (uniformValue >= 0.0)
                {
                    return uniformValue;
                }

                if (channel == 4)
                {
                    return ToGrayScale(color);
                }

                return color[channel];
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 output;

                output.r = Sample(tex2D(_MetallicMap, i.uv), _MetallicMapChannel, _MetallicUniform);
                output.g = Sample(tex2D(_OcclusionMap, i.uv), _OcclusionMapChannel, _OcclusionUniform);
                output.b = Sample(tex2D(_EmissionMap, i.uv), _EmissionMapChannel, _EmissionUniform);
                output.a = Sample(tex2D(_SmoothnessMap, i.uv), _SmoothnessMapChannel, _SmoothnessUniform);

                return output;
            }

            ENDCG
        }
    }
}

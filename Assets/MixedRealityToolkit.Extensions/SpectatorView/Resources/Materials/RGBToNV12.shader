// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/RGBToNV12"
{
    Properties
    {
        _RGBTex("Texture", 2D) = "white" {}
        _Width("Width", int) = 0
        _Height("Height", int) = 0
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma target 4.5
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "YUVHelper.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            struct pixeldata
            {
                fixed4 yuv1;
                fixed4 yuv2;
            };

            sampler2D _RGBTex;
            int _Width;
            int _Height;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            pixeldata GetYUV(uint rgbaIndex)
            {
                pixeldata pd;

                fixed2 uv1, uv2, uv3, uv4;
                uv1.x = ((rgbaIndex / 4.0f) % (_Width / 4.0f)) / (_Width / 4.0f);
                uv1.y = 1 - ((rgbaIndex / 4.0f) / (_Width / 4.0f)) / _Height;
                uv2.x = (((rgbaIndex + 1) / 4.0f) % (_Width / 4.0f)) / (_Width / 4.0f);
                uv2.y = 1 - (((rgbaIndex + 1) / 4.0f) / (_Width / 4.0f)) / _Height;
                uv3.x = (((rgbaIndex + 2) / 4.0f) % (_Width / 4.0f)) / (_Width / 4.0f);
                uv3.y = 1 - (((rgbaIndex + 2) / 4.0f) / (_Width / 4.0f)) / _Height;
                uv4.x = (((rgbaIndex + 3) / 4.0f) % (_Width / 4.0f)) / (_Width / 4.0f);
                uv4.y = 1 - (((rgbaIndex + 3) / 4.0f) / (_Width / 4.0f)) / _Height;

                fixed4 col1 = tex2D(_RGBTex, uv1);
                fixed4 col2 = tex2D(_RGBTex, uv2);
                fixed4 col3 = tex2D(_RGBTex, uv3);
                fixed4 col4 = tex2D(_RGBTex, uv4);

                pd.yuv1 = GetYUV(col1, col2);
                pd.yuv2 = GetYUV(col3, col4);

                return pd;
            }

            fixed4 frag(v2f i) : SV_Target
            { 
                float w = _Width;
                uint index = (int)floor(((int)floor(_Height * i.uv.y) * w) + floor(w * i.uv.x));
                int maxIndex = (1.5f * _Width * _Height);
                int rgbaIndex = index * 4;

                if (rgbaIndex > maxIndex)
                {
                    return fixed4(0, 0, 0, 1);
                }

                if (rgbaIndex < (_Width * _Height))
                {
                    // Y values
                    pixeldata pd = GetYUV(rgbaIndex);
                    return fixed4(pd.yuv1.g, pd.yuv1.a, pd.yuv2.g, pd.yuv2.a);
                }
                else
                {
                    // UV values
                    rgbaIndex -= (_Width * _Height);
                    int row = (int)(rgbaIndex / (uint)_Width);
                    rgbaIndex += row * _Width;

                    pixeldata pd = GetYUV(rgbaIndex);

                    return fixed4(pd.yuv1.r, pd.yuv1.b, pd.yuv2.r, pd.yuv2.b);
                }
            }
            ENDCG
        }
    }
}


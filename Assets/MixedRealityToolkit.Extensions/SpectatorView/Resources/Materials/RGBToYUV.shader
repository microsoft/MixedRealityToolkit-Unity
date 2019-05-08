// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "SV/RGBToYUV"
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"
            #include "YUVHelper.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
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

            float4 sampleTexture(float2 uv)
            {
                float2 offset = float2(0, 0);
                if (uv.x < 0.0f)
                {
                    offset.y = (0.5 / _Height);
                }
                else
                {
                    offset.y = -(0.5 / _Height);
                }
                return tex2D(_RGBTex, uv + offset);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 holoUV = i.uv;

                // yflip because we output the texture upside down
                holoUV.y = 1.f - holoUV.y;
                // UV range with YUV data is 0.5-1 but holo data is 0-1
                holoUV = 2 * (holoUV - float2(0.5, 0.5));

                // calculated UV is now dircetly between two pixels, offset to center on them
                holoUV.x -= 0.5f / _Width;
                fixed4 frontCol = sampleTexture(holoUV);
                holoUV.x += 1.0f / _Width;
                fixed4 frontCol2 = sampleTexture(holoUV);

                return GetYUV(frontCol, frontCol2);
            }
            ENDCG
        }
    }
}

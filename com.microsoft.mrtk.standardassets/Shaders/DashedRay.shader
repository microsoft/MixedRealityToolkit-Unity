// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Mixed Reality Toolkit/Dashed Ray"
{
    Properties
    {
        [Header(Dashed Line)]
            _Color_("Tint Color", Color) = (1,1,1,1)
            _Frequency_("Frequency", Float) = 1
            _Power_("Power", Float) = 1
            [PerRendererData]_Shift_("Shift", Float) = 0
            [PerRendererData]_Phase_("Phase", Float) = 0
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Tags {"DisableBatching" = "True"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 4.0
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            half4 _Color_;
            half _Frequency_;
            half _Power_;
            half _Shift_;
            half _Phase_;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                half4 color : COLOR0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the "wave", and multiply by _Power_ to make the cutoffs sharper/fuzzier.
                half amt = (sin((1-i.uv.x) * _Frequency_ + _Phase_) + _Shift_ * 1.1) * _Power_;
                return half4(_Color_.rgb * i.color.rgb, clamp(amt, 0.0, 1.0) * _Color_.a * i.color.a);
            }
            ENDCG
        }
    }
}

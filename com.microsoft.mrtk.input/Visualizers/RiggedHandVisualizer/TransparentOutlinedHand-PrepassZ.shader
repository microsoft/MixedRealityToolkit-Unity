// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//                              Important note:
//
// This is the first pass of a three-pass shader ("TransparentOutlinedHand-XXXX").
// Unfortunately, this shader is split into three shaders (and three materials),
// as URP does not support multi-pass shaders. In order to support both built-in
// pipelines and URP, we split the multi-pass shader into three shaders and use
// material chaining on the hand mesh. Using this shader alone is not recommended.

Shader "Mixed Reality Toolkit/Transparent Outlined Hand (PrepassZ)" {

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        // Pre-pass Zwrite. Makes the semitransparent hands
        // sort correctly (i.e., the fingers blend onto the
        // background, but not each other!)
        Pass {
            ZWrite On
            ColorMask 0 // Prevents Z prepass from actually drawing anything.
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            uniform float _HandThickness;
 
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Inflate/deflate hand based on thickness.
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _HandThickness);

                return o;
            }

            fixed4 frag() : SV_Target { return 0; } // nop!

            ENDCG
        }
    }
    FallBack "Diffuse"
}
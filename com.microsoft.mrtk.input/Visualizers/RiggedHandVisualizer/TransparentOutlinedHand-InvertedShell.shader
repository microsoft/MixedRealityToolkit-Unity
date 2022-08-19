// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//                              Important note:
//
// This is the third pass of a three-pass shader ("TransparentOutlinedHand-XXXX").
// Unfortunately, this shader is split into three shaders (and three materials),
// as URP does not support multi-pass shaders. In order to support both built-in
// pipelines and URP, we split the multi-pass shader into three shaders and use
// material chaining on the hand mesh. Using this shader alone is not recommended.

Shader "Mixed Reality Toolkit/Transparent Outlined Hand (Inverted Shell)" {
    Properties {
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineColorPinching ("Outline Color (Pinching)", Color) = (1,1,1,1)
        _OutlineThickness ("_OutlineThickness", Range(0.0,0.00003)) = 0.000012
        _HandThickness ("_HandThickness", Range(-0.0001,0.0001)) = 0.0
        [PerRendererData]_PinchAmount ("Pinch Amount", Float) = 0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200
        
        // Draw the outline as an inverted shell.
        // We invert the cull order (Cull Front) to
        // draw a nice outline around the mesh.
        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float _OutlineThickness;
            uniform float _HandThickness;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * (_HandThickness + _OutlineThickness));
                o.color = v.color;
                
                return o;
            }

            uniform float4 _OutlineColor;
            uniform float4 _OutlineColorPinching;
            uniform float _PinchAmount;

            fixed4 frag(v2f i) : SV_Target
            {
                // Vertex color green channel controls whether the non-pinching outline color or the
                // pinch color is used. This determines where the pinch "glow" effect appears on the
                // hand; generally, the tips of the forefinger and thumb.
                float4 blendedOutlineColor = lerp(_OutlineColor, _OutlineColorPinching, _PinchAmount * i.color.g);

                // Fade the entire result based on the red channel. This is used to fade the hand
                // out by the wrist, so the abrupt edge of the hand model is not visible.
                return float4(blendedOutlineColor.r, blendedOutlineColor.g, blendedOutlineColor.b, blendedOutlineColor.a * i.color.r);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Mixed Reality Toolkit/Transparent Outlined Hand" {
    Properties {
        _HandColor ("Hand Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineColorPinching ("Outline Color (Pinching)", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _OutlineThreshold ("_OutlineThreshold", Range(0.0,1)) = 0.95
        _OutlineSmoothing ("_OutlineSmoothing", Range(0,0.1)) = 0.05
        _OutlineExponent ("_OutlineExponent", Range(0,10)) = 1
        _IlluminationExponent ("Illumination Exponent", Range(0,10)) = 1
        _IlluminationAmount ("Illumination Amount", Range(0,10)) = 1
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200
 
        // Pre-pass Zwrite
        Pass {
            ZWrite On
            ColorMask 0
 
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
 
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag() : SV_Target { return 0; }

            ENDCG
        }

        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
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
                float3 viewDir: TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color;

                // Distance-invariant view pos. Create "normalized view point"
                // offset from object origin, then construct a new view vector
                // from the synthetic view point.
                float3 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0)).xyz;
                float3 cameraToObjectOrigin = normalize(objectOrigin - _WorldSpaceCameraPos);
                float3 offsetViewPos = objectOrigin - cameraToObjectOrigin * 1.0f;
                o.viewDir = mul(unity_ObjectToWorld, v.vertex) - offsetViewPos;
                
                return o;
            }

            uniform float _OutlineThreshold;
            uniform float _OutlineSmoothing;
            uniform float _OutlineExponent;

            uniform float _IlluminationAmount;
            uniform float _IlluminationExponent;

            uniform float4 _HandColor;
            uniform float4 _OutlineColor;
            uniform float4 _OutlineColorPinching;

            fixed4 frag(v2f i) : SV_Target
            {
                half rim = 1.0 - abs(dot(i.viewDir, i.normal));
                half spotlight = 1.0 - rim;

                rim = pow(rim, _OutlineExponent);
                half amt = smoothstep(_OutlineThreshold, _OutlineThreshold + _OutlineSmoothing, rim);

                // Vertex color green channel controls whether the non-pinching outline color or the
                // pinch color is used. This determines where the pinch "glow" effect appears on the
                // hand; generally, the tips of the forefinger and thumb.
                float4 blendedOutlineColor = lerp(_OutlineColor, _OutlineColorPinching, i.color.g);
                
                // Lerp betwen the normal transparent hand color and the outline color, based on the
                // rimlight amount determined earlier.
                float4 blended = lerp(_HandColor, blendedOutlineColor, amt) + pow(spotlight, _IlluminationExponent) * _IlluminationAmount;
                
                // Fade the entire result based on the red channel. This is used to fade the hand
                // out by the wrist, so the abrupt edge of the hand model is not visible.
                return float4(blended.r, blended.g, blended.b, blended.a * i.color.r);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
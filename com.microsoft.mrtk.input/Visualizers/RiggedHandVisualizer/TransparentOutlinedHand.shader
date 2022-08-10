// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Mixed Reality Toolkit/Transparent Outlined Hand" {
    Properties {
        _HandColor ("Hand Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineColorPinching ("Outline Color (Pinching)", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _OutlineThickness ("_OutlineThickness", Range(0.0,0.00003)) = 0.000012
        _HandThickness ("_HandThickness", Range(-0.0001,0.0001)) = 0.0
        _IlluminationExponent ("Illumination Exponent", Range(0,10)) = 1
        _IlluminationAmount ("Illumination Amount", Range(0,10)) = 1
        [PerRendererData]_PinchAmount ("Pinch Amount", Float) = 0
    }

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
        
        // Main pass. Draws a subtle fresnel/spotlight onto the
        // hands, using a distance-invariant "view dir".
        Pass {
            ZWrite On
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

            uniform float _HandThickness;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _HandThickness);
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

            uniform float _IlluminationAmount;
            uniform float _IlluminationExponent;
            uniform float4 _HandColor;

            fixed4 frag(v2f i) : SV_Target
            {
                half rim = 1.0 - abs(dot(i.viewDir, i.normal));
                half spotlight = 1.0 - rim;

                // Blend base color with the illumination/spotlight.
                float4 hand = _HandColor + pow(spotlight, _IlluminationExponent) * _IlluminationAmount;
                return hand * float4(1,1,1,i.color.r);
            }

            ENDCG
        }
        
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
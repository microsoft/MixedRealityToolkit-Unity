// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//                              Important note:
//
// This is the second pass of a three-pass shader ("TransparentOutlinedHand-XXXX").
// Unfortunately, this shader is split into three shaders (and three materials),
// as URP does not support multi-pass shaders. In order to support both built-in
// pipelines and URP, we split the multi-pass shader into three shaders and use
// material chaining on the hand mesh. Using this shader alone is not recommended.

Shader "Mixed Reality Toolkit/Transparent Outlined Hand (Main)" {
    Properties {
        _HandColor ("Hand Color", Color) = (1,1,1,1)
        _HandThickness ("_HandThickness", Range(-0.0001,0.0001)) = 0.0
        _IlluminationExponent ("Illumination Exponent", Range(0,10)) = 1
        _IlluminationAmount ("Illumination Amount", Range(0,10)) = 1
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200
        
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
    }
    FallBack "Diffuse"
}
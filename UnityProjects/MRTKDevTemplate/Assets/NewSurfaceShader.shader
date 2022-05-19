Shader "Custom/PreZ Standard Fade" {
    Properties {
        _HandColor ("Hand Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _OutlineThreshold ("_OutlineThreshold", Range(0.0,1)) = 0.95
        _OutlineSmoothing ("_OutlineSmoothing", Range(0,0.1)) = 0.05
        _OutlineExponent ("_OutlineExponent", Range(0,10)) = 1
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
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag() : SV_Target { return 0; }

            ENDCG
        }

        // Actually draw hands
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
                float4 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : POSITION;
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
                o.viewDir = UnityWorldSpaceViewDir(mul(UNITY_MATRIX_M, v.vertex));

                return o;
            }

            uniform float _OutlineThreshold;
            uniform float _OutlineSmoothing;
            uniform float _OutlineExponent;

            uniform float4 _HandColor;
            uniform float4 _OutlineColor;

            fixed4 frag(v2f i) : SV_Target
            {
                half rim = 1.0 - abs(dot(i.viewDir, i.normal));

                rim = pow(rim, _OutlineExponent);

                half amt = smoothstep(_OutlineThreshold, _OutlineThreshold + _OutlineSmoothing, rim);
                
                return lerp(_HandColor, _OutlineColor, amt);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
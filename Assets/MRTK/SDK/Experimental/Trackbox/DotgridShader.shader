Shader "MRTK/DotgridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Multiplier("Glow Distance Multiplier", Float) = 10
        _BoxSize("Glow Box Size", Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha One
        Cull Off
        Colormask RGB

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _GlowPos;
            float _Multiplier;
            uniform float4x4 _GlowTransform;

            #define PROXIMITY_LIGHT_COUNT 2
            #define PROXIMITY_LIGHT_DATA_SIZE 6
            float4 _ProximityLightData[PROXIMITY_LIGHT_COUNT * PROXIMITY_LIGHT_DATA_SIZE];

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);

                float proximityGlow = 0;

                [unroll]
                for (int proximityLightIndex = 0; proximityLightIndex < PROXIMITY_LIGHT_COUNT; ++proximityLightIndex)
                {
                    int dataIndex = proximityLightIndex * PROXIMITY_LIGHT_DATA_SIZE;
                    proximityGlow += (1.0f - clamp(length(i.worldPos - _ProximityLightData[dataIndex].xyz) * _Multiplier,0,1));
                }

                return tex * proximityGlow;
            }
            ENDCG
        }
    }
}

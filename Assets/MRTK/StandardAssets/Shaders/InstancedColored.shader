// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// NOTE: MRTK Shaders are versioned via the MRTK.Shaders.sentinel file.
// When making changes to any shader's source file, the value in the sentinel _must_ be incremented.

Shader "Hidden/Instanced-Colored"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _ZWrite("ZWrite", Int) = 1.0 // On
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Int) = 4.0 // LEqual
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Int) = 0.0 // Off
    }

    SubShader
    {
        Pass
        {
            Name "Main"
            Tags{ "RenderType" = "Opaque" }
            ZWrite[_ZWrite]
            ZTest[_ZTest]
            Cull[_Cull]

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata_t
            {
                fixed4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4x4 _ParentLocalToWorldMatrix;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = mul(UNITY_MATRIX_VP, mul(_ParentLocalToWorldMatrix, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0))));
                o.color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }
    }
}

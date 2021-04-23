// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// NOTE: MRTK Shaders are versioned via the MRTK.Shaders.sentinel file.
// When making changes to any shader's source file, the value in the sentinel _must_ be incremented.

///
/// Basic 3D TextMesh shader with proper z-sorting and culling options.
///
Shader "Mixed Reality Toolkit/Text3DShader"
{
    Properties
    {
        _MainTex("Alpha (A)", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 0

        [HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
    }

    SubShader
    {
        LOD 200

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull[_Cull]
        Lighting Off
        ZWrite On
        ZTest[unity_GUIZTestMode]
        Offset -1, -1
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
                
            #pragma multi_compile_instancing

            #pragma multi_compile __ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX                

            #if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
                #define _CLIPPING_PRIMITIVE
            #else
                #undef _CLIPPING_PRIMITIVE
            #endif

            #include "UnityCG.cginc"
            #include "MixedRealityShaderUtils.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO

            #if defined(_CLIPPING_PRIMITIVE)
                float3 worldPosition    : TEXCOORD5;
            #endif
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        
        #if defined(_CLIPPING_PLANE)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _ClipPlaneSide)
            UNITY_DEFINE_INSTANCED_PROP(float4, _ClipPlane)
        #endif

        #if defined(_CLIPPING_SPHERE)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _ClipSphereSide)
            UNITY_DEFINE_INSTANCED_PROP(float4x4, _ClipSphereInverseTransform)
        #endif

        #if defined(_CLIPPING_BOX)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _ClipBoxSide)
            UNITY_DEFINE_INSTANCED_PROP(float4x4, _ClipBoxInverseTransform)
        #endif

            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata_t v)
            {
                v2f o;
                    
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

            #ifdef UNITY_HALF_TEXEL_OFFSET
                o.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
            #endif

            #if defined(_CLIPPING_PRIMITIVE)
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
            #endif

                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 col = i.color;
                col.a *= tex2D(_MainTex, i.texcoord).a;
                col = col * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                    
                // Primitive clipping.
        #if defined(_CLIPPING_PRIMITIVE)
                float primitiveDistance = 1.0;
        #if defined(_CLIPPING_PLANE)
                fixed clipPlaneSide = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipPlaneSide);
                float4 clipPlane = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipPlane);
                primitiveDistance = min(primitiveDistance, PointVsPlane(i.worldPosition.xyz, clipPlane) * clipPlaneSide);
        #endif
        #if defined(_CLIPPING_SPHERE)
                fixed clipSphereSide = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipSphereSide);
                float4x4 clipSphereInverseTransform = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipSphereInverseTransform);
                primitiveDistance = min(primitiveDistance, PointVsSphere(i.worldPosition.xyz, clipSphereInverseTransform) * clipSphereSide);
        #endif
        #if defined(_CLIPPING_BOX)
                fixed clipBoxSide = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipBoxSide);
                float4x4 clipBoxInverseTransform = UNITY_ACCESS_INSTANCED_PROP(Props, _ClipBoxInverseTransform);
                primitiveDistance = min(primitiveDistance, PointVsBox(i.worldPosition.xyz, clipBoxInverseTransform) * clipBoxSide);
        #endif
                col *= step(0.0, primitiveDistance);
        #endif

                clip(col.a - 0.01);
                    
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "Microsoft.MixedReality.Toolkit.Editor.Text3DShaderGUI"
}

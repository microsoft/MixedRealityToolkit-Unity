//
// Copyright (C) Microsoft. All rights reserved. 
//

Shader "HoloToolkit/SpecularHighlight"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _HighlightColor("Highlight Color", Color) = (1,1,1,1)
        _MainTex1("Albedo", 2D) = "white" { }
        _Specular("Specular", Color) = (0.08,0.075,0.065,1)
        _Shininess("Shininess", float) = 3.0
        _Highlight("Highlight", float) = 0.0
    }

        CGINCLUDE
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "Lighting.cginc"
            ENDCG

            SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
            Pass
            {
                Lighting On
                Tags {"LightMode" = "ForwardBase"}

                CGPROGRAM

                #pragma exclude_renderers gles
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase 
                #pragma target 5.0
                #pragma only_renderers d3d11

            // Low precision floating point types
            #define lowp min16float
            #define lowp2 min16float2
            #define lowp3 min16float3
            #define lowp4 min16float4
            #define WORLD_NORMAL(vertexNormal) lowp4((normalize((lowp3)mul(vertexNormal, (float3x3)unity_WorldToObject))), 1.0f)

            #define SPECULAR_ON 1
            #define REFLECTION_ON 0

            float3 _Color;
            float3 _HighlightColor;
            sampler2D _MainTex1;
            float4 _Specular;
            float _Shininess;
            float _Highlight;

            sampler2D _SimpleShadow;
            uniform float4x4 _World2SimpleShadow;

            float _CubeContribution;
            float _FresnelPower;
            samplerCUBE _Cube;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                lowp4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : POSITION;
                lowp2 uv : TEXCOORD0;
                lowp3 color : COLOR;
                lowp3 diffuse : TEXCOORD2;

#if SPECULAR_ON
                lowp3 specular : TEXCOORD4;
#endif

#if REFLECTION_ON
                lowp3 worldViewDir : TEXCOORD5;
                lowp3 worldNormal : TEXCOORD6;
                lowp3 reflection : TEXCOORD7;
#endif

            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                //Setup vectors for light probe contribution, w = 1.
                lowp4 worldNormal = WORLD_NORMAL(v.normal);

                //Calculate light probe contribution
                lowp3 x1;
                // Linear + constant polynomial terms
                x1.r = dot(unity_SHAr, worldNormal);
                x1.g = dot(unity_SHAg, worldNormal);
                x1.b = dot(unity_SHAb, worldNormal);

                //transfering color to pixel shader for use in diffuse output
                o.color = v.color * _Color;

                lowp nDotL = saturate(dot((lowp3)_WorldSpaceLightPos0.xyz, worldNormal.xyz));
                lowp3 halfLightColor = _LightColor0.rgb * (lowp).5f;

                // Store half the color of the light as diffuse and half as ambient
                o.diffuse = halfLightColor * nDotL;

#if SPECULAR_ON || REFLECTION_ON
                lowp3 worldViewDir = normalize(WorldSpaceViewDir(v.vertex));
#endif

#if SPECULAR_ON
                // Calculate specular
                // TODO WorldLightDir is constant each frame. Normalize this on the CPU instead.
                lowp3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
                lowp3 halfVector = normalize(worldLightDir + worldViewDir);

                lowp3 specNormal = normalize(worldNormal);
                lowp specDot = saturate(dot(specNormal, halfVector));

                // Pre-adding specular into ambient so we don't
                // have to do it in the fragment shader
                o.specular = _Specular * saturate(pow(specDot, _Shininess)) * 5;
#endif

#if REFLECTION_ON
                o.worldNormal = worldNormal;
                o.worldViewDir = worldViewDir;
                o.reflection = reflect(-worldViewDir, worldNormal.xyz * 1.0);
#endif
                return o;
            }


            lowp4 frag(v2f i) : COLOR
            {
                lowp attenuation = 1.0f;
                lowp3 albedo;
                {
                    albedo = (lowp3)tex2D(_MainTex1, i.uv) * i.color;
#if SPECULAR_ON
                    albedo += i.specular;
#endif

#if REFLECTION_ON
                    lowp fresnel = saturate(dot(i.worldNormal, i.worldViewDir));
                    // Try and sneak a MAD in there
                    lowp fresnelRatio = (fresnel * fresnel) * (-(lowp)_FresnelPower) + (lowp)1.0;
                    albedo += texCUBE(_Cube, i.reflection) * _CubeContribution * fresnelRatio;
#endif
                }

                lowp3 irradiance;
                {
                    irradiance = i.diffuse;
                    irradiance = irradiance * attenuation + albedo;
                }

                lowp3 exitantRadiance = albedo * irradiance + _HighlightColor * _Highlight;

                lowp alpha = 1.0f;
                return lowp4(exitantRadiance, alpha);
            }
        ENDCG
        }
        }
            Fallback "VertexLit"
}
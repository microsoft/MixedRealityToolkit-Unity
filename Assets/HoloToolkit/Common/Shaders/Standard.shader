// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

Shader "MixedRealityToolkit/Standard"
{
    Properties
    {
        // Main maps.
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex("Albedo", 2D) = "white" {}
        [Enum(AlbedoAlphaMode)] _AlbedoAlphaMode("Albedo Alpha Mode", Float) = 0 // "Transparency"
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        [Toggle(_NORMAL_MAP)] _EnableNormalMap("Enable Normal Map", Float) = 0.0
        [NoScaleOffset] _NormalMap("Normal Map", 2D) = "bump" {}
        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
        _EmissiveColor("Emissive Color", Color) = (0.0, 0.0, 0.0, 1.0)

        // Rendering options.
        [Toggle(_DIRECTIONAL_LIGHT)] _DirectionalLight("Directional Light", Float) = 1.0
        [Toggle(_SPECULAR_HIGHLIGHTS)] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [Toggle(_REFLECTIONS)] _Reflections("Reflections", Float) = 0.0
        [Toggle(_REFRACTION)] _Refraction("Refraction", Float) = 0.0
        _RefractiveIndex("Refractive Index", Range(0.0, 3.0)) = 0.0
        [Toggle(_RIM_LIGHT)] _RimLight("Rim Light", Float) = 0.0
        _RimColor("Rim Color", Color) = (0.5, 0.5, 0.5, 1.0)
        _RimPower("Rim Power", Range(0.0, 8.0)) = 0.25
        [Toggle(_CLIPPING_PLANE)] _ClippingPlane("Clipping Plane", Float) = 0.0
        _ClipPlane("Clip Plane", Vector) = (0.0, 1.0, 0.0, 0.0)
        [Toggle(_CLIPPING_PLANE_BORDER)] _ClippingPlaneBorder("Clipping Plane Border", Float) = 0.0
        _ClippingPlaneBorderWidth("Clipping Plane Border Width", Range(0.005, 1.0)) = 0.025
        _ClippingPlaneBorderColor("Clipping Plane Border Color", Color) = (1.0, 0.2, 0.0, 1.0)
        [Toggle(_NEAR_PLANE_FADE)] _NearPlaneFade("Near Plane Fade", Float) = 0.0
        _FadeBeginDistance("Fade Begin Distance", Range(0.01, 10.0)) = 0.85
        _FadeCompleteDistance("Fade Complete Distance", Range(0.01, 10.0)) = 0.5

        // Fluent options.
        [Toggle(_HOVER_LIGHT)] _HoverLight("Hover Light", Float) = 1.0
        [Toggle(_HOVER_COLOR_OVERRIDE)] _EnableHoverColorOverride("Hover Color Override", Float) = 0.0
        _HoverColorOverride("Hover Color Override", Color) = (1.0, 1.0, 1.0, 1.0)
        [Toggle(_HOVER_LIGHT_OPAQUE)] _HoverLightOpaque("Hover Light Opaque", Float) = 0.0
        [Toggle(_HOVER_COLOR_OPAQUE_OVERRIDE)] _EnableHoverColorOpaqueOverride("Hover Color Opaque Override", Float) = 0.0
        _HoverColorOpaqueOverride("Hover Color Override for Transparent Pixels", Color) = (1.0, 1.0, 1.0, 1.0)
        [Toggle(_ROUND_CORNERS)] _RoundCorners("Round Corners", Float) = 0.0
        _RoundCornerRadius("Round Corner Radius", Range(0.01, 0.5)) = 0.25
        _RoundCornerMargin("Round Corner Margin", Range(0.0, 0.5)) = 0.0
        [Toggle(_BORDER_LIGHT)] _BorderLight("Border Light", Float) = 0.0
        [Toggle(_BORDER_LIGHT_USES_HOVER_COLOR)] _BorderLightUsesHoverColor("Border Light Uses Hover Color", Float) = 1.0
        [Toggle(_BORDER_LIGHT_OPAQUE)] _BorderLightOpaque("Border Light Opaque", Float) = 0.0
        _BorderWidth("Border Width", Range(0.0, 1.0)) = 0.1
        _BorderMinValue("Border Min Value", Range(0.0, 1.0)) = 0.1
        _EdgeSmoothingValue("Edge Smoothing Value", Range(0.0001, 0.2)) = 0.002
        [Toggle(_INNER_GLOW)] _InnerGlow("Inner Glow", Float) = 0.0
        _InnerGlowColor("Inner Glow Color (RGB) and Intensity (A)", Color) = (1.0, 1.0, 1.0, 0.75)
        [Toggle(_ENVIRONMENT_COLORING)] _EnvironmentColoring("Environment Coloring", Float) = 0.0
        _EnvironmentColorThreshold("Environment Color Threshold", Range(0.0, 3.0)) = 1.5
        _EnvironmentColorIntensity("Environment Color Intensity", Range(0.0, 1.0)) = 0.5
        _EnvironmentColorX("Environment Color X (RGB)", Color) = (1.0, 0.0, 0.0, 1.0)
        _EnvironmentColorY("Environment Color Y (RGB)", Color) = (0.0, 1.0, 0.0, 1.0)
        _EnvironmentColorZ("Environment Color Z (RGB)", Color) = (0.0, 0.0, 1.0, 1.0)

        // Advanced options.
        [Enum(RenderingMode)] _Mode("Rendering Mode", Float) = 0                                     // "Opaque"
        [Enum(CustomRenderingMode)] _CustomMode("Mode", Float) = 0                                   // "Opaque"
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend", Float) = 1                 // "One"
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend", Float) = 0            // "Zero"
        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("Blend Operation", Float) = 0                 // "Add"
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Depth Test", Float) = 4                // "LessEqual"
        [Enum(DepthWrite)] _ZWrite("Depth Write", Float) = 1                                         // "On"
        [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("Color Write Mask", Float) = 15 // "All"
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2                     // "Back"
        _RenderQueueOverride("Render Queue Override", Range(-1.0, 5000)) = -1
    }

    SubShader
    {
        Pass
        {
            Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" "PerformanceChecks" = "False" }
            LOD 100
            Blend[_SrcBlend][_DstBlend]
            BlendOp[_BlendOp]
            ZTest[_ZTest]
            ZWrite[_ZWrite]
            Cull[_CullMode]
            ColorMask[_ColorWriteMask]

            CGPROGRAM

            #pragma target 5.0
            #pragma only_renderers d3d11
            #pragma multi_compile_instancing
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON
            #pragma shader_feature _DISABLE_ALBEDO_MAP
            #pragma shader_feature _ _METALLIC_TEXTURE_ALBEDO_CHANNEL_A _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _NORMAL_MAP
            #pragma shader_feature _EMISSION
            #pragma shader_feature _DIRECTIONAL_LIGHT
            #pragma shader_feature _SPECULAR_HIGHLIGHTS
            #pragma shader_feature _REFLECTIONS
            #pragma shader_feature _REFRACTION
            #pragma shader_feature _RIM_LIGHT
            #pragma shader_feature _CLIPPING_PLANE
            #pragma shader_feature _CLIPPING_PLANE_BORDER
            #pragma shader_feature _NEAR_PLANE_FADE
            #pragma shader_feature _HOVER_LIGHT
            #pragma shader_feature _HOVER_COLOR_OVERRIDE
            #pragma shader_feature _HOVER_LIGHT_OPAQUE
            #pragma shader_feature _HOVER_COLOR_OPAQUE_OVERRIDE
            #pragma shader_feature _ROUND_CORNERS
            #pragma shader_feature _BORDER_LIGHT
            #pragma shader_feature _BORDER_LIGHT_USES_HOVER_COLOR
            #pragma shader_feature _BORDER_LIGHT_OPAQUE
            #pragma shader_feature _INNER_GLOW
            #pragma shader_feature _ENVIRONMENT_COLORING

            #include "UnityCG.cginc"
            #include "UnityStandardConfig.cginc"

#if defined(_DIRECTIONAL_LIGHT) || defined(_REFLECTIONS) || defined(_RIM_LIGHT) || defined(_ENVIRONMENT_COLORING)
            #define _NORMAL
#else
            #undef _NORMAL
#endif

#if defined(_NORMAL) || defined(_CLIPPING_PLANE) || defined(_NEAR_PLANE_FADE) || defined(_HOVER_LIGHT)
            #define _WORLD_POSITION
#else
            #undef _WORLD_POSITION
#endif

#if defined(_ALPHATEST_ON) || defined(_CLIPPING_PLANE) || defined(_ROUND_CORNERS)
            #define _ALPHA_CLIP
#else
            #undef _ALPHA_CLIP
#endif

#if defined(_ALPHABLEND_ON)
            #define _TRANSPARENT
            #undef _ALPHA_CLIP
#else
            #undef _TRANSPARENT
#endif

#if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT)
            #define _SCALE
#else
            #undef _SCALE
#endif

#if defined(_DIRECTIONAL_LIGHT) || defined(_RIM_LIGHT)
            #define _FRESNEL
#else
            #undef _FRESNEL
#endif

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
#if defined(LIGHTMAP_ON)
                float2 lightMapUV : TEXCOORD1;
#endif
                fixed3 normal : NORMAL;
#if defined(_NORMAL_MAP)
                fixed4 tangent : TANGENT;
#endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f 
            {
                float4 position : SV_POSITION;
#if defined(_BORDER_LIGHT)
                float4 uv : TEXCOORD0;
#else
                float2 uv : TEXCOORD0;
#endif
#if defined(LIGHTMAP_ON)
                float2 lightMapUV : TEXCOORD1;
#endif
#if defined(_WORLD_POSITION)
#if defined(_NEAR_PLANE_FADE)
                float4 worldPosition : TEXCOORD2;
#else
                float3 worldPosition : TEXCOORD2;
#endif
#endif
#if defined(_SCALE)
                float3 scale : TEXCOORD3;
#endif
#if defined(_NORMAL)
#if defined(_NORMAL_MAP)
                fixed3 tangentX : TEXCOORD4;
                fixed3 tangentY : TEXCOORD5;
                fixed3 tangentZ : TEXCOORD6;
#else
                fixed3 worldNormal : TEXCOORD4;
#endif
#endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            sampler2D _MainTex;
            fixed4 _MainTex_ST;

#if defined(_ALPHA_CLIP)
            fixed _Cutoff;
#endif

            fixed _Metallic;
            fixed _Smoothness;

#if defined(_NORMAL_MAP)
            sampler2D _NormalMap;
#endif

#if defined(_EMISSION)
            fixed4 _EmissiveColor;
#endif

#if defined(_DIRECTIONAL_LIGHT)
            fixed4 _LightColor0;
#endif

#if defined(_REFRACTION)
            fixed _RefractiveIndex;
#endif

#if defined(_RIM_LIGHT)
            fixed3 _RimColor;
            fixed _RimPower;
#endif

#if defined(_CLIPPING_PLANE)
            float4 _ClipPlane;
            fixed _ClippingPlaneBorderWidth;
            fixed3 _ClippingPlaneBorderColor;
#endif

#if defined(_NEAR_PLANE_FADE)
            float _FadeBeginDistance;
            float _FadeCompleteDistance;
#endif

#if defined(_HOVER_LIGHT)
            float3 _HoverPosition;
            float _HoverRadius;
            fixed4 _HoverColor;
#if defined(_HOVER_COLOR_OVERRIDE)
            fixed3 _HoverColorOverride;
#endif
#if defined(_HOVER_COLOR_OPAQUE_OVERRIDE)
            fixed3 _HoverColorOpaqueOverride;
#endif
#endif

#if defined(_ROUND_CORNERS)
            fixed _RoundCornerRadius;
            fixed _RoundCornerMargin;
#endif

#if defined(_BORDER_LIGHT)
            fixed _BorderWidth;
            fixed _BorderMinValue;
#endif

#if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT)
            fixed _EdgeSmoothingValue;
#endif

#if defined(_INNER_GLOW)
            fixed4 _InnerGlowColor;
#endif

#if defined(_ENVIRONMENT_COLORING)
            fixed _EnvironmentColorThreshold;
            fixed _EnvironmentColorIntensity;
            fixed3 _EnvironmentColorX;
            fixed3 _EnvironmentColorY;
            fixed3 _EnvironmentColorZ;
#endif

#if defined(_SPECULAR_HIGHLIGHTS)
            static const fixed _Shininess = 800.0;
#endif

#if defined(_FRESNEL)
            static const fixed _FresnelPower = 4.0;
            static const fixed _FresnelPowerInverse = 1.0 / _FresnelPower;
#endif

#if defined(_BORDER_LIGHT)
            static const fixed _BorderPower = 10.0;
            static const fixed _InverseBorderPower = 1.0 / _BorderPower;
#endif

#if defined(_CLIPPING_PLANE)
            inline float PointVsPlane(float3 worldPosition, float4 plane)
            {
                float3 planePosition = plane.xyz * plane.w;
                return dot(worldPosition - planePosition, plane.xyz);
            }
#endif

#if defined(_ROUND_CORNERS)
            inline fixed RoundCorners(fixed2 position, fixed2 cornerCircleDistance, fixed cornerCircleRadius)
            {
                fixed distance = length(max(abs(position) - cornerCircleDistance, 0.0)) - cornerCircleRadius;

#if defined(_TRANSPARENT)
                return smoothstep(1.0, 0.0, distance / _EdgeSmoothingValue);
#else
                return step(distance, 0.0);
#endif
            }
#endif
            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.position = UnityObjectToClipPos(v.vertex);

#if defined(_WORLD_POSITION)
                o.worldPosition.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
#endif

#if defined(_NEAR_PLANE_FADE)
                float rangeInverse = 1.0 / (_FadeBeginDistance - _FadeCompleteDistance);
                float distanceToCamera = -UnityObjectToViewPos(v.vertex.xyz).z;
                o.worldPosition.w = saturate(mad(distanceToCamera, rangeInverse, -_FadeCompleteDistance * rangeInverse));
#endif

#if defined(_SCALE)
                o.scale.x = length(mul(unity_ObjectToWorld, float4(1.0, 0.0, 0.0, 0.0)));
                o.scale.y = length(mul(unity_ObjectToWorld, float4(0.0, 1.0, 0.0, 0.0)));
                o.scale.z = length(mul(unity_ObjectToWorld, float4(0.0, 0.0, 1.0, 0.0)));
#endif

#if defined(_BORDER_LIGHT)
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
   
                float minScale = min(min(o.scale.x, o.scale.y), o.scale.z);
                float maxScale = max(max(o.scale.x, o.scale.y), o.scale.z);
                float minOverMiddleScale = minScale / (o.scale.x + o.scale.y + o.scale.z - minScale - maxScale);

                float areaYZ = o.scale.y * o.scale.z;
                float areaXZ = o.scale.z * o.scale.x;
                float areaXY = o.scale.x * o.scale.y;
                float borderWidth = _BorderWidth;

                if (abs(v.normal.x) == 1.0) // Y,Z plane.
                {
                    o.scale.x = o.scale.z;
                    o.scale.y = o.scale.y;

                    if (areaYZ > areaXZ && areaYZ > areaXY)
                    {
                        borderWidth *= minOverMiddleScale;
                    }
                }
                else if (abs(v.normal.y) == 1.0) // X,Z plane.
                {
                    o.scale.x = o.scale.x;
                    o.scale.y = o.scale.z;

                    if (areaXZ > areaXY && areaXZ > areaYZ)
                    {
                        borderWidth *= minOverMiddleScale;
                    }
                }
                else  // X,Y plane.
                {
                    o.scale.x = o.scale.x;
                    o.scale.y = o.scale.y;

                    if (areaXY > areaYZ && areaXY > areaXZ)
                    {
                        borderWidth *= minOverMiddleScale;
                    }
                }

                o.scale.z = minScale;
                float scaleRatio = min(o.scale.x, o.scale.y) / max(o.scale.x, o.scale.y);

                if (o.scale.x > o.scale.y)
                {
                    o.uv.z = 1.0 - (borderWidth * scaleRatio);
                    o.uv.w = 1.0 - borderWidth;
                }
                else
                {
                    o.uv.z = 1.0 - borderWidth;
                    o.uv.w = 1.0 - (borderWidth * scaleRatio);
                }
#else
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#endif


#if defined(LIGHTMAP_ON)
                o.lightMapUV.xy = v.lightMapUV.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

#if defined(_NORMAL)
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);

#if defined(_NORMAL_MAP)
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                fixed3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
                o.tangentX = fixed3(worldTangent.x, worldBitangent.x, worldNormal.x);
                o.tangentY = fixed3(worldTangent.y, worldBitangent.y, worldNormal.y);
                o.tangentZ = fixed3(worldTangent.z, worldBitangent.z, worldNormal.z);
#else
                o.worldNormal = worldNormal;
#endif
#endif

                return o;
            }

#if !defined(_ALPHA_CLIP) && !defined(_TRANSPARENT)
            [earlydepthstencil]
#endif
            fixed4 frag(v2f i) : SV_Target
            {
                // Texturing.
#if defined(_DISABLE_ALBEDO_MAP)
                fixed4 albedo = fixed4(1.0, 1.0, 1.0, 1.0);
#else
                fixed4 albedo = tex2D(_MainTex, i.uv);
#endif

#ifdef LIGHTMAP_ON
                albedo.rgb *= DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lightMapUV));
#endif

#if defined(_METALLIC_TEXTURE_ALBEDO_CHANNEL_A)
                _Metallic = albedo.a;
                albedo.a = 1.0;
#elif defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
                _Smoothness = albedo.a;
                albedo.a = 1.0;
#endif 

                // Plane clipping.
#if defined(_CLIPPING_PLANE)
                float planeDistance = PointVsPlane(i.worldPosition.xyz, _ClipPlane);
#if defined(_CLIPPING_PLANE_BORDER)
                fixed3 planeBorderColor = lerp(_ClippingPlaneBorderColor, fixed3(0.0, 0.0, 0.0), planeDistance / _ClippingPlaneBorderWidth);
                albedo.rgb += step(planeDistance, _ClippingPlaneBorderWidth) * planeBorderColor;
#endif
#if defined(_ALPHA_CLIP)
                albedo *= step(0.0, planeDistance);
#else
                albedo *= saturate(planeDistance);
#endif
#endif

#if defined(_ROUND_CORNERS) || defined(_BORDER_LIGHT) || defined(_INNER_GLOW)
                fixed2 distanceToEdge;
                distanceToEdge.x = abs(i.uv.x - 0.5) * 2.0;
                distanceToEdge.y = abs(i.uv.y - 0.5) * 2.0;
#endif

                // Rounded corner clipping.
#if defined(_ROUND_CORNERS)
                fixed cornerCircleRadius = (_RoundCornerRadius - _RoundCornerMargin) * i.scale.z;
                fixed2 roundCornerPosition = distanceToEdge * 0.5 * i.scale.xy;
                fixed2 cornerCircleDistance = (i.scale.xy * 0.5) - cornerCircleRadius - _RoundCornerMargin * i.scale.xy;
                fixed roundCornerClip = RoundCorners(roundCornerPosition, cornerCircleDistance, cornerCircleRadius);
#endif

                albedo *= _Color;

                // Hover light.
#if defined(_HOVER_LIGHT)
                fixed pointToHover = (1.0 - saturate(length(_HoverPosition - i.worldPosition.xyz) / _HoverRadius)) * _HoverColor.a;
#if defined(_HOVER_COLOR_OVERRIDE)
                _HoverColor.rgb = _HoverColorOverride.rgb;
#endif
#if defined(_HOVER_LIGHT_OPAQUE)
#if defined(_HOVER_COLOR_OPAQUE_OVERRIDE)
                _HoverColor.rgb = lerp(_HoverColorOpaqueOverride, _HoverColor.rgb, albedo.a);
#endif
                fixed baseBlend = 1.0 + (albedo.a - 1.0) * saturate(pointToHover / (pointToHover + albedo.a));
                albedo.rgb += -(1.0 - baseBlend) * albedo.rgb + _HoverColor.rgb * max(pointToHover, 1.0 - baseBlend);
                albedo.a = (albedo.a + pointToHover);
#else
                albedo.rgb = saturate(albedo.rgb + _HoverColor.rgb * pointToHover);
#endif
#endif

                // Border light.
#if defined(_BORDER_LIGHT)
                fixed3 borderColor = albedo.rgb * _BorderPower;
#if defined(_HOVER_LIGHT)
#if defined(_BORDER_LIGHT_USES_HOVER_COLOR)
                borderColor *= _HoverColor.rgb;
#endif
#else
                fixed pointToHover = 1.0;
#endif
                fixed borderValue;
#if defined(_ROUND_CORNERS)
                borderValue = 1.0 - RoundCorners(roundCornerPosition, cornerCircleDistance, cornerCircleRadius * (1.0 - (_BorderWidth * 2.0)));
#else
                borderValue = max(smoothstep(i.uv.z - _EdgeSmoothingValue, i.uv.z + _EdgeSmoothingValue, distanceToEdge.x),
                                  smoothstep(i.uv.w - _EdgeSmoothingValue, i.uv.w + _EdgeSmoothingValue, distanceToEdge.y));
#endif
                borderColor = borderColor * borderValue * max(_BorderMinValue * _InverseBorderPower, pointToHover);
                albedo.rgb += borderColor;
#if defined(_BORDER_LIGHT_OPAQUE)
                albedo.a = max(albedo.a, borderValue);
#endif           
#endif

#if defined(_ROUND_CORNERS)
                albedo *= roundCornerClip;
#endif

#if defined(_ALPHA_CLIP)
#if !defined(_ALPHATEST_ON)
                _Cutoff = 0.5;
#endif
                clip(albedo.a - _Cutoff);
#endif

#if defined(_NORMAL)
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPosition.xyz));
#if defined(_REFLECTIONS) || defined(_ENVIRONMENT_COLORING)
                fixed3 incident = -worldViewDir;
#endif
                fixed3 worldNormal;

                // Normal calculation.
#if defined(_NORMAL_MAP)
                fixed3 tangentNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                worldNormal.x = dot(i.tangentX, tangentNormal);
                worldNormal.y = dot(i.tangentY, tangentNormal);
                worldNormal.z = dot(i.tangentZ, tangentNormal);
                worldNormal = normalize(worldNormal);
#else
                worldNormal = normalize(i.worldNormal);
#endif
#endif

                // Blinn phong lighting.
#if defined(_DIRECTIONAL_LIGHT)
                fixed diffuse = max(0.0, dot(worldNormal, _WorldSpaceLightPos0));

#if defined(_SPECULAR_HIGHLIGHTS)
                fixed halfVector = max(0.0, dot(worldNormal, normalize(_WorldSpaceLightPos0 + worldViewDir)));
                fixed specular = saturate(pow(halfVector, _Shininess * pow(_Smoothness, 4)) * _Smoothness);
#else
                fixed specular = 0.0;
#endif
#endif

                // Image based lighting (attempt to mimic the Standard shader).
#if defined(_REFLECTIONS)
                fixed3 worldReflection = reflect(incident, worldNormal);
                fixed4 iblData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldReflection, (1.0 - _Smoothness) * UNITY_SPECCUBE_LOD_STEPS);
                fixed3 ibl = DecodeHDR(iblData, unity_SpecCube0_HDR);
#if defined(_REFRACTION)
                fixed4 refractColor = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, refract(incident, worldNormal, _RefractiveIndex));
                ibl *= DecodeHDR(refractColor, unity_SpecCube0_HDR);
#endif 
#endif

                // Fresnel lighting.
#if defined(_FRESNEL)
                fixed fresnel = 1.0 - saturate(dot(worldViewDir, worldNormal));
#if defined(_RIM_LIGHT)
                fixed3 fresnelColor = _RimColor * pow(fresnel, _RimPower);
#else
                fixed3 fresnelColor = unity_AmbientSky.rgb * _FresnelPowerInverse * pow(fresnel, _FresnelPower);
#endif
#endif
                // Final lighting mix.
                fixed4 output = albedo;

#if defined(_REFLECTIONS) || defined(_DIRECTIONAL_LIGHT)
                fixed minProperty = min(_Smoothness, _Metallic);
#endif

#if defined(_REFLECTIONS)
                output.rgb += ibl * min((1.0 - _Metallic), 0.5);
                output.rgb = lerp(output.rgb, ibl, minProperty);
#endif

#if defined(_DIRECTIONAL_LIGHT)
                output.rgb *= lerp(unity_AmbientSky.rgb * 1.5 + (albedo.rgb *_LightColor0.rgb * diffuse + _LightColor0.rgb * specular), albedo, minProperty);
                output.rgb += (_LightColor0.rgb * albedo * specular) + (_LightColor0.rgb * specular * _Smoothness);
#endif

#if defined(_FRESNEL)
#if defined(_RIM_LIGHT)
                output.rgb += fresnelColor;
#else
                output.rgb += fresnelColor * (1 - minProperty);
#endif
#endif

#if defined(_EMISSION)
                output.rgb += _EmissiveColor;
#endif

                // Inner glow.
#if defined(_INNER_GLOW)
                fixed2 uvGlow = (i.uv - fixed2(0.5, 0.5)) * (_InnerGlowColor.a * 2.0);
                uvGlow = uvGlow * uvGlow;
                uvGlow = uvGlow * uvGlow;
                output.rgb += lerp(fixed3(0.0, 0.0, 0.0), _InnerGlowColor.rgb, uvGlow.x + uvGlow.y);
#endif

                // Environment coloring.
#if defined(_ENVIRONMENT_COLORING)
                fixed3 environmentColor = incident.x * incident.x * _EnvironmentColorX +
                                          incident.y * incident.y * _EnvironmentColorY + 
                                          incident.z * incident.z * _EnvironmentColorZ;
                output.rgb += environmentColor * max(0.0, dot(incident, worldNormal) + _EnvironmentColorThreshold) * _EnvironmentColorIntensity;

#endif

#if defined(_NEAR_PLANE_FADE)
                output *= i.worldPosition.w;
#endif

                return output;
            }

            ENDCG
        }
    }
    
    FallBack "VertexLit"
    CustomEditor "HoloToolkit.Unity.StandardShaderGUI"
}

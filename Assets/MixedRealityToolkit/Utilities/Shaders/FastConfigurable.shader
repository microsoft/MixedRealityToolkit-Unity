// Very fast shader that uses the Unity light system.
// Compiles down to only performing the operations you're actually using.
// Uses material property drawers rather than a custom editor for ease of maintenance.

// Textured ambient+diffuse:
// Stats for Vertex shader:
//        d3d11: 24 avg math (9..44)
// Stats for Fragment shader:
//        d3d11: 2 avg math (1..5), 0 avg texture (0..2)

Shader "HoloToolkit/Fast Configurable"
{
    Properties
    {
        [Header(Base Texture and Color)]
        [Indent]
            [Toggle] _UseVertexColor("Vertex Color Enabled?", Float) = 0
            [Toggle] _UseMainColor("Main Color Enabled?", Float) = 0
            _Color("Main Color", Color) = (1,1,1,1)		
            [Toggle] _UseMainTex("Main Texture Enabled?", Float) = 0
            [NoScaleOffset]_MainTex("Main Texture", 2D) = "red" {}
            [Toggle] _UseOcclusionMap("Occlusion/Detail Texture Enabled?", Float) = 0
            [NoScaleOffset]_OcclusionMap("Occlusion/Detail Texture", 2D) = "blue" {}
        [Dedent]
       
        [Space(12)]
        [Header(Lighting)]
        [Indent]
            [Toggle] _UseAmbient("Ambient Lighting Enabled?", Float) = 1
            [Toggle] _UseDiffuse("Diffuse Lighting Enabled?", Float) = 1
            [Toggle] _UseSpecular("Specular Lighting Enabled?", Float) = 0
            [Indent]
                _SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
                [PowerSlider(5.0)]_Specular("Specular (Specular Power)", Range(1.0, 100.0)) = 10.0
                _Gloss("Gloss (Specular Scale)", Range(0.1, 10.0)) = 1.0
				[Toggle] _UseGlossMap("Use Gloss Map? (per-pixel)", Float) = 0
				[NoScaleOffset]_MetallicGlossMap("Gloss Map", 2D) = "white" {}
            [Dedent]
            [Toggle] _Shade4("Use additional lighting data? (Expensive!)", Float) = 0
            [Toggle] _UseBumpMap("Normal Map Enabled? (per-pixel)", Float) = 0
            [NoScaleOffset][Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        [Dedent]

        [Space(12)]
        [Header(Emission)]
        [Indent]
            [Toggle] _UseEmissionColor("Emission Color Enabled?", Float) = 0
            _EmissionColor("Emission Color", Color) = (1,1,1,1)
            [Toggle] _UseEmissionTex("Emission Texture Enabled?", Float) = 0
            [NoScaleOffset] _EmissionTex("Emission Texture", 2D) = "blue" {}
        [Dedent]

        [Space(12]
        [Header(Texture Scale and Offset)]
        [Indent]
            [Toggle(_MainTex_SCALE_ON)] _MainTex_SCALE("Use Texture Scale? (Applies to all textures)", Float) = 0
            [Toggle(_MainTex_OFFSET_ON)] _MainTex_OFFSET("Use Texture Offset? (Applies to all textures)", Float) = 0
            _TextureScaleOffset("Texture Scale (XY) and Offset (ZW)", Vector) = (1, 1, 0, 0)
        [Dedent]

        [Space(12)]
        [Header(Alpha Blending)]
        [Indent]
            [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 1 //"One"
            [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DestBlend", Float) = 0 //"Zero"
            [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0 //"Add"
        [Dedent]

        [Space(12)]
        [Header(Misc.)]
        [Indent]
            [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
            [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4 //"LessEqual"
            [Enum(Off,0,On,1)] _ZWrite("ZWrite", Float) = 1 //"On"
            [Enum(UnityEngine.Rendering.ColorWriteMask)] _ColorWriteMask("ColorWriteMask", Float) = 15 //"All"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100
        Blend[_SrcBlend][_DstBlend]
        BlendOp[_BlendOp]
        ZTest[_ZTest]
        ZWrite[_ZWrite]
        Cull[_Cull]
        ColorMask[_ColorWriteMask]

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //compiles all variants needed by ForwardBase (forward rendering base) pass type. The variants deal with different lightmap types and main directional light having shadows on or off.
            #pragma multi_compile_fwdbase

            //expands to several variants to handle different fog types
            #pragma multi_compile_fog

            //We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            //shader features are only compiled if a material uses them
            #pragma shader_feature _USEMAINCOLOR_ON
            #pragma shader_feature _USEMAINTEX_ON
            #pragma shader_feature _USESOCCLUSIONMAP_ON
            #pragma shader_feature _USEBUMPMAP_ON
            #pragma shader_feature _USEAMBIENT_ON
            #pragma shader_feature _USEDIFFUSE_ON
            #pragma shader_feature _USESPECULAR_ON
			#pragma shader_feature _USEGLOSSMAP_ON
            #pragma shader_feature _SHADE4_ON
            #pragma shader_feature _USEEMISSIONCOLOR_ON
            #pragma shader_feature _USEEMISSIONTEX_ON

            //scale and offset will apply to all
            #pragma shader_feature _MainTex_SCALE_ON
            #pragma shader_feature _MainTex_OFFSET_ON

            //may be set from script so generate both paths
            #pragma multi_compile __ _NEAR_PLANE_FADE_ON
            
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            #include "HoloToolkitCommon.cginc"
            #include "macro.cginc"

			#define USE_PER_PIXEL (_USEBUMPMAP_ON || _USEGLOSSMAP_ON)
			#define PIXEL_SHADER_USES_WORLDPOS  (USE_PER_PIXEL && (_USESPECULAR_ON || _SHADE4_ON))
			#define USES_TEX_XY (_USEMAINTEX_ON || _USEOCCLUSIONMAP_ON || _USEEMISSIONTEX_ON || _USEBUMPMAP_ON || _USEGLOSSMAP_ON)

            #if _USEMAINCOLOR_ON
                float4 _Color;
            #endif	

            #if _USEMAINTEX_ON
                UNITY_DECLARE_TEX2D(_MainTex);
            #endif

            #if _USEMAINTEX_ON
                UNITY_DECLARE_TEX2D(_SecondaryTex);
            #endif
        
            #if _USEBUMPMAP_ON
                UNITY_DECLARE_TEX2D(_BumpMap);
            #endif
           
            #if _USESPECULAR_ON
                float3 _SpecularColor;
                float _Specular;
                float _Gloss;
            #endif

			#if _USEGLOSSMAP_ON
				UNITY_DECLARE_TEX2D(_MetallicGlossMap);
			#endif

            #if _USEEMISSIONCOLOR_ON
                float4 _EmissionColor;
            #endif

            #if _USEEMISSIONTEX_ON
                UNITY_DECLARE_TEX2D(_EmissionTex);
            #endif

            float4 _TextureScaleOffset;

            struct a2v
            {
                float4 vertex : POSITION;

                #if _USEBUMPMAP_ON
                #else
                    float3 normal : NORMAL;
                #endif

                #if _USEVERTEXCOLOR_ON
                    float4 color : COLOR;
                #endif
                #if USES_TEX_XY
                    float2 texcoord : TEXCOORD0;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                
                #if _USEVERTEXCOLOR_ON
                    float4 color : COLOR;
                #endif

				#if _USEBUMPMAP_ON
				#else
					#if _USEGLOSSMAP_ON
						float3 normal : NORMAL;
					#endif
				#endif

                #if USES_TEX_XY || _NEAR_PLANE_FADE_ON
                    float3 texXYFadeZ : TEXCOORD0;
                #endif

                #if LIGHTMAP_ON
                    float2 lmap : TEXCOORD1;
                #else
                    float3 vertexLighting : TEXCOORD1;
                #endif

                #if PIXEL_SHADER_USES_WORLDPOS
                    float3 worldPos: TEXCOORD2;
                #endif

                LIGHTING_COORDS(3, 4)                
                UNITY_FOG_COORDS(5)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(a2v v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);

                #if _USEVERTEXCOLOR_ON
                    #if _USEMAINCOLOR_ON
                        o.color = v.color * _Color;
                    #else
                        o.color = v.color;
                    #endif
                #endif

                #if (_USESPECULAR_ON && USE_PER_PIXEL) || _SHADE4_ON
                    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                #endif

                #if PIXEL_SHADER_USES_WORLDPOS
                    o.worldPos = worldPos;
                #endif

                #if USES_TEX_XY
                    o.texXYFadeZ.xy = TRANSFORM_TEX_MAINTEX(v.texcoord.xy, _TextureScaleOffset);
                #endif

                #if LIGHTMAP_ON
                    o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #else
                    #if USE_PER_PIXEL
                    //no bump maps, do per-vertex lighting
                    #else
                        float3 normalWorld = UnityObjectToWorldNormal(v.normal);
                        #if _USEAMBIENT_ON
                            //grab ambient color from Unity's spherical harmonics					
                            o.vertexLighting += ShadeSH9(float4(normalWorld, 1.0));
                        #endif
                        #if _USEDIFFUSE_ON
                            o.vertexLighting += HoloTKLightingLambertian(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
                        #endif
                        #if _USESPECULAR_ON
                            o.vertexLighting += HoloTKLightingBlinnPhong(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)), _Specular, _Gloss, _SpecularColor);
                        #endif
                        #if _SHADE4_ON
                            //handles point and spot lights
                            o.vertexLighting += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                                                  unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                                                  unity_4LightAtten0, worldPos, normalWorld);
                        #endif
                    #endif
                #endif


				#if _USEBUMPMAP_ON
				#else
					#if _USEGLOSSMAP_ON
						o.normal = v.normal;
					#endif
				#endif
                
                //fade away objects closer to the camera
                #if _NEAR_PLANE_FADE_ON
                    o.texXYFadeZ.z = ComputeNearPlaneFadeLinear(v.vertex);
                #endif

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            float4 frag(v2f IN) : SV_Target
            {
                #if _USEMAINTEX_ON
                    float4 color = UNITY_SAMPLE_TEX2D(_MainTex, IN.texXYFadeZ.xy);
                #else
                    float4 color = 1;
                #endif

                #if _USEOCCLUSIONMAP_ON
                    color *= UNITY_SAMPLE_TEX2D(_OcclusionMap, IN.texXYFadeZ.xy);
                #endif

                #if _USEVERTEXCOLOR_ON
                    color *= IN.color;
                //if vertex color is on, we've already scaled it by the main color if needed in the vertex shader
                #elif _USEMAINCOLOR_ON 
                    color *= _Color;
                #endif

                //light attenuation from shadows cast onto the object
                float lightAttenuation = SHADOW_ATTENUATION(IN);
                float3 lightColorShadowAttenuated = 0;

                #if LIGHTMAP_ON
                    float3 lightmapResult = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.lmap.xy));
                    #ifdef SHADOWS_SCREEN
                        lightColorShadowAttenuated = min(lightmapResult, lightAttenuation * 2);
                    #else
                        lightColorShadowAttenuated = lightmapResult;
                    #endif	
                #else //not using lightmapping
                    #if USE_PER_PIXEL
                        //if a normal map is on, it makes sense to do most calculations per-pixel
                        //unpack can be expensive if normal map is dxt5
						float3 normalObject;
						#if (_USEBUMPMAP_ON)
							normalObject = UnpackNormal(UNITY_SAMPLE_TEX2D(_BumpMap, IN.texXYFadeZ.xy));
						#else
							normalObject = IN.normal;
						#endif
                        float3 normalWorld = UnityObjectToWorldNormal(normalObject);
                        #if _USEAMBIENT_ON
                            //grab ambient color from Unity's spherical harmonics					
                            lightColorShadowAttenuated += ShadeSH9(float4(normalWorld, 1.0));
                        #endif
                        #if _USEDIFFUSE_ON
                            lightColorShadowAttenuated += HoloTKLightingLambertian(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb);
                        #endif
                        #if _USESPECULAR_ON
							float gloss = _Gloss;
							#if _USEGLOSSMAP_ON
								gloss *= UNITY_SAMPLE_TEX2D(_MetallicGlossMap, IN.texXYFadeZ.xy).y;
							#endif
                            lightColorShadowAttenuated += HoloTKLightingBlinnPhong(normalWorld, _WorldSpaceLightPos0.xyz, _LightColor0.rgb, UnityWorldSpaceViewDir(IN.worldPos), _Specular, gloss, _SpecularColor);
                        #endif
                        #if _SHADE4_ON
                            //This handles point and directional lights
                            lightColorShadowAttenuated += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                                                            unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                                                            unity_4LightAtten0, IN.worldPos, normalWorld);
                        #endif
                    #else
                        //no normal map, so vertex lighting is sufficient
                        lightColorShadowAttenuated = IN.vertexLighting;
                    #endif
                    //could save some work here in the 0 case
                    lightColorShadowAttenuated *= lightAttenuation;
                #endif
                
                color.rgb *= lightColorShadowAttenuated;

                #if _USEEMISSIONTEX_ON
                    color.rgb += UNITY_SAMPLE_TEX2D(_EmissionTex, IN.texXYFadeZ.xy);
                #endif

                #if _USEEMISSIONCOLOR_ON
                    color.rgb += _EmissionColor;
                #endif

                #if _NEAR_PLANE_FADE_ON
                    color.rgb *= IN.texXYFadeZ.z;
                #endif

                UNITY_APPLY_FOG(IN.fogCoord, color);
                
                return color;
            }

            ENDCG
        }
    } 
    Fallback "VertexLit" //for shadows
}
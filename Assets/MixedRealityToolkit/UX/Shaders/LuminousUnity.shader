// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Unity port of HoloFX Shell shader
//
// v0.1 Mathew Lamb
// based upon work by Doug Service
//
// This shader implements the surface properties of the Prototype Shader, implemented in
// the Fulcrum render engine. It does so shackled by Unity's lighting engine which limits the
// number and type of lights used in a render pass.
//
// It is a two pass shader, the first pass computes the major Directional light and all additive
// sources (emission, reflection etc), the second pass captures the first 4 spot lights.
//
// All computation based upon lighting is code-identical to the HLSL equivalent.
//
// To Do:
// - implement lights as parameters to the shader and wrap filling in the relevant values with a script
//   run from within Unity

Shader "Custom/MayaHLSL4" {
	Properties {
		_fDiffuseIntensity("Diffuse Intensity", Float) = 1
		_cAlbedo("Albedo Color", Color) = (0.0,0.0,0.0)
		_txAlbedo("Albedo Texture (RGB Texture)", 2D) = "white" {}
		_bEnableHalfLambert("Half Lambert Toggle",Range(0,1)) = 0
		_fHalfLambertExp("Half Lambert Exponent", Float) = 1

		_cSpecular("Specular Color", Color) = (0.0,0.0,0.0)
		_txSpecular("Specular Texture (RGB Texture)", 2D) = "white" {}
		_fSpecularPower("Specular Power", Float) = 1
		_bEnableFresnel("Fresnel Toggle",Range(0,1)) = 0
		_bEnableAlphaFresnel("Alpha Fresnel Toggle",Range(0,1)) = 0
		_fFresnelExp("Fresnel Exponent", Float) = 1
		_fFresnelMin("Fresnel Minimum", Float) = 1
		_cReflection("Reflection Color", Color) = (0.0,0.0,0.0)
		_txReflection("Reflection Texture (RGB Cubemap)", Cube) = "white" {}

		_cEmission("Emission Color", Color) = (0.0,0.0,0.0)
		_txEmission("Emission Texture (RGB Texture)", 2D) = "white" {}

		_cAmbient("Ambient Color", Color) = (0.0,0.0,0.0)
		_txAmbient("Ambient Texture (RGB Cubemap)", Cube) = "white" {}

		_fAlphaScale("Alpha Scale", Float) = 1
		_txAlpha("Alpha Texture (RGB Texture)", 2D) = "white" {}

		_txOcclusion("Occlusion Texture (RGB Texture)", 2D) = "white" {}

		_txNormal("Normal Texture (RGB Texture)", 2D) = "white" { TexGen CubeNormal }
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		// 1 directional light and 4 point lights go with this pass
		// total Unity nastiness
		// all reflective and emissive terms are here as this pass is only evaluated once
		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			Cull Back
			Lighting On
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma only_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#pragma target 5.0

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			uniform float4 _LightColor0;	// color of light source - from Lighting.cginc

			uniform float3 _cEmission;
			uniform float3 _cAmbient;
			uniform float3 _cAlbedo;
			uniform float3 _cSpecular;
			uniform float3 _cReflection;

			uniform float _fDiffuseIntensity;
			uniform float _fSpecularPower;
			uniform float _fHalfLambertExp;
			uniform float _fFresnelMin;
			uniform float _fFresnelExp;
			uniform float _fAlphaScale;

			uniform bool _bEnableFresnel;
			uniform bool _bEnableAlphaFresnel;
			uniform bool _bEnableHalfLambert;

			uniform sampler2D _txEmission;
			uniform samplerCUBE _txAmbient;
			uniform sampler2D _txOcclusion;
			uniform sampler2D _txAlbedo;
			uniform sampler2D _txNormal;
			uniform sampler2D _txSpecular;
			uniform samplerCUBE _txReflection;
			uniform sampler2D _txAlpha;

			struct VsIn {
				float4 vPosition 		: POSITION;
				float4 vTangent	 		: TANGENT;
				float3 vNormal 			: NORMAL;
				float4 vTexCoord0 		: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Vs2Ps {
				float4 vPosition 		: SV_POSITION;
				float3 vWorldPosition	: TEXCOORD0;
				float3 vToEye 			: TEXCOORD1;
				float3 vWorldNormal 	: TEXCOORD2;
				float3 vWorldTangent 	: TEXCOORD3;
				float3 vWorldBitangent 	: TEXCOORD4;
				float2 vTexCoord0 		: TEXCOORD5;
                UNITY_VERTEX_OUTPUT_STEREO
			};

			Vs2Ps vert(in VsIn vsIn)
			{
				Vs2Ps vsOut;
                UNITY_SETUP_INSTANCE_ID(vsIn);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vsOut);

				// vertex from model space to view space and world space
				// normally a geometry shader would come next, in which case we would only go to world space
				vsOut.vPosition = UnityObjectToClipPos(vsIn.vPosition);
				vsOut.vWorldPosition = mul(unity_ObjectToWorld,vsIn.vPosition).xyz;

				vsOut.vToEye = normalize(WorldSpaceViewDir(vsIn.vPosition));

				// tangent space basis vector from model space to view space
				// normally a geometry shader would come next, in which case we would only go to world space
				vsOut.vWorldNormal = normalize(mul((float3x3)unity_ObjectToWorld,vsIn.vNormal));
				vsOut.vWorldTangent = normalize(mul((float3x3)unity_ObjectToWorld,vsIn.vTangent));
				float3 BiTangent = cross(vsIn.vNormal,vsIn.vTangent.xyz) * vsIn.vTangent.w;
				vsOut.vWorldBitangent = normalize(mul((float3x3)unity_ObjectToWorld,BiTangent));

				// pass through the texture coordinates
				vsOut.vTexCoord0 = vsIn.vTexCoord0;

				return vsOut;
			}

			//
			// wrap-around lambertian
			//
			float HalfLambert(float nDotL)
			{
				return saturate((nDotL + _fHalfLambertExp) / ((1 + _fHalfLambertExp) * (1 + _fHalfLambertExp)));
			}

			//
			// this class is the per-light computation of the illuminance
			//
			class Material
			{

				// Material properties used in lighting calculation.
				float  fFresnelMin;
				float  fFresnelExp;
				float  fSpecPower;
				float3 cSpecular;
				float3 cAlbedo;
				float3 cOcclusion;
				float3 cResult;

				void Illuminate(
					in float3 nVertex,     // Iterated world space vertex normal
					in float3 nLight,      // World space surface normal for lighting
					in float3 vToLight,    // Direction to light.
					in float3 vToEye,      // Direction to the eye.
					in float3 cIrradiance  // Light irradiance.
					)
				{
					//
					// this next piece of code is the fulcrum illuminance loop
					//

					float nDotL = dot(nLight,vToLight);	// normal due to the normal map
					float nDotLVert = dot(nVertex,vToLight); // interpolated vertex normal

					// calculate the reflection vector from the light
					float3 vReflect = 2.0f * nDotL * nLight - vToLight;

					// Calculate the diffuse intensity and clamp lighting 
					// on backside normal map normals.
					float fBackClamp = 1.0;
					float fIDiffuse = 0.0;

					if (_bEnableHalfLambert)
					{
						fBackClamp = HalfLambert(nDotLVert);
						fIDiffuse = _fDiffuseIntensity * fBackClamp;
					}
					else
					{
						fBackClamp = step(0, nDotLVert);
						fIDiffuse = fBackClamp * _fDiffuseIntensity * saturate(nDotL);
					}

					// Calculate the Fresnel term for the light.
					float2 vCoeff = float2(1, 1);
					if (_bEnableFresnel)
					{
						float fReflect = _fFresnelMin + (1.0 - _fFresnelMin) * pow(1.0 - fIDiffuse, _fFresnelExp);
						vCoeff = float2(1 - fReflect, fReflect);
					}

					// Calculate the specular intensity.
					float fISpecular = fBackClamp* pow(saturate(dot(vReflect, vToEye)), _fSpecularPower);

					//
					// bring the two color computations together
					//
					cResult = cOcclusion * cIrradiance * (vCoeff.x * fIDiffuse * cAlbedo + vCoeff.y * fISpecular * cSpecular);
				}
			};

			float4 frag(in Vs2Ps psIn) : SV_TARGET
			{
				float gamma = 1.0;

				//
				// the following code is more-or-less lifted straight from the fulcrum shader
				// cubemap implementation differs
				//

				// read emissive, ambient, diffuse and specular maps
				// correct gamma to linear
				float3 ctEmission = pow(tex2D(_txEmission, psIn.vTexCoord0).rgb, gamma);
				float3 ctOcclusion = pow(tex2D(_txOcclusion, psIn.vTexCoord0).rgb, gamma);
				float3 ctAlbedo = pow(tex2D(_txAlbedo, psIn.vTexCoord0).rgb, gamma);
				float3 ctSpecular = pow(tex2D(_txSpecular, psIn.vTexCoord0).rgb, gamma);

				// read normal map and Eye, Normal, Tangent and Bitangent vectors
				// from the vertex shader data structure
				float3 vtNormal = tex2D(_txNormal, psIn.vTexCoord0).rgb;
				float3 vToEye = normalize(psIn.vToEye);
				float3 nVertex = normalize(psIn.vWorldNormal);
				float3 nTangent = normalize(psIn.vWorldTangent);
				float3 nBitangent = normalize(psIn.vWorldBitangent);

				// convert the normal map from normalized range [0,1] to [-1,1]
				vtNormal.xyz = 2 * vtNormal - 1;

				// build the tangent space to world space transform and transform
				// the normal map normal from tangent space to world space
				float3x3 mTangentToWorld = { nTangent, nBitangent, nVertex };
				float3 nLight = mul(vtNormal, mTangentToWorld);
				nLight = normalize(nLight);

				// get the ambient illumination along the normal
				float3 ctAmbient = pow(texCUBE(_txAmbient, nLight).rgb, gamma);

				// calculate the eye reflection vector and get reflection map color
				float3 vEyeReflect = 2.0f * dot(nLight,vToEye) * nLight - vToEye;
				float3 ctReflection = pow(texCUBE(_txReflection,vEyeReflect).rgb, gamma);

				// add the emissive and ambient irradiance colorized by albedo
				float3 cOut = _cEmission * ctEmission + _cAmbient * ctAmbient * ctOcclusion * _cAlbedo * ctAlbedo;

				// at this point in the fulcrum shader the illuminance loop is called
				// due to the idiosyncrasies of Unity at this point we sample the one light that is known to be calling
				// this shader - a  single directional light
				// additional (spot) lights are computed in the next pass
				// in this pass we compute the light due to the directional and all non-light related quantities

				// work with the directional light (if it exists) - compute its direction and attenuation
				float3 L;
				float atten;
				if (0.0 == _WorldSpaceLightPos0.w)   // directional light
				{
					atten = 1.0;
					L = normalize(float3(_WorldSpaceLightPos0.xyz));
				}
				else
				{
					L = float3(_WorldSpaceLightPos0 - psIn.vWorldPosition);
					float dist = length(L);
					atten = 1.0 / dist;
					L = normalize(L);
				}

				// hide the material characteristics in class and call its Illuminate() method
				// to light it
				Material obj;
				obj.fFresnelMin = _fFresnelMin;
				obj.fFresnelExp = _fFresnelExp;
				obj.fSpecPower = _fSpecularPower;
				obj.cSpecular = ctSpecular * _cSpecular;
				obj.cAlbedo = ctAlbedo * _cAlbedo;
				obj.cOcclusion = ctOcclusion;
				obj.cResult = float3(0,0,0);

				obj.Illuminate(nVertex,nLight,L,vToEye,float3(_LightColor0.rgb) * atten);

				cOut = cOut + obj.cResult;

				// transparency - modulated by the 'r' channel of the Alpha texture
				float alpha = _fAlphaScale * tex2D(_txAlpha,psIn.vTexCoord0).r;


				// calculate the Fresnel term for the reflected light
				float fFresnelReflect = lerp(1.0f, _fFresnelMin + (1.0f - _fFresnelMin) *
					pow(1.0f - saturate(dot(vEyeReflect,nLight)), _fFresnelExp), _bEnableFresnel);
				cOut += fFresnelReflect * ctSpecular * _cReflection * ctReflection;

				return float4(cOut,alpha);
			}

			ENDCG
		}

		// additional light sources
		// and all light sources with cookies - i.e. spot lights - for these use cookies to achieve the correct angle
		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}
			Blend One One		// additive blending for additional lights

			CGPROGRAM

			#pragma only_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#pragma target 5.0

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			uniform float4 _LightColor0;	// color of light source - from Lighting.cginc

			uniform float3 _cAlbedo;
			uniform float3 _cSpecular;

			uniform float _fDiffuseIntensity;
			uniform float _fSpecularPower;
			uniform float _fHalfLambertExp;
			uniform float _fFresnelMin;
			uniform float _fFresnelExp;
			uniform float _fAlphaScale;

			uniform bool _bEnableFresnel;
			uniform bool _bEnableAlphaFresnel;
			uniform bool _bEnableHalfLambert;

			uniform sampler2D _txOcclusion;
			uniform sampler2D _txAlbedo;
			uniform sampler2D _txNormal;
			uniform sampler2D _txSpecular;
			uniform sampler2D _txAlpha;

			struct VsIn {
				float4 vPosition 		: POSITION;
				float4 vTangent	 		: TANGENT;
				float3 vNormal 			: NORMAL;
				float4 vTexCoord0 		: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Vs2Ps {
				float4 vPosition 		: SV_POSITION;
				float3 vWorldPosition	: TEXCOORD0;
				float3 vToEye 			: TEXCOORD1;
				float3 vWorldNormal 	: TEXCOORD2;
				float3 vWorldTangent 	: TEXCOORD3;
				float3 vWorldBitangent 	: TEXCOORD4;
				float2 vTexCoord0 		: TEXCOORD5;
				LIGHTING_COORDS(6,7)
                UNITY_VERTEX_OUTPUT_STEREO
			};

			Vs2Ps vert(in VsIn vsIn)
			{
				Vs2Ps vsOut;
                UNITY_SETUP_INSTANCE_ID(vsIn);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vsOut);

				// vertex from model space to view space and world space
				// normally a geometry shader would come next, in which case we would only go to world space
				vsOut.vPosition = UnityObjectToClipPos(vsIn.vPosition);
				vsOut.vWorldPosition = mul(unity_ObjectToWorld,vsIn.vPosition).xyz;

				vsOut.vToEye = normalize(WorldSpaceViewDir(vsIn.vPosition));

				// tangent space basis vector from model space to view space
				// normally a geometry shader would come next, in which case we would only go to world space
				vsOut.vWorldNormal = normalize(mul((float3x3)unity_ObjectToWorld,vsIn.vNormal));
				vsOut.vWorldTangent = normalize(mul((float3x3)unity_ObjectToWorld,vsIn.vTangent));
				float3 BiTangent = cross(vsIn.vNormal,vsIn.vTangent.xyz) * vsIn.vTangent.w;
				vsOut.vWorldBitangent = normalize(mul((float3x3)unity_ObjectToWorld,BiTangent));

				// pass through the texture coord
				vsOut.vTexCoord0 = vsIn.vTexCoord0;

				return vsOut;
			}

			// wrap-around lambertian
			float HalfLambert(float nDotL)
			{
				return saturate((nDotL + _fHalfLambertExp) / ((1 + _fHalfLambertExp) * (1 + _fHalfLambertExp)));
			}

			//
			// this class is the per-light computation of the illuminance
			//
			class Material
			{
				// Material properties used in lighting calculation.
				float  fFresnelMin;
				float  fFresnelExp;
				float  fSpecPower;
				float3 cSpecular;
				float3 cAlbedo;
				float3 cOcclusion;
				float3 cResult;

				void Illuminate(
					in float3 nVertex,     // Iterated world space vertex normal
					in float3 nLight,      // World space surface normal for lighting
					in float3 vToLight,    // Direction to light.
					in float3 vToEye,      // Direction to the eye.
					in float3 cIrradiance  // Light irradiance.
					)
				{
					// this next piece of code is the fulcrum illuminance loop

					float nDotL = dot(nLight,vToLight);	// normal due to the normal map
					float nDotLVert = dot(nVertex,vToLight); // interpolated vertex normal

					// calculate the reflection vector from the light
					float3 vReflect = 2.0f * nDotL * nLight - vToLight;

					// Calculate the diffuse intensity and clamp lighting 
					// on backside normal map normals.
					float fBackClamp = 1.0;
					float fIDiffuse = 0.0;

					if (_bEnableHalfLambert)
					{
						fBackClamp = HalfLambert(nDotLVert);
						fIDiffuse = _fDiffuseIntensity * fBackClamp;
					}
					else
					{
						fBackClamp = step(0, nDotLVert);
						fIDiffuse = fBackClamp * _fDiffuseIntensity * saturate(nDotL);
					}

					// Calculate the Fresnel term for the light.
					float2 vCoeff = float2(1, 1);
					if (_bEnableFresnel)
					{
						float fReflect = _fFresnelMin + (1.0 - _fFresnelMin) * pow(1.0 - fIDiffuse, _fFresnelExp);
						vCoeff = float2(1 - fReflect, fReflect);
					}

					// Calculate the specular intensity.
					float fISpecular = fBackClamp * pow(saturate(dot(vReflect, vToEye)), _fSpecularPower);

					// bring the two color computations together
					cResult = cOcclusion * cIrradiance * (vCoeff.x * fIDiffuse * _cAlbedo + vCoeff.y * fISpecular * _cSpecular);
				}
			};

			float4 frag(in Vs2Ps psIn) : SV_TARGET
			{

				// the following code is more-or-less lifted straight from the fulcrum shader
				// cubemap implementation differs

				// read emissive, ambient, diffuse and specular maps
				// correct gamma to linear
				float3 ctOcclusion = pow(tex2D(_txOcclusion, psIn.vTexCoord0).rgb, 2.2);
				float3 ctAlbedo = pow(tex2D(_txAlbedo, psIn.vTexCoord0).rgb, 2.2);
				float3 ctSpecular = pow(tex2D(_txSpecular, psIn.vTexCoord0).rgb, 2.2);

				// read normal map and Eye, Normal, Tangent and Bitangent vectors
				// from the vertex shader data structure
				float3 vtNormal = tex2D(_txNormal, psIn.vTexCoord0).rgb;
				float3 V = normalize(psIn.vToEye);
				float3 nVertex = normalize(psIn.vWorldNormal);
				float3 nTangent = normalize(psIn.vWorldTangent);
				float3 nBitangent = normalize(psIn.vWorldBitangent);

				// convert the normal map from normalized range [0,1] to [-1,1]
				vtNormal.xyz = 2 * vtNormal - 1;

				// build the tangent space to world space transform and transform
				// the normal map normal from tangent space to world space
				float3x3 mTangentToWorld = { nTangent, nBitangent, nVertex };
				float3 N = mul(vtNormal, mTangentToWorld);
				N = normalize(N);

				// calculate the eye reflection vector and get reflection map color
				float3 vEyeReflect = 2.0f * dot(N,V) * N - V;

				// at this point in the fulcrum shader the illuminance loop is called
				// due to the idiosyncrasies of Unity at this point we sample the one light that is known to be calling
				// this shader - a  single directional light
				// additional (spot) lights are computed in the next pass
				// in this pass we compute the light due to the directional and all non-light related quantities

				// work with the directional light (if it exists) - compute its direction and attenuation
				float3 L;
				float atten;
				if (0.0 == _WorldSpaceLightPos0.w)   // directional light
				{
					atten = 1.0;
					L = normalize(float3(_WorldSpaceLightPos0.xyz));
				}
				else
				{
					L = float3(_WorldSpaceLightPos0 - psIn.vWorldPosition);
					float dist = length(L);
					atten = 1.0 / dist;
					L = normalize(L);
				}

				// hide the material characteristics in class and call its Illuminate() method
				// to light it
				Material obj;
				obj.fFresnelMin = _fFresnelMin;
				obj.fFresnelExp = _fFresnelExp;
				obj.fSpecPower = _fSpecularPower;
				obj.cSpecular = ctSpecular * _cSpecular;
				obj.cAlbedo = ctAlbedo * _cAlbedo;
				obj.cOcclusion = ctOcclusion;
				obj.cResult = float3(0,0,0);

				obj.Illuminate(nVertex,N,L,V,float3(_LightColor0.rgb) * atten);

				float3 cOut = obj.cResult;

				// transparency - modulated by the 'r' channel of the Alpha texture
				float alpha = _fAlphaScale * tex2D(_txAlpha,psIn.vTexCoord0).r;

				//cOut=float3(0,0,0);

				return float4(cOut,alpha);
			}
			ENDCG
		}
	}
}
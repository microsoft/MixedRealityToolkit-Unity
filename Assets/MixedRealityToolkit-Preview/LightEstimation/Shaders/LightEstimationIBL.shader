Shader "Mixed Reality Toolkit/LightEstimation IBL" {
	Properties {
		_MainTex   ("Texture",   2D) = "white" {}
		[Toggle(USE_NORMALMAP)]
		_UseNormals("Use NormalMap",    float) = 1
		[NoScaleOffset]
		_BumpMap   ("Normal",    2D) = "bump" {}
		[NoScaleOffset]
		_RoughMap  ("Roughness", 2D) = "white" {}

		_Smoothness("Smoothness", Range(0,1)) = 1
		[Toggle(USE_REFLECTION)]
		_UseReflections("Use Reflection", float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "LightMode" = "ForwardBase" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature USE_NORMALMAP
			#pragma shader_feature USE_REFLECTION
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "UnityStandardConfig.cginc"
			#include "UnityStandardUtils.cginc"

			struct appdata {
				float3 vertex  : POSITION;
				float2 uv      : TEXCOORD0;
				half3  normal  : NORMAL;
#if USE_NORMALMAP
				float4 tangent : TANGENT;
#endif
			};

			struct v2f {
				float2 uv       : TEXCOORD0;
				float4 vertex   : SV_POSITION;
				float3 worldPos : TEXCOORD2;
#if USE_NORMALMAP
				half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
#else
				half3 normal  : NORMAL;
#endif
			};

			sampler2D _RoughMap;
			sampler2D _BumpMap;
			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _Smoothness;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 world = mul(unity_ObjectToWorld, float4(v.vertex, 1.0));
				o.worldPos = world.xyz;
				o.vertex   = mul(UNITY_MATRIX_VP, world);
				o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
#if USE_NORMALMAP
				half3 wNormal     = UnityObjectToWorldNormal(v.normal);
				half3 wTangent    = UnityObjectToWorldDir   (v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half  tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent  = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
#else
				o.normal = UnityObjectToWorldNormal(v.normal);
#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				// Get texture data
				fixed4 albedo = tex2D(_MainTex,  i.uv);
				fixed4 rough  = tex2D(_RoughMap, i.uv);

				// Calculate normal information
				half3 normal;
#if USE_NORMALMAP
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
				normal.x = dot(i.tspace0, tnormal);
				normal.y = dot(i.tspace1, tnormal);
				normal.z = dot(i.tspace2, tnormal);
#else
				normal = i.normal;
#endif
				normal = normalize(normal);

				// Setup modifier values
				float smooth    = max(0.0001f, _Smoothness) * rough.a;
				float iSmooth   = 1 - smooth;
				float metal     = rough.r * _Smoothness;
				float occlusion = rough.g;

				// Prepare vector information for calculating specular information
				half3  worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				half3  worldRefl    = reflect  (-worldViewDir, normal);
				float3 halfVector   = normalize(_WorldSpaceLightPos0.xyz + worldViewDir);

				// Calculate the specular term, including reflections
				//half3 reflectionSample = ShadeSH9(half4(worldRefl, 1));
				float specularHigh = pow(saturate(dot(halfVector, normal)), smooth * 40);
				half3 specColor    = _LightColor0*lerp(half3(1,1,1), albedo, 1 - metal);
#if USE_REFLECTION
				half3 reflection   = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, iSmooth * 8) * smooth;
				half3 specular     = occlusion * metal * (specColor * specularHigh + reflection);
				albedo.rgb *= iSmooth; 
#else
				half3 specular = occlusion * metal * (specColor * specularHigh);
#endif

				// Calculate diffuse and ambient lighting
				//half4 ambientSample    = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, normal, 8);
				half3 ambient = ShadeSH9(half4(normal, 1));// DecodeHDR(ambientSample, unity_SpecCube0_HDR);
				half  nl      = max(0, dot(normal, _WorldSpaceLightPos0.xyz));
				
				// Now combine it all
				// 1 - max(specular.r, max(specular.g, specular.b)); // 1 - max(reflection.r, max(reflection.g, reflection.b));
				return fixed4((ambient + nl*_LightColor0)*albedo + specular, 1);
			}
			ENDCG
		}
	}
}

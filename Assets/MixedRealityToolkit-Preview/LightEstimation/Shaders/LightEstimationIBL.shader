Shader "Mixed Reality Toolkit/LightEstimation IBL" {
	Properties {
		_MainTex   ("Texture", 2D) = "white" {}
		_BumpMap   ("Normal",  2D) = "bump" {}

		[Toggle(USE_NORMALMAP)]
		_UseNormals("Use Normals", float) = 1

		//_Smoothness("Smoothness", Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque"  "LightMode" = "ForwardBase" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma shader_feature USE_NORMALMAP
			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct appdata {
				float4 vertex  : POSITION;
				float2 uv      : TEXCOORD0;
				half3  normal  : NORMAL;
#if USE_NORMALMAP
				float4 tangent : TANGENT;
#endif
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
#if USE_NORMALMAP
				float3 worldPos : TEXCOORD2;
				half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
#else
				half3 normal : NORMAL;
#endif
			};

			sampler2D _BumpMap;
			sampler2D _MainTex;
			float4    _MainTex_ST;
			float     _Smoothness;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex   = UnityObjectToClipPos(v.vertex);
				o.uv       = TRANSFORM_TEX(v.uv, _MainTex);
#if USE_NORMALMAP
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 wNormal  = UnityObjectToWorldNormal(v.normal);
				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
#else
				o.normal = UnityObjectToWorldNormal(v.normal);
#endif

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				half3 normal;
#if USE_NORMALMAP
				half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
				normal.x = dot(i.tspace0, tnormal);
				normal.y = dot(i.tspace1, tnormal);
				normal.z = dot(i.tspace2, tnormal);

				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				half3 worldRefl    = reflect(-worldViewDir, normal);
#else
				normal = i.normal;
#endif
				half nl = max(0, dot(normal, _WorldSpaceLightPos0.xyz));

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				half4 ambient = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, normal, 8);
				ambient = half4(DecodeHDR(ambient, unity_SpecCube0_HDR), 1);
				half4 light   = (ambient + nl*_LightColor0);
				//half4 reflection = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, worldRefl, (1-_Smoothness)*8);
				return light * col;
			}
			ENDCG
		}
	}
}

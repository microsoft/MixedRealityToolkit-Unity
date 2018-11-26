// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "HUX/Acrylic Reflective"
{
		Properties
		{
			_Color("Color", Color) = (1,1,1,1)
			_MainTex("R (Reflection / Specular) G (Emission) B (Diffuse)", 2D) = "white" {}
			_ReflectionTexture ("Reflection Texture", Cube) = "white" { TexGen ObjectLinear }
			_ReflectionPower ("Reflection Power", Range (0.01, 10)) = 0.5
			_EmissionStrength ("Emission Strength", Range (0, 1)) = 0.5
			_RimPower ("Rim Power", Range (0.01, 10)) = 0.5
			_NearPlaneFadeDistance("Near Fade Distance", Range(0, 1)) = 0.1
		}

		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			Cull Back
			LOD 300

				CGPROGRAM
				#pragma target 5.0
				#pragma only_renderers d3d11
				#pragma surface surf BlinnPhong vertex:vert

				fixed3 _Color;
				sampler2D _MainTex;
				samplerCUBE _ReflectionTexture;
				float _ReflectionPower;
				float _EmissionStrength;
				float _RimPower;
				float4 _NearPlaneFadeDistance;

				inline float ComputeNearPlaneFadeLinear(float4 vertex)
				{
					float distToCamera = -(mul(UNITY_MATRIX_MV, vertex).z);
					return saturate(mad(distToCamera, _NearPlaneFadeDistance.y, _NearPlaneFadeDistance.x));
				}

				struct Input {
					half2 uv_MainTex;
					half2 uv_ReflTex;
					half4 screenPos;
					half3 viewDir;
					float3 worldRefl;
					float fade;
				};

				void surf(Input IN, inout SurfaceOutput o) {
					fixed3 tex 	= tex2D (_MainTex, IN.uv_MainTex).rgb;

					half inc	= saturate(dot (normalize(IN.viewDir), o.Normal));
				 	half rim 	= pow (1.0 - inc, _RimPower);
				 	half em 	= inc * _EmissionStrength * tex.g;
				 	fixed3 refl	= (texCUBE (_ReflectionTexture, IN.worldRefl).rgb) * pow (rim, _ReflectionPower);

				 	o.Albedo 	= tex.b * _Color;
				 	o.Emission 	= refl + (rim * _Color) + (em * _Color);
				 	//o.Alpha = IN.fade;
				}

				void vert(inout appdata_full v, out Input o)
				{
					UNITY_INITIALIZE_OUTPUT(Input, o);
					o.fade = ComputeNearPlaneFadeLinear(v.vertex);
				}
				ENDCG
		}
}
// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

///
/// Simple vertex shader that blends static lighting and camera lighting.
///
Shader "MixedRealityToolkit/Examples/UnlitTriplanar"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Ambient("Ambient", Range(0.0,1.0)) = 0.4
		_LightDir("Light Direction", Vector) = (0.3, 0.8, -0.6, 1.0)
		_LightIntensity("Light Intensity", Range(0.0, 1.0)) = 0.4
		_CameraIntensity("Camera Intensity", Range (0.0, 1.0)) = 0.1
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				half3 normal : TEXCOORD0;
				half3 diffuse : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed _Ambient;
			fixed3 _LightDir;
			float1 _LightIntensity;
			float1 _CameraIntensity;
			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr Props
			UNITY_INSTANCING_BUFFER_END(Props)


			v2f vert(float4 pos : POSITION, float3 normal : NORMAL, appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = normal;

				// get camera vector
				float3 cameraDirection = (_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, pos).xyz) * _CameraIntensity;

				// Dot product between normal and light direction for standard 
				// diffuse lambert lighting plus camera vector light.
				float3 light = _LightDir * _LightIntensity;
				o.diffuse = max(_Ambient, dot(UnityObjectToWorldNormal(normal), light)) + dot(UnityObjectToWorldNormal(normal), cameraDirection);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				// Return the color with the diffuse color.
				return  UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color) * fixed4(i.diffuse, 1.0);
			}
			ENDCG
		}
	}
}
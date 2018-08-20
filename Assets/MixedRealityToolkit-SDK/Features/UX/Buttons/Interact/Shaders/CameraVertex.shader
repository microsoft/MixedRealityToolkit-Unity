// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Interact/CameraVertex"
{
	Properties
	{
		[Enum(Opaque, 0, Transparent, 1)] _Opacity("Opacity", Float) = 0
		_Color("Main Color", Color) = (1,1,1,1)
		_Ambient("Ambient", Range(0.0,1.0)) = 0.4
		_CameraIntensity("Camera Intensity", Range(0.0, 1.0)) = 0.1
		_Contrast("_Contrast", Range(0.0, 1.0)) = 0.1

		//blend mode
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 1.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		Pass
		{

			Blend [_SrcBlend] [_DstBlend]
			//ZWrite On

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
				half3 direction : TEXTCORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed _Ambient;
			float1 _CameraIntensity;
			float1 _Contrast;
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

				o.pos = UnityObjectToClipPos(pos);
				o.normal = normal;

				// get camera vector
				float3 cameraDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, pos).xyz);

				float3 diffuse = _Ambient + dot(UnityObjectToWorldNormal(normal), cameraDirection * _CameraIntensity);

				o.direction = cameraDirection;
				o.diffuse = diffuse;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				float edgeFactor = abs(dot(normalize(i.direction), normalize(i.normal)));

				float oneMinusEdge = 1.0 - edgeFactor;
				float3 edge = (i.diffuse * edgeFactor) + oneMinusEdge + _Contrast;

				// Return the color with the diffuse color.
				return UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color) * fixed4(i.diffuse * edge, 1.0);
			}

			ENDCG
		}
	}

	CustomEditor "Interact.CameraVertexGUI"
	Fallback "Legacy Shaders/VertexLit" //for shadow casting
}
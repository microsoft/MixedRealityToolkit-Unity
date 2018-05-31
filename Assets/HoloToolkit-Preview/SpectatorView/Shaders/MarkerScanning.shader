// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

Shader "SpectatorView/MarkerScanning"
{
	Properties
	{
		_Color ("Color", Color) = (0, 0, 0, 0)
		_PulseRadius("Pulse Radius", Range(0, 1)) = 0
		_TransitionCompletion("Transition Completion", Range(0, 1)) = 0
		_Grid("Grid", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"  "DisableBatching" = "True" }
	 	Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 worldPosition : TEXCOORD1;
			};

			float4 _Color;
			float _PulseRadius;
			sampler2D _Grid;
			float _TransitionCompletion;

			v2f vert (appdata v)
			{
				v2f o;
				o.worldPosition = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 r=abs(float2(i.worldPosition.x, i.worldPosition.z));
				float s = max(r.x, r.y);
				_PulseRadius = frac(_Time.y * 0.6);
				float radius = 0.05;
				float outerCircle = ceil(_PulseRadius - (distance(float3(0, 0, 0), i.worldPosition.xyz)-radius));
				float innerCircle = max(0.0,(_PulseRadius - distance(float3(0, 0, 0), i.worldPosition.xyz))) * 10.0;

				float4 gridTex= tex2D(_Grid, i.uv);
				gridTex.a = 1.0 - gridTex.r;
				// fixed4 col = lerp(float4(.97, .97, .97 ,1), lerp(float4(0, 0.4,0.9,1) * float4(gridTex, gridTex, gridTex, gridTex), float4(0, 0, 0, 1), 1.0 - floor(min(1.0, max(0, (outerCircle - innerCircle))))), min(1.0, max(0, (outerCircle - innerCircle))));
				fixed4 col = lerp(float4(.97, .97, .97 ,1), gridTex, min(1.0, max(0, (outerCircle - innerCircle))));

				float doTransition = ceil(_TransitionCompletion);

				fixed4 transitionCol = lerp(float4(.97, .97, .97 ,1),float4(0, 0, 0, 1), ceil(_TransitionCompletion - (distance(float3(0, 0, 0), i.worldPosition.xyz)-radius)));

				col = lerp(col, transitionCol, doTransition);

				return col;
			}
			ENDCG
		}
	}
}

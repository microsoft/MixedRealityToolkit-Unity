// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "MixedRealityToolkit/Cursor"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _Color;
			
			v2f vert (appdata v)
			{
				UNITY_SETUP_INSTANCE_ID(v);

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				return col;
			}
			ENDCG
		}
	}
}

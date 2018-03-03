// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Surface Reconstruction/Debug Normals"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

			// vertex input: position, normal
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert (appdata v) {
				v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos( v.vertex );
				o.color.rgb = v.normal * 0.5 + 0.5;
				o.color.a = 1.0;
				return o;
			}
			fixed4 frag (v2f i) : COLOR0 { return i.color; }
			ENDCG
		}
	}
	Fallback "VertexLit"
}

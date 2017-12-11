// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Copyright (C) Microsoft. All rights reserved.
//

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

			// vertex input: position, normal
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};
			v2f vert (appdata v) {
				v2f o;
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

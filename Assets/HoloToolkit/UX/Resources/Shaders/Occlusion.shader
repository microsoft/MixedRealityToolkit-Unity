// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Copyright (C) Microsoft. All rights reserved.
//

//
// Depth only shader.  Writes to the depth buffer, but color writes are disabled for performance.
//

Shader "Surface Reconstruction/Occlusion"
{
	Properties
	{
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}

		Pass
		{
			ColorMask 0
			Offset 50, 100

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers d3d11_9x

			#include "UnityCG.cginc"

			struct v2f {
				float4  pos : SV_POSITION;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				return float4(1,1,1,1);
			}
			ENDCG
		}
	}
}

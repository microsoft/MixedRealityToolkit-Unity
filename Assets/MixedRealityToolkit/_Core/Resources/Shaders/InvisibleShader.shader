// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "MixedRealityToolkit/InvisibleShader" {

	Subshader
	{
		Pass
		{
			GLSLPROGRAM
			#ifdef VERTEX
			void main() {}
			#endif

			#ifdef FRAGMENT
			void main() {}
			#endif
			ENDGLSL
		}
	}

	Subshader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

			struct v2f 
			{
				fixed4 position : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_base v)
			{
				v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.position = fixed4(0,0,0,0);
				return o;
			}

			fixed4 frag() : COLOR
			{
				return fixed4(0,0,0,0);
			}
		ENDCG
		}
	}
}
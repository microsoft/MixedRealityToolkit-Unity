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
			struct v2f 
			{
				fixed4 position : SV_POSITION;
			};

			v2f vert() 
			{
				v2f o;
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
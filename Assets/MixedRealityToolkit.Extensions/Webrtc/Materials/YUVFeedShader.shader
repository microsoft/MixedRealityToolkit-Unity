Shader "Custom/YUVFeedShader" {
	Properties{
		_MainTex("Main Tex", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma surface surf Lambert //alpha
			struct Input {
				float2 uv_MainTex;
			};
			sampler2D _MainTex;
			float3 yuv2rgb(float3 yuv) {
				// The YUV to RBA conversion, please refer to: http://en.wikipedia.org/wiki/YUV
				// Y'UV420sp (NV21) to RGB conversion (Android) section.
				float y_value = yuv[0];
				float u_value = yuv[1];
				float v_value = yuv[2];
				float r = y_value + 1.370705 * (v_value - 0.5);
				float g = y_value - 0.698001 * (v_value - 0.5) - (0.337633 * (u_value - 0.5));
				float b = y_value + 1.732446 * (u_value - 0.5);
				return float3(r, g, b);
			}
			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = yuv2rgb(tex2D(_MainTex, IN.uv_MainTex).rgb);
			}
			ENDCG
	}
		Fallback "Diffuse"
}

// Very fast unlit shader.
// No lighting, lightmap support, etc.
// Compiles down to only performing the operations you're actually using.
// Uses material property drawers rather than a custom editor for ease of maintenance.

Shader "HUX/Button Highlight"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite On
		Cull Back
		ColorMask RGBA

		CGPROGRAM
		#pragma target 5.0
		#pragma only_renderers d3d11
		#pragma surface surf Lambert alpha

		sampler2D _MainTex;
		fixed4 _Color;
		uniform float3 _HUXButtonGlowTarget;
		uniform float3 _HUXButtonGlowColor;
		uniform float3 _HUXButtonEdgeColor;
		uniform float _HUXButtonGlowRadius;

		struct Input {
			half2 uv_MainTex;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = _Color * tex;
			// DISABLE THIS UNTIL LUMINOUS SHADER IS FINISHED
			//float dist = distance(IN.worldPos, _HUXButtonGlowTarget);
			//dist = dist * dist;
			//float glow = clamp((_HUXButtonGlowRadius / dist), 0, 1) * tex.a;
			o.Emission = _Color * tex;// _HUXButtonEdgeColor * glow;
			// HUX buttons glow just a bit even if they're not selected
			// So add a bit of that glow to the alpha
			o.Alpha = tex.a * _Color.a;// max(_Color.a, glow);
		}
		ENDCG
	}
}
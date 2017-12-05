//
// Copyright (C) Microsoft. All rights reserved.
//

Shader "Analog/TransparentUnlit"
{
	Properties
	{
		_MainTex ("Alpha", 2D) = "white" {}
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		_Color("Color", Color ) = (0.3,0.3, 0.7, 1.0)
		_Brightness("Brightness", Range(0.0,16.0)) = 1.0
	}
	SubShader
	{
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue" = "Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Unlit alpha

		sampler2D _MainTex;
		float _Opacity;
		float3 _Color;
		float _Brightness;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			fixed4 c = fixed4(0.0,0.0,0.0, s.Alpha);
			return c;
		}


		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color * _Brightness;
			o.Alpha = c.a *_Opacity;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}


//
// Copyright (C) Microsoft. All rights reserved.
//

Shader "Analog/TransparentUnlit2"
{
	Properties
	{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Opacity("Opacity", Range(0.0, 1.0)) = 1.0
		_Color("Main Color", Color) = (0.3,0.3, 0.7, 1.0)
		//_Brightness("Brightness", Range(0.0,16.0)) = 1.0
		//_Cutoff("Base Alpha cutoff", Range(0, 1.0)) = 0.5
	}

	SubShader
	{
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200

		// extra pass that renders tod depth buffer only
	Pass
	{
		//ZTest Greater
		//ZWrite On
		ColorMask 0
		//Offset 10, 250 // Hacky away to avoid z-depth issues
	}

		// paste in forward rendering passes from Transparent/Diffuse
		//UsePass "Transparent/Diffuse/FORWARD"

	CGPROGRAM
	#pragma surface surf Unlit alpha

	sampler2D _MainTex;
	float _Opacity;
	float3 _Color;
	float _Brightness;

	half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
	{
		//fixed4 c = fixed4(_Color.r, _Color.g, _Color.b, s.Alpha);
		//fixed4 c = fixed4(0.0,0.0,0.0, s.Alpha);
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}


	struct Input
	{
		float2 uv_MainTex;
		//float3 worldPos;
	};

	void surf(Input IN, inout SurfaceOutput o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb *_Color;// *_Brightness;
		o.Alpha = c.a *_Opacity;
	}
	ENDCG
	}
	FallBack "Diffuse"
}


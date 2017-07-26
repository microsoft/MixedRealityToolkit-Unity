// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "HoloToolkit/UnlitTransparentTriplanar"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_Ambient("Ambient", Range(0.0,1.0)) = 0.4
		_LightDir("Light Direction", Vector) = (0.3, 0.8, -0.6, 1.0)
		_LightIntensity("Light Intensity", Range(0.0, 1.0)) = 0.4
		_CameraIntensity("Camera Intensity", Range (0.0, 1.0)) = 0.1
	}
		SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

			struct v2f
		{
			float4 pos : SV_POSITION;
			half3 normal : TEXCOORD0;
			half3 diffuse : COLOR0;
		};

		fixed _Ambient;
		fixed3 _LightDir;
		fixed4 _Color;
		float1 _LightIntensity;
		float1 _CameraIntensity;

		v2f vert(float4 pos : POSITION, float3 normal : NORMAL)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(pos);
			o.normal = normal;

			// get camera vector
			float3 cameraDirection = (_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, pos).xyz) * _CameraIntensity;

			// Dot product between normal and light direction for standard 
			// diffuse lambert lighting plus camera vector light.
			float3 light = _LightDir * _LightIntensity;
			o.diffuse = max(_Ambient, dot(UnityObjectToWorldNormal(normal), light)) + dot(UnityObjectToWorldNormal(normal), cameraDirection);

			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// Return the color with the diffuse color.
			return _Color * fixed4(i.diffuse, 1.0);
		}
			ENDCG
		}
	}
}
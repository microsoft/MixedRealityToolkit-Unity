// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnlitTextureVertColor"
{
	Properties
	{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB) Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		//Tags{ "RenderType" = "Transparent"}
		Tags{ Queue = Transparent }

		Lighting Off
		ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.
		ZTest LEqual
		Cull back
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0

			struct appdata {
				float4 pos : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float4 vertColor : COLOR; // Vertex color stored here by vert() method
			};

			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			// vertex shader
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				// just pass the texture coordinate
				o.uv = v.uv_MainTex;
				o.color = v.vertColor;
				return o;
			}

			sampler2D _MainTex;
			fixed4 _Color;

			fixed4 frag(v2f IN) : SV_Target
			{
				// Albedo comes from a texture tinted by color
				fixed4 col = tex2D(_MainTex, IN.uv) * _Color * IN.color;
				return col;
			}
			ENDCG
		}
	}
}

/*Shader "Custom/UnlitTextureColor"
{
	Properties
	{
		_Color("Color Tint", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white"
	}

	Category
		{
			Lighting Off
			//ZWrite Off
			ZWrite On  // uncomment if you have problems like the sprite disappear in some rotations.
			ZTest LEqual
			Cull back
			Blend SrcAlpha OneMinusSrcAlpha
			//AlphaTest Greater 0.001  // uncomment if you have problems like the sprites or 3d text have white quads instead of alpha pixels.
			Tags{ Queue = Transparent }

			BindChannels
			{
				Bind "Color", color
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
			}

			SubShader
			{
				Pass
				{
					SetTexture[_MainTex]
					{
						ConstantColor[_Color]
						Combine Texture * constant
					}
				}
			}
		}
}*/
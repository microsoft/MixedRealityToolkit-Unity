Shader "Custom/Simple3DTextShader"{
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Lighting Off Cull Off ZWrite Off Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		BindChannels{
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
		Pass{
			Color[_Color]
			SetTexture[_MainTex]{
				combine primary, texture * primary
			}
		}
	}
}
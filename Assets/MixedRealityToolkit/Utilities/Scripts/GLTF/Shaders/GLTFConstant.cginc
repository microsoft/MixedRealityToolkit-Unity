#include "UnityCG.cginc"

uniform fixed4 _AmbientFactor;
uniform fixed4 _EmissionColor;
uniform fixed4 _LightFactor;

uniform half _Cutoff;

#ifdef EMISSION_MAP_ON
	uniform sampler2D _EmissionMap;
	uniform float4 _EmissionMap_ST;
	uniform half _EmissionUV;
#endif

#ifdef LIGHTMAP_ON
	uniform sampler2D _LightMap;
	uniform float4 _LightMap_ST;
	uniform half _LightUV;
#endif

struct vertexInput {
	float4 vertex : POSITION;
	#ifdef VERTEX_COLOR_ON
		fixed4 color : COLOR;
	#endif
	#if defined(EMISSION_MAP_ON) || defined(LIGHTMAP_ON)
		float2 uv0 : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
	#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
struct vertexOutput {
	float4 pos : SV_POSITION;
	#ifdef VERTEX_COLOR_ON
		fixed4 color : COLOR;
	#endif
	#ifdef EMISSION_MAP_ON
		float2 emissionCoord : TEXCOORD0;
	#endif
	#ifdef LIGHTMAP_ON
		float2 lightmapCoord : TEXCOORD1;
	#endif
    UNITY_VERTEX_OUTPUT_STEREO
};

vertexOutput vert(vertexInput input)
{
	vertexOutput output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

	#ifdef EMISSION_MAP_ON
		float2 emissionCoord =
			(_EmissionUV == 0) * input.uv0 +
			(_EmissionUV == 1) * input.uv1;
		output.emissionCoord = TRANSFORM_TEX(emissionCoord, _EmissionMap);
	#endif
	#ifdef LIGHTMAP_ON
		float2 lightmapCoord =
			(_LightUV == 0) * input.uv0 +
			(_LightUV == 1) * input.uv1;
		output.lightmapCoord = TRANSFORM_TEX(lightmapCoord, _LightMap);
	#endif

	output.pos = UnityObjectToClipPos(input.vertex);
	#ifdef VERTEX_COLOR_ON
		output.color = (fixed4) input.color;
	#endif
	return output;
}

fixed4 frag(vertexOutput input) : COLOR
{
	fixed4 finalColor = _EmissionColor;

	#ifdef EMISSION_MAP_ON
		finalColor *= tex2D(_EmissionMap, input.emissionCoord);
	#endif

	#ifdef _ALPHATEST_ON
		if (finalColor.a < _Cutoff) {
			discard;
		}
	#endif

	#ifdef VERTEX_COLOR_ON
		finalColor *= fixed4(input.color.rgb, 1);
	#endif

	#ifdef LIGHTMAP_ON
		fixed4 lightmapColor = tex2D(_LightMap, input.lightmapCoord);
		finalColor = lerp(finalColor, finalColor*lightmapColor, fixed4(_LightFactor.rgb, 0));
	#endif

	fixed4 ambient = unity_AmbientSky * _AmbientFactor;
	finalColor = ambient + finalColor;

	return finalColor;
}

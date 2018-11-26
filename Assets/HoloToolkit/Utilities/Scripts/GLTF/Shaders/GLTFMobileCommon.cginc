#ifndef GLTF_MOBILE_COMMON_INCLUDED
#define GLTF_MOBILE_COMMON_INCLUDED

#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardBRDF.cginc"

#ifdef _ALPHATEST_ON
half _Cutoff;
#endif

fixed4 _Color;
fixed4 _MainTex_ST;
sampler2D _MainTex;

half _Metallic;
half _Roughness;
sampler2D _MetallicRoughnessMap;

half _OcclusionStrength;
sampler2D _OcclusionMap;

fixed3 _EmissionColor;
sampler2D _EmissionMap;

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
	#ifdef VERTEX_COLOR_ON
	fixed4 vertColor : COLOR;
	#endif
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
	float3 normal : TEXCOORD2;
	float3 viewDir : TEXCOORD3;
	#ifdef VERTEX_COLOR_ON
	fixed4 vertColor : COLOR;
	#endif
};

v2f gltfMobileVert (appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);

	#ifdef VERTEX_COLOR_ON
	o.vertColor = v.vertColor;
	#endif

	o.normal = UnityObjectToWorldNormal(v.normal);

	float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
	o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

	return o;
}

// NOTE: assumes that this is only used with directional light pass
fixed4 gltfMobileFrag (v2f i) : SV_Target
{
	i.normal = normalize(i.normal);
	#if defined(_ALPHATEST_ON)

	#ifdef VERTEX_COLOR_ON
	fixed4 color = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
	#else
	fixed4 color = tex2D(_MainTex, i.uv) * _Color;
	#endif

	clip(color.a - _Cutoff);
	fixed3 albedo = color.rgb;

	#elif defined(_ALPHABLEND_ON)

	#ifdef VERTEX_COLOR_ON
	fixed4 color = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
	#else
	fixed4 color = tex2D(_MainTex, i.uv) * _Color;
	#endif

	fixed3 albedo = color.rgb;

	#else // ALPHA_MODE

	#ifdef VERTEX_COLOR_ON
	fixed3 albedo = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
	#else
	fixed3 albedo = tex2D(_MainTex, i.uv) * _Color;
	#endif

	#endif // ALPHA_MODE

	fixed3 metallicRoughness = tex2D(_MetallicRoughnessMap, i.uv);
	fixed metallic = metallicRoughness.b * _Metallic;

	#ifdef OCC_METAL_ROUGH_ON
	fixed3 occlusion = metallicRoughness.r * _OcclusionStrength;
	#else
	fixed3 occlusion = tex2D(_OcclusionMap, i.uv).r * _OcclusionStrength;
	#endif

	fixed3 specularTint;
	fixed oneMinusReflectivity;
	albedo = DiffuseAndSpecularFromMetallic(
		albedo, metallic, specularTint, oneMinusReflectivity
	);

	fixed3 emmission = tex2D(_EmissionMap, i.uv).rgb * _EmissionColor;

	UnityLight light;
	light.color = _LightColor0.rgb;
	light.dir = _WorldSpaceLightPos0.xyz;
	light.ndotl = DotClamped(i.normal, light.dir);

	UnityIndirect indirectLight;
	indirectLight.diffuse = max(0, ShadeSH9(float4(i.normal, 1)));
	indirectLight.specular = 0;

	fixed smoothness = metallicRoughness.g * (1 - _Roughness);

	// we might want to also consider BRDF2_Unity_PBS
	fixed3 pbsValue = BRDF3_Unity_PBS(
		albedo, specularTint,
		oneMinusReflectivity, smoothness,
		i.normal, i.viewDir,
		light, indirectLight
	);
	// TODO: are we using occlusion and emission correctly??
	#ifdef _ALPHABLEND_ON
	return fixed4(pbsValue * occlusion + emmission, color.a);
	#else
	return fixed4(pbsValue * occlusion + emmission, 1);
	#endif

}
#endif // GLTF_MOBILE_COMMON_INCLUDED

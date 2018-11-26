#ifndef GLTF_VERTEX_LIT_COMMON_INCLUDED
#define GLTF_VERTEX_LIT_COMMON_INCLUDED
#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#ifdef _ALPHATEST_ON
half _Cutoff;
#endif

float4 _MainTex_ST;
sampler2D _MainTex;
fixed4 _Color;
half _OcclusionStrength;
#ifdef OCC_METAL_ROUGH_ON
sampler2D _MetallicRoughnessMap;
#else
sampler2D _OcclusionMap;
#endif
fixed4 _EmissionColor;
sampler2D _EmissionMap;

struct vertIn 
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2f 
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	fixed3 computedShading : TEXCOORD2;
	#ifdef VERTEX_COLOR_ON
	fixed4 vertColor : COLOR;
	#endif
	LIGHTING_COORDS(3, 4)
	UNITY_FOG_COORDS(5)
};

// NOTE: this assumes that we only calculate lighting for directional lights!
v2f gltfVertexFunc(vertIn v)
{
	v2f o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

	float3 worldNormal = UnityObjectToWorldNormal(v.normal);
	// add ambient via spherical harmonics
	o.computedShading = max(0, ShadeSH9(float4(v.normal, 1)));
	fixed lambertianValue = DotClamped(worldNormal, _WorldSpaceLightPos0.xyz);
	o.computedShading += lambertianValue * _LightColor0.rgb;

	TRANSFER_VERTEX_TO_FRAGMENT(o);
	UNITY_TRANSFER_FOG(o, o.pos);

	#ifdef VERTEX_COLOR_ON
	o.vertColor = v.color;
	#endif

	return o;
}

fixed4 gltfFragFunc(v2f i) : SV_Target
{
	#ifdef VERTEX_COLOR_ON
	half4 albedo = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
	#else
	half4 albedo = tex2D(_MainTex, i.uv) * _Color;
	#endif
	fixed4 mainColor = fixed4(albedo.rgb * i.computedShading, albedo.a);

	UNITY_APPLY_FOG(i.fogCoord, mainColor);

	#ifdef _ALPHATEST_ON
	clip(mainColor.a  - _Cutoff);
	#endif

	#ifdef OCC_METAL_ROUGH_ON
	fixed4 occlusion = tex2D(_MetallicRoughnessMap, i.uv).r * _OcclusionStrength;
	#else
	fixed4 occlusion = tex2D(_OcclusionMap, i.uv).r * _OcclusionStrength;
	#endif

	fixed4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;

	return mainColor * fixed4(occlusion.rgb, 1.0) + fixed4(emission.rgb, 0.0);
}

struct vertInUnlit
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	fixed4 color : COLOR;
};

struct v2fUnlit
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
#ifdef VERTEX_COLOR_ON
	fixed4 vertColor : COLOR;
#endif
	UNITY_FOG_COORDS(4)
};

v2fUnlit gltfVertexUnlit(vertInUnlit v)
{
	v2fUnlit o;
	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

	UNITY_TRANSFER_FOG(o, o.pos);

#ifdef VERTEX_COLOR_ON
	o.vertColor = v.color;
#endif

	return o;
}

fixed4 gltfFragUnlit(v2fUnlit i) : SV_Target
{
#ifdef VERTEX_COLOR_ON
	half4 mainColor = tex2D(_MainTex, i.uv) * _Color * i.vertColor;
#else
	half4 mainColor = tex2D(_MainTex, i.uv) * _Color;
#endif

	UNITY_APPLY_FOG(i.fogCoord, mainColor);

#ifdef _ALPHATEST_ON
	clip(mainColor.a - _Cutoff);
#endif

#ifdef OCC_METAL_ROUGH_ON
	fixed4 occlusion = tex2D(_MetallicRoughnessMap, i.uv).r * _OcclusionStrength;
#else
	fixed4 occlusion = tex2D(_OcclusionMap, i.uv).r * _OcclusionStrength;
#endif

	fixed4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;

	return mainColor * fixed4(occlusion.rgb, 1.0) + fixed4(emission.rgb, 0.0);
}

#endif

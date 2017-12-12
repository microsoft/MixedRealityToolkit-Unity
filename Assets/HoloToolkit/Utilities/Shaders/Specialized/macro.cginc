#ifndef MACRO
#define MACRO

//bootleg ternary operator - no __VA_ARGS__
#define IIF(cond) IIF_ ## cond
#define IIF_0(t, f) f
#define IIF_1(t, f) t

#define CONCAT(arg1, arg2) arg1 ## arg2

#define TRANSFORM_TEX_00(uv, st) uv
#define TRANSFORM_TEX_10(uv, st) uv * st.xy
#define TRANSFORM_TEX_01(uv, st) uv + st.zw
#define TRANSFORM_TEX_11(uv, st) uv * st.xy + st.zw

// main tex
#ifdef _MainTex_SCALE_ON
	#define _MainTex_SCALE_TOGGLE 1
#else
	#define _MainTex_SCALE_TOGGLE 0
#endif

#ifdef _MainTex_OFFSET_ON
	#define _MainTex_OFFSET_TOGGLE 1
#else
	#define _MainTex_OFFSET_TOGGLE 0
#endif

#define MAINTEX_TYPE_S CONCAT(TRANSFORM_TEX_, _MainTex_SCALE_TOGGLE)
#define MAINTEX_TYPE_SO CONCAT(MAINTEX_TYPE_S, _MainTex_OFFSET_TOGGLE)
#define TRANSFORM_TEX_MAINTEX(uv, st) MAINTEX_TYPE_SO ## (uv, st)

//bump map
#ifdef _BumpMap_SCALE_ON
	#define _BumpMap_SCALE_TOGGLE 1
#else
	#define _BumpMap_SCALE_TOGGLE 0
#endif

#ifdef _BumpMap_OFFSET_ON
	#define _BumpMap_OFFSET_TOGGLE 1
#else
	#define _BumpMap_OFFSET_TOGGLE 0
#endif

#define BUMPMAP_TYPE_S CONCAT(TRANSFORM_TEX_, _BumpMap_SCALE_TOGGLE)
#define BUMPMAP_TYPE_SO CONCAT(BUMPMAP_TYPE_S, _BumpMap_OFFSET_TOGGLE)
#define TRANSFORM_TEX_BUMPMAP(uv, st) BUMPMAP_TYPE_SO ## (uv, st)

//occlusion map
#ifdef _OcclusionMap_SCALE_ON
	#define _OcclusionMap_SCALE_TOGGLE 1
#else
	#define _OcclusionMap_SCALE_TOGGLE 0
#endif

#ifdef _OcclusionMap_OFFSET_ON
	#define _OcclusionMap_OFFSET_TOGGLE 1
#else
	#define _OcclusionMap_OFFSET_TOGGLE 0
#endif

#define OCCLUSIONMAP_TYPE_S CONCAT(TRANSFORM_TEX_, _OcclusionMap_SCALE_TOGGLE)
#define OCCLUSIONMAP_TYPE_SO CONCAT(OCCLUSIONMAP_TYPE_S, _OcclusionMap_OFFSET_TOGGLE)
#define TRANSFORM_TEX_OCCLUSIONMAP(uv, st) OCCLUSIONMAP_TYPE_SO ## (uv, st)

#endif //MACRO
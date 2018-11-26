// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "HUX/Button Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull Back
		LOD 300

		CGPROGRAM
		#pragma target 5.0
		#pragma only_renderers d3d11
		#pragma surface surf Lambert

		uniform float3 _HUXButtonGlowTarget;
		uniform float3 _HUXButtonGlowColor;
		uniform float3 _HUXButtonEdgeColor;
		uniform float _HUXButtonGlowRadius;

		fixed3 _Color;
								
		struct Input {
			float3 worldPos;
		};
								
		void surf(Input IN, inout SurfaceOutput o) {
					
			o.Albedo = _Color;
			float dist = distance(IN.worldPos, _HUXButtonGlowTarget);
			dist = dist * dist;
			o.Emission = _HUXButtonGlowColor * clamp((_HUXButtonGlowRadius / dist), 0, 1);
		}
		ENDCG
	}
}
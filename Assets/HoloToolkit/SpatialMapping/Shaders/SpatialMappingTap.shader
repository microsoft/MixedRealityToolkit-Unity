// Licensed under the MIT License. See LICENSE in the project root for license information.

Shader "Spatial Mapping/Spatial Mapping Tap"
{
	Properties
	{
		// Main knobs
		_Center ("Center", Vector) = (0, 0, 0, -1) // world space position
		_Radius ("Radius", Range(0, 10)) = 1 // grows the pulse

		// Pulse knobs
		_PulseColor ("Pulse Color", Color) = (.145, .447, .922)
		_PulseWidth ("Pulse Width", Float) = 1

		// Wireframe knobs
		[MaterialToggle] _UseWireframe ("Use Wireframe", Int) = 1
		_WireframeColor ("Wireframe Color", Color) = (.5, .5, .5)
		_WireframeFill ("Wireframe Fill", Range(0, 1)) = .1
	}
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			Offset 50, 100

			CGPROGRAM

			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			#include "UnityCG.cginc"

			half _Radius;
			half3 _Center;
			half3 _PulseColor;
			half  _PulseWidth;
			half3 _WireframeColor;
			half  _WireframeFill;
			int _UseWireframe;

		    // http://www.iquilezles.org/www/articles/functions/functions.htm
			half cubicPulse(half c, half w, half x)
			{
				x = abs(x - c);
				if ( x > w )
					return 0;
				x /= w;
				return 1 - x * x * (3 - 2 * x);
			}

			struct v2g
			{
				half4 viewPos : SV_POSITION;
				half  pulse : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2g vert(appdata_base v)
			{
				v2g o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.viewPos = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				half distToCenter = distance(_Center, worldPos.xyz);		
				half pulse = cubicPulse(_Radius, _PulseWidth, distToCenter);

				o.pulse = pulse;

				return o;
			}

			struct g2f
			{
				float4 viewPos : SV_POSITION;
				half3  bary    : COLOR;
				half   pulse   : COLOR1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
			{
				// For wireframe
				half3 barys[3] = {
					half3(1, 0, 0),
					half3(0, 1, 0),
					half3(0, 0, 1)
				};

				g2f o;

				[unroll]
				for (uint idx = 0; idx < 3; ++idx)
				{
					UNITY_SETUP_INSTANCE_ID(i[idx]);
					UNITY_TRANSFER_INSTANCE_ID(i[idx], o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.viewPos = i[idx].viewPos;
					o.bary = barys[idx];
					o.pulse = i[idx].pulse;
					triStream.Append(o);
				}
			}

			half4 frag(g2f i) : COLOR
			{
				UNITY_SETUP_INSTANCE_ID(i);

				half3 result = i.pulse * _PulseColor;

				if (!_UseWireframe)
					return half4(result, 1);

				half triBary = min( min(i.bary.x, i.bary.y), i.bary.z) * 3;
				half fwt = fwidth(triBary);
				half w = smoothstep(fwt, 0, triBary - _WireframeFill);
				
				result += w * _WireframeColor * i.pulse;
				
				return half4(result, 1);
			}

			ENDCG
		}
	}
	
	FallBack "Diffuse"
}


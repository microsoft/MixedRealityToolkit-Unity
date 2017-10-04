// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Copyright (C) Microsoft. All rights reserved.
//

Shader "Surface Reconstruction/Wireframe" {
    Properties {
        _BaseColor ("Base color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor ("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WireThickness ("Wire thickness", Range (0, 800)) = 100
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            Offset 50, 100

            CGPROGRAM

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _BaseColor;
            float4 _WireColor;
            float _WireThickness;

            // Based on approach described in "Shader-Based Wireframe Drawing", http://cgg-journal.com/2008-2/06/index.html

            struct v2g {
                float4 viewPos : SV_POSITION;
            };

            v2g vert(appdata_base v) {
                v2g o;
                o.viewPos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            // inverseW is to counter-act the effect of perspective-correct interpolation so that the lines look the same thickness
            // regardless of their depth in the scene.
            struct g2f {
                float4 viewPos : SV_POSITION;
                float inverseW : TEXCOORD0;
                float3 dist : TEXCOORD1;
            };

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream) {

				float2 WIN_SCALE = float2(_ScreenParams.x / 2.0, _ScreenParams.y / 2.0);

                // Calculate the vectors that define the triangle from the input points.
                float2 point0 = WIN_SCALE * i[0].viewPos.xy / i[0].viewPos.w;
                float2 point1 = WIN_SCALE * i[1].viewPos.xy / i[1].viewPos.w;
                float2 point2 = WIN_SCALE * i[2].viewPos.xy / i[2].viewPos.w;

                // Calculate the area of the triangle.
                float2 vector0 = point2 - point1;
                float2 vector1 = point2 - point0;
                float2 vector2 = point1 - point0;
                float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

				float wireScale = 1 - (_WireThickness / 800);

                // Output each original vertex with its distance to the opposing line defined
                // by the other two vertices.

                g2f o;

                o.viewPos = i[0].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(area / length(vector0), 0, 0) *wireScale;
                triStream.Append(o);

                o.viewPos = i[1].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(0, area / length(vector1), 0) *wireScale;
                triStream.Append(o);

                o.viewPos = i[2].viewPos;
                o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(0, 0, area / length(vector2)) *wireScale;
                triStream.Append(o);
            }

            float4 frag(g2f i) : COLOR {
                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
				float dist = min(i.dist[0], min(i.dist[1], i.dist[2]));

                // Make the intensity of the line very bright along the triangle edges but fall-off very
                // quickly.
                float I = exp2(-2 * dist * dist);

                // Fade out the alpha but not the color so we don't get any weird halo effects from
                // a fade to a different color.
                float4 color = I * _WireColor + (1 - I) * _BaseColor;
                color.a = I;
                return color;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}

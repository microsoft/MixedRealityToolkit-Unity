///
/// Basic wireframe shader that can be used for rendering spatial mapping meshes.
///
Shader "HoloToolkit/Wireframe"
{
    Properties
    {
        _BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WireThickness("Wire thickness", Range(0, 800)) = 100
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

            // We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
            #pragma target 5.0
            #pragma only_renderers d3d11

            #include "UnityCG.cginc"

            float4 _BaseColor;
            float4 _WireColor;
            float _WireThickness;

            // Based on approach described in Shader-Based Wireframe Drawing (2008)
            // http://orbit.dtu.dk/en/publications/id(13e2122d-bec7-48de-beca-03ce6ea1c3f1).html

            struct v2g
            {
                float4 viewPos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2g vert(appdata_base v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2g o;
                o.viewPos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                return o;
            }

            // inverseW is to counteract the effect of perspective-correct interpolation so that the lines
            // look the same thickness regardless of their depth in the scene.
            struct g2f
            {
                float4 viewPos : SV_POSITION;
                float inverseW : TEXCOORD0;
                float3 dist : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
            {
                // Calculate the vectors that define the triangle from the input points.
                float2 point0 = i[0].viewPos.xy / i[0].viewPos.w;
                float2 point1 = i[1].viewPos.xy / i[1].viewPos.w;
                float2 point2 = i[2].viewPos.xy / i[2].viewPos.w;

                // Calculate the area of the triangle.
                float2 vector0 = point2 - point1;
                float2 vector1 = point2 - point0;
                float2 vector2 = point1 - point0;
                float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

                float3 distScale[3];
                distScale[0] = float3(area / length(vector0), 0, 0);
                distScale[1] = float3(0, area / length(vector1), 0);
                distScale[2] = float3(0, 0, area / length(vector2));

                float wireScale = 800 - _WireThickness;

                // Output each original vertex with its distance to the opposing line defined
                // by the other two vertices.
                g2f o;

                [unroll]
                for (uint idx = 0; idx < 3; ++idx)
                {
                   o.viewPos = i[idx].viewPos;
                   o.inverseW = 1.0 / o.viewPos.w;
                   o.dist = distScale[idx] * o.viewPos.w * wireScale;
                   UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(i[idx], o);
                   triStream.Append(o);
                }
            }

            float4 frag(g2f i) : COLOR
            {
                // Calculate  minimum distance to one of the triangle lines, making sure to correct
                // for perspective-correct interpolation.
                float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;

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

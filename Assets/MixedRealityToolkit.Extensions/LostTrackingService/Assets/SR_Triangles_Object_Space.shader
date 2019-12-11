// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "SR_Triangles_Object_Space" {
    Properties {
        [Header(Fill)]
            _Intensity_("Intensity", Range(0,5)) = 3
            _Fill_Color_("Fill Color", Color) = (0.004,0.004,0.004,1)
            _Vary_Color_("Vary Color", Range(0,1)) = 0.75
            [Toggle(_COLOR_MAP_ENABLE_)] _Color_Map_Enable_("Color Map Enable", Float) = 1
            [NoScaleOffset] _Color_Map_("Color Map", 2D) = "" {}
            _Vary_UV_("Vary UV", Range(0,1)) = 1
            _Fill_Start_Time_("Fill Start Time", Range(0,1)) = 0.5

        [Header(Lines)]
            _Line_Color_("Line Color", Color) = (0.639,0.639,0.639,1)
            _Line_Width_("Line Width", Range(0,10)) = 0.5
            _Line_Width_Fade_("Line Width Fade", Range(0,1)) = 0.25
            _Line_Width_Fade_Far_("Line Width Fade Far", Range(0,30)) = 5
            _Filter_Width_("Filter Width", Range(1,5)) = 1.5
            _Draw_Fuzz_("Draw Fuzz", Range(0,1)) = 0.2
            _Draw_End_Time_("Draw End Time", Range(0,1)) = 0.5

        [Header(Pulse Driver)]
            _Pulse_("Pulse", Range(0,1)) = 0.103
            _Pulse_Origin_("Pulse Origin", Vector) = (0, 0, 0, 1)
            [Toggle] _Auto_Pulse_("Auto Pulse", Float) = 0
            _Period_("Period", Float) = 3.5

        [Header(Pulse)]
            [Toggle] _Pulse_Enabled_("Pulse Enabled", Float) = 1
            _Pulse_Outer_Size_("Pulse Outer Size", Range(0,30)) = 10
            _Pulse_Outer_Fuzz_("Pulse Outer Fuzz", Range(0,5)) = 1
            _Pulse_Lead_Fuzz_("Pulse Lead Fuzz", Range(0,5)) = 1.2
            _Pulse_Middle_("Pulse Middle", Range(0,5)) = 0
            _Pulse_Tail_Fuzz_("Pulse Tail Fuzz", Range(0,5)) = 2.4
            _Pulse_Vary_("Pulse Vary", Range(0,1)) = 0

        [Header(Color Pulse)]
            _Color_Center_("Color Center", Range(0,1)) = 0.4
            _Color_Leading_Fuzz_("Color Leading Fuzz", Range(0,1)) = 1
            _Color_Trailing_Fuzz_("Color Trailing Fuzz", Range(0,1)) = 1

        [Header(Noise)]
            _Noise_Frequency_("Noise Frequency", Range(0,1000)) = 40.5
    }

    SubShader {
        Tags{ "RenderType" = "Opaque" }
        Blend Off
        Tags {"DisableBatching" = "True"}

        LOD 100

        Pass {
            CGPROGRAM

            #pragma vertex vertex_main
            #pragma fragment fragment_main
            #pragma geometry geometry_main
            #pragma multi_compile_instancing
            #pragma multi_compile _ _COLOR_MAP_ENABLE_

            #include "UnityCG.cginc"

            float _Noise_Frequency_;
            float _Intensity_;
            float4 _Fill_Color_;
            float _Vary_Color_;
            sampler2D _Color_Map_;
            float _Vary_UV_;
            float _Fill_Start_Time_;
            bool _Pulse_Enabled_;
            float _Pulse_Outer_Size_;
            float _Pulse_Outer_Fuzz_;
            float _Pulse_Lead_Fuzz_;
            float _Pulse_Middle_;
            float _Pulse_Tail_Fuzz_;
            float _Pulse_Vary_;
            float _Color_Center_;
            float _Color_Leading_Fuzz_;
            float _Color_Trailing_Fuzz_;
            float4 _Line_Color_;
            float _Line_Width_;
            float _Line_Width_Fade_;
            float _Line_Width_Fade_Far_;
            float _Filter_Width_;
            float _Draw_Fuzz_;
            float _Draw_End_Time_;
            float _Pulse_;
            float3 _Pulse_Origin_;
            bool _Auto_Pulse_;
            float _Period_;

            struct VertexInput {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 posWorld : TEXCOORD8;
                float4 extra1 : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct FragmentInput {
                float4 pos : SV_POSITION;
                float3 posWorld : TEXCOORD8;
                float4 extra1 : TEXCOORD4;
                float4 extra2 : TEXCOORD5;
                float4 extra3 : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #define Alpha_Blend 0
            #define No_Depth_Write 0
            #define Double_Sided 0
            #define Geo_Max_Out_Vertices 3

            FragmentInput vxOut[Geo_Max_Out_Vertices];
            int stripVxCount;
            int vxOutCount;

            //BLOCK_BEGIN Object_To_World_Pos 144

            void Object_To_World_Pos_B144(
                float3 Pos_Object,
                out float3 Pos_World)
            {
                Pos_World = (mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
            }

            //BLOCK_END Object_To_World_Pos

            //BLOCK_BEGIN To_XYZ 149

            void To_XYZ_B149(
                float3 Vec3,
                out float X,
                out float Y,
                out float Z)
            {
                X = Vec3.x;
                Y = Vec3.y;
                Z = Vec3.z;
            }

            //BLOCK_END To_XYZ

            #define HUX_VIEW_TO_WORLD_DIR(V) (mul(transpose(UNITY_MATRIX_V), float4(V,0)).xyz)

            VertexOutput vertex_main(VertexInput vertInput)
            {
                UNITY_SETUP_INSTANCE_ID(vertInput);
                VertexOutput o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(vertInput, o);

                float3 Pos_World_Q144;
                Object_To_World_Pos_B144(vertInput.vertex.xyz,Pos_World_Q144);

                float X_Q149;
                float Y_Q149;
                float Z_Q149;
                To_XYZ_B149(vertInput.vertex.xyz,X_Q149,Y_Q149,Z_Q149);

                // From_XYZW
                float4 Vec4_Q148 = float4(X_Q149, Y_Q149, Z_Q149, 0);

                float3 Position = Pos_World_Q144;
                float3 Normal = float3(0,0,0);
                float2 UV = float2(0,0);
                float3 Tangent = float3(0,0,0);
                float3 Binormal = float3(0,0,0);
                float4 Color = float4(1,1,1,1);
                float4 Extra1 = Vec4_Q148;
                float4 Extra2 = float4(0,0,0,0);

                o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
                o.posWorld = Position;
                o.extra1 = Extra1;
                return o;
            }

            #ifndef HUX_GEO_SET_UV
                #define HUX_GEO_SET_UV(A)
            #endif

            #ifndef HUX_GEO_SET_NORMAL
                #define HUX_GEO_SET_NORMAL(A)
            #endif

            #ifndef HUX_GEO_SET_TANGENT
                #define HUX_GEO_SET_TANGENT(A)
            #endif

            #ifndef HUX_GEO_SET_COLOR
                #define HUX_GEO_SET_COLOR(A)
            #endif

            #define HUX_GEO_SET_EXTRA1(A) vxOut[vxOutCount].extra1=A;
            #ifndef HUX_GEO_SET_EXTRA1
                #define HUX_GEO_SET_EXTRA1(A)
            #endif

            #define HUX_GEO_SET_EXTRA2(A) vxOut[vxOutCount].extra2=A;
            #ifndef HUX_GEO_SET_EXTRA2
                #define HUX_GEO_SET_EXTRA2(A)
            #endif

            #define HUX_GEO_SET_EXTRA3(A) vxOut[vxOutCount].extra3=A;
            #ifndef HUX_GEO_SET_EXTRA3
                #define HUX_GEO_SET_EXTRA3(A)
            #endif

            //BLOCK_BEGIN Emit_Triangle 121

            void emitVertex_Bid121(float3 P, float4 Extra1, float4 Extra2, float4 Extra3)
            {
              vxOut[vxOutCount].posWorld = P; vxOut[vxOutCount].pos = mul(unity_MatrixVP, float4(P,1.0f));;
              HUX_GEO_SET_EXTRA1(Extra1);
              HUX_GEO_SET_EXTRA2(Extra2);
              HUX_GEO_SET_EXTRA3(Extra3);
              vxOutCount += 1; stripVxCount += 1;
            }

            void Emit_Triangle_B121(
                bool Previous,
                float3 P1,
                float3 P2,
                float3 P3,
                float4 Extra1_1,
                float4 Extra1_2,
                float4 Extra1_3,
                float4 Color,
                float Width,
                out bool Next)
            {
                // This function can be called at most once per geometry_main invocation.
                // If this is called multiple times, stripVxCount must be augmented as an
                // array (see https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6609
                // for more details).
                emitVertex_Bid121(P1,Extra1_1,Color,float4(1.0,0.0,0.0,Width));
                emitVertex_Bid121(P2,Extra1_2,Color,float4(0.0,1.0,0.0,Width));
                emitVertex_Bid121(P3,Extra1_3,Color,float4(0.0,0.0,1.0,Width));
                Next = Previous;
            }

            //BLOCK_END Emit_Triangle

            //BLOCK_BEGIN Find_Nearest 130

            void Find_Nearest_B130(
                float3 P1,
                float3 P2,
                float3 P3,
                float Transition,
                bool FadingOut,
                float3 Distances,
                out float3 Nearest,
                out float4 Extra2_1,
                out float4 Extra2_2,
                out float4 Extra2_3)
            {
                if (FadingOut) {
                    if (Distances.x > Distances.y&& Distances.x > Distances.z) {
                        Extra2_1 = float4(1.0,0.0,0.0,0.0);
                        Extra2_2 = float4(0.0,0.0,0.0,0.0);
                        Extra2_3 = float4(0.0,0.0,0.0,0.0);
                        Nearest = P1;
                    }
                    else if (Distances.y > Distances.z) {
                        Extra2_1 = float4(0.0,0.0,0.0,0.0);
                        Extra2_2 = float4(1.0,0.0,0.0,0.0);
                        Extra2_3 = float4(0.0,0.0,0.0,0.0);
                        Nearest = P2;
                    }
                    else {
                        Extra2_1 = float4(0.0,0.0,0.0,0.0);
                        Extra2_2 = float4(0.0,0.0,0.0,0.0);
                        Extra2_3 = float4(1.0,0.0,0.0,0.0);
                        Nearest = P3;
                    }
                }
                else {
                    if (Distances.x < Distances.y && Distances.x < Distances.z) {
                        Extra2_1 = float4(1.0,0.0,0.0,0.0);
                        Extra2_2 = float4(0.0,0.0,0.0,0.0);
                        Extra2_3 = float4(0.0,0.0,0.0,0.0);
                        Nearest = P1;
                    }
                    else if (Distances.y < Distances.z) {
                        Extra2_1 = float4(0.0,0.0,0.0,0.0);
                        Extra2_2 = float4(1.0,0.0,0.0,0.0);
                        Extra2_3 = float4(0.0,0.0,0.0,0.0);
                        Nearest = P2;
                    }
                    else {
                        Extra2_1 = float4(0.0,0.0,0.0,0.0);
                        Extra2_2 = float4(0.0,0.0,0.0,0.0);
                        Extra2_3 = float4(1.0,0.0,0.0,0.0);
                        Nearest = P3;
                    }
                }

                Extra2_1.a = Transition;
                Extra2_2.a = Transition;
                Extra2_3.a = Transition;
            }

            //BLOCK_END Find_Nearest

            //BLOCK_BEGIN Calc_Width 125

            void Calc_Width_B125(
                float3 Eye,
                float3 Position,
                float Base_Width,
                float Width_Fade,
                float Fade_Far_Distance,
                out float Width)
            {

                float d = saturate(distance(Eye,Position) / Fade_Far_Distance);
                Width = Base_Width * (1.0 - d + d * Width_Fade);

            }

            //BLOCK_END Calc_Width

            //BLOCK_BEGIN Pulse 124

            float ramp_Bid124(float start, float end, float x)
            {
                return saturate((x - start) / (end - start));
            }

            void Pulse_B124(
                float Distance,
                float Noise,
                bool Pulse_Enabled,
                float Pulse,
                float Pulse_Middle,
                float Pulse_Outer_Size,
                float Pulse_Lead_Fuzz,
                float Pulse_Tail_Fuzz,
                float Pulse_Vary,
                float Pulse_Outer_Fuzz,
                float Color_Center,
                float Color_Leading_Fuzz,
                float Color_Trailing_Fuzz,
                out float Transition,
                out bool FadingOut,
                out float Fade_Color,
                out float Colorize)
            {
                Transition = 1.0;
                Fade_Color = 1.0;
                Colorize = 0.0;
                FadingOut = false;
                if (Pulse_Enabled) {
                    float d = Distance + Pulse_Vary * Noise;

                    float Pulse_Width = Pulse_Lead_Fuzz + Pulse_Middle + Pulse_Tail_Fuzz;
                    float totalSize = Pulse_Outer_Size + Pulse_Vary + Pulse_Width;
                    float pulseFront = Pulse * totalSize;

                    float edge1 = pulseFront - Pulse_Lead_Fuzz;
                    float fadeIn = ramp_Bid124(pulseFront,edge1,d);
                    float fadeOut = ramp_Bid124(pulseFront - Pulse_Width + Pulse_Tail_Fuzz,pulseFront - Pulse_Width,d);
                    float outerFade = 1.0 - ramp_Bid124(Pulse_Outer_Size - Pulse_Outer_Fuzz,Pulse_Outer_Size,d);

                    float colorCenter = pulseFront - Pulse_Width * Color_Center;
                    Colorize = (ramp_Bid124(colorCenter - Pulse_Width * Color_Trailing_Fuzz,colorCenter,d)) - ramp_Bid124(colorCenter,colorCenter + Pulse_Width * Color_Leading_Fuzz,d);

                    Transition = saturate(fadeIn * (1.0 - fadeOut)) * outerFade;
                    FadingOut = fadeOut > 0.0;
                    Fade_Color = (1.0 - fadeOut) * outerFade;
                }
            }

            //BLOCK_END Pulse

            //BLOCK_BEGIN Pick_Reference 129

            void Pick_Reference_B129(
                float3 VA,
                float3 VB,
                float3 VC,
                float3 Pulse_Origin,
                float4 Extra1_1,
                float4 Extra1_2,
                float4 Extra1_3,
                out float3 Reference,
                out float Distance,
                out float3 Distances_Sqrd)
            {
                Distances_Sqrd.x = dot(VA - Pulse_Origin,VA - Pulse_Origin);
                Distances_Sqrd.y = dot(VB - Pulse_Origin,VB - Pulse_Origin);
                Distances_Sqrd.z = dot(VC - Pulse_Origin,VC - Pulse_Origin);

                if (Distances_Sqrd.x < Distances_Sqrd.y && Distances_Sqrd.x < Distances_Sqrd.z) {
                    Reference = Extra1_1.xyz;
                    Distance = sqrt(Distances_Sqrd.x);
                }
                else if (Distances_Sqrd.y < Distances_Sqrd.z) {
                    Reference = Extra1_2.xyz;
                    Distance = sqrt(Distances_Sqrd.y);
                }
                else {
                    Reference = Extra1_3.xyz;
                    Distance = sqrt(Distances_Sqrd.z);
                }
            }

            //BLOCK_END Pick_Reference

            //BLOCK_BEGIN Noise 120

            void Noise_B120(
                float3 P,
                float Frequency,
                out float Noise1,
                out float Noise2)
            {
                float3 P1 = P * Frequency;
                float3 P2 = P1 * float3(-1.0,2.0,3.0);
                Noise1 = frac(P1.x + P1.y + P1.z);
                Noise2 = frac(P2.x + P2.y + P2.z);
            }

            //BLOCK_END Noise

            //BLOCK_BEGIN Pt_Sample_Texture 117

            void Pt_Sample_Texture_B117(
                float Noise,
                sampler2D Texture,
                float Vary_UV,
                float Map_Intensity,
                out float4 Color)
            {
                float2 xy = float2(0.5,0.5) + float2(Noise - 0.5,Noise - 0.5) * Vary_UV;
                Color = tex2Dlod(Texture,float4(xy.x,xy.y,0.0,0.0));
            }

            //BLOCK_END Pt_Sample_Texture

            //BLOCK_BEGIN AutoPulse 113

            void AutoPulse_B113(
                float Pulse,
                bool Auto_Pulse,
                float Period,
                float Time,
                out float Result)
            {

                if (Auto_Pulse) {
                    Result = frac(Time / Period);
                }
                else {
                    Result = Pulse;
                }
            }

            //BLOCK_END AutoPulse

            [maxvertexcount(Geo_Max_Out_Vertices)]
            void geometry_main(triangle VertexOutput vxIn[3], inout TriangleStream<FragmentInput> triStream)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(vxIn[0]);
                vxOutCount = 0;
                stripVxCount = 0;

                float3 Reference_Q129;
                float Distance_Q129;
                float3 Distances_Sqrd_Q129;
                Pick_Reference_B129(vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,_Pulse_Origin_,vxIn[0].extra1,vxIn[1].extra1,vxIn[2].extra1,Reference_Q129,Distance_Q129,Distances_Sqrd_Q129);

                float Noise1_Q120;
                float Noise2_Q120;
                Noise_B120(Reference_Q129,_Noise_Frequency_,Noise1_Q120,Noise2_Q120);

                float4 Color_Q117;
                #if defined(_COLOR_MAP_ENABLE_)
                    Pt_Sample_Texture_B117(Noise2_Q120,_Color_Map_,_Vary_UV_,1,Color_Q117);
                #else
                    Color_Q117 = float4(1,1,1,1);
                #endif

                float Result_Q113;
                AutoPulse_B113(_Pulse_,_Auto_Pulse_,_Period_,_Time.y,Result_Q113);

                float Width_Q125;
                Calc_Width_B125(_WorldSpaceCameraPos,Reference_Q129,_Line_Width_,_Line_Width_Fade_,_Line_Width_Fade_Far_,Width_Q125);

                float Transition_Q124;
                bool FadingOut_Q124;
                float Fade_Color_Q124;
                float Colorize_Q124;
                Pulse_B124(Distance_Q129,Noise1_Q120,_Pulse_Enabled_,Result_Q113,_Pulse_Middle_,_Pulse_Outer_Size_,_Pulse_Lead_Fuzz_,_Pulse_Tail_Fuzz_,_Pulse_Vary_,_Pulse_Outer_Fuzz_,_Color_Center_,_Color_Leading_Fuzz_,_Color_Trailing_Fuzz_,Transition_Q124,FadingOut_Q124,Fade_Color_Q124,Colorize_Q124);

                float3 Nearest_Q130;
                float4 Extra2_1_Q130;
                float4 Extra2_2_Q130;
                float4 Extra2_3_Q130;
                Find_Nearest_B130(vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,Transition_Q124,FadingOut_Q124,Distances_Sqrd_Q129,Nearest_Q130,Extra2_1_Q130,Extra2_2_Q130,Extra2_3_Q130);

                // Color
                float4 Result_Q122;
                Result_Q122 = lerp(_Fill_Color_,Color_Q117 * _Intensity_,float4(Colorize_Q124,Colorize_Q124,Colorize_Q124,Colorize_Q124)) * (1.0 - _Vary_Color_ * Noise1_Q120) * Fade_Color_Q124;
                Result_Q122.a = Fade_Color_Q124;

                bool Next_Q121;
                Emit_Triangle_B121(false,vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,Extra2_1_Q130,Extra2_2_Q130,Extra2_3_Q130,Result_Q122,Width_Q125,Next_Q121);

                bool Root = Next_Q121;

                int vxix = 0;
                int i = 0;
                while (i < stripVxCount) {
                    UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(vxIn[0],vxOut[vxix]);
                    triStream.Append(vxOut[vxix]);
                    i += 1; vxix += 1;
                }
                triStream.RestartStrip();
            }

            //BLOCK_BEGIN Main_Fragment 134

            void Main_Fragment_B134(
                float Fill_Start_Time,
                float4 Line_Color_Base,
                float Fuzz,
                float Line_End_Time,
                float4 Transition,
                float4 Fill_Color_In,
                float4 V,
                float In_Line,
                out float4 Fill_Color,
                out float4 Line_Color,
                out float4 Final_Color)
            {
                half fillProgress = saturate((Transition.w - Fill_Start_Time) / (1.0 - Fill_Start_Time));

                Fill_Color.rgb = Fill_Color_In.rgb * fillProgress;
                Fill_Color.a = fillProgress;

                half lineProgress = saturate(Transition.w / Line_End_Time);
                half Edge = 1.0 - lineProgress;
                half k = Edge * (1.0 + Fuzz) - Fuzz;
                half kk = saturate(Transition.x);
                half lineFade = saturate(smoothstep(k, k + Fuzz, kk));
                Line_Color = Line_Color_Base * lineFade * Fill_Color_In.w;

                Final_Color = lerp(Fill_Color, Line_Color,float4(In_Line, In_Line, In_Line, In_Line));
            }

            //BLOCK_END Main_Fragment

            //BLOCK_BEGIN Edges 126

            float3 FilterStep_Bid126(float3 edge, float3 x, float3 dx)
            {
                return (x + dx * 0.5 - max(edge,x - dx * 0.5)) / max(dx,1.0E-8);
            }

            void Edges_B126(
                float Edge_Width,
                float Filter_Width,
                float4 Edges,
                out half inLine)
            {
                float3 fw = fwidth(Edges.xyz);
                float3 a = FilterStep_Bid126(fw * Edges.w,Edges.xyz,fw * Filter_Width);
                inLine = saturate((1.0 - min(a.x,min(a.y,a.z))));
            }

            //BLOCK_END Edges

            fixed4 fragment_main(FragmentInput fragInput) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(fragInput);
                float4 result;

                half inLine_Q126;
                Edges_B126(_Line_Width_,_Filter_Width_,fragInput.extra3,inLine_Q126);

                float4 Fill_Color_Q134;
                float4 Line_Color_Q134;
                float4 Final_Color_Q134;
                Main_Fragment_B134(_Fill_Start_Time_,_Line_Color_,_Draw_Fuzz_,_Draw_End_Time_,fragInput.extra1,fragInput.extra2,fragInput.extra3,inLine_Q126,Fill_Color_Q134,Line_Color_Q134,Final_Color_Q134);

                float4 Out_Color = Final_Color_Q134;
                float Clip_Threshold = 0;

                result = Out_Color;

                return result;
            }

            ENDCG
        }
    }
}

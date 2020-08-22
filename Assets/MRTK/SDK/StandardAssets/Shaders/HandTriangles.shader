// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <summary>
/// Note, this shader is generated from a tool and is not formated for user readability.
/// </summary>

Shader "Mixed Reality Toolkit/HandTriangles" {

Properties {

    [Header(Colors)]
        _Intensity_("Intensity", Range(0,5)) = 5
        _Fill_Color_("Fill Color", Color) = (0.317647,0.317647,0.317647,1)
        _Line_Color_("Line Color", Color) = (0.717647,0.717647,0.717647,1)
        [Toggle(USE_ALBEDO_TEXTURE)] USE_ALBEDO_TEXTURE("USE ALBEDO TEXTURE", Float) = 1
        [NoScaleOffset] _Color_Map_("Color Map", 2D) = "" {}
        _Vary_UV_("Vary UV", Range(0,1)) = 0.71
        _Vary_Color_("Vary Color", Range(0,1)) = 0.7
        _Desaturated_Intensity_("Desaturated Intensity", Range(0,1)) = 0
     
    [Header(Edges)]
        _Edge_Width_("Edge Width", Range(0,10)) = 1
        _Filter_Width_("Filter Width", Range(1,5)) = 1.5
     
    [Header(Pulse)]
        [Toggle] _Pulse_Enabled_("Pulse Enabled", Float) = 1
        _Pulse_("Pulse", Range(0,1)) = 0.346
        _Pulse_Width_("Pulse Width", Range(0,5)) = 1
        _Pulse_Outer_Size_("Pulse Outer Size", Range(0,2)) = 1.05
        _Pulse_Lead_Fuzz_("Pulse Lead Fuzz", Range(0,1)) = 0.67
        _Pulse_Tail_Fuzz_("Pulse Tail Fuzz", Range(0,1)) = 0.8
        _Pulse_Vary_("Pulse Vary", Range(0,1)) = 0.075
        _Pulse_Line_Fuzz_("Pulse Line Fuzz", Range(0.01,1)) = 0.2
        _Pulse_Noise_Frequency_("Pulse Noise Frequency", Range(0,2000)) = 777
        _Pulse_Origin_("Pulse Origin", Vector) = (0.5, 0, 0, 0)
        _Pulse_Color_Width_("Pulse Color Width", Range(0,1)) = 1
        _Pulse_Amplify_Leading_("Pulse Amplify Leading", Range(0,2)) = 0
     
    [Header(AutoPulse)]
        [Toggle] _Auto_Pulse_("Auto Pulse", Float) = 1
        _Period_("Period", Float) = 2.7
     
    [Header(Edge Timing)]
        _Line_End_Time_("Line End Time", Range(0,1)) = 0.5
        _Fill_Start_Time_("Fill Start Time", Range(0,1)) = 0.5
     
    [Header(Wrist Fade)]
        _Wrist_Fade_Start_("Wrist Fade Start", Range(0,1)) = 0.1
        _Wrist_Fade_End_("Wrist Fade End", Range(0,1)) = 0.16
     
    [Header(Flip V For Hydrogen)]
        [Toggle] _Flip_V_("Flip V", Float) = 0
     
    [Header(Fly)]
        _Max_Hover_("Max Hover", Range(0,1)) = 0.004
        _Max_In_Angle_("Max In Angle", Range(0,2)) = 0.6
        _Max_Out_Angle_("Max Out Angle", Range(0,2)) = 0.4
     

}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    ZWrite Off
    Tags {"DisableBatching" = "True"}

    LOD 100


  Pass

  {

  CGPROGRAM

    #pragma vertex vertex_main
    #pragma fragment fragment_main
    #pragma geometry geometry_main
    #pragma multi_compile _ USE_ALBEDO_TEXTURE

    #include "UnityCG.cginc"

    float _Edge_Width_;
    float _Filter_Width_;
    bool _Flip_V_;
    float _Wrist_Fade_Start_;
    float _Wrist_Fade_End_;
    float _Intensity_;
    float4 _Fill_Color_;
    float4 _Line_Color_;
    //bool USE_ALBEDO_TEXTURE;
    sampler2D _Color_Map_;
    float _Vary_UV_;
    float _Vary_Color_;
    float _Desaturated_Intensity_;
    float _Line_End_Time_;
    float _Fill_Start_Time_;
    float _Max_Hover_;
    float _Max_In_Angle_;
    float _Max_Out_Angle_;
    bool _Pulse_Enabled_;
    float _Pulse_;
    float _Pulse_Width_;
    float _Pulse_Outer_Size_;
    float _Pulse_Lead_Fuzz_;
    float _Pulse_Tail_Fuzz_;
    float _Pulse_Vary_;
    float _Pulse_Line_Fuzz_;
    float _Pulse_Noise_Frequency_;
    float4 _Pulse_Origin_;
    float _Pulse_Color_Width_;
    float _Pulse_Amplify_Leading_;
    bool _Auto_Pulse_;
    float _Period_;


    struct VertexInput {
        float4 vertex : POSITION;
        float2 uv0 : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        float3 posWorld : TEXCOORD8;
        float2 uv : TEXCOORD0;
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

    #define Double_Sided 0
    #define Alpha_Blend 2
    #define No_Depth_Write 1

    #ifndef Geo_Max_Out_Vertices
    #define Geo_Max_Out_Vertices 16
    #endif

    FragmentInput vxOut[Geo_Max_Out_Vertices];
    int stripVxCount[Geo_Max_Out_Vertices];
    int vxOutCount;
    int stripCount;


    #define HUX_VIEW_TO_WORLD_DIR(V) (mul(transpose(UNITY_MATRIX_V), float4(V,0)).xyz)

    VertexOutput vertex_main(VertexInput v)
    {
        VertexOutput o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        UNITY_TRANSFER_INSTANCE_ID(v, o);


        o.pos = UnityObjectToClipPos(v.vertex);
        float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
        o.posWorld = posWorld.xyz;

        o.uv = v.uv0;

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

    //BLOCK_BEGIN Emit_Triangle 177

    void emitVertex_Bid177(float3 P, float4 extra1, float4 C, float4 extra3)
    {
      vxOut[vxOutCount].posWorld=P; vxOut[vxOutCount].pos=mul(unity_MatrixVP, float4(P,1.0f));;
      HUX_GEO_SET_EXTRA1(extra1);
      HUX_GEO_SET_EXTRA2(C);
      HUX_GEO_SET_EXTRA3(extra3);
      vxOutCount+=1; stripVxCount[stripCount]+=1;
    }
    
    void Emit_Triangle_B177(
        bool Previous,
        float3 P1,
        float3 P2,
        float3 P3,
        float4 Extra1_1,
        float4 Extra1_2,
        float4 Extra1_3,
        float4 Color,
        float Wrist_1,
        float Wrist_2,
        float Wrist_3,
        float Transition,
        out bool Next    )
    {
        
        float2 uv;
        
        float t = Transition>0.0 ? 1.0 : 0.0;
        float3 p2 = P1 + (P2-P1)*t;
        float3 p3 = P1 + (P3-P1)*t;
        
        uv = float2(0,0);
        emitVertex_Bid177(P1,Extra1_1,Color*Wrist_1,float4(1.0,0.0,0.0,0.0));
        
        uv = float2(1,0);
        emitVertex_Bid177(p2,Extra1_2,Color*Wrist_2,float4(0.0,1.0,0.0,0.0));
        
        uv = float2(0,1);
        emitVertex_Bid177(p3,Extra1_3,Color*Wrist_3,float4(0.0,0.0,1.0,0.0));
        
        stripCount+=1; stripVxCount[stripCount]=0;
        
        Next = Previous;
        
    }
    //BLOCK_END Emit_Triangle

    //BLOCK_BEGIN Fly 164

    float3 Rotate_Bid164(float A, float3 Center, float3 Axis, float3 XYZ)
    {
        float cosA = cos(A);
        float sinA = sin(A);
    
        float3 v = XYZ - Center;
    
        float ux = Axis.x;
        float uy = Axis.y;
        float uz = Axis.z;
    
        float r00 = cosA + ux*ux*(1-cosA);
        float r10 = uy*ux*(1-cosA)+uz*sinA;
        float r20 = uz*ux*(1-cosA)-uy*sinA;
        
        float r01 = ux*uy*(1-cosA)-uz*sinA;
        float r11 = cosA+uy*uy*(1-cosA);
        float r21 = uz*uy*(1-cosA)+ux*sinA;
    
        float r02 = ux*uz*(1-cosA)+uy*sinA;
        float r12 = uy*uz*(1-cosA)-ux*sinA;
        float r22 = cosA+uz*uz*(1-cosA);
    
        float rot_x = dot(v,float3(r00,r10,r20));
        float rot_y = dot(v,float3(r01,r11,r21));
        float rot_z = dot(v,float3(r02,r12,r22));
    
        return float3(rot_x,rot_y,rot_z) + Center;
    }
    
    void Fly_B164(
        float3 P0,
        float3 P1,
        float3 P2,
        float Transition,
        float Max_Hover,
        float Max_In_Angle,
        float Max_Out_Angle,
        float2 UV0,
        float2 UV1,
        float2 UV2,
        float2 PulseOrigin,
        float3 Nearest_P,
        bool Fading_Out,
        out float3 Q0,
        out float3 Q1,
        out float3 Q2    )
    {
        float3 N = normalize(cross(P1-P0,P2-P1));
        
        float k01 = dot(normalize((UV0+UV1)*0.5-PulseOrigin),normalize(UV1-UV0));
        float k12 = dot(normalize((UV1+UV2)*0.5-PulseOrigin),normalize(UV2-UV1));
        float k20 = dot(normalize((UV0+UV2)*0.5-PulseOrigin),normalize(UV2-UV0));
        
        float3 pulseDir = normalize( normalize(P1-P0)*k01+normalize(P2-P1)*k12 + normalize(P2-P0)*k20);
        float3 axis = normalize(cross(N,pulseDir));
        float3 center = Nearest_P; //(P0+P1+P2)/3.0;
        
        float angle, k;
        if (Fading_Out) {
            //float t = Transition<0.5 ? 2.0*(0.5-Transition)*(0.5-Transition) : 2.0*(0.5-Transition)*(0.5-Transition)+0.5;
            float t = smoothstep(0,1,Transition);
            angle = -Max_Out_Angle * (1.0-t);
            k = (1-t) * Max_Hover;
        } else {
            float t = smoothstep(0,1,Transition);
        //    float t = Transition*Transition;
            angle = Max_In_Angle * (1.0-t);
            k = (1-Transition) * Max_Hover;
        }
        
        float3 p0 = Rotate_Bid164(angle,center,axis,P0);
        float3 p1 = Rotate_Bid164(angle,center,axis,P1);
        float3 p2 = Rotate_Bid164(angle,center,axis,P2);
        
        if (false) {  ///(!Fading_Out) {
            float t = (Transition);
            p0 = Nearest_P + (p0-Nearest_P)*t;
            p1 = Nearest_P + (p1-Nearest_P)*t;
            p2 = Nearest_P + (p2-Nearest_P)*t;
        }
        
        Q0 = p0 + N * k;
        Q1 = p1 + N * k;
        Q2 = p2 + N * k;
        
        
    }
    //BLOCK_END Fly

    //BLOCK_BEGIN Find_Nearest 163

    void Find_Nearest_B163(
        float2 UV1,
        float2 UV2,
        float2 UV3,
        float2 Pulse_Origin,
        float Transition,
        bool FadingOut,
        float3 P1,
        float3 P2,
        float3 P3,
        out float4 Extra1_1,
        out float4 Extra1_2,
        out float4 Extra1_3,
        out float3 Nearest_P    )
    {
        float d1 = distance(UV1,Pulse_Origin);
        float d2 = distance(UV2,Pulse_Origin);
        float d3 = distance(UV3,Pulse_Origin);
        
        Extra1_1 = float4(0.0,0.0,0.0,Transition);
        Extra1_2 = float4(0.0,0.0,0.0,Transition);
        Extra1_3 = float4(0.0,0.0,0.0,Transition);
        
        if  (FadingOut) {
            if (d1>d2 && d1>d3) {
                Extra1_1.x=1.0;
                Nearest_P = P1;
            } else if (d2>d3) {
                Extra1_2.x=1.0;
                Nearest_P = P2;
            } else {
                Extra1_3.x=1.0;
                Nearest_P = P3;
            }
        } else {
            if (d1<d2 && d1<d3) {
                Nearest_P = P1;
                Extra1_1.x=1.0;
            } else if (d2<d3) {
                Extra1_2.x=1.0;
                Nearest_P = P2;
            } else {
                Extra1_3.x=1.0;
                Nearest_P = P3;
            }
        }
        
    }
    //BLOCK_END Find_Nearest

    //BLOCK_BEGIN Average 153

    void Average_B153(
        float2 A2,
        float2 B2,
        float2 C2,
        float3 P1,
        float3 P2,
        float3 P3,
        float Wrist_Start,
        float Wrist_End,
        out float2 Average,
        out float Wrist_1,
        out float Wrist_2,
        out float Wrist_3    )
    {
        Average = (A2 + B2 + C2) * (1.0/3.0);
        Wrist_1 = saturate((A2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
        Wrist_2 = saturate((B2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
        Wrist_3 = saturate((C2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
        
    }
    //BLOCK_END Average

    //BLOCK_BEGIN Pulse 160

    float ramp_Bid160(float start, float end, float x)
    {
    //    return saturate((x-start)/(end-start));
        return smoothstep(start,end,x);
    }
    
    void Pulse_B160(
        float Distance,
        float Noise,
        bool Pulse_Enabled,
        float Pulse,
        float Pulse_Width,
        float Pulse_Outer_Size,
        float Pulse_Lead_Fuzz,
        float Pulse_Tail_Fuzz,
        float Pulse_Vary,
        float Pulse_Saturation,
        out float Transition,
        out bool FadingOut,
        out float Saturation,
        out float Fade_Color    )
    {
        Transition = 1.0;
        Saturation = 1.0;
        Fade_Color = 1.0;
        
        if (Pulse_Enabled) {
            float d = Distance - Pulse_Vary*Noise; 
            
            float totalSize = Pulse_Outer_Size+Pulse_Vary+Pulse_Width;
            float pulseFront = Pulse * totalSize;
        
            float edge1 = pulseFront-Pulse_Width*0.5*Pulse_Lead_Fuzz;
            float fadeIn = saturate(1.0-ramp_Bid160(edge1,pulseFront,d));
            float fadeOut = saturate(1.0-ramp_Bid160(pulseFront-Pulse_Width,pulseFront-Pulse_Width+Pulse_Width*0.5*Pulse_Tail_Fuzz,d));
            Saturation = saturate(smoothstep(edge1-Pulse_Saturation,edge1,d));
        
            float clip = 1.0-step(Pulse_Outer_Size,d);
            
            Transition= saturate(fadeIn-fadeOut)*clip;
            FadingOut = fadeOut>0.0;
            Fade_Color = 1.0-fadeOut;
        }
        
    }
    //BLOCK_END Pulse

    //BLOCK_BEGIN Flip_V_For_Hydrogen 154

    void Flip_V_For_Hydrogen_B154(
        bool Flip_V,
        float2 UV_1,
        float2 UV_2,
        float2 UV_3,
        out float2 Out_UV_1,
        out float2 Out_UV_2,
        out float2 Out_UV_3    )
    {
        Out_UV_1 = Flip_V ? float2(UV_1.x,1.0-UV_1.y) : UV_1;
        Out_UV_2 = Flip_V ? float2(UV_2.x,1.0-UV_2.y) : UV_2;
        Out_UV_3 = Flip_V ? float2(UV_3.x,1.0-UV_3.y) : UV_3;
        
    }
    //BLOCK_END Flip_V_For_Hydrogen

    //BLOCK_BEGIN Cell_Noise_2D 150

    float2 mod289_Bid150(float2 x)
    {
      return x - floor(x * (1.0 / 289.0)) * 289.0;
    }
    
    float2 permute_Bid150(float2 x)
    {
      return mod289_Bid150(((x*float2(33.0,35.0))+1.0)*x);
    }
    
    float2 permuteB_Bid150(float2 x)
    {
      return mod289_Bid150(((x*float2(37.0,34.0))-1.0)*x);
    }
    
    
    void Cell_Noise_2D_B150(
        float2 XY,
        float Frequency,
        float Seed,
        out float Result    )
    {
        
        float2 P = XY * float2(Frequency,Frequency)+float2(Seed,Seed);
        float2 Pi = floor(P);
        
        Pi = mod289_Bid150(Pi); // To avoid truncation effects in permutation
        
        float2 ix = Pi.xy;
        float2 iy = Pi.yx;
        
        float2 i = permute_Bid150(permuteB_Bid150(ix) + iy);
        
        Result = frac(i.x*(1.0/41.0)+i.y*(1.0/42.0));
        
        //Result = lerp(Out_Min, Out_Max, r);
        
    }
    //BLOCK_END Cell_Noise_2D

    //BLOCK_BEGIN Pt_Sample_Texture 157

    void Pt_Sample_Texture_B157(
        float2 UV,
        float Noise,
        sampler2D Texture,
        float Vary_UV,
        float Map_Intensity,
        out float4 Color    )
    {
        float2 xy = UV + float2(Noise-0.5,Noise-0.5)*Vary_UV;
        Color = tex2D(Texture,xy,float2(0,0),float2(0,0))*Map_Intensity;
    }
    //BLOCK_END Pt_Sample_Texture

    //BLOCK_BEGIN AutoPulse 149

    void AutoPulse_B149(
        float Pulse,
        bool Auto_Pulse,
        float Period,
        float Time,
        out float Result    )
    {
        
        if (Auto_Pulse) {
            Result = frac(Time/Period);
        } else {
            Result = Pulse;
        }
    }
    //BLOCK_END AutoPulse

    //BLOCK_BEGIN Cell_Noise_2D 151

    float2 mod289_Bid151(float2 x)
    {
      return x - floor(x * (1.0 / 289.0)) * 289.0;
    }
    
    float2 permute_Bid151(float2 x)
    {
      return mod289_Bid151(((x*float2(33.0,35.0))+1.0)*x);
    }
    
    float2 permuteB_Bid151(float2 x)
    {
      return mod289_Bid151(((x*float2(37.0,34.0))-1.0)*x);
    }
    
    
    void Cell_Noise_2D_B151(
        float2 XY,
        float Frequency,
        float Seed,
        out float Result    )
    {
        
        float2 P = XY * float2(Frequency,Frequency)+float2(Seed,Seed);
        float2 Pi = floor(P);
        
        Pi = mod289_Bid151(Pi); // To avoid truncation effects in permutation
        
        float2 ix = Pi.xy;
        float2 iy = Pi.yx;
        
        float2 i = permute_Bid151(permuteB_Bid151(ix) + iy);
        
        Result = frac(i.x*(1.0/41.0)+i.y*(1.0/42.0));
        
        //Result = lerp(Out_Min, Out_Max, r);
        
    }
    //BLOCK_END Cell_Noise_2D


    [maxvertexcount(Geo_Max_Out_Vertices)]
    void geometry_main(triangle VertexOutput vxIn[3], inout TriangleStream<FragmentInput> triStream)
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(vxIn[0]);
        //huxEye = _WorldSpaceCameraPos;
        //workaround for Unity's auto updater in 5.6
        vxOutCount=0;
        stripCount=0;
        stripVxCount[0]=0;

        float2 Out_UV_1_Q154;
        float2 Out_UV_2_Q154;
        float2 Out_UV_3_Q154;
        Flip_V_For_Hydrogen_B154(_Flip_V_,vxIn[0].uv,vxIn[1].uv,vxIn[2].uv,Out_UV_1_Q154,Out_UV_2_Q154,Out_UV_3_Q154);

        float Result_Q149;
        AutoPulse_B149(_Pulse_,_Auto_Pulse_,_Period_,_Time.y,Result_Q149);

        // To_XYZW
        float X_Q166;
        float Y_Q166;
        float Z_Q166;
        float W_Q166;
        X_Q166=_Pulse_Origin_.x;
        Y_Q166=_Pulse_Origin_.y;
        Z_Q166=_Pulse_Origin_.z;
        W_Q166=_Pulse_Origin_.w;

        float2 Average_Q153;
        float Wrist_1_Q153;
        float Wrist_2_Q153;
        float Wrist_3_Q153;
        Average_B153(Out_UV_1_Q154,Out_UV_2_Q154,Out_UV_3_Q154,vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,_Wrist_Fade_Start_,_Wrist_Fade_End_,Average_Q153,Wrist_1_Q153,Wrist_2_Q153,Wrist_3_Q153);

        // From_XY
        float2 Vec2_Q167 = float2(X_Q166,Y_Q166);

        float Result_Q150;
        Cell_Noise_2D_B150(Average_Q153,_Pulse_Noise_Frequency_,111,Result_Q150);

        // Distance2
        float Distance_Q159 = distance(Average_Q153,Vec2_Q167);

        float Result_Q151;
        Cell_Noise_2D_B151(Average_Q153,_Pulse_Noise_Frequency_,333,Result_Q151);

        float Transition_Q160;
        bool FadingOut_Q160;
        float Saturation_Q160;
        float Fade_Color_Q160;
        Pulse_B160(Distance_Q159,Result_Q150,_Pulse_Enabled_,Result_Q149,_Pulse_Width_,_Pulse_Outer_Size_,_Pulse_Lead_Fuzz_,_Pulse_Tail_Fuzz_,_Pulse_Vary_,_Pulse_Color_Width_,Transition_Q160,FadingOut_Q160,Saturation_Q160,Fade_Color_Q160);

        float4 Color_Q157;
        #if defined(USE_ALBEDO_TEXTURE)
          Pt_Sample_Texture_B157(Average_Q153,Result_Q151,_Color_Map_,_Vary_UV_,1,Color_Q157);
        #else
          Color_Q157 = float4(1,1,1,1);
        #endif

        float4 Extra1_1_Q163;
        float4 Extra1_2_Q163;
        float4 Extra1_3_Q163;
        float3 Nearest_P_Q163;
        Find_Nearest_B163(Out_UV_1_Q154,Out_UV_2_Q154,Out_UV_3_Q154,Vec2_Q167,Transition_Q160,FadingOut_Q160,vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,Extra1_1_Q163,Extra1_2_Q163,Extra1_3_Q163,Nearest_P_Q163);

        // Color
        float4 Result_Q161;
        float k = max(Color_Q157.r,max(Color_Q157.g,Color_Q157.b))*_Desaturated_Intensity_;
        Result_Q161 = lerp(float4(k,k,k,1),Color_Q157,float4(Saturation_Q160,Saturation_Q160,Saturation_Q160,Saturation_Q160))*(1.0-_Vary_Color_*Result_Q150)*Fade_Color_Q160;
        Result_Q161.rgb *= _Intensity_;
        
        float3 Q0_Q164;
        float3 Q1_Q164;
        float3 Q2_Q164;
        Fly_B164(vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,Transition_Q160,_Max_Hover_,_Max_In_Angle_,_Max_Out_Angle_,Out_UV_1_Q154,Out_UV_2_Q154,Out_UV_3_Q154,Vec2_Q167,Nearest_P_Q163,FadingOut_Q160,Q0_Q164,Q1_Q164,Q2_Q164);

        bool Next_Q177;
        Emit_Triangle_B177(false,Q0_Q164,Q1_Q164,Q2_Q164,Extra1_1_Q163,Extra1_2_Q163,Extra1_3_Q163,Result_Q161,Wrist_1_Q153,Wrist_2_Q153,Wrist_3_Q153,Transition_Q160,Next_Q177);

        bool Root = Next_Q177;


        int vxix=0;
        int strip=0;
        [unroll]
        while (strip<stripCount) {
            int i=0;
            [unroll]
            while (i<stripVxCount[strip]) {
                //UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(vxOut[vxix]);
                UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(vxIn[0],vxOut[vxix]);

                triStream.Append(vxOut[vxix]);
                i+=1; vxix+=1;
            }
            triStream.RestartStrip();
            strip+=1;
        }
    }

    //BLOCK_BEGIN Transition 171

    void Transition_B171(
        half Fuzz,
        half4 Fill_Color_Base,
        half4 Line_Color_Base,
        half4 V,
        half4 Transition,
        half Tip_Bump,
        half Line_End_Time,
        half Fill_Start_Time,
        out half4 Fill_Color,
        out half4 Line_Color    )
    {
        float fillProgress = saturate((Transition.w-Fill_Start_Time)/(1.0-Fill_Start_Time));
        
        //float t = Transition.w*2.0;
        
        //float3 d = (V.xyz-float3(0.5,0.5,0.5))*2.0;
        
        //float fillTransition = max(0.0,t-1.0);
        Fill_Color.rgb = Fill_Color_Base.rgb * fillProgress; //* sqrt(dot(d,d)) 
        Fill_Color.a = fillProgress;
        
        float lineProgress = saturate(Transition.w/Line_End_Time);
        float Edge = 1.0-lineProgress;
        float k = Transition.x*(1.0-Fuzz)+Fuzz;
        float k2 = saturate(smoothstep(Edge, Edge+Fuzz, k));
        float lineFade = (1.0+Tip_Bump*sqrt(Edge))*k2; //lineProgress;
        Line_Color = Line_Color_Base * lineFade;
        
    }
    //BLOCK_END Transition

    //BLOCK_BEGIN Edges 168

    void Edges_B168(
        half Edge_Width,
        half Filter_Width,
        half4 Edges,
        out half inLine    )
    {
        float3 fw = Filter_Width*fwidth(Edges.xyz)*max(Edge_Width,1.0);
        float3 a = smoothstep(float3(0.0,0.0,0.0),fw,Edges.xyz);
        inLine = (1.0-min(a.x,min(a.y,a.z)))*min(Edge_Width,1.0);
        
    }
    //BLOCK_END Edges

    //BLOCK_BEGIN Split_Color_Alpha 172

    void Split_Color_Alpha_B172(
        half4 Vector4,
        out half4 Color,
        out half Alpha    )
    {
          Color = Vector4;
          Alpha = Vector4.w;
    }
    //BLOCK_END Split_Color_Alpha


    fixed4 fragment_main(FragmentInput fragInput) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(fragInput);
        float4 result;

        half inLine_Q168;
        Edges_B168(_Edge_Width_,_Filter_Width_,fragInput.extra3,inLine_Q168);

        half4 Color_Q172;
        half Alpha_Q172;
        Split_Color_Alpha_B172(fragInput.extra2,Color_Q172,Alpha_Q172);

        // Multiply_Colors
        half4 Product_Q170 = _Fill_Color_ * Color_Q172;

        // Scale_Color
        half4 Result_Q169 = Alpha_Q172 * _Line_Color_;

        half4 Fill_Color_Q171;
        half4 Line_Color_Q171;
        Transition_B171(_Pulse_Line_Fuzz_,Product_Q170,Result_Q169,fragInput.extra3,fragInput.extra1,_Pulse_Amplify_Leading_,_Line_End_Time_,_Fill_Start_Time_,Fill_Color_Q171,Line_Color_Q171);

        // Mix_Colors
        half4 Color_At_T_Q173 = lerp(Fill_Color_Q171, Line_Color_Q171,float4( inLine_Q168, inLine_Q168, inLine_Q168, inLine_Q168));

        float4 Out_Color = Color_At_T_Q173;
        float Clip_Threshold = 0.00;
        bool To_sRGB = false;


        result = Out_Color;
        float clipVal = (Out_Color.a<Clip_Threshold) ? -1 : 1;
        clip(clipVal);

      return result;
    }

    ENDCG
  }
 }

    FallBack "Mixed Reality Toolkit/Standard"
}

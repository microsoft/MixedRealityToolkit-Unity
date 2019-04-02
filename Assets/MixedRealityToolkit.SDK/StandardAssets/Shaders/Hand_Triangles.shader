// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hand_Triangles" {

Properties {

  [Header(Colors)]
      _Intensity_("Intensity", Range(0,5)) = 3
      _Fill_Color_("Fill Color", Color) = (0.085,0.085,0.085,1)
      _Line_Color_("Line Color", Color) = (0.231,0.231,0.231,1)
      [Toggle] _Color_Map_Enable_("Color Map Enable", Float) = 1
      [NoScaleOffset] _Color_Map_("Color Map", 2D) = "" {}
      _Vary_UV_("Vary UV", Range(0,1)) = 0.71
      _Vary_Color_("Vary Color", Range(0,1)) = 0.7
      _Desaturated_Intensity_("Desaturated Intensity", Range(0,1)) = 0.27
   
  [Header(Edges)]
      _Edge_Width_("Edge Width", Range(0,10)) = 1
      _Filter_Width_("Filter Width", Range(1,5)) = 1.5
   
  [Header(Pulse)]
      [Toggle] _Pulse_Enabled_("Pulse Enabled", Float) = 0
      _Pulse_("Pulse", Range(0,1)) = 0.464
      _Pulse_Width_("Pulse Width", Range(0,5)) = 1.16
      _Pulse_Outer_Size_("Pulse Outer Size", Range(0,2)) = 1
      _Pulse_Lead_Fuzz_("Pulse Lead Fuzz", Range(0,1)) = 0.62
      _Pulse_Tail_Fuzz_("Pulse Tail Fuzz", Range(0,1)) = 0.91
      _Pulse_Vary_("Pulse Vary", Range(0,1)) = 0.09
      _Pulse_Line_Fuzz_("Pulse Line Fuzz", Range(0.01,1)) = 0.18
      _Pulse_Noise_Frequency_("Pulse Noise Frequency", Range(0,2000)) = 777
      _Pulse_Origin_("Pulse Origin", Vector) = (0.5,0,0,0)
      _Pulse_Color_Width_("Pulse Color Width", Range(0,1)) = 0.28
      _Pulse_Amplify_Leading_("Pulse Amplify Leading", Range(0,2)) = 2
   
  [Header(AutoPulse)]
      [Toggle] _Auto_Pulse_("Auto Pulse", Float) = 1
      _Period_("Period", Float) = 3.5
   
  [Header(Edge Timing)]
      _Line_End_Time_("Line End Time", Range(0,1)) = 0.5
      _Fill_Start_Time_("Fill Start Time", Range(0,1)) = 0.5
   
  [Header(Wrist Fade)]
      _Wrist_Fade_Start_("Wrist Fade Start", Range(0,1)) = 0.1
      _Wrist_Fade_End_("Wrist Fade End", Range(0,1)) = 0.16
   
  [Header(Color Space)]
      [Toggle] _Unity_To_sRGB_("Unity To sRGB", Float) = 1
   
  [Header(Flip V For Hydrogen)]
      [Toggle] _Flip_V_("Flip V", Float) = 0
   

}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    ZWrite Off
    Tags {"DisableBatching" = "True"}

  LOD 100 Pass

  {

  CGPROGRAM

  #pragma vertex vertex_main
  #pragma fragment fragment_main
  #pragma geometry geometry_main

  #include "UnityCG.cginc"

  float _Edge_Width_;
  float _Filter_Width_;
  float _Line_End_Time_;
  float _Fill_Start_Time_;
  bool _Flip_V_;
  bool _Unity_To_sRGB_;
  float _Wrist_Fade_Start_;
  float _Wrist_Fade_End_;
  float _Intensity_;
  float4 _Fill_Color_;
  float4 _Line_Color_;
  bool _Color_Map_Enable_;
  sampler2D _Color_Map_;
  float _Vary_UV_;
  float _Vary_Color_;
  float _Desaturated_Intensity_;
  bool _Auto_Pulse_;
  float _Period_;
  bool _Pulse_Enabled_;
  float _Pulse_;
  float _Pulse_Width_;
  float _Pulse_Outer_Size_;
  float _Pulse_Lead_Fuzz_;
  float _Pulse_Tail_Fuzz_;
  float _Pulse_Vary_;
  float _Pulse_Line_Fuzz_;
  float _Pulse_Noise_Frequency_;
  float2 _Pulse_Origin_;
  float _Pulse_Color_Width_;
  float _Pulse_Amplify_Leading_;

  struct VertexInput {
    float4 vertex : POSITION;
    float2 uv0 : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  struct VertexOutput {
    float4 pos : SV_POSITION;
    float3 posWorld : TEXCOORD8;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
    UNITY_VERTEX_INPUT_INSTANCE_ID
  };

  struct FragmentInput {
    float4 pos : SV_POSITION;
    float3 posWorld : TEXCOORD8;
    float4 extra1 : TEXCOORD4;
    float4 extra2 : TEXCOORD5;
        float4 extra3 : TEXCOORD2;
  };

  #define Double_Sided 0
  #define Alpha_Blend 2
  #define No_Depth_Write 1

  #ifndef Geo_Max_Out_Vertices
  #define Geo_Max_Out_Vertices 16
  #endif

  FragmentInput vxOut[Geo_Max_Out_Vertices];
  int stripVxCount[Geo_Max_Out_Vertices];
  float4x4 matrixVP;
  int vxOutCount;
  int stripCount;


  #define HUX_VIEW_TO_WORLD_DIR(V) (mul(transpose(UNITY_MATRIX_V), float4(V,0)).xyz)

  VertexOutput vertex_main(VertexInput v)
  {
    UNITY_SETUP_INSTANCE_ID(v);
    VertexOutput o;
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

  //BLOCK_BEGIN Emit_Triangle 39

  void emitVertex_Bid39(float3 P, float4 extra1, float4 C, float4 extra3)
  {
    vxOut[vxOutCount].posWorld=P; vxOut[vxOutCount].pos=mul(matrixVP, float4(P,1.0f));;
    HUX_GEO_SET_EXTRA1(extra1);
    HUX_GEO_SET_EXTRA2(C);
    HUX_GEO_SET_EXTRA3(extra3);
    vxOutCount+=1; stripVxCount[stripCount]+=1;
  }
  
  void Emit_Triangle_B39(
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
      out bool Next  )
  {
      
      float2 uv;
      
      uv = float2(0,0);
      emitVertex_Bid39(P1,Extra1_1,Color*Wrist_1,float4(1.0,0.0,0.0,0.0));
      
      uv = float2(1,0);
      emitVertex_Bid39(P2,Extra1_2,Color*Wrist_2,float4(0.0,1.0,0.0,0.0));
      
      uv = float2(0,1);
      emitVertex_Bid39(P3,Extra1_3,Color*Wrist_3,float4(0.0,0.0,1.0,0.0));
      
      stripCount+=1; stripVxCount[stripCount]=0;
      
      Next = Previous;
      
  }
  //BLOCK_END Emit_Triangle

  //BLOCK_BEGIN Find_Nearest 35

  void Find_Nearest_B35(
      float2 UV1,
      float2 UV2,
      float2 UV3,
      float2 Pulse_Origin,
      float Transition,
      bool FadingOut,
      out float4 Extra1_1,
      out float4 Extra1_2,
      out float4 Extra1_3  )
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
          } else if (d2>d3) {
              Extra1_2.x=1.0;
          } else {
              Extra1_3.x=1.0;
          }
      } else {
          if (d1<d2 && d1<d3) {
              Extra1_1.x=1.0;
          } else if (d2<d3) {
              Extra1_2.x=1.0;
          } else {
              Extra1_3.x=1.0;
          }
      }
      
  }
  //BLOCK_END Find_Nearest

  //BLOCK_BEGIN Average 26

  void Average_B26(
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
      out float Wrist_3  )
  {
      Average = (A2 + B2 + C2) * (1.0/3.0);
      Wrist_1 = saturate((A2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
      Wrist_2 = saturate((B2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
      Wrist_3 = saturate((C2.y-Wrist_Start)/(Wrist_End-Wrist_Start));
      
  }
  //BLOCK_END Average

  //BLOCK_BEGIN Flip_V_For_Hydrogen 28

  void Flip_V_For_Hydrogen_B28(
      bool Flip_V,
      float2 UV_1,
      float2 UV_2,
      float2 UV_3,
      out float2 Out_UV_1,
      out float2 Out_UV_2,
      out float2 Out_UV_3  )
  {
      Out_UV_1 = Flip_V ? float2(UV_1.x,1.0-UV_1.y) : UV_1;
      Out_UV_2 = Flip_V ? float2(UV_2.x,1.0-UV_2.y) : UV_2;
      Out_UV_3 = Flip_V ? float2(UV_3.x,1.0-UV_3.y) : UV_3;
      
  }
  //BLOCK_END Flip_V_For_Hydrogen

  //BLOCK_BEGIN Pulse 41

  float ramp_Bid41(float start, float end, float x)
  {
  //    return saturate((x-start)/(end-start));
      return smoothstep(start,end,x);
  }
  
  void Pulse_B41(
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
      out float Fade_Color  )
  {
      Transition = 1.0;
      Saturation = 1.0;
      Fade_Color = 1.0;
      
      if (Pulse_Enabled) {
          float d = Distance - Pulse_Vary*Noise; 
          
          float totalSize = Pulse_Outer_Size+Pulse_Vary+Pulse_Width;
          float pulseFront = Pulse * totalSize;
      
          float edge1 = pulseFront-Pulse_Width*0.5*Pulse_Lead_Fuzz;
          float fadeIn = saturate(1.0-ramp_Bid41(edge1,pulseFront,d));
          float fadeOut = saturate(1.0-ramp_Bid41(pulseFront-Pulse_Width,pulseFront-Pulse_Width+Pulse_Width*0.5*Pulse_Tail_Fuzz,d));
          Saturation = saturate(smoothstep(edge1-Pulse_Saturation,edge1,d));
      
          float clip = 1.0-step(Pulse_Outer_Size,d);
          
          Transition= saturate(fadeIn-fadeOut)*clip;
          FadingOut = fadeOut>0.0;
          Fade_Color = 1.0-fadeOut;
      }
      
  }
  //BLOCK_END Pulse

  //BLOCK_BEGIN Cell_Noise_2D 22

  float2 mod289_Bid22(float2 x)
  {
    return x - floor(x * (1.0 / 289.0)) * 289.0;
  }
  
  float2 permute_Bid22(float2 x)
  {
    return mod289_Bid22(((x*float2(33.0,35.0))+1.0)*x);
  }
  
  float2 permuteB_Bid22(float2 x)
  {
    return mod289_Bid22(((x*float2(37.0,34.0))-1.0)*x);
  }
  
  
  void Cell_Noise_2D_B22(
      float2 XY,
      float Frequency,
      float Seed,
      out float Result  )
  {
      
      float2 P = XY * float2(Frequency,Frequency)+float2(Seed,Seed);
      float2 Pi = floor(P);
      
      Pi = mod289_Bid22(Pi); // To avoid truncation effects in permutation
      
      float2 ix = Pi.xy;
      float2 iy = Pi.yx;
      
      float2 i = permute_Bid22(permuteB_Bid22(ix) + iy);
      
      Result = frac(i.x*(1.0/41.0)+i.y*(1.0/42.0));
      
      //Result = lerp(Out_Min, Out_Max, r);
      
  }
  //BLOCK_END Cell_Noise_2D

  //BLOCK_BEGIN Pt_Sample_Texture 34

  void Pt_Sample_Texture_B34(
      float2 UV,
      float Noise,
      sampler2D Texture,
      float Vary_UV,
      float Map_Intensity,
      out float4 Color  )
  {
      float2 xy = UV + float2(Noise-0.5,Noise-0.5)*Vary_UV;
      Color = tex2D(Texture,xy,float2(0,0),float2(0,0))*Map_Intensity;
  }
  //BLOCK_END Pt_Sample_Texture

  //BLOCK_BEGIN AutoPulse 19

  void AutoPulse_B19(
      float Pulse,
      bool Auto_Pulse,
      float Period,
      float Time,
      out float Result  )
  {
      
      if (Auto_Pulse) {
          Result = frac(Time/Period);
      } else {
          Result = Pulse;
      }
  }
  //BLOCK_END AutoPulse

  //BLOCK_BEGIN Cell_Noise_2D 23

  float2 mod289_Bid23(float2 x)
  {
    return x - floor(x * (1.0 / 289.0)) * 289.0;
  }
  
  float2 permute_Bid23(float2 x)
  {
    return mod289_Bid23(((x*float2(33.0,35.0))+1.0)*x);
  }
  
  float2 permuteB_Bid23(float2 x)
  {
    return mod289_Bid23(((x*float2(37.0,34.0))-1.0)*x);
  }
  
  
  void Cell_Noise_2D_B23(
      float2 XY,
      float Frequency,
      float Seed,
      out float Result  )
  {
      
      float2 P = XY * float2(Frequency,Frequency)+float2(Seed,Seed);
      float2 Pi = floor(P);
      
      Pi = mod289_Bid23(Pi); // To avoid truncation effects in permutation
      
      float2 ix = Pi.xy;
      float2 iy = Pi.yx;
      
      float2 i = permute_Bid23(permuteB_Bid23(ix) + iy);
      
      Result = frac(i.x*(1.0/41.0)+i.y*(1.0/42.0));
      
      //Result = lerp(Out_Min, Out_Max, r);
      
  }
  //BLOCK_END Cell_Noise_2D


  [maxvertexcount(Geo_Max_Out_Vertices)]
  void geometry_main(triangle VertexOutput vxIn[3], inout TriangleStream<FragmentInput> triStream)
  {
    //huxEye = _WorldSpaceCameraPos;
  //workaround for Unity's auto updater in 5.6
    float4x4 tmp = UNITY_MATRIX_MVP;
  matrixVP = mul(tmp, unity_WorldToObject);
    //matrixVP = mul(UNITY_MATRIX_MVP, _World2Object);
    vxOutCount=0;
    stripCount=0;
    stripVxCount[0]=0;

    float2 Out_UV_1_Q28;
    float2 Out_UV_2_Q28;
    float2 Out_UV_3_Q28;
    Flip_V_For_Hydrogen_B28(_Flip_V_,vxIn[0].uv,vxIn[1].uv,vxIn[2].uv,Out_UV_1_Q28,Out_UV_2_Q28,Out_UV_3_Q28);

    float Result_Q19;
    AutoPulse_B19(_Pulse_,_Auto_Pulse_,_Period_,_Time.y,Result_Q19);

    float2 Average_Q26;
    float Wrist_1_Q26;
    float Wrist_2_Q26;
    float Wrist_3_Q26;
    Average_B26(Out_UV_1_Q28,Out_UV_2_Q28,Out_UV_3_Q28,vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,_Wrist_Fade_Start_,_Wrist_Fade_End_,Average_Q26,Wrist_1_Q26,Wrist_2_Q26,Wrist_3_Q26);

    float Result_Q22;
    Cell_Noise_2D_B22(Average_Q26,_Pulse_Noise_Frequency_,111,Result_Q22);

    // Distance2
    float Distance_Q40 = distance(Average_Q26,_Pulse_Origin_);

    float Result_Q23;
    Cell_Noise_2D_B23(Average_Q26,_Pulse_Noise_Frequency_,333,Result_Q23);

    float Transition_Q41;
    bool FadingOut_Q41;
    float Saturation_Q41;
    float Fade_Color_Q41;
    Pulse_B41(Distance_Q40,Result_Q22,_Pulse_Enabled_,Result_Q19,_Pulse_Width_,_Pulse_Outer_Size_,_Pulse_Lead_Fuzz_,_Pulse_Tail_Fuzz_,_Pulse_Vary_,_Pulse_Color_Width_,Transition_Q41,FadingOut_Q41,Saturation_Q41,Fade_Color_Q41);

    float4 Color_Q34;
    if (_Color_Map_Enable_) {
      Pt_Sample_Texture_B34(Average_Q26,Result_Q23,_Color_Map_,_Vary_UV_,1,Color_Q34);
    } else {
      Color_Q34 = float4(1,1,1,1);
    }

    float4 Extra1_1_Q35;
    float4 Extra1_2_Q35;
    float4 Extra1_3_Q35;
    Find_Nearest_B35(Out_UV_1_Q28,Out_UV_2_Q28,Out_UV_3_Q28,_Pulse_Origin_,Transition_Q41,FadingOut_Q41,Extra1_1_Q35,Extra1_2_Q35,Extra1_3_Q35);

    // Color
    float4 Result_Q33;
    float k = max(Color_Q34.r,max(Color_Q34.g,Color_Q34.b))*_Desaturated_Intensity_;
    Result_Q33 = lerp(float4(k,k,k,1),Color_Q34,float4(Saturation_Q41,Saturation_Q41,Saturation_Q41,Saturation_Q41))*(1.0-_Vary_Color_*Result_Q22)*Fade_Color_Q41;
    Result_Q33.rgb *= _Intensity_;
    
    bool Next_Q39;
    Emit_Triangle_B39(false,vxIn[0].posWorld,vxIn[1].posWorld,vxIn[2].posWorld,Extra1_1_Q35,Extra1_2_Q35,Extra1_3_Q35,Result_Q33,Wrist_1_Q26,Wrist_2_Q26,Wrist_3_Q26,Next_Q39);

    bool Root = Next_Q39;


    int vxix=0;
    int strip=0;
    while (strip<stripCount) {
       int i=0;
       while (i<stripVxCount[strip]) {
         triStream.Append(vxOut[vxix]);
         i+=1; vxix+=1;
       }
       triStream.RestartStrip();
       strip+=1;
    }
  }

  //BLOCK_BEGIN Transition 37

  void Transition_B37(
      float Fuzz,
      float4 Fill_Color_Base,
      float4 Line_Color_Base,
      float4 V,
      float4 Transition,
      float Tip_Bump,
      float Line_End_Time,
      float Fill_Start_Time,
      out float4 Fill_Color,
      out float4 Line_Color  )
  {
      float fillProgress = saturate((Transition.w-Fill_Start_Time)/(1.0-Fill_Start_Time));
      
      //float t = Transition.w*2.0;
      
      float3 d = (V.xyz-float3(0.5,0.5,0.5))*2.0;
      
      //float fillTransition = max(0.0,t-1.0);
      Fill_Color.rgb = Fill_Color_Base.rgb * sqrt(dot(d,d)) * fillProgress;
      Fill_Color.a = fillProgress;
      
      float lineProgress = saturate(Transition.w/Line_End_Time);
      float Edge = 1.0-lineProgress;
      float k = Transition.x*(1.0-Fuzz)+Fuzz;
      float k2 = saturate(smoothstep(Edge, Edge+Fuzz, k));
      float lineFade = k2*(1.0+Tip_Bump*Edge);
      Line_Color = Line_Color_Base * lineFade;
      
  }
  //BLOCK_END Transition

  //BLOCK_BEGIN Edges 38

  void Edges_B38(
      float Edge_Width,
      float Filter_Width,
      float4 Edges,
      out float inLine  )
  {
      float3 fw = Filter_Width*fwidth(Edges.xyz)*max(Edge_Width,1.0);
      float3 a = smoothstep(float3(0.0,0.0,0.0),fw,Edges.xyz);
      inLine = (1.0-min(a.x,min(a.y,a.z)))*min(Edge_Width,1.0);
      
  }
  //BLOCK_END Edges

  //BLOCK_BEGIN Split_Color_Alpha 20

  void Split_Color_Alpha_B20(
      float4 Vector4,
      out float4 Color,
      out float Alpha  )
  {
        Color = Vector4;
        Alpha = Vector4.w;
  }
  //BLOCK_END Split_Color_Alpha


  fixed4 fragment_main(FragmentInput fragInput) : SV_Target
  {
    float4 result;

    float inLine_Q38;
    Edges_B38(_Edge_Width_,_Filter_Width_,fragInput.extra3,inLine_Q38);

    float4 Color_Q20;
    float Alpha_Q20;
    Split_Color_Alpha_B20(fragInput.extra2,Color_Q20,Alpha_Q20);

    // Multiply_Colors
    float4 Product_Q21 = _Fill_Color_ * Color_Q20;

    // Scale_Color
    float4 Result_Q25 = Alpha_Q20 * _Line_Color_;

    float4 Fill_Color_Q37;
    float4 Line_Color_Q37;
    Transition_B37(_Pulse_Line_Fuzz_,Product_Q21,Result_Q25,fragInput.extra3,fragInput.extra1,_Pulse_Amplify_Leading_,_Line_End_Time_,_Fill_Start_Time_,Fill_Color_Q37,Line_Color_Q37);

    // Mix_Colors
    float4 Color_At_T_Q13 = lerp(Fill_Color_Q37, Line_Color_Q37,float4( inLine_Q38, inLine_Q38, inLine_Q38, inLine_Q38));

    float4 Out_Color = Color_At_T_Q13;
    float Clip_Threshold = 0.00;
    bool To_sRGB = _Unity_To_sRGB_;


    result = Out_Color;
    float clipVal = (Out_Color.a<Clip_Threshold) ? -1 : 1;
    clip(clipVal);
    if (To_sRGB) {
             result.rgb = clamp(float3(1.055,1.055,1.055) * pow(result.rgb, float3(0.416666667,0.416666667,0.416666667)) - float3(0.055,0.055,0.055), 0.0, Out_Color.a);
    }

    return result;
  }

  ENDCG
  }
 }
}

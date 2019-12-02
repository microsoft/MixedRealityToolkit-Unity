// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

/// <summary>
/// Note, this shader is generated from a tool and is not formated for user readability.
/// </summary>

Shader "Mixed Reality Toolkit/FingerTipCursor" {

Properties {

    [Header(Proximity)]
        _Proximity_Distance_("Proximity Distance", Range(0,1)) = 0.05
        _Fade_Near_Distance_("Fade Near Distance", Range(0,1)) = 0.1
        _Fade_Far_Distance_("Fade Far Distance", Range(0,1)) = 0.2
        _Shrink_Start_Distance_("Shrink Start Distance", Range(0,1)) = 0.1
     
    [Header(Shape)]
        _Near_Radius_Fraction_("Near Radius Fraction", Range(0,1)) = 0.5
        _Far_Center_Fraction_("Far Center Fraction", Range(0,1)) = 1
        _Near_Center_Fraction_("Near Center Fraction", Range(0,1)) = 0
        _Thickness_("Thickness", Range(0,1)) = 1
     
    [Header(Smooth Pulse)]
        _Rise_Start_("Rise Start", Range(0,1)) = 0
        _Rise_End_("Rise End", Range(0,1)) = 0.3
        _Fall_Start_("Fall Start", Range(0,1)) = 0.77
        _Fall_End_("Fall End", Range(0,1)) = 1
        _Start_Fall_Fade_("Start Fall Fade", Range(0,1)) = 0.05
     
    [Header(Colors)]
        _Edge_Color_("Edge Color", Color) = (0.188235,0.188235,0.188235,0.502)
        _Base_Color_("Base Color", Color) = (0.6,0.6,0.6,0.698)
}

SubShader {
    Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}
    Blend One OneMinusSrcAlpha

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0

    #include "UnityCG.cginc"

    float _Near_Radius_Fraction_;
    float _Far_Center_Fraction_;
    float _Near_Center_Fraction_;
    float _Thickness_;
    half _Rise_Start_;
    half _Rise_End_;
    half _Fall_Start_;
    half _Fall_End_;
    float _Start_Fall_Fade_;
    half4 _Edge_Color_;
    half4 _Base_Color_;
    bool _Right_Hand_;
    bool _Use_Local_Proximity_;
    float _Proximity_Distance_;
    float _Fade_Near_Distance_;
    float _Fade_Far_Distance_;
    float _Shrink_Start_Distance_;

    struct VertexInput {
        float4 vertex : POSITION;
        float2 uv0 : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    // declare parm vars here

    //BLOCK_BEGIN Object_To_World_Pos 207

    void Object_To_World_Pos_B207(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Resize 220

    void Resize_B220(
        float Distance,
        float Shrink_Start_Distance,
        float Far_Center_Fraction,
        float Near_Center_Fraction,
        float Near_Radius_Fraction,
        float3 Position,
        float2 UV,
        float Thickness,
        float Fade_Near,
        float Fade_Far,
        float Start_Fall_Fade,
        out float Center_Fraction,
        out float Radius_At_D,
        out float3 New_Position,
        out float Outer_Ring,
        out float Rim,
        out float Fade,
        out float Inner_Fade    )
    {
        float k = saturate(Distance/Shrink_Start_Distance);
        Center_Fraction = lerp(Near_Center_Fraction, Far_Center_Fraction, k);
        
        Radius_At_D = lerp(Near_Radius_Fraction, 1.0, k);
        
        //Outer_Ring = length(Position.xy)<Ring_Middle ? 0 : 1;
        Rim = UV.x*2.0;
        
        if (false) {
            Outer_Ring = 1.0-UV.y;
        } else {
            Outer_Ring = UV.y;
        }
        
        float scale = lerp(Center_Fraction,Radius_At_D,Outer_Ring);
        
        New_Position = Position * float3(scale,scale,Thickness);
        
        Fade = 1.0-saturate((Distance-Fade_Near)/(Fade_Far-Fade_Near));
        
        Inner_Fade = saturate(k/Start_Fall_Fade);
        
    }
    //BLOCK_END Resize


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        float Center_Fraction_Q220;
        float Radius_At_D_Q220;
        float3 New_Position_Q220;
        float Outer_Ring_Q220;
        float Rim_Q220;
        float Fade_Q220;
        float Inner_Fade_Q220;
        Resize_B220(_Proximity_Distance_,_Shrink_Start_Distance_,_Far_Center_Fraction_,_Near_Center_Fraction_,_Near_Radius_Fraction_,vertInput.vertex.xyz,vertInput.uv0,_Thickness_,_Fade_Near_Distance_,_Fade_Far_Distance_,0.05,Center_Fraction_Q220,Radius_At_D_Q220,New_Position_Q220,Outer_Ring_Q220,Rim_Q220,Fade_Q220,Inner_Fade_Q220);

        float3 Pos_World_Q207;
        Object_To_World_Pos_B207(New_Position_Q220,Pos_World_Q207);

        // From_XYZ
        float3 Vec3_Q217 = float3(Rim_Q220,Fade_Q220,Inner_Fade_Q220);

        float3 Position = Pos_World_Q207;
        float3 Normal = Vec3_Q217;
        float2 UV = float2(0,0);
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);


        o.pos = UnityObjectToClipPos(vertInput.vertex);
        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;

        return o;
    }

    //BLOCK_BEGIN Scale_Color 215

    void Scale_Color_B215(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = Scalar * Color;
    }
    //BLOCK_END Scale_Color

    //BLOCK_BEGIN To_XYZ 218

    void To_XYZ_B218(
        float3 Vec3,
        out float X,
        out float Y,
        out float Z    )
    {
        X=Vec3.x;
        Y=Vec3.y;
        Z=Vec3.z;
        
    }
    //BLOCK_END To_XYZ

    //BLOCK_BEGIN Smooth_Pulse 219

    float ramp(float s, float e, float x)
    {
        return saturate((x-s)/(e-s));
    }
    
    void Smooth_Pulse_B219(
        half Rise_Start,
        half Rise_End,
        half Fall_Start,
        half Fall_End,
        half X,
        float Inner_Fade,
        out half Pulse    )
    {
        //Pulse = smoothstep(Rise_Start,Rise_End,X)-smoothstep(Fall_Start,Fall_End,X);
        float x = abs(1.0-X);
        Pulse = ramp(Rise_Start,Rise_End,x)-ramp(Fall_Start,Fall_End,x)*Inner_Fade;
        
    }
    //BLOCK_END Smooth_Pulse


    //fixed4 frag(VertexOutput fragInput, fixed facing : VFACE) : SV_Target
    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        float X_Q218;
        float Y_Q218;
        float Z_Q218;
        To_XYZ_B218(fragInput.normalWorld.xyz,X_Q218,Y_Q218,Z_Q218);

        half Pulse_Q219;
        Smooth_Pulse_B219(_Rise_Start_,_Rise_End_,_Fall_Start_,_Fall_End_,X_Q218,Z_Q218,Pulse_Q219);

        // Mix_Colors
        half4 Color_At_T_Q214 = lerp(_Edge_Color_, _Base_Color_,float4( Pulse_Q219, Pulse_Q219, Pulse_Q219, Pulse_Q219));

        half4 Result_Q215;
        Scale_Color_B215(Color_At_T_Q214,Y_Q218,Result_Q215);

        float4 Out_Color = Result_Q215;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

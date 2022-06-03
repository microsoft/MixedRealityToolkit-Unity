// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

Shader "Shell_Rounded_Bound" {

Properties {

    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
        _Selection_Color_("Selection Color", Color) = (1,1,1,1)
        _Inner_Color_("Inner Color", Color) = (0.2,0.2,0.2,1)
        _Focus_Max_Intensity_("Focus Max Intensity", Range(0,1)) = 0.6
        _Proximity_Max_Intensity_("Proximity Max Intensity", Range(0,1)) = 0.7
        _Selection_Inner_Intensity_("Selection Inner Intensity", Range(0,1)) = 0.5
     
    [Header(Size)]
        _Line_Radius_("Line Radius", Range(0,0.1)) = 0.005
        _Bevel_Radius_("Bevel Radius", Range(0,0.1)) = 0.01
        [Toggle] _Shrink_On_Pinch_("Shrink On Pinch", Float) = 0
        _Shrunk_Radius_Fraction_("Shrunk Radius Fraction", Range(0,1)) = 0.25
     
    [Header(Interaction)]
        [PerRendererData] _Gaze_Focus_("Gaze Focus",Float) = 0
        [PerRendererData] _Extra_Input_Progress_("Extra Input Progress",Range(0,1)) = 0.0
        [PerRendererData] _Pinched_("Pinched",Float) = 0.0
     
    [Header(Finger Tip Proximity)]
        _Finger_Tip_Proximity_Radius_("Finger Tip Proximity Radius", Range(0,1)) = 0.05
        _Finger_Tip_Glow_("Finger Tip Glow", Range(0,1)) = 0.3
     
    [Header(Edges)]
        [Toggle] _Show_Internal_Back_("Show Internal Back", Float) = 1
        [Toggle] _Show_Internal_Front_("Show Internal Front", Float) = 0
        [Toggle] _Show_Internal_On_Focus_("Show Internal On Focus", Float) = 0
        [PerRendererData] _Show_All_Internal_On_Focus_("Show All Internal On Focus",Float) = 0
     

}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    Cull Off
    Tags {"DisableBatching" = "True"}

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0

    #include "UnityCG.cginc"

    float4 _Color_;
    float4 _Selection_Color_;
    float4 _Inner_Color_;
    float _Focus_Max_Intensity_;
    float _Proximity_Max_Intensity_;
    float _Selection_Inner_Intensity_;
    float _Line_Radius_;
    float _Bevel_Radius_;
    bool _Shrink_On_Pinch_;
    float _Shrunk_Radius_Fraction_;
    float _Gaze_Focus_;
    float _Extra_Input_Progress_;
    float _Pinched_;
    float _Finger_Tip_Proximity_Radius_;
    float _Finger_Tip_Glow_;
    bool _Show_Internal_Back_;
    bool _Show_Internal_Front_;
    bool _Show_Internal_On_Focus_;
    bool _Show_All_Internal_On_Focus_;
    float3 Global_Left_Index_Tip_Position;
    float3 Global_Right_Index_Tip_Position;



    float3 _Gaze_Position_;
    float3 _Gaze_Direction_;

    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 vertexColor : COLOR;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN FaceCamera 40

    void FaceCamera_B40(
        float3 Incident,
        float3 Position,
        float3 Tangent,
        float Radius,
        float Side,
        out float3 Result,
        out float U    )
    {
        float3 dir = normalize(cross(Tangent,Incident));
        Result = Position + dir * Radius * Side;
        U = Side*0.5+0.5;
        
    }
    //BLOCK_END FaceCamera

    //BLOCK_BEGIN CreateEdge 49

    void CreateEdge_B49(
        float3 EdgeA,
        float3 EdgeB,
        float3 EdgeC,
        float3 Corner,
        float2 UV,
        float Radius,
        float3 DirOut,
        float WhichEdge,
        out float3 Result,
        out float3 Tangent,
        out float Length    )
    {
        float3 deltaA = Corner-EdgeA;
        float3 deltaB = Corner-EdgeB;
        float3 deltaC = Corner-EdgeC;
        
        float lenA = length(deltaA);
        float lenB = length(deltaB);
        float lenC = length(deltaC);
        
        float3 dira = deltaA/lenA;
        float3 dirb = deltaB/lenB;
        float3 dirc = deltaC/lenC;
        
        //float radius = min(Radius,min(lenA,lenB));
        float radius = Radius;
        
        float angle = saturate((UV.y-0.5)*2.0)*3.14159*0.25;
        float cosa = cos(angle)*radius;
        float sina = sin(angle)*radius;
        
        float3 start, end;
        if (WhichEdge==0.0) {
            start = EdgeA+radius*dirb;
            Tangent = -cosa*dira+sina*dirb;    
            end = Corner + cosa*dirb+sina*dira;
            Length=max(0.0,distance(Corner,EdgeA));
        } else if (WhichEdge==1.0) {
            start = EdgeB+radius*dira;
            Tangent = -cosa*dirb+sina*dira;    
            end = Corner + cosa*dira+sina*dirb;
            Length=max(0.0,distance(Corner,EdgeB));
        } else {
            start = EdgeC+radius*(dira+dirb) - DirOut*radius;
            Tangent = dirc;
            end = (Corner +radius*(dira+dirb)) - DirOut*radius;
            Tangent = normalize(end-start);
            Length=max(0.0,distance(Corner,EdgeC));
        }
        float t = saturate(UV.y*2.0);
        Result = start + (end-start)*t + DirOut*radius;
        
    }
    //BLOCK_END CreateEdge

    //BLOCK_BEGIN Object_To_World_Pos 22

    void Object_To_World_Pos_B22(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN DecodeCorner 65

    void DecodeCorner_B65(
        float3 RawNormal,
        float2 UV,
        out float2 XY,
        out float3 N1,
        out float3 N2,
        out float3 N3,
        out float3 Corner,
        out float WhichEdge    )
    {
        float strip = floor(UV.x);
        XY = float2((UV.x-strip)*4.0-1.0,UV.y);
        
        float3 Normal = sign(RawNormal);
        
        Corner = Normal*0.5f;
        
        N1 = float3(Normal.x,0.0,0.0);
        N2 = float3(0.0,Normal.y,0.0);
        N3 = float3(0.0,0.0,Normal.z);
        
        //SelectEdge = floor(strip*0.5)==strip*0.5;
        WhichEdge = strip-floor(strip/3.0)*3.0;
        
        
    }
    //BLOCK_END DecodeCorner

    //BLOCK_BEGIN Visibility 50

    void Visibility_B50(
        float3 Corner,
        float3 DirA,
        float3 DirB,
        float3 DirC,
        float3 Eye,
        float3 ObjectCorner,
        float Which_Edge,
        out float Result,
        out float3 EdgeA,
        out float3 EdgeB,
        out float3 EdgeC,
        out float3 DirOut,
        out float3 Out_Edge,
        out float OnBack,
        out float OnFront    )
    {
        float3 I = Corner - Eye;
        
        float dota = dot(I,DirA);
        float dotb = dot(I,DirB);
        float dotc = dot(I,DirC);
        
        float signa = sign(dota);
        float signb = sign(dotb);
        float signc = sign(dotc);
        
        Result = (signa!=signb || signa!=signc) && Which_Edge<2.0 ? 1.0 : 0.0;
        
        if (signa!=signb && signa!=signc) {
            EdgeA = float3(ObjectCorner.x,ObjectCorner.y,0.0);
            EdgeB = float3(ObjectCorner.x,0.0,ObjectCorner.z);
            EdgeC = float3(0.0,ObjectCorner.y,ObjectCorner.z);
            OnBack = Result==0 ? saturate(min(signb,signc)) : Result;
            OnFront = Result==0 ? 1.0-saturate(min(signb,signc)) : Result;
            DirOut = DirA;
        } else if (signb!=signa && signb!=signc) {
            EdgeA = float3(ObjectCorner.x,ObjectCorner.y,0.0);
            EdgeB = float3(0.0,ObjectCorner.y,ObjectCorner.z);
            EdgeC = float3(ObjectCorner.x,0.0,ObjectCorner.z);
            OnBack = Result==0 ? saturate(min(signa,signc)) : Result;
            OnFront = Result==0 ? 1.0-saturate(min(signa,signc)) : Result;
            DirOut = DirB;
        } else {
            EdgeA = float3(ObjectCorner.x,0.0,ObjectCorner.z);
            EdgeB = float3(0.0,ObjectCorner.y,ObjectCorner.z);
            EdgeC = float3(ObjectCorner.x,ObjectCorner.y,0.0);
            OnBack = Result==0 ? saturate(min(signa,signb)) : Result;
            OnFront = Result==0 ? 1.0-saturate(min(signa,signb)) : Result;
            DirOut = DirC;
        }
        Out_Edge = Which_Edge==0 ? EdgeA : (Which_Edge==1.0 ? EdgeB : EdgeC);
        
    }
    //BLOCK_END Visibility

    //BLOCK_BEGIN Scale_RGB 44

    void Scale_RGB_B44(
        float4 Color,
        float Scalar,
        out float4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Object_To_World_Normal 17

    void Object_To_World_Normal_B17(
        float3 Nrm_Object,
        out float3 Nrm_World    )
    {
        Nrm_World=UnityObjectToWorldNormal(Nrm_Object);
        
    }
    //BLOCK_END Object_To_World_Normal

    //BLOCK_BEGIN Conditional_Float 26

    void Conditional_Float_B26(
        bool Which,
        float If_True,
        float If_False,
        out float Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Float

    //BLOCK_BEGIN Conditional_Float 52

    void Conditional_Float_B52(
        bool Which,
        float If_True,
        float If_False,
        out float Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Float

    //BLOCK_BEGIN Or 85

    void Or_B85(
        bool A,
        bool B,
        out bool AorB    )
    {
        AorB = A || B;
    }
    //BLOCK_END Or


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        #if defined(USING_STEREO_MATRICES)
            _Gaze_Position_ = (unity_StereoWorldSpaceCameraPos[0]+unity_StereoWorldSpaceCameraPos[1])*0.5;
            _Gaze_Direction_ = normalize(float3(unity_StereoMatrixInvV[0][0][2],unity_StereoMatrixInvV[0][1][2],unity_StereoMatrixInvV[0][2][2])+
                                         float3(unity_StereoMatrixInvV[1][0][2],unity_StereoMatrixInvV[1][1][2],unity_StereoMatrixInvV[1][2][2]));
        #else
            _Gaze_Position_ = _WorldSpaceCameraPos;
            _Gaze_Direction_ = float3(UNITY_MATRIX_I_V[0][2],UNITY_MATRIX_I_V[1][2],UNITY_MATRIX_I_V[2][2]);
        #endif

        float2 XY_Q65;
        float3 N1_Q65;
        float3 N2_Q65;
        float3 N3_Q65;
        float3 Corner_Q65;
        float WhichEdge_Q65;
        DecodeCorner_B65(vertInput.normal,vertInput.uv0,XY_Q65,N1_Q65,N2_Q65,N3_Q65,Corner_Q65,WhichEdge_Q65);

        float4 Result_Q44;
        Scale_RGB_B44(_Selection_Color_,_Selection_Inner_Intensity_,Result_Q44);

        float3 Nrm_World_Q17;
        Object_To_World_Normal_B17(N1_Q65,Nrm_World_Q17);

        float3 Nrm_World_Q18;
        Object_To_World_Normal_B17(N2_Q65,Nrm_World_Q18);

        float3 Nrm_World_Q21;
        Object_To_World_Normal_B17(N3_Q65,Nrm_World_Q21);

        // Multiply (#33)
        float Product_Q33 = _Gaze_Focus_ * _Focus_Max_Intensity_;

        // Multiply (#30)
        float Product_Q30 = _Gaze_Focus_ * _Extra_Input_Progress_;

        // Max (#55)
        float MaxAB_Q55=max(_Gaze_Focus_,_Extra_Input_Progress_);

        bool AorB_Q85;
        Or_B85(_Show_Internal_On_Focus_,_Show_All_Internal_On_Focus_,AorB_Q85);

        // Smooth_Step (#47)
        float Smooth_Step_Q47 = smoothstep(0.5, 1.0, _Extra_Input_Progress_);

        float Result_Q87;
        Conditional_Float_B52(_Show_Internal_Back_,1,0,Result_Q87);

        float Result_Q89;
        Conditional_Float_B52(_Show_Internal_Front_,1,0,Result_Q89);

        // To_XY (#23)
        float X_Q23;
        float Y_Q23;
        X_Q23 = XY_Q65.x;
        Y_Q23 = XY_Q65.y;

        float3 Pos_World_Q19;
        Object_To_World_Pos_B22(Corner_Q65,Pos_World_Q19);

        float Result_Q50;
        float3 EdgeA_Q50;
        float3 EdgeB_Q50;
        float3 EdgeC_Q50;
        float3 DirOut_Q50;
        float3 Out_Edge_Q50;
        float OnBack_Q50;
        float OnFront_Q50;
        Visibility_B50(Pos_World_Q19,Nrm_World_Q17,Nrm_World_Q18,Nrm_World_Q21,_Gaze_Position_,Corner_Q65,WhichEdge_Q65,Result_Q50,EdgeA_Q50,EdgeB_Q50,EdgeC_Q50,DirOut_Q50,Out_Edge_Q50,OnBack_Q50,OnFront_Q50);

        // Mix_Colors (#42)
        float4 Color_At_T_Q42 = lerp(_Inner_Color_, _Color_,float4( Result_Q50, Result_Q50, Result_Q50, Result_Q50));

        // Max (#58)
        float MaxAB_Q58=max(Product_Q33,_Extra_Input_Progress_);

        float Result_Q26;
        Conditional_Float_B26(_Shrink_On_Pinch_,Product_Q30,1.0,Result_Q26);

        // Sqrt (#29)
        float Sqrt_F_Q29 = sqrt(MaxAB_Q55);

        float Result_Q81;
        Conditional_Float_B26(AorB_Q85,1,Smooth_Step_Q47,Result_Q81);

        // Multiply (#90)
        float Product_Q90 = OnBack_Q50 * Result_Q87;

        // Multiply (#92)
        float Product_Q92 = OnFront_Q50 * Result_Q89;

        // From_XY (#41)
        float2 Vec2_Q41 = float2(X_Q23,Y_Q23);

        // Mix_Colors (#43)
        float4 Color_At_T_Q43 = lerp(Result_Q44, _Selection_Color_,float4( Result_Q50, Result_Q50, Result_Q50, Result_Q50));

        float3 Pos_World_Q22;
        Object_To_World_Pos_B22(EdgeA_Q50,Pos_World_Q22);

        float3 Pos_World_Q24;
        Object_To_World_Pos_B22(EdgeB_Q50,Pos_World_Q24);

        float3 Pos_World_Q39;
        Object_To_World_Pos_B22(EdgeC_Q50,Pos_World_Q39);

        // Max (#36)
        float MaxAB_Q36=max(MaxAB_Q58,_Proximity_Max_Intensity_);

        // Lerp (#35)
        float Value_At_T_Q35=lerp(1,_Shrunk_Radius_Fraction_,Result_Q26);

        // Multiply (#27)
        float Product_Q27 = _Line_Radius_ * Sqrt_F_Q29;

        // Max (#93)
        float MaxAB_Q93=max(Product_Q90,Product_Q92);

        // Scale_Color (#28)
        float4 Result_Q28 = MaxAB_Q36 * Color_At_T_Q42;

        // Multiply (#25)
        float Product_Q25 = _Bevel_Radius_ * Value_At_T_Q35;

        float Result_Q52;
        Conditional_Float_B52(_Show_All_Internal_On_Focus_,1,MaxAB_Q93,Result_Q52);

        // Mix_Colors (#37)
        float4 Color_At_T_Q37 = lerp(Result_Q28, Color_At_T_Q43,float4( _Pinched_, _Pinched_, _Pinched_, _Pinched_));

        float3 Result_Q49;
        float3 Tangent_Q49;
        float Length_Q49;
        CreateEdge_B49(Pos_World_Q22,Pos_World_Q24,Pos_World_Q39,Pos_World_Q19,XY_Q65,Product_Q25,DirOut_Q50,WhichEdge_Q65,Result_Q49,Tangent_Q49,Length_Q49);

        // Max (#69)
        float MaxAB_Q69=max(Result_Q50,Result_Q52);

        // Incident3 (#31)
        float3 Incident_Q31;
        float ScaleWidth_Q31;
        Incident_Q31 = normalize(Result_Q49 - _Gaze_Position_);
        ScaleWidth_Q31 = max(1.0,distance(Result_Q49,_Gaze_Position_));
        
        // Lerp (#45)
        float Value_At_T_Q45=lerp(Result_Q50,MaxAB_Q69,Result_Q81);

        // Multiply (#46)
        float Product_Q46 = Product_Q27 * Value_At_T_Q45;

        // Multiply (#20)
        float Product_Q20 = Product_Q46 * 1;

        // Multiply (#32)
        float Product_Q32 = Product_Q20 * ScaleWidth_Q31;

        float3 Result_Q40;
        float U_Q40;
        FaceCamera_B40(Incident_Q31,Result_Q49,Tangent_Q49,Product_Q32,X_Q23,Result_Q40,U_Q40);

        float3 Position = Result_Q40;
        float3 Normal = float3(0,0,0);
        float2 UV = Vec2_Q41;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = Color_At_T_Q37;


        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.uv = UV;
        o.vertexColor = Color;

        return o;
    }

    //BLOCK_BEGIN FingerTipProximity 63

    void FingerTipProximity_B63(
        half3 Left_Tip,
        half3 Right_Tip,
        half Radius,
        half Finger_Tip_Glow,
        half3 Position,
        out half Result    )
    {
        float3 leftDelta = Position - Left_Tip;
        float3 rightDelta = Position - Right_Tip;
        
        half leftDistanceSqrd = dot(leftDelta,leftDelta);
        half rightDistanceSqrd = dot(rightDelta,rightDelta);
        
        half d = sqrt(min(leftDistanceSqrd, rightDistanceSqrd));
        Result = (1.0-saturate(d/Radius))*Finger_Tip_Glow;
        
    }
    //BLOCK_END FingerTipProximity


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half Result_Q63;
        FingerTipProximity_B63(Global_Left_Index_Tip_Position,Global_Right_Index_Tip_Position,_Finger_Tip_Proximity_Radius_,_Finger_Tip_Glow_,fragInput.posWorld,Result_Q63);

        // To_XY (#62)
        half X_Q62;
        half Y_Q62;
        X_Q62 = fragInput.uv.x;
        Y_Q62 = fragInput.uv.y;

        // Add_Glow (#60)
        half4 Sum_Q60;
        Sum_Q60.rgb = fragInput.vertexColor.rgb + half3(Result_Q63,Result_Q63,Result_Q63);
        Sum_Q60.a = fragInput.vertexColor.a;
        
        // Pulse (#61)
        half Pulse_Q61;
        half dX = fwidth(X_Q62)*1;
        Pulse_Q61 = max(0,min(X_Q62+dX*0.5,1.0) - max(-1.0,X_Q62-dX*0.5))/max(dX,1.0E-8);
        
        // Scale_Color (#59)
        half4 Result_Q59 = Pulse_Q61 * Sum_Q60;

        float4 Out_Color = Result_Q59;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

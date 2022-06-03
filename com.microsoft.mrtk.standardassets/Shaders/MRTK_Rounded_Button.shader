// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Shell_Rounded_Button" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.01
        _Line_Width_("Line Width", Range(0,1)) = 0.002
        [Toggle] _Relative_To_Height_("Relative To Height", Float) = 0
        _Filter_Width_("Filter Width", Range(0,4)) = 1.5
        _Edge_Color_("Edge Color", Color) = (0.53,0.53,0.53,1)
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle] _Smooth_Edges_("Smooth Edges", Float) = 1
     
    [Header(Blob)]
        [Toggle] _Blob_Enable_("Blob Enable", Float) = 1
        [PerRendererData] _Blob_Position_("Blob Position",Vector) = (100,100,100,1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.5
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.025
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.05
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.08
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.08
        _Blob_Inner_Fade_("Blob Inner Fade", Range(0.001,1)) = 0.01
        [PerRendererData] _Blob_Pulse_("Blob Pulse",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_("Blob Fade",Range(0,1)) = 1
        _Blob_Pulse_Max_Size_("Blob Pulse Max Size", Range(0,1)) = 0.05
     
    [Header(Blob 2)]
        [Toggle] _Blob_Enable_2_("Blob Enable 2", Float) = 1
        [PerRendererData] _Blob_Position_2_("Blob Position 2",Vector) = (10,10.1,-0.6,1)
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.025
        _Blob_Inner_Fade_2_("Blob Inner Fade 2", Range(0,1)) = 0.1
        [PerRendererData] _Blob_Pulse_2_("Blob Pulse 2",Range(0,1)) = 0
        [PerRendererData] _Blob_Fade_2_("Blob Fade 2",Range(0,1)) = 1
     
    [Header(Gaze)]
        _Gaze_Intensity_("Gaze Intensity", Range(0,1)) = 0.3
        [PerRendererData] _Gaze_Focus_("Gaze Focus",Range(0,1)) = 0.0
        [PerRendererData] _Pinched_("Pinched",Float) = 0.0
     
    [Header(Blob Texture)]
        [NoScaleOffset] _Blob_Texture_("Blob Texture", 2D) = "" {}
     
    [Header(Selection)]
        _Selection_Fuzz_("Selection Fuzz", Range(0,1)) = 0.5
        _Selected_("Selected", Range(0,1)) = 0
        _Selection_Fade_("Selection Fade", Range(0,1)) = 0
        _Selection_Fade_Size_("Selection Fade Size", Range(0,1)) = 0.3
        _Selected_Distance_("Selected Distance", Range(0,1)) = 0.08
        _Selected_Fade_Length_("Selected Fade Length", Range(0,1)) = 0.08
     
    [Header(Proximity)]
        _Proximity_Max_Intensity_("Proximity Max Intensity", Range(0,1)) = 0.45
        _Proximity_Far_Distance_("Proximity Far Distance", Range(0,2)) = 0.16
        _Proximity_Near_Radius_("Proximity Near Radius", Range(0,2)) = .03
        _Proximity_Anisotropy_("Proximity Anisotropy", Range(0,1)) = 1
     
    [Header(Global)]
        [PerRendererData] _Use_Global_Left_Index_("Use Global Left Index",Float) = 1
        [PerRendererData] _Use_Global_Right_Index_("Use Global Right Index",Float) = 1
     

}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    Cull Off
    ZWrite Off
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

    float _Radius_;
    float _Line_Width_;
    bool _Relative_To_Height_;
    float _Filter_Width_;
    half4 _Edge_Color_;
    half _Fade_Out_;
    bool _Smooth_Edges_;
    bool _Blob_Enable_;
    float3 _Blob_Position_;
    float _Blob_Intensity_;
    float _Blob_Near_Size_;
    float _Blob_Far_Size_;
    float _Blob_Near_Distance_;
    float _Blob_Far_Distance_;
    float _Blob_Fade_Length_;
    float _Blob_Inner_Fade_;
    float _Blob_Pulse_;
    float _Blob_Fade_;
    float _Blob_Pulse_Max_Size_;
    bool _Blob_Enable_2_;
    float3 _Blob_Position_2_;
    float _Blob_Near_Size_2_;
    float _Blob_Inner_Fade_2_;
    float _Blob_Pulse_2_;
    float _Blob_Fade_2_;
    float _Gaze_Intensity_;
    float _Gaze_Focus_;
    float _Pinched_;
    sampler2D _Blob_Texture_;
    float _Selection_Fuzz_;
    float _Selected_;
    float _Selection_Fade_;
    float _Selection_Fade_Size_;
    float _Selected_Distance_;
    float _Selected_Fade_Length_;
    half _Proximity_Max_Intensity_;
    float _Proximity_Far_Distance_;
    half _Proximity_Near_Radius_;
    float _Proximity_Anisotropy_;
    bool _Use_Global_Left_Index_;
    bool _Use_Global_Right_Index_;
    float4 Global_Left_Index_Tip_Position;
    float4 Global_Right_Index_Tip_Position;




    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float4 tangent : TANGENT;
        float4 color : COLOR;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float4 tangent : TANGENT;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN Blob_Vertex 62

    void Blob_Vertex_B62(
        float3 Position,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float3 Blob_Position,
        float Intensity,
        float Blob_Near_Size,
        float Blob_Far_Size,
        float Blob_Near_Distance,
        float Blob_Far_Distance,
        float4 Vx_Color,
        float2 UV,
        float3 Face_Center,
        float2 Face_Size,
        float2 In_UV,
        float Blob_Fade_Length,
        float Selection_Fade,
        float Selection_Fade_Size,
        float Inner_Fade,
        float Blob_Pulse,
        float Blob_Fade,
        float Blob_Enabled,
        float DistanceOffset,
        out float3 Out_Position,
        out float2 Out_UV,
        out float3 Blob_Info,
        out float2 Blob_Relative_UV    )
    {
        
        float blobSize, fadeIn;
        float3 Hit_Position;
        Blob_Info = float3(0.0,0.0,0.0);
        
        float Hit_Distance = dot(Blob_Position-Face_Center, Normal) + DistanceOffset*Blob_Far_Distance;
        Hit_Position = Blob_Position - Hit_Distance * Normal;
        
        float absD = abs(Hit_Distance);
        float lerpVal = clamp((absD-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance),0.0,1.0);
        fadeIn = 1.0-clamp((absD-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float innerFade = 1.0-clamp(-Hit_Distance/Inner_Fade,0.0,1.0);
        
        //compute blob size
        float farClip = saturate(1.0-step(Blob_Far_Distance+Blob_Fade_Length,absD));
        float size = lerp(Blob_Near_Size,Blob_Far_Size,lerpVal)*farClip;
        blobSize = lerp(size,Selection_Fade_Size,Selection_Fade)*innerFade*Blob_Enabled;
        Blob_Info.x = lerpVal*0.5+0.5;
            
        Blob_Info.y = fadeIn*Intensity*(1.0-Selection_Fade)*Blob_Fade;
        Blob_Info.x *= (1.0-Blob_Pulse);
        
        //compute blob position
        float3 delta = Hit_Position - Face_Center;
        float2 blobCenterXY = float2(dot(delta,Tangent),dot(delta,Bitangent));
        
        float2 quadUVin = 2.0*UV-1.0;  // remap to (-.5,.5)
        float2 blobXY = blobCenterXY+quadUVin*blobSize;
        
        //keep the quad within the face
        float2 blobClipped = clamp(blobXY,-Face_Size*0.5,Face_Size*0.5);
        float2 blobUV = (blobClipped-blobCenterXY)/max(blobSize,0.0001)*2.0;
        
        float3 blobCorner = Face_Center + blobClipped.x*Tangent + blobClipped.y*Bitangent;
        
        //blend using VxColor.r=1 for blob quad, 0 otherwise
        Out_Position = lerp(Position,blobCorner,Vx_Color.rrr);
        Out_UV = lerp(In_UV,blobUV,Vx_Color.rr);
        Blob_Relative_UV = blobClipped/Face_Size.y;
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Round_Rect_Vertex 53

    void Round_Rect_Vertex_B53(
        float2 UV,
        float3 Tangent,
        float3 Binormal,
        float Radius,
        float Anisotropy,
        float2 Blob_Center_UV,
        out float2 Rect_UV,
        out float2 Scale_XY,
        out float4 Rect_Parms    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Rect_UV = (UV - float2(0.5,0.5)) * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius);
        Rect_Parms.zw = Blob_Center_UV;
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Proximity_Vertex 49

    float2 ProjectProximity(
        float3 blobPosition,
        float3 position,
        float3 center,
        float3 dir,
        float3 xdir,
        float3 ydir,
        out float vdistance
    )
    {
        float3 delta = blobPosition - position;
        float2 xy = float2(dot(delta,xdir),dot(delta,ydir));
        vdistance = abs(dot(delta,dir));
        return xy;
    }
    
    void Proximity_Vertex_B49(
        float3 Blob_Position,
        float3 Blob_Position_2,
        float3 Face_Center,
        float3 Position,
        float Proximity_Far_Distance,
        float Relative_Scale,
        float Proximity_Anisotropy,
        float3 Normal,
        float3 Tangent,
        float3 Binormal,
        out float4 Extra,
        out float Distance_To_Face,
        out float Distance_Fade1,
        out float Distance_Fade2    )
    {
        //float3 Active_Face_Dir_X = normalize(cross(Active_Face_Dir,Up));
        //float3 Active_Face_Dir_X = normalize(float3(Active_Face_Dir.y-Active_Face_Dir.z,Active_Face_Dir.z-Active_Face_Dir.x,Active_Face_Dir.x-Active_Face_Dir.y));
        //float3 Active_Face_Dir_Y = cross(Active_Face_Dir,Active_Face_Dir_X);
        
        float distz1,distz2;
        Extra.xy = ProjectProximity(Blob_Position,Position,Face_Center,Normal,Tangent*Proximity_Anisotropy,Binormal,distz1)/Relative_Scale;
        Extra.zw = ProjectProximity(Blob_Position_2,Position,Face_Center,Normal,Tangent*Proximity_Anisotropy,Binormal,distz2)/Relative_Scale;
        
        Distance_To_Face = dot(Normal,Position-Face_Center);
        Distance_Fade1 = 1.0 - saturate(distz1/Proximity_Far_Distance);
        Distance_Fade2 = 1.0 - saturate(distz2/Proximity_Far_Distance);
        
    }
    //BLOCK_END Proximity_Vertex

    //BLOCK_BEGIN Object_To_World_Pos 13

    void Object_To_World_Pos_B13(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Choose_Blob 34

    void Choose_Blob_B34(
        float4 Vx_Color,
        float3 Position1,
        float3 Position2,
        bool Blob_Enable_1,
        bool Blob_Enable_2,
        float Near_Size_1,
        float Near_Size_2,
        float Blob_Inner_Fade_1,
        float Blob_Inner_Fade_2,
        float Blob_Pulse_1,
        float Blob_Pulse_2,
        float Blob_Fade_1,
        float Blob_Fade_2,
        out float3 Position,
        out float Near_Size,
        out float Inner_Fade,
        out float Blob_Enable,
        out float Fade,
        out float Pulse    )
    {
        Position = Position1*(1.0-Vx_Color.g)+Vx_Color.g*Position2;
        
        float b1 = Blob_Enable_1 ? 1.0 : 0.0;
        float b2 = Blob_Enable_2 ? 1.0 : 0.0;
        Blob_Enable = b1+(b2-b1)*Vx_Color.g;
        
        Pulse = Blob_Pulse_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Pulse_2;
        Fade = Blob_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Fade_2;
        Near_Size = Near_Size_1*(1.0-Vx_Color.g)+Vx_Color.g*Near_Size_2;
        Inner_Fade = Blob_Inner_Fade_1*(1.0-Vx_Color.g)+Vx_Color.g*Blob_Inner_Fade_2;
    }
    //BLOCK_END Choose_Blob

    //BLOCK_BEGIN Move_Verts 48

    void Move_Verts_B48(
        float2 UV,
        float Radius,
        float Anisotropy,
        float Line_Width,
        float Visible,
        out float3 New_P,
        out float2 New_UV    )
    {
        
        float2 xy = 2 * UV - float2(0.5,0.5);
        float2 center = saturate(xy);
        
        float2 delta = 2 * (xy - center);
        float deltaLength = length(delta);
        
        float2 aniso = float2(1.0 / Anisotropy, 1.0);
        center = (center-float2(0.5,0.5))*(1.0-2.0*Radius*aniso);
        
        New_UV = float2((2.0-2.0*deltaLength)*Visible,0.0);
        
        float deltaRadius =  (Radius - Line_Width * New_UV.x);
        
        New_P.xy = (center + deltaRadius / deltaLength *aniso * delta);
        New_P.z = 0.0;
        
    }
    //BLOCK_END Move_Verts

    //BLOCK_BEGIN Object_To_World_Dir 15

    void Object_To_World_Dir_B15(
        float3 Dir_Object,
        out float3 Binormal_World    )
    {
        Binormal_World = (mul((float3x3)unity_ObjectToWorld, Dir_Object));
        
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN Proximity_Visibility 51

    void Proximity_Visibility_B51(
        float Selection,
        float3 Proximity_Center,
        float3 Proximity_Center_2,
        float Proximity_Far_Distance,
        float Proximity_Radius,
        float3 Face_Center,
        float3 Normal,
        float2 Face_Size,
        float Gaze,
        out float Width    )
    {
        //make all edges invisible if no proximity or selection visible
        float boxMaxSize = length(Face_Size)*0.5;
        
        float d1 = dot(Proximity_Center-Face_Center, Normal);
        float3 blob1 = Proximity_Center - d1 * Normal;
        
        float d2 = dot(Proximity_Center_2-Face_Center, Normal);
        float3 blob2 = Proximity_Center_2 - d2 * Normal;
        
        //float3 objectOriginInWorld = (mul(_Object2World, float4(float3(0.0,0.0,0.0), 1)));
        float3 delta1 = blob1 - Face_Center;
        float3 delta2 = blob2 - Face_Center;
        
        float dist1 = dot(delta1,delta1);
        float dist2 = dot(delta2,delta2);
        
        float nearestProxDist = sqrt(min(dist1,dist2));
        
        Width = (1.0 - step(boxMaxSize+Proximity_Radius,nearestProxDist))*(1.0-step(Proximity_Far_Distance,min(d1,d2))*(1.0-step(0.0001,Selection)));
        Width = max(Gaze, Width);
    }
    //BLOCK_END Proximity_Visibility

    //BLOCK_BEGIN Selection_Vertex 47

    float2 ramp2(float2 start, float2 end, float2 x)
    {
       return clamp((x-start)/(end-start),float2(0.0,0.0),float2(1.0,1.0));
    }
    
    float computeSelection(
        float3 blobPosition,
        float3 normal,
        float3 tangent,
        float3 bitangent,
        float3 faceCenter,
        float2 faceSize,
        float selectionFuzz,
        float farDistance,
        float fadeLength
    )
    {
        float3 delta = blobPosition - faceCenter;
        float absD = abs(dot(delta,normal));
        float fadeIn = 1.0-clamp((absD-farDistance)/fadeLength,0.0,1.0);
        
        float2 blobCenterXY = float2(dot(delta,tangent),dot(delta,bitangent));
    
        float2 innerFace = faceSize * (1.0-selectionFuzz) * 0.5;
        float2 selectPulse = ramp2(-faceSize*0.5,-innerFace,blobCenterXY)-ramp2(innerFace,faceSize*0.5,blobCenterXY);
    
        return selectPulse.x * selectPulse.y * fadeIn;
    }
    
    void Selection_Vertex_B47(
        float3 Blob_Position,
        float3 Blob_Position_2,
        float3 Face_Center,
        float2 Face_Size,
        float3 Normal,
        float3 Tangent,
        float3 Bitangent,
        float Selection_Fuzz,
        float Selected,
        float Far_Distance,
        float Fade_Length,
        float3 Active_Face_Dir,
        out float Show_Selection    )
    {
        float select1 = computeSelection(Blob_Position,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length);
        float select2 = computeSelection(Blob_Position_2,Normal,Tangent,Bitangent,Face_Center,Face_Size,Selection_Fuzz,Far_Distance,Fade_Length);
        
        Show_Selection = lerp(max(select1,select2),1.0,Selected);
    }
    //BLOCK_END Selection_Vertex


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Pack_For_Vertex (#37)
        float3 Vec3_Q37 = float3(float2(0,0).x,float2(0,0).y,vertInput.color.r);

        // Object_To_World_Dir (#28)
        float3 Nrm_World_Q28;
        Nrm_World_Q28 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        // Object_To_World_Pos (#46)
        float3 Face_Center_Q46;
        Face_Center_Q46=(mul(unity_ObjectToWorld, float4(float3(0,0,0), 1)));
        
        // Object_To_World_Dir (#14)
        float3 Tangent_World_Q14;
        Tangent_World_Q14 = (mul((float3x3)unity_ObjectToWorld, vertInput.tangent));
        
        float3 Binormal_World_Q15;
        Object_To_World_Dir_B15((normalize(cross(vertInput.normal,vertInput.tangent))),Binormal_World_Q15);

        // Anisotropy (#22)
        float Anisotropy_Q22=length(Tangent_World_Q14)/length(Binormal_World_Q15);

        // Conditional (#77)
        float3 Result_Q77;
        Result_Q77 = _Use_Global_Left_Index_ ? Global_Left_Index_Tip_Position.xyz : _Blob_Position_;
        
        // Conditional (#78)
        float3 Result_Q78;
        Result_Q78 = _Use_Global_Right_Index_ ? Global_Right_Index_Tip_Position.xyz : _Blob_Position_2_;
        
        // Lerp (#124)
        float Value_At_T_Q124=lerp(_Blob_Near_Size_,_Blob_Pulse_Max_Size_,_Blob_Pulse_);

        // Lerp (#126)
        float Value_At_T_Q126=lerp(_Blob_Near_Size_2_,_Blob_Pulse_Max_Size_,_Blob_Pulse_2_);

        // Multiply (#93)
        float Product_Q93 = _Gaze_Intensity_ * _Gaze_Focus_;

        // Step (#95)
        float Step_Q95 = step(0.0001, Product_Q93);

        // Normalize3 (#16)
        float3 Tangent_World_N_Q16 = normalize(Tangent_World_Q14);

        // Normalize3 (#17)
        float3 Binormal_World_N_Q17 = normalize(Binormal_World_Q15);

        float3 Position_Q34;
        float Near_Size_Q34;
        float Inner_Fade_Q34;
        float Blob_Enable_Q34;
        float Fade_Q34;
        float Pulse_Q34;
        Choose_Blob_B34(vertInput.color,Result_Q77,Result_Q78,_Blob_Enable_,_Blob_Enable_2_,Value_At_T_Q124,Value_At_T_Q126,_Blob_Inner_Fade_,_Blob_Inner_Fade_2_,_Blob_Pulse_,_Blob_Pulse_2_,_Blob_Fade_,_Blob_Fade_2_,Position_Q34,Near_Size_Q34,Inner_Fade_Q34,Blob_Enable_Q34,Fade_Q34,Pulse_Q34);

        // Face_Size (#52)
        float2 Face_Size_Q52;
        float ScaleY_Q52;
        Face_Size_Q52 = float2(length(Tangent_World_Q14),length(Binormal_World_Q15));
        ScaleY_Q52 = Face_Size_Q52.y;
        
        // Scale_Radius_And_Width (#56)
        float Out_Radius_Q56;
        float Out_Line_Width_Q56;
        Out_Radius_Q56 = _Relative_To_Height_ ? _Radius_ : _Radius_ / ScaleY_Q52;
        Out_Line_Width_Q56 = _Relative_To_Height_ ? _Line_Width_ : _Line_Width_ / ScaleY_Q52;

        float Show_Selection_Q47;
        Selection_Vertex_B47(Result_Q77,Result_Q78,Face_Center_Q46,Face_Size_Q52,Nrm_World_Q28,Tangent_World_N_Q16,Binormal_World_N_Q17,_Selection_Fuzz_,_Selected_,_Selected_Distance_,_Selected_Fade_Length_,float3(0,0,-1),Show_Selection_Q47);

        // Max (#72)
        float MaxAB_Q72=max(Show_Selection_Q47,Product_Q93);

        float Width_Q51;
        Proximity_Visibility_B51(Show_Selection_Q47,Result_Q77,Result_Q78,_Proximity_Far_Distance_,_Proximity_Near_Radius_,Face_Center_Q46,Nrm_World_Q28,Face_Size_Q52,Step_Q95,Width_Q51);

        float3 New_P_Q48;
        float2 New_UV_Q48;
        Move_Verts_B48(vertInput.uv0,Out_Radius_Q56,Anisotropy_Q22,Out_Line_Width_Q56,Width_Q51,New_P_Q48,New_UV_Q48);

        float3 Pos_World_Q13;
        Object_To_World_Pos_B13(New_P_Q48,Pos_World_Q13);

        float3 Out_Position_Q62;
        float2 Out_UV_Q62;
        float3 Blob_Info_Q62;
        float2 Blob_Relative_UV_Q62;
        Blob_Vertex_B62(Pos_World_Q13,Nrm_World_Q28,Tangent_World_N_Q16,Binormal_World_N_Q17,Position_Q34,_Blob_Intensity_,Near_Size_Q34,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,vertInput.color,vertInput.uv0,Face_Center_Q46,Face_Size_Q52,New_UV_Q48,_Blob_Fade_Length_,_Selection_Fade_,_Selection_Fade_Size_,Inner_Fade_Q34,Pulse_Q34,Fade_Q34,Blob_Enable_Q34,0.0,Out_Position_Q62,Out_UV_Q62,Blob_Info_Q62,Blob_Relative_UV_Q62);

        float2 Rect_UV_Q53;
        float2 Scale_XY_Q53;
        float4 Rect_Parms_Q53;
        Round_Rect_Vertex_B53(New_UV_Q48,Tangent_World_Q14,Binormal_World_Q15,Out_Radius_Q56,Anisotropy_Q22,Blob_Relative_UV_Q62,Rect_UV_Q53,Scale_XY_Q53,Rect_Parms_Q53);

        float4 Extra_Q49;
        float Distance_To_Face_Q49;
        float Distance_Fade1_Q49;
        float Distance_Fade2_Q49;
        Proximity_Vertex_B49(Result_Q77,Result_Q78,Face_Center_Q46,Pos_World_Q13,_Proximity_Far_Distance_,1.0,_Proximity_Anisotropy_,Nrm_World_Q28,Tangent_World_N_Q16,Binormal_World_N_Q17,Extra_Q49,Distance_To_Face_Q49,Distance_Fade1_Q49,Distance_Fade2_Q49);

        // From_XYZW (#54)
        float4 Vec4_Q54 = float4(MaxAB_Q72, Distance_Fade1_Q49, Distance_Fade2_Q49, Out_Radius_Q56);

        float3 Position = Out_Position_Q62;
        float3 Normal = Vec3_Q37;
        float2 UV = Out_UV_Q62;
        float3 Tangent = Blob_Info_Q62;
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);
        float4 Extra1 = Rect_Parms_Q53;
        float4 Extra2 = Extra_Q49;
        float4 Extra3 = Vec4_Q54;


        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    //BLOCK_BEGIN Scale_Color 39

    void Scale_Color_B39(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = Scalar * Color;
    }
    //BLOCK_END Scale_Color

    //BLOCK_BEGIN Scale_RGB 91

    void Scale_RGB_B91(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Proximity_Fragment 61

    void Proximity_Fragment_B61(
        half Proximity_Max_Intensity,
        half Proximity_Near_Radius,
        half4 Deltas,
        half Show_Selection,
        half Distance_Fade1,
        half Distance_Fade2,
        half Strength,
        out half Proximity    )
    {
        float proximity1 = (1.0-saturate(length(Deltas.xy)/Proximity_Near_Radius))*Distance_Fade1;
        float proximity2 = (1.0-saturate(length(Deltas.zw)/Proximity_Near_Radius))*Distance_Fade2;
        
        Proximity = Strength * (Proximity_Max_Intensity * max(proximity1, proximity2) *(1.0-Show_Selection)+Show_Selection);
        
    }
    //BLOCK_END Proximity_Fragment

    //BLOCK_BEGIN Blob_Fragment 36

    void Blob_Fragment_B36(
        float2 UV,
        float3 Blob_Info,
        sampler2D Blob_Texture,
        out half4 Blob_Color    )
    {
        float k = dot(UV,UV);
        Blob_Color = Blob_Info.y * tex2D(Blob_Texture,float2(float2(sqrt(k),Blob_Info.x).x,1.0-float2(sqrt(k),Blob_Info.x).y))*(1.0-saturate(k));
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN Round_Rect_Fragment 68

    void Round_Rect_Fragment_B68(
        half Radius,
        half4 Line_Color,
        half Filter_Width,
        half Line_Visibility,
        half4 Fill_Color,
        bool Smooth_Edges,
        half4 Rect_Parms,
        out half Inside_Rect    )
    {
        half d = length(max(abs(Rect_Parms.zw)-Rect_Parms.xy,0.0));
        half dx = max(fwidth(d)*Filter_Width,0.00001);
        
        Inside_Rect = Smooth_Edges ? saturate((Radius-d)/dx) : 1.0-step(Radius,d);
        
    }
    //BLOCK_END Round_Rect_Fragment


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // Is_Quad (#38)
        half Is_Quad_Q38;
        Is_Quad_Q38=fragInput.normalWorld.xyz.z;
        
        half4 Blob_Color_Q36;
        Blob_Fragment_B36(fragInput.uv,fragInput.tangent.xyz,_Blob_Texture_,Blob_Color_Q36);

        // To_XYZW (#55)
        half X_Q55;
        half Y_Q55;
        half Z_Q55;
        half W_Q55;
        X_Q55=fragInput.extra3.x;
        Y_Q55=fragInput.extra3.y;
        Z_Q55=fragInput.extra3.z;
        W_Q55=fragInput.extra3.w;

        half Proximity_Q61;
        Proximity_Fragment_B61(_Proximity_Max_Intensity_,_Proximity_Near_Radius_,fragInput.extra2,X_Q55,Y_Q55,Z_Q55,1.0,Proximity_Q61);

        half Inside_Rect_Q68;
        Round_Rect_Fragment_B68(W_Q55,half4(1,1,1,1),_Filter_Width_,1,half4(0,0,0,0),_Smooth_Edges_,fragInput.extra1,Inside_Rect_Q68);

        half4 Result_Q91;
        Scale_RGB_B91(_Edge_Color_,Proximity_Q61,Result_Q91);

        // Scale_Color (#40)
        half4 Result_Q40 = Inside_Rect_Q68 * Blob_Color_Q36;

        // Mix_Colors (#45)
        half4 Color_At_T_Q45 = lerp(Result_Q91, Result_Q40,float4( Is_Quad_Q38, Is_Quad_Q38, Is_Quad_Q38, Is_Quad_Q38));

        half4 Result_Q39;
        Scale_Color_B39(Color_At_T_Q45,_Fade_Out_,Result_Q39);

        float4 Out_Color = Result_Q39;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

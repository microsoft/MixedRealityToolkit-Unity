// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Beveled_Edgy" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.2
        _Bevel_Front_("Bevel Front", Range(0,1)) = 0.07
        _Bevel_Front_Stretch_("Bevel Front Stretch", Range(0,1)) = 0
        _Bevel_Back_("Bevel Back", Range(0,1)) = 0.02
        _Bevel_Back_Stretch_("Bevel Back Stretch", Range(0,1)) = 0
     
    [Header(Radii Multipliers)]
        _Radius_Top_Left_("Radius Top Left", Range(0,1)) = 1
        _Radius_Top_Right_("Radius Top Right", Range(0,1)) = 1.0
        _Radius_Bottom_Left_("Radius Bottom Left", Range(0,1)) = 1.0
        _Radius_Bottom_Right_("Radius Bottom Right", Range(0,1)) = 1.0
     
    [Header(Bulge)]
        [Toggle] _Bulge_Enabled_("Bulge Enabled", Float) = 0
        _Bulge_Height_("Bulge Height", Range(-1,1)) = -0.323
        _Bulge_Radius_("Bulge Radius", Range(0,1.25)) = 0.73
     
    [Header(Sun)]
        _Sun_Intensity_("Sun Intensity", Range(0,2)) = 0
        _Sun_Theta_("Sun Theta", Range(0,1)) = 0.73
        _Sun_Phi_("Sun Phi", Range(0,1)) = 0.48
        _Indirect_Diffuse_("Indirect Diffuse", Range(0,1)) = 0.51
     
    [Header(Diffuse And Specular)]
        _Albedo_("Albedo", Color) = (1,1,1,1)
        _Specular_("Specular", Range(0,5)) = 0
        _Shininess_("Shininess", Range(0,10)) = 10
        _Sharpness_("Sharpness", Range(0,1)) = 0
        _Subsurface_("Subsurface", Range(0,1)) = 0
     
    [Header(Gradient)]
        _Left_Color_("Left Color", Color) = (1,1,1,1)
        _Right_Color_("Right Color", Color) = (1,1,1,1)
     
    [Header(Reflection)]
        _Reflection_("Reflection", Range(0,2)) = 0
        _Front_Reflect_("Front Reflect", Range(0,1)) = 0
        _Edge_Reflect_("Edge Reflect", Range(0,1)) = 1
        _Power_("Power", Range(0,10)) = 1
     
    [Header(Sky Environment)]
        [Toggle(_SKY_ENABLED_)] _Sky_Enabled_("Sky Enabled", Float) = 1
        _Sky_Color_("Sky Color", Color) = (0.866667,0.917647,1,1)
        _Horizon_Color_("Horizon Color", Color) = (0.843137,0.87451,1,1)
        _Ground_Color_("Ground Color", Color) = (0.603922,0.611765,0.764706,1)
        _Horizon_Power_("Horizon Power", Range(0,10)) = 1
     
    [Header(Mapped Environment)]
        [Toggle(_ENV_ENABLE_)] _Env_Enable_("Env Enable", Float) = 0
        [NoScaleOffset] _Reflection_Map_("Reflection Map", Cube) = "" {}
        [NoScaleOffset] _Indirect_Environment_("Indirect Environment", Cube) = "" {}
     
    [Header(FingerOcclusion)]
        [Toggle(_OCCLUSION_ENABLED_)] _Occlusion_Enabled_("Occlusion Enabled", Float) = 0
        _Width_("Width", Range(0,1)) = 0.02
        _Fuzz_("Fuzz", Range(0,1)) = 0.5
        _Min_Fuzz_("Min Fuzz", Range(0,1)) = 0.001
        _Clip_Fade_("Clip Fade", Range(0,1)) = 0.01
     
    [Header(View Based Color Shift)]
        _Hue_Shift_("Hue Shift", Range(-1,1)) = 0
        _Saturation_Shift_("Saturation Shift", Range(-1,1)) = 0
        _Value_Shift_("Value Shift", Range(-1,1)) = 0
     
    [Header(Blob)]
        [Toggle(_BLOB_ENABLE_)] _Blob_Enable_("Blob Enable", Float) = 0
        _Blob_Position_("Blob Position", Vector) = (0, 0, 0.1, 1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.5
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.01
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.03
        _Blob_Near_Distance_("Blob Near Distance", Range(0,1)) = 0
        _Blob_Far_Distance_("Blob Far Distance", Range(0,1)) = 0.08
        _Blob_Fade_Length_("Blob Fade Length", Range(0,1)) = 0.08
        _Blob_Pulse_("Blob Pulse", Range(0,1)) = 0
        _Blob_Fade_("Blob Fade", Range(0,1)) = 1
     
    [Header(Blob Texture)]
        [NoScaleOffset] _Blob_Texture_("Blob Texture", 2D) = "" {}
     
    [Header(Blob 2)]
        [Toggle(_BLOB_ENABLE_2_)] _Blob_Enable_2_("Blob Enable 2", Float) = 1
        _Blob_Position_2_("Blob Position 2", Vector) = (0.2, 0, 0.1, 1)
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.01
        _Blob_Pulse_2_("Blob Pulse 2", Range(0,1)) = 0
        _Blob_Fade_2_("Blob Fade 2", Range(0,1)) = 1
     
    [Header(Finger Positions)]
        _Left_Index_Pos_("Left Index Pos", Vector) = (0, 0, 1, 1)
        _Right_Index_Pos_("Right Index Pos", Vector) = (-1, -1, -1, 1)
        _Left_Index_Middle_Pos_("Left Index Middle Pos", Vector) = (0, 0, 0, 1)
        _Right_Index_Middle_Pos_("Right Index Middle Pos", Vector) = (0, 0, 0, 1)
     
    [Header(Decal Texture)]
        [Toggle(_DECAL_ENABLE_)] _Decal_Enable_("Decal Enable", Float) = 0
        [NoScaleOffset] _Decal_("Decal", 2D) = "" {}
        _Decal_Scale_XY_("Decal Scale XY", Vector) = (1.5,1.5,0,0)
        [Toggle] _Decal_Front_Only_("Decal Front Only", Float) = 1
     
    [Header(Rim Light)]
        _Rim_Intensity_("Rim Intensity", Range(0,2)) = 0.3
        _Rim_Power_("Rim Power", Range(0,10)) = 1
        [NoScaleOffset] _Rim_Texture_("Rim Texture", 2D) = "" {}
        _Rim_Hue_Shift_("Rim Hue Shift", Range(-1,1)) = 0
        _Rim_Saturation_Shift_("Rim Saturation Shift", Range(-1,1)) = 0.0
        _Rim_Value_Shift_("Rim Value Shift", Range(-1,1)) = 0.0
     
    [Header(Iridescence)]
        [Toggle(_IRIDESCENCE_ENABLED_)] _Iridescence_Enabled_("Iridescence Enabled", Float) = 0
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0
        [NoScaleOffset] _Iridescence_Texture_("Iridescence Texture", 2D) = "" {}
     

    [Header(Global)]
        [Toggle] Use_Global_Left_Index("Use Global Left Index", Float) = 0
        [Toggle] Use_Global_Right_Index("Use Global Right Index", Float) = 0
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    Tags {"DisableBatching" = "True"}
	Zwrite On
    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _ENV_ENABLE_
    #pragma multi_compile _ _OCCLUSION_ENABLED_
    #pragma multi_compile _ _BLOB_ENABLE_2_
    #pragma multi_compile _ _BLOB_ENABLE_
    #pragma multi_compile _ _DECAL_ENABLE_
    #pragma multi_compile _ _SKY_ENABLED_
    #pragma multi_compile _ _IRIDESCENCE_ENABLED_

    #include "UnityCG.cginc"

    sampler2D _Blob_Texture_;
    //bool _Env_Enable_;
    samplerCUBE _Reflection_Map_;
    samplerCUBE _Indirect_Environment_;
    float3 _Left_Index_Pos_;
    float3 _Right_Index_Pos_;
    float3 _Left_Index_Middle_Pos_;
    float3 _Right_Index_Middle_Pos_;
    //bool _Occlusion_Enabled_;
    float _Width_;
    float _Fuzz_;
    float _Min_Fuzz_;
    float _Clip_Fade_;
    //bool _Blob_Enable_2_;
    float3 _Blob_Position_2_;
    float _Blob_Near_Size_2_;
    float _Blob_Pulse_2_;
    float _Blob_Fade_2_;
    //bool _Blob_Enable_;
    float3 _Blob_Position_;
    float _Blob_Intensity_;
    float _Blob_Near_Size_;
    float _Blob_Far_Size_;
    float _Blob_Near_Distance_;
    float _Blob_Far_Distance_;
    float _Blob_Fade_Length_;
    float _Blob_Pulse_;
    float _Blob_Fade_;
    float _Radius_Top_Left_;
    float _Radius_Top_Right_;
    float _Radius_Bottom_Left_;
    float _Radius_Bottom_Right_;
    bool _Bulge_Enabled_;
    float _Bulge_Height_;
    float _Bulge_Radius_;
    //bool _Decal_Enable_;
    sampler2D _Decal_;
    float2 _Decal_Scale_XY_;
    bool _Decal_Front_Only_;
    float4 _Left_Color_;
    float4 _Right_Color_;
    float _Hue_Shift_;
    float _Saturation_Shift_;
    float _Value_Shift_;
    float _Sun_Intensity_;
    float _Sun_Theta_;
    float _Sun_Phi_;
    float _Indirect_Diffuse_;
    float _Radius_;
    float _Bevel_Front_;
    float _Bevel_Front_Stretch_;
    float _Bevel_Back_;
    float _Bevel_Back_Stretch_;
    float4 _Albedo_;
    float _Specular_;
    float _Shininess_;
    float _Sharpness_;
    float _Subsurface_;
    //bool _Sky_Enabled_;
    float4 _Sky_Color_;
    float4 _Horizon_Color_;
    float4 _Ground_Color_;
    float _Horizon_Power_;
    //bool _Iridescence_Enabled_;
    float _Iridescence_Intensity_;
    sampler2D _Iridescence_Texture_;
    float _Reflection_;
    float _Front_Reflect_;
    float _Edge_Reflect_;
    float _Power_;
    float _Rim_Intensity_;
    float _Rim_Power_;
    sampler2D _Rim_Texture_;
    float _Rim_Hue_Shift_;
    float _Rim_Saturation_Shift_;
    float _Rim_Value_Shift_;

    bool Use_Global_Left_Index;
    bool Use_Global_Right_Index;
    float4 Global_Left_Index_Tip_Position;
    float4 Global_Right_Index_Tip_Position;
	float4 Global_Left_Index_Middle_Position;
	float4 Global_Right_Index_Middle_Position;
    float4 Global_Left_Thumb_Tip_Position;
    float4 Global_Right_Thumb_Tip_Position;
    float  Global_Left_Index_Tip_Proximity;
    float  Global_Right_Index_Tip_Proximity;



    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
        float4 binormal : TEXCOORD6;
        float4 vertexColor : COLOR;
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
        float4 extra3 : TEXCOORD2;
      UNITY_VERTEX_OUTPUT_STEREO
    };

    // declare parm vars here

    //BLOCK_BEGIN Object_To_World_Pos 17

    void Object_To_World_Pos_B17(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Object_To_World_Normal 37

    void Object_To_World_Normal_B37(
        float3 Nrm_Object,
        out float3 Nrm_World    )
    {
        Nrm_World=UnityObjectToWorldNormal(Nrm_Object);
        
    }
    //BLOCK_END Object_To_World_Normal

    //BLOCK_BEGIN Blob_Vertex 28

    void Blob_Vertex_B28(
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
        float Blob_Fade_Length,
        float Blob_Pulse,
        float Blob_Fade,
        out float4 Blob_Info    )
    {
        
        float3 blob =  (Use_Global_Left_Index ? Global_Left_Index_Tip_Position.xyz :  Blob_Position);
        float3 delta = blob - Position;
        float dist = dot(Normal,delta);
        
        float lerpValue = saturate((abs(dist)-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance));
        float fadeValue = 1.0-clamp((abs(dist)-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float size = Blob_Near_Size + (Blob_Far_Size-Blob_Near_Size)*lerpValue;
        
        float2 blobXY = float2(dot(delta,Tangent),dot(delta,Bitangent))/(0.0001+size);
        
        float Fade = fadeValue*Intensity*Blob_Fade;
        
        float Distance = (lerpValue*0.5+0.5)*(1.0-Blob_Pulse);
        Blob_Info = float4(blobXY.x,blobXY.y,Distance,Fade);
        
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Blob_Vertex 29

    void Blob_Vertex_B29(
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
        float Blob_Fade_Length,
        float Blob_Pulse,
        float Blob_Fade,
        out float4 Blob_Info    )
    {
        
        float3 blob =  (Use_Global_Right_Index ? Global_Right_Index_Tip_Position.xyz :  Blob_Position);
        float3 delta = blob - Position;
        float dist = dot(Normal,delta);
        
        float lerpValue = saturate((abs(dist)-Blob_Near_Distance)/(Blob_Far_Distance-Blob_Near_Distance));
        float fadeValue = 1.0-clamp((abs(dist)-Blob_Far_Distance)/Blob_Fade_Length,0.0,1.0);
        
        float size = Blob_Near_Size + (Blob_Far_Size-Blob_Near_Size)*lerpValue;
        
        float2 blobXY = float2(dot(delta,Tangent),dot(delta,Bitangent))/(0.0001+size);
        
        float Fade = fadeValue*Intensity*Blob_Fade;
        
        float Distance = (lerpValue*0.5+0.5)*(1.0-Blob_Pulse);
        Blob_Info = float4(blobXY.x,blobXY.y,Distance,Fade);
        
    }
    //BLOCK_END Blob_Vertex

    //BLOCK_BEGIN Move_Verts 140

    void Move_Verts_B140(
        float Anisotropy,
        float3 P,
        float Radius,
        float Bevel,
        float3 Normal_Object,
        float ScaleZ,
        float Stretch,
        out float3 New_P,
        out float2 New_UV,
        out float Radial_Gradient,
        out float3 Radial_Dir,
        out float3 New_Normal    )
    {
        float2 UV = P.xy * 2 + 0.5;
        float2 center = saturate(UV);
        float2 delta = UV - center;
        float deltad = (length(delta)*2);
        float f = (Bevel+(Radius-Bevel)*Stretch)/Radius;
        //float br = saturate((deltad-(1-f))/f);
        float innerd = saturate(deltad*2);
        float outerd = saturate(deltad*2-1);
        float bevelAngle = outerd*3.14159*0.5;
        float sinb = sin(bevelAngle);
        float cosb = cos(bevelAngle);
        float beveld = (1-f)*innerd + f * sinb;
        float br = outerd;
        float2 r2 = 2.0 * float2(Radius / Anisotropy, Radius);
        
        float dir = P.z<0.0001 ? 1.0 : -1.0;
        
        //New_UV = center + r2 * (UV - 2 * center + 0.5);
        New_UV = center + r2 * ((0.5-center)+normalize(delta+float2(0.0,0.000001))*beveld*0.5);
        New_P = float3(New_UV - 0.5, P.z+dir*(1-cosb)*Bevel*ScaleZ);
                
        Radial_Gradient = saturate((deltad-0.5)*2);
        Radial_Dir = float3(delta * r2, 0.0);
        
        float3 beveledNormal = cosb*Normal_Object + sinb*float3(delta.x,delta.y,0.0);
        New_Normal = Normal_Object.z==0 ? Normal_Object : beveledNormal;
        
    }
    //BLOCK_END Move_Verts

    //BLOCK_BEGIN Object_To_World_Dir 65

    void Object_To_World_Dir_B65(
        float3 Dir_Object,
        out float3 Normal_World,
        out float3 Normal_World_N,
        out float Normal_Length    )
    {
        Normal_World = (mul((float3x3)unity_ObjectToWorld, Dir_Object));
        Normal_Length = length(Normal_World);
        Normal_World_N = Normal_World / Normal_Length;
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN To_XYZ 83

    void To_XYZ_B83(
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

    //BLOCK_BEGIN Conditional_Float 98

    void Conditional_Float_B98(
        bool Which,
        float If_True,
        float If_False,
        out float Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Float

    //BLOCK_BEGIN Object_To_World_Dir 33

    void Object_To_World_Dir_B33(
        float3 Dir_Object,
        out float3 Binormal_World,
        out float3 Binormal_World_N,
        out float Binormal_Length    )
    {
        Binormal_World = (mul((float3x3)unity_ObjectToWorld, Dir_Object));
        Binormal_Length = length(Binormal_World);
        Binormal_World_N = Binormal_World / Binormal_Length;
    }
    //BLOCK_END Object_To_World_Dir

    //BLOCK_BEGIN Pick_Radius 74

    void Pick_Radius_B74(
        float Radius,
        float Radius_Top_Left,
        float Radius_Top_Right,
        float Radius_Bottom_Left,
        float Radius_Bottom_Right,
        float3 Position,
        out float Result    )
    {
        bool whichY = Position.y>0;
        Result = Position.x<0 ? (whichY ? Radius_Top_Left : Radius_Bottom_Left) : (whichY ? Radius_Top_Right : Radius_Bottom_Right);
        Result *= Radius;
    }
    //BLOCK_END Pick_Radius

    //BLOCK_BEGIN Conditional_Float 41

    void Conditional_Float_B41(
        bool Which,
        float If_True,
        float If_False,
        out float Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Float

    //BLOCK_BEGIN Greater_Than 42

    void Greater_Than_B42(
        float Left,
        float Right,
        out bool Not_Greater_Than,
        out bool Greater_Than    )
    {
        Greater_Than = Left > Right;
        Not_Greater_Than = !Greater_Than;
        
    }
    //BLOCK_END Greater_Than

    //BLOCK_BEGIN Remap_Range 110

    void Remap_Range_B110(
        float In_Min,
        float In_Max,
        float Out_Min,
        float Out_Max,
        float In,
        out float Out    )
    {
        Out = lerp(Out_Min,Out_Max,clamp((In-In_Min)/(In_Max-In_Min),0,1));
        
    }
    //BLOCK_END Remap_Range


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Tex_Coords
        float2 XY_Q90;
        XY_Q90 = (vertInput.uv0-float2(0.5,0.5))*_Decal_Scale_XY_ + float2(0.5,0.5);
        
        // Object_To_World_Dir
        float3 Tangent_World_Q32;
        float3 Tangent_World_N_Q32;
        float Tangent_Length_Q32;
        Tangent_World_Q32 = (mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
        Tangent_Length_Q32 = length(Tangent_World_Q32);
        Tangent_World_N_Q32 = Tangent_World_Q32 / Tangent_Length_Q32;

        float3 Normal_World_Q65;
        float3 Normal_World_N_Q65;
        float Normal_Length_Q65;
        Object_To_World_Dir_B65(float3(0,0,1),Normal_World_Q65,Normal_World_N_Q65,Normal_Length_Q65);

        float X_Q83;
        float Y_Q83;
        float Z_Q83;
        To_XYZ_B83(vertInput.vertex.xyz,X_Q83,Y_Q83,Z_Q83);

        // Object_To_World_Dir
        float3 Nrm_World_Q31;
        Nrm_World_Q31 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        float3 Binormal_World_Q33;
        float3 Binormal_World_N_Q33;
        float Binormal_Length_Q33;
        Object_To_World_Dir_B33(float3(0,1,0),Binormal_World_Q33,Binormal_World_N_Q33,Binormal_Length_Q33);

        // Divide
        float Anisotropy_Q34 = Tangent_Length_Q32 / Binormal_Length_Q33;

        float Result_Q74;
        Pick_Radius_B74(_Radius_,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q74);

        // Divide
        float Anisotropy_Q58 = Binormal_Length_Q33 / Normal_Length_Q65;

        bool Not_Greater_Than_Q42;
        bool Greater_Than_Q42;
        Greater_Than_B42(Z_Q83,0,Not_Greater_Than_Q42,Greater_Than_Q42);

        // FastsRGBtoLinear
        float4 Linear_Q106;
        Linear_Q106.rgb = saturate(_Left_Color_.rgb*_Left_Color_.rgb);
        Linear_Q106.a=_Left_Color_.a;
        
        // FastsRGBtoLinear
        float4 Linear_Q107;
        Linear_Q107.rgb = saturate(_Right_Color_.rgb*_Right_Color_.rgb);
        Linear_Q107.a=_Right_Color_.a;
        
        // Subtract3
        float3 Difference_Q66 = float3(0,0,0) - Normal_World_N_Q65;

        // From_RGBA
        float4 Out_Color_Q39 = float4(X_Q83, Y_Q83, Z_Q83, 1);

        float Result_Q41;
        Conditional_Float_B41(Greater_Than_Q42,_Bevel_Back_,_Bevel_Front_,Result_Q41);

        float Result_Q99;
        Conditional_Float_B41(Greater_Than_Q42,_Bevel_Back_Stretch_,_Bevel_Front_Stretch_,Result_Q99);

        float3 New_P_Q140;
        float2 New_UV_Q140;
        float Radial_Gradient_Q140;
        float3 Radial_Dir_Q140;
        float3 New_Normal_Q140;
        Move_Verts_B140(Anisotropy_Q34,vertInput.vertex.xyz,Result_Q74,Result_Q41,vertInput.normal,Anisotropy_Q58,Result_Q99,New_P_Q140,New_UV_Q140,Radial_Gradient_Q140,Radial_Dir_Q140,New_Normal_Q140);

        // To_XY
        float X_Q103;
        float Y_Q103;
        X_Q103 = New_UV_Q140.x;
        Y_Q103 = New_UV_Q140.y;

        float3 Pos_World_Q17;
        Object_To_World_Pos_B17(New_P_Q140,Pos_World_Q17);

        float3 Nrm_World_Q37;
        Object_To_World_Normal_B37(New_Normal_Q140,Nrm_World_Q37);

        float4 Blob_Info_Q28;
        #if defined(_BLOB_ENABLE_)
          Blob_Vertex_B28(Pos_World_Q17,Nrm_World_Q31,Tangent_World_N_Q32,Binormal_World_N_Q33,_Blob_Position_,_Blob_Intensity_,_Blob_Near_Size_,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,_Blob_Fade_Length_,_Blob_Pulse_,_Blob_Fade_,Blob_Info_Q28);
        #else
          Blob_Info_Q28 = float4(0,0,0,0);
        #endif

        float4 Blob_Info_Q29;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Vertex_B29(Pos_World_Q17,Nrm_World_Q31,Tangent_World_N_Q32,Binormal_World_N_Q33,_Blob_Position_2_,_Blob_Intensity_,_Blob_Near_Size_2_,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,_Blob_Fade_Length_,_Blob_Pulse_2_,_Blob_Fade_2_,Blob_Info_Q29);
        #else
          Blob_Info_Q29 = float4(0,0,0,0);
        #endif

        float Out_Q110;
        Remap_Range_B110(0,1,0,1,X_Q103,Out_Q110);

        float X_Q91;
        float Y_Q91;
        float Z_Q91;
        To_XYZ_B83(Nrm_World_Q37,X_Q91,Y_Q91,Z_Q91);

        // Mix_Colors
        float4 Color_At_T_Q102 = lerp(Linear_Q106, Linear_Q107,float4( Out_Q110, Out_Q110, Out_Q110, Out_Q110));

        // Negate
        float Minus_F_Q92 = -Z_Q91;

        // To_RGBA
        float R_Q104;
        float G_Q104;
        float B_Q104;
        float A_Q104;
        R_Q104=Color_At_T_Q102.r; G_Q104=Color_At_T_Q102.g; B_Q104=Color_At_T_Q102.b; A_Q104=Color_At_T_Q102.a;

        // Clamp
        float ClampF_Q93=clamp(0,Minus_F_Q92,1);

        float Result_Q98;
        Conditional_Float_B98(_Decal_Front_Only_,ClampF_Q93,1,Result_Q98);

        // From_XYZW
        float4 Vec4_Q94 = float4(Result_Q98, Radial_Gradient_Q140, G_Q104, B_Q104);

        float3 Position = Pos_World_Q17;
        float3 Normal = Nrm_World_Q37;
        float2 UV = XY_Q90;
        float3 Tangent = Tangent_World_N_Q32;
        float3 Binormal = Difference_Q66;
        float4 Color = Out_Color_Q39;
        float4 Extra1 = Vec4_Q94;
        float4 Extra2 = Blob_Info_Q28;
        float4 Extra3 = Blob_Info_Q29;


        o.pos = UnityObjectToClipPos(vertInput.vertex);
        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.vertexColor = Color;
        o.extra1=Extra1;
        o.extra2=Extra2;
        o.extra3=Extra3;

        return o;
    }

    //BLOCK_BEGIN Blob_Fragment 35

    void Blob_Fragment_B35(
        sampler2D Blob_Texture,
        float4 Blob_Info1,
        float4 Blob_Info2,
        out half4 Blob_Color    )
    {
        half k1 = dot(Blob_Info1.xy,Blob_Info1.xy);
        half k2 = dot(Blob_Info2.xy,Blob_Info2.xy);
        half3 closer = k1<k2 ? half3(k1,Blob_Info1.z,Blob_Info1.w) : half3(k2,Blob_Info2.z,Blob_Info2.w);
        Blob_Color = closer.z * tex2D(Blob_Texture,float2(float2(sqrt(closer.x),closer.y).x,1.0-float2(sqrt(closer.x),closer.y).y))*saturate(1.0-closer.x);
        
    }
    //BLOCK_END Blob_Fragment

    //BLOCK_BEGIN FastLinearTosRGB 47

    void FastLinearTosRGB_B47(
        float4 Linear,
        out float4 sRGB    )
    {
        sRGB.rgb = sqrt(saturate(Linear.rgb));
        sRGB.a = Linear.a;
        
    }
    //BLOCK_END FastLinearTosRGB

    //BLOCK_BEGIN Scale_RGB 64

    void Scale_RGB_B64(
        float4 Color,
        float Scalar,
        out float4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Fragment_Main 129

    void Fragment_Main_B129(
        float Sun_Intensity,
        float Sun_Theta,
        float Sun_Phi,
        float3 Normal,
        float4 Albedo,
        float Fresnel_Reflect,
        float Shininess,
        float3 Incident,
        float4 Horizon_Color,
        float4 Sky_Color,
        float4 Ground_Color,
        float Indirect_Diffuse,
        float Specular,
        float Horizon_Power,
        float Reflection,
        float4 Reflection_Sample,
        half4 Indirect_Sample,
        float Sharpness,
        float SSS,
        float Subsurface,
        float4 Translucence,
        float4 Rim_Light,
        float4 Iridescence,
        out float4 Result    )
    {
        
        float theta = Sun_Theta * 2.0 * 3.14159;
        float phi = Sun_Phi * 3.14159;
        
        float3 lightDir =  float3(cos(phi)*cos(theta),sin(phi),cos(phi)*sin(theta));
        float NdotL = max(dot(lightDir,Normal),0.0);
        
        //float3 H = normalize(Normal-Incident);
        float3 R = reflect(Incident,Normal);
        float RdotL = max(0.0,dot(R,lightDir));
        float specular = pow(RdotL,Shininess);
        specular = lerp(specular,smoothstep(0.495*Sharpness,1.0-0.495*Sharpness,specular),Sharpness);
        
        float4 gi = lerp(Ground_Color,Sky_Color,float4(Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5));
        //SampleEnv(Normal,Sky_Color,Horizon_Color,Ground_Color,1);
        
        Result = ((Sun_Intensity*NdotL + Indirect_Sample * Indirect_Diffuse + Translucence)*(1.0 + SSS * Subsurface)) * Albedo * (1.0-Fresnel_Reflect) + (Sun_Intensity*specular*Specular + Fresnel_Reflect * Reflection*Reflection_Sample) + Fresnel_Reflect * Rim_Light + Iridescence;
        
    }
    //BLOCK_END Fragment_Main

    //BLOCK_BEGIN Bulge 84

    void Bulge_B84(
        bool Enabled,
        float3 Normal,
        float3 Tangent,
        float Bulge_Height,
        float4 UV,
        float Bulge_Radius,
        float3 ButtonN,
        out float3 New_Normal    )
    {
        float2 xy = clamp(UV.xy*2.0,float2(-1,-1),float2(1,1));
        
        float3 B = (cross(Normal,Tangent));
        
        //float3 dirX = Normal * cosa.x + Tangent * sina.x;
        //New_Normal = Normal; // * cosa.y + B * sina.y;
        //New_Normal = normalize(Normal + (New_Normal-Normal)*(1-saturate(xy.x))*(1-saturate(xy.y)));
        
        //float r = saturate(length(xy))*Bulge_Height;
        float k = -saturate(1-length(xy)/Bulge_Radius)*Bulge_Height;
        k = sin(k*3.14159*0.5);
        k *= smoothstep(0.9998,0.9999,abs(dot(ButtonN,Normal)));
        New_Normal = Normal * sqrt(1-k*k)+(xy.x*Tangent + xy.y*B)*k;
        New_Normal = Enabled ? New_Normal : Normal;
    }
    //BLOCK_END Bulge

    //BLOCK_BEGIN SSS 82

    void SSS_B82(
        float3 ButtonN,
        float3 Normal,
        float3 Incident,
        out float Result    )
    {
        float NdotI = abs(dot(Normal,Incident));
        float BdotI = abs(dot(ButtonN,Incident));
        Result = (abs(NdotI-BdotI)); //*abs(ButtonN.y); //*sqrt(1.0-NdotI);
        //Result = abs(NdotI-BdotI)*exp(-1.0/max(NdotI,0.01));
        
        
        
    }
    //BLOCK_END SSS

    //BLOCK_BEGIN FingerOcclusion 72

    void FingerOcclusion_B72(
        float Width,
        float DistToCenter,
        float Fuzz,
        float Min_Fuzz,
        float3 Position,
        float3 Forward,
        float3 Nearest,
        float Fade_Out,
        out float NotInShadow    )
    {
        float d = dot((Nearest-Position),Forward);
        float sh = smoothstep(Width*0.5,Width*0.5+Fuzz*max(d,0)+Min_Fuzz,DistToCenter);
        NotInShadow = 1-(1-sh)*smoothstep(-Fade_Out,0,d);
        
    }
    //BLOCK_END FingerOcclusion

    //BLOCK_BEGIN FingerOcclusion 73

    void FingerOcclusion_B73(
        float Width,
        float DistToCenter,
        float Fuzz,
        float Min_Fuzz,
        float3 Position,
        float3 Forward,
        float3 Nearest,
        float Fade_Out,
        out float NotInShadow    )
    {
        float d = dot((Nearest-Position),Forward);
        float sh = smoothstep(Width*0.5,Width*0.5+Fuzz*max(d,0)+Min_Fuzz,DistToCenter);
        NotInShadow = 1-(1-sh)*smoothstep(-Fade_Out,0,d);
        
    }
    //BLOCK_END FingerOcclusion

    //BLOCK_BEGIN Scale_Color 96

    void Scale_Color_B96(
        float4 Color,
        float Scalar,
        out float4 Result    )
    {
        Result = Scalar * Color;
    }
    //BLOCK_END Scale_Color

    //BLOCK_BEGIN From_HSV 78

    void From_HSV_B78(
        float Hue,
        float Saturation,
        float Value,
        float Alpha,
        out float4 Color    )
    {
        
        // from http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
        
        float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
        
        float3 p = abs(frac(float3(Hue,Hue,Hue) + K.xyz) * 6.0 - K.www);
        
        Color.rgb = Value * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), Saturation);
        Color.a = Alpha;
        
    }
    //BLOCK_END From_HSV

    //BLOCK_BEGIN Fast_Fresnel 130

    void Fast_Fresnel_B130(
        float Front_Reflect,
        float Edge_Reflect,
        float Power,
        float3 Normal,
        float3 Incident,
        out float Transmit,
        out float Reflect    )
    {
        
        float d = max(-dot(Incident,Normal),0);
        Reflect = Front_Reflect+(Edge_Reflect-Front_Reflect)*pow(1-d,Power);
        Transmit=1-Reflect;
        
    }
    //BLOCK_END Fast_Fresnel

    //BLOCK_BEGIN Mapped_Environment 56

    void Mapped_Environment_B56(
        samplerCUBE Reflected_Environment,
        samplerCUBE Indirect_Environment,
        float3 Dir,
        out float4 Reflected_Color,
        out float4 Indirect_Diffuse    )
    {
        // main code goes here
        Reflected_Color = texCUBE(Reflected_Environment,Dir);
        Indirect_Diffuse = texCUBE(Indirect_Environment,Dir);
        
    }
    //BLOCK_END Mapped_Environment

    //BLOCK_BEGIN Sky_Environment 55

    float4 SampleEnv_Bid55(float3 D, float4 S, float4 H, float4 G, float exponent)
    {
        float k = pow(abs(D.y),exponent);
        float4 C;
        if (D.y>0.0) {
            C=lerp(H,S,float4(k,k,k,k));
        } else {
            C=lerp(H,G,float4(k,k,k,k));    
        }
        return C;
    }
    
    void Sky_Environment_B55(
        half3 Normal,
        float3 Reflected,
        half4 Sky_Color,
        half4 Horizon_Color,
        half4 Ground_Color,
        half Horizon_Power,
        out half4 Reflected_Color,
        out half4 Indirect_Color    )
    {
        // main code goes here
        Reflected_Color = SampleEnv_Bid55(Reflected,Sky_Color,Horizon_Color,Ground_Color,Horizon_Power);
        Indirect_Color = lerp(Ground_Color,Sky_Color,float4(Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5,Normal.y*0.5+0.5));
        
    }
    //BLOCK_END Sky_Environment

    //BLOCK_BEGIN Min_Segment_Distance 70

    void Min_Segment_Distance_B70(
        float3 P0,
        float3 P1,
        float3 Q0,
        float3 Q1,
        out float3 NearP,
        out float3 NearQ,
        out float Distance    )
    {
        float3 u = P1 - P0;
        float3 v = Q1 - Q0;
        float3 w = P0 - Q0;
        
        float a = dot(u,u);
        float b = dot(u,v);
        float c = dot(v,v);
        float d = dot(u,w);
        float e = dot(v,w);
        
        float D = a*c-b*b;
        float sD = D;
        float tD = D;
        float sc, sN, tc, tN;
        
        if (D<0.00001) {
            sN = 0.0;
            sD = 1.0;
            tN = e;
            tD = c;
        } else {
            sN = (b*e - c*d);
            tN = (a*e - b*d);
            if (sN < 0.0) {
                sN = 0.0;
                tN = e;
                tD = c;
            } else if (sN > sD) {
                sN = sD;
                tN = e + b;
                tD = c;
            }
        }
        
        if (tN < 0.0) {
            tN = 0.0;
            if (-d < 0.0) {
                sN = 0.0;
            } else if (-d > a) {
                sN = sD;
            } else {
                sN = -d;
                sD = a;
            }
        } else if (tN > tD) {
            tN = tD;
            if ((-d + b) < 0.0) {
                sN = 0.0;
            } else if ((-d + b) > a) {
                sN = sD;
            } else {
                sN = (-d + b);
                sD = a;
            }
        }
        
        sc = abs(sN)<0.000001 ? 0.0 : sN / sD;
        tc = abs(tN)<0.000001 ? 0.0 : tN / tD;
        
        NearP = P0 + sc * u;
        NearQ = Q0 + tc * v;
        
        Distance = distance(NearP,NearQ);
        
    }
    //BLOCK_END Min_Segment_Distance

    //BLOCK_BEGIN To_XYZ 79

    void To_XYZ_B79(
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

    //BLOCK_BEGIN Finger_Positions 69

    void Finger_Positions_B69(
        float3 Left_Index_Pos,
        float3 Right_Index_Pos,
        float3 Left_Index_Middle_Pos,
        float3 Right_Index_Middle_Pos,
        out float3 Left_Index,
        out float3 Right_Index,
        out float3 Left_Index_Middle,
        out float3 Right_Index_Middle    )
    {
        Left_Index =  (Use_Global_Left_Index ? Global_Left_Index_Tip_Position.xyz :  Left_Index_Pos);
        Right_Index =  (Use_Global_Right_Index ? Global_Right_Index_Tip_Position.xyz :  Right_Index_Pos);
        
        Left_Index_Middle =  (Use_Global_Left_Index ? Global_Left_Index_Middle_Position.xyz :  Left_Index_Middle_Pos);
        Right_Index_Middle =  (Use_Global_Right_Index ? Global_Right_Index_Middle_Position.xyz :  Right_Index_Middle_Pos);
        
    }
    //BLOCK_END Finger_Positions

    //BLOCK_BEGIN VaryHSV 113

    void VaryHSV_B113(
        float3 HSV_In,
        float Hue_Shift,
        float Saturation_Shift,
        float Value_Shift,
        out float3 HSV_Out    )
    {
        HSV_Out = float3(frac(HSV_In.x+Hue_Shift),saturate(HSV_In.y+Saturation_Shift),saturate(HSV_In.z+Value_Shift));
    }
    //BLOCK_END VaryHSV

    //BLOCK_BEGIN Remap_Range 120

    void Remap_Range_B120(
        float In_Min,
        float In_Max,
        float Out_Min,
        float Out_Max,
        float In,
        out float Out    )
    {
        Out = lerp(Out_Min,Out_Max,clamp((In-In_Min)/(In_Max-In_Min),0,1));
        
    }
    //BLOCK_END Remap_Range

    //BLOCK_BEGIN To_HSV 80

    void To_HSV_B80(
        float4 Color,
        out float Hue,
        out float Saturation,
        out float Value,
        out float Alpha,
        out float3 HSV    )
    {
        
        // from http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
        
        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 p = Color.g < Color.b ? float4(Color.bg, K.wz) : float4(Color.gb, K.xy);
        float4 q = Color.r < p.x ? float4(p.xyw, Color.r) : float4(Color.r, p.yzx);
        
        float d = q.x - min(q.w, q.y);
        float e = 1.0e-10;
        
        Hue = abs(q.z + (q.w - q.y) / (6.0 * d + e));
        Saturation = d / (q.x + e);
        Value = q.x;
        Alpha = Color.a;
        HSV = float3(Hue,Saturation,Value);
    }
    //BLOCK_END To_HSV

    //BLOCK_BEGIN Code 115

    void Code_B115(
        float X,
        out float Result    )
    {
        Result = (acos(X)/3.14159-0.5)*2;
    }
    //BLOCK_END Code

    //BLOCK_BEGIN Rim_Light 128

    void Rim_Light_B128(
        float3 Front,
        float3 Normal,
        float3 Incident,
        float Rim_Intensity,
        float Rim_Power,
        sampler2D Texture,
        out float4 Result    )
    {
        float3 R = reflect(Incident,Normal);
        float RdotF = dot(R,Front);
        float RdotL = sqrt(1.0-RdotF*RdotF);
        float2 UV = float2(R.y*0.5+0.5,0.5);
        float4 Color = tex2D(Texture,UV);
        
        Result = Color;
        
    }
    //BLOCK_END Rim_Light


    //fixed4 frag(VertexOutput fragInput, fixed facing : VFACE) : SV_Target
    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half4 Blob_Color_Q35;
        #if defined(_BLOB_ENABLE_)
          Blob_Fragment_B35(_Blob_Texture_,fragInput.extra2,fragInput.extra3,Blob_Color_Q35);
        #else
          Blob_Color_Q35 = half4(0,0,0,0);
        #endif

        // Incident3
        float3 Incident_Q44 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        // Normalize3
        float3 Normalized_Q43 = normalize(fragInput.normalWorld.xyz);

        // Normalize3
        float3 Normalized_Q76 = normalize(fragInput.tangent.xyz);

        // Color_Texture
        float4 Color_Q88;
        #if defined(_DECAL_ENABLE_)
          Color_Q88 = tex2D(_Decal_,fragInput.uv);
        #else
          Color_Q88 = float4(0,0,0,0);
        #endif

        // To_XYZW
        float X_Q95;
        float Y_Q95;
        float Z_Q95;
        float W_Q95;
        X_Q95=fragInput.extra1.x;
        Y_Q95=fragInput.extra1.y;
        Z_Q95=fragInput.extra1.z;
        W_Q95=fragInput.extra1.w;

        // FastsRGBtoLinear
        float4 Linear_Q48;
        Linear_Q48.rgb = saturate(_Sky_Color_.rgb*_Sky_Color_.rgb);
        Linear_Q48.a=_Sky_Color_.a;
        
        // FastsRGBtoLinear
        float4 Linear_Q49;
        Linear_Q49.rgb = saturate(_Horizon_Color_.rgb*_Horizon_Color_.rgb);
        Linear_Q49.a=_Horizon_Color_.a;
        
        // FastsRGBtoLinear
        float4 Linear_Q50;
        Linear_Q50.rgb = saturate(_Ground_Color_.rgb*_Ground_Color_.rgb);
        Linear_Q50.a=_Ground_Color_.a;
        
        float3 Left_Index_Q69;
        float3 Right_Index_Q69;
        float3 Left_Index_Middle_Q69;
        float3 Right_Index_Middle_Q69;
        Finger_Positions_B69(_Left_Index_Pos_,_Right_Index_Pos_,_Left_Index_Middle_Pos_,_Right_Index_Middle_Pos_,Left_Index_Q69,Right_Index_Q69,Left_Index_Middle_Q69,Right_Index_Middle_Q69);

        // FastsRGBtoLinear
        float4 Linear_Q51;
        Linear_Q51.rgb = saturate(_Albedo_.rgb*_Albedo_.rgb);
        Linear_Q51.a=_Albedo_.a;
        
        // Normalize3
        float3 Normalized_Q112 = normalize(fragInput.binormal.xyz);

        // Incident3
        float3 Incident_Q75 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        float3 New_Normal_Q84;
        Bulge_B84(_Bulge_Enabled_,Normalized_Q43,Normalized_Q76,_Bulge_Height_,fragInput.vertexColor,_Bulge_Radius_,fragInput.binormal.xyz,New_Normal_Q84);

        float Result_Q82;
        SSS_B82(fragInput.binormal.xyz,New_Normal_Q84,Incident_Q44,Result_Q82);

        float4 Result_Q96;
        Scale_Color_B96(Color_Q88,X_Q95,Result_Q96);

        float Transmit_Q130;
        float Reflect_Q130;
        Fast_Fresnel_B130(_Front_Reflect_,_Edge_Reflect_,_Power_,New_Normal_Q84,Incident_Q44,Transmit_Q130,Reflect_Q130);

        // Multiply
        float Product_Q134 = Y_Q95 * Y_Q95;

        float3 NearP_Q70;
        float3 NearQ_Q70;
        float Distance_Q70;
        Min_Segment_Distance_B70(Left_Index_Q69,Left_Index_Middle_Q69,fragInput.posWorld,_WorldSpaceCameraPos,NearP_Q70,NearQ_Q70,Distance_Q70);

        float3 NearP_Q68;
        float3 NearQ_Q68;
        float Distance_Q68;
        Min_Segment_Distance_B70(Right_Index_Q69,Right_Index_Middle_Q69,fragInput.posWorld,_WorldSpaceCameraPos,NearP_Q68,NearQ_Q68,Distance_Q68);

        // Reflect
        float3 Reflected_Q52 = reflect(Incident_Q44, New_Normal_Q84);

        // Multiply_Colors
        float4 Product_Q108 = Linear_Q51 * float4(1,1,1,1);

        float4 Result_Q128;
        Rim_Light_B128(Normalized_Q112,Normalized_Q43,Incident_Q75,_Rim_Intensity_,_Rim_Power_,_Rim_Texture_,Result_Q128);

        // DotProduct3
        float Dot_Q77 = dot(Incident_Q75, Normalized_Q76);

        // Max
        float MaxAB_Q132=max(Reflect_Q130,Product_Q134);

        float NotInShadow_Q72;
        #if defined(_OCCLUSION_ENABLED_)
          FingerOcclusion_B72(_Width_,Distance_Q70,_Fuzz_,_Min_Fuzz_,fragInput.posWorld,fragInput.binormal.xyz,NearP_Q70,_Clip_Fade_,NotInShadow_Q72);
        #else
          NotInShadow_Q72 = 1;
        #endif

        float NotInShadow_Q73;
        #if defined(_OCCLUSION_ENABLED_)
          FingerOcclusion_B73(_Width_,Distance_Q68,_Fuzz_,_Min_Fuzz_,fragInput.posWorld,fragInput.binormal.xyz,NearP_Q68,_Clip_Fade_,NotInShadow_Q73);
        #else
          NotInShadow_Q73 = 1;
        #endif

        float4 Reflected_Color_Q56;
        float4 Indirect_Diffuse_Q56;
        #if defined(_ENV_ENABLE_)
          Mapped_Environment_B56(_Reflection_Map_,_Indirect_Environment_,Reflected_Q52,Reflected_Color_Q56,Indirect_Diffuse_Q56);
        #else
          Reflected_Color_Q56 = float4(0,0,0,1);
          Indirect_Diffuse_Q56 = float4(0,0,0,1);
        #endif

        half4 Reflected_Color_Q55;
        half4 Indirect_Color_Q55;
        #if defined(_SKY_ENABLED_)
          Sky_Environment_B55(New_Normal_Q84,Reflected_Q52,Linear_Q48,Linear_Q49,Linear_Q50,_Horizon_Power_,Reflected_Color_Q55,Indirect_Color_Q55);
        #else
          Reflected_Color_Q55 = half4(0,0,0,1);
          Indirect_Color_Q55 = half4(0,0,0,1);
        #endif

        float Hue_Q80;
        float Saturation_Q80;
        float Value_Q80;
        float Alpha_Q80;
        float3 HSV_Q80;
        To_HSV_B80(Product_Q108,Hue_Q80,Saturation_Q80,Value_Q80,Alpha_Q80,HSV_Q80);

        float Hue_Q136;
        float Saturation_Q136;
        float Value_Q136;
        float Alpha_Q136;
        float3 HSV_Q136;
        To_HSV_B80(Result_Q128,Hue_Q136,Saturation_Q136,Value_Q136,Alpha_Q136,HSV_Q136);

        float Result_Q115;
        Code_B115(Dot_Q77,Result_Q115);

        // Abs
        float AbsA_Q81 = abs(Result_Q115);

        // Min
        float MinAB_Q63=min(NotInShadow_Q72,NotInShadow_Q73);

        // Add_Colors
        half4 Sum_Q53 = Reflected_Color_Q56 + Reflected_Color_Q55;

        // Add_Colors
        half4 Sum_Q54 = Indirect_Diffuse_Q56 + Indirect_Color_Q55;

        float3 HSV_Out_Q135;
        VaryHSV_B113(HSV_Q136,_Rim_Hue_Shift_,_Rim_Saturation_Shift_,_Rim_Value_Shift_,HSV_Out_Q135);

        float Out_Q120;
        Remap_Range_B120(-1,1,0,1,Result_Q115,Out_Q120);

        // Modify
        float Product_Q111;
        Product_Q111 = AbsA_Q81 * _Hue_Shift_;
        //Product_Q111 = sign(AbsA_Q81)*sqrt(abs(AbsA_Q81))*_Hue_Shift_;

        float X_Q137;
        float Y_Q137;
        float Z_Q137;
        To_XYZ_B79(HSV_Out_Q135,X_Q137,Y_Q137,Z_Q137);

        // From_XY
        float2 Vec2_Q117 = float2(Out_Q120,0.5);

        float3 HSV_Out_Q113;
        VaryHSV_B113(HSV_Q80,Product_Q111,_Saturation_Shift_,_Value_Shift_,HSV_Out_Q113);

        float4 Color_Q138;
        From_HSV_B78(X_Q137,Y_Q137,Z_Q137,0,Color_Q138);

        // Color_Texture
        float4 Color_Q116;
        #if defined(_IRIDESCENCE_ENABLED_)
          Color_Q116 = tex2D(_Iridescence_Texture_,Vec2_Q117);
        #else
          Color_Q116 = float4(0,0,0,0);
        #endif

        float X_Q79;
        float Y_Q79;
        float Z_Q79;
        To_XYZ_B79(HSV_Out_Q113,X_Q79,Y_Q79,Z_Q79);

        // Scale_Color
        float4 Result_Q141 = _Rim_Intensity_ * Color_Q138;

        // Scale_Color
        float4 Result_Q118 = _Iridescence_Intensity_ * Color_Q116;

        float4 Color_Q78;
        From_HSV_B78(X_Q79,Y_Q79,Z_Q79,0,Color_Q78);

        // Blend_Over
        float4 Result_Q89 = Result_Q96 + (1.0 - Result_Q96.a) * Color_Q78;

        float4 Result_Q129;
        Fragment_Main_B129(_Sun_Intensity_,_Sun_Theta_,_Sun_Phi_,New_Normal_Q84,Result_Q89,MaxAB_Q132,_Shininess_,Incident_Q44,_Horizon_Color_,_Sky_Color_,_Ground_Color_,_Indirect_Diffuse_,_Specular_,_Horizon_Power_,_Reflection_,Sum_Q53,Sum_Q54,_Sharpness_,Result_Q82,_Subsurface_,float4(0,0,0,0),Result_Q141,Result_Q118,Result_Q129);

        float4 Result_Q64;
        Scale_RGB_B64(Result_Q129,MinAB_Q63,Result_Q64);

        float4 sRGB_Q47;
        FastLinearTosRGB_B47(Result_Q64,sRGB_Q47);

        // Blend_Over
        half4 Result_Q36 = Blob_Color_Q35 + (1.0 - Blob_Color_Q35.a) * sRGB_Q47;

        // Set_Alpha
        float4 Result_Q45 = Result_Q36; Result_Q45.a = 1;

        float4 Out_Color = Result_Q45;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Rounded_Thick_Iridescent_Map" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.03
        _Line_Width_("Line Width", Range(0,1)) = 0.01
        [Toggle] _Absolute_Sizes_("Absolute Sizes", Float) = 0
        _Filter_Width_("Filter Width", Range(0,4)) = 1
        _Base_Color_("Base Color", Color) = (0,0,0,1)
        _Line_Color_("Line Color", Color) = (0.53,0.53,0.53,1)
     
    [Header(Radii Multipliers)]
        _Radius_Top_Left_("Radius Top Left", Range(0,1)) = 1
        _Radius_Top_Right_("Radius Top Right", Range(0,1)) = 1.0
        _Radius_Bottom_Left_("Radius Bottom Left", Range(0,1)) = 1.0
        _Radius_Bottom_Right_("Radius Bottom Right", Range(0,1)) = 1.0
     
    [Header(Blob)]
        [Toggle(_BLOB_ENABLE_)] _Blob_Enable_("Blob Enable", Float) = 1
        _Blob_Position_("Blob Position", Vector) = (0, 0, 0.1, 1)
        _Blob_Intensity_("Blob Intensity", Range(0,3)) = 0.98
        _Blob_Near_Size_("Blob Near Size", Range(0,1)) = 0.22
        _Blob_Far_Size_("Blob Far Size", Range(0,1)) = 0.04
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
        _Blob_Near_Size_2_("Blob Near Size 2", Range(0,1)) = 0.26
        _Blob_Pulse_2_("Blob Pulse 2", Range(0,1)) = 0
        _Blob_Fade_2_("Blob Fade 2", Range(0,1)) = 1
     
    [Header(Line Highlight)]
        _Rate_("Rate", Range(0,1)) = 0.135
        _Highlight_Color_("Highlight Color", Color) = (0.98,0.98,0.98,1)
        _Highlight_Width_("Highlight Width", Range(0,2)) = 0.25
        _Highlight_Transform_("Highlight Transform", Vector) = (1, 1, 0, 0)
        _Highlight_("Highlight", Range(0,1)) = 1
     
    [Header(Iridescence)]
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0
        _Iridescence_Edge_Intensity_("Iridescence Edge Intensity", Range(0,1)) = 1
        _Angle_("Angle", Range(-90,90)) = -45
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle(_SMOOTH_EDGES_)] _Smooth_Edges_("Smooth Edges", Float) = 1
     
    [Header(ChooseAngle)]
        [Toggle] _Reflected_("Reflected", Float) = 1
     
    [Header(Multiply)]
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
     
    [Header(Color Texture)]
        [Toggle(_IRIDESCENT_MAP_ENABLE_)] _Iridescent_Map_Enable_("Iridescent Map Enable", Float) = 1
        [NoScaleOffset] _Iridescent_Map_("Iridescent Map", 2D) = "" {}
     

    [Header(Global)]
        [Toggle] Use_Global_Left_Index("Use Global Left Index", Float) = 0
        [Toggle] Use_Global_Right_Index("Use Global Right Index", Float) = 0
}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    Tags {"DisableBatching" = "True"}

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _SMOOTH_EDGES_
    #pragma multi_compile _ _BLOB_ENABLE_2_
    #pragma multi_compile _ _BLOB_ENABLE_
    #pragma multi_compile _ _IRIDESCENT_MAP_ENABLE_

    #include "UnityCG.cginc"

    float _Radius_;
    float _Line_Width_;
    bool _Absolute_Sizes_;
    float _Filter_Width_;
    float4 _Base_Color_;
    float4 _Line_Color_;
    float _Radius_Top_Left_;
    float _Radius_Top_Right_;
    float _Radius_Bottom_Left_;
    float _Radius_Bottom_Right_;
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
    sampler2D _Blob_Texture_;
    //bool _Blob_Enable_2_;
    float3 _Blob_Position_2_;
    float _Blob_Near_Size_2_;
    float _Blob_Pulse_2_;
    float _Blob_Fade_2_;
    float _Rate_;
    float4 _Highlight_Color_;
    float _Highlight_Width_;
    float4 _Highlight_Transform_;
    half _Highlight_;
    float _Iridescence_Intensity_;
    float _Iridescence_Edge_Intensity_;
    float _Angle_;
    float _Fade_Out_;
    //bool _Smooth_Edges_;
    bool _Reflected_;
    float _Frequency_;
    float _Vertical_Offset_;
    //bool _Iridescent_Map_Enable_;
    sampler2D _Iridescent_Map_;

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
        float4 tangent : TANGENT;
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


    //BLOCK_BEGIN Object_To_World_Pos 94

    void Object_To_World_Pos_B94(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN PickDir 132

    void PickDir_B132(
        float Degrees,
        float3 DirX,
        float3 DirY,
        out float3 Dir    )
    {
        // main code goes here
        float a = Degrees*3.14159/180.0;
        Dir = cos(a)*DirX+sin(a)*DirY;
        
    }
    //BLOCK_END PickDir

    //BLOCK_BEGIN Round_Rect_Vertex 127

    void Round_Rect_Vertex_B127(
        float2 UV,
        float Radius,
        float Margin,
        float Anisotropy,
        float Gradient1,
        float Gradient2,
        out float2 Rect_UV,
        out float4 Rect_Parms,
        out float2 Scale_XY,
        out float2 Line_UV    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Line_UV = (UV - float2(0.5,0.5));
        Rect_UV = Line_UV * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius)-float2(Margin,Margin);
        Rect_Parms.z = Gradient1; //Radius - Line_Width;
        Rect_Parms.w = Gradient2;
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Line_Vertex 122

    void Line_Vertex_B122(
        float2 Scale_XY,
        float2 UV,
        float Time,
        float Rate,
        float4 Highlight_Transform,
        out float3 Line_Vertex    )
    {
        float angle2 = (Rate*Time) * 2.0 * 3.1416;
        float sinAngle2 = sin(angle2);
        float cosAngle2 = cos(angle2);
        
        float2 xformUV = UV * Highlight_Transform.xy + Highlight_Transform.zw;
        Line_Vertex.x = 0.0;
        Line_Vertex.y = cosAngle2*xformUV.x-sinAngle2*xformUV.y;
        Line_Vertex.z = 0.0; //sinAngle2*xformUV.x+cosAngle2*xformUV.y;
        
    }
    //BLOCK_END Line_Vertex

    //BLOCK_BEGIN Blob_Vertex 109

    void Blob_Vertex_B109(
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

    //BLOCK_BEGIN Blob_Vertex 110

    void Blob_Vertex_B110(
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

    //BLOCK_BEGIN Move_Verts 116

    void Move_Verts_B116(
        float Anisotropy,
        float3 P,
        float Radius,
        out float3 New_P,
        out float2 New_UV,
        out float Radial_Gradient,
        out float3 Radial_Dir    )
    {
        float2 UV = P.xy * 2 + 0.5;
        float2 center = saturate(UV);
        float2 delta = UV - center;
                
        float2 r2 = 2.0 * float2(Radius / Anisotropy, Radius);
                
        New_UV = center + r2 * (UV - 2 * center + 0.5);
        New_P = float3(New_UV - 0.5, P.z);
                
        Radial_Gradient = 1.0 - length(delta) * 2.0;
        Radial_Dir = float3(delta * r2, 0.0);
        
    }
    //BLOCK_END Move_Verts

    //BLOCK_BEGIN Object_To_World_Dir 119

    void Object_To_World_Dir_B119(
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

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 157

    void RelativeOrAbsoluteDetail_B157(
        float Nominal_Radius,
        float Nominal_LineWidth,
        bool Absolute_Measurements,
        float Height,
        out float Radius,
        out float Line_Width    )
    {
        float scale = Absolute_Measurements ? 1.0/Height : 1.0;
        Radius = Nominal_Radius * scale;
        Line_Width = Nominal_LineWidth * scale;
        
        
    }
    //BLOCK_END RelativeOrAbsoluteDetail

    //BLOCK_BEGIN Edge_AA_Vertex 117

    void Edge_AA_Vertex_B117(
        float3 Position_World,
        float3 Position_Object,
        float3 Normal_Object,
        float3 Eye,
        float Radial_Gradient,
        float3 Radial_Dir,
        float3 Tangent,
        out float Gradient1,
        out float Gradient2    )
    {
        // main code goes here
        float3 I = (Eye-Position_World);
        float3 T = UnityObjectToWorldNormal(Tangent);
        float g = (dot(T,I)<0.0) ? 0.0 : 1.0;
        if (Normal_Object.z==0) { // edge
            //float3 T = Position_Object.z>0.0 ? float3(0.0,0.0,1.0) : float3(0.0,0.0,-1.0);
            Gradient1 = Position_Object.z>0.0 ? g : 1.0;
            Gradient2 = Position_Object.z>0.0 ? 1.0 : g;
        } else {
        //    float3 R = UnityObjectToWorldNormal(Tangent); //Radial_Dir);
        //    float k = (dot(R,I)>0.0 ? 1.0 : 0.0);
        //    float kk = dot(normalize(R),normalize(I));
        //    float k =  kk>0.0 ? kk*Edge_Bend : 0.0;
            Gradient1 = g + (1.0-g)*(Radial_Gradient);
            Gradient2 = 1.0;
        }
        
    }
    //BLOCK_END Edge_AA_Vertex

    //BLOCK_BEGIN Pick_Radius 149

    void Pick_Radius_B149(
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


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Object_To_World_Dir (#115)
        float3 Nrm_World_Q115;
        Nrm_World_Q115 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        // Object_To_World_Dir (#118)
        float3 Tangent_World_Q118;
        float3 Tangent_World_N_Q118;
        float Tangent_Length_Q118;
        Tangent_World_Q118 = (mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
        Tangent_Length_Q118 = length(Tangent_World_Q118);
        Tangent_World_N_Q118 = Tangent_World_Q118 / Tangent_Length_Q118;

        float3 Binormal_World_Q119;
        float3 Binormal_World_N_Q119;
        float Binormal_Length_Q119;
        Object_To_World_Dir_B119(float3(0,1,0),Binormal_World_Q119,Binormal_World_N_Q119,Binormal_Length_Q119);

        // Divide (#120)
        float Anisotropy_Q120 = Tangent_Length_Q118 / Binormal_Length_Q119;

        float Result_Q149;
        Pick_Radius_B149(_Radius_,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q149);

        float3 Dir_Q132;
        PickDir_B132(_Angle_,Tangent_World_N_Q118,Binormal_World_N_Q119,Dir_Q132);

        float Radius_Q157;
        float Line_Width_Q157;
        RelativeOrAbsoluteDetail_B157(Result_Q149,_Line_Width_,_Absolute_Sizes_,Binormal_Length_Q119,Radius_Q157,Line_Width_Q157);

        // From_RGBA (#150)
        float4 Out_Color_Q150 = float4(Radius_Q157, Line_Width_Q157, 0, 1);

        float3 New_P_Q116;
        float2 New_UV_Q116;
        float Radial_Gradient_Q116;
        float3 Radial_Dir_Q116;
        Move_Verts_B116(Anisotropy_Q120,vertInput.vertex.xyz,Radius_Q157,New_P_Q116,New_UV_Q116,Radial_Gradient_Q116,Radial_Dir_Q116);

        float3 Pos_World_Q94;
        Object_To_World_Pos_B94(New_P_Q116,Pos_World_Q94);

        float4 Blob_Info_Q109;
        #if defined(_BLOB_ENABLE_)
          Blob_Vertex_B109(Pos_World_Q94,Nrm_World_Q115,Tangent_World_N_Q118,Binormal_World_N_Q119,_Blob_Position_,_Blob_Intensity_,_Blob_Near_Size_,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,_Blob_Fade_Length_,_Blob_Pulse_,_Blob_Fade_,Blob_Info_Q109);
        #else
          Blob_Info_Q109 = float4(0,0,0,0);
        #endif

        float4 Blob_Info_Q110;
        #if defined(_BLOB_ENABLE_2_)
          Blob_Vertex_B110(Pos_World_Q94,Nrm_World_Q115,Tangent_World_N_Q118,Binormal_World_N_Q119,_Blob_Position_2_,_Blob_Intensity_,_Blob_Near_Size_2_,_Blob_Far_Size_,_Blob_Near_Distance_,_Blob_Far_Distance_,_Blob_Fade_Length_,_Blob_Pulse_2_,_Blob_Fade_2_,Blob_Info_Q110);
        #else
          Blob_Info_Q110 = float4(0,0,0,0);
        #endif

        float Gradient1_Q117;
        float Gradient2_Q117;
        #if defined(_SMOOTH_EDGES_)
          Edge_AA_Vertex_B117(Pos_World_Q94,vertInput.vertex.xyz,vertInput.normal,_WorldSpaceCameraPos,Radial_Gradient_Q116,Radial_Dir_Q116,vertInput.tangent,Gradient1_Q117,Gradient2_Q117);
        #else
          Gradient1_Q117 = 1;
          Gradient2_Q117 = 1;
        #endif

        float2 Rect_UV_Q127;
        float4 Rect_Parms_Q127;
        float2 Scale_XY_Q127;
        float2 Line_UV_Q127;
        Round_Rect_Vertex_B127(New_UV_Q116,Radius_Q157,0,Anisotropy_Q120,Gradient1_Q117,Gradient2_Q117,Rect_UV_Q127,Rect_Parms_Q127,Scale_XY_Q127,Line_UV_Q127);

        float3 Line_Vertex_Q122;
        Line_Vertex_B122(Scale_XY_Q127,Line_UV_Q127,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q122);

        float3 Position = Pos_World_Q94;
        float3 Normal = Dir_Q132;
        float2 UV = Rect_UV_Q127;
        float3 Tangent = Line_Vertex_Q122;
        float3 Binormal = Nrm_World_Q115;
        float4 Color = Out_Color_Q150;
        float4 Extra1 = Rect_Parms_Q127;
        float4 Extra2 = Blob_Info_Q109;
        float4 Extra3 = Blob_Info_Q110;


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

    //BLOCK_BEGIN Round_Rect_Fragment 121

    void Round_Rect_Fragment_B121(
        half Radius,
        half Line_Width,
        half4 Line_Color,
        half Filter_Width,
        float2 UV,
        half Line_Visibility,
        half4 Rect_Parms,
        half4 Fill_Color,
        out half4 Color    )
    {
        float d = length(max(abs(UV)-Rect_Parms.xy,0.0));
        float dx = max(fwidth(d)*Filter_Width,0.00001);
        
        //float Inside_Rect = saturate((Radius-d)/dx);
        float g = min(Rect_Parms.z,Rect_Parms.w);
        float dgrad = max(fwidth(g)*Filter_Width,0.00001);
        float Inside_Rect = saturate(g/dgrad);
        
        //this is arguably more correct...
        //float inner = saturate((d+dx*0.5-max(Rect_Parms.z,d-dx*0.5))/dx);
        float inner = saturate((d+dx*0.5-max(Radius-Line_Width,d-dx*0.5))/dx);
        
        Color = saturate(lerp(Fill_Color, Line_Color,float4( inner, inner, inner, inner)))*Inside_Rect;
        //but this saves 3 ops
        //float inner = saturate((Rect_Parms.z-d)/dx);
        //Color = lerp(Line_Color*Line_Visibility, Fill_Color,float4( inner, inner, inner, inner))*Inside_Rect;
    }
    //BLOCK_END Round_Rect_Fragment

    //BLOCK_BEGIN Blob_Fragment 125

    void Blob_Fragment_B125(
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

    //BLOCK_BEGIN Line_Fragment 152

    void Line_Fragment_B152(
        float4 Base_Color,
        float4 Highlight_Color,
        half Highlight_Width,
        half3 Line_Vertex,
        half Highlight,
        out float4 Line_Color    )
    {
        half k2 = 1.0-saturate(abs(Line_Vertex.y/Highlight_Width));
        Line_Color = lerp(Base_Color,Highlight_Color,float4(Highlight*k2,Highlight*k2,Highlight*k2,Highlight*k2));
    }
    //BLOCK_END Line_Fragment

    //BLOCK_BEGIN Scale_RGB 163

    void Scale_RGB_B163(
        float4 Color,
        float Scalar,
        out float4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Conditional_Float 137

    void Conditional_Float_B137(
        bool Which,
        float If_True,
        float If_False,
        out float Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Float


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // To_RGBA (#151)
        float R_Q151;
        float G_Q151;
        float B_Q151;
        float A_Q151;
        R_Q151=fragInput.vertexColor.r; G_Q151=fragInput.vertexColor.g; B_Q151=fragInput.vertexColor.b; A_Q151=fragInput.vertexColor.a;

        half4 Blob_Color_Q125;
        #if defined(_BLOB_ENABLE_)
          Blob_Fragment_B125(_Blob_Texture_,fragInput.extra2,fragInput.extra3,Blob_Color_Q125);
        #else
          Blob_Color_Q125 = half4(0,0,0,0);
        #endif

        float4 Line_Color_Q152;
        Line_Fragment_B152(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.tangent.xyz,_Highlight_,Line_Color_Q152);

        // To_XY (#154)
        float X_Q154;
        float Y_Q154;
        X_Q154 = fragInput.uv.x;
        Y_Q154 = fragInput.uv.y;

        // Incident3 (#128)
        float3 Incident_Q128 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        // Reflect (#136)
        float3 Reflected_Q136 = reflect(Incident_Q128, fragInput.binormal.xyz);

        // Multiply (#155)
        float Product_Q155 = Y_Q154 * _Vertical_Offset_;

        // DotProduct3 (#138)
        float Dot_Q138 = dot(Incident_Q128, Reflected_Q136);

        // DotProduct3 (#129)
        float Dot_Q129 = dot(fragInput.normalWorld.xyz, Incident_Q128);

        float Result_Q137;
        Conditional_Float_B137(_Reflected_,Dot_Q138,Dot_Q129,Result_Q137);

        // Multiply (#139)
        float Product_Q139 = Result_Q137 * _Frequency_;

        // Add (#141)
        float Sum_Q141 = Product_Q139 + 1;

        // Multiply (#142)
        float Product_Q142 = Sum_Q141 * 0.5;

        // Add (#140)
        float Sum_Q140 = Product_Q155 + Product_Q142;

        // Fract (#143)
        float FractF_Q143=frac(Sum_Q140);

        // From_XY (#145)
        float2 Vec2_Q145 = float2(FractF_Q143,0.5);

        // Color_Texture (#144)
        float4 Color_Q144;
        #if defined(_IRIDESCENT_MAP_ENABLE_)
          Color_Q144 = tex2D(_Iridescent_Map_,Vec2_Q145);
        #else
          Color_Q144 = float4(0,0,0,0);
        #endif

        float4 Result_Q163;
        Scale_RGB_B163(Color_Q144,_Iridescence_Edge_Intensity_,Result_Q163);

        float4 Result_Q130;
        Scale_RGB_B163(Color_Q144,_Iridescence_Intensity_,Result_Q130);

        // Add_Colors (#165)
        float4 Base_And_Iridescent_Q165;
        Base_And_Iridescent_Q165 = Line_Color_Q152 + float4(Result_Q163.rgb,0.0);
        
        // Add_Colors (#131)
        float4 Base_And_Iridescent_Q131;
        Base_And_Iridescent_Q131 = _Base_Color_ + float4(Result_Q130.rgb,0.0);
        
        // Set_Alpha (#135)
        float4 Result_Q135 = Base_And_Iridescent_Q165; Result_Q135.a = 1;

        // Blend_Over (#126)
        half4 Result_Q126 = Blob_Color_Q125 + (1.0 - Blob_Color_Q125.a) * Base_And_Iridescent_Q131;

        half4 Color_Q121;
        Round_Rect_Fragment_B121(R_Q151,G_Q151,Result_Q135,_Filter_Width_,fragInput.uv,1,fragInput.extra1,Result_Q126,Color_Q121);

        // Scale_Color (#111)
        float4 Result_Q111 = _Fade_Out_ * Color_Q121;

        float4 Out_Color = Result_Q111;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

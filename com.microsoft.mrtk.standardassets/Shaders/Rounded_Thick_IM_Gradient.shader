
Shader "Rounded_Thick_IM_Gradient" {

Properties {

    [Header(Round Rect)]
        _Radius_("Radius", Range(0,0.5)) = 0.03
        _Line_Width_("Line Width", Range(0,1)) = 0.01
        [Toggle] _Absolute_Sizes_("Absolute Sizes", Float) = 0
        _Filter_Width_("Filter Width", Range(0,4)) = 1
        _Base_Color_("Base Color", Color) = (0,0,0,1)
        _Line_Color_("Line Color", Color) = (0,0,0,1)
     
    [Header(Radii Multipliers)]
        _Radius_Top_Left_("Radius Top Left", Range(0,1)) = 1
        _Radius_Top_Right_("Radius Top Right", Range(0,1)) = 1.0
        _Radius_Bottom_Left_("Radius Bottom Left", Range(0,1)) = 1.0
        _Radius_Bottom_Right_("Radius Bottom Right", Range(0,1)) = 1.0
     
    [Header(Line Highlight)]
        _Rate_("Rate", Range(0,1)) = 0.135
        _Highlight_Color_("Highlight Color", Color) = (0.98,0.98,0.98,1)
        _Highlight_Width_("Highlight Width", Range(0,2)) = 0.25
        _Highlight_Transform_("Highlight Transform", Vector) = (1, 1, 0, 0)
        _Highlight_("Highlight", Range(0,1)) = 1
     
    [Header(Iridescence)]
        [Toggle(_IRIDESCENCE_ENABLE_)] _Iridescence_Enable_("Iridescence Enable", Float) = 1
        _Iridescence_Intensity_("Iridescence Intensity", Range(0,1)) = 0
        _Iridescence_Edge_Intensity_("Iridescence Edge Intensity", Range(0,1)) = 0.56
        _Iridescence_Tint_("Iridescence Tint", Color) = (1,1,1,1)
        [NoScaleOffset] _Iridescent_Map_("Iridescent Map", 2D) = "" {}
        _Angle_("Angle", Range(-90,90)) = -45
        [Toggle] _Reflected_("Reflected", Float) = 1
        _Frequency_("Frequency", Range(0,10)) = 1
        _Vertical_Offset_("Vertical Offset", Range(0,2)) = 0
     
    [Header(Gradient)]
        _Gradient_Color_("Gradient Color", Color) = (0.905882,0.905882,0.905882,1)
        _Top_Left_("Top Left", Color) = (1,0.690196,0.976471,1)
        _Top_Right_("Top Right", Color) = (0.670588,0.819608,1,1)
        _Bottom_Left_("Bottom Left", Color) = (1,0.945098,0.733333,1)
        _Bottom_Right_("Bottom Right", Color) = (1,1,1,1)
        [Toggle(_EDGE_ONLY_)] _Edge_Only_("Edge Only", Float) = 0
        _Edge_Width_("Edge Width", Range(0,1)) = 0.5
        _Edge_Power_("Edge Power", Range(0,10)) = 2.0
        _Line_Gradient_Blend_("Line Gradient Blend", Range(0,1)) = 0.36
     
    [Header(Fade)]
        _Fade_Out_("Fade Out", Range(0,1)) = 1
     
    [Header(Antialiasing)]
        [Toggle(_SMOOTH_EDGES_)] _Smooth_Edges_("Smooth Edges", Float) = 1
     

    [Header(Stencil)]
        _StencilReference("Stencil Reference", Range(0, 255)) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComparison("Stencil Comparison", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOperation("Stencil Operation", Int) = 0

}

SubShader {
    Tags{ "RenderType" = "Opaque" }
    Blend Off
    Tags {"DisableBatching" = "True"}
    Stencil
    {
        Ref[_StencilReference]
        Comp[_StencilComparison]
        Pass[_StencilOperation]
    }

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _SMOOTH_EDGES_
    #pragma multi_compile _ _IRIDESCENCE_ENABLE_
    #pragma multi_compile _ _EDGE_ONLY_

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
    float _Rate_;
    half4 _Highlight_Color_;
    half _Highlight_Width_;
    float4 _Highlight_Transform_;
    half _Highlight_;
    //bool _Iridescence_Enable_;
    float _Iridescence_Intensity_;
    float _Iridescence_Edge_Intensity_;
    half4 _Iridescence_Tint_;
    sampler2D _Iridescent_Map_;
    float _Angle_;
    bool _Reflected_;
    float _Frequency_;
    float _Vertical_Offset_;
    half4 _Gradient_Color_;
    half4 _Top_Left_;
    half4 _Top_Right_;
    half4 _Bottom_Left_;
    half4 _Bottom_Right_;
    //bool _Edge_Only_;
    half _Edge_Width_;
    half _Edge_Power_;
    half _Line_Gradient_Blend_;
    half _Fade_Out_;
    //bool _Smooth_Edges_;




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
        float4 extra1 : TEXCOORD4;
        float4 extra2 : TEXCOORD3;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN Object_To_World_Pos 117

    void Object_To_World_Pos_B117(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN Round_Rect_Vertex 171

    void Round_Rect_Vertex_B171(
        float2 UV,
        float Radius,
        float Margin,
        float Anisotropy,
        float Gradient1,
        float Gradient2,
        float3 Normal,
        float4 Color_Scale_Translate,
        out float2 Rect_UV,
        out float4 Rect_Parms,
        out float2 Scale_XY,
        out float2 Line_UV,
        out float2 Color_UV_Info    )
    {
        Scale_XY = float2(Anisotropy,1.0);
        Line_UV = (UV - float2(0.5,0.5));
        Rect_UV = Line_UV * Scale_XY;
        Rect_Parms.xy = Scale_XY*0.5-float2(Radius,Radius)-float2(Margin,Margin);
        Rect_Parms.z = Gradient1; //Radius - Line_Width;
        Rect_Parms.w = Gradient2;
        
        Color_UV_Info = (Line_UV + float2(0.5,0.5)) * Color_Scale_Translate.xy + Color_Scale_Translate.zw;
        
        
    }
    //BLOCK_END Round_Rect_Vertex

    //BLOCK_BEGIN Line_Vertex 139

    void Line_Vertex_B139(
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

    //BLOCK_BEGIN PickDir 144

    void PickDir_B144(
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

    //BLOCK_BEGIN Move_Verts 133

    void Move_Verts_B133(
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

    //BLOCK_BEGIN Pick_Radius 157

    void Pick_Radius_B157(
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

    //BLOCK_BEGIN Edge_AA_Vertex 134

    void Edge_AA_Vertex_B134(
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

    //BLOCK_BEGIN Object_To_World_Dir 136

    void Object_To_World_Dir_B136(
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

    //BLOCK_BEGIN RelativeOrAbsoluteDetail 180

    void RelativeOrAbsoluteDetail_B180(
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


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Object_To_World_Dir (#132)
        float3 Nrm_World_Q132;
        Nrm_World_Q132 = normalize((mul((float3x3)unity_ObjectToWorld, vertInput.normal)));
        
        // Object_To_World_Dir (#135)
        float3 Tangent_World_Q135;
        float3 Tangent_World_N_Q135;
        float Tangent_Length_Q135;
        Tangent_World_Q135 = (mul((float3x3)unity_ObjectToWorld, float3(1,0,0)));
        Tangent_Length_Q135 = length(Tangent_World_Q135);
        Tangent_World_N_Q135 = Tangent_World_Q135 / Tangent_Length_Q135;

        float3 Binormal_World_Q136;
        float3 Binormal_World_N_Q136;
        float Binormal_Length_Q136;
        Object_To_World_Dir_B136(float3(0,1,0),Binormal_World_Q136,Binormal_World_N_Q136,Binormal_Length_Q136);

        float Radius_Q180;
        float Line_Width_Q180;
        RelativeOrAbsoluteDetail_B180(_Radius_,_Line_Width_,_Absolute_Sizes_,Binormal_Length_Q136,Radius_Q180,Line_Width_Q180);

        float3 Dir_Q144;
        PickDir_B144(_Angle_,Tangent_World_N_Q135,Binormal_World_N_Q136,Dir_Q144);

        float Result_Q157;
        Pick_Radius_B157(Radius_Q180,_Radius_Top_Left_,_Radius_Top_Right_,_Radius_Bottom_Left_,_Radius_Bottom_Right_,vertInput.vertex.xyz,Result_Q157);

        // Divide (#137)
        float Anisotropy_Q137 = Tangent_Length_Q135 / Binormal_Length_Q136;

        // From_RGBA (#158)
        float4 Out_Color_Q158 = float4(Result_Q157, Line_Width_Q180, 0, 1);

        float3 New_P_Q133;
        float2 New_UV_Q133;
        float Radial_Gradient_Q133;
        float3 Radial_Dir_Q133;
        Move_Verts_B133(Anisotropy_Q137,vertInput.vertex.xyz,Result_Q157,New_P_Q133,New_UV_Q133,Radial_Gradient_Q133,Radial_Dir_Q133);

        float3 Pos_World_Q117;
        Object_To_World_Pos_B117(New_P_Q133,Pos_World_Q117);

        float Gradient1_Q134;
        float Gradient2_Q134;
        #if defined(_SMOOTH_EDGES_)
          Edge_AA_Vertex_B134(Pos_World_Q117,vertInput.vertex.xyz,vertInput.normal,_WorldSpaceCameraPos,Radial_Gradient_Q133,Radial_Dir_Q133,vertInput.tangent,Gradient1_Q134,Gradient2_Q134);
        #else
          Gradient1_Q134 = 1;
          Gradient2_Q134 = 1;
        #endif

        float2 Rect_UV_Q171;
        float4 Rect_Parms_Q171;
        float2 Scale_XY_Q171;
        float2 Line_UV_Q171;
        float2 Color_UV_Info_Q171;
        Round_Rect_Vertex_B171(New_UV_Q133,Result_Q157,0,Anisotropy_Q137,Gradient1_Q134,Gradient2_Q134,vertInput.normal,float4(1,1,0,0),Rect_UV_Q171,Rect_Parms_Q171,Scale_XY_Q171,Line_UV_Q171,Color_UV_Info_Q171);

        float3 Line_Vertex_Q139;
        Line_Vertex_B139(Scale_XY_Q171,Line_UV_Q171,_Time.y,_Rate_,_Highlight_Transform_,Line_Vertex_Q139);

        // To_XY (#207)
        float X_Q207;
        float Y_Q207;
        X_Q207 = Color_UV_Info_Q171.x;
        Y_Q207 = Color_UV_Info_Q171.y;

        // From_XYZW (#206)
        float4 Vec4_Q206 = float4(X_Q207, Y_Q207, Result_Q157, Line_Width_Q180);

        float3 Position = Pos_World_Q117;
        float3 Normal = Nrm_World_Q132;
        float2 UV = Rect_UV_Q171;
        float3 Tangent = Line_Vertex_Q139;
        float3 Binormal = Dir_Q144;
        float4 Color = Out_Color_Q158;
        float4 Extra1 = Rect_Parms_Q171;
        float4 Extra2 = Vec4_Q206;
        float4 Extra3 = float4(0,0,0,0);


        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;
        o.binormal.xyz = Binormal; o.binormal.w=1.0;
        o.extra1=Extra1;
        o.extra2=Extra2;

        return o;
    }

    //BLOCK_BEGIN FastLinearTosRGB 145

    void FastLinearTosRGB_B145(
        half4 Linear,
        out half4 sRGB    )
    {
        sRGB.rgb = sqrt(saturate(Linear.rgb));
        sRGB.a = Linear.a;
        
    }
    //BLOCK_END FastLinearTosRGB

    //BLOCK_BEGIN Round_Rect_Fragment 138

    void Round_Rect_Fragment_B138(
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

    //BLOCK_BEGIN Iridescence 183

    void Iridescence_B183(
        half3 Position,
        half3 Normal,
        half2 UV,
        half3 Axis,
        half3 Eye,
        half4 Tint,
        sampler2D Texture,
        bool Reflected,
        half Frequency,
        half Vertical_Offset,
        out half4 Color    )
    {
        
        half3 i = normalize(Position-Eye);
        half3 r = reflect(i,Normal);
        half idota = dot(i,Axis);
        half idotr = dot(i,r);
        
        half x = Reflected ? idotr : idota;
        
        half2 xy;
        xy.x = frac((x*Frequency+1.0)*0.5 + UV.y*Vertical_Offset);
        xy.y = 0.5;
        
        Color = tex2D(Texture,xy);
        Color.rgb*=Tint.rgb;
    }
    //BLOCK_END Iridescence

    //BLOCK_BEGIN Scale_RGB 166

    void Scale_RGB_B166(
        half4 Color,
        half Scalar,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Scale_RGB 142

    void Scale_RGB_B142(
        half Scalar,
        half4 Color,
        out half4 Result    )
    {
        Result = float4(Scalar,Scalar,Scalar,1) * Color;
    }
    //BLOCK_END Scale_RGB

    //BLOCK_BEGIN Line_Fragment 160

    void Line_Fragment_B160(
        half4 Base_Color,
        half4 Highlight_Color,
        half Highlight_Width,
        half3 Line_Vertex,
        half Highlight,
        out half4 Line_Color    )
    {
        half k2 = 1.0-saturate(abs(Line_Vertex.y/Highlight_Width));
        Line_Color = lerp(Base_Color,Highlight_Color,float4(Highlight*k2,Highlight*k2,Highlight*k2,Highlight*k2));
    }
    //BLOCK_END Line_Fragment

    //BLOCK_BEGIN Edge 175

    void Edge_B175(
        float4 RectParms,
        half Radius,
        half Line_Width,
        float2 UV,
        half Edge_Width,
        half Edge_Power,
        out half Result    )
    {
        half d = length(max(abs(UV)-RectParms.xy,0.0));
        half edge = 1.0-saturate((1.0-d/(Radius-Line_Width))/Edge_Width);
        Result = pow(edge, Edge_Power);
        
    }
    //BLOCK_END Edge

    //BLOCK_BEGIN Gradient 173

    void Gradient_B173(
        half4 Gradient_Color,
        half4 Top_Left,
        half4 Top_Right,
        half4 Bottom_Left,
        half4 Bottom_Right,
        half2 UV,
        out half4 Result    )
    {
        half3 top = Top_Left.rgb + (Top_Right.rgb - Top_Left.rgb)*UV.x;
        half3 bottom = Bottom_Left.rgb + (Bottom_Right.rgb - Bottom_Left.rgb)*UV.x;
        
        Result.rgb = Gradient_Color.rgb * (bottom + (top - bottom)*UV.y);
        Result.a = 1.0;
        
        
    }
    //BLOCK_END Gradient


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // To_XYZW (#164)
        float X_Q164;
        float Y_Q164;
        float Z_Q164;
        float W_Q164;
        X_Q164=fragInput.extra2.x;
        Y_Q164=fragInput.extra2.y;
        Z_Q164=fragInput.extra2.z;
        W_Q164=fragInput.extra2.w;

        half4 Color_Q183;
        #if defined(_IRIDESCENCE_ENABLE_)
          Iridescence_B183(fragInput.posWorld,fragInput.normalWorld.xyz,fragInput.uv,fragInput.binormal.xyz,_WorldSpaceCameraPos,_Iridescence_Tint_,_Iridescent_Map_,_Reflected_,_Frequency_,_Vertical_Offset_,Color_Q183);
        #else
          Color_Q183 = half4(0,0,0,0);
        #endif

        half4 Result_Q142;
        Scale_RGB_B142(_Iridescence_Intensity_,Color_Q183,Result_Q142);

        half4 Line_Color_Q160;
        Line_Fragment_B160(_Line_Color_,_Highlight_Color_,_Highlight_Width_,fragInput.tangent.xyz,_Highlight_,Line_Color_Q160);

        half Result_Q175;
        #if defined(_EDGE_ONLY_)
          Edge_B175(fragInput.extra1,Z_Q164,W_Q164,fragInput.uv,_Edge_Width_,_Edge_Power_,Result_Q175);
        #else
          Result_Q175 = 1;
        #endif

        // From_XY (#165)
        float2 Vec2_Q165 = float2(X_Q164,Y_Q164);

        half4 Result_Q173;
        Gradient_B173(_Gradient_Color_,_Top_Left_,_Top_Right_,_Bottom_Left_,_Bottom_Right_,Vec2_Q165,Result_Q173);

        // FastsRGBtoLinear (#169)
        half4 Linear_Q169;
        Linear_Q169.rgb = saturate(Result_Q173.rgb*Result_Q173.rgb);
        Linear_Q169.a=Result_Q173.a;
        
        half4 Result_Q166;
        Scale_RGB_B166(Linear_Q169,Result_Q175,Result_Q166);

        // Add_Colors (#172)
        half4 Sum_Q172 = Result_Q166 + Result_Q142;

        // Mix_Colors (#178)
        half4 Color_At_T_Q178 = lerp(Line_Color_Q160, Result_Q166,float4( _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_, _Line_Gradient_Blend_));

        // Add_Colors (#143)
        half4 Base_And_Iridescent_Q143;
        Base_And_Iridescent_Q143 = _Base_Color_ + float4(Sum_Q172.rgb,0.0);
        
        // Add_Scaled_Color (#176)
        half4 Sum_Q176 = Color_At_T_Q178 + _Iridescence_Edge_Intensity_ * Color_Q183;

        // Set_Alpha (#146)
        half4 Result_Q146 = Sum_Q176; Result_Q146.a = 1;

        half4 Color_Q138;
        Round_Rect_Fragment_B138(Z_Q164,W_Q164,Result_Q146,_Filter_Width_,fragInput.uv,1,fragInput.extra1,Base_And_Iridescent_Q143,Color_Q138);

        // Scale_Color (#128)
        half4 Result_Q128 = _Fade_Out_ * Color_Q138;

        half4 sRGB_Q145;
        FastLinearTosRGB_B145(Result_Q128,sRGB_Q145);

        float4 Out_Color = sRGB_Q145;
        float Clip_Threshold = 0.001;
        bool To_sRGB = false;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

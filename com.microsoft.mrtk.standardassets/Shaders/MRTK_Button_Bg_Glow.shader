// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


Shader "Shell_Button_Bg_Glow" {

Properties {

    [Header(Rounded Rectangle)]
        _Bevel_Radius_("Bevel Radius", Range(0,1)) = 0.05
        _Line_Width_("Line Width", Range(0,1)) = 0.03
     
    [Header(Animation)]
        _Tuning_Motion_("Tuning Motion", Range(0,1)) = 0.0
        [PerRendererData] _Motion_("Motion",Range(0,1)) = 1
        _Max_Intensity_("Max Intensity", Range(0,1)) = 0.5
        _Intensity_Fade_In_Exponent_("Intensity Fade In Exponent", Range(0,5)) = 2
        _Outer_Fuzz_Start_("Outer Fuzz Start", Range(0,1)) = 0.002
        _Outer_Fuzz_End_("Outer Fuzz End", Range(0,1)) = 0.001
     
    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
        _Inner_Color_("Inner Color", Color) = (1,1,1,1)
        _Blend_Exponent_("Blend Exponent", Range(0,9)) = 1
     
    [Header(Inner Transition)]
        _Falloff_("Falloff", Range(0,5)) = 1.0
        _Bias_("Bias", Range(0,1)) = 0.5
     

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

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0

    #include "UnityCG.cginc"

    half _Bevel_Radius_;
    half _Line_Width_;
    half _Tuning_Motion_;
    half _Motion_;
    half _Max_Intensity_;
    half _Intensity_Fade_In_Exponent_;
    half _Outer_Fuzz_Start_;
    half _Outer_Fuzz_End_;
    half4 _Color_;
    half4 _Inner_Color_;
    half _Blend_Exponent_;
    half _Falloff_;
    half _Bias_;




    struct VertexInput {
        float4 vertex : POSITION;
        half3 normal : NORMAL;
        float2 uv0 : TEXCOORD0;
        float4 tangent : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct VertexOutput {
        float4 pos : SV_POSITION;
        half4 normalWorld : TEXCOORD5;
        float2 uv : TEXCOORD0;
      UNITY_VERTEX_OUTPUT_STEREO
    };


    //BLOCK_BEGIN Object_To_World_Pos 131

    void Object_To_World_Pos_B131(
        float3 Pos_Object,
        out float3 Pos_World    )
    {
        Pos_World=(mul(unity_ObjectToWorld, float4(Pos_Object, 1)));
        
    }
    //BLOCK_END Object_To_World_Pos

    //BLOCK_BEGIN ScaleUVs 136

    void ScaleUVs_B136(
        float2 UV,
        float SizeX,
        float SizeY,
        out float2 XY    )
    {
        XY = (UV - float2(0.5,0.5))*float2(SizeX,SizeY);
        
    }
    //BLOCK_END ScaleUVs

    //BLOCK_BEGIN Conditional_Vec3 139

    void Conditional_Vec3_B139(
        bool Which,
        float3 If_False,
        float3 If_True,
        out float3 Result    )
    {
        Result = Which ? If_True : If_False;
        
    }
    //BLOCK_END Conditional_Vec3

    //BLOCK_BEGIN Object_To_World_Dir 138

    void Object_To_World_Dir_B138(
        float3 Dir_Object,
        out float3 Dir_World    )
    {
        Dir_World=(mul((float3x3)unity_ObjectToWorld, Dir_Object));
        
    }
    //BLOCK_END Object_To_World_Dir


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        float3 Dir_World_Q138;
        Object_To_World_Dir_B138(vertInput.tangent,Dir_World_Q138);

        float3 Dir_World_Q137;
        Object_To_World_Dir_B138((normalize(cross(vertInput.normal,vertInput.tangent))),Dir_World_Q137);

        // Max (#149)
        half MaxAB_Q149=max(_Tuning_Motion_,_Motion_);

        // Length3 (#132)
        float Length_Q132 = length(Dir_World_Q138);

        // Length3 (#133)
        float Length_Q133 = length(Dir_World_Q137);

        // Greater_Than (#148)
        bool Greater_Than_Q148;
        Greater_Than_Q148 = MaxAB_Q149 > 0;
        
        // From_XYZ (#142)
        float3 Vec3_Q142 = float3(Length_Q132,Length_Q133,0);

        float2 XY_Q136;
        ScaleUVs_B136(vertInput.uv0,Length_Q132,Length_Q133,XY_Q136);

        float3 Result_Q139;
        Conditional_Vec3_B139(Greater_Than_Q148,float3(0,0,0),vertInput.vertex.xyz,Result_Q139);

        float3 Pos_World_Q131;
        Object_To_World_Pos_B131(Result_Q139,Pos_World_Q131);

        float3 Position = Pos_World_Q131;
        float3 Normal = Vec3_Q142;
        float2 UV = XY_Q136;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);


        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;

        return o;
    }

    //BLOCK_BEGIN Power 157

    void Power_B157(
        half Base,
        half Exponent,
        out half Power    )
    {
        Power = pow(Base, Exponent);
        
    }
    //BLOCK_END Power

    //BLOCK_BEGIN Ease_Transition 152

    float Bias_Bid152(float b, float v) {
      return pow(v,log(clamp(b,0.001,0.999))/log(0.5));
    }
    
    void Ease_Transition_B152(
        half Falloff,
        half Bias,
        half F,
        out half Result    )
    {
        Result = pow(Bias_Bid152(Bias,F),Falloff);
        
    }
    //BLOCK_END Ease_Transition

    //BLOCK_BEGIN Fuzzy_Round_Rect 156

    void Fuzzy_Round_Rect_B156(
        half Size_X,
        half Size_Y,
        half Radius_X,
        half Radius_Y,
        half Line_Width,
        half2 UV,
        half Outer_Fuzz,
        half Max_Outer_Fuzz,
        out half Rect_Distance,
        out half Inner_Distance    )
    {
        half2 halfSize = half2(Size_X,Size_Y)*0.5;
        half halfLineWidth = Line_Width*0.5;
        half2 r = max(min(float2(Radius_X,Radius_Y),halfSize),half2(0.001,0.001));
        half radius = min(r.x,r.y)-Max_Outer_Fuzz;
        half2 v = abs(UV);
        
        half2 nearestp = min(v,halfSize-r);
        half d = distance(nearestp,v);
        Inner_Distance = saturate(1.0-(radius-d)/Line_Width);
        Rect_Distance = saturate(1.0-(d-radius)/Outer_Fuzz)*Inner_Distance;
    }
    //BLOCK_END Fuzzy_Round_Rect

    //BLOCK_BEGIN To_XYZ 147

    void To_XYZ_B147(
        half3 Vec3,
        out half X,
        out half Y,
        out half Z    )
    {
        X=Vec3.x;
        Y=Vec3.y;
        Z=Vec3.z;
        
    }
    //BLOCK_END To_XYZ


    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        half X_Q147;
        half Y_Q147;
        half Z_Q147;
        To_XYZ_B147(fragInput.normalWorld.xyz,X_Q147,Y_Q147,Z_Q147);

        // Max (#149)
        half MaxAB_Q149=max(_Tuning_Motion_,_Motion_);

        // Sqrt (#159)
        half Sqrt_F_Q159 = sqrt(MaxAB_Q149);

        half Power_Q145;
        Power_B157(MaxAB_Q149,_Intensity_Fade_In_Exponent_,Power_Q145);

        // Lerp (#155)
        half Value_At_T_Q155=lerp(_Outer_Fuzz_Start_,_Outer_Fuzz_End_,Sqrt_F_Q159);

        // Multiply (#144)
        half Product_Q144 = _Max_Intensity_ * Power_Q145;

        half Rect_Distance_Q156;
        half Inner_Distance_Q156;
        Fuzzy_Round_Rect_B156(X_Q147,Y_Q147,_Bevel_Radius_,_Bevel_Radius_,_Line_Width_,fragInput.uv,Value_At_T_Q155,_Outer_Fuzz_Start_,Rect_Distance_Q156,Inner_Distance_Q156);

        half Power_Q157;
        Power_B157(Inner_Distance_Q156,_Blend_Exponent_,Power_Q157);

        half Result_Q152;
        Ease_Transition_B152(_Falloff_,_Bias_,Rect_Distance_Q156,Result_Q152);

        // Mix_Colors (#153)
        half4 Color_At_T_Q153 = lerp(_Inner_Color_, _Color_,float4( Power_Q157, Power_Q157, Power_Q157, Power_Q157));

        // Multiply (#143)
        half Product_Q143 = Result_Q152 * Product_Q144;

        // Scale_Color (#146)
        half4 Result_Q146 = Product_Q143 * Color_At_T_Q153;

        float4 Out_Color = Result_Q146;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

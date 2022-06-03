
Shader "RoundedQuadGlow" {

Properties {

    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
     
    [Header(Shape)]
        _Radius_("Radius", Range(0,0.5)) = 0.25
        _Filter_Width_("Filter Width", Range(0,4)) = 2
     
    [Header(Glow)]
        _Glow_Fraction_("Glow Fraction", Range(0.01,0.99)) = 0.5
        _Glow_Max_("Glow Max", Range(0,1)) = 0.5
        _Glow_Falloff_("Glow Falloff", Range(0,5)) = 2
     


}

SubShader {
    Tags{ "RenderType" = "AlphaTest" "Queue" = "AlphaTest"}
    Blend One OneMinusSrcAlpha
    Tags {"DisableBatching" = "True"}

    LOD 100


    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _CLIPPING_PLANE _CLIPPING_SPHERE _CLIPPING_BOX

    #if defined(_CLIPPING_PLANE) || defined(_CLIPPING_SPHERE) || defined(_CLIPPING_BOX)
        #define _CLIPPING_PRIMITIVE
    #else
        #undef _CLIPPING_PRIMITIVE
    #endif

    #include "UnityCG.cginc"

    half4 _Color_;
    half _Radius_;
    half _Filter_Width_;
    half _Glow_Fraction_;
    half _Glow_Max_;
    half _Glow_Falloff_;


#if defined (_CLIPPING_PRIMITIVE)
    fixed _ClipBoxSide;
    float4 _ClipBoxSize;
    float4x4 _ClipBoxInverseTransform;

    inline float PointVsBox(float3 worldPosition, float3 boxSize, float4x4 boxInverseTransform)
    {
         float3 distance = abs(mul(boxInverseTransform, float4(worldPosition, 1.0))) - boxSize;
         return length(max(distance, 0.0)) + min(max(distance.x, max(distance.y, distance.z)), 0.0);
    }
#endif


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
        float2 uv : TEXCOORD0;
        float3 posWorld : TEXCOORD7;
        float4 tangent : TANGENT;
      UNITY_VERTEX_OUTPUT_STEREO
    };



    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Object_To_World_Pos (#132)
        float3 Pos_World_Q132;
        Pos_World_Q132=(mul(unity_ObjectToWorld, float4(vertInput.vertex.xyz, 1)));
        
        // Object_To_World_Dir (#131)
        float3 Dir_World_Q131;
        Dir_World_Q131=(mul((float3x3)unity_ObjectToWorld, vertInput.tangent));
        
        // Object_To_World_Dir (#137)
        float3 Dir_World_Q137;
        Dir_World_Q137=(mul((float3x3)unity_ObjectToWorld, (normalize(cross(vertInput.normal,vertInput.tangent)))));
        
        // Length3 (#138)
        float Length_Q138 = length(Dir_World_Q131);

        // Length3 (#139)
        float Length_Q139 = length(Dir_World_Q137);

        // Divide (#143)
        float Quotient_Q143 = Length_Q138 / Length_Q139;

        // TransformUVs (#141)
        float2 Result_Q141;
        Result_Q141 = float2((vertInput.uv0.x-0.5)*Length_Q138/Length_Q139,(vertInput.uv0.y-0.5));
        
        // From_XYZ (#142)
        float3 Vec3_Q142 = float3(Quotient_Q143,0,0);

        float3 Position = Pos_World_Q132;
        float3 Normal = float3(0,0,0);
        float2 UV = Result_Q141;
        float3 Tangent = Vec3_Q142;
        float3 Binormal = float3(0,0,0);
        float4 Color = vertInput.color;


        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.uv = UV;
        o.tangent.xyz = Tangent; o.tangent.w=1.0;

        return o;
    }

    //BLOCK_BEGIN Round_Rect 156

    half FilterStep_Bid156(half edge, half x, half filterWidth)
    {
       half dx = max(1.0E-5,fwidth(x)*filterWidth);
       return max((x+dx*0.5 - max(edge,x-dx*0.5))/dx,0.0);
    }
    void Round_Rect_B156(
        half2 Center_XY,
        half Size_X,
        half Size_Y,
        half Radius_X,
        half Radius_Y,
        half4 Rect_Color,
        half Filter_Width,
        half2 UV,
        half Glow_Fraction,
        half Glow_Max,
        half Glow_Falloff,
        out half4 Color    )
    {
        half2 halfSize = half2(Size_X,Size_Y)*0.5;
        half2 r = max(min(half2(Radius_X,Radius_Y),halfSize),half2(0.01,0.01));
        
        half2 v = abs(UV-Center_XY);
        
        half2 nearestp = min(v,halfSize-r);
        half2 delta = (v-nearestp)/max(half2(0.01,0.01),r);
        half Distance = length(delta);
        
        half insideRect = 1.0 - FilterStep_Bid156(1.0-Glow_Fraction,Distance,Filter_Width);
        
        half glow = saturate((1.0-Distance)/Glow_Fraction);
        glow = pow(glow, Glow_Falloff);
        Color = Rect_Color * max(insideRect, glow*Glow_Max);
    }
    //BLOCK_END Round_Rect


    half4 frag(VertexOutput fragInput) : SV_Target
    {
#if defined (_CLIPPING_BOX)
        clip(PointVsBox(fragInput.posWorld, _ClipBoxSize.xyz, _ClipBoxInverseTransform) * _ClipBoxSide);
#endif   
        half4 result;

        // To_XYZ (#154)
        half X_Q154;
        half Y_Q154;
        half Z_Q154;
        X_Q154=fragInput.tangent.xyz.x;
        Y_Q154=fragInput.tangent.xyz.y;
        Z_Q154=fragInput.tangent.xyz.z;
        
        half4 Color_Q156;
        Round_Rect_B156(half2(0.0,0.0),X_Q154,1.0,_Radius_,_Radius_,_Color_,_Filter_Width_,fragInput.uv,_Glow_Fraction_,_Glow_Max_,_Glow_Falloff_,Color_Q156);

        float4 Out_Color = Color_Q156;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

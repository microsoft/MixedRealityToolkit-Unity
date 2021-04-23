// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Teaching_Hand2" {

Properties {

    [Header(Displacement)]
        _Displacement_("Displacement", Range(0,1)) = 0.0001
     
    [Header(Color)]
        _Color_("Color", Color) = (1,1,1,1)
        _Intensity_("Intensity", Range(0,5)) = 2.5
     
    [Header(Edge)]
        _Exponent_("Exponent", Range(0,10)) = 2
        _Soften_("Soften", Range(0.001,1)) = 0.3
     
    [Header(Fade In)]
        _Fade_In_Start_("Fade In Start", Range(0,1)) = 0.25
        _Fade_In_End_("Fade In End", Range(0,1)) = 0.5
     
    [Header(Soften Hold Out)]
        [Toggle(_ENABLE_)] _Enable_("Enable", Float) = 0
        _Center_("Center", Vector) = (0.5,0.5,0,0)
        _Radius_("Radius", Range(0,1)) = 0.071
        _Blur_("Blur", Range(0,1)) = 0.061
     

}

SubShader {
    Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
    Blend One One
    ZWrite Off

    LOD 100

    Pass {
        ColorMask 0
        ZWrite On

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct v2f {
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_img v)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            v2f o;
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return 0;
        }
        ENDCG
    }

    Pass

    {

    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #pragma multi_compile_instancing
    #pragma target 4.0
    #pragma multi_compile _ _ENABLE_

    #include "UnityCG.cginc"

    float _Displacement_;
    float _Fade_In_Start_;
    float _Fade_In_End_;
    half4 _Color_;
    half _Intensity_;
    //bool _Enable_;
    float2 _Center_;
    half _Radius_;
    half _Blur_;
    half _Exponent_;
    half _Soften_;




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
      UNITY_VERTEX_OUTPUT_STEREO
    };

    // declare parm vars here


    VertexOutput vert(VertexInput vertInput)
    {
        UNITY_SETUP_INSTANCE_ID(vertInput);
        VertexOutput o;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


        // Object_To_World_Dir
        float3 Dir_World_Q47=(mul((float3x3)unity_ObjectToWorld, vertInput.normal));

        // Object_To_World_Pos
        float3 Pos_World_Q48=(mul(unity_ObjectToWorld, float4(vertInput.vertex.xyz, 1)));

        // Scale3
        float3 Result_Q23 = _Displacement_ * Dir_World_Q47;

        // Add3
        float3 Sum3_Q24 = Pos_World_Q48 + Result_Q23;

        float3 Position = Sum3_Q24;
        float3 Normal = Dir_World_Q47;
        float2 UV = vertInput.uv0;
        float3 Tangent = float3(0,0,0);
        float3 Binormal = float3(0,0,0);
        float4 Color = float4(1,1,1,1);


        o.pos = UnityObjectToClipPos(vertInput.vertex);
        o.pos = mul(UNITY_MATRIX_VP, float4(Position,1));
        o.posWorld = Position;
        o.normalWorld.xyz = Normal; o.normalWorld.w=1.0;
        o.uv = UV;

        return o;
    }


    //fixed4 frag(VertexOutput fragInput, fixed facing : VFACE) : SV_Target
    half4 frag(VertexOutput fragInput) : SV_Target
    {
        half4 result;

        // To_XY
        float X_Q34;
        float Y_Q34;
        X_Q34 = fragInput.uv.x;
        Y_Q34 = fragInput.uv.y;

        // Soften_Hold_Out
        half Result_Q46;
        #if defined(_ENABLE_)
          Result_Q46 = saturate((distance(fragInput.uv,_Center_)-_Radius_)/_Blur_);
        #else
          Result_Q46 = 1.0;
        #endif

        // Incident3
        float3 Incident_Q25 = normalize(fragInput.posWorld - _WorldSpaceCameraPos);

        // Normalize3
        float3 Normalized_Q26 = normalize(fragInput.normalWorld.xyz);

        // Ramp
        half Ramp_Q45 = clamp((Y_Q34-_Fade_In_Start_)/(_Fade_In_End_-_Fade_In_Start_),0,1);

        // DotProduct3
        half Dot_Q30 = dot(Incident_Q25, Normalized_Q26);

        // Abs
        half AbsA_Q31 = abs(Dot_Q30);

        // SoftenEdges
        half Result_Q43 = 1.0-(1.0-saturate(-Dot_Q30/_Soften_))*Result_Q46;

        // One_Minus
        half One_Minus_F_Q32 = 1.0 - AbsA_Q31;

        // Power
        half Power_Q44 = pow(One_Minus_F_Q32, _Exponent_);

        // Multiply
        half Product_Q33 = Power_Q44 * Result_Q43;

        // Multiply
        half Product_Q37 = Ramp_Q45 * Product_Q33;

        // Multiply
        half Product_Q38 = _Intensity_ * Product_Q37;

        // Scale_Color
        half4 Result_Q40 = Product_Q38 * _Color_;

        float4 Out_Color = Result_Q40;
        float Clip_Threshold = 0;

        result = Out_Color;
        return result;
    }

    ENDCG
  }
 }
}

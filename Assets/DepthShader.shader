Shader "Custom/DepthShader" 
{
    Properties
    {
        _DepthTex("Texture", 2D) = "black" {}
        _MainTex("Base (RGB)", 2D) = "white" {}
    }

        SubShader
    {
        Pass
        {
        CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            sampler2D _DepthTex;

            float4 frag(v2f_img i) : COLOR 
            {
                return Linear01Depth(SAMPLE_DEPTH_TEXTURE(_DepthTex, i.uv));
            }
        ENDCG
        }
    }
}
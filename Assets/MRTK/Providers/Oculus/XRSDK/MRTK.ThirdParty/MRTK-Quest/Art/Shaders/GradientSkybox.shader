/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Eric Provencher
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

/*
  Original:
  "Oculus/SampleFramework/Usage/HandsTrainExample/Shaders/GradientSkybox.shader"

  Changes:
  2020-01-24: Andrei Torres: Added support for Single Pass Instanced rendering mode
*/

Shader "MixedRealityToolkit/Procedural Gradient Skybox"
{
  Properties
  {
    _TopColor ("Top Color", Color) = (1, 1, 1, 0)
    _HorizonColor ("Horizon Color", Color) = (1, 1, 1, 0)
    _BottomColor ("Bottom Color", Color) = (1, 1, 1, 0)
    _TopExponent ("Top Exponent", Float) = 0.5
    _BottomExponent ("Bottom Exponent", Float) = 0.5
    _AmplFactor ("Amplification", Float) = 1.0
  }
  SubShader
  {
    Tags{"RenderType" ="Background" "Queue" = "Background"}
    ZWrite Off Cull Off 
    Fog { Mode Off }
    LOD 100

    Pass
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
    
      #include "UnityCG.cginc"

      struct vertIn
      {
        float4 vertex : POSITION;
        float3 uv : TEXCOORD0;
		    UNITY_VERTEX_INPUT_INSTANCE_ID //Single pass instanced fix
      };

      struct vertOut
      {
        float4 vertex : SV_POSITION;
        float3 uv: TEXCOORD0;
		    UNITY_VERTEX_OUTPUT_STEREO //Single pass instanced fix
      };
      
      vertOut vert (vertIn v)
      {
        vertOut o;
        UNITY_SETUP_INSTANCE_ID(v); //Single pass instanced fix
        UNITY_INITIALIZE_OUTPUT(vertOut, o); //Single pass instanced fix
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Single pass instanced fix
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
      }

      half _TopExponent;
      half _BottomExponent;
      fixed4 _TopColor;
      fixed4 _HorizonColor;
      fixed4 _BottomColor;
      half _AmplFactor;
      
      fixed4 frag (vertOut i) : SV_Target
      {
        float interpUv = normalize (i.uv).y;
        // top goes from 0->1 going down toward horizon
        float topLerp = 1.0f - pow (min (1.0f, 1.0f - interpUv), _TopExponent);
        // bottom goes from 0->1 going up toward horizon
        float bottomLerp = 1.0f - pow (min (1.0f, 1.0f + interpUv), _BottomExponent);
        // last lerp param is horizon. all must add up to 1.0
        float horizonLerp = 1.0f - topLerp - bottomLerp;
        return (_TopColor * topLerp + _HorizonColor * horizonLerp + _BottomColor * bottomLerp) *
          _AmplFactor;
      }

      ENDCG
    }
  }
}

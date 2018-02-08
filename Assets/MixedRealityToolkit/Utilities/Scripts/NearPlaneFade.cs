// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// Sets global shader variables relating to near plane fade
    /// </summary>
    [ExecuteInEditMode]
    public class NearPlaneFade : MonoBehaviour
    {
        public float FadeDistanceStart = 0.85f;
        public float FadeDistanceEnd = 0.5f;

        public bool NearPlaneFadeOn = true;

        private const string FadeKeywordOn = "_NEAR_PLANE_FADE_ON";

        private int fadeDistancePropertyID;

        private void Awake()
        {
            fadeDistancePropertyID = Shader.PropertyToID("_NearPlaneFadeDistance");
            UpdateShaderParams();
        }

        private void OnValidate()
        {
            UpdateShaderParams();
        }

        private void UpdateShaderParams()
        {
            FadeDistanceStart = Mathf.Max(FadeDistanceStart, 0);
            FadeDistanceEnd = Mathf.Max(FadeDistanceEnd, 0);
            FadeDistanceStart = Mathf.Max(FadeDistanceStart, FadeDistanceEnd);

            if (FadeDistanceStart != FadeDistanceEnd)
            {
                float rangeInverse = 1.0f / (FadeDistanceStart - FadeDistanceEnd);
                var fadeDist = new Vector4(-FadeDistanceEnd * rangeInverse, rangeInverse, 0, 0);
                Shader.SetGlobalVector(fadeDistancePropertyID, fadeDist);
            }

            if (NearPlaneFadeOn)
            {
                Shader.EnableKeyword(FadeKeywordOn);
            }
            else
            {
                Shader.DisableKeyword(FadeKeywordOn);
            }
        }
    }
}

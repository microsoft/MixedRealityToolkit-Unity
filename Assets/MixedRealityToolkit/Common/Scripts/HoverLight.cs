// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace MixedRealityToolkit.Common
{
    /// <summary>
    /// Utility component to animate and visualize a hover light that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_HoverLight" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class HoverLight : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float Radius = 0.15f;
        public Color Color = Color.white;
        [Range(0.0f, 1.0f)]
        public float Intensity = 0.3f;

        private int hoverPositionID;
        private int hoverRadiusID;
        private int hoverColorID;

        private void Awake()
        {
            Initialize();
        }

#if UNITY_EDITOR
        private void Update()
        {
            Initialize();
#else
        private void LateUpdate()
        {
#endif
            Shader.SetGlobalVector(hoverPositionID, transform.position);
            Shader.SetGlobalFloat(hoverRadiusID, Radius);
            Shader.SetGlobalVector(hoverColorID, new Vector4(Color.r,
                                                              Color.g,
                                                              Color.b,
                                                              Intensity));
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled)
            {
                return;
            }

            Gizmos.color = Color;
            Gizmos.DrawWireSphere(transform.position, Radius);
            Gizmos.DrawIcon(transform.position + Vector3.right * Radius, string.Empty, false);
            Gizmos.DrawIcon(transform.position + Vector3.left * Radius, string.Empty, false);
            Gizmos.DrawIcon(transform.position + Vector3.up * Radius, string.Empty, false);
            Gizmos.DrawIcon(transform.position + Vector3.down * Radius, string.Empty, false);
            Gizmos.DrawIcon(transform.position + Vector3.forward * Radius, string.Empty, false);
            Gizmos.DrawIcon(transform.position + Vector3.back * Radius, string.Empty, false);
        }

        private void Initialize()
        {
            hoverPositionID = Shader.PropertyToID("_HoverPosition");
            hoverRadiusID = Shader.PropertyToID("_HoverRadius");
            hoverColorID = Shader.PropertyToID("_HoverColor");
        }
    }
}

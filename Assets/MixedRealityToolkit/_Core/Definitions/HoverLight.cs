// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Utility component to animate and visualize a light that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_HoverLight" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class HoverLight : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float radius = 0.15f;

        [SerializeField]
        private Color color = new Color(0.3f, 0.3f, 0.3f, 1.0f);

        private int hoverPositionId;
        private int hoverRadiusId;
        private int hoverColorId;

        public float Radius
        {
            get
            {
                return radius;
            }

            set
            {
                radius = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            UpdateHoverLight();
        }

        private void Update()
        {
            if (Application.isPlaying || !Application.isEditor)
            {
                return;
            }

            Initialize();
            UpdateHoverLight();
        }

        private void LateUpdate()
        {
            UpdateHoverLight();
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
            hoverPositionId = Shader.PropertyToID("_HoverPosition");
            hoverRadiusId = Shader.PropertyToID("_HoverRadius");
            hoverColorId = Shader.PropertyToID("_HoverColor");
        }

        private void UpdateHoverLight()
        {
            Shader.SetGlobalVector(hoverPositionId, transform.position);
            Shader.SetGlobalFloat(hoverRadiusId, Radius);
            Shader.SetGlobalVector(hoverColorId, new Vector4(Color.r,
                                                             Color.g,
                                                             Color.b,
                                                             isActiveAndEnabled ? 1.0f : 0.0f));
        }
    }
}

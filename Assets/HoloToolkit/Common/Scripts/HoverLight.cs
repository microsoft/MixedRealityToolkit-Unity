// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace HoloToolkit.Unity
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

        private int hoverPositionID;
        private int hoverRadiusID;
        private int hoverColorID;

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

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateHoverLight();
        }
#endif

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
            hoverPositionID = Shader.PropertyToID("_HoverPosition");
            hoverRadiusID = Shader.PropertyToID("_HoverRadius");
            hoverColorID = Shader.PropertyToID("_HoverColor");
        }

        private void UpdateHoverLight()
        {
            Shader.SetGlobalVector(hoverPositionID, transform.position);
            Shader.SetGlobalFloat(hoverRadiusID, Radius);
            Shader.SetGlobalVector(hoverColorID, new Vector4(Color.r,
                                                              Color.g,
                                                              Color.b,
                                                              isActiveAndEnabled ? 1.0f : 0.0f));
        }
    }
}

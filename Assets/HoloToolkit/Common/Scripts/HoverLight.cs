// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
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

        // Three hover lights are supported at this time.
        private const int hoverLightCount = 3;
        private const int hoverLightDataSize = 2;
        private static List<HoverLight> activeHoverLights = new List<HoverLight>(hoverLightCount);
        private static Vector4[] hoverLightData = new Vector4[hoverLightCount * hoverLightDataSize];
        private static int _HoverLightDataID;
        private static int lastHoverLightUpdate = -1;

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
            AddHoverLight(this);
        }

        private void OnDisable()
        {
            RemoveHoverLight(this);
            UpdateHoverLights(true);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateHoverLights();
        }
#endif

        private void LateUpdate()
        {
            UpdateHoverLights();
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

        private static void AddHoverLight(HoverLight light)
        {
            if (activeHoverLights.Count >= hoverLightCount)
            {
                Debug.LogWarningFormat("Max hover light count ({0}) exceeded.", hoverLightCount);
            }

            activeHoverLights.Add(light);
        }

        private static void RemoveHoverLight(HoverLight light)
        {
            activeHoverLights.Remove(light);
        }

        private static void Initialize()
        {
            _HoverLightDataID = Shader.PropertyToID("_HoverLightData");
        }

        private static void UpdateHoverLights(bool forceUpdate = false)
        {
            if (lastHoverLightUpdate == -1)
            {
                Initialize();
            }

            if (!forceUpdate && (Time.frameCount == lastHoverLightUpdate))
            {
                return;
            }

            if (activeHoverLights.Count > 1)
            {
                Shader.EnableKeyword("_MULTI_HOVER_LIGHT");
            }
            else
            {
                Shader.DisableKeyword("_MULTI_HOVER_LIGHT");
            }

            for (int i = 0; i < hoverLightCount; ++i)
            {
                HoverLight light = (i >= activeHoverLights.Count) ? null : activeHoverLights[i];
                int dataIndex = i * hoverLightDataSize;

                if (light)
                {
                    hoverLightData[dataIndex] = new Vector4(light.transform.position.x,
                                                            light.transform.position.y,
                                                            light.transform.position.z,
                                                            light.Radius);
                    hoverLightData[dataIndex + 1] = new Vector4(light.Color.r,
                                                                light.Color.g,
                                                                light.Color.b,
                                                                1.0f);
                }
                else
                {
                    hoverLightData[dataIndex] = Vector4.zero;
                    hoverLightData[dataIndex + 1] = Vector4.zero;
                }
            }

            Shader.SetGlobalVectorArray(_HoverLightDataID, hoverLightData);

            lastHoverLightUpdate = Time.frameCount;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(HoverLight))]
    public class HoverLightEditor : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            HoverLight light = target as HoverLight;
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }
    }
#endif
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.UX
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

        // Two hover lights are supported at this time.
        private const int hoverLightCount = 2;
        private static List<HoverLight> activeHoverLights = new List<HoverLight>(hoverLightCount);
        private static int[] hoverLightIDs = new int[hoverLightCount];
        private static int[] hoverLightColorIDs = new int[hoverLightCount];
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
            for (int i = 0; i < hoverLightCount; ++i)
            {
                hoverLightIDs[i] = Shader.PropertyToID("_HoverLight" + i);
                hoverLightColorIDs[i] = Shader.PropertyToID("_HoverLightColor" + i);
            }
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

                if (light)
                {
                    Shader.SetGlobalVector(hoverLightIDs[i], new Vector4(light.transform.position.x,
                                                 light.transform.position.y,
                                                 light.transform.position.z,
                                                 light.Radius));
                    Shader.SetGlobalVector(hoverLightColorIDs[i], new Vector4(light.Color.r,
                                                                          light.Color.g,
                                                                          light.Color.b,
                                                                          1.0f));
                }
                else
                {
                    Shader.SetGlobalVector(hoverLightColorIDs[i], Vector4.zero);
                }
            }

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

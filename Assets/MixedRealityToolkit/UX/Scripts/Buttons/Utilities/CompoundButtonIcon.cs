// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Buttons.Profiles;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Utilities
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonIcon : ProfileButtonBase<ButtonIconProfile>
    {
        [Header("Icon Settings")]
        [SerializeField]
        [DropDownComponent]
        private MeshRenderer targetIconRenderer = null;

        [Tooltip("Turns off the icon entirely")]
        public bool DisableIcon = false;

        [Tooltip("Disregard the icon in the profile and use Icon Override instead")]
        public bool OverrideIcon = false;

        [SerializeField]
        [HideInMRTKInspector]
        private string iconName;

        [SerializeField]
        [ShowIfBoolValue("OverrideIcon")]
        [Tooltip("Icon to use for override")]
        private Texture2D iconOverride = null;

        [SerializeField]
        [Tooltip("Alpha value for the text mesh component")]
        private float alpha = 1f;

        private Material instantiatedMaterial;
        private Mesh instantiatedMesh;
        private bool updatingAlpha = false;
        private float alphaTarget = 1f;

        private const float AlphaThreshold = 0.01f;

        /// <summary>
        /// Property to use in IconMaterial for alpha control
        /// Useful for animating icon transparency
        /// </summary>
        public float Alpha
        {
            get
            {
                return alphaTarget;
            }
            set
            {
                if (alphaTarget != value)
                {
                    alphaTarget = value;
                    if (Application.isPlaying)
                    {
                        if (Mathf.Abs(alpha - alphaTarget) < AlphaThreshold)
                        {
                            return;
                        }

                        if (updatingAlpha)
                        {
                            return;
                        }

                        if (gameObject.activeSelf && gameObject.activeInHierarchy)
                        {
                            // Update over time
                            updatingAlpha = true;
                            StartCoroutine(UpdateAlpha());
                        }
                        else
                        {
                            // If we're not active, just set the alpha immediately
                            alpha = alphaTarget;
                            RefreshAlpha();
                        }
                    }
                    else
                    {
                        alphaTarget = value;
                        alpha = alphaTarget;
                        RefreshAlpha();
                    }
                }
            }
        }

        public MeshFilter IconMeshFilter
        {
            get
            {
                return targetIconRenderer != null ? targetIconRenderer.GetComponent<MeshFilter>() : null;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called by CompoundButtonSaveInterceptor
        /// Prevents saving a scene with instanced materials / meshes
        /// </summary>
        public void OnWillSaveScene()
        {
            ClearInstancedAssets();

            SetIconName(iconName);

        }
#endif

        public string IconName
        {
            get
            {
                return iconName;
            }
            set
            {
                SetIconName(value);
            }
        }

        private void SetIconName(string newName)
        {
            // Avoid exploding if possible
            if (Profile == null)
            {
                return;
            }

            // Set the name regardless
            iconName = newName;

            if (targetIconRenderer == null)
            {
                return;
            }

            if (DisableIcon)
            {
                targetIconRenderer.enabled = false;
                return;
            }

            if (Profile.IconMaterial == null || Profile.IconMesh == null)
            {
                return;
            }

            // Instantiate our local material now, if we don't have one
            if (instantiatedMaterial == null)
            {
                instantiatedMaterial = new Material(Profile.IconMaterial)
                {
                    name = Profile.IconMaterial.name
                };
            }
            targetIconRenderer.sharedMaterial = instantiatedMaterial;

            // Instantiate our local mesh now, if we don't have one
            if (instantiatedMesh == null)
            {
                instantiatedMesh = Instantiate(Profile.IconMesh);
                instantiatedMesh.name = Profile.IconMesh.name;
            }
            IconMeshFilter.sharedMesh = instantiatedMesh;

            if (OverrideIcon)
            {
                // Use the default mesh for override icons
                targetIconRenderer.enabled = true;
                IconMeshFilter.sharedMesh = Profile.IconMesh;
                IconMeshFilter.transform.localScale = Vector3.one;
                instantiatedMaterial.mainTexture = iconOverride;
                return;
            }

            // Disable the renderer if the name is empty
            if (string.IsNullOrEmpty(newName))
            {
                targetIconRenderer.enabled = false;
                return;
            }

            // Moment of truth - try to get our icon
            if (!Profile.GetIcon(newName, targetIconRenderer, IconMeshFilter, true))
            {
                targetIconRenderer.enabled = false;
                return;
            }

            // If we've made it this far we're golden
            targetIconRenderer.enabled = true;
            RefreshAlpha();
        }

        private void OnDisable()
        {
            ClearInstancedAssets();
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                ClearInstancedAssets();
            }

            SetIconName(iconName);
        }

        private void Start()
        {
            SetIconName(iconName);
        }

        private void RefreshAlpha()
        {
            string alphaColorProperty = string.IsNullOrEmpty(Profile.AlphaColorProperty) ? "_Color" : Profile.AlphaColorProperty;
            if (instantiatedMaterial != null)
            {
                Color c = instantiatedMaterial.GetColor(alphaColorProperty);
                c.a = alpha;
                instantiatedMaterial.SetColor(alphaColorProperty, c);
            }
        }

        private void ClearInstancedAssets()
        {
            // Prevent material leaks
            if (instantiatedMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(instantiatedMaterial);
                }
                else
                {
                    DestroyImmediate(instantiatedMaterial);
                }

                instantiatedMaterial = null;
            }
            if (instantiatedMesh != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(instantiatedMesh);
                }
                else
                {
                    DestroyImmediate(instantiatedMesh);
                }

                instantiatedMesh = null;
            }

            // Reset to default mats and meshes
            if (Profile != null)
            {
                if (targetIconRenderer != null)
                {
                    // Restore the icon material to the renderer
                    targetIconRenderer.sharedMaterial = Profile.IconMaterial;
                }
                if (IconMeshFilter != null)
                {
                    IconMeshFilter.sharedMesh = Profile.IconMesh;
                }
            }
            // Reset our alpha to the alpha target
            alpha = alphaTarget;
        }

        private IEnumerator UpdateAlpha()
        {
            float startTime = Time.time;
            Color color = Color.white;
            string alphaColorProperty = string.IsNullOrEmpty(Profile.AlphaColorProperty) ? "_Color" : Profile.AlphaColorProperty;

            if (instantiatedMaterial != null)
            {
                color = instantiatedMaterial.GetColor(alphaColorProperty);
                color.a = alpha;
            }

            while (Time.time < startTime + Profile.AlphaTransitionSpeed)
            {
                alpha = Mathf.Lerp(alpha, alphaTarget, (Time.time - startTime) / Profile.AlphaTransitionSpeed);
                if (instantiatedMaterial != null && !string.IsNullOrEmpty(alphaColorProperty))
                {
                    color.a = alpha;
                    instantiatedMaterial.SetColor(alphaColorProperty, color);
                }
                yield return null;
            }

            alpha = alphaTarget;
            RefreshAlpha();
            updatingAlpha = false;
        }
    }
}

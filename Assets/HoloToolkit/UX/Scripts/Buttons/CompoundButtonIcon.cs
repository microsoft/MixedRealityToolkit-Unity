//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonIcon : ProfileButtonBase<ButtonIconProfile>
    {
        /// <summary>
        /// Turns off the icon entirely
        /// </summary>
        public bool DisableIcon = false;

        /// <summary>
        /// Disregard the icon in the profile
        /// </summary>
        public bool OverrideIcon = false;

        /// <summary>
        /// Icon to use for override
        /// </summary>
        public Texture2D IconOverride
        {
            get
            {
                return iconOverride;
            }
            set
            {
                iconOverride = value;
                SetIconName(string.Empty);
            }
        }

        [SerializeField]
        private Texture2D iconOverride;

        [SerializeField]
        private string iconName;

        [SerializeField]
        private float alpha = 1f;

        [SerializeField]
        private MeshRenderer targetIconRenderer;

        private Material instantiatedMaterial;
        private Mesh instantiatedMesh;
        private bool updatingAlpha = false;
        private float alphaTarget = 1f;

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
                        if (Mathf.Abs (alpha - alphaTarget) < 0.01f)
                            return;

                        if (updatingAlpha)
                            return;

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

        public MeshRenderer IconRenderer
        {
            get
            {
                return targetIconRenderer;
            }
            set
            {
                targetIconRenderer = value;
            }
        }

        public MeshFilter IconMeshFilter
        {
            get
            {
                return IconRenderer != null ? IconRenderer.GetComponent<MeshFilter>() : null;
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

            if (IconRenderer == null)
            {
                return;
            }

            if (DisableIcon)
            {
                IconRenderer.enabled = false;
                return;
            }

            if (Profile.IconMaterial == null || Profile.IconMesh == null)
            {
                return;
            }

            // Instantiate our local material now, if we don't have one
            if (instantiatedMaterial == null)
            {
                instantiatedMaterial = new Material(Profile.IconMaterial);
                instantiatedMaterial.name = Profile.IconMaterial.name;
            }
            IconRenderer.sharedMaterial = instantiatedMaterial;
            
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
                IconRenderer.enabled = true;
                IconMeshFilter.sharedMesh = Profile.IconMesh;
                IconMeshFilter.transform.localScale = Vector3.one;
                instantiatedMaterial.mainTexture = iconOverride;
                return;
            }

            // Disable the renderer if the name is empty
            if (string.IsNullOrEmpty(newName))
            {
                IconRenderer.enabled = false;
                iconName = newName;
                return;
            }
            
            // Moment of truth - try to get our icon
            if (!Profile.GetIcon(iconName, IconRenderer, IconMeshFilter, true))
            {
                IconRenderer.enabled = false;
                return;
            }

            // If we've made it this far we're golden
            IconRenderer.enabled = true;
            iconName = newName;
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
                if (IconRenderer != null)
                {
                    // Restore the icon material to the renderer
                    IconRenderer.sharedMaterial = Profile.IconMaterial;
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

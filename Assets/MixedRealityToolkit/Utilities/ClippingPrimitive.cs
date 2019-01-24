  // Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// An abstract primitive component to animate and visualize a clipping primitive that can be 
    /// used to drive per pixel based clipping.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class ClippingPrimitive : MonoBehaviour
    {
        [Tooltip("The renderer(s) that should be affected by the primitive.")]
        [SerializeField]
        protected List<Renderer> renderers = new List<Renderer>();

        public enum Side
        {
            Inside = 1,
            Outside = -1
        }

        [Tooltip("Which side of the primitive to clip pixels against.")]
        [SerializeField]
        protected Side clippingSide = Side.Inside;

        /// <summary>
        /// The renderer(s) that should be affected by the primitive.
        /// </summary>
        public Side ClippingSide
        {
            get { return clippingSide; }
            set { clippingSide = value; }
        }

        protected abstract string Keyword { get; }
        protected abstract string KeywordProperty { get; }
        protected abstract string ClippingSideProperty { get; }

        protected MaterialPropertyBlock materialPropertyBlock;
        protected Dictionary<Material, bool> modifiedMaterials = new Dictionary<Material, bool>();
        protected List<Material> allocatedMaterials = new List<Material>();

        private int clippingSideID;

        public void AddRenderer(Renderer _renderer)
        {
            if (_renderer != null && !renderers.Contains(_renderer))
            {
                renderers.Add(_renderer);

                Material material = GetMaterial(_renderer, false);

                if (material != null)
                {
                    ToggleClippingFeature(material, true);
                }
            }
        }

        public void RemoveRenderer(Renderer _renderer)
        {
            if (renderers.Contains(_renderer))
            {
                Material material = GetMaterial(_renderer, false);

                if (material != null)
                {
                    ToggleClippingFeature(material, false);
                }

                renderers.Remove(_renderer);
            }
        }

        protected void OnValidate()
        {
            ToggleClippingFeature(true);
            RestoreUnassignedMaterials();
        }

        protected void OnEnable()
        {
            Initialize();
            UpdateRenderers();
            ToggleClippingFeature(true);
        }

        protected void OnDisable()
        {
            UpdateRenderers();
            ToggleClippingFeature(false);
        }

#if UNITY_EDITOR
        // We need this class to be updated once per frame even when in edit mode. Ideally this would 
        // occur after all other objects are updated in LateUpdate(), but because the ExecuteInEditMode 
        // attribute only invokes Update() we handle edit mode updating in Update() and runtime updating 
        // in LateUpdate().
        protected void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdateRenderers();
        }
#endif

        protected void LateUpdate()
        {
            UpdateRenderers();
        }

        protected void OnDestroy()
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                Material material = GetMaterial(renderers[i]);

                if (material != null)
                {
                    bool clippingPlaneOn;

                    if (modifiedMaterials.TryGetValue(material, out clippingPlaneOn))
                    {
                        ToggleClippingFeature(material, clippingPlaneOn);
                        modifiedMaterials.Remove(material);
                    }
                }
            }

            RestoreUnassignedMaterials();

            for (int i = 0; i < allocatedMaterials.Count; ++i)
            {
                Destroy(allocatedMaterials[i]);
            }
        }

        protected virtual void Initialize()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            clippingSideID = Shader.PropertyToID(ClippingSideProperty);
        }

        protected virtual void UpdateRenderers()
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                Renderer _renderer = renderers[i];

                if (_renderer == null)
                {
                    continue;
                }

                _renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetFloat(clippingSideID, (float)clippingSide);
                UpdateShaderProperties(materialPropertyBlock);
                _renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        protected abstract void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock);

        protected void ToggleClippingFeature(bool keywordOn)
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Count; ++i)
            {
                Material material = GetMaterial(renderers[i]);

                if (material != null)
                {
                    // Cache the initial keyword state of the material.
                    if (!modifiedMaterials.ContainsKey(material))
                    {
                        modifiedMaterials[material] = material.IsKeywordEnabled(Keyword);
                    }

                    ToggleClippingFeature(material, keywordOn);
                }
            }
        }

        protected void ToggleClippingFeature(Material material, bool keywordOn)
        {
            if (keywordOn)
            {
                material.EnableKeyword(Keyword);
                material.SetFloat(KeywordProperty, 1.0f);
            }
            else
            {
                material.DisableKeyword(Keyword);
                material.SetFloat(KeywordProperty, 0.0f);
            }
        }

        protected Material GetMaterial(Renderer _renderer, bool trackAllocations = true)
        {
            if (_renderer == null)
            {
                return null;
            }

            if (Application.isEditor && !Application.isPlaying)
            {
                return _renderer.sharedMaterial;
            }
            else
            {
                Material material = _renderer.material;

                if (trackAllocations && !allocatedMaterials.Contains(material))
                {
                    allocatedMaterials.Add(material);
                }

                return material;
            }
        }

        protected void RestoreUnassignedMaterials()
        {
            List<Material> toRemove = new List<Material>();

            foreach (var modifiedMaterial in modifiedMaterials)
            {
                if (modifiedMaterial.Key == null)
                {
                    toRemove.Add(modifiedMaterial.Key);
                }
                else if (renderers.Find(x => (GetMaterial(x) == modifiedMaterial.Key)) == null)
                {
                    ToggleClippingFeature(modifiedMaterial.Key, modifiedMaterial.Value);
                    toRemove.Add(modifiedMaterial.Key);
                }
            }

            foreach (var material in toRemove)
            {
                modifiedMaterials.Remove(material);
            }
        }
    }
}

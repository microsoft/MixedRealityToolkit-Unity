// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// An abstract primitive component to animate and visualize a clipping primitive that can be 
    /// used to drive per pixel based clipping.
    /// </summary>
    [ExecuteAlways]
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

        [SerializeField]
        [Tooltip("Toggles whether the primitive will use the Camera OnPreRender event")]
        private bool useOnPreRender;

        /// <summary>
        /// Toggles whether the primitive will use the Camera OnPreRender event.
        /// </summary>
        /// <remarks>This is especially helpful if you're trying to clip dynamically created objects that may be added to the scene after LateUpdate such as OnWillRender</remarks>
        public bool UseOnPreRender
        {
            get { return useOnPreRender; }
            set
            {
                if (cameraMethods == null)
                {
                    cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                }

                if (value)
                {
                    cameraMethods.OnCameraPreRender += OnCameraPreRender;
                }
                else
                {
                    cameraMethods.OnCameraPreRender -= OnCameraPreRender;
                }

                useOnPreRender = value;
            }
        }

        protected abstract string Keyword { get; }
        protected abstract string ClippingSideProperty { get; }

        protected MaterialPropertyBlock materialPropertyBlock;
        protected static Dictionary<Material, Material> materialClones = new Dictionary<Material, Material>();

        private int clippingSideID;
        private CameraEventRouter cameraMethods;

        /// <summary>
        /// Adds a renderer to the list of objects this clipping primitive clips.
        /// </summary>
        /// <param name="_renderer"></param>
        public void AddRenderer(Renderer _renderer)
        {
            if (!renderers.Contains(_renderer))
            {
                renderers.Add(_renderer);
            }

            var material = GetMaterial(_renderer);

            if (material != null)
            {
                ToggleClippingFeature(material, true);
            }
        }

        /// <summary>
        /// Removes a renderer to the list of objects this clipping primitive clips.
        /// </summary>
        public void RemoveRenderer(Renderer _renderer)
        {
            renderers.Remove(_renderer);

            // Restore the original material.
            var material = GetMaterial(_renderer);

            if (material != null)
            {
                _renderer.sharedMaterial = materialClones[material];
                materialClones.Remove(material);

                if (Application.isPlaying)
                {
                    Destroy(material);
                }
                else
                {
                    DestroyImmediate(material);
                }
            }
        }

        /// <summary>
        /// Returns a copy of the current list of renderers.
        /// </summary>
        /// <returns>The current list of renderers.</returns>
        public IEnumerable<Renderer> GetRenderersCopy()
        {
            return new List<Renderer>(renderers);
        }

        #region MonoBehaviour Implementation

        protected void OnEnable()
        {
            Initialize();
            UpdateRenderers();
            ToggleClippingFeature(true);

            if (useOnPreRender)
            {
                cameraMethods = CameraCache.Main.gameObject.EnsureComponent<CameraEventRouter>();
                cameraMethods.OnCameraPreRender += OnCameraPreRender;
            }
        }

        protected void OnDisable()
        {
            UpdateRenderers();
            ToggleClippingFeature(false);

            if (cameraMethods != null)
            {
                cameraMethods.OnCameraPreRender -= OnCameraPreRender;
            }
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
            //Deferring the LateUpdate() call to OnCameraPreRender()
            if (!useOnPreRender)
            {
                UpdateRenderers();
            }
        }

        protected void OnCameraPreRender(CameraEventRouter router)
        {
            UpdateRenderers();
        }

        protected void OnDestroy()
        {
            if (renderers == null)
            {
                return;
            }

            while (renderers.Count != 0)
            {
                RemoveRenderer(renderers[0]);
            }
        }

        #endregion MonoBehaviour Implementation

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

            for (var i = 0; i < renderers.Count; ++i)
            {
                var _renderer = renderers[i];

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

            for (var i = 0; i < renderers.Count; ++i)
            {
                var material = GetMaterial(renderers[i]);

                if (material != null)
                {
                    ToggleClippingFeature(material, keywordOn);
                }
            }
        }

        protected void ToggleClippingFeature(Material material, bool keywordOn)
        {
            if (keywordOn)
            {
                material.EnableKeyword(Keyword);
            }
            else
            {
                material.DisableKeyword(Keyword);
            }
        }

        protected Material GetMaterial(Renderer _renderer)
        {
            Material output = null;

            if (_renderer != null)
            {
                var material = _renderer.sharedMaterial;

                if (material != null && !materialClones.TryGetValue(material, out output))
                {
                    // Create a material clone. This keeps the code path the same at edit time and 
                    // run time since renderer.material cannot be invoked when editing.
                    output = new Material(material);
                    output.name += " (Clone)";
                    materialClones.Add(output, material);
                    _renderer.sharedMaterial = output;
                }
                else
                {
                    output = material;
                }
            }

            return output;
        }
    }
}

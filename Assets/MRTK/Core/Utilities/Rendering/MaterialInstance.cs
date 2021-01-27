// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Microsoft.MixedReality.Toolkit.Rendering
{
    /// <summary>
    /// The MaterialInstance behavior aides in tracking instance material lifetime and automatically destroys instanced materials for the user. 
    /// This utility component can be used as a replacement to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see> or 
    /// <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>. When invoking Unity's Renderer.material(s), Unity 
    /// automatically instantiates new materials. It is the caller's responsibility to destroy the materials when a material is no longer needed or the game object is 
    /// destroyed. The MaterialInstance behavior helps avoid material leaks and keeps material allocation paths consistent during edit and run time.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Rendering/MaterialInstance.html")]
    [ExecuteAlways, RequireComponent(typeof(Renderer))]
    [AddComponentMenu("Scripts/MRTK/Core/MaterialInstance")]
    public class MaterialInstance : MonoBehaviour
    {
        /// <summary>
        /// Returns the first instantiated Material assigned to the renderer, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see>. 
        /// If any owner is specified the instanced material(s) will not be released until all owners are released. When a material
        /// is no longer needed ReleaseMaterial should be called with the matching owner.
        /// </summary>
        /// <param name="owner">An optional owner to track instance ownership.</param>
        /// <returns>The first instantiated Material.</returns>
        public Material AcquireMaterial(Object owner = null, bool instance = true)
        {
            if (owner != null)
            {
                _materialOwners.Add(owner);
            }

            if (instance)
            {
                AcquireInstances();
            }

            return _instanceMaterials?.Length > 0 ? _instanceMaterials[0] : null;
        }

        /// <summary>
        /// Returns all the instantiated materials of this object, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>. 
        /// If any owner is specified the instanced material(s) will not be released until all owners are released. When a material
        /// is no longer needed ReleaseMaterial should be called with the matching owner.
        /// </summary>
        /// <param name="owner">An optional owner to track instance ownership.</param>
        /// <param name="instance">Should this acquisition attempt to instance materials?</param>
        /// <returns>All the instantiated materials.</returns>
        public Material[] AcquireMaterials(Object owner = null, bool instance = true)
        {
            if (owner != null)
            {
                _materialOwners.Add(owner);
            }

            if (instance)
            {
                AcquireInstances();
            }

            return _instanceMaterials;
        }

        /// <summary>
        /// Relinquishes ownership of a material instance. This should be called when a material is no longer needed
        /// after acquire ownership with AcquireMaterial(s).
        /// </summary>
        /// <param name="owner">The same owner which originally acquire ownership via AcquireMaterial(s).</param>
        /// <param name="instance">Should this acquisition attempt to instance materials?</param>
        /// <param name="autoDestroy">When ownership count hits zero should the MaterialInstance component be destroyed?</param>
        public void ReleaseMaterial(Object owner, bool autoDestroy = true)
        {
            _materialOwners.Remove(owner);

            if (autoDestroy && _materialOwners.Count == 0)
            {
                DestroySafe(this);

                // OnDestroy not called on inactive objects
                if (!gameObject.activeInHierarchy)
                {
                    RestoreRenderer();
                }
            }
        }
        
        /// <summary>
        /// Returns the first instantiated Material assigned to the renderer, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-material.html">Renderer.material</see>.
        /// </summary>
        public Material Material => AcquireMaterial();

        /// <summary>
        /// Returns all the instantiated materials of this object, similar to <see href="https://docs.unity3d.com/ScriptReference/Renderer-materials.html">Renderer.materials</see>.
        /// </summary>
        public Material[] Materials => AcquireMaterials();

        private Renderer CachedRenderer
        {
            get
            {
                if (!_cachedRenderer)
                {
                    _cachedRenderer = GetComponent<Renderer>();
                    _cachedSharedMaterials = _cachedRenderer.sharedMaterials;

                    return _cachedRenderer;
                }

                return _cachedRenderer;
            }
        }

        private Material[] CachedSharedMaterials
        {
            get => _cachedSharedMaterials;
            set
            {
                _cachedSharedMaterials = value;
                _cachedRenderer.sharedMaterials = value;
            }
        }
        
        
        private Renderer _cachedRenderer;

        [SerializeField]
        [HideInInspector]
        private Material[] defaultMaterials;
        private Material[] _instanceMaterials;
        private Material[] _cachedSharedMaterials;
        private bool _initialized;
        private bool _materialsInstanced;
        private readonly HashSet<Object> _materialOwners = new HashSet<Object>();

        private const string InstancePostfix = " (Instance)";
        
        #region MonoBehaviour Implementation

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            // If the materials get changed via outside of MaterialInstance.
            var sharedMaterials = CachedSharedMaterials;

            if (!MaterialsMatch(sharedMaterials, _instanceMaterials))
            {
                // Re-create the material instances.
                var newDefaultMaterials = new Material[sharedMaterials.Length];
                var min = Math.Min(newDefaultMaterials.Length, defaultMaterials.Length);

                // Copy the old defaults.
                for (var i = 0; i < min; ++i)
                {
                    newDefaultMaterials[i] = defaultMaterials[i];
                }

                // Patch in the new defaults.
                for (var i = 0; i < newDefaultMaterials.Length; ++i)
                {
                    var material = sharedMaterials[i];

                    if (!IsInstanceMaterial(material))
                    {
                        newDefaultMaterials[i] = material;
                    }
                }

                defaultMaterials = newDefaultMaterials;
                CreateInstances();

                // Notify owners of the change.
                foreach (var owner in _materialOwners)
                {
                    (owner as IMaterialInstanceOwner)?.OnMaterialChanged(this);
                }
            }
        }

        private void OnDestroy()
        {
            RestoreRenderer();
        }

        private void RestoreRenderer()
        {
            if (CachedRenderer && defaultMaterials != null)
            {
                CachedSharedMaterials = defaultMaterials;
            }

            DestroyMaterials(_instanceMaterials);
            _instanceMaterials = null;
        }

        #endregion MonoBehaviour Implementation


        private void Initialize()
        {
            if (!_initialized && CachedRenderer)
            {
                // Cache the default materials if ones do not already exist.
                if (!HasValidMaterial(defaultMaterials))
                {
                    defaultMaterials = CachedSharedMaterials;
                }
                else if (!_materialsInstanced) // Restore the clone to its initial state.
                {
                    CachedSharedMaterials = defaultMaterials;
                }

                _initialized = true;
            }
        }

        private void AcquireInstances()
        {
            if (CachedRenderer)
            {
                if (!MaterialsMatch(CachedSharedMaterials, _instanceMaterials))
                {
                    CreateInstances();
                }
            }
        }

        private void CreateInstances()
        {
            // Initialize must get called to set the defaultMaterials in case CreateInstances get's invoked before Awake.
            Initialize();

            DestroyMaterials(_instanceMaterials);
            _instanceMaterials = InstanceMaterials(defaultMaterials);

            if (CachedRenderer && _instanceMaterials != null)
            {
                CachedSharedMaterials = _instanceMaterials;
            }

            _materialsInstanced = true;
        }

        private static bool MaterialsMatch(Material[] a, Material[] b)
        {
            if (a?.Length != b?.Length)
            {
                return false;
            }

            for (var i = 0; i < a?.Length; ++i)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        private static Material[] InstanceMaterials(Material[] source)
        {
            if (source == null)
            {
                return null;
            }

            var output = new Material[source.Length];

            for (var i = 0; i < source.Length; ++i)
            {
                if (source[i])
                {
                    if (IsInstanceMaterial(source[i]))
                    {
                        Debug.LogWarning($"A material ({source[i].name}) which is already instanced was instanced multiple times.");
                    }

                    output[i] = new Material(source[i]);
                    output[i].name = $"{output[i].name}{InstancePostfix}";
                }
            }

            return output;
        }

        private static void DestroyMaterials(Material[] materials)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    DestroySafe(material);
                }
            }
        }

        private static bool IsInstanceMaterial(Material material)
        {
            return material && material.name.Contains(InstancePostfix);
        }

        private static bool HasValidMaterial(Material[] materials)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    if (material)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        private static void DestroySafe(Object toDestroy)
        {
            if (toDestroy)
            {
                if (Application.isPlaying)
                {
                    Destroy(toDestroy);
                }
                else
                {
#if UNITY_EDITOR
                    // Let Unity handle unload of unused assets if lifecycle is transitioning from editor to play mode
                    // Defering the call during this transition would destroy reference only after play mode Awake, leading to possible broken material references on TMPro objects
                    if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        EditorApplication.delayCall += () =>
                        {
                            if (toDestroy != null)
                                DestroyImmediate(toDestroy);
                        };
                    }
#endif
                }
            }
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// TODO
    /// </summary>
    [ExecuteAlways, RequireComponent(typeof(Renderer))]
    public class MaterialInstance : MonoBehaviour
    {
        /// <summary>
        /// TODO
        /// </summary>
        public Material Material
        {
            get
            {
                AcquireInstances();

                if (instanceMaterials?.Length > 0)
                {
                    return instanceMaterials[0];
                }

                return null;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Material[] Materials
        {
            get
            {
                AcquireInstances();

                return instanceMaterials;
            }
        }

        [SerializeField]//, HideInInspector]
        private Material[] defaultMaterials = null;
        private Material[] instanceMaterials = null;

        private const string instancePostfix = " (Clone)";

        private void Awake()
        {
            var _renderer = GetComponent<Renderer>();

            // Cache the default materials if ones do not already exist.
            if (defaultMaterials == null || defaultMaterials.Length == 0)
            {
                defaultMaterials = _renderer?.sharedMaterials;
            }
            else // Restore the clone to it's initial state.
            {
                if (_renderer != null)
                {
                    _renderer.sharedMaterials = defaultMaterials;
                }
            }
        }

        private void OnDestroy()
        {
            var _renderer = GetComponent<Renderer>();

            if (_renderer != null)
            {
                _renderer.sharedMaterials = defaultMaterials;
            }

            DestroyMaterials(instanceMaterials);
        }

        private void AcquireInstances()
        {
            if (!MaterialsMatch(defaultMaterials, instanceMaterials))
            {
                DestroyMaterials(instanceMaterials);
                instanceMaterials = InstanceMaterials(defaultMaterials);

                var _renderer = GetComponent<Renderer>();

                if (_renderer != null)
                {
                    _renderer.sharedMaterials = instanceMaterials;
                }
            }
        }

        private static bool MaterialsMatch(Material[] a, Material[] b)
        {
            if (a?.Length != b?.Length)
            {
                return false;
            }

            for (int i = 0; i < a?.Length; ++i)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
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
                if (source[i] != null)
                {
                    Debug.Assert(!source[i].name.Contains(instancePostfix), "A material which is already instanced was instanced multiple times.");

                    output[i] = new Material(source[i]);
                    output[i].name += instancePostfix;
                }
            }

            return output;
        }

        private static void DestroyMaterials(Material[] materials)
        {
            if (materials != null)
            {
                for (var i = 0; i < materials.Length; ++i)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(materials[i]);
                    }
                    else
                    {
                        DestroyImmediate(materials[i]);
                    }
                }

                materials = null;
            }
        }
    }
}

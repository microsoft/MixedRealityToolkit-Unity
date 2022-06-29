// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("MRTK/Examples/Toggle Collection Color Change")]
    public class ToggleCollectionColorChange : MonoBehaviour
    {
        [Tooltip("The ToggleCollection for this color changer.")]
        [SerializeField]
        private ToggleCollection toggleCollection;

        /// <summary>
        /// The ToggleCollection for this color changer.
        /// </summary>
        public ToggleCollection ToggleCollection
        {
            get => toggleCollection;
            set => toggleCollection = value;
        }

        [Tooltip("The mesh renderer to update for this color changer.")]
        [SerializeField]
        private MeshRenderer meshRenderer;

        /// <summary>
        /// he mesh renderer to update for this color changer.
        /// </summary>
        public MeshRenderer MeshRenderer
        {
            get => meshRenderer;
            set => meshRenderer = value;
        }

        [Tooltip("The materials for this color changer.")]
        [SerializeField]
        private Material[] materials;

        /// <summary>
        /// The materials for this color changer.
        /// </summary>
        public Material[] Materials
        {
            get => materials;
            set => materials = value;
        }

        private void Start()
        {
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<MeshRenderer>();
            }

            if (materials.Length == 0)
            {
                Debug.LogWarning($"The materials array in the ToggleCollectionColorChange script attached to {gameObject.name} is empty, add materials.");
            }

            ToggleCollection.OnToggleSelected.AddListener((toggleSelectedIndex) =>
            {
                if (toggleSelectedIndex < materials.Length && toggleSelectedIndex > -1)
                {
                    meshRenderer.material = materials[toggleSelectedIndex];
                }
            });
        }
    }
}

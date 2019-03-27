// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    /// <summary>
    /// Helper class for creating and placing multiple prefabs at different locations in the scene
    /// </summary>
    public class DebugVisualHelper : MonoBehaviour
    {
        /// <summary>
        /// Prefab to create at the provided location
        /// </summary>
        [Tooltip("Prefab to create at the provided location")]
        [SerializeField]
        protected GameObject prefab;

        /// <summary>
        /// Scale applied to the provided prefab when shown at the specified location
        /// </summary>
        [Tooltip("Default scale to apply to the provided prefab")]
        [SerializeField]
        protected Vector3 _scale = Vector3.one;

        /// <summary>
        /// Call to transform or create and transform a GameObject to the provided position and rotation.
        /// </summary>
        /// <param name="visual">If null, a new GameObject will be istantiated from the provided prefab. If non-null the provided GameObject's position and rotation will be updated</param>
        /// <param name="position">Position to apply to the provided GameObject</param>
        /// <param name="rotation">Rotation to apply to the provided GameObject</param>
        public void CreateOrUpdateVisual(ref GameObject visual, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab not defined. No visual created");
            }

            if (visual == null)
            {
                visual = Instantiate(prefab);
            }

            if (visual != null)
            {
                SetTransform(visual, position, rotation);
            }
        }

        protected void SetTransform(GameObject visual, Vector3 position, Quaternion rotation)
        {
            visual.transform.position = position;
            visual.transform.rotation = rotation;
            visual.transform.localScale = _scale;
        }
    }
}


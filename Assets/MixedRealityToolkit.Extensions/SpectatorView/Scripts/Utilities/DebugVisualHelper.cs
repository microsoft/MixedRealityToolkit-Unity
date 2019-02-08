// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class DebugVisualHelper : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 _scale = Vector3.one;

        public GameObject CreateVisual(Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab not defined. No visual created");
                return null;
            }

            var visual = Instantiate(prefab);
            SetTransform(ref visual, position, rotation);
            return visual;
        }

        public void UpdateVisual(ref GameObject visual, Vector3 position, Quaternion rotation)
        {
            SetTransform(ref visual, position, rotation);
        }

        void SetTransform(ref GameObject visual, Vector3 position, Quaternion rotation)
        {
            visual.transform.position = position;
            visual.transform.rotation = rotation;
            visual.transform.localScale = _scale;
        }
    }
}


// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class DebugVisualHelper : MonoBehaviour
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 _scale = Vector3.one;

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

        void SetTransform(GameObject visual, Vector3 position, Quaternion rotation)
        {
            visual.transform.position = position;
            visual.transform.rotation = rotation;
            visual.transform.localScale = _scale;
        }
    }
}


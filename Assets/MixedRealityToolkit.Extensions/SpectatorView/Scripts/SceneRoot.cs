// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView
{
    public class SceneRoot : MonoBehaviour
    {
        private void Start()
        {
            var spectatorView = FindObjectOfType<SpectatorView>();
            if (spectatorView == null)
            {
                Debug.Log("Failed to find spectator view");
                return;
            }

            Debug.Log("Setting scene root: " + gameObject.name);
            spectatorView.SceneRoot = gameObject;
        }
    }
}

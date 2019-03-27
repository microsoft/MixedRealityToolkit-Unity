// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView
{
    /// <summary>
    /// A helper class for tagging the GameObject to transform relative to the shared application origin in a spectator view scene.
    /// On Start, this script will attempt to set its parent GameObject as <see cref="SpectatorView.SceneRoot"/>.
    /// </summary>
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

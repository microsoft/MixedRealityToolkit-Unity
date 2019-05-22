// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class Spin : MonoBehaviour
    {
        /// <summary>
        /// SpectatorView instance
        /// </summary>
        [Tooltip("SpectatorView instance")]
        [SerializeField]
        private SpectatorView spectatorView = null;

        void Update()
        {
            if ((StateSynchronizationDemo.Instance != null && StateSynchronizationDemo.Instance.StateSynchronizationRole == StateSynchronizationRole.Broadcaster) ||
                (spectatorView != null && spectatorView.Role == Role.User))
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 100 * Time.time, 0);
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Settings used by a <see cref="Broadcaster"/>
    /// </summary>
    public class BroadcasterSettings : Singleton<BroadcasterSettings>
    {
        /// <summary>
        /// Determines whether or not all GameObjects are synchronized or only those with a SynchronizedGameObjectHierarchy are synchronized.
        /// </summary>
        [SerializeField]
        [Tooltip("Determines whether or not all GameObjects are synchronized or only those with a SynchronizedGameObjectHierarchy are synchronized.")]
        private bool automaticallySynchronizeAllGameObjects = false;

        /// <summary>
        /// Determines whether or not all GameObjects are synchronized or only those with a SynchronizedGameObjectHierarchy are synchronized.
        /// </summary>
        public bool AutomaticallySynchronizeAllGameObjects
        {
            get { return automaticallySynchronizeAllGameObjects; }
        }
    }
}

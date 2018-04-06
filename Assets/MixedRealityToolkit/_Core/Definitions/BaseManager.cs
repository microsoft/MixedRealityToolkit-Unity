// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The base manager implements the IMixedRealityManager interface and provides default properties for all managers
    /// </summary>
    public class BaseManager : Interfaces.IMixedRealityManager
    {

        /// <summary>
        /// Controls whether the manager is enabled and active in the scene
        /// </summary>
        [SerializeField]
        [Tooltip("Is the selected manager enabled")]
        private bool enabled;

        /// <summary>
        /// public property for the enabled property, used to control whether the selected manager is active or not
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Virtual method for the IMixedRealityManager interface
        /// Subscription required to the "InitializeEvent" event of the MixedRealityManager
        /// </summary>
        public virtual void Initialize() { }
    }
}

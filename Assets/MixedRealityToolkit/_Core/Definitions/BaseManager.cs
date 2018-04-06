// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The base manager implements the IMixedRealityManager interface and provides default properties for all managers
    /// </summary>
    public class BaseManager : Interfaces.IMixedRealityManager
    {
        /// <summary>
        /// BaseManager constructor
        /// </summary>
        public BaseManager()
        {
            //Attach to Manager events
            MixedRealityManager.Instance.InitializeEvent += Initialize;
            MixedRealityManager.Instance.UpdateEvent += Update;
            MixedRealityManager.Instance.DestroyEvent += Destroy;
            MixedRealityManager.Instance.ProfileUpdateEvent += ProfileUpdate;
        }

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

        public virtual void Update() { }

        public virtual void Destroy() { }

        public virtual void ProfileUpdate() { }

    }
}

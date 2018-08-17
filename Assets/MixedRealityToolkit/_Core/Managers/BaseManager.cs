// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// The base manager implements the IMixedRealityManager interface and provides default properties for all managers
    /// </summary>
    public class BaseManager : Interfaces.IMixedRealityManager
    {
        /// <summary>
        /// Optional Name attribute if multiple managers of the same type are required, enables targeting a manager for action
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Optional Priority to reorder registered managers based on their respective priority, reduces the risk of race conditions by prioritizing the order in which managers are evaluated.
        /// </summary>
        public virtual uint Priority { get; set; }

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Optional Reset function to perform that will Reset the manager, for example, whenever there is a profile change.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Optional Enable function to enable / re-enable the manager.
        /// </summary>
        public virtual void Enable() { }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Optional Disable function to pause the manager.
        /// </summary>
        public virtual void Disable() { }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed.
        /// </summary>
        public virtual void Destroy() { }
    }
}

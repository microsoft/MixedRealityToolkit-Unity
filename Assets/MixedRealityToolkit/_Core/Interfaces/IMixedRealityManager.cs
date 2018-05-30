// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Generic interface for all Mixed Reality Managers
    /// </summary>
    public interface IMixedRealityManager
    {
        /// <summary>
        /// Optional Priority attribute if multiple managers of the same type are required, enables targeting a manager for action
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Optional Priority to reorder registered managers based on their respective priority, reduces the risk of race conditions by prioritizing the order in which managers are evaluated.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The initialize function is used to setup the manager once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Optional Reset function to perform that will Reset the manager, for example, whenever there is a profile change.
        /// </summary>
        void Reset();

        /// <summary>
        /// Optional Enable function to enable / re-enable the manager.
        /// </summary>
        void Enable();

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager.
        /// </summary>
        void Update();

        /// <summary>
        /// Optional Disable function to pause the manager.
        /// </summary>
        void Disable();

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed.
        /// </summary>
        void Destroy();
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// The base manager implements the IMixedRealityManager interface and provides default properties for all managers
    /// </summary>
    public class BaseManager : Interfaces.IMixedRealityManager
    {
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
        /// Optional Update function to perform per-frame updates of the manager.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed.
        /// </summary>
        public virtual void Destroy() { }
    }
}

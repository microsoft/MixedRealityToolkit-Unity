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
        /// The initialize function is used to setup the manager once created.  Called once all managers have been registered in the Mixed Reality Manager
        /// </summary>
        void Initialize();
    }
}
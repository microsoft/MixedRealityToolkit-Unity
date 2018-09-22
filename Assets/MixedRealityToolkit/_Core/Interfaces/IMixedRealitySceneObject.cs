// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces
{
    /// <summary>
    /// Generic interface for all optional Mixed Reality systems, components, or features that can be added to the <see cref="Definitions.MixedRealityComponentConfiguration"/>
    /// </summary>
    public interface IMixedRealitySceneObject 
    {
        /// <summary>
        /// Register the gameobject with the Mixed Reality Manager Scene Object registry
        /// </summary>
        void RegisterSceneObject();

        /// <summary>
        /// Unregister the gameobject with the Mixed Reality Manager Scene Object registry
        /// </summary>
        void UnregisterSceneObject();
    }
}

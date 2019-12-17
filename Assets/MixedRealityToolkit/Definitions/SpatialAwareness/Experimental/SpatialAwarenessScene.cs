// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// A group of <see cref="SpatialAwarenessSceneObject"/>s at a moment in time returned by a <see cref="BaseSpatialSceneObserver"/>.
    /// </summary>
    public class SpatialAwarenessScene
    {
        /// <summary>
        /// A unique number representing the <see cref="SpatialAwarenessScene"/>
        /// </summary>
        public int Id { get; set; }

        private List<SpatialAwarenessSceneObject> childSceneObjects = new List<SpatialAwarenessSceneObject>();

        /// <summary>
        /// Returns list of child planes
        /// </summary>
        public IReadOnlyList<SpatialAwarenessSceneObject> ChildSceneObjects => childSceneObjects.AsReadOnly();
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// which method to use to calculate bounds since not all objects have colliders.
    /// </summary>
    public enum BoundsCalculationMethodEnum
    {
        MeshFilterBounds,   // Better for flattened objects - this mode also treats RectTransforms as quad meshes
        RendererBounds,     // Better for objects with non-mesh renderers
        Colliders,          // Better if you want precise control
        Default,            // Use the default method (RendererBounds)
    }
}
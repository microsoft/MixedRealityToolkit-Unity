// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A near interaction object which is a flat surface and can be pressed in one direction.
    /// </summary>
    public abstract class NearInteractionTouchableSurface : BaseNearInteractionTouchable
    {
        /// <summary>
        /// The local center point of interaction.  This may be based on a collider position or Unity UI RectTransform.
        /// </summary>
        public abstract Vector3 LocalCenter { get; }

        /// <summary>
        /// This is the direction that a user will press on this element.
        /// </summary>
        public abstract Vector3 LocalPressDirection { get; }

        /// <summary>
        /// Bounds specify where touchable interactions can occur.  They are local bounds on the plane specified by the LocalCenter and LocalPressDirection (as a normal).
        /// </summary>
        public abstract Vector2 Bounds { get; }
    }
}

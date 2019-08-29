// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A near interaction object which can be pressed.  This interface is used by PressableButton.
    /// </summary>
    public interface INearInteractionTouchableDirected
    {
        /// <summary>
        /// The local center point of interaction.  This may be based on a collider position or Unity UI RectTransform.
        /// </summary>
        Vector3 LocalCenter { get; }

        /// <summary>
        /// This is the direction that a user will press on this element.
        /// </summary>
        Vector3 LocalPressDirection { get; }

        /// <summary>
        /// Bounds specify where touchable interactions can occur.  They are local bounds on the plane specified by the LocalCenter and LocalPressDirection (as a normal).
        /// </summary>
        Vector2 Bounds { get; }

        /// <summary>
        /// The transform of the near interaction touchable directed object.
        /// </summary>
        Transform transform { get; }
    }
}

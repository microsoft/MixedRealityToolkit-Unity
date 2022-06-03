// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Flags that are used to provide contextual data about the object being described.
    /// </summary>
    [Flags]
    public enum DescribableFlag
    {
        /// <summary>
        /// Indicates that the object is relevant in the context of the scene / scenario.
        /// </summary>
        IsSalient = 1 << 0,

        /// <summary>
        /// The object is interactable (ex: button control or a non-player character).
        /// </summary>
        Interactable = 1 << 1,

        /// <summary>
        /// The object is static (ex: informational text or a tree).
        /// </summary>
        Static = 1 << 2,

        /// <summary>
        /// The object is part of the user interface (ex: a sign-in button).
        /// </summary>
        UserInterface = 1 << 3,

        /// <summary>
        /// The object is an item within the world (ex: a table).
        /// </summary>
        Item = 1 << 4,

        /// <summary>
        /// The object is a part of the background environment.
        /// </summary>
        Background = 1 << 5,

        /// <summary>
        /// A point of interest in the world (ex: a gathering place or quest location).
        /// </summary>
        PointOfInterest = 1 << 6
    }
}

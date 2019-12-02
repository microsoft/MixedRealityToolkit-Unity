// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The ExperienceScale identifies the environment for which the experience is designed.
    /// </summary>
    [System.Serializable]
    public enum ExperienceScale
    {
        /// <summary>
        /// An experience which utilizes only the headset orientation and is gravity aligned. The coordinate system origin is at head level.
        /// </summary>
        OrientationOnly = 0,
        /// <summary>
        /// An experience designed for seated use. The coordinate system origin is at head level.
        /// </summary>
        Seated,
        /// <summary>
        /// An experience designed for stationary standing use. The coordinate system origin is at floor level.
        /// </summary>
        Standing,
        /// <summary>
        /// An experience designed to support movement throughout a room. The coordinate system origin is at floor level.
        /// </summary>
        Room,
        /// <summary>
        /// An experience designed to utilize and move through the physical world. The coordinate system origin is at head level.
        /// </summary>
        World
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumeration defining the types of events that spatial awareness subsystems will send.
    /// </summary>
    public enum MixedRealitySpatialAwarenessEventType
    {
        /// <summary>
        /// A spatial awareness subsystem is reporting that a new spatial element 
        /// has been identified.
        /// </summary>
        Added = 0,

        /// <summary>
        /// A spatial awareness subsystem is reporting that an existing spatial
        /// element has been modified.
        /// </summary>
        Updated,

        /// <summary>
        /// A spatial awareness subsystem is reporting that an existing spatial
        /// element has been discarded.
        /// </summary>
        Deleted
    }
}

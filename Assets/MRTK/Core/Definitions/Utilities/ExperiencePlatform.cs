// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The ExperiencePlatform identifies the platform for which the experience is designed.
    /// </summary>
    [System.Serializable]
    public enum ExperiencePlatform
    {
        /// <summary>
        /// An experience which the user see's virtual objects placed on top of the real world
        /// </summary>
        AR = 0,
        /// <summary>
        /// An experience which the user is fully immersed in a virtual environment with virtual objects.
        /// </summary>
        VR
    }
}
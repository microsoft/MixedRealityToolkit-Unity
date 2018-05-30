// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Examples.Graph
{
    /// <summary>
    /// Class that represents a partial Graph profile.
    /// </summary>
    [Serializable]
    public struct GraphProfile
    {
        /// <summary>
        /// The display name.
        /// </summary>
        public string displayName;

        /// <summary>
        /// The job title.
        /// </summary>
        public string jobTitle;
    }
}

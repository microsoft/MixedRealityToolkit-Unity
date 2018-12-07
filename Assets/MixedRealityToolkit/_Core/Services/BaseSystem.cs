// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Definitions;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base class for Mixed Reality Systems to inherit from.
    /// </summary>
    public abstract class BaseSystem : BaseServiceWithConstructor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseSystem(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority)
        {
            Debug.Assert(profile != null, $"Missing the profile for {name}");
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using System;

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
        /// <param name="profile"></param>
        protected BaseSystem(BaseMixedRealityProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentException($"Missing the profile for {base.Name} system!");
            }
        }
    }
}
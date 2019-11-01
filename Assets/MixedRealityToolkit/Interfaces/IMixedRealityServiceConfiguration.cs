// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines a system, feature, or manager to be registered with as a <see cref="IMixedRealityExtensionService"/> on startup.
    /// </summary>
    public interface IMixedRealityServiceConfiguration
    {
        /// <summary>
        /// The concrete type for the system, feature or manager.
        /// </summary>
        SystemType ComponentType { get; }

        /// <summary>
        /// The name of the system, feature or manager.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// The priority this system, feature or manager will be initialized in.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The runtime platform(s) to run this service.
        /// </summary>
        SupportedPlatforms RuntimePlatform { get; }
    }
}
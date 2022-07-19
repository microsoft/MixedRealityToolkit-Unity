// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a HandsAggregatorSubsystem needs to be able to provide.
    /// Both the HandsAggregatorSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// <remarks>
    /// HandsAggregators aggregate skeletal hand joint data from all available sources.
    /// Implementations can aggregate hand joint data from multiple APIs, or from multiple
    /// HandsSubsystems, or from any other source they choose.
    /// Recommended use is for aggregating from all loaded HandsSubsystems.
    /// See <cref see="MRTKHandsAggregatorSubsystem"> for the MRTK implementation.
    /// </remarks>
    /// </summary>
    public interface IControllerModelSubsystem
    {

    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit data provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityDataProviderAttribute : MixedRealityExtensionServiceAttribute
    {
        /// <summary>
        /// The interface type of the IMixedRealityService for which the data provider is supported.
        /// </summary>
        public Type ServiceInterfaceType { get; }

        /// <summary>
        /// The supported Unity XR pipelines for this data provider.
        /// </summary>
        public SupportedUnityXRPipelines SupportedUnityXRPipelines { get; }

        public MixedRealityDataProviderAttribute(
            Type serviceInterfaceType,
            SupportedPlatforms runtimePlatforms,
            string name = "",
            string profilePath = "",
            string packageFolder = "MixedRealityToolkit",
            bool requiresProfile = false,
            SupportedUnityXRPipelines supportedUnityXRPipelines = (SupportedUnityXRPipelines)(-1))
            : base(runtimePlatforms, name, profilePath, packageFolder, requiresProfile)
        {
            ServiceInterfaceType = serviceInterfaceType;
            SupportedUnityXRPipelines = supportedUnityXRPipelines;
        }
    }
}

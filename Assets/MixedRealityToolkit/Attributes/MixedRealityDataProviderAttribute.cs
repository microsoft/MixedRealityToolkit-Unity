// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor.Setup;
using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Attributes
{
    /// <summary>
    /// Attribute that defines the properties of a Mixed Reality Toolkit data provider.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class MixedRealityDataProviderAttribute : MixedRealityExensionServiceAttribute
    {
        /// <summary>
        /// The interface type of the IMixedRealityService for which the data provider is supported.
        /// </summary>
        public Type ServiceInterfaceType { get; }

        public MixedRealityDataProviderAttribute(
            Type serviceInterfaceType,
            SupportedPlatforms runtimePlatforms,
            string profilePath = "",
            string packageFolder = "MixedRealityToolkit") : base(runtimePlatforms, profilePath, packageFolder)
        {
            ServiceInterfaceType = serviceInterfaceType;
        }
    }
}

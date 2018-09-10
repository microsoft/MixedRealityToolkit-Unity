// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities
{
    /// <summary>
    /// Helper class for determining if a Windows API contract is available.
    /// <remarks> See https://docs.microsoft.com/en-us/uwp/extension-sdks/windows-universal-sdk
    /// for a full list of contracts.</remarks>
    /// </summary>
    public static class WindowsApiChecker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void CheckApiContracts()
        {
#if !UNITY_EDITOR && UNITY_WSA
            UniversalApiContractV6_IsAvailable = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6);
            UniversalApiContractV5_IsAvailable = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);
            UniversalApiContractV4_IsAvailable = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);
            UniversalApiContractV3_IsAvailable = Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
#else
            UniversalApiContractV6_IsAvailable = false;
            UniversalApiContractV5_IsAvailable = false;
            UniversalApiContractV4_IsAvailable = false;
            UniversalApiContractV3_IsAvailable = false;
#endif
        }

        /// <summary>
        /// Is the Universal API Contract v6.0 Available?
        /// </summary>
        public static bool UniversalApiContractV6_IsAvailable { get; private set; }

        /// <summary>
        /// Is the Universal API Contract v5.0 Available?
        /// </summary>
        public static bool UniversalApiContractV5_IsAvailable { get; private set; }

        /// <summary>
        /// Is the Universal API Contract v4.0 Available?
        /// </summary>
        public static bool UniversalApiContractV4_IsAvailable { get; private set; }

        /// <summary>
        /// Is the Universal API Contract v3.0 Available?
        /// </summary>
        public static bool UniversalApiContractV3_IsAvailable { get; private set; }
    }
}

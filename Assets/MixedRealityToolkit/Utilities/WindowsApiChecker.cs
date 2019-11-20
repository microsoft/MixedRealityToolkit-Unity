// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if WINDOWS_UWP
using Windows.Foundation.Metadata;
#endif

namespace Microsoft.MixedReality.Toolkit.Windows.Utilities
{
    /// <summary>
    /// Helper class for determining if a Windows API contract is available.
    /// </summary>
    /// <remarks> See https://docs.microsoft.com/en-us/uwp/extension-sdks/windows-universal-sdk
    /// for a full list of contracts.</remarks>
    public static class WindowsApiChecker
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void CheckApiContracts()
        {
#if WINDOWS_UWP
            UniversalApiContractV8_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 8);
            UniversalApiContractV7_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7);
            UniversalApiContractV6_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 6);
            UniversalApiContractV5_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 5);
            UniversalApiContractV4_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4);
            UniversalApiContractV3_IsAvailable = ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 3);
#else
            UniversalApiContractV8_IsAvailable = false;
            UniversalApiContractV7_IsAvailable = false;
            UniversalApiContractV6_IsAvailable = false;
            UniversalApiContractV5_IsAvailable = false;
            UniversalApiContractV4_IsAvailable = false;
            UniversalApiContractV3_IsAvailable = false;
#endif
        }
		
        /// <summary>
        /// Is the Universal API Contract v8.0 Available?
        /// </summary>
        public static bool UniversalApiContractV8_IsAvailable { get; private set; }
        
        /// <summary>
        /// Is the Universal API Contract v7.0 Available?
        /// </summary>
        public static bool UniversalApiContractV7_IsAvailable { get; private set; }

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

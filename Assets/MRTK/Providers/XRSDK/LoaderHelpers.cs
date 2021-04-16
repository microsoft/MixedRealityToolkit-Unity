// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    public static class LoaderHelpers
    {
        /// <summary>
        /// Checks if the active loader has a specific name. Used in cases where the loader class is internal, like WindowsMRLoader.
        /// </summary>
        /// <param name="loaderName">The string name to compare against the active loader.</param>
        /// <returns>True if the active loader has the same name as the parameter.</returns>
        public static bool IsLoaderActive(string loaderName) =>
#if XR_MANAGEMENT_ENABLED
            XRGeneralSettings.Instance != null
            && XRGeneralSettings.Instance.Manager != null
            && XRGeneralSettings.Instance.Manager.activeLoader != null
            && XRGeneralSettings.Instance.Manager.activeLoader.name == loaderName;
#else
            false;
#endif

#if XR_MANAGEMENT_ENABLED
        /// <summary>
        /// Checks if the active loader is of a specific type. Used in cases where the loader class is accessible, like OculusLoader.
        /// </summary>
        /// <typeparam name="T">The loader class type to check against the active loader.</typeparam>
        /// <returns>True if the active loader is of the specified type.</returns>
        public static bool IsLoaderActive<T>() where T : XRLoader =>
            XRGeneralSettings.Instance != null
            && XRGeneralSettings.Instance.Manager != null
            && XRGeneralSettings.Instance.Manager.activeLoader is T;
#endif
    }
}

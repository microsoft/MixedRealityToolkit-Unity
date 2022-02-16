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
        /// <returns>True if the active loader has the same name as the parameter. Null if there isn't an active loader.</returns>
        public static bool? IsLoaderActive(string loaderName)
        {
#if XR_MANAGEMENT_ENABLED
            if (XRGeneralSettings.Instance != null
                && XRGeneralSettings.Instance.Manager != null
                && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                return XRGeneralSettings.Instance.Manager.activeLoader.name == loaderName;
            }

            return null;
#else
            return false;
#endif
        }

#if XR_MANAGEMENT_ENABLED
        /// <summary>
        /// Checks if the active loader is of a specific type. Used in cases where the loader class is accessible, like OculusLoader.
        /// </summary>
        /// <typeparam name="T">The loader class type to check against the active loader.</typeparam>
        /// <returns>True if the active loader is of the specified type. Null if there isn't an active loader.</returns>
        public static bool? IsLoaderActive<T>() where T : XRLoader
        {
            if (XRGeneralSettings.Instance != null
                && XRGeneralSettings.Instance.Manager != null
                && XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                return XRGeneralSettings.Instance.Manager.activeLoader is T;
            }

            return null;
        }
#endif
    }
}

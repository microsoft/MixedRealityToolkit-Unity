// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [AddComponentMenu("Scripts/MRTK/Examples/LaunchUri")]
    public class LaunchUri : MonoBehaviour
    {
        /// <summary>
        /// Launch a UWP slate app. In most cases, your experience can continue running while the
        /// launched app renders on top.
        /// </summary>
        /// <param name="uri">Url of the web page or app to launch. See https://docs.microsoft.com/windows/uwp/launch-resume/launch-default-app
        /// for more information about the protocols that can be used when launching apps.</param>
        public void Launch(string uri)
        {
            Debug.Log($"LaunchUri: Launching {uri}");

#if UNITY_WSA
            UnityEngine.WSA.Launcher.LaunchUri(uri, false);
#else
            Application.OpenURL(uri);
#endif
        }
    }
}

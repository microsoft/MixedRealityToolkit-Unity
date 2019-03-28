// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities
{
    /// <summary>
    /// Helper class for enabling/disabling platform specific GameObjects
    /// </summary>
    public class PlatformSpecificGameObject : MonoBehaviour
    {
        /// <summary>
        /// Check to have associated GameObject enabled for HoloLens Platform.
        /// </summary>
        [Tooltip("Check to have associated GameObject enabled for HoloLens Platform.")]
        [SerializeField]
        protected bool _enableOnHoloLens;

        /// <summary>
        /// Check to have associated GameObject enabled for Android Platform.
        /// </summary>
        [Tooltip("Check to have associated GameObject enabled for Android Platform.")]
        [SerializeField]
        protected bool _enableOnAndroid;

        /// <summary>
        /// Check to have associated GameObject enabled for iOS Platform.
        /// </summary>
        [Tooltip("Check to have associated GameObject enabled for iOS Platform.")]
        [SerializeField]
        protected bool _enableOnIos;

        private void OnValidate()
        {
#if UNITY_EDITOR
#if UNITY_WSA
            gameObject.SetActive(_enableOnHoloLens);
#elif UNITY_ANDROID
            gameObject.SetActive(_enableOnAndroid);
#elif UNITY_IOS
            gameObject.SetActive(_enableOnIos);
#endif
#endif
        }

        private void Awake()
        {
#if UNITY_WSA
            gameObject.SetActive(_enableOnHoloLens);
#elif UNITY_ANDROID
            gameObject.SetActive(_enableOnAndroid);
#elif UNITY_IOS
            gameObject.SetActive(_enableOnIos);
#endif
        }
    }
}

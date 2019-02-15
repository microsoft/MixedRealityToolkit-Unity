// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class PlatformSpecificGameObject : MonoBehaviour
    {
        [SerializeField] bool _enableOnHoloLens;
        [SerializeField] bool _enableOnAndroid;
        [SerializeField] bool _enableOnIos;

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

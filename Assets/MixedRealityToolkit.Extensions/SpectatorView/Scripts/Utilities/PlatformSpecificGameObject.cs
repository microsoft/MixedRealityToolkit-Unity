// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Utilities
{
    public class PlatformSpecificGameObject : MonoBehaviour
    {
        [SerializeField] bool _forHoloLens;

        private void OnValidate()
        {
#if UNITY_EDITOR
#if UNITY_WSA
            gameObject.SetActive(_forHoloLens);
#elif UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(!_forHoloLens);
#endif
#endif
        }

        private void Awake()
        {
#if UNITY_WSA
            gameObject.SetActive(_forHoloLens);
#elif UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(!_forHoloLens);
#endif
        }
    }
}

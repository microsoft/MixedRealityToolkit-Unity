// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// Enforces to keep this GameObject alive across different scenes.
    /// </summary>
    public class KeepThisAlive : MonoBehaviour
    {
        public static KeepThisAlive Instance { get; private set; }

        private void Start()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                Instance.gameObject.SetActive(true);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(Instance);
            }
        }
    }
}
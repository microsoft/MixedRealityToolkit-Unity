// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Utility to switch scene state between HoloLens and mobile platforms
    /// </summary>
    public class PlatformSwitcher : MonoBehaviour
    {
        /// <summary>
        /// Available platforms
        /// </summary>
        [Serializable]
        public enum Platform
        {
            Hololens = 0,
            IPhone
        }

        /// <summary>
        /// The current active platform
        /// </summary>
        [SerializeField]
        private Platform targetPlatform;
        /// <summary>
        /// The current active platform
        /// </summary>
        public Platform TargetPlatform
        {
            get
            {
                return targetPlatform;
            }

            set
            {
                targetPlatform = value;
            }
        }

        /// <summary>
        /// Switches scene state between iPhone and HoloLens platforms
        /// </summary>
        /// <param name="platform">The target platform</param>
        public void SwitchPlatform(Platform platform)
        {
#if UNITY_EDITOR
            TargetPlatform = platform;

            string platformGameObjectName = "";

            switch(platform)
            {
                case Platform.Hololens:
                    platformGameObjectName = "Hololens";
                    break;

                case Platform.IPhone:
                    platformGameObjectName = "IPhone";
                    break;
            }

            // Disables platform root objects for inactive platforms
            for(int i=0; i<transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if(child.gameObject.name == "Shared")
                {
                    continue;
                }

                if (child.gameObject.name == "WorldSync")
                {
                    continue;
                }

                child.gameObject.SetActive(child.name == platformGameObjectName);
            }
#endif
        }
    }
}

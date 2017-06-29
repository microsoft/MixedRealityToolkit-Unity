// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    public class SceneLauncherChecker : MonoBehaviour
    {
        [SerializeField]
        private GameObject sceneLauncherPrefab;

        private void Start()
        {
            if (FindObjectOfType<SceneLauncher>() == null)
            {
                Instantiate(sceneLauncherPrefab);
            }
        }
    }
}

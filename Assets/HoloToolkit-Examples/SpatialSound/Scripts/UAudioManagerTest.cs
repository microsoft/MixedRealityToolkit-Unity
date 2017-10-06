// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.Tests
{
    public class UAudioManagerTest : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(ContinouslyPlaySounds());
        }

        private IEnumerator ContinouslyPlaySounds()
        {
            while (true)
            {
                UAudioManager.Instance.PlayEvent("Laser");

                yield return new WaitForSeconds(1.0f);

                UAudioManager.Instance.PlayEvent("Vocals");

                yield return new WaitForSeconds(10.0f);
            }
        }

    }
}
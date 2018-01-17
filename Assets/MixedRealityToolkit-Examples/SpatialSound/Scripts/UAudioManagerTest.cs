// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.SpatialSound;
using MixedRealityToolkit.SpatialSound.Attributes;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.Examples.SpatialSound
{
    public class UAudioManagerTest : MonoBehaviour
    {
        [AudioEvent]
        public string Vocals3d;
        [AudioEvent]
        public string VocalsSpatialized;
        [AudioEvent]
        public string Laser;

        private void Start()
        {
            StartCoroutine(ContinouslyPlaySounds());
        }

        private IEnumerator ContinouslyPlaySounds()
        {
            while (true)
            {
                UAudioManager.Instance.PlayEvent(Vocals3d);

                yield return new WaitForSeconds(10.0f);

                UAudioManager.Instance.PlayEvent(VocalsSpatialized);

                yield return new WaitForSeconds(10.0f);

                UAudioManager.Instance.PlayEvent(Laser);

                yield return new WaitForSeconds(1.0f);
            }
        }

    }
}
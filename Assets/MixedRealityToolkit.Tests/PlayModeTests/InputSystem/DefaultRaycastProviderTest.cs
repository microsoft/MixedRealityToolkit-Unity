// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class DefaultRaycastProviderTest
    {
        private DefaultRaycastProvider defaultRaycastProvider;

        /// <summary>
        /// Validates that when nothing is hit, the default raycast provider doesn't throw an
        /// exception and that the MixedRealityRaycastHit is null.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNoHit()
        {
            // step and layerMasks are set arbitrarily (to something which will not generate a hit).
            RayStep step = new RayStep(Vector3.zero, Vector3.up);
            LayerMask[] layerMasks = new LayerMask[] { UnityEngine.Physics.DefaultRaycastLayers };
            MixedRealityRaycastHit hitInfo;
            Assert.IsFalse(defaultRaycastProvider.Raycast(step, layerMasks, false, out hitInfo));
            Assert.IsFalse(hitInfo.raycastValid);
            Assert.IsFalse(hitInfo.collider != null);
            yield return null;
        }

        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();
            defaultRaycastProvider = new DefaultRaycastProvider(null);
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }
    }
}
#endif
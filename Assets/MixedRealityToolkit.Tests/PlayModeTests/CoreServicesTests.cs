// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class CoreServicesTests
    {
        /// <summary>
        /// Test if we can register and deregister a core MRTK service
        /// </summary>
        /// <returns>enumerator for Unity</returns>
        [UnityTest]
        public IEnumerator TestDynamicServices()
        {
            UnityEngine.Object cameraSystemPrefab = AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit.SDK/Experimental/Features/Camera/Prefabs/CameraSystem.prefab", typeof(UnityEngine.Object));
            Assert.IsNull(CoreServices.CameraSystem);

            GameObject cameraSystem1 = UnityEngine.Object.Instantiate(cameraSystemPrefab) as GameObject;
            Assert.IsNotNull(CoreServices.CameraSystem);

            // Destroying the prefab will cause the CameraSystemManager to unregister the Camera System
            UnityEngine.Object.DestroyImmediate(cameraSystem1);

            Assert.IsTrue(CoreServices.ResetCacheReference(typeof(IMixedRealityCameraSystem)));

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            yield return new WaitForSeconds(1.0f);

            Assert.IsNull(CoreServices.CameraSystem);

            GameObject cameraSystem2 = UnityEngine.Object.Instantiate(cameraSystemPrefab) as GameObject;
            Assert.IsNotNull(CoreServices.CameraSystem);
        }
    }
}
#endif

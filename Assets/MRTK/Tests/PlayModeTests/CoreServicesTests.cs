// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        // SDK/Experimental/ServiceManagers/Camera/Prefabs/CameraSystem.prefab
        private const string CameraSystemPrefabGuid = "8a5d96f565aaee14ca3e07575bb9ce80";
        private static readonly string CameraSystemPrefabPath = AssetDatabase.GUIDToAssetPath(CameraSystemPrefabGuid);

        /// <summary>
        /// Test if we can register and unregister a core MRTK service
        /// </summary>
        /// <returns>enumerator for Unity</returns>
        [UnityTest]
        public IEnumerator TestDynamicServices()
        {
            TestUtilities.InitializeCamera();

            UnityEngine.Object cameraSystemPrefab = AssetDatabase.LoadAssetAtPath(CameraSystemPrefabPath, typeof(UnityEngine.Object));
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

            UnityEngine.Object.Instantiate(cameraSystemPrefab);
            Assert.IsNotNull(CoreServices.CameraSystem);
        }
    }
}
#endif

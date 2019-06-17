// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using System;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class BoundingBoxTests
    {
        #region Utilities
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private GameObject InstantiateSceneAndDefaultBbox()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            RenderSettings.skybox = null;

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Vector3.forward * -1.5f;
            cube.AddComponent<BoundingBox>();

            return cube;
        }
        #endregion

        [UnityTest]
        public IEnumerator BBoxInstantiate()
        {
            GameObject bbox = InstantiateSceneAndDefaultBbox();
            yield return null;
            BoundingBox bboxComponent = bbox.GetComponent<BoundingBox>();
            Assert.IsNotNull(bboxComponent);

            GameObject.Destroy(bbox);
            // Wait for a frame to give Unity a change to actually destroy the object
            yield return null;
        }
        
    }
}
#endif
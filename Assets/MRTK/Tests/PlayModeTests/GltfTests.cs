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

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class GltfTests
    {
        private const string AvocadoCustomAttrGuid = "fea29429b97dbb14b97820f56c74060a";
        private AsyncCoroutineRunner asyncCoroutineRunner;
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
            asyncCoroutineRunner = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunner>();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
            GameObject.Destroy(asyncCoroutineRunner.gameObject);
        }
        
        private IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted) { yield return null; }
            if (task.IsFaulted) { throw task.Exception; }
            yield return null;
        }

        #region Tests
        /// <summary>
        /// Performs basic check that a glTF loads and contains data
        /// </summary>
        [UnityTest]
        public IEnumerator TestGltfLoads()
        {
            // Load glTF
            string path = AssetDatabase.GUIDToAssetPath(AvocadoCustomAttrGuid);
            var task = GltfUtility.ImportGltfObjectFromPathAsync(path);

            yield return WaitForTask(task);

            GltfObject gltfObject = task.Result;

            yield return null;

            Assert.IsNotNull(gltfObject);
            Assert.AreEqual(1, gltfObject.meshes.Length);
            Assert.AreEqual(1, gltfObject.nodes.Length);

            // Check if mesh variables have been set by attributes
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.uv.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.normals.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.tangents.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.vertexCount);
        }

        /// <summary>
        /// Tests that custom glTF attributes are parsed and accessible
        /// </summary>
        [UnityTest]
        public IEnumerator TestGltfCustomAttributes()
        {
            // Load glTF
            string path = AssetDatabase.GUIDToAssetPath(AvocadoCustomAttrGuid);
            var task = GltfUtility.ImportGltfObjectFromPathAsync(path);

            yield return WaitForTask(task);

            GltfObject gltfObject = task.Result;

            yield return null;

            // Check for custom attribute
            int temperatureIdx;
            gltfObject.meshes[0].primitives[0].Attributes.TryGetValue("_TEMPERATURE", out temperatureIdx);

            int temperature = gltfObject.accessors[temperatureIdx].count;
            Assert.AreEqual(100, temperature);
        }
        #endregion
        
    }
}
#endif
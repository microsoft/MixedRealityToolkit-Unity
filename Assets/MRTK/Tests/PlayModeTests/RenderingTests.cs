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

using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Unit tests to check the functionality of a handful of rendering systems.
    /// </summary>
    public class RenderingTests : BasePlayModeTests
    {
        private const float VisualizatonWaitTime = 1.0f;
        private readonly Vector3 LightDirection = new Vector3(-0.5f, -0.5f, 0.0f);

        /// <summary>
        /// Tests if <see cref="Microsoft.MixedReality.Toolkit.Rendering.MaterialInstance"/> can be instantiated and manipulated at runtime.
        /// </summary>
        /// <returns>Enumerator for Unity</returns>
        [UnityTest]
        public IEnumerator TestMaterialInstance()
        {
            // Light only used for developers to visually inspect test.
            var light = InstantiateDirectionalLight(LightDirection);

            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var redMaterial = InstantiateStandardShaderMaterial(Color.red);
            gameObject.GetComponent<Renderer>().sharedMaterial = redMaterial;

            // Test adding a MaterialInstance.
            var materialInstance = gameObject.AddComponent<MaterialInstance>();

            // Test altering properties.
            materialInstance.Material.color = Color.green;
            materialInstance.Material.mainTexture = Texture2D.blackTexture;

            // Test multiple materials and new material tracking.
            var blueMaterial = InstantiateStandardShaderMaterial(Color.blue);
            gameObject.GetComponent<Renderer>().sharedMaterials = new Material[] { redMaterial, blueMaterial };

            yield return null;

            Assert.AreEqual(2, materialInstance.Materials.Length, "The MaterialInstance should contain two materials.");

            // Test material ownership.
            materialInstance.AcquireMaterial(gameObject);
            materialInstance.ReleaseMaterial(gameObject);

            yield return new WaitForSeconds(VisualizatonWaitTime);

            Assert.IsNull(gameObject.GetComponent<MaterialInstance>(), "The MaterialInstance should be null due to ReleaseMaterial with auto destroy being invoked.");

            Object.Destroy(blueMaterial);
            Object.Destroy(redMaterial);
            Object.Destroy(gameObject);
            Object.Destroy(light);
        }

        #region Test Helpers

        private static Light InstantiateDirectionalLight(Vector3 direction)
        {
            var light = new GameObject().AddComponent<Light>();
            light.type = LightType.Directional;
            light.transform.LookAt(direction);

            return light;
        }

        private static Material InstantiateStandardShaderMaterial(Color? color = null)
        {
            var material = new Material(StandardShaderUtility.MrtkStandardShader);
            material.color = color ?? Color.white;

            return material;
        }

        #endregion
    }
}

#endif // !WINDOWS_UWP

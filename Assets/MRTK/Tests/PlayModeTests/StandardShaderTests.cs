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

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class StandardShaderTests : BasePlayModeTests
    {
        private const float VisualizatonWaitTime = 1.0f;
        private readonly Vector3 LightDirection = new Vector3(-0.5f, -0.5f, 0.0f);

        /// <summary>
        /// Tests if the MeshOutline component can be added and manipulated at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMeshOutline()
        {
            var light = InstantiateDirectionalLight(LightDirection);
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var outlineColor = Color.red;
            var meshOutline = gameObject.AddComponent<MeshOutline>();
            meshOutline.OutlineMaterial = InstantiateStandardShaderOutlineMaterial(outlineColor);
            meshOutline.OutlineWidth = 0.1f;

            yield return new WaitForSeconds(VisualizatonWaitTime);

            Assert.True(ColorExistsInFrame(outlineColor, 16, 16), "Outline color could not be found in frame.");

            Object.Destroy(meshOutline.OutlineMaterial);
            Object.Destroy(gameObject);
            Object.Destroy(light);
        }

        /// <summary>
        /// Tests if the MeshOutlineHierarchy component can be added and manipulated at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMeshOutlineHierarchy()
        {
            var light = InstantiateDirectionalLight(LightDirection);
            var gameObject = new GameObject();

            for (int i = 0; i < 3; ++i)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = Vector3.up * 0.5f * (i - 1);
                sphere.transform.localScale = Vector3.one * 0.2f;
                sphere.transform.parent = gameObject.transform;
            }

            var outlineColor = Color.blue;
            var meshOutlineHierarchy = gameObject.AddComponent<MeshOutlineHierarchy>();
            meshOutlineHierarchy.OutlineMaterial = InstantiateStandardShaderOutlineMaterial(outlineColor);
            meshOutlineHierarchy.OutlineWidth = 0.1f;

            yield return new WaitForSeconds(VisualizatonWaitTime);

            Assert.True(ColorExistsInFrame(outlineColor, 16, 16), "Outline color could not be found in frame.");

            Object.Destroy(meshOutlineHierarchy.OutlineMaterial);
            Object.Destroy(gameObject);
            Object.Destroy(light);
        }

        /// <summary>
        /// Tests if the MeshSmoother component can be added an manipulated at runtime.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMeshSmoother()
        {
            var light = InstantiateDirectionalLight(LightDirection);
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            var meshSmoother = gameObject.AddComponent<MeshSmoother>();
            meshSmoother.SmoothNormalsAsync();
            meshSmoother.SmoothNormals();

            yield return new WaitForSeconds(VisualizatonWaitTime);

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

        private static Material InstantiateStandardShaderOutlineMaterial(Color? color = null)
        {
            var material = InstantiateStandardShaderMaterial(color);

            material.SetFloat("_ZWrite", 0);
            material.EnableKeyword("_VERTEX_EXTRUSION");
            material.EnableKeyword("_VERTEX_EXTRUSION_SMOOTH_NORMALS");
            material.DisableKeyword("_DIRECTIONAL_LIGHT");
            material.DisableKeyword("_REFLECTIONS");

            return material;
        }

        private static Texture2D CaptureFrame(Camera camera, int width, int height, int depth = 24, RenderTextureFormat format = RenderTextureFormat.ARGB32)
        {
            var renderTexture = RenderTexture.GetTemporary(width, height, depth, format);

            // Apply the render texture and render a frame.
            var previousTexture = camera.targetTexture;
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = previousTexture;

            // Blit the render from into a texture.
            var outputTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            RenderTexture previousRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            outputTexture.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
            outputTexture.Apply();
            RenderTexture.active = previousRenderTexture;
            RenderTexture.ReleaseTemporary(renderTexture);

            return outputTexture;
        }

        private bool ColorExistsInFrame(Color color, int width, int height)
        {
            var frameCapture = CaptureFrame(CameraCache.Main, width, height);

            Assert.NotNull(frameCapture, "Frame capture is null.");

            var pixels = frameCapture.GetPixels();

            Assert.NotZero(pixels.Length, "Frame capture is empty.");

            foreach (Color pixel in pixels)
            {
                if (pixel == color)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}

#endif // !WINDOWS_UWP

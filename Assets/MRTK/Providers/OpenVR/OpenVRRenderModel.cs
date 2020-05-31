// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Render model of associated tracked object
//
//=============================================================================

using Microsoft.MixedReality.Toolkit.OpenVR.Headers;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    /// <summary>
    /// Represents and loads models from the OpenVR APIs. This class is based on the SteamVR_RenderModel class.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Providers/OpenVRRenderModel")]
    public class OpenVRRenderModel : MonoBehaviour
    {
        private class RenderModel
        {
            public RenderModel(Mesh mesh, Material material)
            {
                Mesh = mesh;
                Material = material;
            }
            public Mesh Mesh { get; private set; }
            public Material Material { get; private set; }
        }

        private static readonly Hashtable models = new Hashtable();
        private static readonly Hashtable materials = new Hashtable();

        private string renderModelName = string.Empty;

        internal Shader shader = null;

        /// <summary>
        /// Attempts to load or reload a controller model based on the passed in handedness.
        /// </summary>
        /// <param name="handedness">The handedness of the controller model to load.</param>
        /// <returns>True if the controller model was found and loaded. False otherwise.</returns>
        public bool LoadModel(Handedness handedness)
        {
            var system = Headers.OpenVR.System;
            if (system == null)
            {
                return false;
            }

            var error = ETrackedPropertyError.TrackedProp_Success;
            uint index = system.GetTrackedDeviceIndexForControllerRole(handedness == Handedness.Left ? ETrackedControllerRole.LeftHand : ETrackedControllerRole.RightHand);
            var capacity = system.GetStringTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_RenderModelName_String, null, 0, ref error);
            if (capacity <= 1)
            {
                Debug.LogError("Failed to get render model name for tracked object " + index);
                return false;
            }

            var buffer = new System.Text.StringBuilder((int)capacity);
            system.GetStringTrackedDeviceProperty(index, ETrackedDeviceProperty.Prop_RenderModelName_String, buffer, capacity, ref error);

            var s = buffer.ToString();
            if (renderModelName != s)
            {
                StartCoroutine(SetModelAsync(s));
            }

            return true;
        }

        private IEnumerator SetModelAsync(string newRenderModelName)
        {
            if (string.IsNullOrEmpty(newRenderModelName))
            {
                yield break;
            }

            // Pre-load all render models before asking for the data to create meshes.
            CVRRenderModels renderModels = Headers.OpenVR.RenderModels;
            if (renderModels == null)
            {
                yield break;
            }

            // Gather names of render models to pre-load.
            string[] renderModelNames;

            uint count = renderModels.GetComponentCount(newRenderModelName);
            if (count > 0)
            {
                renderModelNames = new string[count];

                for (int componentIndex = 0; componentIndex < count; componentIndex++)
                {
                    uint capacity = renderModels.GetComponentName(newRenderModelName, (uint)componentIndex, null, 0);
                    if (capacity == 0)
                    {
                        continue;
                    }

                    var componentNameStringBuilder = new System.Text.StringBuilder((int)capacity);
                    if (renderModels.GetComponentName(newRenderModelName, (uint)componentIndex, componentNameStringBuilder, capacity) == 0)
                    {
                        continue;
                    }

                    string componentName = componentNameStringBuilder.ToString();

                    capacity = renderModels.GetComponentRenderModelName(newRenderModelName, componentName, null, 0);
                    if (capacity == 0)
                    {
                        continue;
                    }

                    var nameStringBuilder = new System.Text.StringBuilder((int)capacity);
                    if (renderModels.GetComponentRenderModelName(newRenderModelName, componentName, nameStringBuilder, capacity) == 0)
                    {
                        continue;
                    }

                    var s = nameStringBuilder.ToString();

                    // Only need to pre-load if not already cached.
                    if (!(models[s] is RenderModel model) || model.Mesh == null)
                    {
                        renderModelNames[componentIndex] = s;
                    }
                }
            }
            else
            {
                // Only need to pre-load if not already cached.
                if (!(models[newRenderModelName] is RenderModel model) || model.Mesh == null)
                {
                    renderModelNames = new string[] { newRenderModelName };
                }
                else
                {
                    renderModelNames = System.Array.Empty<string>();
                }
            }

            // Keep trying every 100ms until all components finish loading.
            while (true)
            {
                var loading = false;
                for (int renderModelNameIndex = 0; renderModelNameIndex < renderModelNames.Length; renderModelNameIndex++)
                {
                    if (string.IsNullOrEmpty(renderModelNames[renderModelNameIndex]))
                    {
                        continue;
                    }

                    var pRenderModel = System.IntPtr.Zero;

                    var error = renderModels.LoadRenderModel_Async(renderModelNames[renderModelNameIndex], ref pRenderModel);

                    if (error == EVRRenderModelError.Loading)
                    {
                        loading = true;
                    }
                    else if (error == EVRRenderModelError.None)
                    {
                        // Pre-load textures as well.
                        var renderModel = MarshalRenderModel(pRenderModel);

                        // Check the cache first.
                        var material = materials[renderModel.diffuseTextureId] as Material;
                        if (material == null || material.mainTexture == null)
                        {
                            var pDiffuseTexture = System.IntPtr.Zero;

                            error = renderModels.LoadTexture_Async(renderModel.diffuseTextureId, ref pDiffuseTexture);

                            if (error == EVRRenderModelError.Loading)
                            {
                                loading = true;
                            }
                        }
                    }
                }

                if (loading)
                {
                    yield return new WaitForSecondsRealtime(0.1f);
                }
                else
                {
                    break;
                }
            }

            SetModel(newRenderModelName);
            renderModelName = newRenderModelName;
        }

        private bool SetModel(string renderModelName)
        {
            StripMesh(gameObject);

            if (!string.IsNullOrEmpty(renderModelName))
            {
                var model = models[renderModelName] as RenderModel;
                if (model == null || model.Mesh == null)
                {
                    var renderModels = Headers.OpenVR.RenderModels;
                    if (renderModels == null)
                    {
                        return false;
                    }

                    model = LoadRenderModel(renderModels, renderModelName, renderModelName);
                    if (model == null)
                    {
                        return false;
                    }

                    models[renderModelName] = model;
                }

                gameObject.AddComponent<MeshFilter>().mesh = model.Mesh;
                MeshRenderer newRenderer = gameObject.AddComponent<MeshRenderer>();
                newRenderer.sharedMaterial = model.Material;
                return true;
            }

            return false;
        }

        private RenderModel LoadRenderModel(CVRRenderModels renderModels, string renderModelName, string baseName)
        {
            var pRenderModel = System.IntPtr.Zero;

            EVRRenderModelError error;
            while (true)
            {
                error = renderModels.LoadRenderModel_Async(renderModelName, ref pRenderModel);
                if (error != EVRRenderModelError.Loading)
                    break;

                Sleep();
            }

            if (error != EVRRenderModelError.None)
            {
                Debug.LogError(string.Format("Failed to load render model {0} - {1}", renderModelName, error.ToString()));
                return null;
            }

            var renderModel = MarshalRenderModel(pRenderModel);

            var vertices = new Vector3[renderModel.unVertexCount];
            var normals = new Vector3[renderModel.unVertexCount];
            var uv = new Vector2[renderModel.unVertexCount];

            var type = typeof(RenderModel_Vertex_t);
            for (int iVert = 0; iVert < renderModel.unVertexCount; iVert++)
            {
                var ptr = new System.IntPtr(renderModel.rVertexData.ToInt64() + iVert * Marshal.SizeOf(type));
                var vert = (RenderModel_Vertex_t)Marshal.PtrToStructure(ptr, type);

                vertices[iVert] = new Vector3(vert.vPosition.v0, vert.vPosition.v1, -vert.vPosition.v2);
                normals[iVert] = new Vector3(vert.vNormal.v0, vert.vNormal.v1, -vert.vNormal.v2);
                uv[iVert] = new Vector2(vert.rfTextureCoord0, vert.rfTextureCoord1);
            }

            int indexCount = (int)renderModel.unTriangleCount * 3;
            var indices = new short[indexCount];
            Marshal.Copy(renderModel.rIndexData, indices, 0, indices.Length);

            var triangles = new int[indexCount];
            for (int iTri = 0; iTri < renderModel.unTriangleCount; iTri++)
            {
                triangles[iTri * 3 + 0] = (int)indices[iTri * 3 + 2];
                triangles[iTri * 3 + 1] = (int)indices[iTri * 3 + 1];
                triangles[iTri * 3 + 2] = (int)indices[iTri * 3 + 0];
            }

            var mesh = new Mesh
            {
                vertices = vertices,
                normals = normals,
                uv = uv,
                triangles = triangles
            };

            // Check cache before loading texture.
            var material = materials[renderModel.diffuseTextureId] as Material;
            if (material == null || material.mainTexture == null)
            {
                var pDiffuseTexture = System.IntPtr.Zero;

                while (true)
                {
                    error = renderModels.LoadTexture_Async(renderModel.diffuseTextureId, ref pDiffuseTexture);
                    if (error != EVRRenderModelError.Loading)
                    {
                        break;
                    }

                    Sleep();
                }

                if (error == EVRRenderModelError.None)
                {
                    var diffuseTexture = MarshalRenderModel_TextureMap(pDiffuseTexture);
                    var texture = new Texture2D(diffuseTexture.unWidth, diffuseTexture.unHeight, TextureFormat.RGBA32, false);
                    if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11)
                    {
                        texture.Apply();
                        System.IntPtr texturePointer = texture.GetNativeTexturePtr();
                        while (true)
                        {
                            error = renderModels.LoadIntoTextureD3D11_Async(renderModel.diffuseTextureId, texturePointer);
                            if (error != EVRRenderModelError.Loading)
                            {
                                break;
                            }

                            Sleep();
                        }
                    }
                    else
                    {
                        var textureMapData = new byte[diffuseTexture.unWidth * diffuseTexture.unHeight * 4]; // RGBA
                        Marshal.Copy(diffuseTexture.rubTextureMapData, textureMapData, 0, textureMapData.Length);

                        var colors = new Color32[diffuseTexture.unWidth * diffuseTexture.unHeight];
                        int iColor = 0;
                        for (int iHeight = 0; iHeight < diffuseTexture.unHeight; iHeight++)
                        {
                            for (int iWidth = 0; iWidth < diffuseTexture.unWidth; iWidth++)
                            {
                                var r = textureMapData[iColor++];
                                var g = textureMapData[iColor++];
                                var b = textureMapData[iColor++];
                                var a = textureMapData[iColor++];
                                colors[iHeight * diffuseTexture.unWidth + iWidth] = new Color32(r, g, b, a);
                            }
                        }

                        texture.SetPixels32(colors);
                        texture.Apply();
                    }

                    material = new Material(shader != null ? shader : Shader.Find("Mixed Reality Toolkit/Standard"))
                    {
                        mainTexture = texture
                    };

                    materials[renderModel.diffuseTextureId] = material;

                    renderModels.FreeTexture(pDiffuseTexture);
                }
                else
                {
                    Debug.Log("Failed to load render model texture for render model " + renderModelName + ". Error: " + error.ToString());
                }
            }

            // Delay freeing when we can since we'll often get multiple requests for the same model right
            // after another (e.g. two controllers or two base stations).
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                renderModels.FreeRenderModel(pRenderModel);
            }
            else
#endif
            {
                StartCoroutine(FreeRenderModel(pRenderModel));
            }

            return new RenderModel(mesh, material);
        }

        private IEnumerator FreeRenderModel(System.IntPtr pRenderModel)
        {
            yield return new WaitForSeconds(1.0f);
            Headers.OpenVR.RenderModels.FreeRenderModel(pRenderModel);
        }

        private void StripMesh(GameObject go)
        {
            var meshRenderer = go.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                DestroyImmediate(meshRenderer);
            }

            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                DestroyImmediate(meshFilter);
            }
        }

        private static void Sleep()
        {
#if !UNITY_WSA
            System.Threading.Thread.Sleep(1);
#endif
        }

        private RenderModel_t MarshalRenderModel(System.IntPtr pRenderModel)
        {
#if !ENABLE_DOTNET
            if ((System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) ||
                (System.Environment.OSVersion.Platform == System.PlatformID.Unix))
            {
                var packedModel = (RenderModel_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t_Packed));
                RenderModel_t model = new RenderModel_t();
                packedModel.Unpack(ref model);
                return model;
            }
            else
#endif
            {
                return (RenderModel_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_t));
            }
        }

        private RenderModel_TextureMap_t MarshalRenderModel_TextureMap(System.IntPtr pRenderModel)
        {
#if !ENABLE_DOTNET
            if ((System.Environment.OSVersion.Platform == System.PlatformID.MacOSX) ||
                (System.Environment.OSVersion.Platform == System.PlatformID.Unix))
            {
                var packedModel = (RenderModel_TextureMap_t_Packed)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t_Packed));
                RenderModel_TextureMap_t model = new RenderModel_TextureMap_t();
                packedModel.Unpack(ref model);
                return model;
            }
            else
#endif
            {
                return (RenderModel_TextureMap_t)Marshal.PtrToStructure(pRenderModel, typeof(RenderModel_TextureMap_t));
            }
        }
    }
}

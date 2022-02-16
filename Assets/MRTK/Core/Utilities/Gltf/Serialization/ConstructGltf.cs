// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Storage.Streams;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization
{
    public static class ConstructGltf
    {
        private static readonly WaitForUpdate Update = new WaitForUpdate();
        private static readonly WaitForBackgroundThread BackgroundThread = new WaitForBackgroundThread();
        private static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");
        private static readonly int ModeId = Shader.PropertyToID("_Mode");
        private static readonly int EmissionMapId = Shader.PropertyToID("_EmissionMap");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        private static readonly int MetallicGlossMapId = Shader.PropertyToID("_MetallicGlossMap");
        private static readonly int GlossinessId = Shader.PropertyToID("_Glossiness");
        private static readonly int MetallicId = Shader.PropertyToID("_Metallic");
        private static readonly int BumpMapId = Shader.PropertyToID("_BumpMap");
        private static readonly int EmissiveColorId = Shader.PropertyToID("_EmissiveColor");
        private static readonly int ChannelMapId = Shader.PropertyToID("_ChannelMap");
        private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
        private static readonly int NormalMapId = Shader.PropertyToID("_NormalMap");
        private static readonly int NormalMapScaleId = Shader.PropertyToID("_NormalMapScale");
        private static readonly int CullModeId = Shader.PropertyToID("_CullMode");

        /// <summary>
        /// Constructs the glTF Object.
        /// </summary>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> of the final constructed <see cref="Schema.GltfScene"/></returns>
        public static async void Construct(this GltfObject gltfObject)
        {
            await gltfObject.ConstructAsync();
        }

        /// <summary>
        /// Constructs the glTF Object.
        /// </summary>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> of the final constructed <see cref="Schema.GltfScene"/></returns>
        public static async Task<GameObject> ConstructAsync(this GltfObject gltfObject)
        {
            if (!gltfObject.asset.version.Contains("2.0"))
            {
                Debug.LogWarning($"Expected glTF 2.0, but this asset is using {gltfObject.asset.version}");
                return null;
            }

            if (gltfObject.UseBackgroundThread) { await Update; }

            var rootObject = new GameObject($"glTF Scene {gltfObject.Name}");
            rootObject.SetActive(false);

            if (gltfObject.UseBackgroundThread) await BackgroundThread;

            for (int i = 0; i < gltfObject.bufferViews?.Length; i++)
            {
                gltfObject.ConstructBufferView(gltfObject.bufferViews[i]);
            }

            for (int i = 0; i < gltfObject.textures?.Length; i++)
            {
                await gltfObject.ConstructTextureAsync(gltfObject.textures[i]);
            }

            for (int i = 0; i < gltfObject.materials?.Length; i++)
            {
                await gltfObject.ConstructMaterialAsync(gltfObject.materials[i], i);
            }

            if (gltfObject.scenes == null)
            {
                Debug.LogError($"No scenes found for {gltfObject.Name}");
            }

            if (gltfObject.UseBackgroundThread) await Update;

            for (int i = 0; i < gltfObject.scenes?.Length; i++)
            {
                await gltfObject.ConstructSceneAsync(gltfObject.scenes[i], rootObject);
            }

            rootObject.SetActive(true);
            return gltfObject.GameObjectReference = rootObject;
        }

        private static void ConstructBufferView(this GltfObject gltfObject, GltfBufferView bufferView)
        {
            bufferView.Buffer = gltfObject.buffers[bufferView.buffer];

            if (bufferView.Buffer.BufferData == null &&
                !string.IsNullOrEmpty(gltfObject.Uri) &&
                !string.IsNullOrEmpty(bufferView.Buffer.uri))
            {
                var parentDirectory = Directory.GetParent(gltfObject.Uri).FullName;
                bufferView.Buffer.BufferData = File.ReadAllBytes(Path.Combine(parentDirectory, bufferView.Buffer.uri));
            }
        }

        private static async Task ConstructTextureAsync(this GltfObject gltfObject, GltfTexture gltfTexture)
        {
            if (gltfObject.UseBackgroundThread) await BackgroundThread;

            if (gltfTexture.source >= 0)
            {
                GltfImage gltfImage = gltfObject.images[gltfTexture.source];

                byte[] imageData = null;
                Texture2D texture = null;

                if (!string.IsNullOrEmpty(gltfObject.Uri) && !string.IsNullOrEmpty(gltfImage.uri))
                {
                    var parentDirectory = Directory.GetParent(gltfObject.Uri).FullName;
                    var path = Path.Combine(parentDirectory, gltfImage.uri);

#if UNITY_EDITOR
                    if (gltfObject.UseBackgroundThread) await Update;
                    var projectPath = Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "Assets");
                    texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(projectPath);

                    if (gltfObject.UseBackgroundThread) await BackgroundThread;
#endif

                    if (texture == null)
                    {
#if WINDOWS_UWP
                        if (gltfObject.UseBackgroundThread)
                        {
                            try
                            {
                                var storageFile = await StorageFile.GetFileFromPathAsync(path);

                                if (storageFile != null)
                                {

                                    var buffer = await FileIO.ReadBufferAsync(storageFile);

                                    using (DataReader dataReader = DataReader.FromBuffer(buffer))
                                    {
                                        imageData = new byte[buffer.Length];
                                        dataReader.ReadBytes(imageData);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e.Message);
                            }
                        }
                        else
                        {
                            imageData = UnityEngine.Windows.File.ReadAllBytes(path);
                        }
#else
                        using (FileStream stream = File.Open(path, FileMode.Open))
                        {
                            imageData = new byte[stream.Length];

                            if (gltfObject.UseBackgroundThread)
                            {
                                await stream.ReadAsync(imageData, 0, (int)stream.Length);
                            }
                            else
                            {
                                stream.Read(imageData, 0, (int)stream.Length);
                            }
                        }
#endif
                    }
                }
                else
                {
                    var imageBufferView = gltfObject.bufferViews[gltfImage.bufferView];
                    imageData = new byte[imageBufferView.byteLength];
                    Array.Copy(imageBufferView.Buffer.BufferData, imageBufferView.byteOffset, imageData, 0, imageData.Length);
                }

                if (texture == null)
                {
                    if (gltfObject.UseBackgroundThread) await Update;
                    // TODO Load texture async
                    texture = new Texture2D(2, 2);
                    gltfImage.Texture = texture;
                    gltfImage.Texture.LoadImage(imageData);
                }
                else
                {
                    gltfImage.Texture = texture;
                }

                gltfTexture.Texture = texture;

                if (gltfObject.UseBackgroundThread) await BackgroundThread;
            }
        }

        private static async Task ConstructMaterialAsync(this GltfObject gltfObject, GltfMaterial gltfMaterial, int materialId)
        {
            if (gltfObject.UseBackgroundThread) await Update;

            Material material = await CreateMRTKShaderMaterial(gltfObject, gltfMaterial, materialId);
            if (material == null)
            {
                Debug.LogWarning("The Mixed Reality Toolkit/Standard Shader was not found. Falling back to Standard Shader");
                material = await CreateStandardShaderMaterial(gltfObject, gltfMaterial, materialId);
            }

            if (material == null)
            {
                Debug.LogWarning("The Standard Shader was not found. Failed to create material for glTF object");
            }
            else
            {
                gltfMaterial.Material = material;
            }

            if (gltfObject.UseBackgroundThread) await BackgroundThread;
        }

        private static async Task<Material> CreateMRTKShaderMaterial(GltfObject gltfObject, GltfMaterial gltfMaterial, int materialId)
        {
            var shader = Shader.Find("Mixed Reality Toolkit/Standard");

            if (shader == null) { return null; }

            var material = new Material(shader)
            {
                name = string.IsNullOrEmpty(gltfMaterial.name) ? $"glTF Material {materialId}" : gltfMaterial.name
            };

            if (gltfMaterial.pbrMetallicRoughness.baseColorTexture?.index >= 0)
            {
                material.mainTexture = gltfObject.images[gltfMaterial.pbrMetallicRoughness.baseColorTexture.index].Texture;
            }

            material.color = gltfMaterial.pbrMetallicRoughness.baseColorFactor.GetColorValue();

            if (gltfMaterial.alphaMode == "MASK")
            {
                material.SetInt(SrcBlendId, (int)BlendMode.One);
                material.SetInt(DstBlendId, (int)BlendMode.Zero);
                material.SetInt(ZWriteId, 1);
                material.SetInt(ModeId, 3);
                material.SetOverrideTag("RenderType", "Cutout");
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
            }
            else if (gltfMaterial.alphaMode == "BLEND")
            {
                material.SetInt(SrcBlendId, (int)BlendMode.One);
                material.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt(ZWriteId, 0);
                material.SetInt(ModeId, 3);
                material.SetOverrideTag("RenderType", "Transparency");
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            if (gltfMaterial.emissiveTexture?.index >= 0 && material.HasProperty("_EmissionMap"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor(EmissiveColorId, gltfMaterial.emissiveFactor.GetColorValue());
            }

            if (gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture?.index >= 0)
            {
                var texture = gltfObject.images[gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index].Texture;

                Texture2D occlusionTexture = null;
                if (gltfMaterial.occlusionTexture.index >= 0)
                {
                    occlusionTexture = gltfObject.images[gltfMaterial.occlusionTexture.index].Texture;
                }

                if (texture.isReadable)
                {
                    var pixels = texture.GetPixels();
                    Color[] occlusionPixels = null;
                    if (occlusionTexture != null &&
                        occlusionTexture.isReadable)
                    {
                        occlusionPixels = occlusionTexture.GetPixels();
                    }

                    if (gltfObject.UseBackgroundThread) await BackgroundThread;

                    var pixelCache = new Color[pixels.Length];

                    for (int c = 0; c < pixels.Length; c++)
                    {
                        pixelCache[c].r = pixels[c].b; // MRTK standard shader metallic value, glTF metallic value
                        pixelCache[c].g = occlusionPixels?[c].r ?? 1.0f; // MRTK standard shader occlusion value, glTF occlusion value if available
                        pixelCache[c].b = 0f; // MRTK standard shader emission value
                        pixelCache[c].a = (1.0f - pixels[c].g); // MRTK standard shader smoothness value, invert of glTF roughness value
                    }

                    if (gltfObject.UseBackgroundThread) await Update;
                    texture.SetPixels(pixelCache);
                    texture.Apply();

                    material.SetTexture(ChannelMapId, texture);
                    material.EnableKeyword("_CHANNEL_MAP");
                }
                else
                {
                    material.DisableKeyword("_CHANNEL_MAP");
                }

                material.SetFloat(SmoothnessId, Mathf.Abs((float)gltfMaterial.pbrMetallicRoughness.roughnessFactor - 1f));
                material.SetFloat(MetallicId, (float)gltfMaterial.pbrMetallicRoughness.metallicFactor);
            }


            if (gltfMaterial.normalTexture?.index >= 0)
            {
                material.SetTexture(NormalMapId, gltfObject.images[gltfMaterial.normalTexture.index].Texture);
                material.SetFloat(NormalMapScaleId, (float)gltfMaterial.normalTexture.scale);
                material.EnableKeyword("_NORMAL_MAP");
            }

            if (gltfMaterial.doubleSided)
            {
                material.SetFloat(CullModeId, (float)UnityEngine.Rendering.CullMode.Off);
            }

            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            return material;
        }

        private static async Task<Material> CreateStandardShaderMaterial(GltfObject gltfObject, GltfMaterial gltfMaterial, int materialId)
        {
            var shader = Shader.Find("Standard");

            if (shader == null) { return null; }

            var material = new Material(shader)
            {
                name = string.IsNullOrEmpty(gltfMaterial.name) ? $"glTF Material {materialId}" : gltfMaterial.name
            };

            if (gltfMaterial.pbrMetallicRoughness.baseColorTexture?.index >= 0)
            {
                material.mainTexture = gltfObject.images[gltfMaterial.pbrMetallicRoughness.baseColorTexture.index].Texture;
            }

            if (gltfMaterial.pbrMetallicRoughness?.baseColorFactor != null)
            {
                material.color = gltfMaterial.pbrMetallicRoughness.baseColorFactor.GetColorValue();
            }

            if (gltfMaterial.alphaMode == "MASK")
            {
                material.SetInt(SrcBlendId, (int)BlendMode.One);
                material.SetInt(DstBlendId, (int)BlendMode.Zero);
                material.SetInt(ZWriteId, 1);
                material.SetInt(ModeId, 3);
                material.SetOverrideTag("RenderType", "Cutout");
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
            }
            else if (gltfMaterial.alphaMode == "BLEND")
            {
                material.SetInt(SrcBlendId, (int)BlendMode.One);
                material.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
                material.SetInt(ZWriteId, 0);
                material.SetInt(ModeId, 3);
                material.SetOverrideTag("RenderType", "Transparency");
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            if (gltfMaterial.emissiveTexture?.index >= 0)
            {
                material.EnableKeyword("_EmissionMap");
                material.EnableKeyword("_EMISSION");
                material.SetTexture(EmissionMapId, gltfObject.images[gltfMaterial.emissiveTexture.index].Texture);
                material.SetColor(EmissionColorId, gltfMaterial.emissiveFactor.GetColorValue());
            }

            if (gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture?.index >= 0)
            {
                var texture = gltfObject.images[gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index].Texture;

                if (texture.isReadable)
                {
                    var pixels = texture.GetPixels();
                    if (gltfObject.UseBackgroundThread) await BackgroundThread;

                    var pixelCache = new Color[pixels.Length];

                    for (int c = 0; c < pixels.Length; c++)
                    {
                        // Unity only looks for metal in R channel, and smoothness in A.
                        pixelCache[c].r = pixels[c].g;
                        pixelCache[c].g = 0f;
                        pixelCache[c].b = 0f;
                        pixelCache[c].a = pixels[c].b;
                    }

                    if (gltfObject.UseBackgroundThread) await Update;
                    texture.SetPixels(pixelCache);
                    texture.Apply();

                    material.SetTexture(MetallicGlossMapId, texture);
                }

                material.SetFloat(GlossinessId, Mathf.Abs((float)gltfMaterial.pbrMetallicRoughness.roughnessFactor - 1f));
                material.SetFloat(MetallicId, (float)gltfMaterial.pbrMetallicRoughness.metallicFactor);
                material.EnableKeyword("_MetallicGlossMap");
                material.EnableKeyword("_METALLICGLOSSMAP");
            }

            if (gltfMaterial.normalTexture?.index >= 0)
            {
                material.SetTexture(BumpMapId, gltfObject.images[gltfMaterial.normalTexture.index].Texture);
                material.EnableKeyword("_BumpMap");
            }

            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            return material;
        }

        private static async Task ConstructSceneAsync(this GltfObject gltfObject, GltfScene gltfScene, GameObject root)
        {
            for (int i = 0; i < gltfScene.nodes.Length; i++)
            {
                // Note: glTF objects are currently imported with their original scale from the glTF scene, which may apply an unexpected transform
                // to the root node. If this behavior needs to be changed, functionality should be added below to ConstructNodeAsync
                await ConstructNodeAsync(gltfObject, gltfObject.nodes[gltfScene.nodes[i]], gltfScene.nodes[i], root.transform);
            }
        }

        private static async Task ConstructNodeAsync(GltfObject gltfObject, GltfNode node, int nodeId, Transform parent)
        {
            if (gltfObject.UseBackgroundThread) await Update;

            var nodeName = string.IsNullOrEmpty(node.name) ? $"glTF Node {nodeId}" : node.name;
            var nodeGameObject = new GameObject(nodeName);

            gltfObject.NodeGameObjectPairs.Add(nodeId, nodeGameObject);

            // If we're creating a really large node, we need it to not be visible in partial stages. So we hide it while we create it
            nodeGameObject.SetActive(false);

            if (gltfObject.UseBackgroundThread) await BackgroundThread;

            node.Matrix = node.GetTrsProperties(out Vector3 position, out Quaternion rotation, out Vector3 scale);

            if (node.Matrix == Matrix4x4.identity)
            {
                if (node.translation != null)
                {
                    position = node.translation.GetVector3Value();
                }

                if (node.rotation != null)
                {
                    rotation = node.rotation.GetQuaternionValue();
                }

                if (node.scale != null)
                {
                    scale = node.scale.GetVector3Value(false);
                }
            }

            if (gltfObject.UseBackgroundThread) await Update;

            nodeGameObject.transform.localPosition = position;
            nodeGameObject.transform.localRotation = rotation;
            nodeGameObject.transform.localScale = scale;

            if (node.mesh >= 0)
            {
                await ConstructMeshAsync(gltfObject, nodeGameObject, node.mesh);
            }

            if (node.children != null)
            {
                for (int i = 0; i < node.children.Length; i++)
                {
                    await ConstructNodeAsync(gltfObject, gltfObject.nodes[node.children[i]], node.children[i], nodeGameObject.transform);
                }
            }

            nodeGameObject.transform.SetParent(parent, false);
            nodeGameObject.SetActive(true);
        }

        private static async Task ConstructMeshAsync(GltfObject gltfObject, GameObject parent, int meshId)
        {
            GltfMesh gltfMesh = gltfObject.meshes[meshId];

            var renderer = parent.AddComponent<MeshRenderer>();
            var filter = parent.AddComponent<MeshFilter>();

            if (gltfMesh.primitives.Length == 1)
            {
                gltfMesh.Mesh = await ConstructMeshPrimitiveAsync(gltfObject, gltfMesh.primitives[0]);
                gltfMesh.Mesh.name = gltfMesh.name;
                filter.sharedMesh = gltfMesh.Mesh;
                renderer.sharedMaterial = gltfObject.materials[gltfMesh.primitives[0].material].Material;
                return;
            }

            var materials = new List<Material>();
            var meshCombines = new CombineInstance[gltfMesh.primitives.Length];

            for (int i = 0; i < gltfMesh.primitives.Length; i++)
            {
                meshCombines[i].mesh = await ConstructMeshPrimitiveAsync(gltfObject, gltfMesh.primitives[i]);

                var meshMaterial = gltfObject.materials[gltfMesh.primitives[i].material].Material;

                if (!materials.Contains(meshMaterial))
                {
                    materials.Add(meshMaterial);
                }
            }

            var newMesh = new Mesh();
            newMesh.CombineMeshes(meshCombines);
            gltfMesh.Mesh = filter.sharedMesh = newMesh;
            gltfMesh.Mesh.name = gltfMesh.name;
            renderer.sharedMaterials = materials.ToArray();
        }

        private static async Task<Mesh> ConstructMeshPrimitiveAsync(GltfObject gltfObject, GltfMeshPrimitive meshPrimitive)
        {
            if (gltfObject.UseBackgroundThread) await BackgroundThread;

            GltfAccessor positionAccessor = null;
            GltfAccessor normalsAccessor = null;
            GltfAccessor textCoord0Accessor = null;
            GltfAccessor textCoord1Accessor = null;
            GltfAccessor textCoord2Accessor = null;
            GltfAccessor textCoord3Accessor = null;
            GltfAccessor colorAccessor = null;
            GltfAccessor indicesAccessor = null;
            GltfAccessor tangentAccessor = null;
            GltfAccessor weight0Accessor = null;
            GltfAccessor joint0Accessor = null;
            int vertexCount = 0;

            positionAccessor = gltfObject.GetAccessor(meshPrimitive.Attributes.POSITION);
            if (positionAccessor != null)
            {
                vertexCount = positionAccessor.count;
            }

            normalsAccessor = gltfObject.GetAccessor(meshPrimitive.Attributes.NORMAL);

            textCoord0Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.TEXCOORD_0);

            textCoord1Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.TEXCOORD_1);

            textCoord2Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.TEXCOORD_2);

            textCoord3Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.TEXCOORD_3);

            colorAccessor = gltfObject.GetAccessor(meshPrimitive.Attributes.COLOR_0);

            indicesAccessor = gltfObject.GetAccessor(meshPrimitive.indices);

            tangentAccessor = gltfObject.GetAccessor(meshPrimitive.Attributes.TANGENT);

            weight0Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.WEIGHTS_0);

            joint0Accessor = gltfObject.GetAccessor(meshPrimitive.Attributes.JOINTS_0);

            if (gltfObject.UseBackgroundThread) await Update;

            var mesh = new Mesh
            {
                indexFormat = vertexCount > UInt16.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16,
            };

            if (positionAccessor != null)
            {
                mesh.vertices = positionAccessor.GetVector3Array();
            }

            if (normalsAccessor != null)
            {
                mesh.normals = normalsAccessor.GetVector3Array();
            }

            if (textCoord0Accessor != null)
            {
                mesh.uv = textCoord0Accessor.GetVector2Array();
            }

            if (textCoord1Accessor != null)
            {
                mesh.uv2 = textCoord1Accessor.GetVector2Array();
            }

            if (textCoord2Accessor != null)
            {
                mesh.uv3 = textCoord2Accessor.GetVector2Array();
            }

            if (textCoord3Accessor != null)
            {
                mesh.uv4 = textCoord3Accessor.GetVector2Array();
            }

            if (colorAccessor != null)
            {
                mesh.colors = colorAccessor.GetColorArray();
            }

            if (indicesAccessor != null)
            {
                mesh.triangles = indicesAccessor.GetIntArray();
            }

            if (tangentAccessor != null)
            {
                mesh.tangents = tangentAccessor.GetVector4Array();
            }

            if (weight0Accessor != null && joint0Accessor != null)
            {
                mesh.boneWeights = CreateBoneWeightArray(joint0Accessor.GetVector4Array(false), weight0Accessor.GetVector4Array(false), vertexCount);
            }

            mesh.RecalculateBounds();
            meshPrimitive.SubMesh = mesh;
            return mesh;
        }

        private static BoneWeight[] CreateBoneWeightArray(Vector4[] joints, Vector4[] weights, int vertexCount)
        {
            NormalizeBoneWeightArray(weights);

            var boneWeights = new BoneWeight[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                boneWeights[i].boneIndex0 = (int)joints[i].x;
                boneWeights[i].boneIndex1 = (int)joints[i].y;
                boneWeights[i].boneIndex2 = (int)joints[i].z;
                boneWeights[i].boneIndex3 = (int)joints[i].w;

                boneWeights[i].weight0 = weights[i].x;
                boneWeights[i].weight1 = weights[i].y;
                boneWeights[i].weight2 = weights[i].z;
                boneWeights[i].weight3 = weights[i].w;
            }

            return boneWeights;
        }

        private static void NormalizeBoneWeightArray(Vector4[] weights)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                var weightSum = weights[i].x + weights[i].y + weights[i].z + weights[i].w;

                if (!Mathf.Approximately(weightSum, 0))
                {
                    weights[i] /= weightSum;
                }
            }
        }
    }
}

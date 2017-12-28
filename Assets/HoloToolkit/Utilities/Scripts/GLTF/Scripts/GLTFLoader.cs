using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace GLTF
{
    public class GLTFLoader
    {
        public enum MaterialType
        {
            PbrMetallicRoughness,
            PbrSpecularGlossiness,
            CommonConstant,
            CommonPhong,
            CommonBlinn,
            CommonLambert
        }

        private struct MaterialCacheKey
        {
            public Material Material;
            public bool UseVertexColors;
        }

        public bool Multithreaded = true;
        public int MaximumLod = 300;
        public UnityEngine.Material ColorMaterial;
        public UnityEngine.Material NoColorMaterial;
        private readonly byte[] _gltfData;
        private GLTFRoot _root;
        private GameObject _lastLoadedScene;
        private AsyncAction asyncAction;
        private readonly Transform _sceneParent;

        private readonly Dictionary<Buffer, byte[]> _bufferCache = new Dictionary<Buffer, byte[]>();
        private readonly Dictionary<MaterialCacheKey, UnityEngine.Material> _materialCache = new Dictionary<MaterialCacheKey, UnityEngine.Material>();
        private readonly Dictionary<Image, Texture2D> _imageCache = new Dictionary<Image, Texture2D>();
        private Dictionary<Mesh, GameObject> _meshCache = new Dictionary<Mesh, GameObject>();
        private readonly Dictionary<MeshPrimitive, MeshPrimitiveAttributes> _attributesCache = new Dictionary<MeshPrimitive, MeshPrimitiveAttributes>();
        private readonly Dictionary<MaterialType, Shader> _shaderCache = new Dictionary<MaterialType, Shader>();

        public GLTFLoader(byte[] gltfData, Transform parent = null)
        {
            _gltfData = gltfData;
            _sceneParent = parent;
            asyncAction = new AsyncAction();
        }

        public GameObject LastLoadedScene
        {
            get
            {
                return _lastLoadedScene;
            }
        }

        public void SetShaderForMaterialType(MaterialType type, Shader shader)
        {
            _shaderCache.Add(type, shader);
        }

        public IEnumerator Load(int sceneIndex = -1)
        {
            if (_root == null)
            {
                if (Multithreaded)
                {
                    yield return asyncAction.RunOnWorkerThread(() => ParseGLTF(_gltfData));
                }
                else
                {
                    ParseGLTF(_gltfData);
                }
            }

            Scene scene;
            if (sceneIndex >= 0 && sceneIndex < _root.Scenes.Count)
            {
                scene = _root.Scenes[sceneIndex];
            }
            else
            {
                scene = _root.GetDefaultScene();
            }

            if (scene == null)
            {
                throw new Exception("No default scene in glTF file.");
            }

            if (_lastLoadedScene == null)
            {
                if (_root.Buffers != null)
                {
                    foreach (var buffer in _root.Buffers)
                    {
                        yield return LoadBuffer(buffer);
                    }
                }

                if (_root.Images != null)
                {
                    foreach (var image in _root.Images)
                    {
                        yield return LoadImage(image);
                    }
                }

                if (Multithreaded)
                {
                    yield return asyncAction.RunOnWorkerThread(() => BuildMeshAttributes());
                }
                else
                {
                    BuildMeshAttributes();
                }
            }
            else
            {
                _meshCache = new Dictionary<Mesh, GameObject>();
            }

            var sceneObj = CreateScene(scene);

            if (_sceneParent != null)
            {
                sceneObj.transform.SetParent(_sceneParent, false);
            }

            _lastLoadedScene = sceneObj;
        }

        private void ParseGLTF(byte[] gltfData)
        {
            byte[] glbBuffer;
            _root = GLTFParser.ParseBinary(gltfData, out glbBuffer);

            if (glbBuffer != null)
            {
                _bufferCache[_root.Buffers[0]] = glbBuffer;
            }
        }

        private void BuildMeshAttributes()
        {
            foreach (var mesh in _root.Meshes)
            {
                foreach (var primitive in mesh.Primitives)
                {
                    var attributes = primitive.BuildMeshAttributes(_bufferCache);
                    _attributesCache[primitive] = attributes;
                }
            }
        }

        private GameObject CreateScene(Scene scene)
        {
            var sceneObj = new GameObject(scene.Name ?? "GLTFScene");

            foreach (var node in scene.Nodes)
            {
                var nodeObj = CreateNode(node.Value);
                nodeObj.transform.SetParent(sceneObj.transform, false);
            }

            return sceneObj;
        }

        private GameObject CreateNode(Node node)
        {
            var nodeObj = new GameObject(node.Name ?? "GLTFNode");

            Vector3 position;
            Quaternion rotation;
            Vector3 scale;
            node.GetUnityTRSProperties(out position, out rotation, out scale);
            nodeObj.transform.localPosition = position;
            nodeObj.transform.localRotation = rotation;
            nodeObj.transform.localScale = scale;

            // TODO: Add support for skin/morph targets
            if (node.Mesh != null)
            {
                var meshObj = FindOrCreateMeshObject(node.Mesh.Value);
                meshObj.transform.SetParent(nodeObj.transform, false);
            }

            /* TODO: implement camera (probably a flag to disable for VR as well)
			if (camera != null)
			{
				GameObject cameraObj = camera.Value.Create();
				cameraObj.transform.parent = nodeObj.transform;
			}
			*/

            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    var childObj = CreateNode(child.Value);
                    childObj.transform.SetParent(nodeObj.transform, false);
                }
            }

            return nodeObj;
        }

        private GameObject FindOrCreateMeshObject(Mesh mesh)
        {
            GameObject meshObj;

            if (_meshCache.TryGetValue(mesh, out meshObj))
            {
                return GameObject.Instantiate(meshObj);
            }

            meshObj = CreateMeshObject(mesh);

            _meshCache.Add(mesh, meshObj);

            return meshObj;
        }

        private GameObject CreateMeshObject(Mesh mesh)
        {
            var meshName = mesh.Name ?? "GLTFMesh";
            var meshObj = new GameObject(meshName);

            foreach (var primitive in mesh.Primitives)
            {
                var primitiveObj = CreateMeshPrimitive(primitive);
                primitiveObj.transform.SetParent(meshObj.transform, false);
            }

            return meshObj;
        }

        private GameObject CreateMeshPrimitive(MeshPrimitive primitive)
        {
            var primitiveObj = new GameObject("Primitive");

            var meshFilter = primitiveObj.AddComponent<MeshFilter>();

            var attributes = _attributesCache[primitive];

            var mesh = new UnityEngine.Mesh
            {
                vertices = attributes.Vertices,
                normals = attributes.Normals,
                uv = attributes.Uv,
                uv2 = attributes.Uv2,
                uv3 = attributes.Uv3,
                uv4 = attributes.Uv4,
                colors = attributes.Colors,
                triangles = attributes.Triangles,
                tangents = attributes.Tangents
            };

            meshFilter.mesh = mesh;

            var meshRenderer = primitiveObj.AddComponent<MeshRenderer>();

            UnityEngine.Material material = null;

            if (primitive.Material != null)
            {
                var materialCacheKey = new MaterialCacheKey
                {
                    Material = primitive.Material.Value,
                    UseVertexColors = attributes.Colors != null
                };

                try
                {
                    material = FindOrCreateMaterial(materialCacheKey);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogWarningFormat("Failed to create material from {0}, using default", materialCacheKey.Material.Name);
                }
            }

            if (material == null)
            {
                var materialCacheKey = new MaterialCacheKey
                {
                    Material = new Material(),
                    UseVertexColors = attributes.Colors != null
                };
                material = FindOrCreateMaterial(materialCacheKey);
            }

            meshRenderer.material = material;

            return primitiveObj;
        }

        private UnityEngine.Material FindOrCreateMaterial(MaterialCacheKey materialKey)
        {
            UnityEngine.Material material;

            if (_materialCache.TryGetValue(materialKey, out material))
            {
                return material;
            }

            material = CreateMaterial(materialKey.Material, materialKey.UseVertexColors);

            _materialCache.Add(materialKey, material);

            return material;
        }

        private UnityEngine.Material CreateMaterial(Material def, bool useVertexColors)
        {
            UnityEngine.Material material = new UnityEngine.Material(useVertexColors ? ColorMaterial : NoColorMaterial);

            material.shader.maximumLOD = MaximumLod;

            if (def.AlphaMode == AlphaMode.MASK)
            {
                material.SetOverrideTag("RenderType", "TransparentCutout");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                material.SetFloat("_Cutoff", (float)def.AlphaCutoff);
            }
            else if (def.AlphaMode == AlphaMode.BLEND)
            {
                material.SetOverrideTag("RenderType", "Transparent");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                material.SetOverrideTag("RenderType", "Opaque");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
            }

            if (def.DoubleSided)
            {
                material.SetInt("_Cull", (int)CullMode.Off);
            }
            else
            {
                material.SetInt("_Cull", (int)CullMode.Back);
            }

            if (useVertexColors)
            {
                material.EnableKeyword("VERTEX_COLOR_ON");
            }

            if (def.PbrMetallicRoughness != null)
            {
                var pbr = def.PbrMetallicRoughness;

                material.SetColor("_Color", pbr.BaseColorFactor);

                if (pbr.BaseColorTexture != null)
                {
                    var texture = pbr.BaseColorTexture.Index.Value;
                    material.SetTexture("_MainTex", _imageCache[texture.Source.Value]);
                }

                material.SetFloat("_Metallic", (float)pbr.MetallicFactor);

                if (pbr.MetallicRoughnessTexture != null)
                {
                    var texture = pbr.MetallicRoughnessTexture.Index.Value;
                    material.SetTexture("_MetallicRoughnessMap", _imageCache[texture.Source.Value]);
                }

                material.SetFloat("_Roughness", (float)pbr.RoughnessFactor);
            }

            if (def.CommonConstant != null)
            {
                material.SetColor("_AmbientFactor", def.CommonConstant.AmbientFactor);

                if (def.CommonConstant.LightmapTexture != null)
                {
                    material.EnableKeyword("LIGHTMAP_ON");

                    var texture = def.CommonConstant.LightmapTexture.Index.Value;
                    material.SetTexture("_LightMap", _imageCache[texture.Source.Value]);
                    material.SetInt("_LightUV", def.CommonConstant.LightmapTexture.TexCoord);
                }

                material.SetColor("_LightFactor", def.CommonConstant.LightmapFactor);
            }

            if (def.NormalTexture != null)
            {
                var texture = def.NormalTexture.Index.Value;
                material.EnableKeyword("_NORMALMAP");
                material.SetTexture("_BumpMap", _imageCache[texture.Source.Value]);
                material.SetFloat("_BumpScale", (float)def.NormalTexture.Scale);
            }
            else
            {
                material.SetTexture("_BumpMap", null);
                material.DisableKeyword("_NORMALMAP");
            }

            if (def.OcclusionTexture != null)
            {
                var texture = def.OcclusionTexture.Index;

                material.SetFloat("_OcclusionStrength", (float)def.OcclusionTexture.Strength);

                if (def.PbrMetallicRoughness != null
                    && def.PbrMetallicRoughness.MetallicRoughnessTexture.Index.Id == texture.Id)
                {
                    material.EnableKeyword("OCC_METAL_ROUGH_ON");
                }
                else
                {
                    material.SetTexture("_OcclusionMap", _imageCache[texture.Value.Source.Value]);
                }
            }

            if (def.EmissiveTexture != null)
            {
                var texture = def.EmissiveTexture.Index.Value;
                material.EnableKeyword("_EMISSION");
                material.EnableKeyword("EMISSION_MAP_ON");
                material.SetTexture("_EmissionMap", _imageCache[texture.Source.Value]);
                material.SetInt("_EmissionUV", def.EmissiveTexture.TexCoord);
            }

            material.SetColor("_EmissionColor", def.EmissiveFactor);

            return material;
        }

        private const string Base64StringInitializer = "^data:[a-z-]+/[a-z-]+;base64,";

        /// <summary>
        ///  Get the absolute path to a glTF URI reference.
        /// </summary>
        /// <param name="relativePath">The relative path stored in the URI.</param>
        /// <returns></returns>
        private string AbsolutePath(string relativePath)
        {
            var uri = new Uri(relativePath);
            var partialPath = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments[uri.Segments.Length - 1].Length);
            return partialPath + relativePath;
        }

        private IEnumerator LoadImage(Image image)
        {
            Texture2D texture;

            if (image.Uri != null)
            {
                var uri = image.Uri;

                Regex regex = new Regex(Base64StringInitializer);
                Match match = regex.Match(uri);
                if (match.Success)
                {
                    var base64Data = uri.Substring(match.Length);
                    var textureData = Convert.FromBase64String(base64Data);
                    texture = new Texture2D(0, 0);
                    texture.LoadImage(textureData);
                }
                else
                {
                    var www = UnityWebRequest.Get(AbsolutePath(uri));
                    www.downloadHandler = new DownloadHandlerTexture();

#if UNITY_2017_2_OR_NEWER
                    yield return www.SendWebRequest();
#else
                    yield return www.Send();
#endif

                    // HACK to enable mipmaps :(
                    var tempTexture = DownloadHandlerTexture.GetContent(www);
                    if (tempTexture != null)
                    {
                        texture = new Texture2D(tempTexture.width, tempTexture.height, tempTexture.format, true);
                        texture.SetPixels(tempTexture.GetPixels());
                        texture.Apply(true);
                    }
                    else
                    {
                        Debug.LogFormat("{0} {1}", www.responseCode, www.url);
                        texture = new Texture2D(16, 16);
                    }
                }
            }
            else
            {
                texture = new Texture2D(0, 0);
                var bufferView = image.BufferView.Value;
                var buffer = bufferView.Buffer.Value;
                var bufferData = _bufferCache[buffer];
                var data = new byte[bufferView.ByteLength];
                System.Buffer.BlockCopy(bufferData, bufferView.ByteOffset, data, 0, data.Length);
                texture.LoadImage(data);
            }

            _imageCache[image] = texture;
        }

        /// <summary>
        /// Load the remote URI data into a byte array.
        /// </summary>
        private IEnumerator LoadBuffer(Buffer buffer)
        {
            if (buffer.Uri != null)
            {
                byte[] bufferData;
                var uri = buffer.Uri;

                Regex regex = new Regex(Base64StringInitializer);
                Match match = regex.Match(uri);
                if (match.Success)
                {
                    var base64Data = uri.Substring(match.Length);
                    bufferData = Convert.FromBase64String(base64Data);
                }
                else
                {
                    var www = UnityWebRequest.Get(AbsolutePath(uri));

#if UNITY_2017_2_OR_NEWER
                    yield return www.SendWebRequest();
#else
                    yield return www.Send();
#endif

                    bufferData = www.downloadHandler.data;
                }

                _bufferCache[buffer] = bufferData;
            }
        }
    }
}

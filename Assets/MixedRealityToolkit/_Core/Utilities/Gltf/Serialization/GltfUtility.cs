// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Serialization
{
    public static class GltfUtility
    {
        public const uint GltfMagicNumber = 0x46546C67;

        private static readonly WaitForUpdate Update = new WaitForUpdate();
        private static readonly WaitForBackgroundThread BackgroundThread = new WaitForBackgroundThread();

        /// <summary>
        /// Imports a glTF object from the provided uri
        /// </summary>
        /// <param name="uri">the path to the file to load</param>
        /// <returns>New <see cref="GltfObject"/> imported from uri.</returns>
        public static async Task<GltfObject> ImportGltfObjectFromPathAsync(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                Debug.LogError("Uri is not valid.");
                return null;
            }

            GltfObject gltfObject;
            bool isGlb = false;
            await BackgroundThread;

            if (uri.Contains(".gltf"))
            {
                string gltfJson = File.ReadAllText(uri);

                gltfObject = GetGltfObjectFromJson(gltfJson);

                if (gltfObject == null)
                {
                    Debug.LogError("Failed load Gltf Object from json schema.");
                    return null;
                }
            }
            else if (uri.Contains(".glb"))
            {
                isGlb = true;
                byte[] glbData;

                using (FileStream stream = File.Open(uri, FileMode.Open))
                {
                    glbData = new byte[stream.Length];
                    await stream.ReadAsync(glbData, 0, (int)stream.Length);
                }

                gltfObject = GetGltfObjectFromGlb(glbData);

                if (gltfObject == null)
                {
                    Debug.LogError("Failed to load GlTF Object from .glb!");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Unsupported file name extension.");
                return null;
            }

            gltfObject.Uri = uri;
            int nameStart = uri.LastIndexOf("\\", StringComparison.Ordinal) + 1;
            int nameLength = uri.Length - nameStart;
            gltfObject.Name = uri.Substring(nameStart, nameLength).Replace(isGlb ? ".glb" : ".gltf", string.Empty);

            await gltfObject.ConstructAsync();

            if (gltfObject.GameObjectReference == null)
            {
                Debug.LogError("Failed to construct Gltf Object.");
            }

            await Update;
            return gltfObject;
        }

        /// <summary>
        /// Gets a glTF object from the provided json string.
        /// </summary>
        /// <param name="jsonString">String defining a glTF Object.</param>
        /// <returns><see cref="GltfObject"/></returns>
        /// <remarks>Returned <see cref="GltfObject"/> still needs to be initialized using <see cref="ConstructGltf.ConstructAsync"/>.</remarks>
        public static GltfObject GetGltfObjectFromJson(string jsonString)
        {
            var gltfObject = JsonUtility.FromJson<GltfObject>(jsonString);

            for (int i = 0; i < gltfObject.extensionsRequired?.Length; i++)
            {
                var extensionsRequired = GetGltfExtensionObjects(jsonString, gltfObject.extensionsRequired[i]);

                foreach (var extensionRequired in extensionsRequired)
                {
                    // TODO Update this after KHR_materials_pbrSpecularGlossiness extension is supported
                    //if (gltfObject.extensionsUsed[i].Equals("KHR_materials_pbrSpecularGlossiness"))
                    //{
                    //    for (int j = 0; j < gltfObject.materials.Length; j++)
                    //    {
                    //        if (!string.IsNullOrEmpty(gltfObject.materials[i].name) &&
                    //            gltfObject.materials[i].name == extensionRequired.Key)
                    //        {
                    //            gltfObject.materials[i].Extensions.Add(gltfObject.extensionsUsed[i], extensionRequired.Value);
                    //            var extension = JsonUtility.FromJson<KHR_Materials_PbrSpecularGlossiness>(extensionRequired.Value);
                    //            extension.ElementName = gltfObject.materials[i].name;
                    //            gltfObject.RegisteredExtensions.Add(extension);
                    //        }
                    //    }
                    //}
                    //else
                    {
                        Debug.LogWarning($"Unsupported Extension: {gltfObject.extensionsRequired[i]}");
                        return null;
                    }
                }
            }

            for (int i = 0; i < gltfObject.extensionsUsed?.Length; i++)
            {
                var extensionsUsed = GetGltfExtensionObjects(jsonString, gltfObject.extensionsUsed[i]);

                foreach (var extensionUsed in extensionsUsed)
                {
                    // TODO Update this after KHR_materials_pbrSpecularGlossiness extension is supported
                    //if (gltfObject.extensionsUsed[i].Equals("KHR_materials_pbrSpecularGlossiness"))
                    //{
                    //    for (int j = 0; j < gltfObject.materials.Length; j++)
                    //    {
                    //        if (!string.IsNullOrEmpty(gltfObject.materials[i].name) &&
                    //            gltfObject.materials[i].name == extensionUsed.Key)
                    //        {
                    //            gltfObject.materials[i].Extensions.Add(gltfObject.extensionsUsed[i], extensionUsed.Value);
                    //            var extension = JsonUtility.FromJson<KHR_Materials_PbrSpecularGlossiness>(extensionUsed.Value);
                    //            extension.ElementName = gltfObject.materials[i].name;
                    //            gltfObject.RegisteredExtensions.Add(extension);
                    //        }
                    //    }
                    //}
                    //else
                    {
                        Debug.LogWarning($"Unsupported Extension: {gltfObject.extensionsUsed[i]}");
                    }
                }
            }

            var meshPrimitiveAttributes = GetGltfMeshPrimitiveAttributes(jsonString);

            for (int i = 0; i < gltfObject.meshes.Length; i++)
            {
                for (int j = 0; j < gltfObject.meshes[i].primitives.Length; j++)
                {
                    gltfObject.meshes[i].primitives[j].Attributes = JsonUtility.FromJson<GltfMeshPrimitiveAttributes>(meshPrimitiveAttributes[0]);
                    meshPrimitiveAttributes.Remove(meshPrimitiveAttributes[0]);
                }
            }

            return gltfObject;
        }

        /// <summary>
        /// Gets a glTF object from the provided path
        /// </summary>
        /// <param name="glbData">Raw glb byte data.</param>
        /// <returns><see cref="GltfObject"/></returns>
        /// <remarks>Returned <see cref="GltfObject"/> still needs to be initialized using <see cref="ConstructGltf.ConstructAsync"/>.</remarks>
        public static GltfObject GetGltfObjectFromGlb(byte[] glbData)
        {
            const int stride = sizeof(uint);

            var magicNumber = BitConverter.ToUInt32(glbData, 0);
            var version = BitConverter.ToUInt32(glbData, stride);
            //var length = BitConverter.ToUInt32(glbData, stride * 2);

            if (magicNumber != GltfMagicNumber)
            {
                Debug.LogError("File is not a glb object!");
                return null;
            }

            if (version != 2)
            {
                Debug.LogError("Glb file version mismatch! Glb must use version 2");
                return null;
            }

            var chunk0Length = BitConverter.ToUInt32(glbData, stride * 3);
            var chunk0Type = BitConverter.ToUInt32(glbData, stride * 4);

            if (chunk0Type != (ulong)GltfChunkType.Json)
            {
                Debug.LogError("Expected chunk 0 to be Json data!");
                return null;
            }

            string jsonChunk = Encoding.ASCII.GetString(glbData, stride * 5, (int)chunk0Length);

            var gltfObject = GetGltfObjectFromJson(jsonChunk);

            var chunk1Length = BitConverter.ToUInt32(glbData, stride * 5 + (int)chunk0Length);
            var chunk1Type = BitConverter.ToUInt32(glbData, stride * 6 + (int)chunk0Length);

            if (chunk1Type != (ulong)GltfChunkType.BIN)
            {
                Debug.LogError("Expected chunk 1 to be BIN data!");
                return null;
            }

            Debug.Assert(gltfObject.buffers[0].byteLength == chunk1Length, "chunk 1 & buffer 0 length mismatch");

            gltfObject.buffers[0].BufferData = new byte[(int)chunk1Length];
            Array.Copy(glbData, stride * 7 + (int)chunk0Length, gltfObject.buffers[0].BufferData, 0, (int)chunk1Length);

            return gltfObject;
        }

        /// <summary>
        /// Get a single Json Object using the handle provided.
        /// </summary>
        /// <param name="jsonString">The json string to search.</param>
        /// <param name="handle">The handle to look for.</param>
        /// <returns>A snippet of the json string that defines the object.</returns>
        private static string GetJsonObject(string jsonString, string handle)
        {
            var regex = new Regex($"\"{handle}\"\\s*:\\s*\\{{");
            var match = regex.Match(jsonString);
            return match.Success ? GetJsonObject(jsonString, match.Index + match.Length) : null;
        }

        private static List<string> GetGltfMeshPrimitiveAttributes(string jsonString)
        {
            var regex = new Regex("(?<Attributes>\"attributes\"[^}]+})");
            return GetGltfMeshPrimitiveAttributes(jsonString, regex);
        }

        private static List<string> GetGltfMeshPrimitiveAttributes(string jsonString, Regex regex)
        {
            var jsonObjects = new List<string>();

            if (!regex.IsMatch(jsonString))
            {
                return jsonObjects;
            }

            MatchCollection matches = regex.Matches(jsonString);

            for (var i = 0; i < matches.Count; i++)
            {
                jsonObjects.Add(matches[i].Groups["Attributes"].Captures[0].Value.Replace("\"attributes\":", string.Empty));
            }

            return jsonObjects;
        }

        /// <summary>
        /// Get a collection of glTF Extensions using the handle provided.
        /// </summary>
        /// <param name="jsonString">The json string to search.</param>
        /// <param name="handle">The handle to look for.</param>
        /// <returns>A collection of snippets with the json string that defines the object.</returns>
        private static Dictionary<string, string> GetGltfExtensionObjects(string jsonString, string handle)
        {
            // Bug: sometimes name isn't always before extension declaration
            var regex = new Regex($"(\"name\":\\s*\"\\w*\",\\s*\"extensions\":\\s*{{\\s*?)(\"{handle}\"\\s*:\\s*{{)");
            return GetGltfExtensions(jsonString, regex);
        }

        /// <summary>
        /// Get a collection of glTF Extras using the handle provided.
        /// </summary>
        /// <param name="jsonString">The json string to search.</param>
        /// <param name="handle">The handle to look for.</param>
        /// <returns>A collection of snippets with the json string that defines the object.</returns>
        public static Dictionary<string, string> GetGltfExtraObjects(string jsonString, string handle)
        {
            // Bug: sometimes name isn't always before extra declaration
            var regex = new Regex($"(\"name\":\\s*\"\\w*\",\\s*\"extras\":\\s*{{\\s*?)(\"{handle}\"\\s*:\\s*{{)");
            return GetGltfExtensions(jsonString, regex);
        }

        private static Dictionary<string, string> GetGltfExtensions(string jsonString, Regex regex)
        {
            var jsonObjects = new Dictionary<string, string>();

            if (!regex.IsMatch(jsonString))
            {
                return jsonObjects;
            }

            var matches = regex.Matches(jsonString);
            var nodeName = string.Empty;

            for (var i = 0; i < matches.Count; i++)
            {
                for (int j = 0; j < matches[i].Groups.Count; j++)
                {
                    for (int k = 0; k < matches[i].Groups[i].Captures.Count; k++)
                    {
                        nodeName = GetGltfNodeName(matches[i].Groups[i].Captures[i].Value);
                    }
                }

                if (!jsonObjects.ContainsKey(nodeName))
                {
                    jsonObjects.Add(nodeName, GetJsonObject(jsonString, matches[i].Index + matches[i].Length));
                }
            }

            return jsonObjects;
        }

        private static string GetJsonObject(string jsonString, int startOfObject)
        {
            int index;
            int bracketCount = 1;

            for (index = startOfObject; bracketCount > 0; index++)
            {
                if (jsonString[index] == '{')
                {
                    bracketCount++;
                }
                else if (jsonString[index] == '}')
                {
                    bracketCount--;
                }
            }

            return $"{{{jsonString.Substring(startOfObject, index - startOfObject)}";
        }

        private static string GetGltfNodeName(string jsonString)
        {
            jsonString = jsonString.Replace("\"name\"", string.Empty);
            jsonString = jsonString.Replace(": \"", string.Empty);
            jsonString = jsonString.Replace(":\"", string.Empty);
            jsonString = jsonString.Substring(0, jsonString.IndexOf("\"", StringComparison.Ordinal));
            return jsonString;
        }
    }
}
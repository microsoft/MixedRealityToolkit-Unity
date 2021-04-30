// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Storage;
using Windows.Storage.Streams;
#else
using Microsoft.MixedReality.Toolkit.Utilities;
#endif

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization
{
    public static class GltfUtility
    {
        private const uint GltfMagicNumber = 0x46546C67;

        private static readonly WaitForUpdate Update = new WaitForUpdate();
        private static readonly WaitForBackgroundThread BackgroundThread = new WaitForBackgroundThread();
        private static readonly string DefaultObjectName = "GLTF Object";

        /// <summary>
        /// Imports a glTF object from the provided uri.
        /// </summary>
        /// <param name="uri">the path to the file to load</param>
        /// <returns>New <see cref="Schema.GltfObject"/> imported from uri.</returns>
        /// <remarks>
        /// <para>Must be called from the main thread.
        /// If the <see href="https://docs.unity3d.com/ScriptReference/Application-isPlaying.html">Application.isPlaying</see> is false, then this method will run synchronously.</para>
        /// </remarks>
        public static async Task<GltfObject> ImportGltfObjectFromPathAsync(string uri)
        {
            if (!SyncContextUtility.IsMainThread)
            {
                Debug.LogError("ImportGltfObjectFromPathAsync must be called from the main thread!");
                return null;
            }

            if (string.IsNullOrWhiteSpace(uri))
            {
                Debug.LogError("Uri is not valid.");
                return null;
            }

            GltfObject gltfObject;
            bool useBackgroundThread = Application.isPlaying;

            if (useBackgroundThread) { await BackgroundThread; }

            if (uri.EndsWith(".gltf", StringComparison.OrdinalIgnoreCase))
            {
                string gltfJson = File.ReadAllText(uri);

                gltfObject = GetGltfObjectFromJson(gltfJson);

                if (gltfObject == null)
                {
                    Debug.LogError("Failed load Gltf Object from json schema.");
                    return null;
                }
            }
            else if (uri.EndsWith(".glb", StringComparison.OrdinalIgnoreCase))
            {
                byte[] glbData;

#if WINDOWS_UWP

                if (useBackgroundThread)
                {
                    try
                    {
                        var storageFile = await StorageFile.GetFileFromPathAsync(uri);

                        if (storageFile == null)
                        {
                            Debug.LogError($"Failed to locate .glb file at {uri}");
                            return null;
                        }

                        var buffer = await FileIO.ReadBufferAsync(storageFile);

                        using (DataReader dataReader = DataReader.FromBuffer(buffer))
                        {
                            glbData = new byte[buffer.Length];
                            dataReader.ReadBytes(glbData);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        return null;
                    }
                }
                else
                {
                    glbData = UnityEngine.Windows.File.ReadAllBytes(uri);
                }
#else
                using (FileStream stream = File.Open(uri, FileMode.Open))
                {
                    glbData = new byte[stream.Length];

                    if (useBackgroundThread)
                    {
                        await stream.ReadAsync(glbData, 0, (int)stream.Length);
                    }
                    else
                    {
                        stream.Read(glbData, 0, (int)stream.Length);
                    }
                }
#endif

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

            try
            {
                gltfObject.Name = Path.GetFileNameWithoutExtension(uri);
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("Uri contained invalid character");
                gltfObject.Name = DefaultObjectName;
            }

            gltfObject.UseBackgroundThread = useBackgroundThread;
            await gltfObject.ConstructAsync();

            if (gltfObject.GameObjectReference == null)
            {
                Debug.LogError("Failed to construct Gltf Object.");
            }

            if (useBackgroundThread) { await Update; }

            return gltfObject;
        }

        /// <summary>
        /// Gets a glTF object from the provided json string.
        /// </summary>
        /// <param name="jsonString">String defining a glTF Object.</param>
        /// <returns><see cref="Schema.GltfObject"/></returns>
        /// <remarks>Returned <see cref="Schema.GltfObject"/> still needs to be initialized using <see cref="ConstructGltf.ConstructAsync"/>.</remarks>
        public static GltfObject GetGltfObjectFromJson(string jsonString)
        {
            var gltfObject = JsonUtility.FromJson<GltfObject>(jsonString);

            if (gltfObject.extensionsRequired?.Length > 0)
            {
                Debug.LogError("One or more unsupported glTF extensions required. Unable to load the model.");
                for (int i = 0; i < gltfObject.extensionsRequired.Length; ++i)
                {
                    Debug.Log($"Extension: {gltfObject.extensionsRequired[i]}");
                }
                return null;
            }

            if (gltfObject.extensionsUsed?.Length > 0)
            {
                Debug.Log("One or more unsupported glTF extensions in use, ignoring them.");
                for (int i = 0; i < gltfObject.extensionsUsed.Length; ++i)
                {
                    Debug.Log($"Extension: {gltfObject.extensionsUsed[i]}");
                }
            }

            var meshPrimitiveAttributes = GetGltfMeshPrimitiveAttributes(jsonString);
            int numPrimitives = 0;

            for (var i = 0; i < gltfObject.meshes?.Length; i++)
            {
                numPrimitives += gltfObject.meshes[i]?.primitives?.Length ?? 0;
            }

            if (numPrimitives != meshPrimitiveAttributes.Count)
            {
                Debug.LogError("The number of mesh primitive attributes does not match the number of mesh primitives");
                return null;
            }

            int primitiveIndex = 0;

            for (int i = 0; i < gltfObject.meshes?.Length; i++)
            {
                for (int j = 0; j < gltfObject.meshes[i].primitives.Length; j++)
                {
                    gltfObject.meshes[i].primitives[j].Attributes = new GltfMeshPrimitiveAttributes(StringIntDictionaryFromJson(meshPrimitiveAttributes[primitiveIndex]));
                    primitiveIndex++;
                }
            }

            return gltfObject;
        }

        /// <summary>
        /// Gets a glTF object from the provided byte array
        /// </summary>
        /// <param name="glbData">Raw glb byte data.</param>
        /// <returns><see cref="Schema.GltfObject"/></returns>
        /// <remarks>Returned <see cref="Schema.GltfObject"/> still needs to be initialized using <see cref="ConstructGltf.ConstructAsync"/>.</remarks>
        public static GltfObject GetGltfObjectFromGlb(byte[] glbData)
        {
            const int stride = sizeof(uint);

            var magicNumber = BitConverter.ToUInt32(glbData, 0);
            var version = BitConverter.ToUInt32(glbData, stride);
            var length = BitConverter.ToUInt32(glbData, stride * 2);

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

            if (length != glbData.Length)
            {
                Debug.LogError("Glb file size does not match the glb header defined size");
                return null;
            }

            var chunk0Length = (int)BitConverter.ToUInt32(glbData, stride * 3);
            var chunk0Type = BitConverter.ToUInt32(glbData, stride * 4);

            if (chunk0Type != (ulong)GltfChunkType.Json)
            {
                Debug.LogError("Expected chunk 0 to be Json data!");
                return null;
            }

            var jsonChunk = Encoding.ASCII.GetString(glbData, stride * 5, chunk0Length);
            var gltfObject = GetGltfObjectFromJson(jsonChunk);
            var chunk1Length = (int)BitConverter.ToUInt32(glbData, stride * 5 + chunk0Length);
            var chunk1Type = BitConverter.ToUInt32(glbData, stride * 6 + chunk0Length);

            if (chunk1Type != (ulong)GltfChunkType.BIN)
            {
                Debug.LogError("Expected chunk 1 to be BIN data!");
                return null;
            }

            // Per the spec, "byte length of BIN chunk could be up to 3 bytes bigger than JSON-defined buffer.byteLength to satisfy GLB padding requirements"
            // https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/README.md#glb-stored-buffer
            Debug.Assert(gltfObject.buffers[0].byteLength <= chunk1Length && gltfObject.buffers[0].byteLength >= chunk1Length - 3, "chunk 1 & buffer 0 length mismatch");

            gltfObject.buffers[0].BufferData = new byte[chunk1Length];
            Array.Copy(glbData, stride * 7 + chunk0Length, gltfObject.buffers[0].BufferData, 0, chunk1Length);

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
            var regex = new Regex("\"attributes\" ?: ?(?<Data>{[^}]+})");
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
                jsonObjects.Add(matches[i].Groups["Data"].Captures[0].Value);
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
            // Assumption: This code assumes that a name is declared before extensions in the glTF schema.
            // This may not work for all exporters. Some exporters may fail to adhere to the standard glTF schema.
            var regex = new Regex($"(\"name\":\\s*\"\\w*\",\\s*\"extensions\":\\s*{{\\s*?)(\"{handle}\"\\s*:\\s*{{)");
            return GetGltfExtensions(jsonString, regex);
        }

        /// <summary>
        /// Get a collection of glTF Extras using the handle provided.
        /// </summary>
        /// <param name="jsonString">The json string to search.</param>
        /// <param name="handle">The handle to look for.</param>
        /// <returns>A collection of snippets with the json string that defines the object.</returns>
        private static Dictionary<string, string> GetGltfExtraObjects(string jsonString, string handle)
        {
            // Assumption: This code assumes that a name is declared before extensions in the glTF schema.
            // This may not work for all exporters. Some exporters may fail to adhere to the standard glTF schema.
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

        /// <summary>
        /// A utility function to work around the JsonUtility inability to deserialize to a dictionary.
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <returns>A dictionary with the key value pairs found in the json</returns>
        private static Dictionary<string, int> StringIntDictionaryFromJson(string json)
        {
            string reformatted = JsonDictionaryToArray(json);
            StringIntKeyValueArray loadedData = JsonUtility.FromJson<StringIntKeyValueArray>(reformatted);
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < loadedData.items.Length; i++)
            {
                dictionary.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            return dictionary;
        }

        /// <summary>
        /// Takes a json object string with key value pairs, and returns a json string
        /// in the format of `{"items": [{"key": $key_name, "value": $value}]}`.
        /// This format can be handled by JsonUtility and support an arbitrary number
        /// of key/value pairs
        /// </summary>
        /// <param name="json">JSON string in the format `{"key": $value}`</param>
        /// <returns>Returns a reformatted JSON string</returns>
        private static string JsonDictionaryToArray(string json)
        {
            string reformatted = "{\"items\": [";
            string pattern = @"""(\w+)"":\s?(""?\w+""?)";
            RegexOptions options = RegexOptions.Multiline;

            foreach (Match m in Regex.Matches(json, pattern, options))
            {
                string key = m.Groups[1].Value;
                string value = m.Groups[2].Value;

                reformatted += $"{{\"key\":\"{key}\", \"value\":{value}}},";
            }
            reformatted = reformatted.TrimEnd(',');
            reformatted += "]}";
            return reformatted;
        }

        [System.Serializable]
        private class StringKeyValue
        {
            public string key = string.Empty;
            public int value = 0;
        }

        [System.Serializable]
        private class StringIntKeyValueArray
        {
            public StringKeyValue[] items = Array.Empty<StringKeyValue>();
        }

    }
}

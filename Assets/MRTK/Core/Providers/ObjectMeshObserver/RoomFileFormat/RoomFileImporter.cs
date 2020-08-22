// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif // UNITY_2020_2_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.RoomFile
{
    [ScriptedImporter(1, "room")]
    public class RoomFileImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            FileInfo fileInfo = new FileInfo(context.assetPath);
            string name = fileInfo.Name.Split(new char[] { '.' })[0];

            IList<Mesh> meshes;

            using (BinaryReader reader = OpenFileForRead(fileInfo.FullName))
            {
                meshes = RoomFileSerializer.Deserialize(reader);
            }

            GameObject model = new GameObject(name);
            context.AddObjectToAsset(name, model);

            for (int i = 0; i < meshes.Count; i++)
            {
                string meshName = $"{name}_{i}";
                GameObject meshObject = new GameObject(meshName, new System.Type[] { typeof(MeshRenderer), typeof(MeshFilter) });

                meshes[i].name = meshName;
                meshObject.GetComponent<MeshFilter>().sharedMesh = meshes[i];
                context.AddObjectToAsset(meshName, meshes[i]);
                meshObject.transform.parent = model.transform;
            }
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="fileName">The name of the file, including extension. </param>
        /// <returns>The reader used to read the file's contents.</returns>
        private BinaryReader OpenFileForRead(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Debug.LogError($"Unable to open {fileName}, the file does not exist.");
                return null;
            }

            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (stream == null)
            {
                return null;
            }

            return new BinaryReader(stream);
        }
    }
}

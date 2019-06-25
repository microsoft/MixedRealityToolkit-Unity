// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Miicrosoft.MixedReality.Toolkit.SpatialObjectMeshObserver.RoomFile
{
    [ScriptedImporter(1, "room")]
    public class RoomFileImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            string fileName = context.assetPath;

            IList<Mesh> meshes;

            using (BinaryReader reader = OpenFileForRead(fileName))
            {
                meshes = RoomFileSerializer.Deserialize(reader);
            }

            // todo
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

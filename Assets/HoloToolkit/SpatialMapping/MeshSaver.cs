// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if !UNITY_EDITOR
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace HoloToolkit.Unity
{
    /// <summary>
    /// MeshSaver is a static class containing methods used for saving and loading meshes.
    /// </summary>
    public static class MeshSaver
    {
        /// <summary>
        /// The extension given to mesh files.
        /// </summary>
        private static string fileExtension = ".room";

        /// <summary>
        /// Read-only property which returns the folder path where mesh files are stored.
        /// </summary>
        public static string MeshFolderName
        {
            get
            {
#if !UNITY_EDITOR
                return ApplicationData.Current.RoamingFolder.Path;
#else
                return Application.persistentDataPath;
#endif
            }
        }

        /// <summary>
        /// Saves the provided meshes to the specified file.
        /// </summary>
        /// <param name="fileName">Name to give the saved mesh file. Exclude path and extension.</param>
        /// <param name="meshes">The collection of Mesh objects to save.</param>
        /// <returns>Fully qualified name of the saved mesh file.</returns>
        /// <remarks>Determines the save path to use and automatically applys the file extension.</remarks>
        public static string Save(string fileName, IEnumerable<Mesh> meshes)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Must specify a valid fileName.");
            }

            if (meshes == null)
            {
                throw new ArgumentNullException("Value of meshes cannot be null.");
            }

            // Create the mesh file.
            String folderName = MeshFolderName;
            Debug.Log(String.Format("Saving mesh file: {0}", Path.Combine(folderName, fileName + fileExtension)));

            using (Stream stream = OpenFileForWrite(folderName, fileName + fileExtension))
            {
                // Serialize and write the meshes to the file.
                byte[] data = SimpleMeshSerializer.Serialize(meshes);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }

            Debug.Log("Mesh file saved.");

            return Path.Combine(folderName, fileName + fileExtension);
        }

        /// <summary>
        /// Loads the specified mesh file.
        /// </summary>
        /// <param name="fileName">Name of the saved mesh file. Exclude path and extension.</param>
        /// <returns>Collection of Mesh objects read from the file.</returns>
        /// <remarks>Determines the path from which to load and automatically applys the file extension.</remarks>
        public static IEnumerable<Mesh> Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("Must specify a valid fileName.");
            }

            List<Mesh> meshes = new List<Mesh>();

            // Open the mesh file.
            String folderName = MeshFolderName;
            Debug.Log(String.Format("Loading mesh file: {0}", Path.Combine(folderName, fileName + fileExtension)));

            using (Stream stream = OpenFileForRead(folderName, fileName + fileExtension))
            {
                // Read the file and deserialize the meshes.
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);

                meshes.AddRange(SimpleMeshSerializer.Deserialize(data));
            }

            Debug.Log("Mesh file loaded.");

            return meshes;
        }

        /// <summary>
        /// Opens the specified file for reading.
        /// </summary>
        /// <param name="folderName">The name of the folder containing the file.</param>
        /// <param name="fileName">The name of the file, including extension. </param>
        /// <returns>Stream used for reading the file's data.</returns>
        private static Stream OpenFileForRead(string folderName, string fileName)
        {
            Stream stream = null;

#if !UNITY_EDITOR
            Task task = new Task(
                            async () =>
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderName);
                                StorageFile file = await folder.GetFileAsync(fileName);
                                stream = await file.OpenStreamForReadAsync();
                            });
            task.Start();
            task.Wait();
#else
            stream = new FileStream(Path.Combine(folderName, fileName), FileMode.Open, FileAccess.Read);
#endif
            return stream;
        }

        /// <summary>
        /// Opens the specified file for writing.
        /// </summary>
        /// <param name="folderName">The name of the folder containing the file.</param>
        /// <param name="fileName">The name of the file, including extension.</param>
        /// <returns>Stream used for writing the file's data.</returns>
        /// <remarks>If the specified file already exists, it will be overwritten.</remarks>
        private static Stream OpenFileForWrite(string folderName, string fileName)
        {
            Stream stream = null;

#if !UNITY_EDITOR
            Task task = new Task(
                            async () =>
                            {
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderName);
                                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                stream = await file.OpenStreamForWriteAsync();
                            });
            task.Start();
            task.Wait();
#else
            stream = new FileStream(Path.Combine(folderName, fileName), FileMode.Create, FileAccess.Write);
#endif
            return stream;
        }
    }
}
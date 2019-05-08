// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CalibrationDataHelper
    {
        private const string ChessboardImageDirectoryName = "CHESS";
        private const string ChessboardDetectedImageDirectoryName = "CHESS_DETECTED";
        private const string DSLRArUcoDirectoryName = "DSLR_ARUCO";
        private const string DSLRArUcoDetectedDirectoryName = "DSLR_ARUCO_DETECTED";
        private const string HeadsetDataDirectory = "HEADSET_DATA";
        private const string RootDirectoryName = "Calibration";

        public static void Initialize()
        {
            string rootFolder = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName);
            InitializeDirectory(rootFolder);
            InitializeDirectory(Path.Combine(rootFolder, ChessboardImageDirectoryName));
            InitializeDirectory(Path.Combine(rootFolder, ChessboardDetectedImageDirectoryName));
            InitializeDirectory(Path.Combine(rootFolder, DSLRArUcoDirectoryName));
            InitializeDirectory(Path.Combine(rootFolder, DSLRArUcoDetectedDirectoryName));
            InitializeDirectory(Path.Combine(rootFolder, HeadsetDataDirectory));

            DirectoryInfo detectedChessboards = new DirectoryInfo(Path.Combine(rootFolder, ChessboardDetectedImageDirectoryName));
            foreach (FileInfo file in detectedChessboards.GetFiles())
            {
                file.Delete();
            }

            DirectoryInfo detectedArUcos = new DirectoryInfo(Path.Combine(rootFolder, DSLRArUcoDetectedDirectoryName));
            foreach (FileInfo file in detectedArUcos.GetFiles())
            {
                file.Delete();
            }
        }

        private static void InitializeDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }

        public static List<string> GetChessboardImageFileNames()
        {
            string rootFolder = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName);
            DirectoryInfo chessboardDirectory = new DirectoryInfo(Path.Combine(rootFolder, ChessboardImageDirectoryName));
            var files = chessboardDirectory.GetFiles();
            List<string> fileNames = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(files[i].Name);
                fileNames.Add(fileName);
            }

            fileNames.Sort();
            return fileNames;
        }

        public static List<string> GetArUcoDatasetFileNames()
        {
            string rootFolder = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName);
            DirectoryInfo arUcoDirectory = new DirectoryInfo(Path.Combine(rootFolder, DSLRArUcoDirectoryName));
            var files = arUcoDirectory.GetFiles();
            List<string> fileNames = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                var fileName = Path.GetFileNameWithoutExtension(files[i].Name);
                fileNames.Add(fileName);
            }

            fileNames.Sort();
            return fileNames;
        }

        public static void SaveImage(Texture2D image, string fileName)
        {
            SaveImage("", image, fileName);
        }

        public static void SaveChessboardImage(Texture2D image, string fileName)
        {
            SaveImage(ChessboardImageDirectoryName, image, fileName);
        }

        public static void SaveChessboardDetectedImage(Texture2D image, string fileName)
        {
            SaveImage(ChessboardDetectedImageDirectoryName, image, fileName);
        }

        public static void SaveDSLRArUcoImage(Texture2D image, string fileName)
        {
            SaveImage(DSLRArUcoDirectoryName, image, fileName);
        }

        public static void SaveDSLRArUcoDetectedImage(Texture2D image, string fileName)
        {
            SaveImage(DSLRArUcoDetectedDirectoryName, image, fileName);
        }

        private static void SaveImage(string directory, Texture2D image, string fileID)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, directory, $"{fileID}.png");
            File.WriteAllBytes(path, image.EncodeToPNG());
        }

        public static Texture2D LoadChessboardImage(string filename)
        {
            return LoadImage(ChessboardImageDirectoryName, filename);
        }

        public static Texture2D LoadDSLRArUcoImage(string filename)
        {
            return LoadImage(DSLRArUcoDirectoryName, filename);
        }

        private static Texture2D LoadImage(string directory, string filename)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, directory, $"{filename}.png");

            if (File.Exists(path))
            {
                var fileData = File.ReadAllBytes(path);
                Texture2D tempTexture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                tempTexture.LoadImage(fileData);
                tempTexture.Apply();

                if (tempTexture.format != TextureFormat.RGB24)
                {
                    Texture2D texture = new Texture2D(tempTexture.width, tempTexture.height, TextureFormat.RGB24, false);
                    texture.SetPixels(tempTexture.GetPixels());
                    texture.Apply();
                    return texture;
                }

                return tempTexture;
            }

            return null;
        }

        public static HeadsetCalibrationData LoadHeadsetData(string filename)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, HeadsetDataDirectory, $"{filename}.json");
            if (File.Exists(path))
            {
                var fileData = File.ReadAllBytes(path);
                if (HeadsetCalibrationData.TryDeserialize(fileData, out var calibrationData))
                {
                    return calibrationData;
                }
            }

            return null;
        }

        public static void SaveHeadsetData(HeadsetCalibrationData data, string filename)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, HeadsetDataDirectory, $"{filename}.json");
            File.WriteAllBytes(path, data.Serialize());
        }

        public static string SaveCameraIntrinsics(CalculatedCameraIntrinsics intrinsics)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, $"CameraIntrinsics.json");
            int i = 0;
            while (File.Exists(path))
            {
                path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, $"CameraIntrinsics_{i}.json");
                i++;
            }
            var str = JsonUtility.ToJson(intrinsics);
            var payload = Encoding.ASCII.GetBytes(str);
            File.WriteAllBytes(path, payload);
            return path;
        }

        public static CalculatedCameraIntrinsics LoadCameraIntrinsics(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var fileData = File.ReadAllBytes(path);
                    var str = Encoding.ASCII.GetString(fileData);
                    var intrinsics = JsonUtility.FromJson<CalculatedCameraIntrinsics>(str);
                    return intrinsics;
                }
                catch
                {
                    Debug.LogError($"Exception thrown loading camera intrinsics file {path}");
                }
            }
            else
            {
                Debug.LogError($"Failed to find camera intrinsics file {path}");
            }

            return null;
        }

        public static string SaveCameraExtrinsics(CalculatedCameraExtrinsics extrinsics)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, $"CameraExtrinsics.json");
            int i = 0;
            while (File.Exists(path))
            {
                path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, $"CameraExtrinsics_{i}.json");
                i++;
            }
            var str = JsonUtility.ToJson(extrinsics);
            var payload = Encoding.ASCII.GetBytes(str);
            File.WriteAllBytes(path, payload);
            return path;
        }

        public static CalculatedCameraExtrinsics LoadCameraExtrinsics(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var fileData = File.ReadAllBytes(path);
                    var str = Encoding.ASCII.GetString(fileData);
                    var extrinsics = JsonUtility.FromJson<CalculatedCameraExtrinsics>(str);
                    return extrinsics;
                }
                catch
                {
                    Debug.LogError($"Exception thrown loading camera extrinsics file {path}");
                }
            }
            else
            {
                Debug.LogError($"Failed to find camera extrinsics file {path}");
            }

            return null;
        }

        private static string GetDocumentsFolderPath()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#elif UNITY_WSA
            return Windows.Storage.KnownFolders.DocumentsLibrary.Path;
#else
            return String.empty;
#endif
        }

        /// <summary>
        /// Creates a unique file name based on the current time.
        /// </summary>
        /// <returns>Returns a file name based on the current time.</returns>
        public static string GetUniqueFileName()
        {
            return DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss");
        }
    }
}

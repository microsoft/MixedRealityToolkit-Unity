using System;
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
        private const string HeadsetDataDirectory = "META";
        private const string RootDirectoryName = "Calibration";

        public static void Initialize(out int nextChessboardImageId, out int nextArUcoImageId)
        {
            nextChessboardImageId = 0;
            nextArUcoImageId = 0;

            string rootFolder = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName);
            if (Directory.Exists(rootFolder))
            {
                InitializeDirectory(rootFolder, ChessboardImageDirectoryName, "*.png", out nextChessboardImageId);
                InitializeDirectory(rootFolder, DSLRArUcoDirectoryName, "*.png", out nextArUcoImageId);
                InitializeDirectory(rootFolder, HeadsetDataDirectory, "*.json", out nextArUcoImageId);
            }
            else
            {
                Directory.CreateDirectory(rootFolder);
                Directory.CreateDirectory(Path.Combine(rootFolder, ChessboardImageDirectoryName));
                Directory.CreateDirectory(Path.Combine(rootFolder, ChessboardDetectedImageDirectoryName));
                Directory.CreateDirectory(Path.Combine(rootFolder, DSLRArUcoDirectoryName));
                Directory.CreateDirectory(Path.Combine(rootFolder, DSLRArUcoDetectedDirectoryName));
                Directory.CreateDirectory(Path.Combine(rootFolder, HeadsetDataDirectory));
            }

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

        private static void InitializeDirectory(string rootFolder, string directoryName, string fileExtensionExpected, out int nextImageId)
        {
            nextImageId = 0;
            string directory = Path.Combine(rootFolder, directoryName);
            if (Directory.Exists(directory))
            {
                nextImageId = Math.Max(Directory.GetFiles(directory, fileExtensionExpected, SearchOption.TopDirectoryOnly).Length, nextImageId);
            }
            else
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void SaveImage(Texture2D image, string fileName)
        {
            SaveImage("", image, fileName);
        }

        public static void SaveChessboardImage(Texture2D image, int fileID)
        {
            SaveImage(ChessboardImageDirectoryName, image, fileID.ToString());
        }

        public static void SaveChessboardDetectedImage(Texture2D image, int fileID)
        {
            SaveImage(ChessboardDetectedImageDirectoryName, image, fileID.ToString());
        }

        public static void SaveDSLRArUcoImage(Texture2D image, int fileID)
        {
            SaveImage(DSLRArUcoDirectoryName, image, fileID.ToString());
        }

        public static void SaveDSLRArUcoDetectedImage(Texture2D image, int fileID)
        {
            SaveImage(DSLRArUcoDetectedDirectoryName, image, fileID.ToString());
        }

        private static void SaveImage(string directory, Texture2D image, string fileID)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, directory, $"{fileID}.png");
            File.WriteAllBytes(path, image.EncodeToPNG());
        }

        public static Texture2D LoadChessboardImage(int fileID)
        {
            return LoadImage(ChessboardImageDirectoryName, fileID);
        }

        public static Texture2D LoadDSLRArUcoImage(int fileID)
        {
            return LoadImage(DSLRArUcoDirectoryName, fileID);
        }

        private static Texture2D LoadImage(string directory, int fileID)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, directory, $"{fileID}.png");

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

        public static HeadsetCalibrationData LoadHeadsetData(int fileID)
        {
            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, HeadsetDataDirectory, $"{fileID}.json");
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

        public static void SaveHeadsetData(HeadsetCalibrationData data, int fileID)
        {
            byte[] temp = data.imageData.pixelData;
            data.imageData.pixelData = null;
            byte[] tempPNG = data.imageData.pngData;
            data.imageData.pngData = null;

            string path = Path.Combine(GetDocumentsFolderPath(), RootDirectoryName, HeadsetDataDirectory, $"{fileID}.json");
            File.WriteAllBytes(path, data.Serialize());

            data.imageData.pixelData = temp;
            data.imageData.pngData = tempPNG;
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
    }
}

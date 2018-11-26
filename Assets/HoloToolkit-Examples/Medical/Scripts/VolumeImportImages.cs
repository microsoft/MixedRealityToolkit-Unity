// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System.Linq;
using System.IO;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Processes a folder of "stacked" images representing 2D slices of a 3D volume and 
    /// creates a volume from them.  Dicom files of all different types can be exported
    /// with external tools to this format then processed here.
    /// </summary>
    public static class VolumeImportImages
    {
        //TODO: handle textures to import as actual assets so more file support works
        //supported formats from: https://docs.unity3d.com/430/Documentation/Components/class-Texture2D.html
        private static readonly string[] ValidImageFileExtensions = { ".jpg", ".png" };
        //private static readonly string[] ValidImageFileExtensions = { ".psd", ".tiff", ".jpg", ".tga", ".png", ".gif", ".bmp", ".iff", ".pict" };

        public static byte[] ConvertFolderToVolume(string folder, bool inferAlpha, out Int3 size)
        {
            var imageNames = GetImagesInFolder(folder);
            size = GetSizeOfVolumeFolder(folder);
            var voxels = new VolumeBuffer<Color32>(size);

            var tex = new Texture2D(2, 2);

            int z = 0;
            foreach (var imageFile in imageNames)
            {
                bool loaded = tex.LoadImage(FileSystemHelper.ReadBytesFromLocalFile(imageFile));
                if (!loaded)
                {
                    Debug.LogError("Couldn't load '" + imageFile + "'...");
                    return null;
                }
                var fromPixels = tex.GetPixels32();
                for (var y = 0; y < size.y; ++y)
                {
                    for (var x = 0; x < size.x; ++x)
                    {
                        var from = fromPixels[x + (y * size.x)];
                        if (inferAlpha)
                        {
                            from.a = (byte)Mathf.Max(from.r, from.g, from.b);
                        }
                        voxels.SetVoxel(new Int3(x, y, z), from);
                    }
                }
                ++z;
            }

            voxels.ClearEdges(new Color32(0, 0, 0, 0));
            return VolumeTextureUtils.Color32ArrayToByteArray(voxels.DataArray);
        }

        public static Int3 GetSizeOfVolumeFolder(string folder)
        {
            var images = GetImagesInFolder(folder);

            if (images.Length == 0)
            {
                return Int3.zero;
            }

            var tex = new Texture2D(2, 2);
            bool loaded = tex.LoadImage(FileSystemHelper.ReadBytesFromLocalFile(images.First()));
            Debug.Assert(loaded);
            return new Int3(tex.width, tex.height, images.Length);
        }

        private static bool IsFileAnImage(string file)
        {
            var fileLower = file.ToLower();
            return ValidImageFileExtensions.Any(k => fileLower.EndsWith(k));
        }

        private static string[] GetImagesInFolder(string folder)
        {
            return Directory.GetFiles(folder)
                            .Where(k => IsFileAnImage(k))
                            .OrderBy(k => k.ToLower())
                            .ToArray();
        }
    }
}
#endif

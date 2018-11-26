// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Helper functions for file I/O
    /// </summary>
    public static class FileSystemHelper
    {
        public static void WriteBytesToLocalFile(string filename, byte[] content)
        {
            try
            {
                var fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                var bw = new System.IO.BinaryWriter(fs);
                bw.Write(content);
                bw.Close();
                fs.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error writing to file: " + ex.ToString());
            }
        }

        public static byte[] ReadBytesFromLocalFile(string fullPath)
        {
            var path = fullPath;
            byte[] result = null;
            try
            {
                var fs = new System.IO.FileStream(path, System.IO.FileMode.Open);
                var br = new System.IO.BinaryReader(fs);

                if (fs.Length > int.MaxValue)
                {
                    throw new System.ArgumentOutOfRangeException();
                }

                result = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Read file exception: " + ex.ToString());
            }
            return result;
        }
    }
}
#endif
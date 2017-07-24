// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Helper functions for dealing with volume data
    /// </summary>
    public static class VolumeTextureUtils
    {
        public static byte[] Color32ArrayToByteArray(Color32[] vals)
        {
            var result = new byte[vals.Length * 4];
            for (var i = 0; i < vals.Length; ++i)
            {
                var v = vals[i];
                var ndx = (i * 4);
                result[ndx + 0] = v.r;
                result[ndx + 1] = v.g;
                result[ndx + 2] = v.b;
                result[ndx + 3] = v.a;
            }
            return result;
        }

        public static Color32[] ByteArrayToColor32Array(byte[] data, Int3 volumeSize, Int3 volumeSizePow2)
        {
            if (data == null)
            {
                throw new NullReferenceException();
            }

            var colors = new Color32[volumeSizePow2.sqrMagnitude];

            Int3 n;

            for (n.z = 0; n.z < volumeSize.z; ++n.z)
            {
                for (n.y = 0; n.y < volumeSize.y; ++n.y)
                {
                    for (n.x = 0; n.x < volumeSize.x; ++n.x)
                    {
                        var colorIndex = MathExtensions.CubicToLinearIndex(n, volumeSizePow2);
                        var dataOffset = MathExtensions.CubicToLinearIndex(n, volumeSize) * 4;

                        byte r = data[dataOffset];
                        byte g = data[dataOffset + 1];
                        byte b = data[dataOffset + 2];
                        byte a = data[dataOffset + 3];

                        var col = new Color32(r, g, b, a);

                        colors[colorIndex] = Color32Extensions.PremultiplyAlpha(col);
                    }
                }
            }

            return colors;
        }

        public static Texture3D BuildTexture(byte[] data, Int3 volumeSize, Int3 volumeSizePow2)
        {
            var colorData = VolumeTextureUtils.ByteArrayToColor32Array(data, volumeSize, volumeSizePow2);

            var tex = new Texture3D(volumeSizePow2.x, volumeSizePow2.y, volumeSizePow2.z, TextureFormat.RGBA4444, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            tex.SetPixels32(colorData);
            tex.Apply();

            return tex;
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for Unity's Color32 struct
    /// </summary>
    public static class Color32Extensions
    {
        public static Color PremultiplyAlpha(Color col)
        {
            col.r *= col.a;
            col.g *= col.a;
            col.b *= col.a;

            return col;
        }

        public static Color32 PremultiplyAlpha(Color32 col)
        {
            Color floatCol = col;
            return (Color32)PremultiplyAlpha(floatCol);
        }

        /// <summary>
        /// Creates a Color from a hexcode string
        /// </summary>
        public static Color ParseHexcode(string hexstring)
        {
            if (hexstring.StartsWith("#"))
            {
                hexstring = hexstring.Substring(1);
            }

            if (hexstring.StartsWith("0x"))
            {
                hexstring = hexstring.Substring(2);
            }

            if (hexstring.Length == 6)
            {
                hexstring += "FF";
            }

            if (hexstring.Length != 8)
            {
                throw new ArgumentException(string.Format("{0} is not a valid color string.", hexstring));
            }

            byte r = byte.Parse(hexstring.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hexstring.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexstring.Substring(4, 2), NumberStyles.HexNumber);
            byte a = byte.Parse(hexstring.Substring(6, 2), NumberStyles.HexNumber);

            const float maxRgbValue = 255;
            Color c = new Color(r / maxRgbValue, g / maxRgbValue, b / maxRgbValue, a / maxRgbValue);
            return c;
        }
    }
}
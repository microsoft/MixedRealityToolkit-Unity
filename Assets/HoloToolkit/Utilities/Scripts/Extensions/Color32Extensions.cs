// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extension methods for Unity's Color32 class
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
    }
}
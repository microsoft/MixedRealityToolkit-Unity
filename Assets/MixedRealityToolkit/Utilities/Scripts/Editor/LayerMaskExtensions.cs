// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.EditorScript
{
    /// <summary>
    /// Extensions for the UnityEngine.LayerMask class.
    /// </summary>
    public static class LayerMaskExtensions
    {
        public const int LayerCount = 32;

        private static string[] layerMaskNames;
        public static string[] LayerMaskNames
        {
            get
            {
                if (layerMaskNames == null)
                {
                    layerMaskNames = new string[LayerCount];
                    for (int layer = 0; layer < LayerCount; ++layer)
                    {
                        layerMaskNames[layer] = LayerMask.LayerToName(layer);
                    }
                }

                return layerMaskNames;
            }
        }

        public static string GetDisplayString(this LayerMask layerMask)
        {
            StringBuilder stringBuilder = null;
            for (int layer = 0; layer < LayerCount; ++layer)
            {
                if ((layerMask & (1 << layer)) != 0)
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder();
                    }
                    else
                    {
                        stringBuilder.Append(" | ");
                    }

                    stringBuilder.Append(LayerMaskNames[layer]);
                }
            }

            return stringBuilder == null ? "None" : stringBuilder.ToString();
        }
    }
}
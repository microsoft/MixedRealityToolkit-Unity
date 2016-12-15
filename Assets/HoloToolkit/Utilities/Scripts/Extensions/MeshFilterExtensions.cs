// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class MeshFilterExtensions
    {

        public static void ChangeColor(this MeshFilter meshFilter, Color color)
        {
            int vertexCount = meshFilter.mesh.vertexCount;
            Color[] meshColors = new Color[vertexCount];

            for (int i = vertexCount; --i >= 0;)
                meshColors[i] = color;

            meshFilter.mesh.colors = meshColors;
        }

    }
}

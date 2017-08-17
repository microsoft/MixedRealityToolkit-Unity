// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public static class EditorUtils
    {
        /// <summary>
        /// Deletes all objects in the scene
        /// </summary>
        public static void ClearScene()
        {
            foreach (var transform in Object.FindObjectsOfType<Transform>().Select(t => t.root).Distinct().ToList())
            {
                Object.DestroyImmediate(transform.gameObject);
            }
        }
    }
}

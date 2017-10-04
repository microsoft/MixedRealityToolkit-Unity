//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// This is a set of Static Extensions for determining if an object is a prefab or in the scene
    /// </summary>
    static class StaticExtensions 
    {
        /// <summary>
        /// Function to check at runtime if the transform is an instance or a prefab
        /// </summary>
        /// <param name="This">
        /// A <see cref="Transform"/> for the object to validate as prefab.
        /// </param>
        internal static bool IsPrefab(GameObject go)
        {
            var TempObject = new GameObject();
            try
            {
                TempObject.transform.parent = go.transform.parent;

                var OriginalIndex = go.transform.GetSiblingIndex();

                go.transform.SetSiblingIndex(int.MaxValue);
                if (go.transform.GetSiblingIndex() == 0) return true;

                go.transform.SetSiblingIndex(OriginalIndex);
                return false;
            }
            finally
            {
                Object.DestroyImmediate(TempObject);
            }
        }
    }
}

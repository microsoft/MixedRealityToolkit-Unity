// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Extension methods for Unity's GameObject class
    /// </summary>
    public static class GameObjectExtensions
    {
        public static string GetFullPath(this GameObject go)
        {
            if (go.transform.parent == null)
            {
                return go.name;
            }

            return go.transform.parent.gameObject.GetFullPath() + "/" + go.name;
        }
    }
}
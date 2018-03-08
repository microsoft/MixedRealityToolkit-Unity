// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    public static class SerializedPropertyExtensions
    {
        public static void SetObjects<T>(this SerializedProperty property, IList<T> objects) where T : Object
        {
            property.arraySize = objects.Count;
            for (var i = 0; i < objects.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
            }
        }
    }
}

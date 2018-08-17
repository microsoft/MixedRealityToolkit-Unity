// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;


namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
{
    [CustomEditor( typeof(BaseCollection), true )]
    public class CollectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default
            base.OnInspectorGUI();

            // Place the button at the bottom
            BaseCollection collection = (BaseCollection)target;
            if (GUILayout.Button("Update Collection"))
            {
                collection.UpdateCollection();
            }
        }
    }
}
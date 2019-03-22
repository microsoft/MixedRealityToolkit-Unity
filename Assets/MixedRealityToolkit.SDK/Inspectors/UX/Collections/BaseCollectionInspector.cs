// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor( typeof(BaseObjectCollection), true )]
    public class BaseCollectionInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default
            base.OnInspectorGUI();

            // Place the button at the bottom
            BaseObjectCollection collection = (BaseObjectCollection)target;
            if (GUILayout.Button("Update Collection"))
            {
                collection.UpdateCollection();
            }
        }
    }
}
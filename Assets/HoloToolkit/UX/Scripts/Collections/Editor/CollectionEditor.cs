//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEditor;
using HoloToolkit.Unity.Collections;

[CustomEditor(typeof(ObjectCollection))]
public class CollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default
        base.OnInspectorGUI();

        // Place the button at the bottom
        ObjectCollection myScript = (ObjectCollection)target;
        if(GUILayout.Button("Update Collection"))
        {
            myScript.UpdateCollection();
        }
    }
}

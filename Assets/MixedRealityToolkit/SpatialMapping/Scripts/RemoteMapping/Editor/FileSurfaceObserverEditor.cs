// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.SpatialMapping.RemoteMapping.EditorScript
{
    [CustomEditor(typeof(FileSurfaceObserver))]
    public class FileSurfaceObserverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Quick way for the user to get access to the room file location.
            if (GUILayout.Button("Open File Location"))
            {
                System.Diagnostics.Process.Start(MeshSaver.MeshFolderName);
            }
        }
    }
}
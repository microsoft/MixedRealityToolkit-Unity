// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEditor;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    [CustomEditor(typeof(MarkerGeneration3D), true)]
    public class MarkerGeneration3DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Generate"))
            {
                MarkerGeneration3D cubeToSphere = (MarkerGeneration3D)target;
                cubeToSphere.Generate();
            }
        }
    }
}

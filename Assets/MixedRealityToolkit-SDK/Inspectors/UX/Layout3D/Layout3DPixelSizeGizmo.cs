// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Layout3D
{
    [CustomEditor(typeof(Layout3DPixelSize))]
    public class Layout3DPixelSizeGizmo : Editor
    {
        public void OnSceneGUI()
        {
            Layout3DPixelSize pixelSize = (Layout3DPixelSize)target;

            float size = HandleUtility.GetHandleSize(pixelSize.transform.position) * 1f;

            EditorGUI.BeginChangeCheck();
            Vector3 itemSize = Handles.ScaleHandle(pixelSize.GetSize(), pixelSize.transform.position, pixelSize.transform.rotation, size);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change ItemSize Value");
                pixelSize.SetSize(itemSize);
                
            }
        }
    }
}

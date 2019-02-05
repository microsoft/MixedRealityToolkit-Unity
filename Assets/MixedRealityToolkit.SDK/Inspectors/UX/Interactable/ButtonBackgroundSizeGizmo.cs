// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Layout
{
    [CustomEditor(typeof(ButtonBackgroundSize))]
    public class ButtonBackgroundSizeGizmo : Editor
    {
        public void OnSceneGUI()
        {
            ButtonBackgroundSize pixelSize = (ButtonBackgroundSize)target;

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

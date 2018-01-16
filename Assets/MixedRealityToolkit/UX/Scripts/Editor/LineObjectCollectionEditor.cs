// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Lines;
using UnityEditor;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(LineObjectCollection))]
    public class LineObjectCollectionEditor : Editor
    {
        public void OnSceneGUI()
        {
            LineObjectCollection loc = (LineObjectCollection)target;

            for (int i = 0; i < loc.Objects.Count; i++)
            {
                if (loc.Objects[i] != null)
                {
                    UnityEditor.Handles.Label(loc.Objects[i].position, "Index: " + i.ToString("000") + "\nOffset: " + loc.GetOffsetFromObjectIndex(i).ToString("00.00"));
                }
            }
        }
    }
}
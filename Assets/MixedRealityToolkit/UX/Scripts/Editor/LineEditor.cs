// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Lines;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(Line))]
    public class LineEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            LineBase line = (LineBase)target;
            line.FirstPoint = SquareMoveHandle(line.FirstPoint);
            line.LastPoint = SquareMoveHandle(line.LastPoint);
        }
    }
}
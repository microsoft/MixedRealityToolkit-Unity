// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Lines;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(Parabola))]
    public class ParabolaEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            Parabola line = (Parabola)target;

            line.FirstPoint = SquareMoveHandle(line.FirstPoint);
            line.LastPoint = SquareMoveHandle(line.LastPoint);
            // Draw a handle for the parabola height
            line.Height = AxisMoveHandle(line.FirstPoint, line.transform.up, line.Height);
        }
    }
}
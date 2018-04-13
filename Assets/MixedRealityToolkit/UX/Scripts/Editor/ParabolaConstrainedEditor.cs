// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Lines;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(ParabolaConstrained))]
    public class ParabolaConstrainedEditor : LineBaseEditor
    {
        protected override void DrawCustomSceneGUI()
        {
            base.DrawCustomSceneGUI();

            ParabolaConstrained line = (ParabolaConstrained)target;

            line.FirstPoint = SquareMoveHandle(line.FirstPoint);
            line.LastPoint = SquareMoveHandle(line.LastPoint);
            // Draw a handle for the parabola height
            line.Height = AxisMoveHandle(line.FirstPoint, line.transform.up, line.Height);
        }
    }
}
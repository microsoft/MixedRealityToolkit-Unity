// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Lines;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(Rectangle))]
    public class RectangleEditor : LineBaseEditor
    {
        // Use FromSource step mode for rectangles since interpolated looks weird
        protected override StepModeEnum EditorStepMode { get { return StepModeEnum.FromSource; } }
    }

}
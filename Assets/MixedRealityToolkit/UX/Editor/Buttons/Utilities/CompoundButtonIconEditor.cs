// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using MixedRealityToolkit.UX.Buttons.Utilities;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(CompoundButtonIcon))]
    public class CompoundButtonIconEditor : MRTKEditor
    {
        protected override void DrawCustomFooter()
        {
            CompoundButtonIcon iconButton = (CompoundButtonIcon)target;
            if (iconButton != null && iconButton.Profile != null)
            {
                iconButton.IconName = iconButton.Profile.DrawIconSelectField(iconButton.IconName);
            }

        }
    }
}
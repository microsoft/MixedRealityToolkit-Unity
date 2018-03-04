// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using MixedRealityToolkit.UX.Buttons.Profiles;
using UnityEngine;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(ButtonIconProfileTexture))]
    public class ButtonIconProfileTextureEditor : ProfileInspector
    {
        private static float textureSize = 50f;

        protected override void DrawCustomFooter()
        {

            ButtonIconProfileTexture iconProfile = (ButtonIconProfileTexture)target;
            UnityEditor.EditorGUILayout.LabelField("Custom Icons", UnityEditor.EditorStyles.boldLabel);

            for (int i = 0; i < iconProfile.CustomIcons.Length; i++)
            {
                Texture2D icon = iconProfile.CustomIcons[i];
                icon = (Texture2D)UnityEditor.EditorGUILayout.ObjectField(icon != null ? icon.name : "(Empty)", icon, typeof(Texture2D), false, GUILayout.MaxHeight(textureSize));
                iconProfile.CustomIcons[i] = icon;
            }

            if (GUILayout.Button("Add custom icon"))
            {
                System.Array.Resize<Texture2D>(ref iconProfile.CustomIcons, iconProfile.CustomIcons.Length + 1);
            }
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Buttons.EditorScript
{
    public class CompoundButtonSaveInterceptor : UnityEditor.AssetModificationProcessor
    {
        public static string[] OnWillSaveAssets(string[] paths)
        {
            // Tell our mesh buttons and icons to switch over to non-instanced materials
            // This will prevent them from saving the scene with broken material links
            // Materials will be re-instanced immediately afterward so this should have no effect 

            CompoundButtonMesh[] meshButtons = Object.FindObjectsOfType<CompoundButtonMesh>();
            foreach (CompoundButtonMesh meshButton in meshButtons)
            {
                meshButton.OnWillSaveScene();
            }

            CompoundButtonIcon[] iconButtons = Object.FindObjectsOfType<CompoundButtonIcon>();
            foreach (CompoundButtonIcon iconButton in iconButtons)
            {
                iconButton.OnWillSaveScene();
            }

            return paths;
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class CompoundButtonSaveInterceptor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// Tell our mesh buttons and icons to switch over to non-instanced materials
        /// This will prevent them from saving the scene with broken material links
        /// Materials will be re-instanced immediately afterward so this should have no effect 
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string[] OnWillSaveAssets(string[] paths)
        {
            CompoundButtonMesh[] meshButtons = Object.FindObjectsOfType<CompoundButtonMesh>();
            for (var i = 0; i < meshButtons.Length; i++)
            {
                meshButtons[i].OnWillSaveScene();
            }

            CompoundButtonIcon[] iconButtons = Object.FindObjectsOfType<CompoundButtonIcon>();
            for (var i = 0; i < iconButtons.Length; i++)
            {
                iconButtons[i].OnWillSaveScene();
            }

            return paths;
        }
    }
}
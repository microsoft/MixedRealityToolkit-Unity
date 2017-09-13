// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Editor for FastConfigurable2Sided shader
    /// </summary>
    public class FastConfigurable2SidedGUI : FastConfigurableGUI
    {
        protected override void ShowOutputConfigurationGUI(MaterialEditor matEditor)
        {
            ShaderGUIUtils.BeginHeader("Output Configuration");
            {
                matEditor.ShaderProperty(zTest, Styles.zTest);
                matEditor.ShaderProperty(zWrite, Styles.zWrite);
                matEditor.ShaderProperty(colorWriteMask, Styles.colorWriteMask);
                matEditor.RenderQueueField();
            }
            ShaderGUIUtils.EndHeader();
        }

        protected override void CacheOutputConfigurationProperties(MaterialProperty[] props)
        {
            zTest = FindProperty("_ZTest", props);
            zWrite = FindProperty("_ZWrite", props);
            colorWriteMask = FindProperty("_ColorWriteMask", props);
        }
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using TMPro.EditorUtilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom TMP_SDFShaderGUI inspector for the "Mixed Reality Toolkit/TextMeshPro" shader.
    /// Adds the ability to change the depth write mode, and a warning about depth write
    /// when depth buffer sharing is enabled.
    /// </summary>
    public class MixedRealityTextMeshProShaderGUI : TMP_SDFShaderGUI
    {
        protected override void DoGUI()
        {
            BeginPanel("Mode", true);
            DoModePanel();
            EndPanel();

            base.DoGUI();
        }

        protected void DoModePanel()
        {
            EditorGUI.indentLevel += 1;

            var depthWrite = FindProperty("_ZWrite", m_Properties, false);

            if (depthWrite != null)
            {
                m_Editor.ShaderProperty(depthWrite, depthWrite.displayName);

                if (depthWrite.floatValue.Equals(0.0f))
                {
                    if (MixedRealityToolkitShaderGUIUtilities.DisplayDepthWriteWarning(m_Editor))
                    {
                        depthWrite.floatValue = 1.0f;
                    }
                }
            }

            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
        }
    }
}

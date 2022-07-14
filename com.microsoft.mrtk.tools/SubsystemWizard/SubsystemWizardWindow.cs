// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Tools
{
    /// <summary>
    /// Editor window to guide developers through the process of creating a
    /// Unity XR Subsystem for use with MRTK3.
    /// </summary>
    public class SubsystemWizardWindow : EditorWindow
    {
        private static SubsystemWizardWindow window = null;
        private static SubsystemGenerator subsystemGenerator = new SubsystemGenerator();

        private static readonly Vector2 WindowSizeWithoutLogo = new Vector2(600, 250);
        private static readonly Vector2 WindowSizeWithLogo = new Vector2(600, 320);

        [MenuItem("Mixed Reality/MRTK3/Utilities/Subsystem Wizard...", false)]
        private static void Init()
        {
            if (window != null)
            {
                window.Show();
                return;
            }

            window = GetWindow<SubsystemWizardWindow>();
            window.titleContent = new GUIContent("MRTK3 Subsystem Wizard", EditorGUIUtility.IconContent("d_CustomTool").image); ;

            if (MixedRealityInspectorUtility.IsMixedRealityToolkitLogoAssetPresent())
            {
                window.minSize = WindowSizeWithLogo;
            }
            else
            {
                window.minSize = WindowSizeWithoutLogo;
            }
        }

        private void OnGUI()
        {
            if (window == null)
            {
                Init();
            }

            using (new EditorGUILayout.VerticalScope())
            {
                RenderCommonElements();

                switch (subsystemGenerator.State)
                {
                    case SubsystemWizardState.Start:
                        RenderWizardStartPage();
                        break;

                    case SubsystemWizardState.PreGenerate:
                        RenderWizardPreGeneratePage();
                        break;

                    case SubsystemWizardState.Complete:
                        RenderWizardCompletePage();
                        break;

                    default:
                        // todo: unknown / unexpected state
                        break;
                }

                // Do not crowd the bottom of the window.
                EditorGUILayout.Space(12);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderCommonElements()
        {
            EditorGUILayout.Space(2);
            if (!MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo())
            {
                // Only add additional space if the text fallback is used in RenderMixedRealityToolkitLogo().
                EditorGUILayout.Space(3);
            }
            EditorGUILayout.LabelField(
                "MRTK3 Subsystem Wizard",
                MixedRealityInspectorUtility.ProductNameStyle);
            EditorGUILayout.Space(9);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                InspectorUIUtility.RenderDocumentationButton("https://aka.ms/MRTK3"); // todo
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space(12);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderWizardStartPage()
        {
            bool hasErrors = false;

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Welcome to the MRTK3 Subsystem Wizard!", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("This tool will guide you through the creation of a new subsystem to extend the functionality of MRTK3.", EditorStyles.boldLabel);

                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Please enter a name for the new subsystem.");
                subsystemGenerator.SubsystemName = EditorGUILayout.TextField(
                    "Subsystem name",
                    subsystemGenerator.SubsystemName);
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("In which namespace should the new subsystem be contained?");
                subsystemGenerator.SubsystemNamespace = EditorGUILayout.TextField("Namespace", subsystemGenerator.SubsystemNamespace);
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Subsystems can optionally support configuration via the MRTK3 project settings.");
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;
                subsystemGenerator.CreateConfiguration = EditorGUILayout.Toggle("Add subsystem configuration", subsystemGenerator.CreateConfiguration);
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.Space(4);
                GUILayout.FlexibleSpace();
                
                if (hasErrors)
                {
                    // todo RenderErrorLog();
                }
            }

            using (new EditorGUI.DisabledGroupScope(hasErrors))
            {
                if (GUILayout.Button("Next"))
                {
                    subsystemGenerator.State = SubsystemWizardState.PreGenerate;
                }
            }
        }

        private void RenderWizardPreGeneratePage()
        {
            bool hasErrors = false;

            using (new EditorGUI.IndentLevelScope())
            {
                // todo: factor out the folder name
                EditorGUILayout.LabelField($"The new subsystem will be created in your project's MRTK.Generated/{subsystemGenerator.SubsystemName} folder.", EditorStyles.boldLabel);

                EditorGUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem interface:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.InterfaceName}.cs");
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem class:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.SubsystemName}.cs");
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem descriptor:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.DescriptorName}.cs");
                }
                if (subsystemGenerator.CreateConfiguration)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Configuration class:", GUILayout.Width(160));
                        EditorGUILayout.LabelField($"{subsystemGenerator.ConfigurationName}.cs");
                    }
                }
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Please click `Next` to continue or `Go back` to make changes.");
                GUILayout.FlexibleSpace();

                if (hasErrors)
                {
                    // todo RenderErrorLog();
                }
            }

            using (new EditorGUI.DisabledGroupScope(hasErrors))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Go back"))
                    {
                        subsystemGenerator.State = SubsystemWizardState.Start;
                    }

                    if (GUILayout.Button("Next"))
                    {
                        // todo
                        List<string> errors = new List<string>();

                        // todo: need to capture error(s)
                        if (!subsystemGenerator.Generate(errors));

                        if (errors.Count == 0)
                        {
                            subsystemGenerator.State = SubsystemWizardState.Complete;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderWizardCompletePage()
        {
            EditorGUILayout.LabelField($"Your subsystem has been successfully created.");

            EditorGUILayout.Space(6);
            // todo: next steps
            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Start Over"))
                {
                    subsystemGenerator.Reset();                    
                }

                if (GUILayout.Button("Close"))
                {
                    subsystemGenerator.Reset();
                    window.Close();
                }
            }
        }
    }
}

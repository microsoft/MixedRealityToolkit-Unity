// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        /// <param name="errors"></param>
        private void RenderErrorLog(List<string> errors)
        {
            EditorGUILayout.LabelField("Errors");
            EditorGUILayout.Space(4);

            StringBuilder sb = new StringBuilder();

            foreach (string s in errors)
            {
                sb.AppendLine(s); 
            }

            EditorGUILayout.HelpBox(sb.ToString(), MessageType.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderWizardStartPage()
        {
            List<string> errors = new List<string>();

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

                // Validate the subsystem name.
                if (!subsystemGenerator.ValidateSubsystemName(
                    subsystemGenerator.SubsystemName,
                    out string error))
                {
                    errors.Add(error);
                }

                // Validate the namespace.
                if (!subsystemGenerator.ValidateNamespace(
                    subsystemGenerator.SubsystemNamespace,
                    out error))
                {
                    errors.Add(error);
                }

                if (errors.Count > 0)
                {
                    RenderErrorLog(errors);
                }

                GUILayout.FlexibleSpace();
            }

            using (new EditorGUI.DisabledGroupScope(errors.Count > 0))
            {
                if (GUILayout.Button("Next"))
                {
                    subsystemGenerator.State = SubsystemWizardState.PreGenerate;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderWizardPreGeneratePage()
        {
            List<string> errors = new List<string>();

            // Validate the tempate files we will use,
            subsystemGenerator.ValidateTemplates(
                errors,
                out FileInfo descriptorTemplate,
                out FileInfo interfaceTemplate,
                out FileInfo classTemplate,
                out FileInfo configTemplate);

            using (new EditorGUI.IndentLevelScope())
            {
                // todo: factor out the folder name
                EditorGUILayout.LabelField($"The new subsystem will be created in your project's MRTK.Generated/{subsystemGenerator.SubsystemName} folder.", EditorStyles.boldLabel);

                EditorGUILayout.Space(6);
                // todo: if !skip
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem interface:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.InterfaceName}.cs");
                }
                // todo: if !skip
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem class:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.SubsystemName}.cs");
                }
                // todo: if !skip
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

                if (errors.Count > 0)
                {
                    RenderErrorLog(errors);
                }

                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Go back"))
                {
                    subsystemGenerator.State = SubsystemWizardState.Start;
                }

                using (new EditorGUI.DisabledGroupScope(errors.Count > 0))
                {
                    if (GUILayout.Button("Next"))
                    {
                        try
                        {
                            subsystemGenerator.Generate(
                                descriptorTemplate,
                                interfaceTemplate,
                                classTemplate,
                                configTemplate);
                            subsystemGenerator.State = SubsystemWizardState.Complete;
                        }
                        catch (Exception e)
                        {
                            EditorUtility.DisplayDialog(
                                "MRTK3 Subsystem Wizard",
                                $"Unable to create {subsystemGenerator.SubsystemName} - {e.Message} ({e.GetType()})",
                                "Ok");
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
            EditorGUILayout.LabelField($"Your new subsystem {subsystemGenerator.SubsystemName} has been successfully created.");

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

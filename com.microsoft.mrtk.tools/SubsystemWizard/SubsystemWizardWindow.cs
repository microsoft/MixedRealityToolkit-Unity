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
    internal class SubsystemWizardWindow : EditorWindow
    {
        private static SubsystemWizardWindow window = null;
        private SubsystemGenerator subsystemGenerator = null;

        private static readonly Vector2 WindowSizeWithoutLogo = new Vector2(600, 510);
        private static readonly Vector2 WindowSizeWithLogo = new Vector2(600, 580);

        [MenuItem("Mixed Reality/MRTK3/Utilities/Subsystem Wizard...", false)]
        private static void Init()
        {
            if (window != null)
            {
                window.Show();
                return;
            }

            window = GetWindow<SubsystemWizardWindow>();
            window.subsystemGenerator = new SubsystemGenerator();
            window.titleContent = new GUIContent("MRTK3 Subsystem Wizard", EditorGUIUtility.IconContent("d_CustomTool").image); ;

            if (InspectorUIUtility.IsMixedRealityToolkitLogoAssetPresent())
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
                        // Unknown / unexpected state
                        Debug.LogWarning($"[Subsystem Wizard] Unknown state: {subsystemGenerator.State}");
                        break;
                }

                // Do not crowd the bottom of the window.
                EditorGUILayout.Space(12);
            }
        }

        /// <summary>
        /// Renders common wizard UI elements.
        /// </summary>
        private void RenderCommonElements()
        {
            EditorGUILayout.Space(2);
            if (!InspectorUIUtility.RenderMixedRealityToolkitLogo())
            {
                // Only add additional space if the text fallback is used in RenderMixedRealityToolkitLogo().
                EditorGUILayout.Space(3);
            }
            EditorGUILayout.LabelField(
                "MRTK3 Subsystem Wizard",
                MRTKEditorStyles.ProductNameStyle);
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
        /// Renders the contents of the provided errors list.
        /// </summary>
        /// <param name="errors">The collection of errors to be rendered.</param>
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
        /// Renders the start page of the wizard. This is the page that collects the
        /// user's desired names.
        /// </summary>
        private void RenderWizardStartPage()
        {
            List<string> errors = new List<string>();

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("Welcome to the MRTK3 Subsystem Wizard!", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("This tool will guide you through the creation of a new subsystem to extend the functionality of MRTK3.");

                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Please enter your organization or project name.", EditorStyles.boldLabel);
                subsystemGenerator.OrganizationName = EditorGUILayout.TextField(
                    "Organization name", subsystemGenerator.OrganizationName);
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Please enter the name for the subsystem base class.", EditorStyles.boldLabel);
                subsystemGenerator.BaseClassName = EditorGUILayout.TextField(
                    "Base class name", subsystemGenerator.BaseClassName);
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("Subsystems can optionally support configuration via the MRTK3 project settings.", EditorStyles.boldLabel);
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;
                subsystemGenerator.CreateConfiguration = EditorGUILayout.Toggle("Add subsystem configuration",
                    subsystemGenerator.CreateConfiguration);
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.Space(6);

                EditorGUILayout.LabelField("Generated names", EditorStyles.boldLabel);
                using (new EditorGUI.IndentLevelScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Namespace: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(subsystemGenerator.SubsystemNamespace);
                    }
                    EditorGUILayout.Space(4);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Implementation class: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(subsystemGenerator.SubsystemName);
                    }
                    EditorGUILayout.Space(4);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Display name: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(subsystemGenerator.DisplayName);
                    }
                    EditorGUILayout.Space(6);
                }

                // Validate the namespace.
                if (!subsystemGenerator.ValidateNamespace(out string error))
                {
                    errors.Add(error);
                }

                // Validate the subsystem base class name.
                if (!subsystemGenerator.ValidateBaseClassName(out error))
                {
                    errors.Add(error);
                }

                // Validate the subsystem name.
                if (!subsystemGenerator.ValidateSubsystemName(out error))
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
        /// Renders the wizard ui indicating that it is ready to generate the user's
        /// subsystem. It also provides the option to skip generating select files.
        /// </summary>
        private void RenderWizardPreGeneratePage()
        {
            List<string> errors = new List<string>();

            // Validate the template files we will use,
            subsystemGenerator.ValidateTemplates(
                errors,
                out FileInfo descriptorTemplate,
                out FileInfo interfaceTemplate,
                out FileInfo baseClassTemplate,
                out FileInfo derivedClassTemplate,
                out FileInfo configTemplate,
                out FileInfo applyConfigTemplate);

            using (new EditorGUI.IndentLevelScope())
            {
                if (subsystemGenerator.DontCreateBaseClass ||
                    subsystemGenerator.DontCreateImplementationClass ||
                    subsystemGenerator.DontCreateDescriptor ||
                    subsystemGenerator.DontCreateInterface)
                {
                    EditorGUILayout.HelpBox(
                        "Skipping generation of one or more files may result in compilation errors, such as one or more missing types.",
                        MessageType.Warning);
                }

                EditorGUILayout.LabelField(
                    $"The new subsystem will be created in your project's MRTK.Generated/{subsystemGenerator.SubsystemName} folder.",
                    EditorStyles.boldLabel);

                EditorGUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem class:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.SubsystemName}.cs", GUILayout.Width(400));
                    subsystemGenerator.DontCreateImplementationClass = EditorGUILayout.ToggleLeft(
                        "Skip",
                        subsystemGenerator.DontCreateImplementationClass);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem interface:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.InterfaceName}.cs", GUILayout.Width(400));
                    subsystemGenerator.DontCreateInterface = EditorGUILayout.ToggleLeft(
                        "Skip",
                        subsystemGenerator.DontCreateInterface);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem base class:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.BaseClassName}.cs", GUILayout.Width(400));
                    subsystemGenerator.DontCreateBaseClass   = EditorGUILayout.ToggleLeft(
                        "Skip",
                        subsystemGenerator.DontCreateBaseClass);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Subsystem descriptor:", GUILayout.Width(160));
                    EditorGUILayout.LabelField($"{subsystemGenerator.DescriptorName}.cs", GUILayout.Width(400));
                    subsystemGenerator.DontCreateDescriptor = EditorGUILayout.ToggleLeft(
                        "Skip",
                        subsystemGenerator.DontCreateDescriptor);
                }
                if (subsystemGenerator.CreateConfiguration)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Configuration class:", GUILayout.Width(160));
                        EditorGUILayout.LabelField($"{subsystemGenerator.ConfigurationName}.cs", GUILayout.Width(400));
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
                                baseClassTemplate,
                                derivedClassTemplate,
                                configTemplate,
                                applyConfigTemplate);
                            subsystemGenerator.State = SubsystemWizardState.Complete;
                        }
                        catch (Exception e)
                        {
                            EditorUtility.DisplayDialog(
                                "MRTK3 Subsystem Wizard",
                                $"Unable to create {subsystemGenerator.SubsystemName}\n\n{e.Message} ({e.GetType()})",
                                "Ok");
                        }
                    }
                }
            }
        }

        private Vector2 scrollPos;

        /// <summary>
        /// Renders the wizard complete page and provides users with recommended next steps.
        /// </summary>
        private void RenderWizardCompletePage()
        {
            StringBuilder sb = new StringBuilder();
            int step = 1;
            sb.AppendLine($" {step}. In the Project view, navigate to Assets/MRTK.Generated/{subsystemGenerator.SubsystemName}");
            step++;
            sb.AppendLine($" {step}. Open {subsystemGenerator.DescriptorName}.cs");
            step++;
            sb.AppendLine($" {step}. Define subsystem specific properties in the {subsystemGenerator.BaseClassName}CInfo class");
            step++;
            sb.AppendLine($" {step}. Add and initialize subsystem specific properties in the {subsystemGenerator.DescriptorName} class");
            step++;
            sb.AppendLine($" {step}. Open {subsystemGenerator.InterfaceName}.cs");
            step++;
            sb.AppendLine($" {step}. Define subsystem specific properties and/or methods");
            step++;
            sb.AppendLine($" {step}. Open {subsystemGenerator.BaseClassName}.cs");
            step++;
            sb.AppendLine($" {step}. Implement {subsystemGenerator.InterfaceName} in the abstract Provider class");
            step++;
            sb.AppendLine($" {step}. Implement {subsystemGenerator.InterfaceName} in the {subsystemGenerator.BaseClassName} class");
            step++;
            sb.AppendLine($" {step}. Open {subsystemGenerator.SubsystemName}.cs");
            step++;
            sb.AppendLine($" {step}. Implement {subsystemGenerator.InterfaceName} in the {subsystemGenerator.SubsystemName}Provider class");
            step++;
            sb.AppendLine($" {step}. Implement {subsystemGenerator.InterfaceName} in the {subsystemGenerator.SubsystemName} class");
            step++;
            if (subsystemGenerator.CreateConfiguration)
            {
                sb.AppendLine($" {step}. Open {subsystemGenerator.ConfigurationName}.cs");
                step++;
                sb.AppendLine($" {step}. Add subsystem configuration properties");
                step++;
                sb.AppendLine($" {step}. Return to {subsystemGenerator.SubsystemName}.cs");
                step++;
                sb.AppendLine($" {step}. Initialize the subsystem configuration");
                step++;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField($"Your new subsystem {subsystemGenerator.SubsystemName} has been successfully created.");                
                EditorGUILayout.Space(6);

                EditorGUILayout.LabelField("Next steps", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(
                    scrollPos,
                    GUILayout.Height(300));
                EditorGUILayout.TextArea(
                    sb.ToString(),
                    GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space(4);
                if (GUILayout.Button("Enable your subsystem"))
                {
                    SettingsService.OpenProjectSettings("MRTK3");
                }
                GUILayout.FlexibleSpace();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Start Over"))
                {
                    subsystemGenerator.Reset();                    
                }

                if (GUILayout.Button("Close"))
                {
                    window.Close();
                }
            }
        }
    }
}

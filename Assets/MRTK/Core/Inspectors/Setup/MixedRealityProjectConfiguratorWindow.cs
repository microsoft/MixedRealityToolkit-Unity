// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using MRConfig = Microsoft.MixedReality.Toolkit.Utilities.Editor.MixedRealityProjectConfigurator.Configurations;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{

    public class MixedRealityProjectConfiguratorWindow : EditorWindow
    {
        private const float Default_Window_Height = 500.0f;
        private const float Default_Window_Width = 300.0f;
        private const string XRPipelineDocsUrl = "https://aka.ms/mrtkxrpipeline";
        private const string XRSDKUnityDocsUrl = "https://docs.unity3d.com/Manual/configuring-project-for-xr.html";
        private const string MSOpenXRPluginUrl = "https://aka.ms/openxr-unity-install";
        private const string MRTKConfiguratorLogPrefix = "[MRTK Configurator]";
        private const string XRPipelineIntro = "To build applications for AR/VR headsets you need to enable an XR pipeline. ";
        private const string AlternativePipelineText = "\n\nFor more information on alternative pipelines, please click on the Learn More button.";
        private readonly GUIContent ApplyButtonContent = new GUIContent("Apply", "Apply configurations to this Unity Project");
        private readonly GUIContent SkipButtonContent = new GUIContent("Skip This Step", "Skip to the next step");
        private readonly GUIContent LaterButtonContent = new GUIContent("Skip Setup Until Next Session", "Do not show this configurator until next session");
        private readonly GUIContent IgnoreButtonContent = new GUIContent("Always Skip Setup", "This configurator will not show up again unless manually launched by clicking Mixed Reality (menu bar) -> Toolkit -> Utilities -> Configure Project for MRTK\nor this preference modified under Edit -> Project Settings -> Mixed Reality Toolkit");
        private GUIStyle multiLineButtonStyle;

        private static ConfigurationStage CurrentStage
        {
            get => MixedRealityProjectPreferences.ConfiguratorState;
            set => MixedRealityProjectPreferences.ConfiguratorState = value;
        }

        public static MixedRealityProjectConfiguratorWindow Instance { get; private set; }

        public static bool IsOpen => Instance != null;

        private static bool? isTMPEssentialsImported = null;
#if UNITY_2019_3_OR_NEWER
        private static bool? isMRTKExamplesPackageImportedViaUPM = null;
#endif

        private void OnEnable()
        {
            Instance = this;
            EditorApplication.projectChanged += ResetNullableBoolState;
#if UNITY_2019_3_OR_NEWER
            CompilationPipeline.compilationStarted += CompilationPipeline_compilationStarted;
#else
            CompilationPipeline.assemblyCompilationStarted += CompilationPipeline_compilationStarted;
#endif // UNITY_2019_3_OR_NEWER
            MixedRealityProjectConfigurator.SelectedSpatializer = SpatializerUtilities.CurrentSpatializer;
        }

        private void CompilationPipeline_compilationStarted(object obj)
        {
            ResetNullableBoolState();
        }

        private static void ResetNullableBoolState()
        {
#if UNITY_2019_3_OR_NEWER
            isMRTKExamplesPackageImportedViaUPM = null;
#endif
            isTMPEssentialsImported = null;
        }

        [MenuItem("Mixed Reality/Toolkit/Utilities/Configure Project for MRTK", false, 499)]
        private static void ShowWindowFromMenu()
        {
            CurrentStage = ConfigurationStage.Init;
            ShowWindow();
        }

        internal static void ShowWindowOnInit(bool switchToConfigurationStage)
        {
            if (!IsOpen && CurrentStage == ConfigurationStage.Done)
            {
                if (switchToConfigurationStage)
                {
                    CurrentStage = ConfigurationStage.ProjectConfiguration;
                }
                else
                {
                    CurrentStage = ConfigurationStage.Init;
                }
            }

            ShowWindow();
        }

        private static void ShowWindow()
        {
            // There should be only one configurator window open as a "pop-up". If already open, then just force focus on our instance
            if (IsOpen)
            {
                Instance.Focus();
            }
            else
            {
                var window = CreateInstance<MixedRealityProjectConfiguratorWindow>();
                window.titleContent = new GUIContent("MRTK Project Configurator", EditorGUIUtility.IconContent("_Popup").image);
                window.position = new Rect(Screen.width / 2.0f, Screen.height / 2.0f, Default_Window_Height, Default_Window_Width);
                window.ShowUtility();
            }
        }

        private void OnGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
            if (CurrentStage != ConfigurationStage.Done)
            {
                GUILayout.Label("Welcome to MRTK!", MixedRealityStylesUtility.BoldLargeTitleStyle);
                CreateSpace(5);
                EditorGUILayout.LabelField("This configurator will go through some settings to make sure the project is ready for MRTK.", EditorStyles.wordWrappedLabel);
                CreateSpace(20);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
            multiLineButtonStyle = new GUIStyle("button")
            {
                richText = true,
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft
            };


            switch (CurrentStage)
            {
                case ConfigurationStage.Init:
                    RenderXRPipelineSelection();
                    break;
                case ConfigurationStage.SelectXRSDKPlugin:
                    RenderSelectXRSDKPlugin();
                    break;
                case ConfigurationStage.InstallOpenXR:
                    RenderEnableOpenXRPlugin();
                    break;
                case ConfigurationStage.InstallMSOpenXR:
                    RenderEnableMicrosoftOpenXRPlugin();
                    break;
                case ConfigurationStage.InstallBuiltinPlugin:
                    RenderEnableXRSDKBuiltinPlugin();
                    break;
                case ConfigurationStage.ProjectConfiguration:
                    RenderProjectConfigurations();
                    break;
                case ConfigurationStage.ImportTMP:
                    RenderImportTMP();
                    break;
                case ConfigurationStage.ShowExamples:
                    RenderShowUPMExamples();
                    break;
                case ConfigurationStage.Done:
                    RenderConfigurationCompleted();
                    break;
                default:
                    break;
            }
        }

        private void RenderXRPipelineSelection()
        {
            if (!XRSettingsUtilities.XREnabled)
            {
                RenderNoPipeline();
            }
            else if (XRSettingsUtilities.LegacyXREnabled)
            {
                RenderLegacyXRPipelineDetected();
            }
            else
            {
                if (XRSettingsUtilities.MicrosoftOpenXREnabled)
                {
                    RenderMicrosoftOpenXRPipelineDetected();
                }
                else if (XRSettingsUtilities.OpenXREnabled)
                {
                    RenderOpenXRPipelineDetected();
                }
                else
                {
                    RenderXRSDKBuiltinPluginPipelineDetected();
                }
            }
        }

        private void RenderNoPipeline()
        {
            if (!XRSettingsUtilities.LegacyXRAvailable)
            {
#if UNITY_2020_2_OR_NEWER
                CurrentStage = ConfigurationStage.SelectXRSDKPlugin;
#else
                CurrentStage = ConfigurationStage.InstallBuiltinPlugin;
#endif // UNITY_2020_2_OR_NEWER

                Repaint();
            }
            EditorGUILayout.LabelField("XR Pipeline Setting", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
#if !UNITY_2019_3_OR_NEWER
                + $"Unity currently provides the Legacy XR pipeline in this version ({Application.unityVersion}). Please click on the Enable Legacy XR button if you are targeting AR/VR headsets (e.g. HoloLens, Windows Mixed Reality headset, OpenVR headset etc.). "
#else
                + $"Unity currently provides the following pipelines in this version ({Application.unityVersion}). Please choose the one you would like to use. "
#endif // UNITY_2019_3_OR_NEWER
                + "You may also skip this step and configure manually later.", EditorStyles.wordWrappedLabel);
#if !UNITY_2019_3_OR_NEWER
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Enable Legacy XR"))
                {
                    Debug.Log(MRTKConfiguratorLogPrefix + " Enabling Legacy XR for this project. Operation performed is equivalent to toggling on Virual Reality Supported under Edit -> Project Settings -> Player -> XR Settings.");
                    XRSettingsUtilities.LegacyXREnabled = true;
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRPipelineDocsUrl);
                }
            }
#else
            CreateSpace(15);
            
            if (GUILayout.Button("\n<b>Legacy XR (recommended)</b><size=4>\n\n</size>"
            + "Choose this if you want maximum stability and are willing to spend more effort when upgrading the project to Unity 2020. Supports HoloLens and Windows Mixed Reality/OpenVR headsets.\n", multiLineButtonStyle))
            {
                Debug.Log(MRTKConfiguratorLogPrefix + " Enabling Legacy XR for this project. Operation performed is equivalent to toggling on Virual Reality Supported under Edit -> Project Settings -> Player -> XR Settings.");
                XRSettingsUtilities.LegacyXREnabled = true;
            }
            CreateSpace(15);
            if (GUILayout.Button("\n<b>XR SDK/XR Management</b><size=4>\n\n</size>"
            + "Choose this if you want to have a smoother upgrade path to Unity 2020. Supports HoloLens and Windows Mixed Reality/Oculus headsets. The Unity XR Management Plugin will be installed if not already. Note: NOT compatible with Azure Spatial Anchors.\n", multiLineButtonStyle))
            {
                if (!XRSettingsUtilities.XRManagementPresent)
                {
                    Debug.Log(MRTKConfiguratorLogPrefix + " Installing the Unity XR Management Plugin. Operation performed is equivalent to clicking on Install XR Plugin Management under Edit -> Project Settings -> XR Plugin Management (the button only appears when the plugin is not installed).");
                    AddUPMPackage("com.unity.xr.management");
                }
                CurrentStage = ConfigurationStage.InstallBuiltinPlugin;
                Repaint();
            }
            CreateSpace(15);
            if (GUILayout.Button("Learn More"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
#endif // !UNITY_2019_3_OR_NEWER

            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            });
        }

        private void RenderLegacyXRPipelineDetected()
        {
            EditorGUILayout.LabelField("XR Pipeline Setting - LegacyXR in use", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
                + $"\n\nThe LegacyXR pipeline is detected in the project. Please be aware that the LegacyXR pipeline is deprecated in Unity 2019 and is removed in Unity 2020."
                + AlternativePipelineText, EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Next"))
                {
                    CurrentStage = ConfigurationStage.ProjectConfiguration;
                    Repaint();
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRPipelineDocsUrl);
                }
            }
            RenderSetupLaterSection();
        }

        private void RenderMicrosoftOpenXRPipelineDetected()
        {
            EditorGUILayout.LabelField("XR Pipeline Setting - XR SDK with Unity + Microsoft OpenXR plugins in use", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
                + $"\n\nThe XR SDK pipeline with Unity and Microsoft OpenXR plugins are detected in the project. You are good to go."
                + AlternativePipelineText, EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Next"))
                {
                    CurrentStage = ConfigurationStage.ProjectConfiguration;
                    Repaint();
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRPipelineDocsUrl);
                }

            }
            RenderSetupLaterSection();
        }

        private void RenderOpenXRPipelineDetected()
        {
            EditorGUILayout.LabelField("XR Pipeline Setting - XR SDK with Unity OpenXR plugin in use", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
                + $"\n\nThe XR SDK pipeline with Unity OpenXR plugin is detected in the project. You are good to go."
                + $"\n\nNote: If you are targeting HoloLens 2 or HP Reverb G2 headset you need to install the Microsoft OpenXR plugin."
                + AlternativePipelineText, EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Next"))
                {
                    CurrentStage = ConfigurationStage.ProjectConfiguration;
                    Repaint();
                }
                if (GUILayout.Button("Install Microsoft OpenXR plugin..."))
                {
                    CurrentStage = ConfigurationStage.InstallMSOpenXR;
                    Repaint();
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRPipelineDocsUrl);
                }

            }
            RenderSetupLaterSection();
        }

        private void RenderXRSDKBuiltinPluginPipelineDetected()
        {
            EditorGUILayout.LabelField("XR Pipeline Setting - XR SDK with builtin plugin (non-OpenXR) in use", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
                + $"\n\nThe XR SDK pipeline with builtin plugin (non-OpenXR) is detected in the project. You are good to go."
                + AlternativePipelineText, EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Next"))
                {
                    CurrentStage = ConfigurationStage.ProjectConfiguration;
                    Repaint();
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRPipelineDocsUrl);
                }
            }
            RenderSetupLaterSection();
        }

        private void RenderSelectXRSDKPlugin()
        {
            if (XRSettingsUtilities.XRSDKEnabled)
            {
                CurrentStage = ConfigurationStage.Init;
                Repaint();
            }
            EditorGUILayout.LabelField("XR Pipeline Setting - Enabling the XR SDK Pipeline", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(XRPipelineIntro
                + "With the XR SDK pipeline there are two categories of provider plugins:", EditorStyles.wordWrappedLabel);
            CreateSpace(15);

            if (GUILayout.Button("\n<b>Unity OpenXR plugin (recommended)</b><size=4>\n\n</size>"
            + "Choose this if you want to embrace the new industry standard and easily support a wide range of AR/VR devices in the future! Currently officially supports HoloLens 2 and Windows Mixed Reality headsets with other devices coming soon. The Unity OpenXR Plugin will be installed.\n", multiLineButtonStyle))
            {

#if UNITY_2020_2_OR_NEWER
                Debug.Log(MRTKConfiguratorLogPrefix + " Installing the Unity OpenXR Plugin. Operation performed is equivalent to installing the OpenXR plugin manually via Window -> Package Manager -> Packages: Unity Registry.");
                AddUPMPackage("com.unity.xr.openxr");
                CurrentStage = ConfigurationStage.InstallOpenXR;
#endif // UNITY_2020_2_OR_NEWER
                Repaint();
            }
            CreateSpace(15);
            if (GUILayout.Button("\n<b>Built-in Unity plugins (non-OpenXR)</b><size=4>\n\n</size>"
            + "Choose this if your application needs to support platforms beyond HoloLens 2 and Windows Mixed Reality headsets (e.g. Oculus/Magic Leap headsets). The Unity XR Management Plugin will be installed if not already.\n", multiLineButtonStyle))
            {
#if UNITY_2019_3_OR_NEWER
                if (!XRSettingsUtilities.XRManagementPresent)
                {
                    Debug.Log(MRTKConfiguratorLogPrefix + " Installing the Unity XR Management Plugin. Operation performed is equivalent to clicking on Install XR Plugin Management under Edit -> Project Settings -> XR Plugin Management (the button only appears when the plugin is not installed).");
                    AddUPMPackage("com.unity.xr.management");
                }
#endif // UNITY_2019_3_OR_NEWER
                CurrentStage = ConfigurationStage.InstallBuiltinPlugin;
                Repaint();
            }
            CreateSpace(25);
            EditorGUILayout.LabelField("For more information, please click on the Learn More button.", EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            if (GUILayout.Button("Learn More"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
            
            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            });
        }

        private void RenderEnableXRSDKBuiltinPlugin()
        {
            if (XRSettingsUtilities.XRSDKEnabled)
            {
                CurrentStage = ConfigurationStage.Init;
                Repaint();
            }
            EditorGUILayout.LabelField("XR Pipeline Setting - Enabling the XR SDK Pipeline with built-in Plugins (non-OpenXR)", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("To enable the XR SDK pipeline with built-in Plugins (non-OpenXR), follow the steps below:"
            + "\n\n1. Press the Show Settings button."
            + "\n2. In the XR management plug-in window that shows up, switch to the current build target (e.g. UWP, Windows standalone) tab by clicking on the corresponding icon right below the XR Plug-in Management title."
            + "\n3. Check the plugin(s) you want to use based on your target device."
            + "\n\nA new page confirming the setup is successful will be shown in place of this page once you finish the steps."
            + $"\n\nFor more information, please click on the Learn More button. (Only the first three steps are needed if following instructions on the page)", EditorStyles.wordWrappedLabel);
            
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show Settings"))
                {
                    SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
                }
                if (GUILayout.Button("Learn More"))
                {
                    Application.OpenURL(XRSDKUnityDocsUrl);
                }
            }
            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            });
        }

        private void RenderEnableOpenXRPlugin()
        {
            if (XRSettingsUtilities.OpenXREnabled)
            {
                CurrentStage = ConfigurationStage.Init;
                Repaint();
            }
            EditorGUILayout.LabelField("XR Pipeline Setting - Enabling the XR SDK Pipeline with OpenXR", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("To enable the XR SDK pipeline with OpenXR follow the instructions below."
                + "\n\n1. Press the Show Settings button."
                + "\n2. In the XR management plug-in window that shows up, please switch to the correct build target (e.g. UWP, Windows standalone) tab by clicking on the icon(s) right below the XR Plug-in Management title."
                + "\n3. Check the checkbox for OpenXR plugin. A new page confirming the detection of OpenXR will be shown in place of this page once you finish the steps."
                , EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            if (GUILayout.Button("Show XR Plug-in Management Settings"))
            {
                SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
            }
            
            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            });
        }

        private void RenderEnableMicrosoftOpenXRPlugin()
        {
            if (XRSettingsUtilities.MicrosoftOpenXREnabled)
            {
                CurrentStage = ConfigurationStage.Init;
                Repaint();
            }
            EditorGUILayout.LabelField("XR Pipeline Setting - Installing the Microsoft OpenXR plugin", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("To target HoloLens 2 or HP Reverb G2 headset you need to install the Microsoft OpenXR plugin by following the instructions below."
                + "\n\n1. Press the Show Instructions button."
                + "\n2. Follow the instructions in the Manual setup without MRTK section as MRTK is already in the project. Also you do not need to manually select MRTK in the feature tool no matter it is shown as installed or not."
                + "\n3. Keep this window and the Unity project open during the process. A new page confirming the setup is successful will be shown in place of this page once you finish the steps."
                , EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            if (GUILayout.Button("Show Instructions"))
            {
                Application.OpenURL(MSOpenXRPluginUrl);
            }
            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            });
        }

        private void RenderProjectConfigurations()
        {
            RenderConfigurations();

            if (!MixedRealityProjectConfigurator.IsProjectConfigured())
            {
                RenderChoiceDialog();
            }
            else
            {
                RenderConfiguredConfirmation();
            }
        }

        private void RenderConfiguredConfirmation()
        {
            const string dialogTitle = "Project Configuration Confirmed";
            const string dialogContent = "This Unity project is properly configured for the Mixed Reality Toolkit. All items shown above are using recommended settings.";

            CreateSpace(15);
            EditorGUILayout.LabelField(dialogTitle, EditorStyles.boldLabel);
            CreateSpace(15);
            EditorGUILayout.LabelField(dialogContent, EditorStyles.wordWrappedLabel);

            CreateSpace(10);
            if (GUILayout.Button("Next"))
            {
                CurrentStage = ConfigurationStage.ImportTMP;
                Repaint();
            }
            RenderSetupLaterSection();
        }

        private void RenderChoiceDialog()
        {
            const string dialogTitle = "Apply Default Settings?";
            const string dialogContent = "The Mixed Reality Toolkit would like to auto-apply useful settings to this Unity project. Enabled options above will be applied to the project. Disabled items are already properly configured.";

            CreateSpace(15);
            EditorGUILayout.LabelField(dialogTitle, EditorStyles.boldLabel);
            CreateSpace(15);
            EditorGUILayout.LabelField(dialogContent, EditorStyles.wordWrappedLabel);

            CreateSpace(10);
            if (GUILayout.Button(ApplyButtonContent))
            {
                ApplyConfigurations();
            }

            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ImportTMP;
                Repaint();
            });
        }

        private void RenderImportTMP()
        {
            if (TMPEssentialsImported())
            {
                CurrentStage = ConfigurationStage.ShowExamples;
                Repaint();
            }
            EditorGUILayout.LabelField("Importing TMP Essentials", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("MRTK contains components that depend on TextMeshPro. It is recommended that you import TMP by clicking the Import TMP Essentials button below.", EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            var m_ResourceImporter = new TMP_PackageResourceImporter();
            m_ResourceImporter.OnGUI();
            CreateSpace(15);
            RenderSetupLaterSection(true, () => {
                CurrentStage = ConfigurationStage.ShowExamples;
                Repaint();
            });
        }

#if UNITY_2019_3_OR_NEWER
        [MenuItem("Mixed Reality/Toolkit/Utilities/Import Examples from Package (UPM)")]
        private static void DisplayExamplesInPackageManager()
        {
            UnityEditor.PackageManager.UI.Window.Open("Mixed Reality Toolkit Examples");
        }
#endif // UNITY_2019_3_OR_NEWER

        private void RenderShowUPMExamples()
        {
            if (!MRTKExamplesPackageImportedViaUPM())
            {
                CurrentStage = ConfigurationStage.Done;
                Repaint();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.LabelField("Locating MRTK Examples", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField("The MRTK Examples package includes samples to help you familiarize yourself with many core features."
                + "\nSince you imported MRTK via MRFT/UPM the examples no longer show up in the Assets folder automatically. They are now located at Window (menu bar) -> Package Manager "
                + "-> Select In Project in the \"Packages:\" dropdown -> Mixed Reality Toolkit Examples", EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show me the examples"))
                {
#if UNITY_2019_3_OR_NEWER
                    DisplayExamplesInPackageManager();
#endif // UNITY_2019_3_OR_NEWER
                }
                if (GUILayout.Button("Got it, next"))
                {
                    CurrentStage = ConfigurationStage.Done;
                    Repaint();
                }
            }
            CreateSpace(15);
            RenderSetupLaterSection();
        }

        private void RenderConfigurationCompleted()
        {
            GUILayout.Label("MRTK Setup Completed!", MixedRealityStylesUtility.BoldLargeTitleStyle);
            CreateSpace(5);
            EditorGUILayout.LabelField("You have finished setting up the project for Mixed Reality Toolkit. You may go through this process again by clicking on Mixed Reality (menu bar) -> Toolkit -> Utilities -> Configure Project for MRTK"
                + $"\n\nIf there are certain settings not set according to the recommendation you may see this configurator pops up again. Use the Ignore or Later button to suppress this behavior."
                + "\n\nWe hope you enjoy using MRTK. Please find the links to our documentation and API references below. If you encountered something looking like a bug please report by opening an issue in our repository."
                + "\n\nThese links are accessible through Mixed Reality (menu bar) -> Toolkit -> Help.", EditorStyles.wordWrappedLabel);
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Show MRTK Documentation"))
                {
                    Application.OpenURL(MixedRealityToolkitHelpLinks.MRTKDocsUrl);
                }
                if (GUILayout.Button("Show MRTK API References"))
                {
                    Application.OpenURL(MixedRealityToolkitHelpLinks.MRTKAPIRefUrl);
                }
                if (GUILayout.Button("Done"))
                {
                    Close();
                }
            }
        }

        private void RenderSetupLaterSection(bool showSkipButton = false, Action skipButtonAction = null)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Not ready to setup the project now?", EditorStyles.boldLabel);
            
            CreateSpace(15);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (showSkipButton)
                {
                    if (GUILayout.Button(SkipButtonContent))
                    {
                        skipButtonAction();
                    }
                }
                
                if (GUILayout.Button(LaterButtonContent))
                {
                    MixedRealityEditorSettings.IgnoreProjectConfigForSession = true;
                    Close();
                }

                if (GUILayout.Button(IgnoreButtonContent))
                {
                    MixedRealityProjectPreferences.IgnoreSettingsPrompt = true;
                    Close();
                }
            }
            CreateSpace(15);
        }

        private bool TMPEssentialsImported()
        {
            if (isTMPEssentialsImported.HasValue)
            {
                return isTMPEssentialsImported.Value;
            }
            isTMPEssentialsImported = File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
            return isTMPEssentialsImported.Value;
        }

        [MenuItem("Mixed Reality/Toolkit/Utilities/Import Examples from Package (UPM)", true)]
        private static bool MRTKExamplesPackageImportedViaUPM()
        {
#if !UNITY_2019_3_OR_NEWER
            return false;
#else
            if (isMRTKExamplesPackageImportedViaUPM.HasValue)
            {
                return isMRTKExamplesPackageImportedViaUPM.Value;
            }
            
            var request = UnityEditor.PackageManager.Client.List(true, true);
            while (!request.IsCompleted) { }
            if (request.Result != null && request.Result.Any(p => p.displayName == "Mixed Reality Toolkit Examples"))
            {
                isMRTKExamplesPackageImportedViaUPM = true;
                return isMRTKExamplesPackageImportedViaUPM.Value;
            }
            
            isMRTKExamplesPackageImportedViaUPM = false;
            return isMRTKExamplesPackageImportedViaUPM.Value;

#endif // !UNITY_2019_3_OR_NEWER
        }
        
        private readonly Dictionary<MRConfig, bool> trackToggles = new Dictionary<MRConfig, bool>()
        {
            { MRConfig.ForceTextSerialization, true },
            { MRConfig.VisibleMetaFiles, true },
            { MRConfig.VirtualRealitySupported, true },
            { MRConfig.OptimalRenderingPath, true },
            { MRConfig.SpatialAwarenessLayer, true },
            { MRConfig.AudioSpatializer, true },

            // UWP Capabilities
            { MRConfig.MicrophoneCapability, true },
            { MRConfig.InternetClientCapability, true },
            { MRConfig.SpatialPerceptionCapability, true },
#if UNITY_2019_3_OR_NEWER
            { MRConfig.EyeTrackingCapability, true },
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_2019_3_OR_NEWER
            { MRConfig.NewInputSystem, true },
#endif // UNITY_2019_3_OR_NEWER

            // Android Settings
            { MRConfig.AndroidMultiThreadedRendering, true },
            { MRConfig.AndroidMinSdkVersion, true },

            // iOS Settings
            { MRConfig.IOSMinOSVersion, true },
            { MRConfig.IOSArchitecture, true },
            { MRConfig.IOSCameraUsageDescription, true },

#if UNITY_2019_3_OR_NEWER
            // A workaround for the Unity bug described in https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8326.
            { MRConfig.GraphicsJobWorkaround, true },
#endif // UNITY_2019_3_OR_NEWER
        };

        private const string None = "None";
        
        private Vector2 scrollPosition = Vector2.zero;

        public void RenderConfigurations()
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollView.scrollPosition;
                EditorGUILayout.LabelField("Project Settings", EditorStyles.boldLabel);
                RenderToggle(MRConfig.ForceTextSerialization, "Force text asset serialization");
                RenderToggle(MRConfig.VisibleMetaFiles, "Enable visible meta files");
                if (!MixedRealityOptimizeUtils.IsBuildTargetAndroid() && !MixedRealityOptimizeUtils.IsBuildTargetIOS() && XRSettingsUtilities.XREnabled)
                {
#if !UNITY_2019_3_OR_NEWER
                    RenderToggle(MRConfig.VirtualRealitySupported, "Enable VR supported");
#endif // !UNITY_2019_3_OR_NEWER
                }
#if UNITY_2019_3_OR_NEWER
                if (XRSettingsUtilities.LegacyXREnabled)
                {
                    RenderToggle(MRConfig.OptimalRenderingPath, "Set Single Pass Instanced rendering path (legacy XR API)");
                }
#else
#if UNITY_ANDROID
                RenderToggle(MRConfig.OptimalRenderingPath, "Set Single Pass Stereo rendering path");
#else
                RenderToggle(MRConfig.OptimalRenderingPath, "Set Single Pass Instanced rendering path");
#endif
#endif // UNITY_2019_3_OR_NEWER
                RenderToggle(MRConfig.SpatialAwarenessLayer, "Set default Spatial Awareness layer");

#if UNITY_2019_3_OR_NEWER
                RenderToggle(MRConfig.NewInputSystem, "Enable old input system for input simulation (won't disable new input system)");
#endif // UNITY_2019_3_OR_NEWER

                PromptForAudioSpatializer();
                EditorGUILayout.Space();

                if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
                {
                    EditorGUILayout.LabelField("UWP Capabilities", EditorStyles.boldLabel);
                    RenderToggle(MRConfig.MicrophoneCapability, "Enable Microphone Capability");
                    RenderToggle(MRConfig.InternetClientCapability, "Enable Internet Client Capability");
                    RenderToggle(MRConfig.SpatialPerceptionCapability, "Enable Spatial Perception Capability");
#if UNITY_2019_3_OR_NEWER
                    RenderToggle(MRConfig.EyeTrackingCapability, "Enable Eye Gaze Input Capability");
                    RenderToggle(MRConfig.GraphicsJobWorkaround, "Avoid Unity 'PlayerSettings.graphicsJob' crash");
#endif // UNITY_2019_3_OR_NEWER
                }
                else
                {
                    trackToggles[MRConfig.MicrophoneCapability] = false;
                    trackToggles[MRConfig.InternetClientCapability] = false;
                    trackToggles[MRConfig.SpatialPerceptionCapability] = false;
#if UNITY_2019_3_OR_NEWER
                    trackToggles[MRConfig.EyeTrackingCapability] = false;
                    trackToggles[MRConfig.GraphicsJobWorkaround] = false;
#endif // UNITY_2019_3_OR_NEWER
                }

                if (MixedRealityOptimizeUtils.IsBuildTargetAndroid())
                {
                    EditorGUILayout.LabelField("Android Settings", EditorStyles.boldLabel);
                    RenderToggle(MRConfig.AndroidMultiThreadedRendering, "Disable Multi-Threaded Rendering");
                    RenderToggle(MRConfig.AndroidMinSdkVersion, "Set Minimum API Level");
                }

                if (MixedRealityOptimizeUtils.IsBuildTargetIOS())
                {
                    EditorGUILayout.LabelField("iOS Settings", EditorStyles.boldLabel);
                    RenderToggle(MRConfig.IOSMinOSVersion, "Set Required OS Version");
                    RenderToggle(MRConfig.IOSArchitecture, "Set Required Architecture");
                    RenderToggle(MRConfig.IOSCameraUsageDescription, "Set Camera Usage Descriptions");
                }
            }
        }

        private void ApplyConfigurations()
        {
            var configurationFilter = new HashSet<MRConfig>();
            foreach (var item in trackToggles)
            {
                if (item.Value)
                {
                    configurationFilter.Add(item.Key);
                }
            }

            MixedRealityProjectConfigurator.ConfigureProject(configurationFilter);
        }

        /// <summary>
        /// Provide the user with the list of spatializers that can be selected.
        /// </summary>
        private void PromptForAudioSpatializer()
        {
            string selectedSpatializer = MixedRealityProjectConfigurator.SelectedSpatializer;
            List<string> spatializers = new List<string>
            {
                None
            };
            spatializers.AddRange(SpatializerUtilities.InstalledSpatializers);
            RenderDropDown(MRConfig.AudioSpatializer, "Audio spatializer:", spatializers.ToArray(), ref selectedSpatializer);
            MixedRealityProjectConfigurator.SelectedSpatializer = selectedSpatializer;
        }

        private void RenderDropDown(MRConfig configKey, string title, string[] collection, ref string selection)
        {
            bool configured = MixedRealityProjectConfigurator.IsConfigured(configKey);
            using (new EditorGUI.DisabledGroupScope(configured))
            {
                if (configured)
                {
                    EditorGUILayout.LabelField(new GUIContent($"{title} {selection}", InspectorUIUtility.SuccessIcon));
                }
                else
                {
                    int index = 0;
                    for (int i = 0; i < collection.Length; i++)
                    {
                        if (collection[i] != selection) { continue; }

                        index = i;
                    }
                    index = EditorGUILayout.Popup(title, index, collection, EditorStyles.popup);

                    selection = collection[index];
                    if (selection == None)
                    {
                        // The user selected "None", return null. Unity uses this string where null
                        // is the underlying value.
                        selection = null;
                    }
                }
            }
        }

        private void RenderToggle(MRConfig configKey, string title)
        {
            bool configured = MixedRealityProjectConfigurator.IsConfigured(configKey);
            using (new EditorGUI.DisabledGroupScope(configured))
            {
                if (configured)
                {
                    EditorGUILayout.LabelField(new GUIContent(title, InspectorUIUtility.SuccessIcon));
                    trackToggles[configKey] = false;
                }
                else
                {
                    trackToggles[configKey] = EditorGUILayout.ToggleLeft(title, trackToggles[configKey]);
                }
            }
        }

        private void CreateSpace(float width)
        {
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.Space(width);
#else
            for (int i = 0; i < Math.Ceiling(width / 5); i++)
            {
                EditorGUILayout.Space();
            }
#endif // UNITY_2019_3_OR_NEWER
        }
#if UNITY_2019_3_OR_NEWER
        private void AddUPMPackage(string packageName)
        {
            var request = UnityEditor.PackageManager.Client.Add(packageName);
            while (!request.IsCompleted) { }
        }
#endif
    }
}

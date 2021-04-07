using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class XRPipelineConfigWindow : EditorWindow
{
    private const float Default_Window_Height = 640.0f;
    private const float Default_Window_Width = 400.0f;
    private const string XRPipelineDocsUrl = "https://";
    private const string XRSDKUnityDocsUrl = "https://";
    private const string MRFTDocsUrl = "https://docs.microsoft.com/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool";
    private readonly GUIContent ApplyButtonContent = new GUIContent("Apply", "Apply configurations to this Unity Project");
    private readonly GUIContent SkipButtonContent = new GUIContent("Skip", "Skip to next step");
    private readonly GUIContent LaterButtonContent = new GUIContent("Later", "Do not show this pop-up notification until next session");
    private readonly GUIContent IgnoreButtonContent = new GUIContent("Ignore", "Modify this preference under Edit > Project Settings > Mixed Reality Toolkit");

    private static ConfigurationStage currentStage
    {
        get => MixedRealityProjectPreferences.ConfiguratorState;
        set => MixedRealityProjectPreferences.ConfiguratorState = value;
    }

    public static XRPipelineConfigWindow Instance { get; private set; }

    public static bool IsOpen => Instance != null;

    private void OnEnable()
    {
        Instance = this;
    }

    [MenuItem("Ut/Configure Demo", false, 499)]
    public static void ShowWindow()
    {
        // There should be only one configurator window open as a "pop-up". If already open, then just force focus on our instance
        if (IsOpen)
        {
            Instance.Focus();
        }
        else
        {
            var window = CreateInstance<XRPipelineConfigWindow>();
            window.titleContent = new GUIContent("MRTK Project Configurator", EditorGUIUtility.IconContent("_Popup").image);
            window.position = new Rect(Screen.width / 2.0f, Screen.height / 2.0f, Default_Window_Height, Default_Window_Width);
            window.ShowUtility();
        }
    }

    private void OnGUI()
    {
        MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();
        EditorGUILayout.LabelField("Welcome to MRTK!", MixedRealityStylesUtility.BoldLargeTitleStyle);
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("This configurator will go through some settings to make sure the project is ready for MRTK.");
        EditorGUILayout.Space(20);

        switch (currentStage)
        {
            case ConfigurationStage.Init:
                RenderXRPipelineSelection();
                break;
            case ConfigurationStage.SelectXRSDKPlugin:
                RenderSelectXRSDKPlugin();
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
                if (TMPEssentialsImported())
                {
                    currentStage = ConfigurationStage.ShowExamples;
                    Repaint();
                }
                RenderImportTMP();
                break;
            case ConfigurationStage.ShowExamples:
                if (!MRTKExamplesPackageImportedViaUPM())
                {
                    currentStage = ConfigurationStage.Done;
                    Repaint();
                }
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
            else
            {
                RenderXRSDKBuiltinPluginPipelineDetected();
            }
        }
    }

    private void RenderNoPipeline()
    {
        EditorGUILayout.LabelField("XR Pipeline Setting", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("To build applications targeting AR/VR headsets you need to specify an XR pipeline. "
            + $"Unity currently provides the following pipeline(s) in this version ({Application.unityVersion}). "
            + "Please choose the one you would like to use. You may also skip this step and configure manually later. "
            + $"More information can be found at {XRPipelineDocsUrl}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
#if !UNITY_2020_1_OR_NEWER
            if (GUILayout.Button("Legacy XR"))
            {
                XRSettingsUtilities.LegacyXREnabled = true;
            }
#endif
#if UNITY_2019_3_OR_NEWER
            if (GUILayout.Button("XR SDK/XR Management (Recommended)"))
            {
                currentStage = ConfigurationStage.SelectXRSDKPlugin;
                Repaint();
            }
#endif
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
        }
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ProjectConfiguration;
            Repaint();
        });
    }

    private void RenderLegacyXRPipelineDetected()
    {
        EditorGUILayout.LabelField("XR Pipeline Setting - LegacyXR in use", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("To build applications targeting AR/VR headsets you need to specify an XR pipeline. "
            + $"\n\nThe LegacyXR pipeline is detected in the project. Please be aware that the LegacyXR pipeline is deprecated in Unity 2019 and is removed in Unity 2020."
            + $"\n\nFor more information on alternative pipelines, please visit {XRPipelineDocsUrl}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Next"))
            {
                currentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            }
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
        }
        RenderSetupLaterSection();
    }

    private void RenderMicrosoftOpenXRPipelineDetected()
    {
        EditorGUILayout.LabelField("XR Pipeline Setting - XR SDK OpenXR with Microsoft plugin in use", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("To build applications targeting AR/VR headsets you need to specify an XR pipeline. "
            + $"\n\nThe XR SDK OpenXR pipeline with Microsoft plugin is detected in the project. You are good to go."
            + $"\n\nFor more information on alternative pipelines, please visit {XRPipelineDocsUrl}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Next"))
            {
                currentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            }
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
            
        }
        RenderSetupLaterSection();
    }

    private void RenderXRSDKBuiltinPluginPipelineDetected()
    {
        EditorGUILayout.LabelField("XR Pipeline Setting - XR SDK with builtin plugin in use", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("To build applications targeting AR/VR headsets you need to specify an XR pipeline. "
            + $"\n\nThe XR SDK pipeline with builtin plugin is detected in the project. You are good to go."
            + $"\n\nFor more information on alternative pipelines, please visit {XRPipelineDocsUrl}", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Next"))
            {
                currentStage = ConfigurationStage.ProjectConfiguration;
                Repaint();
            }
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
        }
        RenderSetupLaterSection();
    }

    private void RenderSelectXRSDKPlugin()
    {
        EditorGUILayout.LabelField("Enabling the XR SDK Pipeline", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("There are several provider plugins for the XR SDK pipeline available in this Unity version. "
            + $"\n\nThe Microsoft OpenXR plugin is recommended if you are targeting HoloLens 2 and/or Windows Mixed Reality (WMR) headsets."
            + "\nThe built-in plugins provided by Unity offers a wide range of supported devices, including HoloLens 2 and WMR headsets. "
            + "\nIf you wish to use OpenXR with a non Microsoft device, select skip now and configure the project according to the device manufacturer after finishing the MRTK configuration process. "
            + $"\n\nMore information can be found at {XRPipelineDocsUrl}.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Use OpenXR with Microsoft plugin"))
            {
                currentStage = ConfigurationStage.InstallMSOpenXR;
                Repaint();
            }
            if (GUILayout.Button("Use built-in Unity plugins"))
            {
                currentStage = ConfigurationStage.InstallBuiltinPlugin;
                Repaint();
            }
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRPipelineDocsUrl);
            }
        }
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ProjectConfiguration;
            Repaint();
        });
    }

    private void RenderEnableXRSDKBuiltinPlugin()
    {
        if (XRSettingsUtilities.XRSDKEnabled)
        {
            currentStage = ConfigurationStage.Init;
            Repaint();
        }
        EditorGUILayout.LabelField("Enabling the XR SDK Pipeline with built-in Plugins", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("To enable the XR SDK pipeline with built-in Plugins, first press the Show Settings button. "
            + $"\n\nIn the XR management plug-in window that shows up, click on the install XR Plugin Management button if you see such button. "
            + "If there is no such button or after clicking on that button, please check the plugin(s) you want to use based on your target device. "
            + "\n\nBe sure to switch to the correct build target (e.g. UWP, Windows standalone) tab first by clicking on the icon(s) right below the XR Plug-in Management title. "
            + $"After checking the desired plugin(s) click on the Next button to continue."
            + $"\n\nMore information can be found at {XRSDKUnityDocsUrl} (Only the first three steps are needed if following instructions on the page)", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Show Settings"))
            {
                //SettingsService.OpenProjectSettings("Project/XR Plug-in Management");
                SettingsService.OpenProjectSettings("Project/XR Plugin Management");
            }
            if (GUILayout.Button("Learn more"))
            {
                Application.OpenURL(XRSDKUnityDocsUrl);
            }
        }
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ProjectConfiguration;
            Repaint();
        });
    }

    private void RenderEnableMicrosoftOpenXRPlugin()
    {
        EditorGUILayout.LabelField("Enabling the XR SDK Pipeline with OpenXR", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("To use OpenXR with the XR SDK pipeline, first press the Download Tool button. "
            + $"\nThe button takes you to a page where you can download the Mixed Reality Feature Tool, which is used to get the OpenXR plugin. "
            + "Follow the instructions there to use the tool. Remember to select \"Mixed Reality OpenXR Plugin\" in the list of packages (step 3). "
            + "\nYou do not need to manually select MRTK no matter it is shown as installed or not. "
            + $"After finishing the process in the feature tool come back here to verify whether the installation is successful. A new page should be shown if you succeeded."
            + $"\n\nMore information can be found at {MRFTDocsUrl}.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Download Tool"))
            {
                Application.OpenURL(MRFTDocsUrl);
            }
        }
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ProjectConfiguration;
            Repaint();
        });
    }

    private void RenderProjectConfigurations()
    {
        var window = MixedRealityProjectConfiguratorWindow.GetObj();
        EditorGUILayout.Space(15);
        window.RenderConfigurations();

        if (!MixedRealityProjectConfigurator.IsProjectConfigured())
        {
            RenderChoiceDialog(window);
        }
        else
        {
            RenderConfiguredConfirmation();
        }
    }

    private void RenderConfiguredConfirmation()
    {
        const string dialogTitle = "Project Configuration Complete";
        const string dialogContent = "This Unity project is properly configured for the Mixed Reality Toolkit.";
        EditorGUILayout.LabelField(dialogTitle, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(dialogContent);
        if (GUILayout.Button("Next"))
        {
            currentStage = ConfigurationStage.ImportTMP;
            Repaint();
        }
        RenderSetupLaterSection();
    }

    private void RenderChoiceDialog(MixedRealityProjectConfiguratorWindow window)
    {
        const string dialogTitle = "Apply Default Settings?";
        const string dialogContent = "The Mixed Reality Toolkit would like to auto-apply useful settings to this Unity project";
        EditorGUILayout.LabelField(dialogTitle, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(dialogContent);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button(ApplyButtonContent))
            {
                window.ApplyConfigurations();
            }
        }
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ImportTMP;
            Repaint();
        });
    }

    private void RenderImportTMP()
    {
        EditorGUILayout.LabelField("Importing TMP Essentials", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("MRTK contains components that depend on TextMeshPro. It is recommended that you import TMP by clicking the Import TMP Essentials button below. "
            //+ $"\n\nThe Microsoft OpenXR plugin is recommended if you are targeting HoloLens 2 and/or Windows Mixed Reality (WMR) headsets."
            //+ "\nThe built-in plugins provided by Unity offers a wide range of supported devices, including HoloLens 2 and WMR headsets. "
            //+ "\nIf you wish to use OpenXR with a non Microsoft device, select skip now and configure the project according to the device manufacturer after finishing the MRTK configuration process. "
            + $"\n\nMore information can be found at {XRPipelineDocsUrl}.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        var m_ResourceImporter = new TMP_PackageResourceImporter();
        m_ResourceImporter.OnGUI();
        EditorGUILayout.Space(15);
        RenderSetupLaterSection(true, () => {
            currentStage = ConfigurationStage.ShowExamples;
            Repaint();
        });
    }

    private void RenderShowUPMExamples()
    {
        EditorGUILayout.LabelField("Locating MRTK Examples", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("The MRTK Examples package includes samples to help you familiarize yourself with many core features. "
            + $"\nSince you imported MRTK via MRFT/UPM the examples no longer show up in the Assets folder automatically. They are now located at window -> upm -> (full instructions omitted)"
            //+ "\nThe built-in plugins provided by Unity offers a wide range of supported devices, including HoloLens 2 and WMR headsets. "
            //+ "\nIf you wish to use OpenXR with a non Microsoft device, select skip now and configure the project according to the device manufacturer after finishing the MRTK configuration process. "
            + $"\n\nMore information can be found at {XRPipelineDocsUrl}.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Show me the examples"))
            {
                UnityEditor.PackageManager.UI.Window.Open("Mixed Reality Toolkit Examples");
            }
            if (GUILayout.Button("Got it, next"))
            {
                currentStage = ConfigurationStage.Done;
                Repaint();
            }
        }
        EditorGUILayout.Space(15);
        RenderSetupLaterSection();
    }

    private void RenderConfigurationCompleted()
    {
        EditorGUILayout.LabelField("Enabling the XR SDK Pipeline with OpenXR", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("To use OpenXR with the XR SDK pipeline, first press the Download Tool button. "
            + $"\nThe button takes you to a page where you can download the Mixed Reality Feature Tool, which is used to get the OpenXR plugin. "
            + "Follow the instructions there to use the tool. Remember to select \"Mixed Reality OpenXR Plugin\" in the list of packages (step 3). "
            + "\nYou do not need to manually select MRTK no matter it is shown as installed or not. "
            + $"After finishing the process in the feature tool come back here to verify whether the installation is successful. A new page should be shown if you succeeded."
            + $"\n\nMore information can be found at {MRFTDocsUrl}.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Show MRTK Documentation"))
            {
                Application.OpenURL(MRFTDocsUrl);
            }
            if (GUILayout.Button("Show MRTK API References"))
            {
                Application.OpenURL(MRFTDocsUrl);
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
        //EditorGUILayout.Space();
        EditorGUILayout.LabelField("You may choose to skip this step, delay the setup until next session or ignore the setup unless reenabled. "
            , EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space(15);
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
        EditorGUILayout.Space(15);
    }

    private bool TMPEssentialsImported()
    {
        return File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset");
    }

    private bool MRTKExamplesPackageImportedViaUPM()
    {
        var request = Client.List(true, true);
        while (!request.IsCompleted) { }
        if (request.Result != null && request.Result.Any(p => p.displayName == "Mixed Reality Toolkit Examples"))
        {
            return true;
        }
        return false;
    }
}

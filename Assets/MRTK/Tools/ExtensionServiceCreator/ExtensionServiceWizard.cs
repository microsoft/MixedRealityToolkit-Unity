// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Editor Window class that renders controls and logic for extension service creation walkthrough
    /// </summary>
    internal class ExtensionServiceWizard : EditorWindow
    {
        private static ExtensionServiceWizard window;
        private static readonly string servicesDocumentationURL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/ExtensionServiceCreationWizard.html";
        private static readonly Vector2 minWindowSize = new Vector2(500, 0);
        private const int DocLinkWidth = 200;
        private const string TargetFolderLabel = "Target Folder";

        private ExtensionServiceCreator creator = new ExtensionServiceCreator();
        private List<string> errors = new List<string>();
        private bool registered = false;
        private static float progressBarTimer = 0.0f;

        private Vector2 outputFoldersScrollPos;
        private bool useUniversalFolder = true;

        [MenuItem("Mixed Reality Toolkit/Utilities/Create Extension Service", false, 500)]
        private static void CreateExtensionServiceMenuItem()
        {
            if (window != null)
            {
                Debug.Log("Only one window allowed at a time");
                return;
            }

            // Dock it next to the Scene View.
            window = GetWindow<ExtensionServiceWizard>(typeof(SceneView));
            window.titleContent = new GUIContent("Extension Service Wizard", EditorGUIUtility.IconContent("d_DefaultSorting").image);
            window.minSize = minWindowSize;
            window.ResetCreator();
            window.Show();
        }

        private void ResetCreator()
        {
            if (creator == null)
            {
                creator = new ExtensionServiceCreator();
            }

            creator.ResetState();
        }

        private void OnEnable()
        {
            if (creator == null)
            {
                creator = new ExtensionServiceCreator();
            }

            creator.LoadStoredState();

        }

        private void OnGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            errors.Clear();

            if (!creator.ValidateAssets(errors))
            {
                EditorGUILayout.LabelField("Validating assets...");
                RenderErrorLog();
                return;
            }

            switch (creator.Stage)
            {
                case ExtensionServiceCreator.CreationStage.SelectNameAndPlatform:
                    DrawSelectNameAndPlatform();
                    break;
                case ExtensionServiceCreator.CreationStage.ChooseOutputFolders:
                    DrawChooseOutputFolders();
                    break;
                case ExtensionServiceCreator.CreationStage.CreatingExtensionService:
                case ExtensionServiceCreator.CreationStage.CreatingProfileInstance:
                    DrawCreatingAssets();
                    break;
                case ExtensionServiceCreator.CreationStage.Finished:
                    DrawFinished();
                    break;
            }
        }

        private void DrawSelectNameAndPlatform()
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("This wizard will help you set up and register a simple extension service. MRTK Services are similar to traditional MonoBehaviour singletons but with more robust access and lifecycle control. Scripts can access services through the MRTK's service provider interface. For more information about services, click the link below.", MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                InspectorUIUtility.RenderDocumentationButton(servicesDocumentationURL);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Choose a name for your service.", EditorStyles.miniLabel);
            creator.ServiceName = EditorGUILayout.TextField("Service Name", creator.ServiceName);
            creator.ValidateName(errors);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Choose which platforms your service will support.", EditorStyles.miniLabel);
            creator.Platforms = (SupportedPlatforms)EditorGUILayout.EnumFlagsField("Platforms", creator.Platforms);
            creator.ValidatePlatforms(errors);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Choose a namespace for your service.", EditorStyles.miniLabel);
            creator.Namespace = EditorGUILayout.TextField("Namespace", creator.Namespace);
            creator.ValidateNamespace(errors);
            EditorGUILayout.Space();

            bool hasErrors = errors.Count > 0;
            if (hasErrors)
            {
                RenderErrorLog();
            }

            using (new EditorGUI.DisabledGroupScope(hasErrors))
            {
                if (GUILayout.Button("Next"))
                {
                    creator.Stage = ExtensionServiceCreator.CreationStage.ChooseOutputFolders;
                    creator.StoreState();
                }
            }
        }

        private void RenderErrorLog()
        {
            for (int i = 0; i < errors.Count; i++)
            {
                EditorGUILayout.HelpBox(errors[i], MessageType.Error);
            }
        }

        private void DrawChooseOutputFolders()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(outputFoldersScrollPos))
            {
                outputFoldersScrollPos = scroll.scrollPosition;

                useUniversalFolder = EditorGUILayout.ToggleLeft("Place all files in same folder", useUniversalFolder);
                if (useUniversalFolder)
                {
                    var newFolder = EditorGUILayout.ObjectField(TargetFolderLabel, creator.ServiceFolderObject, typeof(DefaultAsset), false);

                    string path = AssetDatabase.GetAssetPath(newFolder);
                    creator.SetAllFolders(path);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Below are the files you will be generating", EditorStyles.miniLabel);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    string svcFile = creator.ServiceName + ".cs";
                    EditorGUILayout.LabelField(svcFile, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("This is the main script for your service. It functions similarly to a MonoBehaviour, with Enable, Disable and Update functions.", EditorStyles.wordWrappedMiniLabel);

                    if (!useUniversalFolder)
                    {
                        creator.ServiceFolderObject = EditorGUILayout.ObjectField(TargetFolderLabel, creator.ServiceFolderObject, typeof(DefaultAsset), false);
                    }

                    if (!creator.CanBuildAsset(creator.ServiceFolderObject, creator.ServiceName))
                    {
                        errors.Add($"{svcFile} script cannot be created. Either invalid folder or asset already exists");
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    string interfaceFile = creator.InterfaceName + ".cs";
                    EditorGUILayout.LabelField(interfaceFile, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("This is the interface that other scripts will use to interact with your service.", EditorStyles.wordWrappedMiniLabel);

                    if (!useUniversalFolder)
                    {
                        creator.InterfaceFolderObject = EditorGUILayout.ObjectField(TargetFolderLabel, creator.InterfaceFolderObject, typeof(DefaultAsset), false);
                    }

                    if (!creator.CanBuildAsset(creator.InterfaceFolderObject, creator.InterfaceName))
                    {
                        errors.Add($"{interfaceFile} script cannot be created. Either invalid folder or asset already exists");
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    string inspectorFile = creator.InspectorName + ".cs";
                    creator.UsesInspector = EditorGUILayout.ToggleLeft(inspectorFile + " (Optional)", creator.UsesInspector, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("An optional inspector for your service. This will be displayed in the editor when service inspectors are enabled.", EditorStyles.wordWrappedMiniLabel);

                    if (creator.UsesInspector)
                    {
                        if (!useUniversalFolder)
                        {
                            creator.InspectorFolderObject = EditorGUILayout.ObjectField(TargetFolderLabel, creator.InspectorFolderObject, typeof(DefaultAsset), false);
                        }

                        if (!creator.CanBuildAsset(creator.InspectorFolderObject, creator.InspectorName))
                        {
                            errors.Add($"{inspectorFile} script cannot be created. Either invalid folder or asset already exists");
                        }
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    string profileFile = creator.ProfileName + ".cs";
                    creator.UsesProfile = EditorGUILayout.ToggleLeft(profileFile + " (Optional)", creator.UsesProfile, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("An optional profile script for your service. Profiles are scriptable objects that store permanent config data. If you're not sure whether your service will need a profile, it's best to create one. You can remove it later.", EditorStyles.wordWrappedMiniLabel);

                    if (creator.UsesProfile)
                    {
                        if (!useUniversalFolder)
                        {
                            creator.ProfileFolderObject = EditorGUILayout.ObjectField(TargetFolderLabel, creator.ProfileFolderObject, typeof(DefaultAsset), false);
                        }

                        if (!creator.CanBuildAsset(creator.ProfileFolderObject, creator.ProfileName))
                        {
                            errors.Add($"{profileFile} script cannot be created. Either invalid folder or asset already exists");
                        }
                    }
                }

                if (creator.UsesProfile)
                {
                    EditorGUILayout.Space();
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        string profileAssetFile = creator.ProfileAssetName + ".asset";
                        EditorGUILayout.LabelField(profileAssetFile, EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("A default instance of your profile.", EditorStyles.wordWrappedMiniLabel);

                        if (!useUniversalFolder)
                        {
                            creator.ProfileAssetFolderObject = EditorGUILayout.ObjectField(TargetFolderLabel, creator.ProfileAssetFolderObject, typeof(UnityEngine.Object), false);
                        }

                        if (!creator.CanBuildAsset(creator.ProfileAssetFolderObject, creator.ProfileAssetName))
                        {
                            errors.Add($"{profileAssetFile} script cannot be created. Either invalid folder or asset already exists");
                        }
                    }
                }

                EditorGUILayout.Space();

                bool hasErrors = errors.Count > 0;
                if (hasErrors)
                {
                    RenderErrorLog();
                }

                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Back"))
                    {
                        creator.Stage = ExtensionServiceCreator.CreationStage.SelectNameAndPlatform;
                        creator.StoreState();
                    }

                    using (new EditorGUI.DisabledGroupScope(hasErrors))
                    {
                        if (GUILayout.Button("Create Service"))
                        {
                            // Start the async method that will wait for the service to be created
                            CreateAssetsAsync();
                        }
                    }
                }
            }
        }

        private void DrawCreatingAssets()
        {
            using (var progressBarRect = new EditorGUILayout.VerticalScope())
            {
                progressBarTimer = Mathf.Clamp01(Time.realtimeSinceStartup % 1.0f);

                EditorGUI.ProgressBar(progressBarRect.rect, progressBarTimer, "Creating assets...");
                GUILayout.Space(16);
            }

            if (creator.Result == ExtensionServiceCreator.CreateResult.Error)
            {
                EditorGUILayout.HelpBox("There were errors while creating assets.", MessageType.Error);
            }

            DrawCreationLog();

            Repaint();
        }

        private void DrawFinished()
        {
            EditorGUILayout.Space();

            if (creator.Result == ExtensionServiceCreator.CreateResult.Error)
            {
                EditorGUILayout.HelpBox("There were errors during the creation process:", MessageType.Error);
                DrawCreationLog();

                EditorGUILayout.Space();
                if (GUILayout.Button("Start over"))
                {
                    creator.ResetState();
                }

                // All done, bail early
                return;
            }

            EditorGUILayout.HelpBox("Your service scripts have been created.", MessageType.Info);

            if (!registered)
            {
                EditorGUILayout.LabelField("Would you like to register this service in your current MixedRealityToolkit profile?", EditorStyles.miniLabel);

                // Check to see whether it's possible to register the profile
                bool canRegisterProfile = true;
                if (MixedRealityToolkit.Instance == null || !MixedRealityToolkit.Instance.HasActiveProfile)
                {
                    EditorGUILayout.HelpBox("Toolkit has no active profile. Can't register service.", MessageType.Warning);
                    canRegisterProfile = false;
                }
                else if (MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile == null)
                {
                    EditorGUILayout.HelpBox("Toolkit has no RegisteredServiceProvidersProfile. Can't register service.", MessageType.Warning);
                    canRegisterProfile = false;
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledGroupScope(!canRegisterProfile))
                    {
                        if (GUILayout.Button("Register"))
                        {
                            RegisterServiceWithActiveMixedRealityProfile();
                        }
                    }

                    if (GUILayout.Button("Not Now"))
                    {
                        creator.ResetState();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Your service is now registered. Scripts can access this service like so:", EditorStyles.miniLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.TextField(creator.SampleCode);
                    if (GUILayout.Button("Copy Sample Code", EditorStyles.miniButton))
                    {
                        EditorGUIUtility.systemCopyBuffer = creator.SampleCode;
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Create new service"))
                {
                    creator.ResetState();
                }
            }
        }

        private void DrawCreationLog()
        {
            var style = new GUIStyle(EditorStyles.wordWrappedLabel);
            style.richText = true;
            EditorGUILayout.LabelField(creator.CreationLog, style);
        }

        private void RegisterServiceWithActiveMixedRealityProfile()
        {
            // We assume this has already been validated
            MixedRealityRegisteredServiceProvidersProfile servicesProfile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
            // Use serialized object so this process can be undone
            SerializedObject servicesProfileObject = new SerializedObject(servicesProfile);
            SerializedProperty configurations = servicesProfileObject.FindProperty("configurations");
            int numConfigurations = configurations.arraySize;
            // Insert a new configuration at the end
            configurations.InsertArrayElementAtIndex(numConfigurations);
            // Get that config value
            SerializedProperty newConfig = configurations.GetArrayElementAtIndex(numConfigurations);

            // Configurations look like so:
            /*
                SystemType componentType;
                string componentName;
                uint priority;
                SupportedPlatforms runtimePlatform;
                BaseMixedRealityProfile configurationProfile;
            */

            SerializedProperty componentType = newConfig.FindPropertyRelative("componentType");
            SerializedProperty componentTypeReference = componentType.FindPropertyRelative("reference");
            SerializedProperty componentName = newConfig.FindPropertyRelative("componentName");
            SerializedProperty priority = newConfig.FindPropertyRelative("priority");
            SerializedProperty runtimePlatform = newConfig.FindPropertyRelative("runtimePlatform");
            SerializedProperty configurationProfile = newConfig.FindPropertyRelative("configurationProfile");

            componentTypeReference.stringValue = creator.ServiceType.AssemblyQualifiedName;
            // Add spaces between camel case service name
            componentName.stringValue = System.Text.RegularExpressions.Regex.Replace(creator.ServiceName, "(\\B[A-Z])", " $1");
            configurationProfile.objectReferenceValue = creator.ProfileInstance;
            runtimePlatform.intValue = (int)creator.Platforms;

            servicesProfileObject.ApplyModifiedProperties();

            // Select the profile so we can see what we've done
            Selection.activeObject = servicesProfile;

            registered = true;
        }

        private async void CreateAssetsAsync()
        {
            await creator.BeginAssetCreationProcess();
        }
    }
}
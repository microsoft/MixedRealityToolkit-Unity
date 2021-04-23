// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.CSharp;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Class used to generate service scripts and profile instances. Primarily designed for in-editor use
    /// </summary>
    public class ExtensionServiceCreator
    {
        #region enums and types

        /// <summary>
        /// Result of extension service file(s) create operation
        /// </summary>
        public enum CreateResult
        {
            None,
            Successful,
            Error,
        }

        /// <summary>
        /// The current stage of the creation process
        /// </summary>
        public enum CreationStage
        {
            SelectNameAndPlatform,
            ChooseOutputFolders,
            CreatingExtensionService,
            CreatingProfileInstance,
            Finished,
        }

        /// <summary>
        /// Simple struct for storing state in editor prefs when recompiling
        /// </summary>
        [Serializable]
        private struct PersistentState
        {
            public string ServiceName;
            public bool UsesProfile;
            public bool UsesInspector;
            public SupportedPlatforms Platforms;
            public CreationStage Stage;
            public UnityEngine.Object ServiceFolder;
            public UnityEngine.Object InspectorFolder;
            public UnityEngine.Object InterfaceFolder;
            public UnityEngine.Object ProfileFolder;
            public UnityEngine.Object ProfileAssetFolder;
            public string Namespace;
        }

        #endregion

        #region public properties

        /// <summary>
        /// The name of the new extension service to build
        /// </summary>
        public string ServiceName
        {
            get => state.ServiceName;
            set => state.ServiceName = value;
        }

        /// <summary>
        /// Should a ScriptableObject profile class be created for new extension service
        /// </summary>
        public bool UsesProfile
        {
            get => state.UsesProfile;
            set => state.UsesProfile = value;
        }

        /// <summary>
        /// Should a custom editor inspector class be created for new extension service
        /// </summary>
        public bool UsesInspector
        {
            get => state.UsesInspector;
            set => state.UsesInspector = value;
        }

        /// <summary>
        /// Supported platform flags for new extension service. Added to attribute on service class
        /// </summary>
        public SupportedPlatforms Platforms
        {
            get => state.Platforms;
            set => state.Platforms = value;
        }

        /// <summary>
        /// Current stage in UI workflow for creation
        /// </summary>
        public CreationStage Stage
        {
            get => state.Stage;
            set => state.Stage = value;
        }

        /// <summary>
        /// Namespace to utilize for all classes
        /// </summary>
        public string Namespace
        {
            get => state.Namespace;
            set => state.Namespace = value;
        }

        /// <summary>
        /// Log of errors and updates thus far in the create operation of the new extension service classes
        /// </summary>
        public string CreationLog { get => creationLog.ToString(); }

        /// <summary>
        /// Current result of extension service file(s) create operation
        /// </summary>
        public CreateResult Result { get; private set; } = CreateResult.None;

        /// <summary>
        /// Name of interface to create for new extension service. Value is ServiceName with leading "I"
        /// </summary>
        public string InterfaceName { get => "I" + ServiceName; }

        /// <summary>
        /// Name of ScriptableObject profile class to create. Value is ServiceName concatenated with "Profile"
        /// </summary>
        public string ProfileName { get => ServiceName + "Profile"; }

        /// <summary>
        /// Name of Unity inspector class to create. Value is ServiceName concatenated with "Inspector"
        /// </summary>
        public string InspectorName { get => ServiceName + "Inspector"; }

        /// <summary>
        /// Name of default ScriptableObject instance asset to create. Value is "Default" concatenated with ProfileName
        /// </summary>
        public string ProfileAssetName { get => "Default" + ProfileName; }

        /// <summary>
        /// Unity object pointing to folder asset to place Service class file
        /// </summary>
        public UnityEngine.Object ServiceFolderObject
        {
            get => state.ServiceFolder;
            set
            {
                if (IsValidFolder(value))
                {
                    state.ServiceFolder = value;
                }
            }
        }

        /// <summary>
        /// Unity object pointing to folder asset to place Inspector class file, if applicable
        /// </summary>
        public UnityEngine.Object InspectorFolderObject
        {
            get => state.InspectorFolder;
            set
            {
                if (IsValidFolder(value))
                {
                    state.InspectorFolder = value;
                }
            }
        }

        /// <summary>
        /// Unity object pointing to folder asset to place interface file, if applicable
        /// </summary>
        public UnityEngine.Object InterfaceFolderObject
        {
            get => state.InterfaceFolder;
            set
            {
                if (IsValidFolder(value))
                {
                    state.InterfaceFolder = value;
                }
            }
        }

        /// <summary>
        /// Unity object pointing to folder asset to place ScriptableObject profile class file, if applicable
        /// </summary>
        public UnityEngine.Object ProfileFolderObject
        {
            get => state.ProfileFolder;
            set
            {
                if (IsValidFolder(value))
                {
                    state.ProfileFolder = value;
                }
            }
        }

        /// <summary>
        /// Unity object pointing to folder asset to place ScriptableObject profile asset file, if applicable
        /// </summary>
        public UnityEngine.Object ProfileAssetFolderObject
        {
            get => state.ProfileAssetFolder;
            set
            {
                if (IsValidFolder(value))
                {
                    state.ProfileAssetFolder = value;
                }
            }
        }

        /// <summary>
        /// System.Type of Extension Service created
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Object instance of ScriptableObject profile class for extension service created
        /// </summary>
        public BaseMixedRealityProfile ProfileInstance { get; private set; }

        /// <summary>
        /// Sample code string demonstrating example usage for new Extension service created
        /// </summary>
        public string SampleCode
        {
            get
            {
                string sampleCode = SampleCodeTemplate;
                sampleCode = sampleCode.Replace(InterfaceNameSearchString, InterfaceName);
                sampleCode = sampleCode.Replace(ServiceNameSearchString, ServiceFieldName);
                return sampleCode;
            }
        }

        #endregion

        #region private properties

        #region static

        private static readonly string DefaultGeneratedFolderName = "MixedRealityToolkit.Generated";
        private static readonly string DefaultExtensionsFolderName = "Extensions";
        private static readonly string DefaultExtensionNamespace = "Microsoft.MixedReality.Toolkit.Extensions";
        private static readonly string PersistentStateKey = "MRTK_ExtensionServiceWizard_State_Before_Recompilation";
        private static readonly string ScriptExtension = ".cs";
        private static readonly string ProfileExtension = ".asset";

        private static readonly string ServiceNameSearchString = "#SERVICE_NAME#";
        private static readonly string InspectorNameSearchString = "#INSPECTOR_NAME#";
        private static readonly string InterfaceNameSearchString = "#INTERFACE_NAME#";
        private static readonly string ProfileNameSearchString = "#PROFILE_NAME#";
        private static readonly string ProfileFieldNameSearchString = "#PROFILE_FIELD_NAME#";
        private static readonly string ConstructorSearchString = "#SERVICE_CONSTRUCTOR#";
        private static readonly string SupportedPlatformsSearchString = "#SUPPORTED_PLATFORMS_PARAM#";
        private static readonly string ExtensionNamespaceSearchString = "#NAMESPACE#";
        private static readonly string SampleCodeTemplate = "#INTERFACE_NAME# #SERVICE_NAME# = MixedRealityToolkit.Instance.GetService<#INTERFACE_NAME#>();";

        #endregion

        #region paths

        private string ExtensionsFolder => Path.Combine("Assets", DefaultGeneratedFolderName, DefaultExtensionsFolderName);
        private string ServiceTemplatePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Tools, "ExtensionServiceCreator/Templates/ExtensionScriptTemplate.txt");
        private string ServiceConstructorTemplatePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Tools, "ExtensionServiceCreator/Templates/ExtensionConstructorTemplate.txt");
        private string InspectorTemplatePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Tools, "ExtensionServiceCreator/Templates/ExtensionInspectorTemplate.txt");
        private string InterfaceTemplatePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Tools, "ExtensionServiceCreator/Templates/ExtensionInterfaceTemplate.txt");
        private string ProfileTemplatePath => MixedRealityToolkitFiles.MapRelativeFilePath(MixedRealityToolkitModuleType.Tools, "ExtensionServiceCreator/Templates/ExtensionProfileTemplate.txt");

        #endregion

        private string ServiceFieldName => Char.ToLowerInvariant(ServiceName[0]) + ServiceName.Substring(1);
        private string ProfileFieldName => Char.ToLowerInvariant(ProfileName[0]) + ProfileName.Substring(1);

        private string ServiceFolderPath
        {
            get { return ServiceFolderObject != null ? AssetDatabase.GetAssetPath(ServiceFolderObject) : string.Empty; }
        }

        private string InspectorFolderPath
        {
            get { return InspectorFolderObject != null ? AssetDatabase.GetAssetPath(InspectorFolderObject) : string.Empty; }
        }

        private string InterfaceFolderPath
        {
            get { return InterfaceFolderObject != null ? AssetDatabase.GetAssetPath(InterfaceFolderObject) : string.Empty; }
        }

        private string ProfileFolderPath
        {
            get { return ProfileFolderObject != null ? AssetDatabase.GetAssetPath(ProfileFolderObject) : string.Empty; }
        }

        private string ProfileAssetFolderPath
        {
            get { return ProfileAssetFolderObject != null ? AssetDatabase.GetAssetPath(ProfileAssetFolderObject) : string.Empty; }
        }

        private string ServiceTemplate;
        private string ServiceConstructorTemplate;
        private string InspectorTemplate;
        private string InterfaceTemplate;
        private string ProfileTemplate;
        private StringBuilder creationLog = new StringBuilder();
        private PersistentState state;

        #endregion

        #region public methods

        /// <summary>
        /// Save current creator state to session registry in Unity
        /// </summary>
        public void StoreState()
        {
            string stateString = JsonUtility.ToJson(state);
            SessionState.SetString(PersistentStateKey, stateString);
        }

        /// <summary>
        /// Reset current creator state to default and save
        /// </summary>
        public void ResetState()
        {
            SessionState.EraseString(PersistentStateKey);

            CreateDefaultState();

            StoreState();
        }

        /// <summary>
        /// Load creator state from unity SessionState
        /// </summary>
        public async void LoadStoredState()
        {
            // (We can't call SessionState from inside a constructor)
            // Check to see whether editor prefs exist of our persistent state
            // If it does, load that now and clear the state
            string persistentState = SessionState.GetString(PersistentStateKey, string.Empty);
            if (!string.IsNullOrEmpty(persistentState))
            {
                state = JsonUtility.FromJson<PersistentState>(persistentState);
                // If we got this far we know we were successful
                Result = CreateResult.Successful;
                // If we were interrupted during script creation, move to profile creation
                switch (Stage)
                {
                    case CreationStage.CreatingExtensionService:
                        await ResumeAssetCreationProcessAfterReload();
                        break;
                }
            }
            else
            {
                // Otherwise create a default state
                CreateDefaultState();
            }
        }

        /// <summary>
        /// Validate template assets
        /// </summary>
        /// <remarks>
        /// Adds items to errors log field if not valid
        /// </remarks>
        /// <returns>true if no errors encountered, false otherwise</returns>
        public bool ValidateAssets(List<string> errors)
        {
            if (ServiceTemplate == null)
            {
                if (!ReadTemplate(ServiceTemplatePath, ref ServiceTemplate))
                {
                    errors.Add($"Script template not found in {ServiceTemplatePath}");
                }
            }

            if (ServiceConstructorTemplate == null)
            {
                if (!ReadTemplate(ServiceConstructorTemplatePath, ref ServiceConstructorTemplate))
                {
                    errors.Add($"Script template not found in {ServiceConstructorTemplatePath}");
                }
            }

            if (InspectorTemplate == null)
            {
                if (!ReadTemplate(InspectorTemplatePath, ref InspectorTemplate))
                {
                    errors.Add($"Inspector template not found in {InspectorTemplatePath}");
                }
            }

            if (InterfaceTemplate == null)
            {
                if (!ReadTemplate(InterfaceTemplatePath, ref InterfaceTemplate))
                {
                    errors.Add($"Interface template not found in {InterfaceTemplatePath}");
                }
            }

            if (ProfileTemplate == null)
            {
                if (!ReadTemplate(ProfileTemplatePath, ref ProfileTemplate))
                {
                    errors.Add($"Profile template not found in {ProfileTemplatePath}");
                }
            }

            if (!AssetDatabase.IsValidFolder(ExtensionsFolder))
            {
                var generatedFolder = Path.Combine("Assets", DefaultGeneratedFolderName);
                if (!AssetDatabase.IsValidFolder(generatedFolder))
                {
                    AssetDatabase.CreateFolder("Assets", DefaultGeneratedFolderName);
                }

                AssetDatabase.CreateFolder(generatedFolder, DefaultExtensionsFolderName);
                AssetDatabase.Refresh();

                // Setting the default folders is necessary after the asset database refresh
                // to ensure that the extension service creator's consumers will not need
                // to manually set the location in a separate step.
                SetAllFolders(ExtensionsFolder);
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Return true if configured Extension Service class name is valid. False otherwise
        /// </summary>
        /// <remarks>
        /// Adds items to errors log field if not valid
        /// </remarks>
        public bool ValidateName(List<string> errors)
        {
            if (string.IsNullOrEmpty(ServiceName))
            {
                errors.Add("Name cannot be empty.");
                return false;
            }

            if (!ServiceName.EndsWith("Service"))
            {
                errors.Add("Name must end with 'Service' suffix.");
            }

            if (!CSharpCodeProvider.CreateProvider("C#").IsValidIdentifier(ServiceName))
            {
                errors.Add("Name must not contain illegal characters.");
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Returns true if the asset supplied, via Folder object representing path and file name string (assuming .cs files only), does not exist. False otherwise
        /// </summary>
        public bool CanBuildAsset(UnityEngine.Object folder, string fileName)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            return IsValidFolder(folderPath) && !AssetExists(folderPath, fileName, ScriptExtension);
        }

        /// <summary>
        /// Returns true if the folder path supplied by Folder object is a valid location in the Unity project, false otherwise
        /// </summary>
        public bool IsValidFolder(UnityEngine.Object folder)
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            return IsValidFolder(folderPath);
        }

        /// <summary>
        /// Returns true if the folder path string supplied is a valid location in the Unity project, false otherwise
        /// </summary>
        public bool IsValidFolder(string folderPath)
        {
            return AssetDatabase.IsValidFolder(folderPath);
        }

        /// <summary>
        /// Validate that SupportedPlatforms is not zero.
        /// </summary>
        /// <remarks>
        /// Adds items to errors log field if not valid
        /// </remarks>
        public bool ValidatePlatforms(List<string> errors)
        {
            if (Platforms == 0)
            {
                errors.Add("Service must support at least one platform.");
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Validate namespace property with each class/interface file to be created for new extension service
        /// </summary>
        /// <remarks>
        /// Adds items to errors log field if not valid
        /// </remarks>
        /// <returns>true if no errors, false otherwise</returns>
        public bool ValidateNamespace(List<string> errors)
        {
            if (string.IsNullOrEmpty(Namespace))
            {
                Namespace = DefaultExtensionNamespace;
            }

            string[] assets = { ServiceName, InspectorName, InterfaceName, ProfileName };
            for (int i = 0; i < assets.Length; i++)
            {
                Type serviceType = Type.GetType($"{Namespace}.{assets[i]}");
                if (serviceType != null)
                {
                    errors.Add($"The type '{assets[i]}' already exists in this namespace.");
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Start the creation process for all relevant extension service files based on current creator property settings
        /// </summary>
        public async Task BeginAssetCreationProcess()
        {
            await Task.Yield();

            Stage = CreationStage.CreatingExtensionService;
            Result = CreateResult.Successful;

            creationLog.Clear();

            // At this point, we're ready to store a temporary state in editor prefs
            StoreState();

            string serviceAsset = CreateTextAssetFromTemplate(ServiceTemplate);

            // For service template only, look at adding a constructor if a profile is created
            string svcConstructor = string.Empty;
            if (UsesProfile)
            {
                svcConstructor = CreateTextAssetFromTemplate(ServiceConstructorTemplate);
            }

            serviceAsset = serviceAsset.Replace(ConstructorSearchString, svcConstructor);

            WriteTextAssetToDisk(serviceAsset, ServiceName, ServiceFolderPath);
            if (Result == CreateResult.Error) { return; }

            // This delay is purely for visual flow
            await Task.Delay(100);
            if (UsesInspector)
            {
                string inspectorAsset = CreateTextAssetFromTemplate(InspectorTemplate);
                WriteTextAssetToDisk(inspectorAsset, InspectorName, InspectorFolderPath);
                if (Result == CreateResult.Error) { return; }
            }

            // This delay is purely for visual flow
            await Task.Delay(100);
            string interfaceAsset = CreateTextAssetFromTemplate(InterfaceTemplate);
            WriteTextAssetToDisk(interfaceAsset, InterfaceName, InterfaceFolderPath);
            if (Result == CreateResult.Error) { return; }

            // This delay is purely for visual flow
            await Task.Delay(100);
            string profileAsset = string.Empty;
            if (UsesProfile)
            {
                profileAsset = CreateTextAssetFromTemplate(ProfileTemplate);
                WriteTextAssetToDisk(profileAsset, ProfileName, ProfileFolderPath);
                if (Result == CreateResult.Error) { return; }
            }

            // Wait a moment, then refresh the database and save our assets
            await Task.Delay(100);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            // Subscribe to Unity's log output so we can detect compilation errors
            Application.logMessageReceived += LogMessageReceived;

            // Wait for scripts to finish compiling
            while (EditorApplication.isCompiling)
            {
                await Task.Delay(100);
            }

            // Unsubscribe
            Application.logMessageReceived -= LogMessageReceived;
            // If we've gotten this far, it means that there was a compilation error
            // Otherwise this object would have been wiped from memory

            Result = CreateResult.Error;
            Stage = CreationStage.Finished;
        }

        #endregion

        #region private methods

        private async Task ResumeAssetCreationProcessAfterReload()
        {
            await Task.Yield();

            Result = CreateResult.Successful;
            Stage = CreationStage.CreatingProfileInstance;

            // Wait for scripts to finish compiling
            while (EditorApplication.isCompiling)
            {
                await Task.Delay(100);
            }

            // Search for our service type up front
            ServiceType = FindServiceType(ServiceName);
            if (ServiceType == null)
            {
                Stage = CreationStage.Finished;
                Result = CreateResult.Error;
                creationLog.AppendLine($"<color=red>Couldn't find type {ServiceName} in loaded assemblies.</color>");
                return;
            }

            // If this service doesn't use a profile, skip this step
            if (!UsesProfile)
            {
                Stage = CreationStage.Finished;
                creationLog.AppendLine("<color=red>Service does not use profile - skipping profile creation.</color>");
                return;
            }

            try
            {
                ScriptableObject profileInstance = ScriptableObject.CreateInstance(Namespace + "." + ProfileName);
                if (profileInstance == null)
                {
                    creationLog.AppendLine($"<color=red>Couldn't create instance of profile class {Namespace}.{ProfileName} - aborting</color>");
                    Result = CreateResult.Error;
                    return;
                }

                string profilePath = System.IO.Path.Combine(ProfileFolderPath, ProfileAssetName + ProfileExtension);
                profileInstance.name = ProfileAssetName;

                // Save the asset and refresh
                AssetDatabase.CreateAsset(profileInstance, profilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Force import the asset so it works with object reference values in serialized props
                AssetDatabase.ImportAsset(profilePath, ImportAssetOptions.ForceUpdate);

                // Load asset immediately to ensure it was created, and for registration later
                ProfileInstance = AssetDatabase.LoadAssetAtPath<BaseMixedRealityProfile>(profilePath);
                if (ProfileInstance == null)
                {
                    creationLog.AppendLine("<color=red>Couldn't load profile instance after creation!</color>");
                    Stage = CreationStage.Finished;
                    Result = CreateResult.Error;
                    return;
                }
            }
            catch (Exception e)
            {
                creationLog.AppendLine("<color=red>Exception when creating profile instance</color>");
                creationLog.AppendLine(e.ToString());
                Stage = CreationStage.Finished;
                Result = CreateResult.Error;
                return;
            }

            Stage = CreationStage.Finished;
        }

        private bool ReadTemplate(string templatePath, ref string template)
        {
            string dataPath = Application.dataPath.Replace("/Assets", string.Empty);
            string path = System.IO.Path.Combine(dataPath, templatePath);

            try
            {
                template = System.IO.File.ReadAllText(path);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                return false;
            }

            return !string.IsNullOrEmpty(template);
        }

        private void CreateDefaultState()
        {
            state = new PersistentState();
            state.ServiceName = "NewService";
            state.UsesProfile = true;
            state.UsesInspector = true;
            state.Stage = CreationStage.SelectNameAndPlatform;
            state.Platforms = SupportedPlatforms.LinuxStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal;

            SetAllFolders(ExtensionsFolder);
        }

        public bool SetAllFolders(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                return false;
            }

            var newFolder = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            ServiceFolderObject = newFolder;
            InterfaceFolderObject = newFolder;
            InspectorFolderObject = newFolder;
            ProfileFolderObject = newFolder;
            ProfileAssetFolderObject = newFolder;

            return true;
        }

        private bool AssetExists(string assetPath, string assetName, string extension)
        {
            string path = System.IO.Path.Combine(assetPath, assetName + extension);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            return asset != null;
        }

        private string CreateTextAssetFromTemplate(string templateText)
        {
            string scriptContents = templateText;
            scriptContents = scriptContents.Replace(ServiceNameSearchString, ServiceName);
            scriptContents = scriptContents.Replace(InspectorNameSearchString, InspectorName);
            scriptContents = scriptContents.Replace(InterfaceNameSearchString, InterfaceName);
            scriptContents = scriptContents.Replace(ProfileNameSearchString, ProfileName);
            scriptContents = scriptContents.Replace(ProfileFieldNameSearchString, ProfileFieldName);
            scriptContents = scriptContents.Replace(ExtensionNamespaceSearchString, Namespace);

            const int SUPPORTED_PLATFORM_EVERYTHING = -1;
            if ((int)Platforms == SUPPORTED_PLATFORM_EVERYTHING)
            {
                scriptContents = scriptContents.Replace(SupportedPlatformsSearchString, "(SupportedPlatforms)(-1)");
            }
            else
            {
                List<string> platformValues = new List<string>();

                foreach (SupportedPlatforms platform in Enum.GetValues(typeof(SupportedPlatforms)))
                {
                    if (Platforms.HasFlag(platform))
                    {
                        platformValues.Add("SupportedPlatforms." + platform.ToString());
                    }
                }

                scriptContents = scriptContents.Replace(SupportedPlatformsSearchString, string.Join("|", platformValues.ToArray()));
            }

            if (string.IsNullOrEmpty(scriptContents))
            {
                Result = CreateResult.Error;
                creationLog.AppendLine("<color=red>Script contents were empty, aborting.</color>");
            }

            return scriptContents;
        }

        private void WriteTextAssetToDisk(string contents, string assetName, string folderPath)
        {
            string localPath = folderPath + "/" + assetName + ScriptExtension;
            creationLog.AppendLine("Creating " + localPath);
            try
            {
                System.IO.File.WriteAllText(localPath, contents);
            }
            catch (Exception e)
            {
                Result = CreateResult.Error;
                creationLog.AppendLine($"<b><color=red>Exception throw writing to file {localPath}.</color></b>");
                creationLog.AppendLine(e.ToString());
            }
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    creationLog.AppendLine("<b><color=red>Encountered error while compiling</color></b>");
                    creationLog.AppendLine(condition);
                    Result = CreateResult.Error;
                    break;
                default:
                    break;
            }
        }

        private static Type FindServiceType(string serviceClassName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetLoadableTypes())
                {
                    if (!type.IsClass || type.IsAutoClass || type.IsAbstract || type.IsGenericType || type.IsArray) { continue; }

                    if (type.Name.Equals(serviceClassName)) { return type; }
                }
            }

            return null;
        }

        #endregion
    }
}

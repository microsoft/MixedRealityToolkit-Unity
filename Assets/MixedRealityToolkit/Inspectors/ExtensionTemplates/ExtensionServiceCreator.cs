// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.CSharp;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Class used to generate service scripts and profile instances
    /// </summary>
    public class ExtensionServiceCreator
    {
        #region enums and types

        /// <summary>
        /// Result of create operation
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
            public string ServiceFolderPath;
            public string InspectorFolderPath;
            public string InterfaceFolderPath;
            public string ProfileFolderPath;
            public string ProfileAssetFolderPath;
            public string Namespace;
        }

        #endregion

        #region static

        private static readonly string DefaultExtensionNamespace = "Microsoft.MixedReality.Toolkit.Extensions";
        private static readonly string PersistentStateKey = "MRTK_ExtensionServiceWizard_State_Before_Recompilation";
        private static readonly string DefaultExtensionsFolder = "Assets/MixedRealityToolkit.Extensions";
        private static readonly string DefaultExtensionsFolderName = "MixedRealityToolkit.Extensions";
        private static readonly string ServiceTemplatePath = "ExtensionTemplates/ExtensionScriptTemplate.txt";
        private static readonly string InspectorTemplatePath = "ExtensionTemplates/ExtensionInspectorTemplate.txt";
        private static readonly string InterfaceTemplatePath = "ExtensionTemplates/ExtensionInterfaceTemplate.txt";
        private static readonly string ProfileTemplatePath = "ExtensionTemplates/ExtensionProfileTemplate.txt";
        private static readonly string ScriptExtension = ".cs";
        private static readonly string ProfileExtension = ".asset";
        private static readonly string ServiceNameSearchString = "#SERVICE_NAME#";
        private static readonly string InspectorNameSearchString = "#INSPECTOR_NAME#";
        private static readonly string InterfaceNameSearchString = "#INTERFACE_NAME#";
        private static readonly string ProfileNameSearchString = "#PROFILE_NAME#";
        private static readonly string ProfileFieldNameSearchString = "#PROFILE_FIELD_NAME#";
        private static readonly string SupportedPlatformsSearchString = "#SUPPORTED_PLATFORMS_PARAM#";
        private static readonly string ExtensionNamespaceSearchString = "#NAMESPACE#";
        private static readonly string SampleCodeTemplate = "#INTERFACE_NAME# #SERVICE_NAME# = MixedRealityToolkit.Instance.GetService<#INTERFACE_NAME#>();";

        #endregion

        #region public properties

        public string ServiceName
        {
            get { return state.ServiceName; }
            set { state.ServiceName = value; }
        }

        public bool UsesProfile
        {
            get { return state.UsesProfile; }
            set { state.UsesProfile = value; }
        }

        public bool UsesInspector
        {
            get { return state.UsesInspector; }
            set { state.UsesInspector = value; }
        }

        public SupportedPlatforms Platforms
        {
            get { return state.Platforms; }
            set { state.Platforms = value; }
        }

        public CreationStage Stage
        {
            get { return state.Stage; }
            set { state.Stage = value; }
        }

        public string Namespace
        {
            get { return state.Namespace; }
            set { state.Namespace = value; }
        }

        public IEnumerable<string> CreationLog { get { return creationLog; } }
        public CreateResult Result { get; private set; } = CreateResult.None;
        public string InterfaceName { get { return "I" + ServiceName; } }
        public string ProfileName { get { return ServiceName + "Profile"; } }
        public string InspectorName { get { return ServiceName + "Inspector"; } }
        public string ServiceFieldName { get { return Char.ToLowerInvariant(ServiceName[0]) + ServiceName.Substring(1); } }
        public string ProfileFieldName { get { return Char.ToLowerInvariant(ProfileName[0]) + ProfileName.Substring(1); } }
        public string ProfileAssetName { get { return "Default" + ProfileName; } }

        public UnityEngine.Object ServiceFolderObject { get; set; }
        public UnityEngine.Object InspectorFolderObject { get; set; }
        public UnityEngine.Object InterfaceFolderObject { get; set; }
        public UnityEngine.Object ProfileFolderObject { get; set; }
        public UnityEngine.Object ProfileAssetFolderObject { get; set; }

        public Type ServiceType { get; private set; }
        public BaseMixedRealityProfile ProfileInstance { get; private set; }

        #endregion

        #region private properties

        private string ServiceFolderPath
        {
            get { return state.ServiceFolderPath; }
            set { state.ServiceFolderPath = value; }
        }

        private string InspectorFolderPath
        {
            get { return state.InspectorFolderPath; }
            set { state.InspectorFolderPath = value; }
        }

        private string InterfaceFolderPath
        {
            get { return state.InterfaceFolderPath; }
            set { state.InterfaceFolderPath = value; }
        }

        private string ProfileFolderPath
        {
            get { return state.ProfileFolderPath; }
            set { state.ProfileFolderPath = value; }
        }

        private string ProfileAssetFolderPath
        {
            get { return state.ProfileAssetFolderPath; }
            set { state.ProfileAssetFolderPath = value; }
        }

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

        private string ServiceTemplate;
        private string InspectorTemplate;
        private string InterfaceTemplate;
        private string ProfileTemplate;

        #endregion

        #region private fields

        private List<string> creationLog = new List<string>();
        private PersistentState state;

        #endregion

        #region public methods

        public void StoreState()
        {
            string stateString = JsonUtility.ToJson(state);
            SessionState.SetString(PersistentStateKey, stateString);
        }

        public void ResetState()
        {
            Debug.Log("Resetting state");
            SessionState.EraseString(PersistentStateKey);
        }

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

        public bool ValidateAssets(List<string> errors)
        {
            errors.Clear();

            if (ServiceTemplate == null)
            {
                if (!ReadTemplate(ServiceTemplatePath, ref ServiceTemplate))
                    errors.Add("Script template not found in " + ServiceTemplatePath);             
            }

            if (InspectorTemplate == null)
            {
                if (!ReadTemplate(InspectorTemplatePath, ref InspectorTemplate))
                    errors.Add("Inspector template not found in " + InspectorTemplatePath);
            }

            if (InterfaceTemplate == null)
            {
                if (!ReadTemplate(InterfaceTemplatePath, ref InterfaceTemplate))
                    errors.Add("Interface template not found in " + InterfaceTemplatePath);
            }

            if (ProfileTemplate == null)
            {
                if (!ReadTemplate(ProfileTemplatePath, ref ProfileTemplate))
                    errors.Add("Profile template not found in " + ProfileTemplatePath);
            }

            if (!AssetDatabase.IsValidFolder(DefaultExtensionsFolder))
            {
                AssetDatabase.CreateFolder("Assets", DefaultExtensionsFolderName);
                AssetDatabase.Refresh();
            }

            return errors.Count == 0;
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

        public bool ValidateName(List<string> errors)
        {
            errors.Clear();

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

        public bool ValidateFolders(List<string> errors)
        {
            errors.Clear();

            if (ServiceFolderObject == null)
                ServiceFolderObject = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(DefaultExtensionsFolder, typeof(UnityEngine.Object));

            if (InspectorFolderObject == null && ServiceFolderObject != null)
                InspectorFolderObject = ServiceFolderObject;

            if (InterfaceFolderObject == null && ServiceFolderObject != null)
                InterfaceFolderObject = ServiceFolderObject;

            if (ProfileFolderObject == null && ServiceFolderObject != null)
                ProfileFolderObject = ServiceFolderObject;

            if (ProfileAssetFolderObject == null && ServiceFolderObject != null)
                ProfileAssetFolderObject = ServiceFolderObject;

            ServiceFolderPath = ServiceFolderObject != null ? AssetDatabase.GetAssetPath(ServiceFolderObject) : string.Empty;
            InspectorFolderPath = InspectorFolderObject != null ? AssetDatabase.GetAssetPath(InspectorFolderObject) : string.Empty;
            InterfaceFolderPath = InterfaceFolderObject != null ? AssetDatabase.GetAssetPath(InterfaceFolderObject) : string.Empty;
            ProfileFolderPath = ProfileFolderObject != null ? AssetDatabase.GetAssetPath(ProfileFolderObject) : string.Empty;
            ProfileAssetFolderPath = ProfileAssetFolderObject != null ? AssetDatabase.GetAssetPath(ProfileAssetFolderObject) : string.Empty;

            // Make sure the folders exist and aren't other assets
            if (!AssetDatabase.IsValidFolder(ServiceFolderPath))
                errors.Add("Service folder is not valid.");

            if (!AssetDatabase.IsValidFolder(InspectorFolderPath))
                errors.Add("Inspector folder is not valid.");

            if (!AssetDatabase.IsValidFolder(InterfaceFolderPath))
                errors.Add("Interface folder is not valid.");

            if (!AssetDatabase.IsValidFolder(ProfileFolderPath))
                errors.Add("Profile folder is not valid.");

            if (!AssetDatabase.IsValidFolder(ProfileAssetFolderPath))
                errors.Add("Profile asset folder is not valid.");

            // Make sure there aren't already assets with the same name
            if (AssetExists(ServiceFolderPath, ServiceName, ScriptExtension))
                errors.Add("Service script asset already exists. Delete it or choose a different service name to continue.");

            if (AssetExists(InspectorFolderPath, InspectorName, ScriptExtension))
                errors.Add("Inspector script asset already exists. Delete it or choose a different service name to continue.");

            if (AssetExists(InterfaceFolderPath, InterfaceName, ScriptExtension))
                errors.Add("Interface script asset already exists. Delete it or choose a different service name to continue.");

            if (AssetExists(ProfileFolderPath, ProfileName, ScriptExtension))
                errors.Add("Profile script asset already exists. Delete it or choose a different service name to continue.");

            if (AssetExists(ProfileAssetFolderPath, ProfileAssetName, ProfileExtension))
                errors.Add("Profile asset already exists. Delete it or choose a different service name to continue.");

            return errors.Count == 0;
        }

        public bool ValidatePlatforms(List<string> errors)
        {
            errors.Clear();

            if ((int)Platforms == 0)
            {
                errors.Add("Service must support at least one platform.");
            }

            return errors.Count == 0;
        }

        public bool ValidateNamespace(List<string> errors)
        {
            errors.Clear();

            if (string.IsNullOrEmpty(Namespace))
            {
                Namespace = DefaultExtensionNamespace;
            }

            // Check if a class with this name already exists
            Type serviceType = Type.GetType(Namespace + "." + ServiceName);
            if (serviceType != null)
            {
                errors.Add("The type '" + ServiceName + "' already exists in this namespace.");
            }

            Type inspectorType = Type.GetType(Namespace + ".Editor." + InspectorName);
            if (serviceType != null)
            {
                errors.Add("The type '" + InspectorName + "' already exists in this namespace.");
            }

            Type interfaceType = Type.GetType(Namespace + "." + InterfaceName);
            if (interfaceType != null)
            {
                errors.Add("The type '" + InterfaceName + "' already exists in this namespace.");
            }

            Type profileType = Type.GetType(Namespace + "." + ProfileName);
            if (profileType != null)
            {
                errors.Add("The type '" + ProfileName + "' already exists in this namespace.");
            }

            return errors.Count == 0;
        }

        public async Task BeginAssetCreationProcess()
        {
            await Task.Yield();

            Stage = CreationStage.CreatingExtensionService;
            Result = CreateResult.Successful;

            // At this point, we're ready to store a temporary state in editor prefs
            StoreState();

            string serviceAsset = CreateTextAssetFromTemplate(ServiceTemplate);
            WriteTextAssetToDisk(serviceAsset, ServiceName, ServiceFolderPath);
            if (Result == CreateResult.Error)
                return;

            // This delay is purely for visual flow
            await Task.Delay(100);
            string inspectorAsset = CreateTextAssetFromTemplate(InspectorTemplate);
            WriteTextAssetToDisk(inspectorAsset, InspectorName, InspectorFolderPath);
            if (Result == CreateResult.Error)
                return;

            // This delay is purely for visual flow
            await Task.Delay(100);
            string interfaceAsset = CreateTextAssetFromTemplate(InterfaceTemplate);
            WriteTextAssetToDisk(interfaceAsset, InterfaceName, InterfaceFolderPath);
            if (Result == CreateResult.Error)
                return;

            // This delay is purely for visual flow
            await Task.Delay(100);
            string profileAsset = string.Empty;
            if (UsesProfile)
            {
                profileAsset = CreateTextAssetFromTemplate(ProfileTemplate);
                WriteTextAssetToDisk(profileAsset, ProfileName, ProfileFolderPath);
                if (Result == CreateResult.Error)
                    return;
            }

            // Wait a moment, then refresh the database and save our assets
            await Task.Delay(100);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            await Task.Delay(100);

            // Subscribe to Unity's log output so we can detect compilation errors
            Application.logMessageReceived += LogMessageReceived;

            // Wait for scripts to finish compiling
            while (EditorApplication.isCompiling)
                await Task.Delay(100);

            // Unsubscribe
            Application.logMessageReceived -= LogMessageReceived;
            // If we've gotten this far, it means that there was a compilation error
            // Otherwise this object would have been wiped from memory
        }

        public async Task ResumeAssetCreationProcessAfterReload()
        {
            await Task.Yield();

            Result = CreateResult.Successful;
            Stage = CreationStage.CreatingProfileInstance;
            
            // Wait for scripts to finish compiling
            while (EditorApplication.isCompiling)
                await Task.Delay(100);

            // Search for our service type up front
            ServiceType = FindServiceType(ServiceName);
            if (ServiceType == null)
            {
                Stage = CreationStage.Finished;
                Result = CreateResult.Error;
                creationLog.Add("Couldn't find type " + ServiceName + " in loaded assemblies.");
                return;
            }

            // If this service doesn't use a profile, skip this step
            if (!UsesProfile)
            {
                Stage = CreationStage.Finished;
                creationLog.Add("Service does not use profile - skipping profile creation.");
                return;
            }

            try
            {
                ScriptableObject profileInstance = ScriptableObject.CreateInstance(DefaultExtensionNamespace + "." + ProfileName);
                if (profileInstance == null)
                {
                    creationLog.Add("Couldn't create instance of profile class " + DefaultExtensionNamespace + "." + ProfileName + " - aborting");
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
                    creationLog.Add("Couldn't load profile instance after creation!");
                    Stage = CreationStage.Finished;
                    Result = CreateResult.Error;
                    return;
                }
            }
            catch (Exception e)
            {
                creationLog.Add("Exception when creating profile instance");
                creationLog.Add(e.ToString());
                Stage = CreationStage.Finished;
                Result = CreateResult.Error;
                return;
            }

            Stage = CreationStage.Finished;
        }

        #endregion

        #region private methods

        private void CreateDefaultState()
        {
            state = new PersistentState();
            state.ServiceName = "NewService";
            state.UsesProfile = true;
            state.UsesInspector = true;
            state.Stage = CreationStage.SelectNameAndPlatform;
            state.Platforms = SupportedPlatforms.LinuxStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal;
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
            scriptContents = scriptContents.Replace(ExtensionNamespaceSearchString, DefaultExtensionNamespace);

            List<string> platformValues = new List<string>();
            foreach (SupportedPlatforms platform in Enum.GetValues(typeof(SupportedPlatforms)))
            {
                if ((platform & Platforms) != 0)
                    platformValues.Add("SupportedPlatforms." + platform.ToString());
            }
            scriptContents = scriptContents.Replace(SupportedPlatformsSearchString, String.Join("|", platformValues.ToArray()));

            if (string.IsNullOrEmpty(scriptContents))
            {
                Result = CreateResult.Error;
                creationLog.Add("Script contents were empty, aborting.");
            }

            return scriptContents;
        }

        private void WriteTextAssetToDisk(string contents, string assetName, string folderPath)
        {
            string localPath = folderPath + "/" + assetName + ScriptExtension;
            creationLog.Add("Creating " + localPath);
            try
            {
                System.IO.File.WriteAllText(localPath, contents);
            }
            catch (Exception e)
            {
                Result = CreateResult.Error;
                creationLog.Add(e.ToString());
            }
        }

        private void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    creationLog.Add("Encountered error while compiling");
                    creationLog.Add(condition);
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
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAutoClass || type.IsAbstract || type.IsGenericType || type.IsArray)
                        continue;

                    if (type.Name.Equals(serviceClassName))
                        return type;
                }
            }

            return null;
        }

        #endregion
    }
}
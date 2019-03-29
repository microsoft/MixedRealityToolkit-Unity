using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ExtensionServiceWizard : EditorWindow
{
    private static ExtensionServiceWizard window;
    private static readonly Color enabledColor = Color.white;
    private static readonly Color disabledColor = Color.gray;
    private static readonly Color readOnlyColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

    private ExtensionServiceCreator creator = new ExtensionServiceCreator();
    private List<string> errors = new List<string>();

    // These are stored prior to compilation to ensure results are not wiped out
    private ExtensionServiceCreator.CreateResult result;
    private List<string> resultOutput = new List<string>();

    [MenuItem("Mixed Reality Toolkit/Create Extension Service...", false, 1)]
    private static void CreateExtensionServiceMenuItem()
    {
        if (window != null)
        {
            Debug.Log("Only one window allowed at a time");
            // Only allow one window at a time
            return;
        }
        
        window = EditorWindow.CreateInstance<ExtensionServiceWizard>();
        window.titleContent = new GUIContent("Create Extension Service");
        window.ResetCreator();
        window.Show(true);
    }

    private void ResetCreator()
    {
        if (creator == null)
            creator = new ExtensionServiceCreator();

        creator.ResetState();
    }

    private void OnEnable()
    {
        Debug.Log("Initializing ExtensionServiceWizard window");
        if (creator == null)
            creator = new ExtensionServiceCreator();

        creator.LoadStoredState();
    }

    private void OnGUI()
    {
        if (!creator.ValidateAssets(errors))
        {
            EditorGUILayout.LabelField("Validating assets...", EditorStyles.miniLabel);
            foreach (string error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
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
        EditorGUILayout.LabelField("Choose a name for your service.", EditorStyles.miniLabel);

        creator.ServiceName = EditorGUILayout.TextField("Service Name", creator.ServiceName);

        bool readyToProgress = creator.ValidateName(errors);
        foreach (string error in errors)
        {
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Choose which platforms your service will support.", EditorStyles.miniLabel);

        creator.Platforms = (SupportedPlatforms)EditorGUILayout.EnumFlagsField("Platforms", creator.Platforms);
        if (!creator.ValidatePlatforms(errors))
        {
            readyToProgress = false;
            foreach (string error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
        }
        GUILayout.FlexibleSpace();

        GUI.color = readyToProgress ? enabledColor : disabledColor;
        if (GUILayout.Button("Next") && readyToProgress)
        {
            creator.Stage = ExtensionServiceCreator.CreationStage.ChooseOutputFolders;
            creator.StoreState();
        }
    }

    private void DrawChooseOutputFolders()
    {
        GUI.color = enabledColor;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Below are the files you will be generating", EditorStyles.miniLabel);

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(creator.ServiceName + ".cs", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("This is the main script for your service. It functions simliarly to a MonoBehaviour, with Enable, Disable and Update functions.", EditorStyles.wordWrappedMiniLabel);
        creator.ServiceFolderObject = EditorGUILayout.ObjectField("Target Folder", creator.ServiceFolderObject, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(creator.InterfaceName + ".cs", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("This is the interface that other scripts will use to interact with your service.", EditorStyles.wordWrappedMiniLabel);
        creator.InterfaceFolderObject = EditorGUILayout.ObjectField("Target Folder", creator.InterfaceFolderObject, typeof(UnityEngine.Object), false);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(creator.ProfileName + ".cs", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("An optional profile script for your service. Profiles are scriptable objects that store permanent config data. If you're not sure whether your service will need a profile, it's best to create one. You can remove it later.", EditorStyles.wordWrappedMiniLabel);
        creator.UsesProfile = EditorGUILayout.Toggle("Generate Profile", creator.UsesProfile);
        if (creator.UsesProfile)
        {
            creator.ProfileFolderObject = EditorGUILayout.ObjectField("Target Folder", creator.ProfileFolderObject, typeof(UnityEngine.Object), false);
        }
        EditorGUILayout.EndVertical();

        if (creator.UsesProfile)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(creator.ProfileAssetName + ".asset", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("A default instance of your profile.", EditorStyles.wordWrappedMiniLabel);
            creator.ProfileAssetFolderObject = EditorGUILayout.ObjectField("Target Folder", creator.ProfileAssetFolderObject, typeof(UnityEngine.Object), false);
            EditorGUILayout.EndVertical();
        }

        GUI.color = enabledColor;
        EditorGUILayout.Space();

        bool readyToProgress = creator.ValidateFolders(errors);
        foreach (string error in errors)
        {
            EditorGUILayout.HelpBox(error, MessageType.Error);
        }

        GUILayout.FlexibleSpace();

        GUI.color = enabledColor;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Back"))
        {
            creator.Stage = ExtensionServiceCreator.CreationStage.SelectNameAndPlatform;
            creator.StoreState();
        }
        GUI.color = readyToProgress ? enabledColor : disabledColor;
        if (GUILayout.Button("Next") && readyToProgress)
        {
            // Start the async method that will wait for the service to be created
            CreateAssetsAsync();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawCreatingAssets()
    {
        EditorGUILayout.LabelField("Creating assets...", EditorStyles.boldLabel);

        switch (creator.Result)
        {
            case ExtensionServiceCreator.CreateResult.Error:
                EditorGUILayout.HelpBox("There were errors while creating assets.", MessageType.Error);
                break;

            default:
                break;
        }

        foreach (string info in creator.CreationLog)
        {
            EditorGUILayout.LabelField(info, EditorStyles.wordWrappedMiniLabel);
        }

        Repaint();
    }

    private void DrawFinished()
    {
        switch (creator.Result)
        {
            case ExtensionServiceCreator.CreateResult.Successful:
                EditorGUILayout.HelpBox("Your service scripts have been created. Would you like to register this service in your current MixedRealityToolkit profile?", MessageType.Info);
                foreach (string info in creator.CreationLog)
                {
                    EditorGUILayout.LabelField(info, EditorStyles.wordWrappedMiniLabel);
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Register"))
                {

                }
                if (GUILayout.Button("Not Now"))
                {

                }
                EditorGUILayout.EndHorizontal();
                break;

            case ExtensionServiceCreator.CreateResult.Error:
                EditorGUILayout.HelpBox("There were errors during the creation process:", MessageType.Error);
                foreach (string info in creator.CreationLog)
                {
                    EditorGUILayout.LabelField(info, EditorStyles.wordWrappedMiniLabel);
                }

                GUILayout.FlexibleSpace();
                if(GUILayout.Button("Close"))
                {
                    Close();
                }
                break;
        }
    }

    private async void CreateAssetsAsync()
    {
        await creator.BeginAssetCreationProcess();
    }

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
            public SupportedPlatforms Platforms;
            public CreationStage Stage;
        }

        #endregion

        #region static

        private static readonly string ExtensionNamespace               = "Microsoft.MixedReality.Toolkit.Extensions";
        private static readonly string PersistentStateKey               = "MRTK_ExtensionServiceWizard_State_Before_Recompilation";
        private static readonly string DefaultExtensionsFolder          = "Assets/MixedRealityToolkit.Extensions";
        private static readonly string DefaultExtensionsFolderName      = "MixedRealityToolkit.Extensions";
        private static readonly string ServiceTemplatePath              = "Assets/MixedRealityToolkit/Inspectors/ExtensionTemplates/ExtensionScriptTemplate.txt";
        private static readonly string InterfaceTemplatePath            = "Assets/MixedRealityToolkit/Inspectors/ExtensionTemplates/ExtensionInterfaceTemplate.txt";
        private static readonly string ProfileTemplatePath              = "Assets/MixedRealityToolkit/Inspectors/ExtensionTemplates/ExtensionProfileTemplate.txt";
        private static readonly string ScriptExtension                  = ".cs";
        private static readonly string ProfileExtension                 = ".asset";
        private static readonly string ServiceNameSearchString          = "#SERVICE_NAME#";
        private static readonly string InterfaceNameSearchString        = "#INTERFACE_NAME#";
        private static readonly string ProfileNameSearchString          = "#PROFILE_NAME#";
        private static readonly string ProfileFieldNameSearchString     = "#PROFILE_FIELD_NAME#";
        private static readonly string SupportedPlatformsSearchString   = "#SUPPORTED_PLATFORMS_PARAM#";
        private static readonly string ExtensionNamespaceSearchString   = "#NAMESPACE#";

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

        public IEnumerable<string> CreationLog { get { return creationLog; } }
        public CreateResult Result { get; private set; } = CreateResult.None;
        public string InterfaceName { get { return "I" + ServiceName; } }
        public string ProfileName { get { return ServiceName + "Profile"; } }
        public string ProfileFieldName { get { return Char.ToLowerInvariant(ProfileName[0]) + ProfileName.Substring(1); } }
        public string ProfileAssetName { get { return "Default" + ProfileName; } }

        public UnityEngine.Object ServiceFolderObject { get; set; }
        public UnityEngine.Object InterfaceFolderObject { get; set; }
        public UnityEngine.Object ProfileFolderObject { get; set; }
        public UnityEngine.Object ProfileAssetFolderObject { get; set; }

        #endregion

        #region private properties

        private string ServiceFolderPath { get; set; }
        private string InterfaceFolderPath { get; set; }
        private string ProfileFolderPath { get; set; }
        private string ProfileAssetFolderPath { get; set; }

        private TextAsset ServiceTemplate { get; set; }
        private TextAsset InterfaceTemplate { get; set; }
        private TextAsset ProfileTemplate { get; set; }

        private ScriptableObject ProfileInstance { get; set; }

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
                ServiceTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(ServiceTemplatePath);
                if (ServiceTemplate == null)
                    errors.Add("Script template not found in " + ServiceTemplatePath);
            }

            if (InterfaceTemplate == null)
            {
                InterfaceTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(InterfaceTemplatePath);
                if (InterfaceTemplate == null)
                    errors.Add("Interface template not found in " + InterfaceTemplatePath);
            }

            if (ProfileTemplate == null)
            {
                ProfileTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(ProfileTemplatePath);
                if (ProfileTemplate == null)
                    errors.Add("Profile template not found in " + ProfileTemplatePath);
            }

            if (!AssetDatabase.IsValidFolder(DefaultExtensionsFolder))
            {
                AssetDatabase.CreateFolder("Assets", DefaultExtensionsFolderName);
                AssetDatabase.Refresh();
            }

            return errors.Count == 0;
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
                errors.Add("Name must end with 'Service' suffix");
            }

            return errors.Count == 0;
        }

        public bool ValidateFolders(List<string> errors)
        {
            errors.Clear();

            if (ServiceFolderObject == null)
                ServiceFolderObject = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(DefaultExtensionsFolder, typeof(UnityEngine.Object));

            if (InterfaceFolderObject == null && ServiceFolderObject != null)
                InterfaceFolderObject = ServiceFolderObject;

            if (ProfileFolderObject == null && ServiceFolderObject != null)
                ProfileFolderObject = ServiceFolderObject;

            if (ProfileAssetFolderObject == null && ServiceFolderObject != null)
                ProfileAssetFolderObject = ServiceFolderObject;

            ServiceFolderPath = ServiceFolderObject != null ? AssetDatabase.GetAssetPath(ServiceFolderObject) : string.Empty;
            InterfaceFolderPath = InterfaceFolderObject != null ? AssetDatabase.GetAssetPath(InterfaceFolderObject) : string.Empty;
            ProfileFolderPath = ProfileFolderObject != null ? AssetDatabase.GetAssetPath(ProfileFolderObject) : string.Empty;
            ProfileAssetFolderPath = ProfileAssetFolderObject != null ? AssetDatabase.GetAssetPath(ProfileAssetFolderObject) : string.Empty;

            // Make sure the folders exist and aren't other assets
            if (!AssetDatabase.IsValidFolder(ServiceFolderPath))
                errors.Add("Service folder is not valid.");

            if (!AssetDatabase.IsValidFolder(InterfaceFolderPath))
                errors.Add("Interface folder is not valid.");

            if (!AssetDatabase.IsValidFolder(ProfileFolderPath))
                errors.Add("Profile folder is not valid.");

            if (!AssetDatabase.IsValidFolder(ProfileAssetFolderPath))
                errors.Add("Profile asset folder is not valid.");

            // Make sure there aren't already assets with the same name
            if (AssetExists(ServiceFolderPath, ServiceName, ScriptExtension))
                errors.Add("Service script asset already exists. Delete it or choose a different service name to continue.");

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

        public async Task BeginAssetCreationProcess()
        {
            await Task.Yield();

            Stage = CreationStage.CreatingExtensionService;
            Result = CreateResult.Successful;

            // At this point, we're ready to store a temporary state in editor prefs
            StoreState();

            string serviceAsset = CreateTextAssetFromTemplate(ServiceTemplate.text);
            WriteTextAssetToDisk(serviceAsset, ServiceName, ServiceFolderPath);
            if (Result == CreateResult.Error)
                return;

            await Task.Delay(100);
            string interfaceAsset = CreateTextAssetFromTemplate(InterfaceTemplate.text);
            WriteTextAssetToDisk(interfaceAsset, InterfaceName, InterfaceFolderPath);
            if (Result == CreateResult.Error)
                return;

            await Task.Delay(100);
            string profileAsset = string.Empty;
            if (UsesProfile)
            {
                profileAsset = CreateTextAssetFromTemplate(ProfileTemplate.text);
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

            // If this service doesn't use a profile, skip this step
            if (!UsesProfile)
            {
                Stage = CreationStage.Finished;
                creationLog.Add("Service does not use profile - skipping profile creation.");
                return;
            }

            ScriptableObject profileInstance = ScriptableObject.CreateInstance(ExtensionNamespace + "." + ProfileName);
            if (profileInstance == null)
            {
                creationLog.Add("Couldn't create instance of profile class " + ExtensionNamespace + "." + ProfileName + " - aborting");
                Result = CreateResult.Error;
                return;
            }

            profileInstance.name = ProfileAssetName;
            AssetDatabase.CreateAsset(profileInstance, System.IO.Path.Combine(ProfileFolderPath, ProfileAssetName + ProfileExtension));
            AssetDatabase.SaveAssets();

            Stage = CreationStage.Finished;
        }

        #endregion

        #region private methods

        private void CreateDefaultState()
        {
            state = new PersistentState();
            state.ServiceName = "NewService";
            state.UsesProfile = true;
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
            scriptContents = scriptContents.Replace(InterfaceNameSearchString, InterfaceName);
            scriptContents = scriptContents.Replace(ProfileNameSearchString, ProfileName);
            scriptContents = scriptContents.Replace(ProfileFieldNameSearchString, ProfileFieldName);
            scriptContents = scriptContents.Replace(ExtensionNamespaceSearchString, ExtensionNamespace);

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
            string absolutePath = System.IO.Path.Combine(Application.dataPath, localPath);
            creationLog.Add("Creating " + absolutePath);
            try
            {
                System.IO.File.WriteAllText(localPath, contents);
            }
            catch(Exception e)
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

        #endregion
    }
}

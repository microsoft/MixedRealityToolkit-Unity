// ----------------------------------------------------------------------------
// <copyright file="PhotonEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   MenuItems and in-Editor scripts for PhotonNetwork.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExitGames.Client.Photon;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public class PunWizardText
{
    public string WindowTitle = "PUN Wizard";
    public string SetupWizardWarningTitle = "Warning";
    public string SetupWizardWarningMessage = "You have not yet run the Photon setup wizard! Your game won't be able to connect. See Windows -> Photon Unity Networking.";
    public string MainMenuButton = "Main Menu";
    public string SetupWizardTitle = "PUN Setup";
    public string SetupWizardInfo = "Thanks for importing Photon Unity Networking.\nThis window should set you up.\n\n<b>-</b> To use an existing Photon Cloud App, enter your AppId.\n<b>-</b> To register an account or access an existing one, enter the account's mail address.\n<b>-</b> To use Photon OnPremise, skip this step.";
    public string EmailOrAppIdLabel = "AppId or Email";
    public string AlreadyRegisteredInfo = "The email is registered so we can't fetch your AppId (without password).\n\nPlease login online to get your AppId and paste it above.";
    public string SkipRegistrationInfo = "Skipping? No problem:\nEdit your server settings in the PhotonServerSettings file.";
    public string RegisteredNewAccountInfo = "We created a (free) account and fetched you an AppId.\nWelcome. Your PUN project is setup.";
    public string AppliedToSettingsInfo = "Your AppId is now applied to this project.";
    public string SetupCompleteInfo = "<b>Done!</b>\nAll connection settings can be edited in the <b>PhotonServerSettings</b> now.\nHave a look.";
    public string CloseWindowButton = "Close";
    public string SkipButton = "Skip";
    public string SetupButton = "Setup Project";
    public string MobileExportNoteLabel = "Build for mobiles impossible. Get PUN+ or Unity 4 Pro for mobile or use Unity 5 or newer.";
    public string MobilePunPlusExportNoteLabel = "PUN+ available. Using native sockets for iOS/Android.";
    public string CancelButton = "Cancel";
    public string PUNWizardLabel = "PUN Wizard";
    public string SettingsButton = "Settings";
    public string SetupServerCloudLabel = "Setup wizard for setting up your own server or the cloud.";
    public string WarningPhotonDisconnect = "";
    public string StartButton = "Start";
    public string LocateSettingsButton = "Locate PhotonServerSettings";
    public string SettingsHighlightLabel = "Highlights the used photon settings file in the project.";
    public string DocumentationLabel = "Documentation";
    public string OpenPDFText = "Reference PDF";
    public string OpenPDFTooltip = "Opens the local documentation pdf.";
    public string OpenDevNetText = "DevNet / Manual";
    public string OpenDevNetTooltip = "Online documentation for Photon.";
    public string OpenCloudDashboardText = "Cloud Dashboard Login";
    public string OpenCloudDashboardTooltip = "Review Cloud App information and statistics.";
    public string OpenForumText = "Open Forum";
    public string OpenForumTooltip = "Online support for Photon.";
    public string OkButton = "Ok";
    public string OwnHostCloudCompareLabel = "I am not quite sure how 'my own host' compares to 'cloud'.";
    public string ComparisonPageButton = "Cloud versus OnPremise";
    public string ConnectionTitle = "Connecting";
    public string ConnectionInfo = "Connecting to the account service...";
    public string ErrorTextTitle = "Error";
    public string IncorrectRPCListTitle = "Warning: RPC-list becoming incompatible!";
    public string IncorrectRPCListLabel = "Your project's RPC-list is full, so we can't add some RPCs just compiled.\n\nBy removing outdated RPCs, the list will be long enough but incompatible with older client builds!\n\nMake sure you change the game version where you use PhotonNetwork.ConnectUsingSettings().";
    public string RemoveOutdatedRPCsLabel = "Remove outdated RPCs";
    public string FullRPCListTitle = "Warning: RPC-list is full!";
    public string FullRPCListLabel = "Your project's RPC-list is too long for PUN.\n\nYou can change PUN's source to use short-typed RPC index. Look for comments 'LIMITS RPC COUNT'\n\nAlternatively, remove some RPC methods (use more parameters per RPC maybe).\n\nAfter a RPC-list refresh, make sure you change the game version where you use PhotonNetwork.ConnectUsingSettings().";
    public string SkipRPCListUpdateLabel = "Skip RPC-list update";
    public string PUNNameReplaceTitle = "Warning: RPC-list Compatibility";
    public string PUNNameReplaceLabel = "PUN replaces RPC names with numbers by using the RPC-list. All clients must use the same list for that.\n\nClearing it most likely makes your client incompatible with previous versions! Change your game version or make sure the RPC-list matches other clients.";
    public string RPCListCleared = "Clear RPC-list";
    public string ServerSettingsCleanedWarning = "Cleared the PhotonServerSettings.RpcList! This makes new builds incompatible with older ones. Better change game version in PhotonNetwork.ConnectUsingSettings().";
    public string RpcFoundMessage = "Some code uses the obsolete RPC attribute. PUN now requires the PunRPC attribute to mark remote-callable methods.\nThe Editor can search and replace that code which will modify your source.";
    public string RpcFoundDialogTitle = "RPC Attribute Outdated";
    public string RpcReplaceButton = "Replace. I got a backup.";
    public string RpcSkipReplace = "Not now.";
    public string WizardMainWindowInfo = "This window should help you find important settings for PUN, as well as documentation.";
}


[InitializeOnLoad]
public class PhotonEditor : EditorWindow
{
    protected static Type WindowType = typeof (PhotonEditor);

    protected Vector2 scrollPos = Vector2.zero;

    private readonly Vector2 preferredSize = new Vector2(350, 400);

    private static Texture2D BackgroundImage;

    public static PunWizardText CurrentLang = new PunWizardText();


    protected static AccountService.Origin RegisterOrigin = AccountService.Origin.Pun;

    protected static string DocumentationLocation = "Assets/Photon Unity Networking/PhotonNetwork-Documentation.pdf";

    protected static string UrlFreeLicense = "https://dashboard.photonengine.com/en-US/SelfHosted";

    protected static string UrlDevNet = "https://doc.photonengine.com/en-us/pun/current";

    protected static string UrlForum = "https://forum.photonengine.com";

    protected static string UrlCompare = "https://doc.photonengine.com/en-us/realtime/current/getting-started/onpremise-or-saas";

    protected static string UrlHowToSetup = "https://doc.photonengine.com/en-us/onpremise/current/getting-started/photon-server-in-5min";

    protected static string UrlAppIDExplained = "https://doc.photonengine.com/en-us/realtime/current/getting-started/obtain-your-app-id";

    protected static string UrlAccountPage = "https://dashboard.photonengine.com/Account/SignIn?email="; // opened in browser

    protected static string UrlCloudDashboard = "https://dashboard.photonengine.com?email=";


    private enum PhotonSetupStates
    {
        MainUi,

        RegisterForPhotonCloud,

        EmailAlreadyRegistered,

        GoEditPhotonServerSettings
    }

    private bool isSetupWizard = false;

    private PhotonSetupStates photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;


    private bool minimumInput = false;
    private bool useMail = false;
    private bool useAppId = false;
    private bool useSkip = false;
    private bool highlightedSettings = false;
    private bool close = false;
    private string mailOrAppId = string.Empty;


    private static double lastWarning = 0;
    private static bool postCompileActionsDone;

    private static bool isPunPlus;
    private static bool androidLibExists;
    private static bool iphoneLibExists;


    // setup once on load
    static PhotonEditor()
    {
		#if UNITY_2017_2_OR_NEWER
		EditorApplication.playModeStateChanged += PlaymodeStateChanged;
		#else
		EditorApplication.playmodeStateChanged += PlaymodeStateChanged;
		#endif

        #if UNITY_2018
		EditorApplication.projectChanged += EditorUpdate;
        EditorApplication.hierarchyChanged += EditorUpdate;
        #else
        EditorApplication.projectWindowChanged += EditorUpdate;
        EditorApplication.hierarchyWindowChanged += EditorUpdate;
        #endif
        EditorApplication.update += OnUpdate;

        // detect optional packages
        PhotonEditor.CheckPunPlus();
    }

    // setup per window
    public PhotonEditor()
    {
        minSize = this.preferredSize;
    }

    [MenuItem("Window/Photon Unity Networking/PUN Wizard &p", false, 0)]
    protected static void MenuItemOpenWizard()
    {
        PhotonEditor win = GetWindow(WindowType, false, CurrentLang.WindowTitle, true) as PhotonEditor;
        win.photonSetupState = PhotonSetupStates.MainUi;
        win.isSetupWizard = false;
    }

    [MenuItem("Window/Photon Unity Networking/Highlight Server Settings %#&p", false, 1)]
    protected static void MenuItemHighlightSettings()
    {
        HighlightSettings();
    }

    /// <summary>Creates an Editor window, showing the cloud-registration wizard for Photon (entry point to setup PUN).</summary>
    protected static void ShowRegistrationWizard()
    {
        PhotonEditor win = GetWindow(WindowType, false, CurrentLang.WindowTitle, true) as PhotonEditor;
        win.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
        win.isSetupWizard = true;
    }


    // called 100 times / sec
    private static void OnUpdate()
    {
        // after a compile, check RPCs to create a cache-list
        if (!postCompileActionsDone && !EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode && PhotonNetwork.PhotonServerSettings != null)
        {
			#if UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
            if (EditorApplication.isUpdating)
            {
                return;
            }
            #endif

            PhotonEditor.UpdateRpcList();
            postCompileActionsDone = true; // on compile, this falls back to false (without actively doing anything)

			#if UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
            PhotonEditor.ImportWin8Support();
            #endif
        }
    }


    // called in editor, opens wizard for initial setup, keeps scene PhotonViews up to date and closes connections when compiling (to avoid issues)
    private static void EditorUpdate()
    {
        if (PhotonNetwork.PhotonServerSettings == null)
        {
            PhotonNetwork.CreateSettings();
        }
        if (PhotonNetwork.PhotonServerSettings == null)
        {
            return;
        }

        // serverSetting is null when the file gets deleted. otherwise, the wizard should only run once and only if hosting option is not (yet) set
        if (!PhotonNetwork.PhotonServerSettings.DisableAutoOpenWizard && PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
        {
            ShowRegistrationWizard();
            PhotonNetwork.PhotonServerSettings.DisableAutoOpenWizard = true;
            PhotonEditor.SaveSettings();
        }

        // Workaround for TCP crash. Plus this surpresses any other recompile errors.
        if (EditorApplication.isCompiling)
        {
            if (PhotonNetwork.connected)
            {
                if (lastWarning > EditorApplication.timeSinceStartup - 3)
                {
                    // Prevent error spam
                    Debug.LogWarning(CurrentLang.WarningPhotonDisconnect);
                    lastWarning = EditorApplication.timeSinceStartup;
                }

                PhotonNetwork.Disconnect();
            }
        }
    }


    // called in editor on change of play-mode (used to show a message popup that connection settings are incomplete)
	#if UNITY_2017_2_OR_NEWER
    private static void PlaymodeStateChanged(PlayModeStateChange state)
    #else
    private static void PlaymodeStateChanged()
    #endif
    {
        if (EditorApplication.isPlaying || !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
        {
            EditorUtility.DisplayDialog(CurrentLang.SetupWizardWarningTitle, CurrentLang.SetupWizardWarningMessage, CurrentLang.OkButton);
        }
    }


    #region GUI and Wizard

    // Window Update() callback. On-demand, when Window is open
    protected void Update()
    {
        if (this.close)
        {
            Close();
        }
    }

    protected virtual void OnGUI()
    {
        if (BackgroundImage == null)
        {
            BackgroundImage = AssetDatabase.LoadAssetAtPath("Assets/Photon Unity Networking/Editor/PhotonNetwork/background.jpg", typeof(Texture2D)) as Texture2D;
        }

        PhotonSetupStates oldGuiState = this.photonSetupState; // used to fix an annoying Editor input field issue: wont refresh until focus is changed.

        GUI.SetNextControlName("");
        this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);


        if (this.photonSetupState == PhotonSetupStates.MainUi)
        {
            UiMainWizard();
        }
        else
        {
            UiSetupApp();
        }


        GUILayout.EndScrollView();

        if (oldGuiState != this.photonSetupState)
        {
            GUI.FocusControl("");
        }
    }


    protected virtual void UiSetupApp()
    {
        GUI.skin.label.wordWrap = true;
        if (!this.isSetupWizard)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CurrentLang.MainMenuButton, GUILayout.ExpandWidth(false)))
            {
                this.photonSetupState = PhotonSetupStates.MainUi;
            }

            GUILayout.EndHorizontal();
        }


        // setup header
        UiTitleBox(CurrentLang.SetupWizardTitle, BackgroundImage);

        // setup info text
        GUI.skin.label.richText = true;
        GUILayout.Label(CurrentLang.SetupWizardInfo);

        // input of appid or mail
        EditorGUILayout.Separator();
        GUILayout.Label(CurrentLang.EmailOrAppIdLabel);
        this.mailOrAppId = EditorGUILayout.TextField(this.mailOrAppId).Trim(); // note: we trim all input

        if (this.mailOrAppId.Contains("@"))
        {
            // this should be a mail address
            this.minimumInput = (this.mailOrAppId.Length >= 5 && this.mailOrAppId.Contains("."));
            this.useMail = this.minimumInput;
            this.useAppId = false;
        }
        else
        {
            // this should be an appId
            this.minimumInput = ServerSettings.IsAppId(this.mailOrAppId);
            this.useMail = false;
            this.useAppId = this.minimumInput;
        }

        // button to skip setup
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(CurrentLang.SkipButton, GUILayout.Width(100)))
        {
            this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
            this.useSkip = true;
            this.useMail = false;
            this.useAppId = false;
        }

        // SETUP button
        EditorGUI.BeginDisabledGroup(!this.minimumInput);
        if (GUILayout.Button(CurrentLang.SetupButton, GUILayout.Width(100)))
        {
            this.useSkip = false;
            GUIUtility.keyboardControl = 0;
            if (this.useMail)
            {
                RegisterWithEmail(this.mailOrAppId); // sets state
            }
            if (this.useAppId)
            {
                this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
                Undo.RecordObject(PhotonNetwork.PhotonServerSettings, "Update PhotonServerSettings for PUN");
                PhotonNetwork.PhotonServerSettings.UseCloud(this.mailOrAppId);
                PhotonEditor.SaveSettings();
            }
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        // existing account needs to fetch AppId online
        if (this.photonSetupState == PhotonSetupStates.EmailAlreadyRegistered)
        {
            // button to open dashboard and get the AppId
            GUILayout.Space(15);
            GUILayout.Label(CurrentLang.AlreadyRegisteredInfo);


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(CurrentLang.OpenCloudDashboardText, CurrentLang.OpenCloudDashboardTooltip), GUILayout.Width(205)))
            {
				Application.OpenURL(UrlCloudDashboard + Uri.EscapeUriString(this.mailOrAppId));
                this.mailOrAppId = "";
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        if (this.photonSetupState == PhotonSetupStates.GoEditPhotonServerSettings)
        {
            if (!this.highlightedSettings)
            {
                this.highlightedSettings = true;
                HighlightSettings();
            }

            GUILayout.Space(15);
            if (this.useSkip)
            {
                GUILayout.Label(CurrentLang.SkipRegistrationInfo);
            }
            else if (this.useMail)
            {
                GUILayout.Label(CurrentLang.RegisteredNewAccountInfo);
            }
            else if (this.useAppId)
            {
                GUILayout.Label(CurrentLang.AppliedToSettingsInfo);
            }


            // setup-complete info
            GUILayout.Space(15);
            GUILayout.Label(CurrentLang.SetupCompleteInfo);


            // close window (done)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CurrentLang.CloseWindowButton, GUILayout.Width(205)))
            {
                this.close = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUI.skin.label.richText = false;
    }

    private void UiTitleBox(string title, Texture2D bgIcon)
    {
        GUIStyle bgStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        bgStyle.normal.background = bgIcon;
        bgStyle.fontSize = 22;
        bgStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        Rect scale = GUILayoutUtility.GetLastRect();
        scale.height = 30;

        GUI.Label(scale, title, bgStyle);
        GUILayout.Space(scale.height+5);
    }

    protected virtual void UiMainWizard()
    {
        GUILayout.Space(15);

        // title
        UiTitleBox(CurrentLang.PUNWizardLabel, BackgroundImage);

        // wizard info text
        GUILayout.Label(CurrentLang.WizardMainWindowInfo);
        GUILayout.Space(15);


        // pun+ info
        if (isPunPlus)
        {
            GUILayout.Label(CurrentLang.MobilePunPlusExportNoteLabel);
            GUILayout.Space(15);
        }
#if !(UNITY_5_0 || UNITY_5 || UNITY_5_3_OR_NEWER)
        else if (!InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(BuildTarget.Android) || !InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(BuildTarget.iPhone))
        {
            GUILayout.Label(CurrentLang.MobileExportNoteLabel);
            GUILayout.Space(15);
        }
#endif

        // settings button
        GUILayout.BeginHorizontal();
        GUILayout.Label(CurrentLang.SettingsButton, EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.BeginVertical();
        if (GUILayout.Button(new GUIContent(CurrentLang.LocateSettingsButton, CurrentLang.SettingsHighlightLabel)))
        {
            HighlightSettings();
        }
        if (GUILayout.Button(new GUIContent(CurrentLang.OpenCloudDashboardText, CurrentLang.OpenCloudDashboardTooltip)))
        {
            Application.OpenURL(UrlCloudDashboard + Uri.EscapeUriString(this.mailOrAppId));
        }
        if (GUILayout.Button(new GUIContent(CurrentLang.SetupButton, CurrentLang.SetupServerCloudLabel)))
        {
            this.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(15);


        EditorGUILayout.Separator();


        // documentation
        GUILayout.BeginHorizontal();
        GUILayout.Label(CurrentLang.DocumentationLabel, EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.BeginVertical();
        if (GUILayout.Button(new GUIContent(CurrentLang.OpenPDFText, CurrentLang.OpenPDFTooltip)))
        {
            EditorUtility.OpenWithDefaultApp(DocumentationLocation);
        }

        if (GUILayout.Button(new GUIContent(CurrentLang.OpenDevNetText, CurrentLang.OpenDevNetTooltip)))
        {
            Application.OpenURL(UrlDevNet);
        }

        GUI.skin.label.wordWrap = true;
        GUILayout.Label(CurrentLang.OwnHostCloudCompareLabel);
        if (GUILayout.Button(CurrentLang.ComparisonPageButton))
        {
            Application.OpenURL(UrlCompare);
        }


        if (GUILayout.Button(new GUIContent(CurrentLang.OpenForumText, CurrentLang.OpenForumTooltip)))
        {
			Application.OpenURL(UrlForum);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    #endregion


    protected virtual void RegisterWithEmail(string email)
    {
        EditorUtility.DisplayProgressBar(CurrentLang.ConnectionTitle, CurrentLang.ConnectionInfo, 0.5f);

        string accountServiceType = string.Empty;
        if (PhotonEditorUtils.HasVoice)
        {
            accountServiceType = "voice";
        }


        AccountService client = new AccountService();
        client.RegisterByEmail(email, RegisterOrigin, accountServiceType); // this is the synchronous variant using the static RegisterOrigin. "result" is in the client

        EditorUtility.ClearProgressBar();
        if (client.ReturnCode == 0)
        {
            this.mailOrAppId = client.AppId;
            PhotonNetwork.PhotonServerSettings.UseCloud(this.mailOrAppId, 0);
            if (PhotonEditorUtils.HasVoice)
            {
                PhotonNetwork.PhotonServerSettings.VoiceAppID = client.AppId2;
            }
            PhotonEditor.SaveSettings();

            this.photonSetupState = PhotonSetupStates.GoEditPhotonServerSettings;
        }
        else
        {
            PhotonNetwork.PhotonServerSettings.HostType = ServerSettings.HostingOption.PhotonCloud;
            PhotonEditor.SaveSettings();

            Debug.LogWarning(client.Message + " ReturnCode: " + client.ReturnCode);
            if (client.Message.Contains("registered"))
            {
                this.photonSetupState = PhotonSetupStates.EmailAlreadyRegistered;
            }
            else
            {
                EditorUtility.DisplayDialog(CurrentLang.ErrorTextTitle, client.Message, CurrentLang.OkButton);
                this.photonSetupState = PhotonSetupStates.RegisterForPhotonCloud;
            }
        }
    }


    protected internal static bool CheckPunPlus()
    {
		androidLibExists = 	File.Exists("Assets/Plugins/Android/armeabi-v7a/libPhotonSocketPlugin.so") &&
							File.Exists("Assets/Plugins/Android/x86/libPhotonSocketPlugin.so");


        iphoneLibExists = File.Exists("Assets/Plugins/IOS/libPhotonSocketPlugin.a");

        isPunPlus = androidLibExists || iphoneLibExists;
        return isPunPlus;
    }


    private static void ImportWin8Support()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return; // don't import while compiling
        }

		#if UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_3_OR_NEWER
        const string win8Package = "Assets/Plugins/Photon3Unity3D-Win8.unitypackage";

        bool win8LibsExist = File.Exists("Assets/Plugins/WP8/Photon3Unity3D.dll") && File.Exists("Assets/Plugins/Metro/Photon3Unity3D.dll");
        if (!win8LibsExist && File.Exists(win8Package))
        {
            AssetDatabase.ImportPackage(win8Package, false);
        }
        #endif
    }


    // Pings PhotonServerSettings and makes it selected (show in Inspector)
    private static void HighlightSettings()
    {
        Selection.objects = new UnityEngine.Object[] { PhotonNetwork.PhotonServerSettings };
        EditorGUIUtility.PingObject(PhotonNetwork.PhotonServerSettings);
    }


    // Marks settings object as dirty, so it gets saved.
    // unity 5.3 changes the usecase for SetDirty(). but here we don't modify a scene object! so it's ok to use
    private static void SaveSettings()
    {
        EditorUtility.SetDirty(PhotonNetwork.PhotonServerSettings);
    }


    #region RPC List Handling

    public static void UpdateRpcList()
    {
        List<string> additionalRpcs = new List<string>();
        HashSet<string> currentRpcs = new HashSet<string>();

        var types = GetAllSubTypesInScripts(typeof(MonoBehaviour));

        //int countOldRpcs = 0;
        foreach (var mono in types)
        {
            MethodInfo[] methods = mono.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo method in methods)
            {
                //bool isOldRpc = false;
                //#pragma warning disable 618
                //// we let the Editor check for outdated RPC attributes in code. that should not cause a compile warning
                //if (method.IsDefined(typeof (RPC), false))
                //{
                //    countOldRpcs++;
                //    isOldRpc = true;
                //}
                //#pragma warning restore 618

                //if (isOldRpc || method.IsDefined(typeof(PunRPC), false))
                if (method.IsDefined(typeof(PunRPC), false))
                {
                    currentRpcs.Add(method.Name);

                    if (!additionalRpcs.Contains(method.Name) && !PhotonNetwork.PhotonServerSettings.RpcList.Contains(method.Name))
                    {
                        additionalRpcs.Add(method.Name);
                    }
                }
            }
        }

        if (additionalRpcs.Count > 0)
        {
            // LIMITS RPC COUNT
            if (additionalRpcs.Count + PhotonNetwork.PhotonServerSettings.RpcList.Count >= byte.MaxValue)
            {
                if (currentRpcs.Count <= byte.MaxValue)
                {
                    bool clearList = EditorUtility.DisplayDialog(CurrentLang.IncorrectRPCListTitle, CurrentLang.IncorrectRPCListLabel, CurrentLang.RemoveOutdatedRPCsLabel, CurrentLang.CancelButton);
                    if (clearList)
                    {
                        PhotonNetwork.PhotonServerSettings.RpcList.Clear();
                        PhotonNetwork.PhotonServerSettings.RpcList.AddRange(currentRpcs);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog(CurrentLang.FullRPCListTitle, CurrentLang.FullRPCListLabel, CurrentLang.SkipRPCListUpdateLabel);
                    return;
                }
            }

            additionalRpcs.Sort();
            Undo.RecordObject(PhotonNetwork.PhotonServerSettings, "Update PUN RPC-list");
            PhotonNetwork.PhotonServerSettings.RpcList.AddRange(additionalRpcs);
            PhotonEditor.SaveSettings();
        }
    }

    public static void ClearRpcList()
    {
        bool clearList = EditorUtility.DisplayDialog(CurrentLang.PUNNameReplaceTitle, CurrentLang.PUNNameReplaceLabel, CurrentLang.RPCListCleared, CurrentLang.CancelButton);
        if (clearList)
        {
            PhotonNetwork.PhotonServerSettings.RpcList.Clear();
            Debug.LogWarning(CurrentLang.ServerSettingsCleanedWarning);
        }
    }

    public static System.Type[] GetAllSubTypesInScripts(System.Type aBaseClass)
    {
        var result = new System.Collections.Generic.List<System.Type>();
        System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var A in AS)
        {
            // this skips all but the Unity-scripted assemblies for RPC-list creation. You could remove this to search all assemblies in project
            if (!A.FullName.StartsWith("Assembly-"))
            {
                // Debug.Log("Skipping Assembly: " + A);
                continue;
            }

            //Debug.Log("Assembly: " + A.FullName);
            System.Type[] types = A.GetTypes();
            foreach (var T in types)
            {
                if (T.IsSubclassOf(aBaseClass))
                {
                    result.Add(T);
                }
            }
        }
        return result.ToArray();
    }

    #endregion

}
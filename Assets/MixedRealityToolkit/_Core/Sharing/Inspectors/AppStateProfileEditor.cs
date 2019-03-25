using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.Utilities;
using MRTK.Core;
using UnityEditor;
using UnityEngine;

namespace MRTK.StateControl
{
    [CustomEditor(typeof(AppStateProfile))]
    public class AppStateProfileEditor : Editor
    {
        private const int stateTypeColumnWidth = 150;
        private const int useColumnWidth = 75;
        private const int arrayTypeColumnWidth = 150;
        private const int subscribeColumnWidth = 100;
        private const int searchButtonMaxWidth = 400;

        private static HashSet<string> visibleTypes = new HashSet<string>();
        private static bool searchedForAvailableTypes = false;
        private static List<Type> availableTypes = new List<Type>();
        private static List<Type> stateArrayTypes = new List<Type>();

        private SerializedProperty appRole;
        private SerializedProperty stateTypeDefinitions;
        private SerializedProperty subscriptionMode;
        private AppStateProfile profile;

        private Color enabledColor = Color.white;
        private Color disabledColor = Color.Lerp(Color.white, Color.clear, 0.5f);
        private GUIStyle centeredButtonStyle;
        private GUIStyle errorLabelStyle;

        private void OnEnable()
        {
            appRole = serializedObject.FindProperty("appRole");
            stateTypeDefinitions = serializedObject.FindProperty("stateTypeDefinitions");
            subscriptionMode = serializedObject.FindProperty("subscriptionMode");
            profile = (AppStateProfile)target;

            centeredButtonStyle = new GUIStyle(EditorStyles.miniButton);
            centeredButtonStyle.alignment = TextAnchor.MiddleCenter;

            errorLabelStyle = new GUIStyle(EditorStyles.miniLabel);
            errorLabelStyle.richText = true;
            errorLabelStyle.wordWrap = true;
        }

        public override void OnInspectorGUI()
        {
            GUI.color = enabledColor;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("App Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(appRole);

            switch (profile.AppRole)
            {
                case AppRoleEnum.Server:
                    EditorGUILayout.HelpBox("Server is responsible for creating session prior to clients joining. Shutting down the server stops synchronization.", MessageType.Info);
                    break;

                case AppRoleEnum.Client:
                    EditorGUILayout.HelpBox("Client may drop in / out of experience at any time. App state will be automatically synchronized.", MessageType.Info);
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Types", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Below is a list of the structs the AppState can synchronize. Eligable structs must have the [AppStateType] attribute. If you don't see your type here, click the search button.", MessageType.Info);

            GUI.color = enabledColor;
            if (DrawSearchButton("Search"))
            {
                HashSet<Type> existingTypes = new HashSet<Type>(profile.SynchronizedTypes);
                foreach (Type type in StateUtils.GetAllStateTypes())
                {
                    if (existingTypes.Contains(type))
                        continue;

                    // Insert a new type at index 0, then set it to the state type
                    stateTypeDefinitions.InsertArrayElementAtIndex(0);
                    SerializedProperty itemStateTypeStruct = stateTypeDefinitions.GetArrayElementAtIndex(0);
                    SerializedProperty itemStateType = itemStateTypeStruct.FindPropertyRelative("itemStateType");
                    SerializedProperty itemStateReference = itemStateType.FindPropertyRelative("reference");
                    itemStateReference.stringValue = type.AssemblyQualifiedName;

                    SerializedProperty useType = itemStateTypeStruct.FindPropertyRelative("synchronizeType");
                    useType.boolValue = true;
                }
            }

            if (stateTypeDefinitions.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No ItemState type definitions found. Use the search button.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.BeginVertical();

                // Draw table headers
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUILayout.LabelField("State Type", EditorStyles.miniLabel, GUILayout.MinWidth(stateTypeColumnWidth));
                EditorGUILayout.LabelField("Synchronize", EditorStyles.miniLabel, GUILayout.MinWidth(useColumnWidth));

                if (ScriptingBackendIsIL2CPP())
                {
                    EditorGUILayout.LabelField("Array Type", EditorStyles.miniLabel, GUILayout.MinWidth(arrayTypeColumnWidth));
                }

                switch (profile.SubscriptionMode)
                {
                    case SubscriptionModeEnum.All:
                        GUI.color = disabledColor;
                        break;

                    case SubscriptionModeEnum.Manual:
                        GUI.color = enabledColor;
                        break;
                }

                EditorGUILayout.LabelField("Subscribe", EditorStyles.miniLabel, GUILayout.MinWidth(subscribeColumnWidth));
                GUI.color = enabledColor;

                EditorGUILayout.EndHorizontal();

                HashSet<Type> existingStateTypes = new HashSet<Type>();
                for (int i = 0; i < stateTypeDefinitions.arraySize; i++)
                {
                    DrawStateTypeDefinition(i, existingStateTypes);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data Source", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The AppState requires a Data Source to handle networking. Below is the type you wish to use.", MessageType.Info);
            SerializedProperty dataSource = serializedObject.FindProperty("dataSourceType");
            EditorGUILayout.PropertyField(dataSource);

            switch (profile.AppRole)
            {
                case AppRoleEnum.Client:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Subscription Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(subscriptionMode);

                    switch (profile.SubscriptionMode)
                    {
                        case SubscriptionModeEnum.All:
                            EditorGUILayout.HelpBox("In this mode changes to all state types will be received by this client.", MessageType.Info);
                            break;

                        case SubscriptionModeEnum.Manual:
                            EditorGUILayout.HelpBox("In this mode only changes that are manually subscribed to will be received."
                                + " This can be useful for devices which only care about a subset of state types and want to save bandwidth."
                                + " These settings can also be changed at runtime.", MessageType.Info);
                            break;
                    }

                    break;

                case AppRoleEnum.Server:
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawStateTypeDefinition(int index, HashSet<Type> existingStateTypes)
        {
            SerializedProperty itemStateTypeStruct = stateTypeDefinitions.GetArrayElementAtIndex(index);
            SerializedProperty itemStateType = itemStateTypeStruct.FindPropertyRelative("itemStateType");
            SerializedProperty itemStateReference = itemStateType.FindPropertyRelative("reference");
            SerializedProperty synchronizeType = itemStateTypeStruct.FindPropertyRelative("synchronizeType");
            SerializedProperty subscribeToType = itemStateTypeStruct.FindPropertyRelative("subscribeToType");

            // Do validation
            if (string.IsNullOrEmpty(itemStateReference.stringValue))
            {
                stateTypeDefinitions.DeleteArrayElementAtIndex(index);
                return;
            }

            Type stateType = new SystemType(itemStateReference.stringValue).Type;
            if (stateType == null)
            {
                stateTypeDefinitions.DeleteArrayElementAtIndex(index);
                return;
            }

            if (existingStateTypes.Contains(stateType))
            {   // No duplicates
                stateTypeDefinitions.DeleteArrayElementAtIndex(index);
                return;
            }

            existingStateTypes.Add(stateType);
           
            // Begin columns
            EditorGUILayout.BeginHorizontal();
            GUI.color = synchronizeType.boolValue ? enabledColor : disabledColor;
            EditorGUILayout.LabelField(stateType.Name, GUILayout.MinWidth(arrayTypeColumnWidth));

            GUI.color = enabledColor;
            synchronizeType.boolValue = EditorGUILayout.Toggle(synchronizeType.boolValue, GUILayout.MinWidth(useColumnWidth));

            // Get our array type values here so we can issue an error after the layout is finished
            SerializedProperty stateArrayType = itemStateTypeStruct.FindPropertyRelative("stateArrayType");
            SerializedProperty stateArrayReference = stateArrayType.FindPropertyRelative("reference");
            Type arrayType = Type.GetType(stateArrayReference.stringValue);
            bool arrayTypeIsValid = arrayType != null;

            if (ScriptingBackendIsIL2CPP())
            {
                if (synchronizeType.boolValue)
                {
                    // See if we have an accompanying state array type defined
                    if (arrayType == null)
                    {
                        // Un-set the reference
                        stateArrayReference.stringValue = string.Empty;
                    }
                }

                GUI.color = synchronizeType.boolValue ? enabledColor : disabledColor;
                EditorGUILayout.LabelField(arrayTypeIsValid ? GetFriendlyName(arrayType) : "(Not valid)", GUILayout.MinWidth(arrayTypeColumnWidth));
            }

            switch (profile.AppRole)
            {
                case AppRoleEnum.Client:
                    switch (profile.SubscriptionMode)
                    {
                        case SubscriptionModeEnum.All:
                            GUI.color = disabledColor;
                            EditorGUILayout.Toggle(true, GUILayout.MinWidth(subscribeColumnWidth));
                            break;

                        case SubscriptionModeEnum.Manual:
                            GUI.color = synchronizeType.boolValue ? enabledColor : disabledColor; ;
                            subscribeToType.boolValue = EditorGUILayout.Toggle(subscribeToType.boolValue, GUILayout.MinWidth(subscribeColumnWidth));
                            break;
                    }
                    break;

                case AppRoleEnum.Server:
                    GUI.color = disabledColor;
                    EditorGUILayout.Toggle(true, GUILayout.MinWidth(subscribeColumnWidth));
                    break;
            }

            GUI.color = enabledColor;

            EditorGUILayout.EndHorizontal();

            // Display validation errors
            List<string> errors = new List<string>();
            if (!ValidateStateType(stateType, errors))
            {
                EditorGUILayout.HelpBox(stateType.Name + " had the following errors:", MessageType.Error);
                foreach (string error in errors)
                {
                    EditorGUILayout.LabelField(" • " + error, errorLabelStyle);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            if (!arrayTypeIsValid)
            {
                EditorGUILayout.HelpBox("No accompanying state array type set. In IL2CPP the type must be defined ahead of time. Click button to search.", MessageType.Error);

                if (DrawSearchButton("Search for state array type"))
                { 
                    Type resultType = StateUtils.GetStateArrayType(stateType);
                    if (resultType != null)
                    {
                        stateArrayReference.stringValue = resultType.AssemblyQualifiedName;
                    }
                }
            }
        }

        private static bool ValidateStateType(Type stateType, List<string> errors)
        {
            if (stateType.IsClass)
            {
                errors.Add(stateType.Name + " must be a struct.");
            }

            if (!stateType.IsSerializable)
            { 
                errors.Add(stateType.Name + " is not Serializable.");
            }

            object[] attributes = stateType.GetCustomAttributes(typeof(AppStateTypeAttribute), true);
            if (attributes.Length == 0)
            {
                errors.Add (stateType.Name + " doesn't have the AppStateType attribute.");
            }

            if (!typeof(IItemState).IsAssignableFrom(stateType))
            {
                errors.Add(stateType.Name + " doesn't implement IItemState interface.");
            }
            else
            {
                PropertyInfo keyProperty = stateType.GetProperty("Key");
                if (CheckIfAutoProperty(keyProperty))
                {
                    errors.Add(stateType.Name + " may use an auto-property for its Key property - this is not recommended as its value may not be serialized. Key should be a read-only property that returns a public field.");
                }
            }

            var initConstructor = stateType.GetConstructor(new Type[] { typeof(short) });
            if (initConstructor == null)
            {
                errors.Add(stateType.Name + " does not have a constructor that takes a key: <i>public " + stateType.Name + "(short key)</i>");
            }

            bool isComparer = stateType.GetInterfaces().Any(stateComparer =>
                        stateComparer.IsGenericType &&
                        stateComparer.GetGenericTypeDefinition() == typeof(IItemStateComparer<>));

            if (!isComparer)
            {
                errors.Add(stateType.Name + " doesn't implement IItemStateComparer<" + stateType.Name + "> interface");
            }

            return errors.Count == 0;
        }

        private static bool ValidateStateArrayType(Type stateArrayType, List<string> errors)
        {
            if (stateArrayType == null)
            {
                errors.Add("State array type is null");
                return false;
            }

            bool valid = true;

            return valid;
        }

        private static bool ScriptingBackendIsIL2CPP()
        {
           return PlayerSettings.GetScriptingBackend(ConvertBuildTarget(EditorUserBuildSettings.activeBuildTarget)) == ScriptingImplementation.IL2CPP;
        }

        private static string GetFriendlyName(Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        public static bool CheckIfAutoProperty(PropertyInfo info)
        {
            bool compilerGenerated = info.GetGetMethod().GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();

            if (!compilerGenerated)
                return false;

            bool hasBackingField = info.DeclaringType
                             .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(f => f.Name.Contains(info.Name))
                             .Where(f => f.Name.Contains("BackingField"))
                             .Where(
                                 f => f.GetCustomAttributes(
                                     typeof(CompilerGeneratedAttribute),
                                     true
                                 ).Any()
                             ).Any();

            return hasBackingField;
        }

        private bool DrawSearchButton(string text)
        {
            bool result = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button(text, centeredButtonStyle, GUILayout.MaxWidth(searchButtonMaxWidth)))
            {
                result = true;
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            return result;
        }

        static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.Tizen:
                    return BuildTargetGroup.Tizen;
                case BuildTarget.PSP2:
                    return BuildTargetGroup.PSP2;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.PSM:
                    return BuildTargetGroup.PSM;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.N3DS:
                    return BuildTargetGroup.N3DS;
                case BuildTarget.WiiU:
                    return BuildTargetGroup.WiiU;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
                case BuildTarget.NoTarget:
                default:
                    return BuildTargetGroup.Standalone;
            }
        }
    }
}
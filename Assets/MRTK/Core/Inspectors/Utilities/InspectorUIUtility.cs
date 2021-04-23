// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// This class has handy inspector UI utilities and functions.
    /// </summary>
    public static class InspectorUIUtility
    {
        // Colors
        private static readonly Color PersonalThemeColorTint100 = new Color(1f, 1f, 1f);
        private static readonly Color PersonalThemeColorTint75 = new Color(0.75f, 0.75f, 0.75f);
        private static readonly Color PersonalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);
        private static readonly Color PersonalThemeColorTint25 = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color PersonalThemeColorTint10 = new Color(0.10f, 0.10f, 0.10f);

        private static readonly Color ProfessionalThemeColorTint100 = new Color(0f, 0f, 0f);
        private static readonly Color ProfessionalThemeColorTint75 = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color ProfessionalThemeColorTint50 = new Color(0.5f, 0.5f, 0.5f);
        private static readonly Color ProfessionalThemeColorTint25 = new Color(0.75f, 0.75f, 0.75f);
        private static readonly Color ProfessionalThemeColorTint10 = new Color(0.9f, 0.9f, 0.9f);

        public static Color ColorTint100 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint100 : PersonalThemeColorTint100;
        public static Color ColorTint75 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint75 : PersonalThemeColorTint75;
        public static Color ColorTint50 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint50 : PersonalThemeColorTint50;
        public static Color ColorTint25 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint25 : PersonalThemeColorTint25;
        public static Color ColorTint10 => EditorGUIUtility.isProSkin ? ProfessionalThemeColorTint10 : PersonalThemeColorTint10;

        // default UI sizes
        public const int TitleFontSize = 14;
        public const int HeaderFontSize = 11;
        public const int DefaultFontSize = 10;
        public const float DocLinkWidth = 175f;

        // special characters
        public static readonly string Minus = "\u2212";
        public static readonly string Plus = "\u002B";
        public static readonly string Astrisk = "\u2217";
        public static readonly string Left = "\u02C2";
        public static readonly string Right = "\u02C3";
        public static readonly string Up = "\u02C4";
        public static readonly string Down = "\u02C5";
        public static readonly string Close = "\u2715";
        public static readonly string Heart = "\u2661";
        public static readonly string Star = "\u2606";
        public static readonly string Emoji = "\u263A";

        public static readonly Texture HelpIcon = EditorGUIUtility.IconContent("_Help").image;
        public static readonly Texture SuccessIcon = EditorGUIUtility.IconContent("Collab").image;
        public static readonly Texture WarningIcon = EditorGUIUtility.IconContent("console.warnicon").image;
        public static readonly Texture InfoIcon = EditorGUIUtility.IconContent("console.infoicon").image;

        /// <summary>
        /// A data container for managing scrolling lists or nested drawers in custom inspectors.
        /// </summary>
        public struct ListSettings
        {
            public bool Show;
            public Vector2 Scroll;
        }

        /// <summary>
        /// Delegate for button callbacks, single index
        /// </summary>
        /// <param name="index">location of item in a serialized array</param>
        /// <param name="prop">A serialize property containing information needed if the button was clicked</param>
        public delegate void ListButtonEvent(int index, SerializedProperty prop = null);

        /// <summary>
        /// Delegate for button callbacks, multi-index for nested arrays
        /// </summary>
        /// <param name="indexArray">location of item in a serialized array</param>
        /// <param name="prop">A serialize property containing information needed if the button was clicked</param>
        public delegate void MultiListButtonEvent(int[] indexArray, SerializedProperty prop = null);

        /// <summary>
        /// Box style with left margin
        /// </summary>
        public static GUIStyle Box(int margin)
        {
            GUIStyle box = new GUIStyle(GUI.skin.box);
            box.margin.left = margin;
            return box;
        }

        /// <summary>
        /// Help box style with left margin
        /// </summary>
        /// <param name="margin">amount of left margin</param>
        /// <returns>Configured helpbox GUIStyle</returns>
        public static GUIStyle HelpBox(int margin)
        {
            GUIStyle box = new GUIStyle(EditorStyles.helpBox);
            box.margin.left = margin;
            return box;
        }

        /// <summary>
        /// Create a custom label style based on color and size
        /// </summary>
        public static GUIStyle LableStyle(int size, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = size;
            labelStyle.fixedHeight = size * 2;
            labelStyle.normal.textColor = color;
            return labelStyle;
        }

        /// <summary>
        /// Helper function to render buttons correctly indented according to EditorGUI.indentLevel since GUILayout component don't respond naturally
        /// </summary>
        /// <param name="buttonText">text to place in button</param>
        /// <param name="options">layout options</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(string buttonText, params GUILayoutOption[] options)
        {
            return RenderIndentedButton(() => { return GUILayout.Button(buttonText, options); });
        }

        /// <summary>
        /// Helper function to render buttons correctly indented according to EditorGUI.indentLevel since GUILayout component don't respond naturally
        /// </summary>
        /// <param name="content">What to draw in button</param>
        /// <param name="style">Style configuration for button</param>
        /// <param name="options">layout options</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return RenderIndentedButton(() => { return GUILayout.Button(content, style, options); });
        }

        /// <summary>
        /// Helper function to support primary overloaded version of this functionality
        /// </summary>
        /// <param name="renderButton">The code to render button correctly based on parameter types passed</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(Func<bool> renderButton)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            result = renderButton();
            GUILayout.EndHorizontal();
            return result;
        }

        /// <summary>
        /// Render documentation button routing to relevant URI
        /// </summary>
        /// <param name="docURL">documentation URL to open on button click</param>
        /// <returns>true if button clicked, false otherwise</returns>
        public static bool RenderDocumentationButton(string docURL, float width = DocLinkWidth)
        {
            if (!string.IsNullOrEmpty(docURL))
            {
                var buttonContent = new GUIContent()
                {
                    image = HelpIcon,
                    text = " Documentation",
                    tooltip = docURL,
                };

                // The documentation button should always be enabled.
                using (new GUIEnabledWrapper())
                {
                    if (GUILayout.Button(buttonContent, EditorStyles.miniButton, GUILayout.MaxWidth(width)))
                    {
                        Application.OpenURL(docURL);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Render a documentation header with button if Object contains HelpURLAttribute
        /// </summary>
        /// <param name="targetType">Type to test for HelpURLAttribute</param>
        /// <returns>true if object drawn and button clicked, false otherwise</returns>
        public static bool RenderHelpURL(Type targetType)
        {
            bool result = false;

            if (targetType != null)
            {
                HelpURLAttribute helpURL = targetType.GetCustomAttribute<HelpURLAttribute>();
                if (helpURL != null)
                {
                    result = RenderDocumentationSection(helpURL.URL);
                }
            }

            return result;
        }

        /// <summary>
        /// Render a documentation header with button for given url value
        /// </summary>
        /// <param name="url">Url to open if button is clicked</param>
        /// <returns>true if object drawn and button clicked, false otherwise</returns>
        public static bool RenderDocumentationSection(string url)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(url))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    result = RenderDocumentationButton(url);
                }
            }

            return result;
        }

        /// <summary>
        /// A button that is as wide as the label
        /// </summary>
        public static bool FlexButton(GUIContent label, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {
            if (FlexButton(label))
            {
                callback(index, prop);
                return true;
            }

            return false;
        }

        /// <summary>
        /// A button that is as wide as the label
        /// </summary>
        /// <returns>true if button clicked, false otherwise</returns>
        public static bool FlexButton(GUIContent label, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            if (FlexButton(label))
            {
                callback(indexArr, prop);
                return true;
            }

            return false;
        }

        /// <summary>
        /// A button that is as wide as the label
        /// </summary>
        /// <param name="label">content for button</param>
        /// <returns>true if button clicked, false otherwise</returns>
        public static bool FlexButton(GUIContent label)
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            float buttonWidth = GUI.skin.button.CalcSize(label).x;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(label, buttonStyle, GUILayout.Width(buttonWidth)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A button that is as wide as the available space
        /// </summary>
        public static bool FullWidthButton(GUIContent label, float padding, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle addStyle = new GUIStyle(GUI.skin.button);
            addStyle.fixedHeight = 25;
            float addButtonWidth = GUI.skin.button.CalcSize(label).x * padding;
            bool triggered = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(label, addStyle, GUILayout.Width(addButtonWidth)))
                {
                    callback(index, prop);
                    triggered = true;
                }

                GUILayout.FlexibleSpace();
            }

            return triggered;
        }

        /// <summary>
        /// A button that is as wide as the available space
        /// </summary>
        public static bool FullWidthButton(GUIContent label, float padding, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle addStyle = new GUIStyle(GUI.skin.button);
            addStyle.fixedHeight = 25;
            float addButtonWidth = GUI.skin.button.CalcSize(label).x * padding;
            bool triggered = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(label, addStyle, GUILayout.Width(addButtonWidth)))
                {
                    callback(indexArr, prop);
                    triggered = true;
                }

                GUILayout.FlexibleSpace();
            }

            return triggered;
        }

        /// <summary>
        /// A small button, good for a single icon like + or - with single index callback events
        /// </summary>
        /// <param name="label">content to place in the button</param>
        /// <returns>true if button selected, false otherwise</returns>
        public static bool SmallButton(GUIContent label, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {
            if (SmallButton(label))
            {
                callback(index, prop);
                return true;
            }

            return false;
        }

        /// <summary>
        /// A small button, good for a single icon like + or - with multi-index callback events
        /// </summary>
        /// <param name="label">content to place in the button</param>
        /// <returns>true if button selected, false otherwise</returns>
        public static bool SmallButton(GUIContent label, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            if (SmallButton(label))
            {
                callback(indexArr, prop);
                return true;
            }

            return false;
        }

        /// <summary>
        /// A small button, good for a single icon like + or -
        /// </summary>
        /// <param name="label">content to place in the button</param>
        /// <returns>true if button selected, false otherwise</returns>
        public static bool SmallButton(GUIContent label)
        {
            GUIStyle smallButton = new GUIStyle(EditorStyles.miniButton);
            float smallButtonWidth = GUI.skin.button.CalcSize(label).x;

            if (GUILayout.Button(label, smallButton, GUILayout.Width(smallButtonWidth)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Large title format
        /// </summary>
        public static void DrawTitle(string title)
        {
            GUIStyle labelStyle = LableStyle(TitleFontSize, ColorTint50);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(TitleFontSize * 0.5f);
        }

        /// <summary>
        /// Medium title format
        /// </summary>
        /// <param name="header">string content to render</param>
        public static void DrawHeader(string header)
        {
            GUIStyle labelStyle = LableStyle(HeaderFontSize, ColorTint10);
            EditorGUILayout.LabelField(new GUIContent(header), labelStyle);
        }

        /// <summary>
        /// Draw a basic label
        /// </summary>
        public static void DrawLabel(string title, int size, Color color)
        {
            GUIStyle labelStyle = LableStyle(size, color);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
        }

        /// <summary>
        /// draw a label with a yellow coloring
        /// </summary>
        public static void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = MixedRealityInspectorUtility.WarningColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(warning, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        /// <summary>
        /// draw a notice area, normal coloring
        /// </summary>
        public static void DrawNotice(string notice)
        {
            Color prevColor = GUI.color;

            GUI.color = ColorTint50;
            using (new EditorGUILayout.VerticalScope(EditorStyles.textArea))
            {
                EditorGUILayout.LabelField(notice, EditorStyles.wordWrappedMiniLabel);
            }

            GUI.color = prevColor;
        }

        /// <summary>
        /// draw a notice with green coloring
        /// </summary>
        public static void DrawSuccess(string notice)
        {
            Color prevColor = GUI.color;

            GUI.color = MixedRealityInspectorUtility.SuccessColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(notice, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        /// <summary>
        /// draw a notice with red coloring
        /// </summary>
        public static void DrawError(string error)
        {
            Color prevColor = GUI.color;

            GUI.color = MixedRealityInspectorUtility.ErrorColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(error, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        /// <summary>
        /// Create a line across the negative space
        /// </summary>
        public static void DrawDivider()
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        /// <summary>
        /// Draws a section start (initiated by the Header attribute)
        /// </summary>
        public static bool DrawSectionFoldout(string headerName, bool open = true, GUIStyle style = null)
        {
            if (style == null)
            {
                style = EditorStyles.foldout;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                return EditorGUILayout.Foldout(open, headerName, true, style);
            }
        }
        /// <summary>
        /// Draws a section start with header name and save open/close state to given preference key in SessionState
        /// </summary>
        public static bool DrawSectionFoldoutWithKey(string headerName, string preferenceKey = null, GUIStyle style = null, bool defaultOpen = true)
        {
            bool showPref = SessionState.GetBool(preferenceKey, defaultOpen);
            bool show = DrawSectionFoldout(headerName, showPref, style);
            if (show != showPref)
            {
                SessionState.SetBool(preferenceKey, show);
            }

            return show;
        }

        /// <summary>
        /// Draws a popup UI with PropertyField type features.
        /// Displays prefab pending updates
        /// </summary>
        /// <param name="prop">serialized property corresponding to Enum</param>
        /// <param name="label">label for property</param>
        /// <param name="propValue">Current enum value for property</param>
        /// <returns>New enum value after draw</returns>
        public static Enum DrawEnumSerializedProperty(SerializedProperty prop, GUIContent label, Enum propValue)
        {
            return DrawEnumSerializedProperty(EditorGUILayout.GetControlRect(), prop, label, propValue);
        }

        /// <summary>
        /// Draws a popup UI with PropertyField type features.
        /// Displays prefab pending updates
        /// </summary>
        /// <param name="position">position to render the serialized property</param>
        /// <param name="prop">serialized property corresponding to Enum</param>
        /// <param name="label">label for property</param>
        /// <param name="propValue">Current enum value for property</param>
        /// <returns>New enum value after draw</returns>
        public static Enum DrawEnumSerializedProperty(Rect position, SerializedProperty prop, GUIContent label, Enum propValue)
        {
            Enum result = propValue;
            EditorGUI.BeginProperty(position, label, prop);
            {
                result = EditorGUI.EnumPopup(position, label, propValue);
                prop.enumValueIndex = Convert.ToInt32(result);
            }
            EditorGUI.EndProperty();

            return result;
        }

        /// <summary>
        /// adjust list settings as things change
        /// </summary>
        public static List<ListSettings> AdjustListSettings(List<ListSettings> listSettings, int count)
        {
            if (listSettings == null)
            {
                listSettings = new List<ListSettings>();
            }

            int diff = count - listSettings.Count;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    listSettings.Add(new ListSettings() { Show = false, Scroll = new Vector2() });
                }
            }
            else if (diff < 0)
            {
                int removeCnt = 0;
                for (int i = listSettings.Count - 1; i > -1; i--)
                {
                    if (removeCnt > diff)
                    {
                        listSettings.RemoveAt(i);
                        removeCnt--;
                    }
                }
            }

            return listSettings;
        }

        /// <summary>
        /// Get an array of strings from a serialized list of strings, pop-up field helper
        /// </summary>
        public static string[] GetOptions(SerializedProperty options)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < options.arraySize; i++)
            {
                list.Add(options.GetArrayElementAtIndex(i).stringValue);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Get the index of a serialized array item based on its name, pop-up field helper
        /// </summary>
        public static int GetOptionsIndex(SerializedProperty options, string selection)
        {
            for (int i = 0; i < options.arraySize; i++)
            {
                if (options.GetArrayElementAtIndex(i).stringValue == selection)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// Draws the contents of a scriptable inline inside a foldout. Depending on if there's an actual scriptable
        /// linked, the values will be greyed out or editable in case the scriptable is created inside the serialized object.
        /// </summary>
        static public bool DrawScriptableFoldout<T>(SerializedProperty scriptable, string description, bool isExpanded) where T : ScriptableObject
        {
            isExpanded = EditorGUILayout.Foldout(isExpanded, description, true, MixedRealityStylesUtility.BoldFoldoutStyle);
            if (isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (scriptable.objectReferenceValue == null)
                    {
                        // If there's no scriptable linked we're creating a local instance that allows to store a 
                        // local version of the scriptable in the serialized object owning the scriptable property.
                        scriptable.objectReferenceValue = ScriptableObject.CreateInstance<T>();
                    }

                    // We have currently 5 different states to display to the user:
                    // 1. the scriptable is local to the object instance
                    // 2. the scriptable is a linked nested scriptable inside of the currently displayed prefab
                    // 3. the scriptable is a linked nested scriptable inside of another prefab
                    // 4. the scriptable is a shared standalone asset
                    // 5. the scriptable slot is empty but inside of a prefab asset which needs special handling as 
                    // prefabs can only display linked or nested scriptables.

                    // Depending on the type of link we show the user the scriptable configuration either:
                    // case 1 &2: editable inlined 
                    // case 3 & 4: greyed out readonly
                    // case 5: only show the link

                    // case 5 -> can't create and/or store the local scriptable above - show link
                    bool isStoredAsset = scriptable.objectReferenceValue != null && AssetDatabase.Contains(scriptable.objectReferenceValue);
                    bool isEmptyInStagedPrefab = !isStoredAsset && ((Component)scriptable.serializedObject.targetObject).gameObject.scene.path == "";
                    if (scriptable.objectReferenceValue == null || isEmptyInStagedPrefab)
                    {
                        EditorGUILayout.HelpBox("No scriptable " + scriptable.displayName + " linked to this prefab. Prefabs can't store " +
                            "local versions of scriptables and need to be linked to a scriptable asset.", MessageType.Warning);
                        EditorGUILayout.PropertyField(scriptable, new GUIContent(scriptable.displayName + " (Empty): "));
                    }
                    else
                    {
                        bool isNestedInCurrentPrefab = false;
                        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (prefabStage != null)
                        {
                            var instancePath = AssetDatabase.GetAssetPath(scriptable.objectReferenceValue);
                            isNestedInCurrentPrefab = instancePath != "" &&
#if UNITY_2020_1_OR_NEWER
                                instancePath == prefabStage.assetPath
#else
                                instancePath == prefabStage.prefabAssetPath
#endif
                            ;
                        }

                        if (isStoredAsset && !isNestedInCurrentPrefab)
                        {
                            // case 3 & 4 - greyed out drawer
                            bool isMainAsset = AssetDatabase.IsMainAsset(scriptable.objectReferenceValue);
                            var sharedAssetPath = AssetDatabase.GetAssetPath(scriptable.objectReferenceValue);
                            if (isMainAsset)
                            {
                                EditorGUILayout.HelpBox("Editing a shared " + scriptable.displayName + ", located at " + sharedAssetPath, MessageType.Warning);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Editing a nested " + scriptable.displayName + ", located inside of " + sharedAssetPath, MessageType.Warning);
                            }
                            EditorGUILayout.PropertyField(scriptable, new GUIContent(scriptable.displayName + " (Shared asset): "));

                            // In case there's a shared scriptable linked we're disabling the inlined scriptable properties 
                            // (this will render them grayed out) so users won't accidentally modify the shared scriptable.
                            GUI.enabled = false;
                            DrawScriptableSubEditor(scriptable);
                            GUI.enabled = true;
                        }
                        else
                        {
                            // case 1 & 2 - inline editable drawer
                            if (isNestedInCurrentPrefab)
                            {
                                EditorGUILayout.HelpBox("Editing a nested version of " + scriptable.displayName + ".", MessageType.Info);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Editing a local version of " + scriptable.displayName + ".", MessageType.Info);
                            }
                            EditorGUILayout.PropertyField(scriptable, new GUIContent(scriptable.displayName + " (local): "));
                            DrawScriptableSubEditor(scriptable);
                        }
                    }
                }
            }

            return isExpanded;
        }

        /// <summary>
        /// Draws a foldout enlisting all components (or derived types) of the given type attached to the passed gameobject.
        /// Adds a button for adding any of the component (or derived types) and a follow button to highlight existing attached components.
        /// </summary>
        static public bool DrawComponentTypeFoldout<T>(GameObject gameObject, bool isExpanded, string typeDescription) where T : MonoBehaviour
        {
            isExpanded = EditorGUILayout.Foldout(isExpanded, typeDescription + "s", true);

            if (isExpanded)
            {
                if (EditorGUILayout.DropdownButton(new GUIContent("Add " + typeDescription), FocusType.Keyboard))
                {
                    // create the menu and add items to it
                    GenericMenu menu = new GenericMenu();

                    var type = typeof(T);
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetLoadableTypes())
                                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

                    foreach (var derivedType in types)
                    {
                        menu.AddItem(new GUIContent(derivedType.Name), false, t => gameObject.AddComponent((Type)t), derivedType);
                    }

                    menu.ShowAsContext();
                }

                var constraints = gameObject.GetComponents<T>();

                foreach (var constraint in constraints)
                {
                    EditorGUILayout.BeginHorizontal();
                    string constraintName = constraint.GetType().Name;
                    EditorGUILayout.LabelField(constraintName);
                    if (GUILayout.Button("Go to component"))
                    {
                        Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                        EditorGUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            return isExpanded;
        }


        static private void DrawScriptableSubEditor(SerializedProperty scriptable)
        {
            if (scriptable.objectReferenceValue != null)
            {
                UnityEditor.Editor configEditor = UnityEditor.Editor.CreateEditor(scriptable.objectReferenceValue);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.Space();
                configEditor.OnInspectorGUI();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }
    }
}

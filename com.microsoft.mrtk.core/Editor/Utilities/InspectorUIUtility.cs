// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// This class has handy inspector UI utilities and functions.
    /// </summary>
    public static class InspectorUIUtility
    {
        #region Color

        /// <summary>
        /// Default background color, depending
        /// on light/dark theme.
        /// </summary>
        public static Color DefaultBackgroundColor => EditorGUIUtility.isProSkin
                    ? new Color32(56, 56, 56, 255)
                    : new Color32(194, 194, 194, 255);

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

        public static readonly Color DisabledColor = new Color(0.6f, 0.6f, 0.6f);
        public static readonly Color WarningColor = new Color(1f, 0.85f, 0.6f);
        public static readonly Color ErrorColor = new Color(1f, 0.55f, 0.5f);
        public static readonly Color SuccessColor = new Color(0.8f, 1f, 0.75f);
        public static readonly Color SectionColor = new Color(0.85f, 0.9f, 1f);
        public static readonly Color DarkColor = new Color(0.1f, 0.1f, 0.1f);
        public static readonly Color HandleColorSquare = new Color(0.0f, 0.9f, 1f);
        public static readonly Color HandleColorCircle = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorSphere = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorAxis = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorRotation = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorTangent = new Color(0.1f, 0.8f, 0.5f, 0.7f);
        public static readonly Color LineVelocityColor = new Color(0.9f, 1f, 0f, 0.8f);

        #endregion Color

        #region Sizes

        // default UI sizes
        public const int TitleFontSize = 14;
        public const int HeaderFontSize = 11;
        public const int DefaultFontSize = 10;
        public const float DocLinkWidth = 175f;

        #endregion Sizes

        #region Special unicode characters

        // special characters
        public static readonly string Minus = "\u2212";
        public static readonly string Plus = "\u002B";
        public static readonly string Asterisk = "\u2217";
        public static readonly string Left = "\u02C2";
        public static readonly string Right = "\u02C3";
        public static readonly string Up = "\u02C4";
        public static readonly string Down = "\u02C5";
        public static readonly string Close = "\u2715";
        public static readonly string Heart = "\u2661";
        public static readonly string Star = "\u2606";
        public static readonly string Emoji = "\u263A";
        public static readonly string Reload = "\u21BB";

        #endregion Special unicode characters

        #region Handy icon textures

        public static readonly Texture HelpIcon = EditorGUIUtility.IconContent("_Help").image;
        public static readonly Texture SuccessIcon = EditorGUIUtility.IconContent("Collab").image;
        public static readonly Texture WarningIcon = EditorGUIUtility.IconContent("console.warnicon").image;
        public static readonly Texture InfoIcon = EditorGUIUtility.IconContent("console.infoicon").image;

        // StandardAssets/Textures/MRTK_Logo_Black.png
        private const string LogoLightThemeGuid = "fa0038d8d2df1dd4c99f346c8ec9e746";

        // StandardAssets/Textures/MRTK_Logo_White.png
        private const string LogoDarkThemeGuid = "fe5cc215f12ea5e40b5021c4040bce24";

        /// <summary> MRTK Logo texture suitable for light theme. </summary>
        public static readonly Texture2D LogoLightTheme = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(LogoLightThemeGuid));

        /// <summary> MRTK Logo texture suitable for light theme. </summary>
        public static readonly Texture2D LogoDarkTheme = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(LogoDarkThemeGuid));

        #endregion Handy icon textures

        #region Handy drawables/controls

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

        internal static bool IsMixedRealityToolkitLogoAssetPresent()
        {
            return EditorGUIUtility.isProSkin ? LogoDarkTheme != null : LogoLightTheme != null;
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        /// <returns>True if the logo was loadable and renderable or false if a text fallback was used.</returns>
        public static bool RenderMixedRealityToolkitLogo()
        {
            if (IsMixedRealityToolkitLogoAssetPresent())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(EditorGUIUtility.isProSkin ? LogoDarkTheme : LogoLightTheme, GUILayout.MaxHeight(96f));
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(3f);
                return true;
            }
            else
            {
                EditorGUILayout.LabelField("Microsoft Mixed Reality Toolkit", MRTKEditorStyles.ProductNameStyle);
                GUILayout.Space(3f);
                return false;
            }
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
            GUIStyle labelStyle = MRTKEditorStyles.LabelStyle(TitleFontSize, ColorTint50);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(TitleFontSize * 0.5f);
        }

        /// <summary>
        /// Medium title format
        /// </summary>
        /// <param name="header">string content to render</param>
        public static void DrawHeader(string header)
        {
            GUIStyle labelStyle = MRTKEditorStyles.LabelStyle(HeaderFontSize, ColorTint10);
            EditorGUILayout.LabelField(new GUIContent(header), labelStyle);
        }

        /// <summary>
        /// Draw a basic label
        /// </summary>
        public static void DrawLabel(string title, int size, Color color)
        {
            GUIStyle labelStyle = MRTKEditorStyles.LabelStyle(size, color);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
        }

        /// <summary>
        /// draw a label with a yellow coloring
        /// </summary>
        public static void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = WarningColor;
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

            GUI.color = SuccessColor;
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

            GUI.color = ErrorColor;
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
            style ??= EditorStyles.foldout;

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
                prop.intValue = Convert.ToInt32(result);
            }
            EditorGUI.EndProperty();

            return result;
        }

        /// <summary>
        /// Draws a foldout enlisting all components (or derived types) of the given type attached to the passed GameObject.
        /// Adds a button for adding any of the component (or derived types) and a follow button to highlight existing attached components.
        /// </summary>
        public static bool DrawComponentTypeFoldout<T>(GameObject gameObject, bool isExpanded, string typeDescription) where T : MonoBehaviour
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
                        Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)}");
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

        #endregion Handy drawables/controls

        #region Utilities

        public static string GetBackingField(string propertyName) => $"<{propertyName}>k__BackingField";

        /// <summary>
        /// Found at https://answers.unity.com/questions/960413/editor-window-how-to-center-a-window.html
        /// </summary>
        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).FirstOrDefault(t => t.Name == "ContainerWindow");

            if (containerWinType == null)
            {
                throw new MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            }

            var showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);

            if (showModeField == null || positionProperty == null)
            {
                throw new MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            }

            var windows = Resources.FindObjectsOfTypeAll(containerWinType);

            foreach (var win in windows)
            {

                var showMode = (int)showModeField.GetValue(win);
                if (showMode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        private static Type[] GetAllDerivedTypes(this AppDomain appDomain, Type aType)
        {
            var result = new List<Type>();
            var assemblies = appDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetLoadableTypes();
                result.AddRange(types.Where(type => type.IsSubclassOf(aType)));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Centers an editor window on the main display.
        /// </summary>
        public static void CenterOnMainWin(this EditorWindow window)
        {
            var main = GetEditorMainWindowPos();
            var pos = window.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;
        }

        #endregion Utilities

        #region Handles

        // Magic number for dashed lines for editor handles/affordances.
        private const float DottedLineScreenSpace = 4.65f;

        /// <summary>
        /// Draw an axis move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="origin">The initial position of the axis.</param>
        /// <param name="direction">The direction the axis is facing.</param>
        /// <param name="distance">Distance from the axis.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see cref="float"/> value.</returns>
        public static float AxisMoveHandle(Object target, Vector3 origin, Vector3 direction, float distance, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Vector3 position = origin + (direction.normalized * distance);

            Handles.color = HandleColorAxis;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Handles.DrawDottedLine(origin, position, DottedLineScreenSpace);
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(direction), handleSize * 2, EventType.Repaint);
#if UNITY_2022_1_OR_NEWER
            Vector3 newPosition = Handles.FreeMoveHandle(position, handleSize, Vector3.zero, Handles.CircleHandleCap);
#else
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);
#endif

            if (recordUndo)
            {
                float newDistance = Vector3.Distance(origin, newPosition);

                if (!distance.Equals(newDistance))
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);
                    distance = newDistance;
                }
            }

            return distance;
        }

        /// <summary>
        /// Draw a Circle Move Handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 CircleMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorCircle;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

#if UNITY_2022_1_OR_NEWER
            Vector3 newPosition = Handles.FreeMoveHandle(position, handleSize, Vector3.zero, Handles.CircleHandleCap);
#else
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);
#endif

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a square move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 SquareMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorSquare;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply square handle to match other types
#if UNITY_2022_1_OR_NEWER
            Vector3 newPosition = Handles.FreeMoveHandle(position, handleSize * 0.8f, Vector3.zero, Handles.RectangleHandleCap);
#else
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 0.8f, Vector3.zero, Handles.RectangleHandleCap);
#endif

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a sphere move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 SphereMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorSphere;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply sphere handle size to match other types
#if UNITY_2022_1_OR_NEWER
            Vector3 newPosition = Handles.FreeMoveHandle(position, handleSize * 2, Vector3.zero, Handles.SphereHandleCap);
#else            
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 2, Vector3.zero, Handles.SphereHandleCap);
#endif

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a vector handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="normalize">Optional, Normalize the new vector value.</param>
        /// <param name="clamp">Optional, Clamp new vector's value based on the distance to the origin.</param>
        /// <param name="handleLength">Optional, handle length.</param>
        /// <param name="handleSize">Optional, handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 VectorHandle(Object target, Vector3 origin, Vector3 vector, bool normalize = true, bool clamp = true, float handleLength = 1f, float handleSize = 0.1f, bool recordUndo = true, bool autoSize = true)
        {
            Handles.color = HandleColorTangent;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(origin) * handleSize, 0.75f);
            }

            Vector3 handlePosition = origin + (vector * handleLength);
            float distanceToOrigin = Vector3.Distance(origin, handlePosition) / handleLength;

            if (normalize)
            {
                vector.Normalize();
            }
            else
            {
                // If the handle isn't normalized, brighten based on distance to origin
                Handles.color = Color.Lerp(Color.gray, HandleColorTangent, distanceToOrigin * 0.85f);

                if (clamp)
                {
                    // To indicate that we're at the clamped limit, make the handle 'pop' slightly larger
                    if (distanceToOrigin >= 0.98f)
                    {
                        Handles.color = Color.Lerp(HandleColorTangent, Color.white, 0.5f);
                        handleSize *= 1.5f;
                    }
                }
            }

            // Draw a line from origin to origin + direction
            Handles.DrawLine(origin, handlePosition);

#if UNITY_2022_1_OR_NEWER
            Vector3 newPosition = Handles.FreeMoveHandle(handlePosition, handleSize, Vector3.zero, Handles.DotHandleCap);
#else 
            Vector3 newPosition = Handles.FreeMoveHandle(handlePosition, Quaternion.identity, handleSize, Vector3.zero, Handles.DotHandleCap);
#endif

            if (recordUndo && handlePosition != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);
                vector = (newPosition - origin).normalized;

                // If we normalize, we're done
                // Otherwise, multiply the vector by the distance between origin and target
                if (!normalize)
                {
                    distanceToOrigin = Vector3.Distance(origin, newPosition) / handleLength;

                    if (clamp)
                    {
                        distanceToOrigin = Mathf.Clamp01(distanceToOrigin);
                    }

                    vector *= distanceToOrigin;
                }
            }

            return vector;
        }

        /// <summary>
        /// Draw a rotation handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="rotation">The rotation to draw the handle.</param>
        /// <param name="handleSize">Optional, handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Quaternion.html">Quaternion</see> value.</returns>
        public static Quaternion RotationHandle(Object target, Vector3 position, Quaternion rotation, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorRotation;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Make rotation handles larger so they can overlay movement handles
            Quaternion newRotation = Handles.FreeRotateHandle(rotation, position, handleSize * 2);

            if (recordUndo)
            {
                Handles.color = Handles.zAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.forward), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.xAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.right), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.yAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.up), handleSize * 2, EventType.Repaint);

                if (rotation != newRotation)
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);
                    rotation = newRotation;
                }
            }

            return rotation;
        }

        #endregion Handles
    }
}

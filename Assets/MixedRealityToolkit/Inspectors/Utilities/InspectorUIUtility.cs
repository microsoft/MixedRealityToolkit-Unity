// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// This class has handy inspector UI utilities and functions.
    /// </summary>
    public static class InspectorUIUtility
    {
        //Colors
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
        public static bool RenderDocumentationButton(string docURL)
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
                using (new GUIEnabledWrapper(true, true))
                {
                    if (GUILayout.Button(buttonContent, EditorStyles.miniButton, GUILayout.MaxWidth(DocLinkWidth)))
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

            GUI.color = ColorTint100;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(notice, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

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
        /// Get the index of a serialized array item based on it's name, pop-up field helper
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
    }
}

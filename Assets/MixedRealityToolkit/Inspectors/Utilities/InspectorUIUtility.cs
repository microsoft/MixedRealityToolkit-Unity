// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities
{
    /// <summary>
    /// This class has handy inspector UI utilities and functions.
    /// </summary>
    public static class InspectorUIUtility
    {
        //Colors
        public readonly static Color ColorTint100 = new Color(1f, 1f, 1f);
        public readonly static Color ColorTint75 = new Color(0.75f, 0.75f, 0.75f);
        public readonly static Color ColorTint50 = new Color(0.5f, 0.5f, 0.5f);
        public readonly static Color ColorTint25 = new Color(0.25f, 0.25f, 0.25f);

        // default text sizes
        public const int TitleFontSize = 14;
        public const int DefaultFontSize = 10;

        // special characters
        public readonly static string Minus = "\u2212";
        public readonly static string Plus = "\u002B";
        public readonly static string Astrisk = "\u2217";
        public readonly static string Left = "\u02C2";
        public readonly static string Right = "\u02C3";
        public readonly static string Up = "\u02C4";
        public readonly static string Down = "\u02C5";
        public readonly static string Close = "\u2715";
        public readonly static string Heart = "\u2661";
        public readonly static string Star = "\u2606";
        public readonly static string Emoji = "\u263A";

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
        /// <param name="index">location of item in a serialized array</param>
        /// <param name="prop">A serialize property containing information needed if the button was clicked</param>
        public delegate void MultiListButtonEvent(int[] indexArray, SerializedProperty prop = null);

        /// <summary>
        /// Box style with left margin
        /// </summary>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static GUIStyle Box(int margin)
        {
            GUIStyle box = new GUIStyle(GUI.skin.box);
            box.margin.left = margin;
            return box;
        }

        /// <summary>
        /// Create a custom label style based on color and size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
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
        /// A button that is as wide as the label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FlexButton(GUIContent label, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {
            // delete button
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            float buttonWidth = GUI.skin.button.CalcSize(label).x;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, buttonStyle, GUILayout.Width(buttonWidth)))
            {
                callback(index, prop);
                triggered = true;
            }

            EditorGUILayout.EndHorizontal();
            return triggered;
        }

        /// <summary>
        /// A button that is as wide as the label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="indexArr"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FlexButton(GUIContent label, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            // delete button
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);

            float buttonWidth = GUI.skin.button.CalcSize(label).x;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, buttonStyle, GUILayout.Width(buttonWidth)))
            {
                callback(indexArr, prop);
                triggered = true;
            }

            EditorGUILayout.EndHorizontal();
            return triggered;
        }

        /// <summary>
        /// A button that is as wide as the available space
        /// </summary>
        /// <param name="label"></param>
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FullWidthButton(GUIContent label, float padding, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle addStyle = new GUIStyle(GUI.skin.button);
            addStyle.fixedHeight = 25;
            float addButtonWidth = GUI.skin.button.CalcSize(label).x * padding;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, addStyle, GUILayout.Width(addButtonWidth)))
            {
                callback(index, prop);
                triggered = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return triggered;
        }

        /// <summary>
        /// A button that is as wide as the available space
        /// </summary>
        /// <param name="label"></param>
        /// <param name="indexArr"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FullWidthButton(GUIContent label, float padding, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle addStyle = new GUIStyle(GUI.skin.button);
            addStyle.fixedHeight = 25;
            float addButtonWidth = GUI.skin.button.CalcSize(label).x * padding;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, addStyle, GUILayout.Width(addButtonWidth)))
            {
                callback(indexArr, prop);
                triggered = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return triggered;
        }

        /// <summary>
        /// A small button, good for a single icon like + or -
        /// </summary>
        /// <param name="label"></param>
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool SmallButton(GUIContent label, int index, ListButtonEvent callback, SerializedProperty prop = null)
        {

            GUIStyle smallButton = new GUIStyle(EditorStyles.miniButton);
            float smallButtonWidth = GUI.skin.button.CalcSize(new GUIContent(label)).x;

            bool triggered = false;
            if (GUILayout.Button(label, smallButton, GUILayout.Width(smallButtonWidth)))
            {
                callback(index, prop);
                triggered = true;
            }
            return triggered;
        }

        /// <summary>
        /// A small button, good for a single icon like + or -
        /// </summary>
        /// <param name="label"></param>
        /// <param name="indexArr"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool SmallButton(GUIContent label, int[] indexArr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle smallButton = new GUIStyle(EditorStyles.miniButton);
            float smallButtonWidth = GUI.skin.button.CalcSize(label).x;

            bool triggered = false;
            if (GUILayout.Button(label, smallButton, GUILayout.Width(smallButtonWidth)))
            {
                callback(indexArr, prop);
                triggered = true;
            }
            return triggered;
        }

        /// <summary>
        /// Large title format
        /// </summary>
        /// <param name="title"></param>
        public static void DrawTitle(string title)
        {
            GUIStyle labelStyle = LableStyle(TitleFontSize, ColorTint50);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(TitleFontSize * 0.5f);
        }

        /// <summary>
        /// Draw a basic label
        /// </summary>
        /// <param name="title"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawLabel(string title, int size, Color color)
        {
            GUIStyle labelStyle = LableStyle(size, color);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(TitleFontSize * 0.5f);
        }

        /// <summary>
        /// draw a label with a yellow coloring
        /// </summary>
        /// <param name="warning"></param>
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
        /// <param name="notice"></param>
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
        /// <param name="notice"></param>
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
        /// <param name="error"></param>
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
            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = 1;
            styleHR.border = new RectOffset(1, 1, 1, 0);
            GUILayout.Box("", styleHR);
        }

        /// <summary>
        /// Draws a section start (initiated by the Header attribute)
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="headerName"></param>
        /// <param name="toUpper"></param>
        /// <param name="drawInitially"></param>
        /// <returns></returns>
        public static bool DrawSectionStart(string headerName, int indent, bool open = true, FontStyle style = FontStyle.Bold, bool toUpper = true, int size = 0)
        {
            GUIStyle sectionStyle = new GUIStyle(EditorStyles.foldout);
            sectionStyle.fontStyle = style;
            if (size > 0)
            {
                sectionStyle.fontSize = size;
                sectionStyle.fixedHeight = size * 2;
            }
            Color tColor = GUI.color;
            GUI.color = MixedRealityInspectorUtility.SectionColor;

            if (toUpper)
            {
                headerName = headerName.ToUpper();
            }

            bool drawSection = false;
            drawSection = EditorGUILayout.Foldout(open, headerName, true, sectionStyle);
            EditorGUILayout.BeginVertical();
            GUI.color = tColor;
            EditorGUI.indentLevel = indent;

            return drawSection;
        }

        /// <summary>
        /// Draws section end (initiated by next Header attribute)
        /// </summary>
        public static void DrawSectionEnd(int indent)
        {
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indent;
        }

        /// <summary>
        /// adjust list settings as things change
        /// </summary>
        /// <param name="count"></param>
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
        /// <param name="options"></param>
        /// <returns></returns>
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
        /// <param name="options"></param>
        /// <param name="selection"></param>
        /// <returns></returns>
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
        /// Get the index of an array item based on it's name, pop-up field helper
        /// </summary>
        /// <param name="option"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int ReverseLookup(string option, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == option)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}

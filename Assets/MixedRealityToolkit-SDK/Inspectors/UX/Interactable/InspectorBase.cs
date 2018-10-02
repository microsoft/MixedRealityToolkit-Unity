// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// A colleciton of helper functions to make consistent looking Inspectors easier
    /// </summary>
    
#if UNITY_EDITOR
    public abstract class InspectorBase : Editor
    {
        public struct ListSettings
        {
            public bool Show;
            public Vector2 Scroll;
        }

        // add custom settings

        // Colors
        protected readonly static Color defaultColor = new Color(1f, 1f, 1f);
        protected readonly static Color baseColor = new Color(0.75f, 0.75f, 0.75f);
        protected readonly static Color disabledColor = new Color(0.6f, 0.6f, 0.6f);
        protected readonly static Color warningColor = new Color(1f, 0.85f, 0.6f);
        protected readonly static Color errorColor = new Color(1f, 0.55f, 0.5f);
        protected readonly static Color successColor = new Color(0.5f, 1f, 0.5f);
        protected readonly static Color titleColor = new Color(0.5f, 0.5f, 0.5f);
        protected readonly static Color sectionColor = new Color(0.85f, 0.9f, 1f);

        // default text sizes
        protected const int titleFontSize = 14;
        protected const int defaultFontSize = 10;

        // special characters
        protected readonly static string minus = "\u2212";
        protected readonly static string plus = "\u002B";
        protected readonly static string astrisk = "\u2217";
        protected readonly static string left = "\u02C2";
        protected readonly static string right = "\u02C3";
        protected readonly static string up = "\u02C4";
        protected readonly static string down = "\u02C5";
        protected readonly static string close = "\u2715";
        protected readonly static string heart = "\u2661";
        protected readonly static string star = "\u2606";
        protected readonly static string emoji = "\u263A";

        // indent tracker
        protected static int indentOnSectionStart = 0;
        public const float DottedLineScreenSpace = 4.65f;

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

        protected List<ListSettings> listSettings;

        protected GUIStyle boxStyle;

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
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FlexButton(GUIContent label, int[] arr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            // delete button
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            
            float buttonWidth = GUI.skin.button.CalcSize(label).x;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, buttonStyle, GUILayout.Width(buttonWidth)))
            {
                callback(arr, prop);
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
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool FullWidthButton(GUIContent label, float padding, int[] arr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle addStyle = new GUIStyle(GUI.skin.button);
            addStyle.fixedHeight = 25;
            float addButtonWidth = GUI.skin.button.CalcSize(label).x * padding;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            bool triggered = false;
            if (GUILayout.Button(label, addStyle, GUILayout.Width(addButtonWidth)))
            {
                callback(arr, prop);
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
        /// <param name="index"></param>
        /// <param name="callback"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool SmallButton(GUIContent label, int[] arr, MultiListButtonEvent callback, SerializedProperty prop = null)
        {
            GUIStyle smallButton = new GUIStyle(EditorStyles.miniButton);
            float smallButtonWidth = GUI.skin.button.CalcSize(label).x;

            bool triggered = false;
            if (GUILayout.Button(label, smallButton, GUILayout.Width(smallButtonWidth)))
            {
                callback(arr, prop);
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
            GUIStyle labelStyle = GetLableStyle(titleFontSize, titleColor);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(titleFontSize * 0.5f);
        }

        /// <summary>
        /// Draw a basic lable
        /// </summary>
        /// <param name="title"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawLabel(string title, int size, Color color)
        {
            GUIStyle labelStyle = GetLableStyle(size, color);
            EditorGUILayout.LabelField(new GUIContent(title), labelStyle);
            GUILayout.Space(titleFontSize * 0.5f);
        }

        /// <summary>
        /// Create a custom label style based on color and size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle GetLableStyle(int size, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = size;
            labelStyle.fixedHeight = size * 2;
            labelStyle.normal.textColor = color;
            return labelStyle;
        }

        /// <summary>
        /// draw a label with a yellow coloring
        /// </summary>
        /// <param name="warning"></param>
        public static void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = warningColor;
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

            GUI.color = defaultColor;
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

            GUI.color = successColor;
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

            GUI.color = errorColor;
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
            styleHR.border = new RectOffset(1,1,1,0);
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
            GUI.color = sectionColor;

            if (toUpper)
            {
                headerName = headerName.ToUpper();
            }

            bool drawSection = EditorGUILayout.Foldout(open, headerName, true, sectionStyle);
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
        protected void AdjustListSettings(int count)
        {
            if(listSettings == null)
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
        }

        public static string[] GetOptions(SerializedProperty options)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < options.arraySize; i++)
            {
                list.Add(options.GetArrayElementAtIndex(i).stringValue);
            }

            return list.ToArray();
        }

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

        public static string[] SerializedPropertyToOptions(SerializedProperty arr)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < arr.arraySize; i++)
            {
                list.Add(arr.GetArrayElementAtIndex(i).stringValue);
            }
            return list.ToArray();
        }

        protected static void PropertySettingsList(SerializedProperty settings, List<FieldData> data)
        {
            settings.ClearArray();

            for (int i = 0; i < data.Count; i++)
            {
                settings.InsertArrayElementAtIndex(settings.arraySize);
                SerializedProperty settingItem = settings.GetArrayElementAtIndex(settings.arraySize - 1);

                UpdatePropertySettings(settingItem, (int)data[i].Attributes.Type, data[i].Value);

                SerializedProperty type = settingItem.FindPropertyRelative("Type");
                SerializedProperty tooltip = settingItem.FindPropertyRelative("Tooltip");
                SerializedProperty label = settingItem.FindPropertyRelative("Label");
                SerializedProperty options = settingItem.FindPropertyRelative("Options");
                SerializedProperty name = settingItem.FindPropertyRelative("Name");

                type.enumValueIndex = (int)data[i].Attributes.Type;
                tooltip.stringValue = data[i].Attributes.Tooltip;
                label.stringValue = data[i].Attributes.Label;
                name.stringValue = data[i].Name;

                options.ClearArray();

                if (data[i].Attributes.Options != null)
                {
                    for (int j = 0; j < data[i].Attributes.Options.Length; j++)
                    {
                        options.InsertArrayElementAtIndex(j);
                        SerializedProperty item = options.GetArrayElementAtIndex(j);
                        item.stringValue = data[i].Attributes.Options[j];
                    }
                }
            }
        }

        public static void UpdatePropertySettings(SerializedProperty prop, int type, object update)
        {
            SerializedProperty intValue = prop.FindPropertyRelative("IntValue");
            SerializedProperty stringValue = prop.FindPropertyRelative("StringValue");

            switch ((InspectorField.FieldTypes)type)
            {
                case InspectorField.FieldTypes.Float:
                    SerializedProperty floatValue = prop.FindPropertyRelative("FloatValue");
                    floatValue.floatValue = (float)update;
                    break;
                case InspectorField.FieldTypes.Int:
                    intValue.intValue = (int)update;
                    break;
                case InspectorField.FieldTypes.String:

                    stringValue.stringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.Bool:
                    SerializedProperty boolValue = prop.FindPropertyRelative("BoolValue");
                    boolValue.boolValue = (bool)update;
                    break;
                case InspectorField.FieldTypes.Color:
                    SerializedProperty colorValue = prop.FindPropertyRelative("ColorValue");
                    colorValue.colorValue = (Color)update;
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    intValue.intValue = (int)update;
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    stringValue.stringValue = (string)update;
                    break;
                case InspectorField.FieldTypes.GameObject:
                    SerializedProperty gameObjectValue = prop.FindPropertyRelative("GameObjectValue");
                    gameObjectValue.objectReferenceValue = (GameObject)update;
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    SerializedProperty scriptableObjectValue = prop.FindPropertyRelative("ScriptableObjectValue");
                    scriptableObjectValue.objectReferenceValue = (ScriptableObject)update;
                    break;
                case InspectorField.FieldTypes.Object:
                    SerializedProperty objectValue = prop.FindPropertyRelative("ObjectValue");
                    objectValue.objectReferenceValue = (UnityEngine.Object)update;
                    break;
                case InspectorField.FieldTypes.Material:
                    SerializedProperty materialValue = prop.FindPropertyRelative("MaterialValue");
                    materialValue.objectReferenceValue = (Material)update;
                    break;
                case InspectorField.FieldTypes.Texture:
                    SerializedProperty textureValue = prop.FindPropertyRelative("TextureValue");
                    textureValue.objectReferenceValue = (Texture)update;
                    break;
                case InspectorField.FieldTypes.Vector2:
                    SerializedProperty vector2Value = prop.FindPropertyRelative("Vector2Value");
                    vector2Value.vector2Value = (Vector2)update;
                    break;
                case InspectorField.FieldTypes.Vector3:
                    SerializedProperty vector3Value = prop.FindPropertyRelative("Vector3Value");
                    vector3Value.vector3Value = (Vector3)update;
                    break;
                case InspectorField.FieldTypes.Vector4:
                    SerializedProperty vector4Value = prop.FindPropertyRelative("Vector4Value");
                    vector4Value.vector4Value = (Vector4)update;
                    break;
                case InspectorField.FieldTypes.Curve:
                    SerializedProperty curveValue = prop.FindPropertyRelative("CurveValue");
                    curveValue.animationCurveValue = (AnimationCurve)update;
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    SerializedProperty quaternionValue = prop.FindPropertyRelative("QuaternionValue");
                    quaternionValue.quaternionValue = (Quaternion)update;
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    SerializedProperty audioClip = prop.FindPropertyRelative("AudioClipValue");
                    audioClip.objectReferenceValue = (AudioClip)update;
                    break;
                case InspectorField.FieldTypes.Event:
                    //SerializedProperty uEvent = prop.FindPropertyRelative("EventValue");
                    //uEvent.objectReferenceValue = update as UnityEngine.Object;
                    break;
                default:
                    break;
            }
        }

        public static void DisplayPropertyField(SerializedProperty prop)
        {
            SerializedProperty type = prop.FindPropertyRelative("Type");
            SerializedProperty label = prop.FindPropertyRelative("Label");
            SerializedProperty tooltip = prop.FindPropertyRelative("Tooltip");
            SerializedProperty options = prop.FindPropertyRelative("Options");

            SerializedProperty intValue = prop.FindPropertyRelative("IntValue");
            SerializedProperty stringValue = prop.FindPropertyRelative("StringValue");

            switch ((InspectorField.FieldTypes)type.intValue)
            {
                case InspectorField.FieldTypes.Float:
                    SerializedProperty floatValue = prop.FindPropertyRelative("FloatValue");
                    floatValue.floatValue = EditorGUILayout.FloatField(new GUIContent(label.stringValue, tooltip.stringValue), floatValue.floatValue);
                    break;
                case InspectorField.FieldTypes.Int:
                    intValue.intValue = EditorGUILayout.IntField(new GUIContent(label.stringValue, tooltip.stringValue), intValue.intValue);
                    break;
                case InspectorField.FieldTypes.String:
                    stringValue.stringValue = EditorGUILayout.TextField(new GUIContent(label.stringValue, tooltip.stringValue), stringValue.stringValue);
                    break;
                case InspectorField.FieldTypes.Bool:
                    SerializedProperty boolValue = prop.FindPropertyRelative("BoolValue");
                    boolValue.boolValue = EditorGUILayout.Toggle(new GUIContent(label.stringValue, tooltip.stringValue), boolValue.boolValue);
                    break;
                case InspectorField.FieldTypes.Color:
                    SerializedProperty colorValue = prop.FindPropertyRelative("ColorValue");
                    colorValue.colorValue = EditorGUILayout.ColorField(new GUIContent(label.stringValue, tooltip.stringValue), colorValue.colorValue);
                    break;
                case InspectorField.FieldTypes.DropdownInt:
                    intValue.intValue = EditorGUILayout.Popup(label.stringValue, intValue.intValue, GetOptions(options));
                    break;
                case InspectorField.FieldTypes.DropdownString:
                    string[] stringOptions = GetOptions(options);
                    int selection = GetOptionsIndex(options, stringValue.stringValue);
                    int newIndex = EditorGUILayout.Popup(label.stringValue, intValue.intValue, stringOptions);
                    if (selection != newIndex)
                    {
                        stringValue.stringValue = stringOptions[newIndex];
                    }
                    break;
                case InspectorField.FieldTypes.GameObject:
                    SerializedProperty gameObjectValue = prop.FindPropertyRelative("GameObjectValue");
                    EditorGUILayout.PropertyField(gameObjectValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.ScriptableObject:
                    SerializedProperty scriptableObjectValue = prop.FindPropertyRelative("ScriptableObjectValue");
                    EditorGUILayout.PropertyField(scriptableObjectValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Object:
                    SerializedProperty objectValue = prop.FindPropertyRelative("ObjectValue");
                    EditorGUILayout.PropertyField(objectValue, new GUIContent(label.stringValue, tooltip.stringValue), true);
                    break;
                case InspectorField.FieldTypes.Material:
                    SerializedProperty materialValue = prop.FindPropertyRelative("MaterialValue");
                    EditorGUILayout.PropertyField(materialValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Texture:
                    SerializedProperty textureValue = prop.FindPropertyRelative("TextureValue");
                    EditorGUILayout.PropertyField(textureValue, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Vector2:
                    SerializedProperty vector2Value = prop.FindPropertyRelative("Vector2Value");
                    vector2Value.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(label.stringValue, tooltip.stringValue), vector2Value.vector2Value);
                    break;
                case InspectorField.FieldTypes.Vector3:
                    SerializedProperty vector3Value = prop.FindPropertyRelative("Vector3Value");
                    vector3Value.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(label.stringValue, tooltip.stringValue), vector3Value.vector3Value);
                    break;
                case InspectorField.FieldTypes.Vector4:
                    SerializedProperty vector4Value = prop.FindPropertyRelative("Vector4Value");
                    vector4Value.vector4Value = EditorGUILayout.Vector4Field(new GUIContent(label.stringValue, tooltip.stringValue), vector4Value.vector4Value);
                    break;
                case InspectorField.FieldTypes.Curve:
                    SerializedProperty curveValue = prop.FindPropertyRelative("CurveValue");
                    curveValue.animationCurveValue = EditorGUILayout.CurveField(new GUIContent(label.stringValue, tooltip.stringValue), curveValue.animationCurveValue);
                    break;
                case InspectorField.FieldTypes.Quaternion:
                    SerializedProperty quaternionValue = prop.FindPropertyRelative("QuaternionValue");
                    Vector4 vect4 = new Vector4(quaternionValue.quaternionValue.x, quaternionValue.quaternionValue.y, quaternionValue.quaternionValue.z, quaternionValue.quaternionValue.w);
                    vect4 = EditorGUILayout.Vector4Field(new GUIContent(label.stringValue, tooltip.stringValue), vect4);
                    quaternionValue.quaternionValue = new Quaternion(vect4.x, vect4.y, vect4.z, vect4.w);
                    break;
                case InspectorField.FieldTypes.AudioClip:
                    SerializedProperty audioClip = prop.FindPropertyRelative("AudioClipValue");
                    EditorGUILayout.PropertyField(audioClip, new GUIContent(label.stringValue, tooltip.stringValue), false);
                    break;
                case InspectorField.FieldTypes.Event:
                    SerializedProperty uEvent = prop.FindPropertyRelative("EventValue");
                    if (uEvent != null)
                    {
                        //Debug.Log("update? " + uEvent);
                    }
                    EditorGUILayout.PropertyField(uEvent, new GUIContent(label.stringValue, tooltip.stringValue));
                    break;
                default:
                    break;
            }
        }
    }
#endif
}
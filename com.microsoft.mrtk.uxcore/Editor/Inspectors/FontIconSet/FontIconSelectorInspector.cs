
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UX;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(FontIconSelector))]
    class FontIconSelectorInspector : UnityEditor.Editor
    {

        private const string defaultShaderName = "TextMeshPro/Distance Field SSD";
        private const int glyphDrawSize = 75;
        private const int buttonWidth = 75;
        private const int buttonHeight = 75;
        private const int maxButtonsPerColumn = 6;

        private static Material fontRenderMaterial;

        private const string noFontIconsMessage = "No IconFontSet profile selected. No icons available.";
        private const string emptyFontIconSetMessage = "The selected IconFontSet profile has no icons defined. Please edit the IconFontSet.";

        private SerializedProperty fontIconsProp = null;
        private SerializedProperty currentIconNameProp = null;
        private SerializedProperty tmProProp = null;

        private void OnEnable()
        {
            fontIconsProp = serializedObject.FindProperty("fontIcons");
            currentIconNameProp = serializedObject.FindProperty("currentIconName");
            tmProProp = serializedObject.FindProperty("textMeshProComponent");
        }

        public override void OnInspectorGUI()
        {
            FontIconSelector fontIconSelector = (FontIconSelector)target;
            FontIconSet fontIconSet = fontIconSelector.FontIcons;

            EditorGUILayout.PropertyField(fontIconsProp);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(currentIconNameProp);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(tmProProp);
            EditorGUILayout.Space();

            if (fontIconsProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(noFontIconsMessage, MessageType.Warning);
            }
            else if (!fontIconSet || fontIconSet.GlyphIconsByName.Count == 0)
            {
                EditorGUILayout.HelpBox(emptyFontIconSetMessage, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.LabelField("Choose the icon to show:");

                if (fontIconSet && fontIconSet.GlyphIconsByName.Count > 0)
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        DrawIconGrid(fontIconSelector, maxButtonsPerColumn, buttonHeight, buttonWidth);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void DrawIconGrid(FontIconSelector fontIconSelector, int maxButtonsPerColumn, int buttonHeight, int buttonWidth)
        {
            FontIconSet fontIconSet = fontIconSelector.FontIcons;
            TMP_FontAsset fontAsset = fontIconSet.IconFontAsset;
            int column = 0;
            EditorGUILayout.BeginHorizontal();

            foreach (string iconName in fontIconSet.GlyphIconsByName.Keys)
            {
                uint unicodeValue = fontIconSet.GlyphIconsByName[iconName];
                bool selected = (iconName == fontIconSelector.CurrentIconName);
                if (column >= maxButtonsPerColumn)
                {
                    column = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (GUILayout.Button(" ",
                    GUILayout.MinHeight(buttonHeight),
                    GUILayout.MaxHeight(buttonHeight),
                    GUILayout.MaxWidth(buttonWidth)))
                {
                    fontIconSelector.CurrentIconName = iconName;
                }
                Rect textureRect = GUILayoutUtility.GetLastRect();
                FontIconSetInspector.EditorDrawTMPGlyph(textureRect, unicodeValue, fontAsset, selected);
                column++;
            }

            if (column > 0)
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

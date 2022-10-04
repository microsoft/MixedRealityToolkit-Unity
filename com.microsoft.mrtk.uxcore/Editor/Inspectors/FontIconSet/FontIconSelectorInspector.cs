
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
        private float fontTileSize = 32;

        private static Material fontRenderMaterial;

        private const string noFontIconsMessage = "No IconFontSet profile selected. No icons available.";
        private const string emptyFontIconSetMessage = "The selected IconFontSet profile has no icons defined. Please edit the IconFontSet.";

        private SerializedProperty fontIconsProp = null;
        private SerializedProperty currentIconNameProp = null;
        private SerializedProperty tmProProp = null;

        private GUIStyle currentButtonStyle;

        private bool initializedStyle = false;

        private void OnEnable()
        {
            fontIconsProp = serializedObject.FindProperty("fontIcons");
            currentIconNameProp = serializedObject.FindProperty("currentIconName");
            tmProProp = serializedObject.FindProperty("textMeshProComponent");
            
        }

        public override void OnInspectorGUI()
        {
            if (!initializedStyle)
            {
                currentButtonStyle = new GUIStyle(GUI.skin.button);
                initializedStyle = true;
            }

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
                        fontTileSize = EditorGUILayout.Slider("Zoom", fontTileSize, 16, 64, GUILayout.ExpandWidth(false));
                        DrawIconGrid(fontIconSelector, fontTileSize);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private int numColumns = 4;

        private Vector2 scrollAmount;

        public void DrawIconGrid(FontIconSelector fontIconSelector, float tileSize)
        {
            FontIconSet fontIconSet = fontIconSelector.FontIcons;
            TMP_FontAsset fontAsset = fontIconSet.IconFontAsset;
            int column = 0;

            scrollAmount = EditorGUILayout.BeginScrollView(scrollAmount, GUILayout.MaxHeight(128), GUILayout.MinHeight(64));
            EditorGUILayout.BeginHorizontal();
            
            foreach (string iconName in fontIconSet.GlyphIconsByName.Keys)
            {
                uint unicodeValue = fontIconSet.GlyphIconsByName[iconName];
                bool selected = (iconName == fontIconSelector.CurrentIconName);
                if (column >= numColumns)
                {
                    column = 0;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (GUILayout.Button(" ",
                    GUILayout.MinHeight(tileSize),
                    GUILayout.MaxHeight(tileSize),
                    GUILayout.MinWidth(tileSize),
                    GUILayout.MaxWidth(tileSize)))
                {
                    Undo.RecordObjects(new UnityEngine.Object[]{fontIconSelector, fontIconSelector.TextMeshProComponent}, "Changed icon");
                    fontIconSelector.CurrentIconName = iconName;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(fontIconSelector);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(fontIconSelector.TextMeshProComponent);
                }
                Rect textureRect = GUILayoutUtility.GetLastRect();
                if (textureRect.yMin + 8 < scrollAmount.y || textureRect.yMax - 8 > scrollAmount.y + 128)
                {
                    unicodeValue = 0;
                }

                textureRect.width = tileSize;
                textureRect.height = tileSize;
                FontIconSetInspector.EditorDrawTMPGlyph(textureRect, unicodeValue, fontAsset, selected);
                column++;
            }

            if (column > 0)
            {
                EditorGUILayout.EndHorizontal();
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                float editorWindowWidth = GUILayoutUtility.GetLastRect().width;
                numColumns = (int)Mathf.Floor(editorWindowWidth / (tileSize + currentButtonStyle.margin.right));
            }

            EditorGUILayout.EndScrollView();
        }
    }
}

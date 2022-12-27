
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using Microsoft.MixedReality.Toolkit.UX;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(FontIconSet))]
    public class FontIconSetInspector : UnityEditor.Editor
    {
        private const string ShowGlyphIconsFoldoutKey = "MixedRealityToolkit.FontIconSet.ShowIconFoldout";
        private const string AvailableIconsFoldoutKey = "MixedRealityToolkit.FontIconSet.ShowAvailableIcons";
        private const string SelectedIconsFoldoutKey = "MixedRealityToolkit.FontIconSet.ShowSelectedIcons";

        private const string defaultShaderName = "TextMeshPro/Distance Field SSD"; // Only used for presentation in inspector, not at runtime.
        private const int glyphDrawSize = 75;
        private const int buttonWidth = 75;
        private const int buttonHeight = 90;
        private const int maxButtonsPerColumn = 6;

        private static Material fontRenderMaterial;

        private const string noIconFontMessage = "No icon font asset selected. Icon fonts will be unavailable.";
        private const string downloadIconFontMessage = "For instructions on how to install the HoloLens icon font asset, click the button below.";
        private const string hololensIconFontUrl = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/button";
        private const string mdl2IconFontName = "holomdl2";
        private const string textMeshProMenuItem = "Window/TextMeshPro/Font Asset Creator";

        private SerializedProperty iconFontAssetProp = null;

        private class IconEntry
        {
            public string Name;
            public uint UnicodeValue;

            public IconEntry(string name, uint unicodeValue)
            {
                Name = name;
                UnicodeValue = unicodeValue;
            }
        }

        private List<IconEntry> iconEntries = new List<IconEntry>();

        private void OnEnable()
        {
            FontIconSet fontIconSet = (FontIconSet)target;
            iconFontAssetProp = serializedObject.FindProperty("iconFontAsset");

            // Make a list out of dictionary to avoid changing order while editing names
            foreach (KeyValuePair<string, uint> kv in fontIconSet.GlyphIconsByName)
            {
                iconEntries.Add(new IconEntry(kv.Key, kv.Value));
            }
        }

        public override void OnInspectorGUI()
        {
            FontIconSet fontIconSet = (FontIconSet)target;

            bool showGlyphIconFoldout = SessionState.GetBool(ShowGlyphIconsFoldoutKey, false);
            bool showAvailableIcons = SessionState.GetBool(AvailableIconsFoldoutKey, true);
            bool showSelectedIcons = SessionState.GetBool(SelectedIconsFoldoutKey, true);

            serializedObject.Update();

            showGlyphIconFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showGlyphIconFoldout, "Font Icons");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (showGlyphIconFoldout)
            {
                EditorGUILayout.PropertyField(iconFontAssetProp);

                if (iconFontAssetProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(noIconFontMessage, MessageType.Warning);
                    if (!CheckIfHololensIconFontExists())
                    {
                        EditorGUILayout.HelpBox(downloadIconFontMessage, MessageType.Info);
                        if (GUILayout.Button("View Font Asset Icons Documentation"))
                        {
                            EditorApplication.ExecuteMenuItem(textMeshProMenuItem);
                            Application.OpenURL(hololensIconFontUrl);
                        }
                    }
                }
                else
                {
                    TMP_FontAsset fontAsset = (TMP_FontAsset)iconFontAssetProp.objectReferenceValue;

                    showAvailableIcons = EditorGUILayout.BeginFoldoutHeaderGroup(showAvailableIcons, "Available Icons");
                    if (showAvailableIcons)
                    {
                        if (fontAsset.characterTable.Count == 0)
                        {
                            EditorGUILayout.HelpBox("No icons are available in this font. The font may be configured incorrectly.", MessageType.Warning);
                            if (GUILayout.Button("Open Font Editor"))
                            {
                                Selection.activeObject = fontIconSet.IconFontAsset;
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Click an icon to add it to your selected icons.", MessageType.Info);
                            if (GUILayout.Button("Open Font Editor"))
                            {
                                Selection.activeObject = fontIconSet.IconFontAsset;
                            }

                            DrawFontGlyphsGrid(fontIconSet, maxButtonsPerColumn);
                        }

                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                    showSelectedIcons = EditorGUILayout.BeginFoldoutHeaderGroup(showSelectedIcons, "Selected Icons");
                    if (showSelectedIcons)
                    {
                        if (fontIconSet.GlyphIconsByName.Count > 0)
                        {
                            EditorGUILayout.HelpBox("These icons will appear in the button config helper inspector. Click an icon to remove it from this list.", MessageType.Info);

                            using (new EditorGUILayout.VerticalScope())
                            {
                                string iconToRemove = null;
                                foreach (IconEntry iconEntry in iconEntries)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        if (GUILayout.Button(" ", GUILayout.MinHeight(buttonHeight), GUILayout.MaxHeight(buttonHeight), GUILayout.MaxWidth(buttonWidth)))
                                        {
                                            iconToRemove = iconEntry.Name;
                                        }
                                        Rect textureRect = GUILayoutUtility.GetLastRect();
                                        textureRect.width = glyphDrawSize;
                                        textureRect.height = glyphDrawSize;
                                        EditorDrawTMPGlyph(textureRect, iconEntry.UnicodeValue, fontAsset);

                                        string currentName = iconEntry.Name;
                                        currentName = EditorGUILayout.TextField(currentName);
                                        if (currentName != iconEntry.Name)
                                        {
                                            UpdateIconName(fontIconSet, iconEntry.Name, currentName);
                                        }
                                        EditorGUILayout.Space();
                                    }

                                    EditorGUILayout.Space();
                                }

                                if (iconToRemove != null)
                                {
                                    RemoveIcon(fontIconSet, iconToRemove);
                                }
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No icons added yet. Click available icons to add.", MessageType.Info);
                        }
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();
                }
            }

            SessionState.SetBool(ShowGlyphIconsFoldoutKey, showGlyphIconFoldout);
            SessionState.SetBool(AvailableIconsFoldoutKey, showAvailableIcons);
            SessionState.SetBool(SelectedIconsFoldoutKey, showSelectedIcons);

            serializedObject.ApplyModifiedProperties();
        }

        public void DrawFontGlyphsGrid(FontIconSet fontIconSet, int maxButtonsPerColumn)
        {
            TMP_FontAsset fontAsset = fontIconSet.IconFontAsset;
            int column = 0;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < fontAsset.characterTable.Count; i++)
            {
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
                    AddIcon(fontIconSet, fontAsset.characterTable[i].unicode);
                    EditorUtility.SetDirty(target);
                }
                Rect textureRect = GUILayoutUtility.GetLastRect();
                textureRect.width = glyphDrawSize;
                textureRect.height = glyphDrawSize;
                EditorDrawTMPGlyph(textureRect, fontAsset, fontAsset.characterTable[i]);
                column++;
            }

            if (column > 0)
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private bool AddIcon(FontIconSet fontIconSet, uint unicodeValue)
        {
            string name = "Icon " + (iconEntries.Count + 1);
            if (fontIconSet.AddIcon(name, unicodeValue))
            {
                iconEntries.Add(new IconEntry(name, unicodeValue));
            }
            return true;
        }

        private bool RemoveIcon(FontIconSet fontIconSet, string iconName)
        {
            bool removed = fontIconSet.RemoveIcon(iconName);
            if (removed)
            {
                if (FindIconIndexByName(iconName, out int index))
                {
                    iconEntries.RemoveAt(index);
                }
            }
            return removed;
        }

        private bool FindIconIndexByName(string iconName, out int outIndex)
        {
            for (outIndex = 0; outIndex < iconEntries.Count; outIndex++)
            {
                if (iconEntries[outIndex].Name == iconName)
                {
                    return true;
                }
            }
            outIndex = -1;
            return false;
        }

        private void UpdateIconName(FontIconSet fontIconSet, string oldName, string newName)
        {
            if (fontIconSet.UpdateIconName(oldName, newName))
            {
                if (FindIconIndexByName(oldName, out int index))
                {
                    iconEntries[index].Name = newName;
                }
            }
        }

        private bool CheckIfHololensIconFontExists()
        {
            foreach (string guid in AssetDatabase.FindAssets($"t:{typeof(UnityEngine.Font).Name}"))
            {
                if (AssetDatabase.GUIDToAssetPath(guid).Contains(mdl2IconFontName))
                {
                    return true;
                }
            }
            return false;
        }

        public static void EditorDrawTMPGlyph(Rect position, uint unicode, TMP_FontAsset fontAsset, bool selected = false, Material fontRenderMaterial = null)
        {
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out TMP_Character character))
            {
                EditorDrawTMPGlyph(position, fontAsset, character, selected, fontRenderMaterial);
            }
        }

        /// <summary>
        /// Draws a Text Mesh Pro glyph in the supplied rect. Used for inspectors.
        /// </summary>
        public static void EditorDrawTMPGlyph(Rect glyphRect, TMP_FontAsset fontAsset, TMP_Character character, bool selected = false, Material fontRenderMaterial = null)
        {
            if (Event.current.type == EventType.Repaint)
            {
                try
                {
                    float iconSizeMultiplier = 0.625f;

                    // Get a reference to the Glyph Table
                    int glyphIndex = (int)character.glyphIndex;
                    int elementIndex = fontAsset.glyphTable.FindIndex(item => item.index == glyphIndex);

                    if (elementIndex >= 0)
                    {
                        Glyph glyph = character.glyph;

                        // Get reference to atlas texture.
                        int atlasIndex = glyph.atlasIndex;
                        Texture2D atlasTexture = fontAsset.atlasTextures.Length > atlasIndex ? fontAsset.atlasTextures[atlasIndex] : null;

                        if (atlasTexture != null)
                        {
                            if (fontRenderMaterial == null)
                            {
                                fontRenderMaterial = new Material(Shader.Find(defaultShaderName));
                            }

                            Material glyphMaterial = fontRenderMaterial;
                            glyphMaterial.mainTexture = atlasTexture;

                            if (selected)
                            {
                                glyphMaterial.SetColor("_Color", Color.green);
                            }
                            else
                            {
                                glyphMaterial.SetColor("_Color", Color.white);
                            }

                            int glyphOriginX = glyph.glyphRect.x;
                            int glyphOriginY = glyph.glyphRect.y;
                            int glyphWidth = glyph.glyphRect.width;
                            int glyphHeight = glyph.glyphRect.height;

                            float normalizedHeight = fontAsset.faceInfo.ascentLine - fontAsset.faceInfo.descentLine;
                            float scale = Mathf.Min(glyphRect.width, glyphRect.height) / normalizedHeight * iconSizeMultiplier;

                            // Compute the normalized texture coordinates
                            Rect texCoords = new Rect((float)glyphOriginX / atlasTexture.width, (float)glyphOriginY / atlasTexture.height, (float)glyphWidth / atlasTexture.width, (float)glyphHeight / atlasTexture.height);

                            glyphWidth = (int)Mathf.Min(glyphDrawSize, glyphWidth * scale);
                            glyphHeight = (int)Mathf.Min(glyphDrawSize, glyphHeight * scale);

                            glyphRect.x += (glyphRect.width - glyphWidth) / 2;
                            glyphRect.y += (glyphRect.height - glyphHeight) / 2;
                            glyphRect.width = glyphWidth;
                            glyphRect.height = glyphHeight;

                            // Could switch to using the default material of the font asset which would require passing scale to the shader.
                            Graphics.DrawTexture(glyphRect, atlasTexture, texCoords, 0, 0, 0, 0, new Color(1f, 1f, 1f), glyphMaterial);
                        }
                    }
                }
                catch (Exception)
                {
                    EditorGUILayout.LabelField("Couldn't draw character icon. UnicodeValue may not be available in the font asset.");
                }
            }
        }
    }
}

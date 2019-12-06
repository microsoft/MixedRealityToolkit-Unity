// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using TMPro;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.TextCore;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "IconSet", menuName = "Mixed Reality Toolkit/IconSet")]
    public class ButtonIconSet : ScriptableObject
    {
        // Struct used to pair a font icon with a searchable name.
        [Serializable]
        public struct CharIcon
        {
            public string Name;
            public uint Character;
        }

        public Texture2D[] QuadIcons => quadIcons;

        public Sprite[] SpriteIcons => spriteIcons;

        public CharIcon[] CharIcons => charIcons;

        public TMP_FontAsset CharIconFont => charIconFont;

        [SerializeField]
        private Texture2D[] quadIcons = new Texture2D[0];
        [SerializeField]
        private Sprite[] spriteIcons = new Sprite[0];
        [SerializeField]
        private TMP_FontAsset charIconFont = null;
        [SerializeField]
        [Tooltip("See TextMeshPro font assets for available unicode characters. Default characters are drawn from the HoloSymMDL2 font.")]
        private CharIcon[] charIcons = new CharIcon[]
        {
            new CharIcon{ Character = ConvertCharStringToUInt32("\uEBD2"), Name = "AppBarAdjust" },
            new CharIcon{ Character = ConvertCharStringToUInt32("\uE711"), Name = "AppBarClose" },
            new CharIcon{ Character = ConvertCharStringToUInt32("\uE8FB"), Name = "AppBarDone" },
            new CharIcon{ Character = ConvertCharStringToUInt32("\uE76C"), Name = "AppBarHide" },
            new CharIcon{ Character = ConvertCharStringToUInt32("\uE712"), Name = "AppBarShow" },
            new CharIcon{ Character = ConvertCharStringToUInt32("\uEB0F"), Name = "AppBarHome" },
        };

        private Dictionary<string, uint> charIconLookup = new Dictionary<string, uint>();
        private Dictionary<string, Texture2D> quadIconLookup = new Dictionary<string, Texture2D>();
        private Dictionary<string, Sprite> spriteIconLookup = new Dictionary<string, Sprite>();
        private bool lookupsInitialized = false;

        /// <summary>
        /// Tries to retrieve a unicode character icon by name.
        /// </summary>
        /// <returns>True if icon is found.</returns>
        public bool TryGetCharIcon(string iconName, out uint charIcon)
        {
            InitializeLookups();
            return charIconLookup.TryGetValue(iconName, out charIcon);
        }

        /// <summary>
        /// Tries to retrieve a texture icon by name.
        /// </summary>
        /// <returns>True if icon is found.</returns>
        public bool TryGetQuadIcon(string iconName, out Texture2D quadIcon)
        {
            InitializeLookups();
            return quadIconLookup.TryGetValue(iconName, out quadIcon);
        }

        /// <summary>
        /// Tries to retrieve a sprite icon by name.
        /// </summary>
        /// <returns>True if icon is found.</returns>
        public bool TryGetSpriteIcon(string iconName, out Sprite spriteIcon)
        {
            InitializeLookups();
            return spriteIconLookup.TryGetValue(iconName, out spriteIcon);
        }

        private void InitializeLookups()
        {
            if (lookupsInitialized)
            {
                return;
            }

            try
            {
                foreach (Texture2D quadIcon in quadIcons)
                {
                    if (quadIconLookup.ContainsKey(quadIcon.name))
                    {
                        Debug.LogError("Icon set contains multiple texture assets with the same name. This is not permitted: " + quadIcon.name);
                        continue;
                    }

                    quadIconLookup.Add(quadIcon.name, quadIcon);
                }

                foreach (Sprite spriteIcon in spriteIcons)
                {
                    if (spriteIconLookup.ContainsKey(spriteIcon.name))
                    {
                        Debug.LogError("Icon set contains multiple texture assets with the same name. This is not permitted: " + spriteIcon.name);
                        continue;
                    }

                    spriteIconLookup.Add(spriteIcon.name, spriteIcon);
                }

                foreach (CharIcon charIcon in charIcons)
                {
                    if (string.IsNullOrEmpty(charIcon.Name))
                    {   // Un-named icons are skipped without error.
                        continue;
                    }

                    if (charIcon.Character == 0)
                    {
                        Debug.LogWarning("Char icon " + charIcon.Name +  " in icon set " + name + " is set to 0. This is likely an error.");                        
                    }

                    if (charIconLookup.ContainsKey(charIcon.Name))
                    {
                        Debug.LogError("Icon set contains multiple font characters with the same name. This is not permitted: " + charIcon.Name);
                        continue;
                    }

                    charIconLookup.Add(charIcon.Name, charIcon.Character);
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("There's a null element in your icon set. Icon lookup by name will be disabled until this is resolved.");
            }
            catch (UnassignedReferenceException)
            {
                Debug.LogWarning("There's a null element in your icon set. Icon lookup by name will be disabled until this is resolved.");
            }

            lookupsInitialized = true;
        }

        /// <summary>
        /// Converts a unicode string to a uint code (for use with TextMeshPro).
        /// </summary>
        public static uint ConvertCharStringToUInt32(string charString)
        {
            uint unicode = 0;

            if (string.IsNullOrEmpty(charString))
            {
                return 0;
            }

            for (int i = 0; i < charString.Length; i++)
            {
                unicode = charString[i];
                // Handle surrogate pairs
                if (i < charString.Length - 1 && char.IsHighSurrogate((char)unicode) && char.IsLowSurrogate(charString[i + 1]))
                {
                    unicode = (uint)char.ConvertToUtf32(charString[i], charString[i + 1]);
                    i += 1;
                }
            }
            return unicode;
        }

        /// <summary>
        /// Converts a uint code to a string (used to convert TextMeshPro codes into a string for text fields).
        /// </summary>
        public static string ConvertUInt32ToUnicodeCharString(uint unicode)
        {
            byte[] bytes = System.BitConverter.GetBytes(unicode);
            return Encoding.Unicode.GetString(bytes);
        }

#if UNITY_EDITOR

        private const int maxButtonSize = 75;
        private const int charIconFontSize = 40;
        private const int maxButtonsPerColumn = 6; 
        private Texture[] spriteIconTextures = null;
        private static Material fontRenderMat;

        private const string noIconFontMessage = "No icon font selected. Icon fonts will be unavailable.";
        private const string downloadIconFontMessage = "You can download the Hololens icon font by clicking the button below.";
        private const string hololensIconFontUrl = "https://aka.ms/hololensiconfont";
        private const string mdl2IconFontName = "HoloSymMDL2";
        private const string textMeshProMenuItem = "Window/TextMeshPro/Font Asset Creator";

        public void EditorResetCharIconLookups()
        {
            charIconLookup.Clear();
            quadIconLookup.Clear();
            spriteIconLookup.Clear();
            lookupsInitialized = false;
        }

        /// <summary>
        /// Draws a selectable grid of character icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawCharIconSelector(uint currentChar, out uint newChar, int indentLevel = 0)
        {
            newChar = 0;

            if (charIconFont == null)
            {
                EditorGUILayout.HelpBox("No icon font has been set in the icon set.", MessageType.Warning);
                return false;
            }

            int currentSelection = -1;
            for (int i = 0; i < charIcons.Length; i++)
            {
                if (charIcons[i].Character == currentChar)
                {
                    currentSelection = i;
                    break;
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                int column = 0;
                int newSelection = -1;

                if (charIcons.Length > 0)
                {
                    column = 0;
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < charIcons.Length; i++)
                    {
                        if (column >= maxButtonsPerColumn)
                        {
                            column = 0;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        if (GUILayout.Button(" ", GUILayout.MinHeight(maxButtonSize), GUILayout.MaxHeight(maxButtonSize)))
                        {
                            newSelection = i;
                        }
                        Rect textureRect = GUILayoutUtility.GetLastRect();
                        EditorDrawTMPGlyph(textureRect, charIcons[i].Character, charIconFont);
                        column++;
                    }

                    if (column > 0)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("(No icons in set)");
                }

                if (newSelection >= 0 && newSelection != currentSelection)
                {
                    newChar = charIcons[newSelection].Character;
                }
            }

            return newChar > 0;
        }

        /// <summary>
        /// Draws a selectable grid of sprite icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawSpriteIconSelector(Sprite currentSprite, out Sprite newSprite, int indentLevel = 0)
        {
            newSprite = null;

            int currentSelection = -1;
            for (int i = 0; i < spriteIcons.Length; i++)
            {
                if (spriteIcons[i] == currentSprite)
                {
                    currentSelection = i;
                    break;
                }
            }

            if (spriteIconTextures == null || spriteIconTextures.Length != spriteIcons.Length)
            {
                spriteIconTextures = new Texture[spriteIcons.Length];
                for (int i = 0; i < spriteIcons.Length; i++)
                {   // Note: This will not display correctly if the sprite is using an atlas.
                    spriteIconTextures[i] = spriteIcons[i].texture;
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                float height = maxButtonSize * ((float)spriteIcons.Length / maxButtonsPerColumn);
                var maxHeight = GUILayout.MaxHeight(height);
                int newSelection = GUILayout.SelectionGrid(currentSelection, spriteIconTextures, maxButtonsPerColumn, maxHeight);
                if (newSelection >= 0 && newSelection != currentSelection)
                {
                    newSprite = spriteIcons[newSelection];
                }
            }

            return newSprite != null;
        }

        /// <summary>
        /// Draws a selectable grid of texture icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawQuadIconSelector(Texture currentTexture, out Texture newTexture, int indentLevel = 0)
        {
            newTexture = null;
            int currentSelection = -1;

            for (int i = 0; i < quadIcons.Length; i++)
            {
                if (quadIcons[i] == currentTexture)
                {
                    currentSelection = i;
                    break;
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                float height = maxButtonSize * ((float)quadIcons.Length / maxButtonsPerColumn);
                var maxHeight = GUILayout.MaxHeight(height);
                int newSelection = GUILayout.SelectionGrid(currentSelection, quadIcons, maxButtonsPerColumn, maxHeight);
                if (newSelection >= 0 && newSelection != currentSelection)
                {
                    newTexture = quadIcons[newSelection];
                }
            }

            return newTexture != null;
        }

        private static void EditorDrawTMPGlyph(Rect position, uint unicode, TMP_FontAsset fontAsset)
        {
            TMP_Character character;
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out character))
            {
                EditorDrawTMPGlyph(position, fontAsset, character);
            }
        }

        /// <summary>
        /// Draws a Text Mesh Pro glyph in the supplied rect. Used for inspectors.
        /// </summary>
        private static void EditorDrawTMPGlyph(Rect position, TMP_FontAsset fontAsset, TMP_Character character)
        {
            float iconSizeMultiplier = 0.625f;

            // Get a reference to the Glyph Table
            int glyphIndex = (int)character.glyphIndex;
            int elementIndex = fontAsset.glyphTable.FindIndex(item => item.index == glyphIndex);

            // Return if we can't find the glyph
            if (elementIndex == -1)
            {
                return;
            }

            Glyph glyph = character.glyph;

            // Get reference to atlas texture.
            int atlasIndex = glyph.atlasIndex;
            Texture2D atlasTexture = fontAsset.atlasTextures.Length > atlasIndex ? fontAsset.atlasTextures[atlasIndex] : null;

            if (atlasTexture == null)
                return;

            if (fontRenderMat == null)
                fontRenderMat = new Material(Shader.Find("Mixed Reality Toolkit/TextMeshPro"));

            Material mat = fontRenderMat;
            mat.mainTexture = atlasTexture;
            mat.SetColor("_Color", Color.white);

            // Draw glyph
            Rect glyphDrawPosition = new Rect(
                position.x,
                position.y,
                position.width,
                position.height);

            int glyphOriginX = glyph.glyphRect.x;
            int glyphOriginY = glyph.glyphRect.y;
            int glyphWidth = glyph.glyphRect.width;
            int glyphHeight = glyph.glyphRect.height;

            float normalizedHeight = fontAsset.faceInfo.ascentLine - fontAsset.faceInfo.descentLine;
            float scale = Mathf.Min(glyphDrawPosition.width, glyphDrawPosition.height) / normalizedHeight * iconSizeMultiplier;

            // Compute the normalized texture coordinates
            Rect texCoords = new Rect((float)glyphOriginX / atlasTexture.width, (float)glyphOriginY / atlasTexture.height, (float)glyphWidth / atlasTexture.width, (float)glyphHeight / atlasTexture.height);

            if (Event.current.type == EventType.Repaint)
            {
                glyphHeight = (int)Mathf.Min(maxButtonSize, glyphHeight * scale);
                glyphWidth = (int)Mathf.Min(maxButtonSize, glyphWidth * scale);

                glyphDrawPosition.x += (glyphDrawPosition.width - glyphWidth) / 2;
                glyphDrawPosition.y += (glyphDrawPosition.height - glyphHeight) / 2;
                glyphDrawPosition.width = glyphWidth;
                glyphDrawPosition.height = glyphHeight;

                // Could switch to using the default material of the font asset which would require passing scale to the shader.
                Graphics.DrawTexture(glyphDrawPosition, atlasTexture, texCoords, 0, 0, 0, 0, new Color(1f, 1f, 1f), mat);
            }
        }

        [CustomEditor(typeof(ButtonIconSet))]
        private class ButtonIconSetInspector : UnityEditor.Editor
        {
            private const string ShowIconFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowIconFoldout";

            private SerializedProperty quadIconsProp = null;
            private SerializedProperty spriteIconsProp = null;
            private SerializedProperty charIconFontProp = null;
            private SerializedProperty charIconsProp = null;

            private void OnEnable()
            {
                quadIconsProp = serializedObject.FindProperty("quadIcons");
                spriteIconsProp = serializedObject.FindProperty("spriteIcons");
                charIconFontProp = serializedObject.FindProperty("charIconFont");
                charIconsProp = serializedObject.FindProperty("charIcons");
            }

            public override void OnInspectorGUI()
            {
                ButtonIconSet bis = (ButtonIconSet)target;

                EditorGUILayout.PropertyField(quadIconsProp, true);
                EditorGUILayout.PropertyField(spriteIconsProp, true);

                bool showIconFoldout = SessionState.GetBool(ShowIconFoldoutKey, false);
                showIconFoldout = EditorGUILayout.Foldout(showIconFoldout, "Choose Font Icons", true);

                if (showIconFoldout)
                {
                    EditorGUILayout.PropertyField(charIconFontProp);

                    if (charIconFontProp.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(noIconFontMessage, MessageType.Warning);
                        if (!CheckIfHololensIconFontExists())
                        {
                            EditorGUILayout.HelpBox(downloadIconFontMessage, MessageType.Info);
                            if (GUILayout.Button("Download Icon Font"))
                            {
                                EditorApplication.ExecuteMenuItem(textMeshProMenuItem);
                                Application.OpenURL(hololensIconFontUrl);
                            }
                        }
                    }
                    else
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            TMP_FontAsset fontAsset = (TMP_FontAsset)charIconFontProp.objectReferenceValue;

                            int removeIndex = -1;
                            int addIndex = -1;
                            int column = 0;

                            if (bis.charIcons.Length > 0)
                            {
                                EditorGUILayout.LabelField("Click icon to remove from set");

                                using (new EditorGUILayout.VerticalScope())
                                {
                                    for (int i = 0; i < bis.charIcons.Length; i++)
                                    {
                                        SerializedProperty charIconNameprop = charIconsProp.GetArrayElementAtIndex(i).FindPropertyRelative("Name");

                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            if (GUILayout.Button(" ", GUILayout.MinHeight(maxButtonSize), GUILayout.MaxHeight(maxButtonSize)))
                                            {
                                                removeIndex = i;
                                            }
                                            Rect textureRect = GUILayoutUtility.GetLastRect();
                                            EditorDrawTMPGlyph(textureRect, bis.charIcons[i].Character, fontAsset);
                                            charIconNameprop.stringValue = EditorGUILayout.TextField(charIconNameprop.stringValue);
                                            EditorGUILayout.Space();
                                        }

                                        EditorGUILayout.Space();
                                    }
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("(No icons in set)");
                            }

                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Click icon to add to set");
                            column = 0;
                            EditorGUILayout.BeginHorizontal();
                            for (int i = 0; i < fontAsset.characterTable.Count; i++)
                            {
                                if (column >= maxButtonsPerColumn)
                                {
                                    column = 0;
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                }
                                if (GUILayout.Button(" ", GUILayout.MinHeight(maxButtonSize), GUILayout.MaxHeight(maxButtonSize)))
                                {
                                    addIndex = i;
                                }
                                Rect textureRect = GUILayoutUtility.GetLastRect();
                                EditorDrawTMPGlyph(textureRect, fontAsset, fontAsset.characterTable[i]);
                                column++;
                            }

                            if (column > 0)
                            {
                                EditorGUILayout.EndHorizontal();
                            }

                            if (removeIndex >= 0)
                            {
                                List<CharIcon> charIconsSet = new List<CharIcon>(bis.charIcons);
                                charIconsSet.RemoveAt(removeIndex);
                                bis.charIcons = charIconsSet.ToArray();
                                EditorUtility.SetDirty(target);
                            }

                            if (addIndex >= 0)
                            {
                                uint unicode = fontAsset.characterTable[addIndex].unicode;
                                bool alreadyContainsIcon = false;
                                foreach (CharIcon c in bis.charIcons)
                                {
                                    if (c.Character == unicode)
                                    {
                                        alreadyContainsIcon = true;
                                        break;
                                    }
                                }

                                if (!alreadyContainsIcon)
                                {
                                    List<CharIcon> charIconsSet = new List<CharIcon>(bis.charIcons);
                                    charIconsSet.Add(new CharIcon { Character = unicode, Name = "Icon " + charIconsSet.Count.ToString() });
                                    bis.charIcons = charIconsSet.ToArray();
                                    EditorUtility.SetDirty(target);
                                }
                            }

                            if (GUILayout.Button("Open Font Editor"))
                            {
                                Selection.activeObject = bis.CharIconFont;
                            }
                        }
                    }
                }

                SessionState.SetBool(ShowIconFoldoutKey, showIconFoldout);
                serializedObject.ApplyModifiedProperties();
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
        }
#endif
    }
}
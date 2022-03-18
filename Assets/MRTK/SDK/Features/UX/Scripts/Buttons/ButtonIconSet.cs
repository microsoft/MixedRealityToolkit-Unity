// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// An asset for storing textures, sprites and character icons for use with MRTK buttons. Used by ButtonConfigHelper script.
    /// </summary>
    [CreateAssetMenu(fileName = "IconSet", menuName = "Mixed Reality/Toolkit/IconSet")]
    public class ButtonIconSet : ScriptableObject
    {
        // Struct used to pair a font icon with a searchable name.
        [Serializable]
        public struct CharIcon
        {
            public string Name;
            public uint Character;
        }

        /// <summary>
        /// A set of quad icons for use by ButtonConfigHelper.
        /// </summary>
        public Texture2D[] QuadIcons => quadIcons;

        /// <summary>
        /// A set of sprite icons for use by ButtonConfigHelper.
        /// </summary>
        public Sprite[] SpriteIcons => spriteIcons;

        /// <summary>
        /// A set of CharIcons for use by ButtonConfigHelper.
        /// </summary>
        public CharIcon[] CharIcons => charIcons;

        /// <summary>
        /// The font used to render CharIcons.
        /// </summary>
        public TMP_FontAsset CharIconFont => charIconFont;

        [SerializeField]
        private Texture2D[] quadIcons = new Texture2D[0];
        [SerializeField]
        private Sprite[] spriteIcons = new Sprite[0];
        [SerializeField]
        private TMP_FontAsset charIconFont = null;
        [SerializeField, Tooltip("See TextMeshPro font assets for available unicode characters. Default characters are drawn from the HoloSymMDL2 font.")]
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
        /// Searches for a char icon by name.
        /// </summary>
        /// <returns>True if char icon was found.</returns>
        public bool TryGetCharIcon(string iconName, out uint charIcon)
        {
            InitializeLookups();
            return charIconLookup.TryGetValue(iconName, out charIcon);
        }

        /// <summary>
        /// Searches for a quad texture by name.
        /// </summary>
        /// <returns>True if quad icon was found.</returns>
        public bool TryGetQuadIcon(string iconName, out Texture2D quadIcon)
        {
            InitializeLookups();
            return quadIconLookup.TryGetValue(iconName, out quadIcon);
        }

        /// <summary>
        /// Searches for a sprite for sprite icon by name.
        /// </summary>
        /// <returns>True if sprite icon was found.</returns>
        public bool TryGetSpriteIcon(string iconName, out Sprite spriteIcon)
        {
            InitializeLookups();
            return spriteIconLookup.TryGetValue(iconName, out spriteIcon);
        }

        private void InitializeLookups()
        {
#if UNITY_EDITOR
            if (quadIconLookup.Count != quadIcons.Length ||
                spriteIconLookup.Count != spriteIcons.Length ||
                charIconLookup.Count != charIcons.Length)
            {   // Our lookups are stale
                EditorResetCharIconLookups();
            }
#endif

            if (lookupsInitialized)
                return;

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
                        Debug.LogWarning("Char icon " + charIcon.Name + " in icon set " + name + " is set to 0. This is likely an error.");
                    }

                    if (charIconLookup.ContainsKey(charIcon.Name))
                    {
                        Debug.LogError("Icon set contains multiple font characters with the same name. This is not permitted: " + charIcon.Name);
                        continue;
                    }

                    charIconLookup.Add(charIcon.Name, charIcon.Character);
                }
            }
            catch (System.Exception)
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
                return 0;

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
        private const int maxButtonsPerColumn = 6;

        private Texture[] spriteIconTextures = null;
        private static Material fontRenderMat;

        private const string missingPreviewImagesMessage = "Not all icon previews were loaded. Check the settings of the icons included in your iconset.";
        private const string noIconFontMessage = "No icon font selected. Icon fonts will be unavailable.";
        private const string downloadIconFontMessage = "For instructions on how to install the HoloLens icon font, click the button below.";
        private const string hololensIconFontUrl = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/ux-building-blocks/button";
        private const string mdl2IconFontName = "holomdl2";
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
        public bool EditorDrawCharIconSelector(uint currentChar, out bool foundChar, out uint newChar, int indentLevel = 0)
        {
            newChar = 0;
            foundChar = false;
            int currentSelection = -1;

            if (charIconFont == null)
            {
                EditorGUILayout.HelpBox("No icon font has been set in the icon set.", MessageType.Warning);
                return false;
            }

            for (int i = 0; i < charIcons.Length; i++)
            {
                if (charIcons[i].Character == currentChar)
                {
                    currentSelection = i;
                    break;
                }
            }

            if (currentSelection >= 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Current Icon Name", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(charIcons[currentSelection].Name);

                EditorGUILayout.EndHorizontal();
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                int column = 0;
                int newSelection = currentSelection;

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
                        if (GUILayout.Button(" ",
                            GUILayout.MinHeight(maxButtonSize),
                            GUILayout.MaxHeight(maxButtonSize),
                            GUILayout.MaxWidth(maxButtonSize)))
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

                if (newSelection >= 0)
                {
                    foundChar = true;
                    if (newSelection != currentSelection)
                    {
                        newChar = charIcons[newSelection].Character;
                    }
                }
            }

            return newChar > 0;
        }

        /// <summary>
        /// Draws a selectable grid of sprite icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawSpriteIconSelector(Sprite currentSprite, out bool foundSprite, out Sprite newSprite, int indentLevel = 0)
        {
            newSprite = null;
            int currentSelection = -1;
            foundSprite = false;

            for (int i = 0; i < spriteIcons.Length; i++)
            {
                if (spriteIcons[i] == currentSprite)
                {
                    currentSelection = i;
                    break;
                }
            }

            if (currentSelection >= 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Current Icon Name", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(spriteIcons[currentSelection].name);

                EditorGUILayout.EndHorizontal();
            }

            if (spriteIconTextures == null || spriteIconTextures.Length != spriteIcons.Length)
            {
                UpdateSpriteIconTextures();
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                float height = maxButtonSize * ((float)spriteIcons.Length / maxButtonsPerColumn);
                var maxHeight = GUILayout.MaxHeight(height);

                bool allPreviewsLoaded;
                var gridContent = GenerateGridContent(spriteIconTextures, out allPreviewsLoaded);

                if (!allPreviewsLoaded)
                {
                    EditorGUILayout.HelpBox(missingPreviewImagesMessage, MessageType.Warning);
                }
#if UNITY_2019_3_OR_NEWER
                int newSelection = GUILayout.SelectionGrid(currentSelection, gridContent, maxButtonsPerColumn, maxHeight);
#else
                var maxWidth = GUILayout.MaxWidth(maxButtonSize * maxButtonsPerColumn);
                int newSelection = GUILayout.SelectionGrid(currentSelection, spriteIconTextures, maxButtonsPerColumn, maxHeight, maxWidth);
#endif
                if (newSelection >= 0)
                {
                    foundSprite = true;
                    if (newSelection != currentSelection)
                    {
                        newSprite = spriteIcons[newSelection];
                    }
                }
            }

            return newSprite != null;
        }

        /// <summary>
        /// Updates the cached sprite icon textures to the latest textures in spriteIcons
        /// </summary>
        public void UpdateSpriteIconTextures()
        {
            if (spriteIcons != null)
            {
                spriteIconTextures = new Texture[spriteIcons.Length];
                for (int i = 0; i < spriteIcons.Length; i++)
                {
                    try
                    {
                        spriteIconTextures[i] = GetTextureFromSprite(spriteIcons[i]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("There was an issue processing the Sprite Icons of this IconSet. Ensure that your IconSet is configured properly");
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// Draws a selectable grid of texture icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawQuadIconSelector(Texture currentTexture, out bool foundTexture, out Texture newTexture, int indentLevel = 0)
        {
            newTexture = null;
            int currentSelection = -1;
            foundTexture = false;

            for (int i = 0; i < quadIcons.Length; i++)
            {
                if (quadIcons[i] == currentTexture)
                {
                    currentSelection = i;
                    break;
                }
            }

            if (currentSelection >= 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Current Icon Name", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(quadIcons[currentSelection].name);

                EditorGUILayout.EndHorizontal();
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                float height = maxButtonSize * ((float)quadIcons.Length / maxButtonsPerColumn);
                var maxHeight = GUILayout.MaxHeight(height);

                bool allPreviewsLoaded;
                var gridContent = GenerateGridContent(quadIcons, out allPreviewsLoaded);

                if (!allPreviewsLoaded)
                {
                    EditorGUILayout.HelpBox(missingPreviewImagesMessage, MessageType.Warning);
                }
#if UNITY_2019_3_OR_NEWER
                int newSelection = GUILayout.SelectionGrid(currentSelection, gridContent, maxButtonsPerColumn, maxHeight);
#else
                var maxWidth = GUILayout.MaxWidth(maxButtonSize * maxButtonsPerColumn);
                int newSelection = GUILayout.SelectionGrid(currentSelection, quadIcons, maxButtonsPerColumn, maxHeight, maxWidth);
#endif
                if (newSelection >= 0)
                {
                    foundTexture = true;
                    if (newSelection != currentSelection)
                    {
                        newTexture = quadIcons[newSelection];
                    }
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
            try
            {
                float iconSizeMultiplier = 0.625f;

                // Get a reference to the Glyph Table
                int glyphIndex = (int)character.glyphIndex;
                int elementIndex = fontAsset.glyphTable.FindIndex(item => item.index == glyphIndex);

                // Return if we can't find the glyph
                if (elementIndex == -1)
                    return;

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
            catch (Exception)
            {
                EditorGUILayout.LabelField("Couldn't draw character icon. Character may not be available in the font asset.");
            }
        }

        /// <summary>
        /// Adds a custom quad icon to the quadIcons array. If quad icon already exists in set no action will be taken.
        /// </summary>
        public bool EditorAddCustomQuadIcon(Texture customQuadIcon)
        {
            foreach (Texture quadIcon in quadIcons)
            {
                if (quadIcon == customQuadIcon)
                {   // Already exists!
                    return false;
                }
            }

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty quadIconProp = serializedObject.FindProperty("quadIcons");
            quadIconProp.InsertArrayElementAtIndex(0);
            SerializedProperty quadIconElement = quadIconProp.GetArrayElementAtIndex(0);
            quadIconElement.objectReferenceValue = customQuadIcon;
            serializedObject.ApplyModifiedProperties();

            EditorResetCharIconLookups();
            InitializeLookups();
            return true;
        }

        public GUIContent[] GenerateGridContent(Texture[] previewTextures, out bool allPreviewsLoaded)
        {
            GUIContent[] gridContent = new GUIContent[previewTextures.Length];
            allPreviewsLoaded = true;

            for (int i = 0; i < previewTextures.Length; i++)
            {
                if (previewTextures[i] != null)
                {
                    gridContent[i] = new GUIContent(previewTextures[i]);
                }
                else
                {
                    gridContent[i] = new GUIContent("N/A");
                    allPreviewsLoaded = false;
                }
            }

            return gridContent;
        }

        [CustomEditor(typeof(ButtonIconSet))]
        private class ButtonIconSetInspector : UnityEditor.Editor
        {
            private const string ShowQuadIconsFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowQuadFoldout";
            private const string ShowSpriteIconsFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowSpriteFoldout";
            private const string ShowCharIconsFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowIconFoldout";
            private const string AvailableIconsFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowAvailableIcons";
            private const string SelectedIconsFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowSelectedIcons";

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

                bool showQuadIconFoldout = SessionState.GetBool(ShowQuadIconsFoldoutKey, false);
                bool showSpriteIconFoldout = SessionState.GetBool(ShowSpriteIconsFoldoutKey, false);
                bool showCharIconFoldout = SessionState.GetBool(ShowCharIconsFoldoutKey, false);
                bool showAvailableIcons = SessionState.GetBool(AvailableIconsFoldoutKey, true);
                bool showSelectedIcons = SessionState.GetBool(SelectedIconsFoldoutKey, true);

#if UNITY_2019_3_OR_NEWER
                showQuadIconFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showQuadIconFoldout, "Quad Icons");
                EditorGUILayout.EndFoldoutHeaderGroup();
#else
                showQuadIconFoldout = EditorGUILayout.Foldout(showQuadIconFoldout, "Quad Icons");
#endif
                if (showQuadIconFoldout)
                {
                    using (new EditorGUI.IndentLevelScope(1))
                    {
                        quadIconsProp.isExpanded = true;
                        EditorGUILayout.PropertyField(quadIconsProp, true);
                    }
                }

#if UNITY_2019_3_OR_NEWER
                showSpriteIconFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showSpriteIconFoldout, "Sprite Icons");
                EditorGUILayout.EndFoldoutHeaderGroup();
#else
                showSpriteIconFoldout = EditorGUILayout.Foldout(showSpriteIconFoldout, "Sprite Icons");
#endif
                if (showSpriteIconFoldout)
                {
                    using (new EditorGUI.IndentLevelScope(1))
                    {
                        spriteIconsProp.isExpanded = true;

                        // Check if the sprite Icons were updated
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(spriteIconsProp, true);
                        // End the code block and update the label if a change occurred
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            bis.UpdateSpriteIconTextures();
                        }
                    }
                }

#if UNITY_2019_3_OR_NEWER
                showCharIconFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(showCharIconFoldout, "Font Icons");
                EditorGUILayout.EndFoldoutHeaderGroup();
#else
                showCharIconFoldout = EditorGUILayout.Foldout(showCharIconFoldout, "Font Icons");
#endif

                if (showCharIconFoldout)
                {
                    EditorGUILayout.PropertyField(charIconFontProp);

                    if (charIconFontProp.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(noIconFontMessage, MessageType.Warning);
                        if (!CheckIfHololensIconFontExists())
                        {
                            EditorGUILayout.HelpBox(downloadIconFontMessage, MessageType.Info);
                            if (GUILayout.Button("View Button Documentation"))
                            {
                                EditorApplication.ExecuteMenuItem(textMeshProMenuItem);
                                Application.OpenURL(hololensIconFontUrl);
                            }
                        }
                    }
                    else
                    {
                        TMP_FontAsset fontAsset = (TMP_FontAsset)charIconFontProp.objectReferenceValue;

#if UNITY_2019_3_OR_NEWER
                        showAvailableIcons = EditorGUILayout.BeginFoldoutHeaderGroup(showAvailableIcons, "Available Icons");
#else
                        showAvailableIcons = EditorGUILayout.Foldout(showAvailableIcons, "Available Icons");
#endif

                        if (showAvailableIcons)
                        {
                            if (fontAsset.characterTable.Count == 0)
                            {
                                EditorGUILayout.HelpBox("No icons are available in this font. The font may be configured incorrectly.", MessageType.Warning);
                                if (GUILayout.Button("Open Font Editor"))
                                {
                                    Selection.activeObject = bis.CharIconFont;
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Click an icon to add it to your selected icons.", MessageType.Info);
                                if (GUILayout.Button("Open Font Editor"))
                                {
                                    Selection.activeObject = bis.CharIconFont;
                                }

                                int removeIndex = -1;
                                int addIndex = -1;
                                int column = 0;

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
                                    if (GUILayout.Button(" ",
                                        GUILayout.MinHeight(maxButtonSize),
                                        GUILayout.MaxHeight(maxButtonSize),
                                        GUILayout.MaxWidth(maxButtonSize)))
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
                                        charIconsProp.InsertArrayElementAtIndex(charIconsProp.arraySize);

                                        SerializedProperty newIconProp = charIconsProp.GetArrayElementAtIndex(charIconsProp.arraySize - 1);
                                        SerializedProperty charProp = newIconProp.FindPropertyRelative("Character");
                                        SerializedProperty nameProp = newIconProp.FindPropertyRelative("Name");

                                        charProp.intValue = (int)unicode;
                                        nameProp.stringValue = "Icon " + charIconsProp.arraySize.ToString();

                                        serializedObject.ApplyModifiedProperties();
                                    }
                                }
                            }
                            EditorGUILayout.Space();
                        }
#if UNITY_2019_3_OR_NEWER
                        EditorGUILayout.EndFoldoutHeaderGroup();
                        showSelectedIcons = EditorGUILayout.BeginFoldoutHeaderGroup(showSelectedIcons, "Selected Icons");
#else
                        showSelectedIcons = EditorGUILayout.Foldout(showSelectedIcons, "Selected Icons");
#endif

                        if (showSelectedIcons)
                        {
                            int removeIndex = -1;

                            if (charIconsProp.arraySize > 0)
                            {
                                EditorGUILayout.HelpBox("These icons will appear in the button config helper inspector. Click an icon to remove it from this list.", MessageType.Info);

                                using (new EditorGUILayout.VerticalScope())
                                {
                                    for (int i = 0; i < charIconsProp.arraySize; i++)
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
                                EditorGUILayout.HelpBox("No icons added yet. Click available icons to add.", MessageType.Info);
                            }

                            if (removeIndex >= 0)
                            {
                                charIconsProp.DeleteArrayElementAtIndex(removeIndex);
                            }
                        }

#if UNITY_2019_3_OR_NEWER
                        EditorGUILayout.EndFoldoutHeaderGroup();
#endif
                    }
                }

                SessionState.SetBool(ShowQuadIconsFoldoutKey, showQuadIconFoldout);
                SessionState.SetBool(ShowSpriteIconsFoldoutKey, showSpriteIconFoldout);
                SessionState.SetBool(ShowCharIconsFoldoutKey, showCharIconFoldout);
                SessionState.SetBool(AvailableIconsFoldoutKey, showAvailableIcons);
                SessionState.SetBool(SelectedIconsFoldoutKey, showSelectedIcons);

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

        Texture2D GetTextureFromSprite(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null)
            {
                return null;
            }

            var rect = sprite.rect;
            var tex = new Texture2D((int)rect.width, (int)rect.height);
            var data = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            tex.SetPixels(data);
            tex.Apply(true);
            return tex;
        }
#endif
    }
}

using TMPro;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.TextCore;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "IconSet", menuName = "Mixed Reality Toolkit/IconSet")]
    public class ButtonIconSet : ScriptableObject
    {
        public Texture2D[] QuadIcons => quadIcons;

        public Sprite[] SpriteIcons => spriteIcons;

        public uint[] CharIcons => charIconsSet;

        public TMP_FontAsset CharIconFont => charIconFont;

        [SerializeField]
        private Texture2D[] quadIcons = null;
        [SerializeField]
        private Sprite[] spriteIcons = null;
        [SerializeField]
        private TMP_FontAsset charIconFont = null;
        [SerializeField]
        private uint[] charIconsSet = new uint[]
        {
            ConvertCharStringToUInt32("\uEBD2"),
            ConvertCharStringToUInt32("\uE711"),
            ConvertCharStringToUInt32("\uE8FB"),
            ConvertCharStringToUInt32("\uE76C"),
            ConvertCharStringToUInt32("\uE712"),
            ConvertCharStringToUInt32("\uE840")
        };
        [SerializeField]

#if UNITY_EDITOR
        private const int maxButtonSize = 75;
        private const int charIconFontSize = 40;
        private const int maxButtonsPerColumn = 6;
        private Texture[] spriteIconTextures = null;
        private static Material fontRenderMat;

        private const string noIconFontMessage = "No icon font selected. Icon fonts will be unavailable.";
        private const string downloadIconFontMessage = "You can download the Segoe MDL2 icon font by clicking the button below.";
        private const string hololensIconFontUrl = "https://aka.ms/SegoeFonts";
        private const string mdl2IconFontName = "SegMDL2";
        private const string textMeshProMenuItem = "Window/TextMeshPro/Font Asset Creator";

        public bool DrawCharIconSelector(uint currentChar, out uint newChar, int indentLevel = 0)
        {
            newChar = 0;

            if (charIconFont == null)
            {
                EditorGUILayout.HelpBox("No icon font has been set in the icon set.", MessageType.Warning);
                return false;
            }

            int currentSelection = -1;
            for (int i = 0; i < charIconsSet.Length; i++)
            {
                if (charIconsSet[i] == currentChar)
                {
                    currentSelection = i;
                    break;
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                int column = 0;
                int newSelection = -1;

                if (charIconsSet.Length > 0)
                {
                    column = 0;
                    EditorGUILayout.BeginHorizontal();
                    for (int i = 0; i < charIconsSet.Length; i++)
                    {
                        if (column >= maxButtonsPerColumn)
                        {
                            column = 0;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }
                        if (GUILayout.Button(" ", GUILayout.MinHeight(charIconFontSize), GUILayout.MaxHeight(charIconFontSize)))
                        {
                            newSelection = i;
                        }
                        Rect textureRect = GUILayoutUtility.GetLastRect();
                        DrawTMPGlyph(textureRect, charIconsSet[i], charIconFont);
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
                    newChar = charIconsSet[newSelection];
                }
            }

            return newChar > 0;
        }

        public bool DrawSpriteIconSelector(Sprite currentSprite, out Sprite newSprite, int indentLevel = 0)
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

        public bool DrawQuadIconSelector(Texture currentTexture, out Texture newTexture, int indentLevel = 0)
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

        public static string ConvertUInt32ToUnicodeCharString(uint unicode)
        {
            byte[] bytes = System.BitConverter.GetBytes(unicode);
            return Encoding.Unicode.GetString(bytes);
        }

        private static void DrawTMPGlyph(Rect position, uint unicode, TMP_FontAsset fontAsset)
        {
            TMP_Character character;
            if (fontAsset.characterLookupTable.TryGetValue(unicode, out character))
            {
                DrawTMPGlyph(position, fontAsset, character);
            }
        }

        private static void DrawTMPGlyph(Rect position, TMP_FontAsset fontAsset, TMP_Character character)
        {
            float iconSizeMultiplier = 0.125f;

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
            float scale = glyphDrawPosition.width / normalizedHeight * iconSizeMultiplier;

            // Compute the normalized texture coordinates
            Rect texCoords = new Rect((float)glyphOriginX / atlasTexture.width, (float)glyphOriginY / atlasTexture.height, (float)glyphWidth / atlasTexture.width, (float)glyphHeight / atlasTexture.height);

            if (Event.current.type == EventType.Repaint)
            {
                glyphDrawPosition.x += (glyphDrawPosition.width - glyphWidth * scale) / 2;
                glyphDrawPosition.y += (glyphDrawPosition.height - glyphHeight * scale) / 2;
                glyphDrawPosition.width = glyphWidth * scale;
                glyphDrawPosition.height = glyphHeight * scale;

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

            private void OnEnable()
            {
                quadIconsProp = serializedObject.FindProperty("quadIcons");
                spriteIconsProp = serializedObject.FindProperty("spriteIcons");
                charIconFontProp = serializedObject.FindProperty("charIconFont");
            }

            public override void OnInspectorGUI()
            {
                ButtonIconSet bis = (ButtonIconSet)target;

                EditorGUILayout.PropertyField(quadIconsProp, true);
                EditorGUILayout.PropertyField(spriteIconsProp, true);
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
                    bool showIconFoldout = SessionState.GetBool(ShowIconFoldoutKey, false);
                    showIconFoldout = EditorGUILayout.Foldout(showIconFoldout, "Choose Font Icons", true);

                    if (showIconFoldout)
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            TMP_FontAsset fontAsset = (TMP_FontAsset)charIconFontProp.objectReferenceValue;

                            int removeIndex = -1;
                            int addIndex = -1;
                            int column = 0;

                            if (bis.charIconsSet.Length > 0)
                            {
                                EditorGUILayout.LabelField("Click to remove from set");
                                column = 0;
                                EditorGUILayout.BeginHorizontal();
                                for (int i = 0; i < bis.charIconsSet.Length; i++)
                                {
                                    if (column >= maxButtonsPerColumn)
                                    {
                                        column = 0;
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                    }
                                    if (GUILayout.Button(" ", GUILayout.MinHeight(charIconFontSize), GUILayout.MaxHeight(charIconFontSize)))
                                    {
                                        removeIndex = i;
                                    }
                                    Rect textureRect = GUILayoutUtility.GetLastRect();
                                    DrawTMPGlyph(textureRect, bis.charIconsSet[i], fontAsset);
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

                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Click to add to set");
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
                                if (GUILayout.Button(" ", GUILayout.MinHeight(charIconFontSize), GUILayout.MaxHeight(charIconFontSize)))
                                {
                                    addIndex = i;
                                }
                                Rect textureRect = GUILayoutUtility.GetLastRect();
                                DrawTMPGlyph(textureRect, fontAsset, fontAsset.characterTable[i]);
                                column++;
                            }

                            if (column > 0)
                            {
                                EditorGUILayout.EndHorizontal();
                            }

                            if (removeIndex >= 0)
                            {
                                List<uint> charIconsSet = new List<uint>(bis.charIconsSet);
                                charIconsSet.RemoveAt(removeIndex);
                                charIconsSet.Sort(delegate (uint char1, uint char2) { return char1.CompareTo(char2); });
                                bis.charIconsSet = charIconsSet.ToArray();
                                EditorUtility.SetDirty(target);
                            }

                            if (addIndex >= 0)
                            {
                                List<uint> charIconsSet = new List<uint>(bis.charIconsSet);
                                uint unicode = fontAsset.characterTable[addIndex].unicode;
                                if (!charIconsSet.Contains(unicode))
                                {
                                    charIconsSet.Add(unicode);
                                    charIconsSet.Sort(delegate (uint char1, uint char2) { return char1.CompareTo(char2); });
                                    bis.charIconsSet = charIconsSet.ToArray();
                                    EditorUtility.SetDirty(target);
                                }
                            }

                            if (GUILayout.Button("Open Font Editor"))
                            {
                                Selection.activeObject = bis.CharIconFont;
                            }
                        }
                    }

                    SessionState.SetBool(ShowIconFoldoutKey, showIconFoldout);
                }

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
using TMPro;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
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

        public string[] CharIcons => charIconsSet;

        public TMP_FontAsset CharIconFont => charIconFont;

        [SerializeField]
        private Texture2D[] quadIcons = null;
        [SerializeField]
        private Sprite[] spriteIcons = null;
        [SerializeField]
        private TMP_FontAsset charIconFont = null;
        [SerializeField]
        private string[] charIconsSet = new string[]
        {
            "\uEBD2","\uE711", "\uE8FB", "\uE76C",
            "\uE712", "\uE840"
        };

#if UNITY_EDITOR
        private const int maxButtonSize = 75;
        private const int charIconFontSize = 40;
        private const int maxButtonsPerColumn = 6;
        private Texture[] spriteIconTextures = null;
        private GUIContent[] charIconContent = null;
        private GUIStyle charIconStyle = null;

        public bool DrawCharIconSelector(string currentChar, out string newChar, int indentLevel = 0)
        {
            newChar = null;

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

            if (charIconStyle == null || charIconStyle.font != charIconFont)
            {
                charIconStyle = new GUIStyle("button");
                charIconStyle.font = charIconFont.sourceFontFile;
                charIconStyle.fontSize = charIconFontSize;
            }

            charIconContent = null;

            if (charIconContent == null || charIconContent.Length != charIconsSet.Length)
            {
                charIconContent = new GUIContent[charIconsSet.Length];
                for (int i = 0; i < charIconsSet.Length; i++)
                {
                    charIconContent[i] = new GUIContent(charIconsSet[i]);
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                float height = maxButtonSize * ((float)charIconsSet.Length / maxButtonsPerColumn);
                var maxHeight = GUILayout.MaxHeight(height);
                int newSelection = GUILayout.SelectionGrid(currentSelection, charIconContent, maxButtonsPerColumn, charIconStyle, maxHeight);
                if (newSelection >= 0 && newSelection != currentSelection)
                {
                    newChar = charIconsSet[newSelection];
                }
            }            

            return !string.IsNullOrEmpty(newChar);
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

        [CustomEditor(typeof(ButtonIconSet))]
        private class ButtonIconSetInspector : UnityEditor.Editor
        {
            private const string ShowIconFoldoutKey = "MixedRealityToolkit.ButtonIconSet.ShowIconFoldout";

            private SerializedProperty quadIconsProp = null;
            private SerializedProperty spriteIconsProp = null;
            private SerializedProperty charIconFontProp = null;

            private GUIContent[] fontGlyphContent = null;
            private GUIContent[] charIconContent = null;
            private string[] fontGlyphUnicode = null;
            private GUIStyle fontGlyphStyle = null;
            private Object lastShownFont = null;
            
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
                    EditorGUILayout.HelpBox("No icon font selected. Icon fonts will be unavailable.", MessageType.Warning);
                }
                else
                {
                    bool showIconFoldout = SessionState.GetBool(ShowIconFoldoutKey, false);
                    showIconFoldout = EditorGUILayout.Foldout(showIconFoldout, "Choose Font Icons", true);

                    if (showIconFoldout)
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {
                            if (fontGlyphContent == null || charIconFontProp.objectReferenceValue != lastShownFont)
                            {
                                lastShownFont = charIconFontProp.objectReferenceValue;
                                TMP_FontAsset fontAsset = (TMP_FontAsset)charIconFontProp.objectReferenceValue;

                                fontGlyphContent = new GUIContent[fontAsset.characterTable.Count];
                                fontGlyphUnicode = new string[fontAsset.characterTable.Count];

                                for (int i = 0; i < fontAsset.characterTable.Count; i++)
                                {
                                    byte[] unicode = System.BitConverter.GetBytes(fontAsset.characterTable[i].unicode);
                                    fontGlyphUnicode[i] = Encoding.Unicode.GetString(unicode);
                                    fontGlyphContent[i] = new GUIContent(fontGlyphUnicode[i]);
                                }

                                fontGlyphStyle = new GUIStyle("button");
                                fontGlyphStyle.font = fontAsset.sourceFontFile;
                                fontGlyphStyle.fontSize = 20;
                            }

                            if (charIconContent == null || charIconContent.Length != bis.charIconsSet.Length)
                            {
                                charIconContent = new GUIContent[bis.charIconsSet.Length];
                                for (int i = 0; i < bis.charIconsSet.Length; i++)
                                {
                                    charIconContent[i] = new GUIContent(bis.charIconsSet[i]);
                                }
                            }

                            EditorGUILayout.LabelField("Click to remove from set");
                            int removeIndex = GUILayout.SelectionGrid(-1, charIconContent, maxButtonsPerColumn, fontGlyphStyle);
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Click to add to set");
                            int addIndex = GUILayout.SelectionGrid(-1, fontGlyphContent, maxButtonsPerColumn, fontGlyphStyle);

                            if (removeIndex >= 0)
                            {
                                List<string> charIconsSet = new List<string>(bis.charIconsSet);
                                charIconsSet.RemoveAt(removeIndex);
                                charIconsSet.Sort(delegate (string char1, string char2) { return char1.CompareTo(char2); });
                                bis.charIconsSet = charIconsSet.ToArray();
                                EditorUtility.SetDirty(target);
                            }

                            if (addIndex >= 0)
                            {
                                List<string> charIconsSet = new List<string>(bis.charIconsSet);
                                if (!charIconsSet.Contains(fontGlyphUnicode[addIndex]))
                                {
                                    charIconsSet.Add(fontGlyphUnicode[addIndex]);
                                    charIconsSet.Sort(delegate (string char1, string char2) { return char1.CompareTo(char2); });
                                    bis.charIconsSet = charIconsSet.ToArray();
                                    EditorUtility.SetDirty(target);
                                }
                            }
                        }
                    }

                    SessionState.SetBool(ShowIconFoldoutKey, showIconFoldout);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "IconSet", menuName = "Mixed Reality Toolkit/IconSet")]
    public class ButtonIconSet : ScriptableObject
    {
        public Texture2D[] QuadIcons => quadIcons;

        public Sprite[] SpriteIcons => spriteIcons;

        public string[] CharIcons => charIcons;

        public Font CharIconFont => charIconFont;

        [SerializeField]
        private Texture2D[] quadIcons = null;
        [SerializeField]
        private Sprite[] spriteIcons = null;
        [SerializeField]
        private Font charIconFont = null;
        [SerializeField]
        private string[] charIcons = null;

#if UNITY_EDITOR
        private const int maxButtonSize = 50;
        private const int maxButtonsPerColumn = 6;
        private Texture[] spriteIconTextures = null;
        private GUIContent[] charIconContent = null;

        public bool DrawCharIconSelector(string currentChar, out string newChar, int indentLevel = 0)
        {
            newChar = null;

            int currentSelection = -1;
            for (int i = 0; i < charIcons.Length; i++)
            {
                if (charIcons[i] == currentChar)
                {
                    currentSelection = i;
                    break;
                }
            }

            if (charIconContent == null || charIconContent.Length != charIcons.Length)
            {
                charIconContent = new GUIContent[charIcons.Length];
                for (int i = 0; i < charIcons.Length; i++)
                {
                    charIconContent[i] = new GUIContent(charIcons[i]);
                }
            }

            using (new EditorGUI.IndentLevelScope(indentLevel))
            {
                var maxHeight = GUILayout.MaxHeight(maxButtonSize * (charIcons.Length / maxButtonsPerColumn));
                int newSelection = GUILayout.SelectionGrid(currentSelection, charIconContent, maxButtonsPerColumn, maxHeight);
                if (newSelection >= 0)
                {
                    newChar = charIcons[newSelection];
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
                var maxHeight = GUILayout.MaxHeight(maxButtonSize * (spriteIcons.Length / maxButtonsPerColumn));
                int newSelection = GUILayout.SelectionGrid(currentSelection, spriteIconTextures, maxButtonsPerColumn, maxHeight);
                if (newSelection >= 0)
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
                var maxHeight = GUILayout.MaxHeight(maxButtonSize * (quadIcons.Length / maxButtonsPerColumn));
                int newSelection = GUILayout.SelectionGrid(currentSelection, quadIcons, maxButtonsPerColumn, maxHeight);
                if (newSelection >= 0)
                {
                    newTexture = quadIcons[newSelection];
                }
            }

            return newTexture != null;
        }
#endif
    }
}
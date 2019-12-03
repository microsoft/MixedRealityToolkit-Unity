using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Helper component that gathers the most commonly modified button elemtents in one place.
    /// </summary>
    [ExecuteAlways]
    public partial class ButtonConfigHelper : MonoBehaviour
    {
        private readonly static uint defaultIconChar = ButtonIconSet.ConvertCharStringToUInt32("\uE700");
        private const string defaultIconTextureNameID = "_MainTex";

        /// <summary>
        /// Modifies the main label text.
        /// </summary>
        public string MainLabelText
        {
            get { return mainLabelText.text; }
            set { mainLabelText.text = value; }
        }

        /// <summary>
        /// Modifies the 'See it / Say it' label text.
        /// </summary>
        public string SeeItSayItLabelText
        {
            get { return seeItSatItLabelText.text; }
            set { seeItSatItLabelText.text = value; }
        }

        /// <summary>
        /// Returns the Interactable's OnClick event.
        /// </summary>
        public UnityEvent OnClick => interactable?.OnClick;

        /// <summary>
        /// Modifies the button's icon rendering style.
        /// </summary>
        public ButtonIconStyle IconStyle
        {
            get { return iconStyle; }
            set
            {
                if (iconStyle != value)
                {
                    SetIconStyle(value);
                }
            }
        }

        [SerializeField]
        private TextMeshPro mainLabelText = null;
        [SerializeField]
        private Interactable interactable = null;
        #pragma warning disable // iconSet is only used by the inspector
        [SerializeField]
        private GameObject seeItSayItLabel = null;
        #pragma warning restore
        [SerializeField]
        private TextMeshPro seeItSatItLabelText = null;
        [SerializeField, Tooltip("How the button icon should be rendered.")]
        private ButtonIconStyle iconStyle = ButtonIconStyle.Quad;
        #pragma warning disable // iconSet is only used by the inspector
        [SerializeField]
        private ButtonIconSet iconSet = null;
        #pragma warning restore

        [SerializeField]
        private TextMeshPro iconCharLabel = null;
        [SerializeField]
        private TMP_FontAsset iconCharFont = null;
        [SerializeField]
        private uint iconChar = defaultIconChar;

        [SerializeField]
        private SpriteRenderer iconSpriteRenderer = null;
        [SerializeField]
        private Sprite iconSprite = null;

        [SerializeField]
        private MeshRenderer iconQuadRenderer = null;
        [SerializeField]
        private string iconQuadTextureNameID = defaultIconTextureNameID;
        [SerializeField]
        private Texture iconQuadTexture = null;

        private MaterialPropertyBlock iconTexturePropertyBlock;

        /// <summary>
        /// Sets the character for the button. This automatically sets the bitton icon style to Char.
        /// </summary>
        /// <param name="newIconChar">Unicode string for new icon character.</param>
        /// <param name="newIconCharFont">Optional font asset. If null, the existing font asset will be used.</param>
        public void SetCharIcon(uint newIconChar, TMP_FontAsset newIconCharFont = null)
        {
            if (newIconChar <= 0)
            {
                return;
            }

            if (newIconCharFont != null && newIconCharFont != iconCharFont)
            {
                iconCharFont = newIconCharFont;
            }

            if (iconCharLabel == null)
            {
                Debug.LogWarning("No icon char label in " + name + " - not setting custom icon char.");
                return;
            }

            iconChar = newIconChar;
            if (iconCharFont != null)
            {
                iconCharLabel.font = iconCharFont;
            }

            uint labelChar = ButtonIconSet.ConvertCharStringToUInt32(iconCharLabel.text);
            if (labelChar != iconChar || iconCharLabel.font != iconCharFont)
            {
                iconCharLabel.text = ButtonIconSet.ConvertUInt32ToUnicodeCharString(newIconChar);
            }

            SetIconStyle(ButtonIconStyle.Char);
        }

        /// <summary>
        /// Sets the sprite for the button. This automatically sets the button icon style to Sprite.
        /// </summary>
        public void SetSpriteIcon(Sprite newIconSprite)
        {
            if (newIconSprite == null)
            {
                return;
            }

            if (iconSpriteRenderer == null)
            {
                Debug.LogWarning("No icon sprite renderer in " + name + " - not setting custom icon sprite.");
                return;
            }

            iconSprite = newIconSprite;

            if (iconSpriteRenderer.sprite != iconSprite)
            {
                iconSpriteRenderer.sprite = newIconSprite;
            }

            SetIconStyle(ButtonIconStyle.Sprite);
        }

        /// <summary>
        /// Sets the quad texture for the button. This automatically sets the button icon style to Quad.
        /// </summary>
        public void SetQuadIcon(Texture newIconTexture)
        {
            if (newIconTexture == null)
            {
                return;
            }

            if (iconQuadRenderer == null)
            {
                Debug.LogWarning("No icon quad renderer in " + name + " - not setting custom icon texture.");
                return;
            }

            iconQuadTexture = newIconTexture;

            if (iconTexturePropertyBlock == null)
            {
                iconTexturePropertyBlock = new MaterialPropertyBlock();
            }

            iconQuadRenderer.GetPropertyBlock(iconTexturePropertyBlock);
            iconTexturePropertyBlock.SetTexture(iconQuadTextureNameID, newIconTexture);
            iconQuadRenderer.SetPropertyBlock(iconTexturePropertyBlock);

            SetIconStyle(ButtonIconStyle.Quad);
        }

        /// <summary>
        /// Sets the icon style for the button. Relevant components will be turned on / off based on style.
        /// </summary>
        private void SetIconStyle(ButtonIconStyle newStyle)
        {
            iconStyle = newStyle;
            switch (iconStyle)
            {
                case ButtonIconStyle.Char:
                    iconCharLabel?.gameObject.SetActive(true);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;

                case ButtonIconStyle.Sprite:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(true);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;

                case ButtonIconStyle.Quad:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(true);
                    break;

                case ButtonIconStyle.None:
                    iconCharLabel?.gameObject.SetActive(false);
                    iconSpriteRenderer?.gameObject.SetActive(false);
                    iconQuadRenderer?.gameObject.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// Forces the config helper to apply its internal settings.
        /// </summary>
        public void ForceRefresh()
        {
            switch (iconStyle)
            {
                case ButtonIconStyle.Quad:
                    SetQuadIcon(iconQuadTexture);
                    break;

                case ButtonIconStyle.Char:
                    SetCharIcon(iconChar, iconCharFont);
                    break;

                case ButtonIconStyle.Sprite:
                    SetSpriteIcon(iconSprite);
                    break;

                case ButtonIconStyle.None:
                    SetIconStyle(ButtonIconStyle.None);
                    break;
            }
        }

        private void OnEnable()
        {
            ForceRefresh();
        }
    }
}
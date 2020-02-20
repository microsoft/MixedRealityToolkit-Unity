// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// <summary>
        /// Modifies the main label text.
        /// </summary>
        public string MainLabelText
        {
            get { return mainLabelText?.text; }
            set
            {
                if (mainLabelText == null)
                {
                    Debug.LogWarning("No main label set in " + name + " - not setting main label text.");
                    return;
                }

                mainLabelText.text = value;
            }
        }

        /// <summary>
        /// Modifies the 'See it / Say it' label text.
        /// </summary>
        public string SeeItSayItLabelText
        {
            get { return seeItSatItLabelText?.text; }
            set
            {
                if (seeItSatItLabelText == null)
                {
                    Debug.LogWarning("No see it / say it label set in " + name + " - not setting see it / say it label text.");
                    return;
                }

                seeItSatItLabelText.text = value;
            }
        }

        /// <summary>
        /// Turns the see it / say it label object on / off.
        /// </summary>
        public bool SeeItSayItLabelEnabled
        {
            get
            {
                if (seeItSayItLabel == null)
                {
                    return false;
                }

                return seeItSayItLabel.activeSelf;
            }
            set
            {
                if (seeItSayItLabel == null)
                {
                    Debug.LogWarning("No see it / say it label set in " + name + " - not setting see it / say it label enabled.");
                    return;
                }

                seeItSayItLabel.SetActive(value);
            }
        }

        /// <summary>
        /// Returns the Interactable's OnClick event.
        /// </summary>
        public UnityEvent OnClick
        {
            get
            {
                if (interactable == null)
                {
                    Debug.LogWarning("No interactable set in " + name + " - returning an empty OnClick event.");
                    return emptyOnClickEvent;
                }

                return interactable.OnClick;
            }
        }

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

        /// <summary>
        /// The button's icon set. Note that setting this will not automatically assign an icon from the new set.
        /// </summary>
        public ButtonIconSet IconSet
        {
            get => iconSet;
            set => iconSet = value;
        }

        private readonly static UnityEvent emptyOnClickEvent = new UnityEvent();
        private readonly static uint defaultIconChar = ButtonIconSet.ConvertCharStringToUInt32("\uEBD2");
        private const string defaultIconTextureNameID = "_MainTex";

        [SerializeField]
        [Tooltip("Optional main label used by the button.")]
        private TextMeshPro mainLabelText = null;
        [SerializeField]
        [Tooltip("Optional interactable component used by the button. Used for its OnClick event.")]
        private Interactable interactable = null;
        [SerializeField]
        [Tooltip("Optional see it / say it object.")]
        private GameObject seeItSayItLabel = null;
        [SerializeField]
        [Tooltip("Optional see it / say it label used by the button. Should be subsumed under the seeItSayItLabel object.")]
        private TextMeshPro seeItSatItLabelText = null;
        [SerializeField, Tooltip("How the button icon should be rendered.")]
        private ButtonIconStyle iconStyle = ButtonIconStyle.Quad;

        [Header("Font Icon")]
        [SerializeField]
        [Tooltip("Optional label used for font icon.")]
        private TextMeshPro iconCharLabel = null;
        [SerializeField]
        [Tooltip("Optional font used for font icon. This will be set by configuration actions using the icon set.")]
        private TMP_FontAsset iconCharFont = null;
        [SerializeField]
        [Tooltip("Optional unicode code for font icon. See Text Mesh Pro font asset for available unicode characters. This will be set by configuration actions.")]
        private uint iconChar = 0;

        [Header("Sprite Icon")]
        [SerializeField]
        [Tooltip("Optional sprite renderer used for sprite icon.")]
        private SpriteRenderer iconSpriteRenderer = null;
        [SerializeField]
        [Tooltip("Optional sprite used for sprite icon. This will be set by configuration actions.")]
        private Sprite iconSprite = null;

        [Header("Quad Icon")]
        [SerializeField]
        [Tooltip("Optional quad renderer used for texture icon.")]
        private MeshRenderer iconQuadRenderer = null;
        [SerializeField]
        [Tooltip("The texture name ID. Set to " + defaultIconTextureNameID + " by default.")]
        private string iconQuadTextureNameID = defaultIconTextureNameID;
        [SerializeField]
        [Tooltip("Optional texture used for texture icon. This will be set by configuration actions.")]
        private Texture iconQuadTexture = null;

        [Header("Icon Set")]
        [SerializeField]
        [Tooltip("Optional icon set used to configure icon objects.")]
        private ButtonIconSet iconSet = null;

        private MaterialPropertyBlock iconTexturePropertyBlock;

        /// <summary>
        /// Searches the icon set for a character matching newIconCharName.
        /// If no icon set is available, or if no texture with that name is found, no action is taken.
        /// </summary>
        /// <param name="newIconCharName">Name of the new icon character as defined in the IconSet.</param>
        public void SetCharIconByName(string newIconCharName)
        {
            if (string.IsNullOrEmpty(newIconCharName))
            {
                Debug.LogError("Icon character name cannot be null.");
                return;
            }

            if (iconSet == null)
            {
                Debug.LogWarning("No icon set in " + name + " - taking no action..");
                return;
            }

            uint charIcon = 0;
            if (!iconSet.TryGetCharIcon(newIconCharName, out charIcon))
            {
                Debug.LogWarning("Couldn't find icon character with name " + newIconCharName + " in " + name + " - taking no action..");
                return;
            }

            SetCharIcon(charIcon);
        }

        /// <summary>
        /// Searches the icon set for a texture matching newIconTextureName.
        /// If no icon set is available, or if no texture with that name is found, no action is taken.
        /// </summary>
        /// <param name="newIconTextureName">Name of the new icon texture asset.</param>
        public void SetQuadIconByName(string newIconTextureName)
        {
            if (string.IsNullOrEmpty(newIconTextureName))
            {
                Debug.LogError("Icon texture name cannot be null.");
                return;
            }

            if (iconSet == null)
            {
                Debug.LogWarning("No icon set in " + name + " - taking no action..");
                return;
            }

            Texture2D quadIcon = null;
            if (!iconSet.TryGetQuadIcon(newIconTextureName, out quadIcon))
            {
                Debug.LogWarning("Couldn't find icon texture with name " + newIconTextureName + " in " + name + " - taking no action..");
                return;
            }

            SetQuadIcon(quadIcon);
        }

        /// <summary>
        /// Searches the icon set for a texture matching newIconSpriteName.
        /// If no icon set is available, or if no texture with that name is found, no action is taken.
        /// </summary>
        /// <param name="newIconTextureName">Name of the new icon texture asset.</param>
        public void SetSpriteIconByName(string newIconSpriteName)
        {
            if (string.IsNullOrEmpty(newIconSpriteName))
            {
                Debug.LogError("Icon sprite name cannot be null.");
                return;
            }

            if (iconSet == null)
            {
                Debug.LogWarning("No icon set in " + name + " - taking no action..");
                return;
            }

            Sprite spriteIcon = null;
            if (!iconSet.TryGetSpriteIcon(newIconSpriteName, out spriteIcon))
            {
                Debug.LogWarning("Couldn't find icon sprite with name " + newIconSpriteName + " in " + name + " - taking no action..");
                return;
            }

            SetSpriteIcon(spriteIcon);
        }

        /// <summary>
        /// Sets the character for the button. This automatically sets the button icon style to Char.
        /// </summary>
        /// <param name="newIconChar">Unicode string for new icon character.</param>
        /// <param name="newIconCharFont">Optional TMPro font asset. If null, the existing font asset will be used.</param>
        public void SetCharIcon(uint newIconChar, UnityEngine.Object newIconCharFont = null)
        {
            if (newIconChar <= 0)
            {
                return;
            }

            if (newIconCharFont != null && newIconCharFont != iconCharFont)
            {
                iconCharFont = (TMP_FontAsset)newIconCharFont;
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
                    if (iconCharLabel != null) { iconCharLabel.gameObject.SetActive(true); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(false); }
                    if (iconQuadRenderer != null) { iconQuadRenderer.gameObject.SetActive(false); }
                    break;

                case ButtonIconStyle.Sprite:
                    if (iconCharLabel != null) { iconCharLabel.gameObject.SetActive(false); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(true); }
                    if (iconQuadRenderer != null) { iconQuadRenderer.gameObject.SetActive(false); }
                    break;

                case ButtonIconStyle.Quad:
                    if (iconCharLabel != null) { iconCharLabel.gameObject.SetActive(false); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(false); }
                    if (iconQuadRenderer != null) { iconQuadRenderer.gameObject.SetActive(true); }
                    break;

                case ButtonIconStyle.None:
                    if (iconCharLabel != null) { iconCharLabel.gameObject.SetActive(false); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(false); }
                    if (iconQuadRenderer != null) { iconQuadRenderer.gameObject.SetActive(false); }
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

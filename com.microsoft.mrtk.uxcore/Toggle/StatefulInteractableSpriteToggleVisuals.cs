// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A visuals script to provide a visual layer on top of a
    /// <see cref="StatefulInteractable"/>.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [AddComponentMenu("MRTK/UX/Stateful Interactable Sprite Toggle Visuals")]
    public class StatefulInteractableSpriteToggleVisuals : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        [Tooltip("The stateful interactable whose toggle state is being visualized")]
        private StatefulInteractable statefulInteractable;

        [SerializeField]
        [Tooltip("The sprite to show when toggled on")]
        private Sprite toggleOnSprite;

        /// <summary>
        /// The sprite to show when toggled on
        /// </summary>
        public Sprite ToggleOnSprite { get => toggleOnSprite; set => toggleOnSprite = value; }

        [SerializeField]
        [Tooltip("The color when toggled on")]
        private Color toggleOnColor;

        /// <summary>
        /// The color when toggled on
        /// </summary>
        public Color ToggleOnColor { get => toggleOnColor; set => toggleOnColor = value; }

        [SerializeField]
        [Tooltip("The sprite to show when toggled off")]
        private Sprite toggleOffSprite;

        /// <summary>
        /// The sprite to show when toggled off
        /// </summary>
        public Sprite ToggleOffSprite { get => toggleOffSprite; set => toggleOffSprite = value; }

        [SerializeField]
        [Tooltip("The color when toggled off")]
        private Color toggleOffColor;

        /// <summary>
        /// The color when toggled off
        /// </summary>
        public Color ToggleOffColor { get => toggleOffColor; set => toggleOffColor = value; }

        /// <summary>
        /// The sprite renderer used to display the toggle state
        /// </summary>
        private SpriteRenderer spriteRenderer;

        #endregion

        #region MonoBehaviours

        public void Awake()
        {
            // If the StatefulInteractable is null, 
            if (statefulInteractable == null)
            {
                statefulInteractable = GetComponent<StatefulInteractable>();
            }

            spriteRenderer = GetComponent<SpriteRenderer>();

            // Initializing the toggle state
            bool isToggled = statefulInteractable.IsToggled;

            spriteRenderer.sprite = isToggled ? toggleOnSprite : toggleOffSprite;
            spriteRenderer.color = isToggled ? toggleOnColor : toggleOffColor;

            lastToggleState = isToggled;
        }

        public void LateUpdate()
        {
            UpdateAllVisuals();
        }
        #endregion

        #region Visuals
        // Used to ensure we only update visuals when the toggle state changes
        private bool lastToggleState;

        private void UpdateAllVisuals()
        {
            bool isToggled = statefulInteractable.IsToggled;

            if (lastToggleState != isToggled)
            {
                spriteRenderer.sprite = isToggled ? toggleOnSprite : toggleOffSprite;
                spriteRenderer.color = isToggled ? toggleOnColor : toggleOffColor;

                lastToggleState = isToggled;
            }
        }


        #endregion
    }
}

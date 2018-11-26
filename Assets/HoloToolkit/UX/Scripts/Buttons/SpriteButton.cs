// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Sprite button is a sprite renderer interactable with state data for button state
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteButton : Button
    {
        /// <summary>
        /// Button State data set for different interaction states
        /// </summary>
        [Header("Sprite Button")]
        [Tooltip("Button State information")]
        public SpriteButtonDatum[] ButtonStates = new SpriteButtonDatum[]{ new SpriteButtonDatum((ButtonStateEnum)0), new SpriteButtonDatum((ButtonStateEnum)1),
            new SpriteButtonDatum((ButtonStateEnum)2), new SpriteButtonDatum((ButtonStateEnum)3),
            new SpriteButtonDatum((ButtonStateEnum)4), new SpriteButtonDatum((ButtonStateEnum)5) };


        private SpriteRenderer _renderer;

        /// <summary>
        /// Callback override function to change sprite, color and scale on button state change
        /// </summary>
        /// <param name="newState">
        /// A <see cref="ButtonStateEnum"/> for the new button state.
        /// </param>
        public override void OnStateChange(ButtonStateEnum newState)
        {
            if (_renderer == null)
                _renderer = this.GetComponent<SpriteRenderer>();

            if (ButtonStates[(int)newState].ButtonSprite != null)
            {
                _renderer.sprite = ButtonStates[(int)newState].ButtonSprite;
                _renderer.color = ButtonStates[(int)newState].SpriteColor;
            }

            if (this.transform.localScale != ButtonStates[(int)newState].Scale)
                this.transform.localScale = ButtonStates[(int)newState].Scale;

            base.OnStateChange(newState);
        }
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// A <see cref="IEffect"> that switches a <see cref="UnityEngine.UI.Image"/> between two sprites.
    /// </summary>
    internal class SpriteSwapEffect : IEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Sprite Swap";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("Threshold value to activate this effect. When the state value is above this number, the effect will activate.")]
        private float activationThreshold = 0.001f;

        [SerializeField]
        [Tooltip("The Image to switch sprites for.")]
        private Image target;

        [SerializeField]
        [Tooltip("The texture to set when the state is active.")]
        private Sprite activeSprite;

        [SerializeField]
        [Tooltip("The texture to set when the state is inactive.")]
        private Sprite inactiveSprite;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteSwapEffect"/> class.
        /// </summary>
        public SpriteSwapEffect() { }

        /// <inheritdoc />
        public void Setup(PlayableGraph graph, GameObject owner) { }

        /// <inheritdoc />
        public bool Evaluate(float parameter)
        {
            if (target == null)
            {
                return false;
            }

            Sprite correctSprite = parameter > activationThreshold ? activeSprite : inactiveSprite;

            if (target.sprite != correctSprite)
            {
                target.sprite = correctSprite;
            }

            return true; // We are always immediately done.
        }
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// A <see cref="IEffect"> that sets a collection of specified GameObjects active or inactive.
    /// </summary>
    internal class SetTargetsActiveEffect : IEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Set Targets Active/Inactive";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("If true, the specified objects will be set inactive when the state is on, and vice versa.")]
        private bool invert;

        [SerializeField]
        [Tooltip("The list of gameobjects to be toggled.")]
        private List<GameObject> targets;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTargetsActiveEffect"/> class.
        /// </summary>
        public SetTargetsActiveEffect() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTargetsActiveEffect"/> class.
        /// </summary>
        /// <param name="targets">The list of gameobjects to be set active/inactive.</param>
        /// <param name="invert">If true, the specified objects will be set inactive when the state is on, and vice versa.</param>
        public SetTargetsActiveEffect(List<GameObject> targets, bool invert = false)
        {
            this.targets = targets;
            this.invert = invert;
        }

        /// <inheritdoc />
        public void Setup(PlayableGraph graph, GameObject owner) { }

        /// <inheritdoc />
        public bool Evaluate(float parameter)
        {
            // Toggle all our toggle targets.
            bool shouldBeActive = (parameter > 0.001f) ^ invert;
            foreach (var target in targets)
            {
                if (target.activeSelf != shouldBeActive)
                {
                    target.SetActive(shouldBeActive);
                }
            }

            return true; // We are always immediately done.
        }
    }
}
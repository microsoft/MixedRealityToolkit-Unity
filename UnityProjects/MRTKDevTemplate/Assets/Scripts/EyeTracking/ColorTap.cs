// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    using UnityEngine.XR.Interaction.Toolkit;

    /// <summary>
    /// Example of how to attach functions to StatefulInteractables.
    /// <seealso cref="StatefulInteractable"/>
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [AddComponentMenu("Scripts/MRTK/Examples/ColorTap")]
    public class ColorTap : MonoBehaviour
    {
        [Tooltip("The default color of the GameObject.")]
        [SerializeField]
        private Color idleStateColor = Color.cyan;

        [Tooltip("The hovered color of the GameObject.")]
        [SerializeField]
        private Color onHoverColor = Color.white;

        [Tooltip("The selected color of the GameObject.")]
        [SerializeField]
        private Color onSelectColor = Color.blue;

        private Material material;

        private void Awake()
        {
            material = GetComponent<Renderer>().material;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> enters eye gaze.
        /// </summary>
        public void OnGazeHoverEntered()
        {
            material.color = onHoverColor;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> leaves eye gaze.
        /// </summary>
        public void OnGazeHoverExited()
        {
            material.color = idleStateColor;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> starts a gaze pinch gesture.
        /// </summary>
        public void OnGazePinchEntered()
        {
            material.color = onHoverColor;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> ends a gaze pinch gesture.
        /// </summary>
        public void OnGazePinchExited()
        {
            material.color = idleStateColor;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> is selected.
        /// </summary>
        public void OnSelectEntered(SelectEnterEventArgs _)
        {
            material.color = onSelectColor;
        }

        /// <summary>
        /// Triggered when the attached <see cref="StatefulInteractable"/> is de-selected.
        /// </summary>
        public void OnSelectExited(SelectExitEventArgs _)
        {
            material.color = onHoverColor;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple visuals script to provide a visual layer on top of a 
    /// <see cref="Microsoft.MixedReality.Toolkit.UX.PressableButton">PressableButton</see>.
    /// </summary>
    [RequireComponent(typeof(PressableButton))]
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Basic Pressable Button Visuals")]
    public class BasicPressableButtonVisuals : MonoBehaviour
    {
        private PressableButton buttonState;

        [SerializeField]
        [Tooltip("The front plate object that will move with the button displacement.")]
        private Transform movingVisuals;

        /// <summary>
        /// The "front plate" object that will move with the button displacement.
        /// </summary>
        public Transform MovingVisuals => movingVisuals;

        private void Awake()
        {
            buttonState = GetComponent<PressableButton>();
        }

        private void LateUpdate()
        {
            UpdateMovingVisualsPosition();
        }

        /// <summary>
        /// Update the local position of the specified <see cref="MovingVisuals"/> transform.
        /// </summary>
        protected virtual void UpdateMovingVisualsPosition()
        {
            if (movingVisuals != null)
            {
                movingVisuals.localPosition = buttonState.PushPlaneLocalPosition();
            }
        }
    }
}

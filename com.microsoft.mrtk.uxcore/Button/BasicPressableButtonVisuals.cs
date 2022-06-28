// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple visuals script to provide a visual layer on top of
    /// <see cref="PressableButton"/>.
    /// </summary>
    [RequireComponent(typeof(PressableButton))]
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Basic Pressable Button Visuals")]
    public class BasicPressableButtonVisuals : MonoBehaviour
    {
        private PressableButton buttonState;

        [SerializeField]
        private Transform movingVisuals;

        /// <summary>
        /// The "front plate" object that will move with the button displacement.
        /// </summary>
        public Transform MovingVisuals => movingVisuals;

        protected void Awake()
        {
            buttonState = GetComponent<PressableButton>();
        }

        protected void LateUpdate()
        {
            UpdateMovingVisualsPosition();
        }

        protected virtual void UpdateMovingVisualsPosition()
        {
            if (movingVisuals != null)
            {
                movingVisuals.localPosition = buttonState.PushPlaneLocalPosition();
            }
        }
    }
}

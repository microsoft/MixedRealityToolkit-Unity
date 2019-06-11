// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// HoloLens 2 shell's style button specific elements
    ///</summary>
    public class PressableButtonHoloLens2 : PressableButton
    {
        [SerializeField]
        [Tooltip("The icon and text content moving inside the button.")]
        private GameObject movingButtonIconText = null;

        /// <summary>
        /// Public property to set the moving content part(icon and text) of the button. 
        /// This content part moves 1/2 distance of the front cage 
        /// </summary>
        public GameObject MovingButtonIconText
        {
            get
            {
                return movingButtonIconText;
            }
            set
            {
                if (movingButtonIconText != value)
                {
                    movingButtonIconText = value;
                }
            }
        }

        protected override void UpdateMovingVisualsPosition()
        {
            base.UpdateMovingVisualsPosition();

            if (movingButtonIconText != null)
            {
                // Always move relative to startPushDistance
                movingButtonIconText.transform.position = GetWorldPositionAlongPushDirection((currentPushDistance - startPushDistance) / 2);
            }
        }
    }
}
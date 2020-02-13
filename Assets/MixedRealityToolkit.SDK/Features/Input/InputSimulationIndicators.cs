// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Input simulation service is only built on editor platforms

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A row of indicator buttons to control input simulation features.
    /// </summary>
    [Serializable]
    [AddComponentMenu("Scripts/MRTK/SDK/InputSimulationIndicators")]
    public class InputSimulationIndicators : MonoBehaviour
    {
        /// <summary>
        /// Component displaying the left hand icon.
        /// </summary>
        public UnityEngine.UI.Image imageHandLeft = null;
        /// <summary>
        /// Component displaying the right hand icon.
        /// </summary>
        public UnityEngine.UI.Image imageHandRight = null;

        /// <summary>
        /// Icon for left hand when under user control.
        /// </summary>
        public Sprite iconHandActiveLeft = null;
        /// <summary>
        /// Icon for right hand when under user control.
        /// </summary>
        public Sprite iconHandActiveRight = null;
        /// <summary>
        /// Icon for left hand when visible but not actively controlled.
        /// </summary>
        public Sprite iconHandIdleLeft = null;
        /// <summary>
        /// Icon for right hand when visible but not actively controlled.
        /// </summary>
        public Sprite iconHandIdleRight = null;
        /// <summary>
        /// Icon for left hand when untracked.
        /// </summary>
        public Sprite iconHandUntrackedLeft = null;
        /// <summary>
        /// Icon for right hand when untracked.
        /// </summary>
        public Sprite iconHandUntrackedRight = null;

#if UNITY_EDITOR

        private IInputSimulationService inputSimService = null;
        private IInputSimulationService InputSimService
        {
            get
            {
                if (inputSimService == null)
                {
                    inputSimService = (CoreServices.InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IInputSimulationService>();
                }
                return inputSimService;
            }
        }

        /// <summary>
        /// Updates the left and right hand images according to the tracked state
        /// </summary>
        private void Update()
        {
            if (imageHandLeft)
            {
                Sprite iconHandLeft;
                if (InputSimService.IsSimulatingHandLeft)
                {
                    iconHandLeft = iconHandActiveLeft;
                }
                else if (InputSimService.HandDataLeft.IsTracked)
                {
                    iconHandLeft = iconHandIdleLeft;
                }
                else
                {
                    iconHandLeft = iconHandUntrackedLeft;
                }

                imageHandLeft.sprite = iconHandLeft;
            }

            if (imageHandRight)
            {
                Sprite iconHandRight;
                if (InputSimService.IsSimulatingHandRight)
                {
                    iconHandRight = iconHandActiveRight;
                }
                else if (InputSimService.HandDataRight.IsTracked)
                {
                    iconHandRight = iconHandIdleRight;
                }
                else
                {
                    iconHandRight = iconHandUntrackedRight;
                }

                imageHandRight.sprite = iconHandRight;
            }
        }

        /// <summary>
        /// Toggle permanent visibility of the left hand.
        /// </summary>
        public void ToggleLeftHand()
        {
            InputSimService.IsAlwaysVisibleHandLeft = !InputSimService.IsAlwaysVisibleHandLeft;
        }

        /// <summary>
        /// Toggle permanent visibility of the right hand.
        /// </summary>
        public void ToggleRightHand()
        {
            InputSimService.IsAlwaysVisibleHandRight = !InputSimService.IsAlwaysVisibleHandRight;
        }

        /// <summary>
        /// Reset the state of the left hand to default.
        /// </summary>
        public void ResetLeftHand()
        {
            InputSimService.ResetHandLeft();
        }

        /// <summary>
        /// Reset the state of the right hand to default.
        /// </summary>
        public void ResetRightHand()
        {
            InputSimService.ResetHandRight();
        }
#endif
    }
}

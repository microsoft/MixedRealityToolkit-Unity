// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A row of indicator buttons to control input simulation features.
    /// </summary>
    [Serializable]
    public class InputSimulationIndicators : MonoBehaviour
    {
        /// <summary>
        /// Icon component for the left hand.
        /// </summary>
        public UnityEngine.UI.Image imageHandLeft = null;
        /// <summary>
        /// Icon component for the right hand.
        /// </summary>
        public UnityEngine.UI.Image imageHandRight = null;

        public Sprite iconHandActiveLeft = null;
        public Sprite iconHandActiveRight = null;
        public Sprite iconHandIdleLeft = null;
        public Sprite iconHandIdleRight = null;
        public Sprite iconHandUntrackedLeft = null;
        public Sprite iconHandUntrackedRight = null;

        private IInputSimulationService inputSimService = null;
        private IInputSimulationService InputSimService => inputSimService ?? (inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>());

        /// <inheritdoc />
        void Update()
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

        public void ToggleLeftHand()
        {
            InputSimService.IsAlwaysVisibleHandLeft = !InputSimService.IsAlwaysVisibleHandLeft;
        }

        public void ToggleRightHand()
        {
            InputSimService.IsAlwaysVisibleHandRight = !InputSimService.IsAlwaysVisibleHandRight;
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A <see cref="ProximityDetector"/> that will check if any near interactor is 
    /// selecting an interactable. If a near interactor is selecting an interactable,
    /// the specified <see cref="ProximityDetector.ModeOnDetection"/> will be marked
    /// as being detected.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Near Interaction Mode Detector")]
    public class NearInteractionModeDetector : ProximityDetector
    {
        [SerializeField]
        [Tooltip("The set of near interactors that belongs to near interaction")]
        private List<XRBaseInteractor> nearInteractors;

        /// <inheritdoc />
        public override bool IsModeDetected()
        {
            return base.IsModeDetected() || IsNearInteractorSelecting();
        }

        private bool IsNearInteractorSelecting()
        {
            foreach (XRBaseInteractor nearInteractor in nearInteractors)
            {
                if (nearInteractor.hasSelection)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
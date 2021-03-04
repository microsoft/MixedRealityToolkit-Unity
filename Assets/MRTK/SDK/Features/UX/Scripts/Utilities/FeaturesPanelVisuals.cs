// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Class that initializes the appearance of the features panel according to the toggled states of the associated features
    /// </summary>
    internal class FeaturesPanelVisuals : MonoBehaviour
    {
        [SerializeField]
        private Interactable profilerButton = null;
        [SerializeField]
        private Interactable handRayButton = null;
        [SerializeField]
        private Interactable handMeshButton = null;
        [SerializeField]
        private Interactable handJointsButton = null;

        // Start is called before the first frame update
        void Start()
        {
            profilerButton.IsToggled = (CoreServices.DiagnosticsSystem?.ShowProfiler).GetValueOrDefault(false);
            handRayButton.IsToggled = PointerUtils.GetPointerBehavior<ShellHandRayPointer>(Handedness.Any, InputSourceType.Hand) != PointerBehavior.AlwaysOff;

            MixedRealityHandTrackingProfile handProfile = null;
            if (CoreServices.InputSystem?.InputSystemProfile != null)
            {
                handProfile = CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile;
            }
            handMeshButton.IsToggled = (handProfile != null) ? handProfile.EnableHandMeshVisualization : false;
            handJointsButton.IsToggled = (handProfile != null) ? handProfile.EnableHandJointVisualization : false;
        }
    }
}

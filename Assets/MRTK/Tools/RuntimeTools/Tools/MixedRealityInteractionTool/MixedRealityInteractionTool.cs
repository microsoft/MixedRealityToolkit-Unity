// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tools.Runtime
{
    /// <summary>
    /// Displays active left and right handed controllers with available interactions and their current state.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Tools/MixedRealityInteractionTool")]
    public class MixedRealityInteractionTool : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Used for displaying all detected input source names.")]
        private TextMesh listControllersTextMesh = null;

        [SerializeField]
        [Tooltip("Used for displaying the left handed input source interactions.")]
        private TextMesh leftHandedControllerTextMesh = null;

        [SerializeField]
        [Tooltip("Used for displaying the right handed input source interactions.")]
        private TextMesh rightHandedControllerTextMesh = null;

        private void Update()
        {
            if (listControllersTextMesh == null || leftHandedControllerTextMesh == null || rightHandedControllerTextMesh == null)
            {
                return;
            }

            listControllersTextMesh.text = string.Empty;
            leftHandedControllerTextMesh.text = string.Empty;
            rightHandedControllerTextMesh.text = string.Empty;

            HashSet<IMixedRealityController> controllers = CoreServices.InputSystem?.DetectedControllers;

            listControllersTextMesh.text = $"Detected {controllers?.Count} input source{(controllers?.Count > 1 ? "s:" : controllers?.Count != 0 ? ":" : "s")}\n";

            if (controllers == null)
            {
                return;
            }

            foreach (IMixedRealityController controller in controllers)
            {
                listControllersTextMesh.text += $"{controller.InputSource.SourceName}\n";

                TextMesh textMesh;

                if (controller.ControllerHandedness == Handedness.Left)
                {
                    textMesh = leftHandedControllerTextMesh;
                }
                else if (controller.ControllerHandedness == Handedness.Right)
                {
                    textMesh = rightHandedControllerTextMesh;
                }
                else
                {
                    continue;
                }

                textMesh.text = $"{controller.InputSource.SourceName}\n\n";

                foreach (MixedRealityInteractionMapping interaction in controller.Interactions)
                {
                    textMesh.text += $"{interaction.Description} [{interaction.MixedRealityInputAction.Description}]";

                    if (interaction.BoolData)
                    {
                        textMesh.text += $": {interaction.BoolData}";
                    }
                    else if (!Mathf.Approximately(interaction.FloatData, 0.0f))
                    {
                        textMesh.text += $": {interaction.FloatData}";
                    }
                    else if (interaction.Vector2Data != Vector2.zero)
                    {
                        textMesh.text += $": {interaction.Vector2Data}";
                    }
                    else if (interaction.PoseData != MixedRealityPose.ZeroIdentity)
                    {
                        textMesh.text += $": {interaction.PoseData}";
                    }
                    else if (interaction.PositionData != Vector3.zero)
                    {
                        textMesh.text += $": {interaction.PositionData}";
                    }
                    else if (interaction.RotationData != Quaternion.identity)
                    {
                        textMesh.text += $": {interaction.RotationData}";
                    }

                    textMesh.text += "\n";
                }
            }
        }
    }
}

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

                    switch (interaction.InputType)
                    {
                        case DeviceInputType.SpatialPointer:
                        case DeviceInputType.SpatialGrip:
                        case DeviceInputType.Thumb:
                        case DeviceInputType.IndexFinger:
                        case DeviceInputType.MiddleFinger:
                        case DeviceInputType.RingFinger:
                        case DeviceInputType.PinkyFinger:
                            textMesh.text += $": {interaction.PoseData}";
                            break;
                        case DeviceInputType.PointerPosition:
                        case DeviceInputType.GripPosition:
                            textMesh.text += $": {interaction.PositionData}";
                            break;
                        case DeviceInputType.PointerRotation:
                        case DeviceInputType.GripRotation:
                            textMesh.text += $": {interaction.RotationData}";
                            break;
                        case DeviceInputType.PointerClick:
                        case DeviceInputType.ButtonPress:
                        case DeviceInputType.ButtonTouch:
                        case DeviceInputType.ButtonNearTouch:
                        case DeviceInputType.TriggerTouch:
                        case DeviceInputType.TriggerNearTouch:
                        case DeviceInputType.TriggerPress:
                        case DeviceInputType.ThumbStickPress:
                        case DeviceInputType.ThumbStickTouch:
                        case DeviceInputType.ThumbStickNearTouch:
                        case DeviceInputType.TouchpadTouch:
                        case DeviceInputType.TouchpadNearTouch:
                        case DeviceInputType.TouchpadPress:
                        case DeviceInputType.Select:
                        case DeviceInputType.Start:
                        case DeviceInputType.Menu:
                        case DeviceInputType.PrimaryButtonPress:
                        case DeviceInputType.PrimaryButtonTouch:
                        case DeviceInputType.PrimaryButtonNearTouch:
                        case DeviceInputType.SecondaryButtonPress:
                        case DeviceInputType.SecondaryButtonTouch:
                        case DeviceInputType.SecondaryButtonNearTouch:
                        case DeviceInputType.GripTouch:
                        case DeviceInputType.GripNearTouch:
                        case DeviceInputType.GripPress:
                            textMesh.text += $": {interaction.BoolData}";
                            break;
                        case DeviceInputType.Trigger:
                        case DeviceInputType.Grip:
                            textMesh.text += $": {interaction.FloatData}";
                            break;
                        case DeviceInputType.ThumbStick:
                        case DeviceInputType.Touchpad:
                        case DeviceInputType.DirectionalPad:
                        case DeviceInputType.Scroll:
                            textMesh.text += $": {interaction.Vector2Data}";
                            break;
                    }

                    textMesh.text += "\n";
                }
            }
        }
    }
}

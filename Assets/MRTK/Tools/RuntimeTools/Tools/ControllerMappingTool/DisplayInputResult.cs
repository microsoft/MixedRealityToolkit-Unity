// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tools.Runtime
{
    /// <summary>
    /// Displays a specified axis / button value on a specific TextMesh.
    /// Will display all active axes and buttons if the input type is None.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Tools/DisplayInputResult")]
    public class DisplayInputResult : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Used for displaying data from input.")]
        private TextMesh displayTextMesh = null;

        [SerializeField]
        [Tooltip("The type of input to read. Will read all active if set to None.")]
        private AxisType inputType = AxisType.None;

        [SerializeField]
        [Tooltip("The axis number to read.")]
        [Range(1, UnityInputAxisCount)]
        private int axisNumber = 1;

        [SerializeField]
        [Tooltip("The button number to read.")]
        [Range(0, UnityInputButtonCount - 1)]
        private int buttonNumber = 0;

        [SerializeField]
        private DisplayType displayType = DisplayType.InputAxes;

        // This is defined in Unity's in-editor input axes
        // settings, under the Axis dropdown.
        private const int UnityInputAxisCount = 28;

        // This is defined by Unity's KeyCode enum, per JoystickButton.
        // https://docs.unity3d.com/ScriptReference/KeyCode.html
        private const int UnityInputButtonCount = 20;

        private enum DisplayType
        {
            InputAxes,
            JoystickNames
        }

        private void OnValidate()
        {
            if (displayType == DisplayType.InputAxes)
            {
                switch (inputType)
                {
                    case AxisType.SingleAxis:
                        name = $"{inputType}{axisNumber}";
                        break;
                    case AxisType.Digital:
                        name = $"{inputType}{buttonNumber}";
                        break;
                    case AxisType.None:
                        name = "AllActiveAxes";
                        break;
                }
            }
            else if (displayType == DisplayType.JoystickNames)
            {
                name = "JoystickNames";
            }
        }

        private void Update()
        {
            if (displayTextMesh == null)
            {
                return;
            }

            if (displayType == DisplayType.InputAxes)
            {
                switch (inputType)
                {
                    case AxisType.SingleAxis:
                        displayTextMesh.text = $"Axis {axisNumber}: {UnityEngine.Input.GetAxis($"AXIS_{axisNumber}")}";
                        break;
                    case AxisType.Digital:
                        if (Enum.TryParse($"JoystickButton{buttonNumber}", out KeyCode keyCode))
                        {
                            displayTextMesh.text = $"Button {buttonNumber}: {UnityEngine.Input.GetKey(keyCode)}";
                        }
                        break;
                    case AxisType.None:
                        displayTextMesh.text = "All active:\n";
                        for (int i = 1; i <= UnityInputAxisCount; i++)
                        {
                            float reading = UnityEngine.Input.GetAxis($"AXIS_{i}");

                            if (reading != 0.0)
                            {
                                displayTextMesh.text += $"Axis {i}: {reading}\n";
                            }
                        }

                        for (int i = 0; i < UnityInputButtonCount; i++)
                        {

                            if (Enum.TryParse($"JoystickButton{i}", out KeyCode buttonCode))
                            {
                                bool isPressed = UnityEngine.Input.GetKey(buttonCode);
                                if (isPressed)
                                {
                                    displayTextMesh.text += $"Button {i}: {isPressed}\n";
                                }
                            }
                        }

                        break;
                }
            }
            else
            {
                string[] joystickNames = UnityEngine.Input.GetJoystickNames();

                displayTextMesh.text = $"Detected {joystickNames.Length} controller{(joystickNames.Length != 1 ? "s" : "")}:\n";

                for (int i = 0; i < joystickNames.Length; i++)
                {
                    displayTextMesh.text += $"{joystickNames[i]}\n";
                }
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{

    /// <summary>
    /// ButtonController provides a per key or button component for the Manual input Controls
    /// in the Unity Editor, used to simulate actual HoloLens behavior.
    /// </summary>
    public class ButtonController : MonoBehaviour
    {
        /// <summary>
        /// These enums allow us to activate an axis only by a key press, such as CTRL mouse or ALT mouse
        /// </summary>
        public enum ButtonType
        {
            Left,
            Right,
            Middle,
            Control,
            Shift,
            Alt,
            Space,
            Return,
            Focused,
            ControlAndLeft,
            ControlAndRight,
            ControlAndMiddle,
            ShiftAndLeft,
            ShiftAndRight,
            ShiftAndMiddle,
            AltAndLeft,
            AltAndRight,
            AltAndMiddle,
            SpaceAndLeft,
            SpaceAndRight,
            SpaceAndMiddle,
            None
        }

        /// <summary>
        /// Type of button used for activation.
        /// </summary>
        public ButtonType buttonType = ButtonType.None;

        private bool appHasFocus = true;

        private void Awake()
        {
            // ButtonController is for development only and should not exist--and certainly not be used--in
            // any non-Editor scenario.
#if !UNITY_EDITOR
            Destroy(this);
#else
            // Workaround for Remote Desktop.  Ctrl-mouse, Shift-mouse, and Alt-mouse don't work, so they should be avoided.
            if (IsRunningUnderRemoteDesktop())
            {
                if (this.buttonType == ButtonType.Control)
                {
                    this.buttonType = ButtonType.Left;
                    Debug.LogWarning("Running under Remote Desktop, so changed ButtonController method to Left mouse button");
                }
                if (this.buttonType == ButtonType.Alt)
                {
                    this.buttonType = ButtonType.Right;
                    Debug.LogWarning("Running under Remote Desktop, so changed ButtonController method to Right mouse button");
                }
                if (this.buttonType == ButtonType.Shift)
                {
                    this.buttonType = ButtonType.Middle;
                    Debug.LogWarning("Running under Remote Desktop, so changed ButtonController method to Middle mouse button");
                }
            }
#endif
        }

        /// <summary>
        /// Returns true if the configured button is currently pressed.
        /// </summary>
        /// <returns>True if pressed.</returns>
        public bool Pressed()
        {
            bool left = Input.GetMouseButton(0);
            bool right = Input.GetMouseButton(1);
            bool middle = Input.GetMouseButton(2);
            bool control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            bool space = Input.GetKey(KeyCode.Space);
            switch (buttonType)
            {
                case ButtonType.Left:
                    return left;
                case ButtonType.Right:
                    return right;
                case ButtonType.Middle:
                    return middle;
                case ButtonType.Control:
                    return control;
                case ButtonType.Shift:
                    return shift;
                case ButtonType.Alt:
                    return alt;
                case ButtonType.Space:
                    return space;
                case ButtonType.Return:
                    return Input.GetKey(KeyCode.Return);
                case ButtonType.Focused:
                    return this.appHasFocus;
                case ButtonType.ControlAndLeft:
                    return control && left;
                case ButtonType.ControlAndRight:
                    return control && right;
                case ButtonType.ControlAndMiddle:
                    return control && middle;
                case ButtonType.ShiftAndLeft:
                    return shift && left;
                case ButtonType.ShiftAndRight:
                    return shift && right;
                case ButtonType.ShiftAndMiddle:
                    return shift && middle;
                case ButtonType.AltAndLeft:
                    return alt && left;
                case ButtonType.AltAndRight:
                    return alt && right;
                case ButtonType.AltAndMiddle:
                    return alt && middle;
                case ButtonType.SpaceAndLeft:
                    return space && left;
                case ButtonType.SpaceAndRight:
                    return space && right;
                case ButtonType.SpaceAndMiddle:
                    return space && middle;
                case ButtonType.None:
                default:
                    return false;
            };
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            this.appHasFocus = focusStatus;
        }

#if UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetCurrentProcessId();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        private bool IsRunningUnderRemoteDesktop()
        {
            uint processId = GetCurrentProcessId();
            uint sessionId;
            return ProcessIdToSessionId(processId, out sessionId) && (sessionId != WTSGetActiveConsoleSessionId());
        }
#else
        private bool IsRunningUnderRemoteDesktop()
        {
            return false;
        }
#endif

    }

} // namespace

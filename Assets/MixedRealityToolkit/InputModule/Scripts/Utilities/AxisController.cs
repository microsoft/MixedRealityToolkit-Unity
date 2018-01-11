// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities
{
    /// <summary>
    /// AxisController uses the keyboard, mouse, or joystick and allows
    /// you to map a 1 axis controller to 1 axis displacement via GetDiplacement1()
    ///  or to map a 2 axis controller to 2 axis displacement via GetDisplacement2()
    ///  or to map a 2 axis controller to 2 of the 3 axis displacement via GetDisplacement3()
    /// </summary>
    public class AxisController : MonoBehaviour
    {
        /// <summary>
        /// Type of input axis, based on device.
        /// </summary>
        public enum AxisType
        {
            // Axis are double axis (XY) unless indicated as single axis (X only)
            InputManagerAxis,

            KeyboardArrows,
            KeyboardWASD,
            KeyboardQE,     // single axis
            KeyboardIJKL,
            KeyboardUO,     // single axis
            Keyboard8426,
            Keyboard7193,
            KeyboardPeriodComma,    // single axis
            KeyboardBrackets,
            KeyBoardHomeEndPgUpPgDown,

            Mouse,
            MouseScroll,    // single axis

            None
        }

        /// <summary>
        /// Each input axis, x, y, or z, will get mapped to an output axis, with potential inversion
        /// </summary>
        public enum AxisDestination
        {
            PositiveX,
            NegativeX,
            PositiveY,
            NegativeY,
            PositiveZ,
            NegativeZ,
            None
        }

        public float SensitivityScale = 3.0f;

        [Tooltip("Use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        public AxisType axisType = AxisType.Mouse;
        public ButtonController.ButtonType buttonType = ButtonController.ButtonType.None;

        public string InputManagerHorizontalAxisName;
        public string InputManagerVerticalAxisName;

        public AxisDestination Axis0Destination = AxisDestination.PositiveX;
        public AxisDestination Axis1Destination = AxisDestination.PositiveY;
        public AxisDestination Axis2Destination = AxisDestination.None;

        private Vector3 lastMousePosition = Vector3.zero;

        private const float MouseSensitivity = 0.015f;           // always affects the mouse sensitivity
        private const float MouseUnlockedSensitivity = 0.1f;    // affects the sensitivity when using the mouse buttons
        private const float KeyboardSensitivity = 10.0f;
        private const float InputManagerAxisSensitivity = 0.05f;

        private bool isMouseJumping = false;
        private bool appHasFocus = true;
        private bool usingMouse = false;

        private bool inputManagerAxesNeedApproval = true;
        private bool inputManagerHorizontalAxisApproved = false;
        private bool inputManagerVerticalAxisApproved = false;

        public bool AxisTypeIsKeyboard
        {
            get { return AxisType.KeyboardArrows <= axisType && axisType <= AxisType.KeyBoardHomeEndPgUpPgDown; }
        }
        public bool AxisTypeIsInputManagerAxis
        {
            get { return axisType == AxisType.InputManagerAxis; }
        }
        public bool AxisTypeIsMouse
        {
            get { return axisType == AxisType.Mouse; }
        }
        public bool AxisTypeIsMouseScroll
        {
            get { return axisType == AxisType.MouseScroll; }
        }

        public void EnableAndCheck(bool value)
        {
            this.enabled = value;
            if (value)
            {
                InputManagerAxisCheck();
            }
        }

        private void Awake()
        {
            // AxisController is for development only and should not exist--and certainly not be used--in
            // any non-Editor scenario.
#if !UNITY_EDITOR
            Destroy(this);
#else
            // Workaround for Remote Desktop.  Ctrl-mouse, Shift-mouse, and Alt-mouse don't work, so they should be avoided.
            if (IsRunningUnderRemoteDesktop())
            {
                if (this.buttonType == ButtonController.ButtonType.Control)
                {
                    this.buttonType = ButtonController.ButtonType.Left;
                    Debug.LogWarning("Running under Remote Desktop, so changed AxisContol method to Left mouse button");
                }
                if (this.buttonType == ButtonController.ButtonType.Alt)
                {
                    this.buttonType = ButtonController.ButtonType.Right;
                    Debug.LogWarning("Running under Remote Desktop, so changed AxisContol method to Right mouse button");
                }
                if (this.buttonType == ButtonController.ButtonType.Shift)
                {
                    this.buttonType = ButtonController.ButtonType.Middle;
                    Debug.LogWarning("Running under Remote Desktop, so changed AxisContol method to Middle mouse button");
                }
            }

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
#endif
        }

        private static float InputCurve(float x)
        {
            // smoothing input curve, converts from [-1,1] to [-2,2]
            return (Mathf.Sign(x) * (1.0f - Mathf.Cos(.5f * Mathf.PI * Mathf.Clamp(x, -1.0f, 1.0f))));
        }

        /// <summary>
        /// Get a Vector3 populated with axis mapped displacements.
        /// </summary>
        public Vector3 GetDisplacementVector3()
        {
            Vector3 source = GetDisplacement();
            Vector3 dest = Vector3.zero;
            RemapAdditive(source, 0, ref dest, Axis0Destination);
            RemapAdditive(source, 1, ref dest, Axis1Destination);
            RemapAdditive(source, 2, ref dest, Axis2Destination);
            return dest;
        }

        /// <summary>
        /// Get a Vector2 populated with axis mapped displacements.
        /// </summary>
        public Vector2 GetDisplacementVector2()
        {
            Vector3 source = GetDisplacement();
            Vector3 middle = Vector3.zero;
            Vector2 dest = Vector2.zero;
            RemapAdditive(source, 0, ref middle, Axis0Destination);
            RemapAdditive(source, 1, ref middle, Axis1Destination);
            dest[0] = middle[0];
            dest[1] = middle[1];
            return dest;
        }

        /// <summary>
        /// Get a float populated with axis mapped displacements.
        /// </summary>
        public float GetDisplacementFloat()
        {
            Vector3 source = GetDisplacement();
            Vector3 middle = Vector2.zero;
            RemapAdditive(source, 0, ref middle, Axis0Destination);
            return middle[0];
        }

        private void RemapAdditive(Vector3 source, int sourceDim, ref Vector3 dest, AxisDestination destDim)
        {
            float inp = source[sourceDim];
            if (destDim == AxisDestination.NegativeX || destDim == AxisDestination.NegativeY || destDim == AxisDestination.NegativeZ)
            {
                inp = -inp;
            }
            if (destDim == AxisDestination.PositiveX || destDim == AxisDestination.NegativeX)
            {
                dest[0] += inp;
            }
            else if (destDim == AxisDestination.PositiveY || destDim == AxisDestination.NegativeY)
            {
                dest[1] += inp;
            }
            else if (destDim == AxisDestination.PositiveZ || destDim == AxisDestination.NegativeZ)
            {
                dest[2] += inp;
            }
        }

        private Vector3 GetDisplacement()
        {
            Vector3 rot = Vector3.zero;

            // this check enables us to check the InputManagerAxes names when we are switching on the fly
            if (!AxisTypeIsInputManagerAxis)
            {
                inputManagerAxesNeedApproval = true;
            }

            // Now check to see what sort of input we have, and dispatch the appropriate LookTick routine
            if (AxisTypeIsInputManagerAxis)
            {
                if (inputManagerAxesNeedApproval)
                {
                    InputManagerAxisCheck();
                }
                if (ShouldControl())
                {
                    rot = InputManagerAxisLookTick();
                }
            }
            else if (AxisTypeIsKeyboard)
            {
                if (ShouldControl())
                    rot = KeyboardLookTick();
            }
            else if (AxisTypeIsMouseScroll)
            {
                if (ShouldControl())
                    rot.x += Input.GetAxis("Mouse ScrollWheel");
            }
            else if (AxisTypeIsMouse)
            {
                if (ShouldControl())
                {
                    if (!this.usingMouse)
                    {
                        OnStartMouseLook();
                        this.usingMouse = true;
                    }
                    rot = MouseLookTick();
                }
                else
                {
                    if (this.usingMouse)
                    {
                        OnEndMouseLook();
                        this.usingMouse = false;
                    }
                }
            }

            rot *= this.SensitivityScale;
            return rot;
        }

        private void OnStartMouseLook()
        {
            if (this.buttonType <= ButtonController.ButtonType.Middle)
            {
                // if mouse button is either left, right or middle
                SetWantsMouseJumping(true);
            }
            else if (this.buttonType <= ButtonController.ButtonType.Focused)
            {
                // if mouse button is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void OnEndMouseLook()
        {
            if (this.buttonType <= ButtonController.ButtonType.Middle)
            {
                // if mouse button is either left, right or middle
                SetWantsMouseJumping(false);
            }
            else if (this.buttonType <= ButtonController.ButtonType.Focused)
            {
                // if mouse button is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private Vector3 MouseLookTick()
        {
            Vector3 rot = Vector3.zero;

            // Use frame-to-frame mouse delta in pixels to determine mouse rotation. The traditional
            // GetAxis("Mouse X") method doesn't work under Remote Desktop.
            Vector3 mousePositionDelta = Input.mousePosition - this.lastMousePosition;
            this.lastMousePosition = Input.mousePosition;

            if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
            {
                mousePositionDelta.x = Input.GetAxis("Mouse X");
                mousePositionDelta.y = Input.GetAxis("Mouse Y");
            }
            else
            {
                mousePositionDelta.x *= MouseUnlockedSensitivity;
                mousePositionDelta.y *= MouseUnlockedSensitivity;
            }

            rot.x += -InputCurve(mousePositionDelta.y) * MouseSensitivity;
            rot.y += InputCurve(mousePositionDelta.x) * MouseSensitivity;
            return rot;
        }

        private float GetKeyDir(KeyCode neg, KeyCode pos)
        {
            return Input.GetKey(neg) ? -1.0f : Input.GetKey(pos) ? 1.0f : 0.0f;
        }

        private float GetKeyDir(string neg, string pos)
        {
            return Input.GetKey(neg) ? -1.0f : Input.GetKey(pos) ? 1.0f : 0.0f;
        }

        private void InputManagerAxisCheck()
        {
            inputManagerHorizontalAxisApproved = false;
            inputManagerVerticalAxisApproved = false;

            {
                inputManagerHorizontalAxisApproved = true;
                try
                {
                    Input.GetAxis(InputManagerHorizontalAxisName);
                }
                catch (Exception)
                {
                    Debug.LogWarningFormat("Input Axis {0} is not setup. Use Edit -> Project Settings -> Input", InputManagerHorizontalAxisName);
                    inputManagerHorizontalAxisApproved = false;
                }
            }

            {
                inputManagerVerticalAxisApproved = true;
                try
                {
                    Input.GetAxis(InputManagerVerticalAxisName);
                }
                catch (Exception)
                {
                    Debug.LogWarningFormat("Input Axis {0} is not setup. Use Edit -> Project Settings -> Input", InputManagerVerticalAxisName);
                    inputManagerVerticalAxisApproved = false;
                }
            }
            inputManagerAxesNeedApproval = false;
        }

        private Vector3 InputManagerAxisLookTick()
        {
            Vector3 rot = Vector3.zero;
            if (inputManagerHorizontalAxisApproved)
            {
                rot.x += InputManagerAxisSensitivity * InputCurve(Input.GetAxis(InputManagerHorizontalAxisName));
            }
            if (inputManagerVerticalAxisApproved)
            {
                rot.y += InputManagerAxisSensitivity * InputCurve(Input.GetAxis(InputManagerVerticalAxisName));
            }
            return rot;
        }

        private Vector3 KeyboardLookTick()
        {
            float deltaTime = UseUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            Vector3 rot = Vector3.zero;
            if (axisType == AxisType.KeyboardArrows)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.LeftArrow, KeyCode.RightArrow));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.DownArrow, KeyCode.UpArrow));
            }
            else if (axisType == AxisType.KeyboardWASD)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.A, KeyCode.D));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.S, KeyCode.W));
            }
            else if (axisType == AxisType.KeyboardQE)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Q, KeyCode.E));
            }
            else if (axisType == AxisType.KeyboardIJKL)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.J, KeyCode.L));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.K, KeyCode.I));
            }
            else if (axisType == AxisType.KeyboardUO)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.U, KeyCode.O));
            }
            else if (axisType == AxisType.Keyboard8426)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Keypad4, KeyCode.Keypad6));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Keypad2, KeyCode.Keypad8));
            }
            else if (axisType == AxisType.Keyboard7193)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Keypad1, KeyCode.Keypad7));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Keypad3, KeyCode.Keypad9));
            }
            else if (axisType == AxisType.KeyboardPeriodComma)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.Comma, KeyCode.Period));
            }
            else if (axisType == AxisType.KeyboardBrackets)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.LeftBracket, KeyCode.RightBracket));
            }
            else if (axisType == AxisType.KeyBoardHomeEndPgUpPgDown)
            {
                rot.x += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.End, KeyCode.Home));
                rot.y += InputCurve(deltaTime * KeyboardSensitivity * GetKeyDir(KeyCode.PageDown, KeyCode.PageUp));
            }
            return rot;
        }

        /// <summary>
        /// Only allow the mouse to control rotation when Unity has focus. This enables
        /// the player to temporarily alt-tab away without having the player look around randomly
        /// back in the Unity Game window.
        /// </summary>
        /// <returns>Whether the user is holding down the control button.</returns>
        public bool ShouldControl()
        {
            if (!this.appHasFocus)
            {
                return false;
            }
            else if (this.buttonType == ButtonController.ButtonType.None)
            {
                return true;
            }
            else if (this.buttonType <= ButtonController.ButtonType.Middle)
            {
                return Input.GetMouseButton((int)this.buttonType);
            }
            else if (this.buttonType == ButtonController.ButtonType.Control)
            {
                return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            }
            else if (this.buttonType == ButtonController.ButtonType.Shift)
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
            else if (this.buttonType == ButtonController.ButtonType.Alt)
            {
                return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            }
            else if (this.buttonType == ButtonController.ButtonType.Space)
            {
                return Input.GetKey(KeyCode.Space);
            }
            else if (this.buttonType == ButtonController.ButtonType.Return)
            {
                return Input.GetKey(KeyCode.Return);
            }
            else if (this.buttonType == ButtonController.ButtonType.Focused)
            {
                if (!this.usingMouse)
                {
                    // any kind of click will capture focus
                    return Input.GetMouseButtonDown((int)ButtonController.ButtonType.Left)
                        || Input.GetMouseButtonDown((int)ButtonController.ButtonType.Right)
                        || Input.GetMouseButtonDown((int)ButtonController.ButtonType.Middle);
                }
                else
                {
                    // pressing escape will stop capture
                    return !Input.GetKeyDown(KeyCode.Escape);
                }
            }

            return false;
        }

        private void OnApplicationFocus(bool focusStatus)
        {
            this.appHasFocus = focusStatus;
        }

        /// <summary>
        ///  Mouse jumping is typically used during one of the mouse button modes.
        ///  It means that the cursor will be invisible when it is outside of the
        ///  Unity game view window, and visible when it breaches the outer edges.
        /// </summary>
        /// <param name="wantsJumping">Whether the mouse cursor should be visible over the game window.</param>
        private void SetWantsMouseJumping(bool wantsJumping)
        {
            if (wantsJumping != this.isMouseJumping)
            {
                this.isMouseJumping = wantsJumping;

                if (wantsJumping)
                {
                    // unlock the cursor if it was locked
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // hide the cursor
                    UnityEngine.Cursor.visible = false;

                    this.lastMousePosition = Input.mousePosition;
                }
                else
                {
                    // recenter the cursor (setting lockCursor has side-effects under the hood)
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // show the cursor
                    UnityEngine.Cursor.visible = true;
                }

#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.SetWantsMouseJumping(wantsJumping ? 1 : 0);
#endif
            }
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

}

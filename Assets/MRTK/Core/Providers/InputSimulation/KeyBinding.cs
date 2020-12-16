// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Services.InputSimulation.Editor")]
namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Identifier of a key combination or mouse button for generic input binding.
    /// </summary>
    /// <remarks>
    /// This encodes either a KeyCode with optional modifiers or a mouse button index.
    /// </remarks>
    [System.Serializable]
    public struct KeyBinding
    {
        /// <summary>
        /// The type of value encoded in the <see cref="code"/> property.
        /// </summary>
        public enum KeyType : int
        {
            None = 0,
            Mouse = 1,
            Key = 2,
        }

        /// <summary>
        /// Enum for interpreting the mouse button integer index.
        /// </summary>
        public enum MouseButton : int
        {
            Left = 0,
            Right = 1,
            Middle = 2,
            Button3 = 3,
            Button4 = 4,
            Button5 = 5,
            Button6 = 6,
            Button7 = 7,
        }

        // Array of names to use for a combined enum selection.
        internal static readonly string[] AllCodeNames;
        // Maps (KeyType, code) combination onto the contiguous index used for enums.
        // Value can be used as index in AllCodeNames array.
        internal static readonly Dictionary<Tuple<KeyType, int>, int> KeyBindingToEnumMap;
        // Maps enum index to a KeyBinding, for assignment after selecting an enum value.
        internal static readonly Dictionary<int, Tuple<KeyType, int>> EnumToKeyBindingMap;

        // Static constructor to initialize static fields
        static KeyBinding()
        {
            KeyCode[] KeyCodeValues = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            MouseButton[] MouseButtonValues = (MouseButton[])Enum.GetValues(typeof(MouseButton));

            // Build maps for converting between int enum value and KeyBinding values
            {
                KeyBindingToEnumMap = new Dictionary<Tuple<KeyType, int>, int>();
                EnumToKeyBindingMap = new Dictionary<int, Tuple<KeyType, int>>();
                List<string> names = new List<string>();

                int index = 0;
                Action<KeyType, int> AddEnumValue = (bindingType, code) =>
                {
                    var kb = new KeyBinding() { bindingType = bindingType, code = code };
                    names.Add(kb.ToString());
                    EnumToKeyBindingMap[index] = Tuple.Create(bindingType, code);
                    KeyBindingToEnumMap[Tuple.Create(bindingType, code)] = index;

                    ++index;
                };

                AddEnumValue(KeyType.None, 0);

                foreach (MouseButton mb in MouseButtonValues)
                {
                    AddEnumValue(KeyType.Mouse, (int)mb);
                }

                foreach (KeyCode kc in KeyCodeValues)
                {
                    AddEnumValue(KeyType.Key, (int)kc);
                }

                AllCodeNames = names.ToArray();
            }
        }

        [SerializeField]
        private KeyType bindingType;

        /// <summary>
        /// Type of input this binding maps to.
        /// </summary>
        public KeyType BindingType => bindingType;

        // Internal binding code.
        // This can be a KeyCode or mouse button index, depending on the bindingType;
        [SerializeField]
        private int code;

        public override string ToString()
        {
            string s = "";

            s += bindingType.ToString();

            switch (bindingType)
            {
                case KeyType.Key:
                    s += ": " + ((KeyCode)code).ToString();
                    break;
                case KeyType.Mouse:
                    s += ": " + ((MouseButton)code).ToString();
                    break;
            }
            return s;
        }

        /// <summary>
        /// Try to convert the binding to a KeyCode.
        /// </summary>
        /// <returns>True if the binding is a keyboard key</returns>
        public bool TryGetKeyCode(out KeyCode keyCode)
        {
            keyCode = (KeyCode)code;
            return bindingType == KeyType.Key;
        }

        /// <summary>
        /// Try to convert the binding to a mouse button.
        /// </summary>
        /// <returns>True if the binding is a mouse button</returns>
        public bool TryGetMouseButton(out int mouseButton)
        {
            mouseButton = code;
            return bindingType == KeyType.Mouse;
        }

        /// <summary>
        /// Try to convert the binding to a mouse button.
        /// </summary>
        /// <returns>True if the binding is a mouse button</returns>
        public bool TryGetMouseButton(out MouseButton mouseButton)
        {
            if (TryGetMouseButton(out int iMouseButton))
            {
                mouseButton = (MouseButton)iMouseButton;
                return true;
            }
            mouseButton = MouseButton.Left;
            return false;
        }


        /// <summary>
        /// Create a default empty binding.
        /// </summary>
        public static KeyBinding Unbound()
        {
            KeyBinding kb = new KeyBinding();
            kb.bindingType = KeyType.None;
            kb.code = 0;
            return kb;
        }

        /// <summary>
        /// Create a binding for a keyboard key.
        /// </summary>
        public static KeyBinding FromKey(KeyCode keyCode)
        {
            KeyBinding kb = new KeyBinding();
            kb.bindingType = KeyType.Key;
            kb.code = (int)keyCode;
            return kb;
        }

        /// <summary>
        /// Create a binding for a mouse button.
        /// </summary>
        public static KeyBinding FromMouseButton(int mouseButton)
        {
            KeyBinding kb = new KeyBinding();
            kb.bindingType = KeyType.Mouse;
            kb.code = mouseButton;
            return kb;
        }

        /// <summary>
        /// Create a binding for a mouse button.
        /// </summary>
        public static KeyBinding FromMouseButton(MouseButton mouseButton)
        {
            return FromMouseButton((int)mouseButton);
        }
    }

    /// <summary>
    /// Utility class to poll input for key bindings and to simulate key presses
    /// Need to add mechanisms to poll and simulate input axis: https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7659
    /// </summary>
    public static class KeyInputSystem
    {
        private static bool isSimulated;
        public static bool SimulatingInput => isSimulated;

        private static HashSet<int> SimulatedMouseDownSet;
        private static HashSet<KeyCode> SimulatedKeyDownSet;

        private static HashSet<int> SimulatedMouseSet;
        private static HashSet<KeyCode> SimulatedKeySet;

        private static HashSet<int> SimulatedMouseUpSet;
        private static HashSet<KeyCode> SimulatedKeyUpSet;

        /// <summary>
        /// Starts the key input simulation. Inputs can now be simulated via <see cref="PressKey(KeyBinding)"/> and <see cref="ReleaseKey(KeyBinding)"/>
        /// </summary>
        public static void StartKeyInputStimulation()
        {
            ResetKeyInputSimulation();
            isSimulated = true;
        }

        /// <summary>
        /// Stops the key input simulation
        /// </summary>
        public static void StopKeyInputSimulation()
        {
            isSimulated = false;
        }

        /// <summary>
        /// Resets the key input simulation. All keys will not trigger <see cref="GetKeyDown(KeyBinding)"/>, <see cref="GetKey(KeyBinding)"/>, or <see cref="GetKeyUp(KeyBinding)"/>
        /// </summary>
        public static void ResetKeyInputSimulation()
        {
            SimulatedMouseDownSet = new HashSet<int>();
            SimulatedKeyDownSet = new HashSet<KeyCode>();

            SimulatedMouseSet = new HashSet<int>();
            SimulatedKeySet = new HashSet<KeyCode>();

            SimulatedMouseUpSet = new HashSet<int>();
            SimulatedKeyUpSet = new HashSet<KeyCode>();
        }

        /// <summary>
        /// Presses a key. <see cref="GetKeyDown(KeyBinding)"/> and <see cref="GetKey(KeyBinding)"/> will be true for the keybinding. 
        /// <see cref="GetKeyUp(KeyBinding)"/> will no longer be true for the keybinding.
        /// </summary>
        public static void PressKey(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                SimulatedMouseDownSet.Add(mouseButton);
                SimulatedMouseSet.Add(mouseButton);
                SimulatedMouseUpSet.Remove(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                SimulatedKeyDownSet.Add(keyCode);
                SimulatedKeySet.Add(keyCode);
                SimulatedKeyUpSet.Remove(keyCode);
            }
        }
        /// <summary>
        /// Releases a key. <see cref="GetKeyUp(KeyBinding)"/> will be true for the keybinding. 
        /// <see cref="GetKeyDown(KeyBinding)"/> and <see cref="GetKey(KeyBinding)"/>  will no longer be true for the keybinding.
        /// </summary>
        public static void ReleaseKey(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                SimulatedMouseDownSet.Remove(mouseButton);
                SimulatedMouseSet.Remove(mouseButton);
                SimulatedMouseUpSet.Add(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                SimulatedKeyDownSet.Remove(keyCode);
                SimulatedKeySet.Remove(keyCode);
                SimulatedKeyUpSet.Add(keyCode);
            }
        }

        /// <summary>
        /// Advances the Key press simulation by 1 frame. Keybindings will no longer trigger <see cref="GetKeyDown(KeyBinding)"/>  or <see cref="GetKeyUp(KeyBinding)"/> 
        /// </summary>
        public static void AdvanceSimulation()
        {
            // keys that were just pressed are no longer trigger GetKeyDown
            SimulatedMouseDownSet = new HashSet<int>();
            SimulatedKeyDownSet = new HashSet<KeyCode>();

            // keys that were just released are no longer trigger GetKeyUp
            SimulatedMouseUpSet = new HashSet<int>();
            SimulatedKeyUpSet = new HashSet<KeyCode>();
        }

        /// <summary>
        /// Test if the key is currently pressed.
        /// </summary>
        /// <returns>True if the bound key is currently pressed</returns>
        public static bool GetKey(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                if (isSimulated)
                    return SimulatedMouseSet.Contains(mouseButton);
                else
#if INPUTSYSTEM_PACKAGE
                    switch (mouseButton)
                    {
                        case 0:
                            return UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
                        case 1:
                            return UnityEngine.InputSystem.Mouse.current.rightButton.isPressed;
                        case 2:
                            return UnityEngine.InputSystem.Mouse.current.middleButton.isPressed;
                        default:
                            return UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
                    }
#else
                    return UnityEngine.Input.GetMouseButton(mouseButton);
#endif // INPUTSYSTEM_PACKAGE
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                if (isSimulated)
                    return SimulatedKeySet.Contains(keyCode);
                else
#if INPUTSYSTEM_PACKAGE
                    return UnityEngine.InputSystem.Keyboard.current[MapKeyCodeToKey(keyCode)].isPressed;
#else
                    return UnityEngine.Input.GetKey(keyCode);
#endif // INPUTSYSTEM_PACKAGE
            }
            return false;
        }

        /// <summary>
        /// Test if the key has been pressed since the last frame.
        /// </summary>
        /// <returns>True if the bound key was pressed since the last frame</returns>
        public static bool GetKeyDown(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                if (isSimulated)
                    return SimulatedMouseDownSet.Contains(mouseButton);
                else
                    return UnityEngine.Input.GetMouseButtonDown(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                if (isSimulated)
                    return SimulatedKeyDownSet.Contains(keyCode);
                else
#if INPUTSYSTEM_PACKAGE
                    return UnityEngine.InputSystem.Keyboard.current[MapKeyCodeToKey(keyCode)].wasPressedThisFrame;
#else
                    return UnityEngine.Input.GetKeyDown(keyCode);
#endif // INPUTSYSTEM_PACKAGE
            }
            return false;
        }

        /// <summary>
        /// Test if the key has been released since the last frame.
        /// </summary>
        /// <returns>True if the bound key was released since the last frame</returns>
        public static bool GetKeyUp(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                if (isSimulated)
                    return SimulatedMouseUpSet.Contains(mouseButton);
                else
                    return UnityEngine.Input.GetMouseButtonUp(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                if (isSimulated)
                    return SimulatedKeyUpSet.Contains(keyCode);
                else
#if INPUTSYSTEM_PACKAGE
                    return UnityEngine.InputSystem.Keyboard.current[MapKeyCodeToKey(keyCode)].wasReleasedThisFrame;
#else
                    return UnityEngine.Input.GetKeyUp(keyCode);
#endif // INPUTSYSTEM_PACKAGE
            }
            return false;
        }

#if INPUTSYSTEM_PACKAGE
        internal static UnityEngine.InputSystem.Key MapKeyCodeToKey(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Space:
                    return UnityEngine.InputSystem.Key.Space;
                case KeyCode.Return:
                    return UnityEngine.InputSystem.Key.Enter;
                case KeyCode.Tab:
                    return UnityEngine.InputSystem.Key.Tab;
                case KeyCode.BackQuote:
                    return UnityEngine.InputSystem.Key.Backquote;
                case KeyCode.Quote:
                    return UnityEngine.InputSystem.Key.Quote;
                case KeyCode.Semicolon:
                    return UnityEngine.InputSystem.Key.Semicolon;
                case KeyCode.Comma:
                    return UnityEngine.InputSystem.Key.Comma;
                case KeyCode.Period:
                    return UnityEngine.InputSystem.Key.Period;
                case KeyCode.Slash:
                    return UnityEngine.InputSystem.Key.Slash;
                case KeyCode.Backslash:
                    return UnityEngine.InputSystem.Key.Backslash;
                case KeyCode.LeftBracket:
                    return UnityEngine.InputSystem.Key.LeftBracket;
                case KeyCode.RightBracket:
                    return UnityEngine.InputSystem.Key.RightBracket;
                case KeyCode.Minus:
                    return UnityEngine.InputSystem.Key.Minus;
                case KeyCode.Equals:
                    return UnityEngine.InputSystem.Key.Equals;
                case KeyCode.A:
                    return UnityEngine.InputSystem.Key.A;
                case KeyCode.B:
                    return UnityEngine.InputSystem.Key.B;
                case KeyCode.C:
                    return UnityEngine.InputSystem.Key.C;
                case KeyCode.D:
                    return UnityEngine.InputSystem.Key.D;
                case KeyCode.E:
                    return UnityEngine.InputSystem.Key.E;
                case KeyCode.F:
                    return UnityEngine.InputSystem.Key.F;
                case KeyCode.G:
                    return UnityEngine.InputSystem.Key.G;
                case KeyCode.H:
                    return UnityEngine.InputSystem.Key.H;
                case KeyCode.I:
                    return UnityEngine.InputSystem.Key.I;
                case KeyCode.J:
                    return UnityEngine.InputSystem.Key.J;
                case KeyCode.K:
                    return UnityEngine.InputSystem.Key.K;
                case KeyCode.L:
                    return UnityEngine.InputSystem.Key.L;
                case KeyCode.M:
                    return UnityEngine.InputSystem.Key.M;
                case KeyCode.N:
                    return UnityEngine.InputSystem.Key.N;
                case KeyCode.O:
                    return UnityEngine.InputSystem.Key.O;
                case KeyCode.P:
                    return UnityEngine.InputSystem.Key.P;
                case KeyCode.Q:
                    return UnityEngine.InputSystem.Key.Q;
                case KeyCode.R:
                    return UnityEngine.InputSystem.Key.R;
                case KeyCode.S:
                    return UnityEngine.InputSystem.Key.S;
                case KeyCode.T:
                    return UnityEngine.InputSystem.Key.T;
                case KeyCode.U:
                    return UnityEngine.InputSystem.Key.U;
                case KeyCode.V:
                    return UnityEngine.InputSystem.Key.V;
                case KeyCode.W:
                    return UnityEngine.InputSystem.Key.W;
                case KeyCode.X:
                    return UnityEngine.InputSystem.Key.X;
                case KeyCode.Y:
                    return UnityEngine.InputSystem.Key.Y;
                case KeyCode.Z:
                    return UnityEngine.InputSystem.Key.Z;
                case KeyCode.Alpha1:
                    return UnityEngine.InputSystem.Key.Digit1;
                case KeyCode.Alpha2:
                    return UnityEngine.InputSystem.Key.Digit2;
                case KeyCode.Alpha3:
                    return UnityEngine.InputSystem.Key.Digit3;
                case KeyCode.Alpha4:
                    return UnityEngine.InputSystem.Key.Digit4;
                case KeyCode.Alpha5:
                    return UnityEngine.InputSystem.Key.Digit5;
                case KeyCode.Alpha6:
                    return UnityEngine.InputSystem.Key.Digit6;
                case KeyCode.Alpha7:
                    return UnityEngine.InputSystem.Key.Digit7;
                case KeyCode.Alpha8:
                    return UnityEngine.InputSystem.Key.Digit8;
                case KeyCode.Alpha9:
                    return UnityEngine.InputSystem.Key.Digit9;
                case KeyCode.Alpha0:
                    return UnityEngine.InputSystem.Key.Digit0;
                case KeyCode.LeftShift:
                    return UnityEngine.InputSystem.Key.LeftShift;
                case KeyCode.RightShift:
                    return UnityEngine.InputSystem.Key.RightShift;
                case KeyCode.LeftAlt:
                    return UnityEngine.InputSystem.Key.LeftAlt;
                case KeyCode.RightAlt:
                    return UnityEngine.InputSystem.Key.RightAlt;
                case KeyCode.AltGr:
                    return UnityEngine.InputSystem.Key.AltGr;
                case KeyCode.LeftControl:
                    return UnityEngine.InputSystem.Key.LeftCtrl;
                case KeyCode.RightControl:
                    return UnityEngine.InputSystem.Key.RightCtrl;
                case KeyCode.LeftWindows:
                case KeyCode.LeftCommand:
                    return UnityEngine.InputSystem.Key.LeftCommand;
                case KeyCode.RightWindows:
                case KeyCode.RightCommand:
                    return UnityEngine.InputSystem.Key.RightCommand;
                case KeyCode.Escape:
                    return UnityEngine.InputSystem.Key.Escape;
                case KeyCode.LeftArrow:
                    return UnityEngine.InputSystem.Key.LeftArrow;
                case KeyCode.RightArrow:
                    return UnityEngine.InputSystem.Key.RightArrow;
                case KeyCode.UpArrow:
                    return UnityEngine.InputSystem.Key.UpArrow;
                case KeyCode.DownArrow:
                    return UnityEngine.InputSystem.Key.DownArrow;
                case KeyCode.Backspace:
                    return UnityEngine.InputSystem.Key.Backspace;
                case KeyCode.PageDown:
                    return UnityEngine.InputSystem.Key.PageDown;
                case KeyCode.PageUp:
                    return UnityEngine.InputSystem.Key.PageUp;
                case KeyCode.Home:
                    return UnityEngine.InputSystem.Key.Home;
                case KeyCode.Insert:
                    return UnityEngine.InputSystem.Key.Insert;
                case KeyCode.Delete:
                    return UnityEngine.InputSystem.Key.Delete;
                case KeyCode.CapsLock:
                    return UnityEngine.InputSystem.Key.CapsLock;
                case KeyCode.Numlock:
                    return UnityEngine.InputSystem.Key.NumLock;
                case KeyCode.Print:
                    return UnityEngine.InputSystem.Key.PrintScreen;
                case KeyCode.ScrollLock:
                    return UnityEngine.InputSystem.Key.ScrollLock;
                case KeyCode.Pause:
                    return UnityEngine.InputSystem.Key.Pause;
                case KeyCode.KeypadEnter:
                    return UnityEngine.InputSystem.Key.NumpadEnter;
                case KeyCode.KeypadDivide:
                    return UnityEngine.InputSystem.Key.NumpadDivide;
                case KeyCode.KeypadMultiply:
                    return UnityEngine.InputSystem.Key.NumpadMultiply;
                case KeyCode.KeypadPlus:
                    return UnityEngine.InputSystem.Key.NumpadPlus;
                case KeyCode.KeypadMinus:
                    return UnityEngine.InputSystem.Key.NumpadMinus;
                case KeyCode.KeypadPeriod:
                    return UnityEngine.InputSystem.Key.NumpadPeriod;
                case KeyCode.KeypadEquals:
                    return UnityEngine.InputSystem.Key.NumpadEquals;
                case KeyCode.Keypad0:
                    return UnityEngine.InputSystem.Key.Numpad0;
                case KeyCode.Keypad1:
                    return UnityEngine.InputSystem.Key.Numpad1;
                case KeyCode.Keypad2:
                    return UnityEngine.InputSystem.Key.Numpad2;
                case KeyCode.Keypad3:
                    return UnityEngine.InputSystem.Key.Numpad3;
                case KeyCode.Keypad4:
                    return UnityEngine.InputSystem.Key.Numpad4;
                case KeyCode.Keypad5:
                    return UnityEngine.InputSystem.Key.Numpad5;
                case KeyCode.Keypad6:
                    return UnityEngine.InputSystem.Key.Numpad6;
                case KeyCode.Keypad7:
                    return UnityEngine.InputSystem.Key.Numpad7;
                case KeyCode.Keypad8:
                    return UnityEngine.InputSystem.Key.Numpad8;
                case KeyCode.Keypad9:
                    return UnityEngine.InputSystem.Key.Numpad9;
                case KeyCode.F1:
                    return UnityEngine.InputSystem.Key.F1;
                case KeyCode.F2:
                    return UnityEngine.InputSystem.Key.F2;
                case KeyCode.F3:
                    return UnityEngine.InputSystem.Key.F3;
                case KeyCode.F4:
                    return UnityEngine.InputSystem.Key.F4;
                case KeyCode.F5:
                    return UnityEngine.InputSystem.Key.F5;
                case KeyCode.F6:
                    return UnityEngine.InputSystem.Key.F6;
                case KeyCode.F7:
                    return UnityEngine.InputSystem.Key.F7;
                case KeyCode.F8:
                    return UnityEngine.InputSystem.Key.F8;
                case KeyCode.F9:
                    return UnityEngine.InputSystem.Key.F9;
                case KeyCode.F10:
                    return UnityEngine.InputSystem.Key.F10;
                case KeyCode.F11:
                    return UnityEngine.InputSystem.Key.F11;
                case KeyCode.F12:
                    return UnityEngine.InputSystem.Key.F12;
                default:
                    return UnityEngine.InputSystem.Key.None;
            }
        }
#endif // INPUTSYSTEM_PACKAGE
    }
}

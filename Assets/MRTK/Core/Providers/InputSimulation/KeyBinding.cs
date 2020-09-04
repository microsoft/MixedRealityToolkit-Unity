// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        /// <inheritdoc />
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
                    return UnityEngine.Input.GetMouseButton(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                if (isSimulated)
                    return SimulatedKeySet.Contains(keyCode);
                else
                    return UnityEngine.Input.GetKey(keyCode);
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
                    return UnityEngine.Input.GetKeyDown(keyCode);
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
                    return UnityEngine.Input.GetKeyUp(keyCode);
            }
            return false;
        }
    }
}
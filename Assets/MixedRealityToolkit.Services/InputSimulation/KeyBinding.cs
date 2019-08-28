// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;

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
        /// <remarks>
        /// The actual KeyCode or button index is added to the base type value.
        /// </remarks>
        public enum KeyType : int
        {
            None = 0,
            MouseButton = 1000,
            Key = 2000,
        }
        private static KeyType[] KeyTypeValues = (KeyType[])Enum.GetValues(typeof(KeyType));
        private static string[] KeyTypeNames = Enum.GetNames(typeof(KeyType));

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
        private static MouseButton[] MouseButtonValues = (MouseButton[])Enum.GetValues(typeof(MouseButton));
        private static string[] MouseButtonNames = Enum.GetNames(typeof(MouseButton));

        [SerializeField]
        private int code;
        /// <summary>
        /// The internal value that encodes the key binding.
        /// </summary>
        public int Code => code;

        /// <summary>
        /// Get a string representation of an internal code value.
        /// </summary>
        public static string CodeToString(int code)
        {
            string s = "";

            GetKeyTypeAndCode(code, out KeyType keyType, out int keyCode);
            s += keyType.ToString();

            switch (keyType)
            {
                case KeyType.Key:
                    s += ": " + ((KeyCode)keyCode).ToString();
                    break;
                case KeyType.MouseButton:
                    s += ": " + ((MouseButton)keyCode).ToString();
                    break;
            }
            return s;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return CodeToString(code);
        }

        /// <summary>
        /// Convert an encoded key binding value into a combination of <see cref="KeyType"/> and the actual key code.
        /// </summary>
        public static void GetKeyTypeAndCode(int code, out KeyType keyType, out int keyCode)
        {
            for (int i = KeyTypeValues.Length - 1; i >= 0; --i)
            {
                if (code >= (int)KeyTypeValues[i])
                {
                    keyType = KeyTypeValues[i];
                    keyCode = code - (int)KeyTypeValues[i];
                    return;
                }
            }

            keyType = KeyType.None;
            keyCode = 0;
        }

        /// <summary>
        /// Get the type of binding encoded by value.
        /// </summary>
        public KeyType GetKeyType()
        {
            GetKeyTypeAndCode(code, out KeyType keyType, out int keyCode);
            return keyType;
        }

        /// <summary>
        /// Try to convert the binding to a KeyCode.
        /// </summary>
        /// <returns>True if the binding is a keyboard key</returns>
        public bool TryGetKeyCode(out KeyCode keyCode)
        {
            GetKeyTypeAndCode(code, out KeyType keyType, out int iKeyCode);
            keyCode = (KeyCode)iKeyCode;
            return keyType == KeyType.Key;
        }

        /// <summary>
        /// Try to convert the binding to a mouse button.
        /// </summary>
        /// <returns>True if the binding is a mouse button</returns>
        public bool TryGetMouseButton(out int mouseButton)
        {
            GetKeyTypeAndCode(code, out KeyType keyType, out mouseButton);
            return keyType == KeyType.MouseButton;
        }

        /// <summary>
        /// Try to convert the binding to a mouse button.
        /// </summary>
        /// <returns>True if the binding is a mouse button</returns>
        public bool TryGetMouseButton(out MouseButton mouseButton)
        {
            if (TryGetMouseButton(out int iMouseButton))
            {
                foreach (MouseButton mb in MouseButtonValues)
                {
                    if (iMouseButton == (int)mb)
                    {
                        mouseButton = mb;
                        return true;
                    }
                }
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
            kb.code = (int)KeyType.None;
            return kb;
        }

        /// <summary>
        /// Create a binding for a keyboard key.
        /// </summary>
        public static KeyBinding FromKey(KeyCode keyCode)
        {
            KeyBinding kb = new KeyBinding();
            kb.code = (int)KeyType.Key + (int)keyCode;
            return kb;
        }

        /// <summary>
        /// Create a binding for a mouse button.
        /// </summary>
        public static KeyBinding FromMouseButton(int mouseButton)
        {
            KeyBinding kb = new KeyBinding();
            kb.code = (int)KeyType.MouseButton + mouseButton;
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
    /// Utility class to poll input for key bindings.
    /// </summary>
    public static class KeyInputSystem
    {
        /// <summary>
        /// Test if the key is currently pressed.
        /// </summary>
        /// <returns>True if the bound key is currently pressed</returns>
        public static bool GetKey(KeyBinding kb)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                return UnityEngine.Input.GetMouseButton(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
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
                return UnityEngine.Input.GetMouseButtonDown(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                return UnityEngine.Input.GetKeyDown(keyCode);
            }
            return false;
        }

        /// <summary>
        /// Test if the key has been released since the last frame.
        /// </summary>
        /// <returns>True if the bound key was released since the last frame</returns>
        public static bool GetKeyUp(KeyBinding kb, ref bool wasFocused)
        {
            if (kb.TryGetMouseButton(out int mouseButton))
            {
                return UnityEngine.Input.GetMouseButtonUp(mouseButton);
            }
            if (kb.TryGetKeyCode(out KeyCode keyCode))
            {
                return UnityEngine.Input.GetKeyUp(keyCode);
            }
            return false;
        }
    }
}
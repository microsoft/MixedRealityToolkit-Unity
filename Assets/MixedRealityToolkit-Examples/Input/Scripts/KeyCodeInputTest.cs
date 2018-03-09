// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class KeyCodeInputTest : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private Text text;

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            Debug.LogFormat("KeyUp: {0}", eventData.KeyCode);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            Debug.LogFormat("KeyDown: {0}", eventData.KeyCode);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        void IInputHandler.OnInputPressed(InputPressedEventData eventData)
        {
            Debug.LogFormat("KeyPressed: {0}", eventData.KeyCode);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        public void OnInputPositionChanged(InputPositionEventData eventData) { }

        private void Update()
        {
            foreach (char c in Input.inputString)
            {
                switch (c)
                {
                    case '\b': // Backspace
                        if (text.text.Length != 0)
                        {
                            text.text = text.text.Substring(0, text.text.Length - 1);
                        }

                        break;
                    case '\n': // Newline
                    case '\r': // Return
                        text.text += @"
";
                        break;
                    default:
                        text.text += c.ToString();
                        break;
                }
            }
        }
    }
}

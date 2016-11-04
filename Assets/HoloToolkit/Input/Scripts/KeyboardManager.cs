// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// KeyboardManager allows other scripts to register for (or inject) key events.
    /// </summary>
    public class KeyboardManager : Singleton<KeyboardManager>
    {
        public enum KeyEvent
        {
            /// <summary>
            /// This event is sent once when a key is pressed.
            /// </summary>
            KeyDown = 0,

            /// <summary>
            /// This event is sent repeatedly while a key is held down.
            /// </summary>
            KeyHeld,

            /// <summary>
            /// This event is sent once when a key is released.
            /// </summary>
            KeyUp
        };

        /// <summary>
        /// Simple struct that holds a KeyCode and KeyEvent
        /// </summary>
        public struct KeyCodeEventPair
        {
            public KeyCode KeyCode;
            public KeyEvent KeyEvent;

            public KeyCodeEventPair(KeyCode keyCode, KeyEvent keyEvent)
            {
                this.KeyCode = keyCode;
                this.KeyEvent = keyEvent;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is KeyCodeEventPair))
                {
                    return false;
                }

                KeyCodeEventPair compare = (KeyCodeEventPair)obj;

                return KeyCode == compare.KeyCode && KeyEvent == compare.KeyEvent;
            }

            public override int GetHashCode()
            {
                return (KeyCode.GetHashCode() * 100) ^ KeyEvent.GetHashCode();
            }
        };

        /// <summary>
        /// KeyboardRegistration is returned by RegisterKeyEvent. Calling code should maintain a reference
        /// to the object while the registration should function, then call Dispose on it to unregister.
        /// </summary>
        public class KeyboardRegistration : IDisposable
        {
            private readonly KeyCodeEventPair keyCodeEvent;
            private readonly KeyboardCallback callback;
            private bool isRegistered;

            public KeyboardRegistration(KeyCodeEventPair keyCodeEvent, KeyboardCallback callback)
            {
                this.keyCodeEvent = keyCodeEvent;
                this.callback = callback;
                isRegistered = true;
            }

            public void Dispose()
            {
                if (isRegistered)
                {
                    var keyboard = KeyboardManager.Instance;
                    if (keyboard)
                    {
                        keyboard.UnregisterKeyEvent(keyCodeEvent, callback);
                    }
                    isRegistered = false;
                }
            }
        }

        /// <summary>
        /// Delegate that is called when a registered keyboard event is detected
        /// </summary>
        /// <param name="keyCodeEvent">The KeyCodeEventPair corresponding to the detected input</param>
        public delegate void KeyboardCallback(KeyCodeEventPair keyCodeEvent);

        /// <summary>
        /// A dictionary containing a list of callbacks for each active KeyCodeEventPair
        /// </summary>
        private Dictionary<KeyCodeEventPair, List<KeyboardCallback>> registeredCallbacks
            = new Dictionary<KeyCodeEventPair, List<KeyboardCallback>>();

        /// <summary>
        /// The detected input events. This is done to avoid callbacks interfering with the traversal of the dictionary.
        /// </summary>
        private List<KeyCodeEventPair> detectedKeyEvents = new List<KeyCodeEventPair>();

        /// <summary>
        /// The input events that are being processed. Only used by Update to avoid multithreading issues.
        /// </summary>
        private List<KeyCodeEventPair> pendingKeyEvents = new List<KeyCodeEventPair>();

        private void Update()
        {
            lock (detectedKeyEvents)
            {
                pendingKeyEvents.AddRange(detectedKeyEvents);
                detectedKeyEvents.Clear();
            }

            // Check for all keys that are registered for events
            foreach (KeyCodeEventPair keyCheck in registeredCallbacks.Keys)
            {
                bool eventTriggered = false;

                switch (keyCheck.KeyEvent)
                {
                    case KeyEvent.KeyHeld:
                        eventTriggered = Input.GetKey(keyCheck.KeyCode);
                        break;
                    case KeyEvent.KeyDown:
                        eventTriggered = Input.GetKeyDown(keyCheck.KeyCode);
                        break;
                    case KeyEvent.KeyUp:
                        eventTriggered = Input.GetKeyUp(keyCheck.KeyCode);
                        break;
                }

                if (eventTriggered)
                {
                    pendingKeyEvents.Add(keyCheck);
                }
            }

            for (int eventIndex = 0; eventIndex < pendingKeyEvents.Count; eventIndex++)
            {
                HandleKeyEvent(pendingKeyEvents[eventIndex]);
            }
            pendingKeyEvents.Clear();
        }

        /// <summary>
        /// Unregister a specified KeyCodeEventPair and KeyboardCallback.
        /// </summary>
        private void UnregisterKeyEvent(KeyCodeEventPair keyCodeEvent, KeyboardCallback callback)
        {
            if (registeredCallbacks.ContainsKey(keyCodeEvent))
            {
                List<KeyboardCallback> callbackList = registeredCallbacks[keyCodeEvent];

                if (callbackList.Remove(callback))
                {
                    // remove the list from the dictionary if no callbacks are left
                    if (callbackList.Count == 0)
                    {
                        registeredCallbacks.Remove(keyCodeEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Invoke any registered callbacks for the specified KeyCodeEventPair input.
        /// </summary>
        private void HandleKeyEvent(KeyCodeEventPair keyEventPair)
        {
            List<KeyboardCallback> callbackList;

            if (registeredCallbacks.TryGetValue(keyEventPair, out callbackList))
            {
                // Create a copy of the list in case a listener unregisters.
                KeyboardCallback[] callbacksCopy = callbackList.ToArray();
                foreach (KeyboardCallback callback in callbacksCopy)
                {
                    callback(keyEventPair);
                }
            }
        }

        #region Public Functions
        /// <summary>
        /// Register to get a callback whenever the specified KeyCodeEventPair input is detected
        /// </summary>
        public KeyboardRegistration RegisterKeyEvent(KeyCodeEventPair keycodeEvent, KeyboardCallback callback)
        {
            if (!registeredCallbacks.ContainsKey(keycodeEvent))
            {
                registeredCallbacks.Add(keycodeEvent, new List<KeyboardCallback>());
            }

            // Don't register the same callback more than once
            List<KeyboardCallback> callbackList = registeredCallbacks[keycodeEvent];
            for (int i = 0; i < callbackList.Count; i++)
            {
                if (callbackList[i] == callback)
                {
                    // Duplicate
                    Debug.LogError("Ignoring duplicate keyboard callback.");
                    return null;
                }
            }

            callbackList.Add(callback);

            // return a registration object, which must be referenced until it's disposed to unregister
            return new KeyboardRegistration(keycodeEvent, callback);
        }

        /// <summary>
        /// Queue an artificial keyboard event to be handled on the next Update.
        /// (This can be called from another thread.)
        /// </summary>
        public void InjectKeyboardEvent(KeyCodeEventPair keycodeEvent)
        {
            lock (detectedKeyEvents)
            {
                detectedKeyEvents.Add(keycodeEvent);
            }
        }
        #endregion
    }
}
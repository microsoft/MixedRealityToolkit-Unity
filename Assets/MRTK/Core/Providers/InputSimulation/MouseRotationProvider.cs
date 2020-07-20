// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility class to manage toggling of mouse rotation and associated features,
    /// such as cursor visibility/locking
    /// </summary>
    public class MouseRotationProvider
    {
        private bool isRotating = false;
        /// <summary>
        /// True when rotation is currently active.
        /// </summary>
        public bool IsRotating => isRotating;

        // Refcount to ensure the cursor is locked iff at least one rotation is in progress.
        private static int numRotating = 0;
        private static bool isMouseJumping = false;
        private static bool wasCursorVisible = true;

        /// <summary>
        /// Start or stop rotation based on the key binding.
        /// </summary>
        /// <remarks>
        /// Also manages shared features such as cursor visibility that can be activated by different rotation providers.
        /// </remarks>
        public void Update(KeyBinding rotationKey, KeyBinding cancelRotationKey, bool toggle)
        {
            bool wasRotating = isRotating;

            // Only allow the mouse to control rotation when Unity has focus.
            // This enables the player to temporarily alt-tab away without having the player
            // look around randomly back in the Unity Game window.
            if (!Application.isFocused)
            {
                isRotating = false;
            }
            else
            {
                if (toggle)
                {
                    if (isRotating)
                    {
                        // Pressing escape will stop capture
                        isRotating = !KeyInputSystem.GetKeyDown(cancelRotationKey);
                    }
                    else
                    {
                        // Capture focus when starting rotation
                        isRotating = KeyInputSystem.GetKeyDown(rotationKey);
                    }
                }
                else
                {
                    // Rotate only while key is pressed
                    isRotating = KeyInputSystem.GetKey(rotationKey);
                }
            }

            if (!wasRotating && isRotating)
            {
                OnStartRotating(rotationKey);
            }
            else if (wasRotating && !isRotating)
            {
                OnEndRotating(rotationKey);
            }
        }

        private static void OnStartRotating(KeyBinding rotationKey)
        {
            if (numRotating == 0)
            {
                if (rotationKey.BindingType == KeyBinding.KeyType.Mouse)
                {
                    // Enable jumping when a mouse button is used
                    SetWantsMouseJumping(true);
                }
                else if (rotationKey.BindingType == KeyBinding.KeyType.Key)
                {
                    // Use cursor locking when using a key
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    // save current cursor visibility before hiding it
                    wasCursorVisible = UnityEngine.Cursor.visible;
                    UnityEngine.Cursor.visible = false;
                }
            }

            ++numRotating;
        }

        private static void OnEndRotating(KeyBinding rotationKey)
        {
            --numRotating;

            if (numRotating == 0)
            {
                if (rotationKey.BindingType == KeyBinding.KeyType.Mouse)
                {
                    // Enable jumping when a mouse button is used
                    SetWantsMouseJumping(false);
                }
                else if (rotationKey.BindingType == KeyBinding.KeyType.Key)
                {
                    // Use cursor locking when using a key
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                    UnityEngine.Cursor.visible = wasCursorVisible;
                }
            }
        }

        /// <summary>
        /// Mouse jumping is where the mouse cursor appears outside the Unity game window, but
        /// disappears when it enters the Unity game window.
        /// </summary>
        /// <param name="wantsJumping">Show the cursor</param>
        private static void SetWantsMouseJumping(bool wantsJumping)
        {
            if (wantsJumping != isMouseJumping)
            {
                isMouseJumping = wantsJumping;

                if (wantsJumping)
                {
                    // unlock the cursor if it was locked
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // save original state of cursor before hiding
                    wasCursorVisible = UnityEngine.Cursor.visible;
                    // hide the cursor
                    UnityEngine.Cursor.visible = false;
                }
                else
                {
                    // recenter the cursor (setting lockCursor has side-effects under the hood)
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;

                    // restore the cursor
                    UnityEngine.Cursor.visible = wasCursorVisible;
                }

#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.SetWantsMouseJumping(wantsJumping ? 1 : 0);
#endif
            }
        }
    }
}
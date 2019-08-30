// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility class to manage toggling of mouse rotation and associated features,
    /// such as cursor visibility/locking
    /// </summary>
    /// <param name="wantsJumping">Show the cursor</param>
    internal class MouseRotationProvider
    {
        private bool isRotating = false;
        public bool IsRotating => isRotating;

        private bool isMouseJumping = false;
        private bool wasCursorVisible = true;

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
                        // pressing escape will stop capture
                        isRotating = !KeyInputSystem.GetKeyDown(cancelRotationKey);
                    }
                    else
                    {
                        // any kind of click will capture focus
                        isRotating = KeyInputSystem.GetKeyDown(rotationKey);
                    }
                }
                else
                {
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

        private void OnStartRotating(KeyBinding rotationKey)
        {
            if (rotationKey.BindingType == KeyBinding.KeyType.Mouse)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(true);
            }
            else if (rotationKey.BindingType == KeyBinding.KeyType.Key)
            {
                // if mousebutton is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                // save current cursor visibility before hiding it
                wasCursorVisible = UnityEngine.Cursor.visible;
                UnityEngine.Cursor.visible = false;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        private void OnEndRotating(KeyBinding rotationKey)
        {
            if (rotationKey.BindingType == KeyBinding.KeyType.Mouse)
            {
                // if mousebutton is either left, right or middle
                SetWantsMouseJumping(false);
            }
            else if (rotationKey.BindingType == KeyBinding.KeyType.Key)
            {
                // if mousebutton is either control, shift or focused
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = wasCursorVisible;
            }

            // do nothing if (this.MouseLookButton == MouseButton.None)
        }

        /// <summary>
        /// Mouse jumping is where the mouse cursor appears outside the Unity game window, but
        /// disappears when it enters the Unity game window.
        /// </summary>
        /// <param name="wantsJumping">Show the cursor</param>
        private void SetWantsMouseJumping(bool wantsJumping)
        {
            if (wantsJumping != this.isMouseJumping)
            {
                this.isMouseJumping = wantsJumping;

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
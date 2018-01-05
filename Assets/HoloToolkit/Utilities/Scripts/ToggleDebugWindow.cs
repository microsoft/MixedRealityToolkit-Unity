// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Toggles if the debug window is visible or not.
    /// </summary>
    public class ToggleDebugWindow : MonoBehaviour, IPointerHandler
    {
        /// <summary>
        /// Current state of the debug window.
        /// </summary>
        private bool debugEnabled = false;

        /// <summary>
        /// The debug window.
        /// </summary>
        public GameObject DebugWindow;

        private void Start()
        {
            UpdateChildren();
        }

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData) { }

        /// <summary>
        /// When the user clicks this control, we toggle the state of the DebugWindow
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClicked(PointerEventData eventData)
        {
            debugEnabled = !debugEnabled;
            UpdateChildren();
            eventData.Use();
        }

        /// <summary>
        /// Sets the debugwindow's active flag to debugEnabled.
        /// </summary>
        private void UpdateChildren()
        {
            DebugWindow.SetActive(debugEnabled);
        }
    }
}
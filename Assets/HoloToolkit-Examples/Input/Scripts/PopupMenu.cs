// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class PopupMenu : MonoBehaviour
    {
        [SerializeField]
        private TestButton cancelButton = null;

        [SerializeField]
        private Animator rootAnimator = null;

        [SerializeField]
        private bool isModal = false;

        [SerializeField]
        private bool closeOnNonTargetedTap = false;

        /// <summary>
        /// Called when 'place' is selected.
        /// </summary>
        private Action activatedCallback;

        /// <summary>
        /// Called when 'back' or 'hide' is selected.
        /// </summary>
        private Action cancelledCallback;

        /// <summary>
        /// Called when the user clicks outside of the menu.
        /// </summary>
        private Action deactivatedCallback;

        private int dehydrateButtonId;

        public PopupState CurrentPopupState = PopupState.Closed;

        public enum PopupState { Open, Closed }

        private void Awake()
        {
            gameObject.SetActive(false);

            if (dehydrateButtonId == 0)
            {
                dehydrateButtonId = Animator.StringToHash("Dehydrate");
            }
        }

        private void OnEnable()
        {
            if (cancelButton != null)
            {
                cancelButton.Activated += OnCancelPressed;
            }
        }

        private void OnDisable()
        {
            if (cancelButton != null)
            {
                cancelButton.Activated -= OnCancelPressed;
            }
        }

        public void Show(Action _activatedCallback = null, Action _cancelledCallback = null, Action _deactivatedCallback = null)
        {
            activatedCallback = _activatedCallback;
            cancelledCallback = _cancelledCallback;
            deactivatedCallback = _deactivatedCallback;

            gameObject.SetActive(true);
            CurrentPopupState = PopupState.Open;

            if (isModal)
            {
                InputManager.Instance.PushModalInputHandler(cancelButton.gameObject);
            }

            if (closeOnNonTargetedTap)
            {
                InputManager.Instance.PushFallbackInputHandler(cancelButton.gameObject);
            }

            // The visual was activated via an interaction outside of the menu. Let anyone who cares know.
            if (activatedCallback != null)
            {
                activatedCallback();
            }
        }

        /// <summary>
        /// Dismiss the details pane.
        /// </summary>
        public void Dismiss()
        {
            if (deactivatedCallback != null)
            {
                deactivatedCallback();
            }

            if (isModal)
            {
                InputManager.Instance.PopModalInputHandler();
            }

            if (closeOnNonTargetedTap)
            {
                InputManager.Instance.PopFallbackInputHandler();
            }

            CurrentPopupState = PopupState.Closed;

            activatedCallback = null;
            cancelledCallback = null;
            deactivatedCallback = null;

            if (cancelButton)
            {
                cancelButton.Selected = false;
            }

            // Deactivates the game object
            if (rootAnimator != null && rootAnimator.isInitialized)
            {
                rootAnimator.SetTrigger(dehydrateButtonId);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnCancelPressed(TestButton source)
        {
            if (cancelButton.Focused || closeOnNonTargetedTap)
            {
                if (cancelledCallback != null)
                {
                    cancelledCallback();
                }

                Dismiss();
            }
        }
    }
}
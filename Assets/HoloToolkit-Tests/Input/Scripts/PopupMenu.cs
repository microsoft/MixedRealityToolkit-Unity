// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class PopupMenu : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private TestButton cancelButton = null;

        [SerializeField]
        private Animator rootAnimator = null;

        [SerializeField]
        private bool isModal = false;

        [SerializeField]
        private bool closeOnNonTargetedTap = false;

        private Action activatedCallback;   // called when 'place' is selected
        private Action cancelledCallback;   // called when 'back' or 'hide' is selected
        private Action deactivatedCallback;   // called when the user clicks outside of the menu

        public PopupState CurrentPopupState = PopupState.Closed;

        public GameObject RootObject
        {
            get
            {
                return this.gameObject;
            }
        }

        public enum PopupState { Open, Closed }

        private void Awake()
        {
            gameObject.SetActive(false);
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
                InputManager.Instance.PushModalInputHandler(gameObject);
            }

            if (closeOnNonTargetedTap)
            {
                InputManager.Instance.PushFallbackInputHandler(gameObject);
            }

            // the visual was activated via an interaction outside of the menu, let anyone who cares know
            if (this.activatedCallback != null)
            {
                this.activatedCallback();
            }
        }

        /// <summary>
        /// Dismiss the details pane
        /// </summary>
        public void Dismiss()
        {
            // Deactivates the game object
            if (rootAnimator.isInitialized)
            {
                rootAnimator.SetTrigger("Dehydrate");
            }
            else
            {
                gameObject.SetActive(false);
            }

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
        }

        private void OnCancelPressed(TestButton source)
        {
            if (cancelledCallback != null)
            {
                cancelledCallback();
            }

            Dismiss();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (closeOnNonTargetedTap)
            {
                Dismiss();
            }
        }
    }
}
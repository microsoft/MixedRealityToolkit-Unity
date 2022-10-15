// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    public enum DialogButtonType
    {
        Negative = 0,
        Positive = 1,
        Neutral = 2
    }

    /// <summary>
    /// The Dialog script hydrates and controls the various sub-components
    /// of the dialog view.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("MRTK/UX/Dialog")]
    public class Dialog : MonoBehaviour
    {
        #region View components

        [SerializeField]
        [Tooltip("The header of the dialog. Usually rendered in larger font " +
                 "and on its own line, for emphasis.")]
        private TMP_Text headerText;

        [SerializeField]
        [Tooltip("The body of the dialog. Can be multiline, and the dialog will " +
                 "automatically size itself to fit the text.")]
        private TMP_Text bodyText;

        [SerializeField]
        private DialogButton positiveButton = new DialogButton();

        [SerializeField]
        private DialogButton negativeButton = new DialogButton();

        [SerializeField]
        private DialogButton neutralButton = new DialogButton();
        
        #endregion

        #region Viewmodel

        private string header = null;

        private string body = null;

        private DialogButtonEvent positiveAction = null;

        private DialogButtonEvent negativeAction = null;

        private DialogButtonEvent neutralAction = null;

        private UnityEvent<Dialog> onDismissed = new UnityEvent<Dialog>();

        public UnityEvent<Dialog> OnDismissed => onDismissed;

        // public bool IsVisible => isVisible;

        #endregion

        public Dialog SetHeader(string header)
        {
            this.header = header;
            return this;
        }

        public Dialog SetBody(string body)
        {
            this.body = body;
            return this;
        }

        public Dialog SetPositive(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            positiveButton.Label.text = label;
            positiveAction = new DialogButtonEvent();
            positiveAction.AddListener(action);
            return this;
        }

        public Dialog SetNegative(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            negativeButton.Label.text = label;
            negativeAction = new DialogButtonEvent();
            negativeAction.AddListener(action);
            return this;
        }

        public Dialog SetNeutral(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            neutralButton.Label.text = label;
            neutralAction = new DialogButtonEvent();
            neutralAction.AddListener(action);
            return this;
        }

        protected virtual void Awake()
        {
            if (negativeButton.Interactable != null)
            {
                negativeButton.Interactable.OnClicked.AddListener( () => {
                    negativeAction.Invoke(new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Negative,
                        ButtonText = negativeButton.Label.text,
                        Dialog = this
                    });
                    Dismiss();
                });
            }

            if (positiveButton.Interactable != null)
            {
                positiveButton.Interactable.OnClicked.AddListener( () => {
                    positiveAction.Invoke(new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Positive,
                        ButtonText = positiveButton.Label.text,
                        Dialog = this
                    });
                    Dismiss();
                });
            }

            if (neutralButton.Interactable != null)
            {
                neutralButton.Interactable.OnClicked.AddListener( () => {
                    neutralAction.Invoke(new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Neutral,
                        ButtonText = neutralButton.Label.text,
                        Dialog = this
                    });
                    Dismiss();
                });
            }
        }

        public void Show()
        {
            headerText.gameObject.SetActive(header != null);
            headerText.text = header;
            bodyText.gameObject.SetActive(body != null);
            bodyText.text = body;

            positiveButton.Interactable.gameObject.SetActive(positiveAction != null);
            negativeButton.Interactable.gameObject.SetActive(negativeAction != null);
            neutralButton.Interactable.gameObject.SetActive(neutralAction != null);

            gameObject.SetActive(true);
        }

        public void Dismiss()
        {
            gameObject.SetActive(false);
            negativeAction?.RemoveAllListeners();
            positiveAction?.RemoveAllListeners();
            neutralAction?.RemoveAllListeners();
            onDismissed.Invoke(this);
        }
    }

    [Serializable]
    internal struct DialogButton
    {
        public StatefulInteractable Interactable;

        public TMP_Text Label;

    }
}
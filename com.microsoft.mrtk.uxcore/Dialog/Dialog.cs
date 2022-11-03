// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

#if MRTK_SPATIAL_PRESENT
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
#endif

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

        /// <summary>
        /// Clears all content, events, and configuration from the dialog.
        /// Useful when pooling Dialog objects, to ensure that subsequent
        /// uses of the object don't retain stale data.
        /// </summary>
        /// <remarks>
        /// When implementing custom dialog types, be sure to override
        /// this method to clear any custom state or fields.
        /// </remarks>
        public virtual void Reset()
        {
            header = null;
            body = null;
            positiveAction?.RemoveAllListeners();
            positiveAction = null;
            negativeAction?.RemoveAllListeners();
            negativeAction = null;
            neutralAction?.RemoveAllListeners();
            neutralAction = null;
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

#if MRTK_SPATIAL_PRESENT
            Follow followSolver = gameObject.AddComponent<Follow>();
#endif

        }

        
    

        public virtual void Show()
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

        /// <summary>
        /// Dismisses the dialog. Unsubscribes all listeners from the dialog's
        /// events, plays the dismiss animation, and then invokes onDismissed.
        /// </summary>
        /// <remarks>
        /// Those writing subclassed Dialogs should unsubscribe listeners from their custom
        /// events, if any, as well.
        /// </remarks>
        public virtual void Dismiss()
        {
            negativeAction?.RemoveAllListeners();
            positiveAction?.RemoveAllListeners();
            neutralAction?.RemoveAllListeners();
            
            // Only invoke the Dismissed callback after we've played the full dismissal animation.
            StartCoroutine(InvokeDismissalAfterAnimation());
        }

        protected IEnumerator InvokeDismissalAfterAnimation()
        {
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Dismiss");
            yield return null;

            // Spin while we're still animating.
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1 || animator.IsInTransition(0))
            {
                yield return null;
            }
            
            gameObject.SetActive(false);
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
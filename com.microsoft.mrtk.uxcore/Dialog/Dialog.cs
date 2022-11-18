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
    /// <summary>
    /// The Dialog script hydrates and controls the various sub-components
    /// of the dialog view.
    /// </summary>
    [AddComponentMenu("MRTK/UX/Dialog")]
    public class Dialog : MonoBehaviour, IDialog
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
        [Tooltip("The button representing the positive action. If specified by the user, " +
                 "the button will be enabled and activated, with actions hooked up through code.")]
        private DialogButton positiveButton = new DialogButton();

        [SerializeField]
        [Tooltip("The button representing the negative action. If specified by the user, " +
                 "the button will be enabled and activated, with actions hooked up through code.")]
        private DialogButton negativeButton = new DialogButton();

        [SerializeField]
        [Tooltip("The button representing the neutral action. If specified by the user, " +
                 "the button will be enabled and activated, with actions hooked up through code.")]
        private DialogButton neutralButton = new DialogButton();
        
        #endregion

        #region Viewmodel

        private string header = null;

        private string body = null;

        private DialogButtonEvent positiveAction = null;

        private DialogButtonEvent negativeAction = null;

        private DialogButtonEvent neutralAction = null;

        private UnityEvent<Dialog> onDismissed = new UnityEvent<Dialog>();

        /// <inheritdoc />
        public UnityEvent<Dialog> OnDismissed => onDismissed;

        #endregion

        /// <inheritdoc />
        public Dialog SetHeader(string header)
        {
            this.header = header;
            return this;
        }

        /// <inheritdoc />
        public Dialog SetBody(string body)
        {
            this.body = body;
            return this;
        }
        
        /// <inheritdoc />
        public Dialog SetPositive(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            positiveButton.Label.text = label;
            positiveAction = new DialogButtonEvent();
            positiveAction.AddListener(action);
            return this;
        }

        /// <inheritdoc />
        public Dialog SetNegative(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            negativeButton.Label.text = label;
            negativeAction = new DialogButtonEvent();
            negativeAction.AddListener(action);
            return this;
        }

        /// <inheritdoc />
        public Dialog SetNeutral(string label, UnityAction<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            neutralButton.Label.text = label;
            neutralAction = new DialogButtonEvent();
            neutralAction.AddListener(action);
            return this;
        }

        /// <inheritdoc />
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

        /// <summary>
        /// Adds the listeners/actions to the buttons that have been
        /// specified/added to the dialog.
        /// </summary>
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
            if (gameObject.GetComponent<Follow>() == null)
            {
                Follow followSolver = gameObject.AddComponent<Follow>();
                followSolver.Smoothing = true;
                followSolver.MoveLerpTime = 1.0f;
                followSolver.RotateLerpTime = 1.0f;
                followSolver.OrientToControllerDeadzoneDegrees = 25.0f;
            }
#endif

        }
        
        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual void Dismiss()
        {
            negativeAction?.RemoveAllListeners();
            positiveAction?.RemoveAllListeners();
            neutralAction?.RemoveAllListeners();
            
            // Only invoke the Dismissed callback after we've played the full dismissal animation.
            StartCoroutine(InvokeDismissalAfterAnimation());
        }

        /// <inheritdoc />
        public GameObject VisibleRoot => gameObject;

        /// <summary>
        /// Coroutine to set the animation trigger, wait for the animation to finish,
        /// and then hide the dialog and invoke the dismissal action. This coroutine
        /// is started by the base <see cref="Dismiss"/> method once all listeners
        /// have been removed.
        private IEnumerator InvokeDismissalAfterAnimation()
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
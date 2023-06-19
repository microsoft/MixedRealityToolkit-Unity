// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <remarks>
    /// Dialogs are typically spawned, pooled, and killed
    /// by <see cref="Microsoft.MixedReality.Toolkit.UX.DialogPool">DialogPools</see>. 
    /// Generally, developers should not directly manage or instantiate instances of their dialogs,
    /// as it is essential that they are pooled and managed correctly by a pooler.
    /// </remarks>
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

        private Action<DialogButtonEventArgs> positiveAction = null;

        private Action<DialogButtonEventArgs> negativeAction = null;

        private Action<DialogButtonEventArgs> neutralAction = null;

        private Action<DialogDismissedEventArgs> onDismissed;

        /// <inheritdoc />
        public Action<DialogDismissedEventArgs> OnDismissed
        {
            get => onDismissed;
            set => onDismissed = value;
        }

        #endregion

        #region Private helper fields

        private bool hasDismissed = false;

        // If the dialog has had some choice made on it,
        // that choice will be recorded here.
        private DialogButtonEventArgs buttonClickedArgs = null;

        #endregion

        /// <inheritdoc />
        public IDialog SetHeader(string header)
        {
            this.header = header;
            return this;
        }

        /// <inheritdoc />
        public IDialog SetBody(string body)
        {
            this.body = body;
            return this;
        }
        
        /// <inheritdoc />
        public IDialog SetPositive(string label, Action<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            positiveButton.Label.text = label;
            positiveAction = action ?? ((args) => {});
            return this;
        }

        /// <inheritdoc />
        public IDialog SetNegative(string label, Action<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            negativeButton.Label.text = label;
            negativeAction = action ?? ((args) => {});
            return this;
        }

        /// <inheritdoc />
        public IDialog SetNeutral(string label, Action<DialogButtonEventArgs> action)
        {
            if (label == null) { return this; }
            neutralButton.Label.text = label;
            neutralAction = action ?? ((args) => {});
            return this;
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            header = null;
            body = null;
            positiveAction = null;
            negativeAction = null;
            neutralAction = null;
            buttonClickedArgs = null;
            OnDismissed = null;

            hasDismissed = false;
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
                    var args = new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Negative,
                        ButtonText = negativeButton.Label.text,
                        Dialog = this
                    };
                    buttonClickedArgs = args;
                    negativeAction.Invoke(args);
                    Dismiss();
                });
            }

            if (positiveButton.Interactable != null)
            {
                positiveButton.Interactable.OnClicked.AddListener( () => {
                    var args = new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Positive,
                        ButtonText = positiveButton.Label.text,
                        Dialog = this
                    };
                    buttonClickedArgs = args;
                    positiveAction.Invoke(args);
                    Dismiss();
                });
            }

            if (neutralButton.Interactable != null)
            {
                neutralButton.Interactable.OnClicked.AddListener( () => {
                    var args = new DialogButtonEventArgs() {
                        ButtonType = DialogButtonType.Neutral,
                        ButtonText = neutralButton.Label.text,
                        Dialog = this
                    };
                    buttonClickedArgs = args;
                    neutralAction.Invoke(args);
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
        public virtual IDialog Show()
        {
            headerText.gameObject.SetActive(header != null);
            headerText.text = header;
            bodyText.gameObject.SetActive(body != null);
            bodyText.text = body;

            positiveButton.Interactable.gameObject.SetActive(positiveAction != null);
            negativeButton.Interactable.gameObject.SetActive(negativeAction != null);
            neutralButton.Interactable.gameObject.SetActive(neutralAction != null);

            gameObject.SetActive(true);

            return this;
        }

        public virtual async Task<DialogDismissedEventArgs> ShowAsync()
        {
            Show();
            var tcs = new TaskCompletionSource<DialogDismissedEventArgs>();
            OnDismissed += (args) => {
                tcs.SetResult(args);
            };
            
            return await tcs.Task;
        }

        /// <inheritdoc />
        public virtual void Dismiss()
        {
            negativeAction = null;
            positiveAction = null;
            neutralAction = null;
            
            // Lock. Dismissal idempotent.
            if (hasDismissed) { return; }
            hasDismissed = true;
            
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
            onDismissed(new DialogDismissedEventArgs()
            {
                Dialog = this,
                Choice = buttonClickedArgs
            });
        }
    }

    [Serializable]
    internal struct DialogButton
    {
        public StatefulInteractable Interactable;

        public TMP_Text Label;

    }
}
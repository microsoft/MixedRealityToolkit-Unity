// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Receivers;
using System;
using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.UX.Dialog
{
    /// <summary>
    /// Used to tell simple dialogs which buttons to create
    /// And to tell whatever launched the dialog which button was pressed
    /// Can be extended to include more information for dialog construction
    /// (eg detailed messages, button names, colors etc)
    public abstract class Dialog : InteractionReceiver
    {
        public enum StateEnum
        {
            Uninitialized,
            Opening,
            WaitingForInput,
            InputReceived,
            Closing,
            Closed,
        }

        [Flags]
        public enum ButtonTypeEnum
        {
            None = 0,
            Close = 1,
            Confirm = 2,
            Cancel = 4,
            Accept = 8,
            Yes = 16,
            No = 32,
            OK = 64,
        }

        /// <summary>
        /// Where the instantiated buttons will be placed
        /// </summary>
        [SerializeField]
        protected Transform buttonParent;

        /// <summary>
        /// Current state of the dialog
        /// Can be used to monitor state in place of events
        /// </summary>
        public StateEnum State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        /// <summary>
        /// Called after user has clicked a button and the dialog has finished closing
        /// </summary>
        public Action<DialogResult> OnClosed;

        /// <summary>
        /// Can be used to monitor result instead of events
        /// </summary>
        public DialogResult Result
        {
            get
            {
                return result;
            }
        }

        protected void Launch(DialogResult newResult)
        {
            if (state != StateEnum.Uninitialized)
            {
                return;
            }

            result = newResult;
            StartCoroutine(RunDialogOverTime());
        }

        /// <summary>
        /// Opens dialog, waits for input, then closes
        /// </summary>
        /// <returns></returns>
        protected IEnumerator RunDialogOverTime()
        {  
            // Create our buttons and set up our message
            GenerateButtons();
            SetTitleAndMessage();
            FinalizeLayout();

            // Open dialog
            state = StateEnum.Opening;
            yield return StartCoroutine(OpenDialog());
            state = StateEnum.WaitingForInput;
            // Wait for input
            while (state == StateEnum.WaitingForInput)
            {
                UpdateDialog();
                yield return null;
            }
            // Close dialog
            state = StateEnum.Closing;
            yield return StartCoroutine(CloseDialog());
            state = StateEnum.Closed;
            // Callback
            if (OnClosed != null)
            {
                OnClosed(result);
            }
            // Wait a moment to give scripts a chance to respond
            yield return null;
            // Destroy ourselves
            GameObject.Destroy(gameObject);
            yield break;
        }

        /// <summary>
        /// Opens the dialog - state will be set to WaitingForInput afterwards
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OpenDialog()
        {
            yield break;
        }

        /// <summary>
        /// Closes the dialog - state must be set to Closed afterwards
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CloseDialog()
        {
            yield break;
        }

        /// <summary>
        /// Perform any updates (animation, tagalong, etc) here
        /// This will be called every frame while waiting for input
        /// </summary>
        protected virtual void UpdateDialog()
        {
            return;
        }

        /// <summary>
        /// Generates buttons - Must parent them under buttonParent!
        /// </summary>
        protected abstract void GenerateButtons();

        /// <summary>
        /// Lays out the buttons on the dialog
        /// Eg using an ObjectCollection
        /// </summary>
        protected abstract void FinalizeLayout();

        /// <summary>
        /// Set the title and message using the result
        /// Eg using TextMesh components 
        /// </summary>
        protected abstract void SetTitleAndMessage();

        protected DialogResult result;
        private StateEnum state = StateEnum.Uninitialized;

        /// <summary>
        /// Instantiates a dialog and passes it a result
        /// </summary>
        /// <param name="dialogPrefab"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Dialog Open(GameObject dialogPrefab, DialogResult result)
        {
            GameObject dialogGo = GameObject.Instantiate(dialogPrefab) as GameObject;
            Dialog dialog = dialogGo.GetComponent<Dialog>();

            dialog.Launch(result);
            return dialog;
        }

        /// <summary>
        /// Instantiates a dialog and passes a generated result
        /// </summary>
        /// <param name="dialogPrefab"></param>
        /// <param name="buttons"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Dialog Open(GameObject dialogPrefab, ButtonTypeEnum buttons, string title, string message)
        {
            GameObject dialogGameObject = GameObject.Instantiate(dialogPrefab) as GameObject;
            Dialog dialog = dialogGameObject.GetComponent<Dialog>();

            DialogResult result = new DialogResult
            {
                Buttons = buttons,
                Title = title,
                Message = message
            };
            
            dialog.Launch(result);
            return dialog;
        }
    }
}
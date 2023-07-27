// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for samples. While nice to have, this XML documentation is not required for samples.
#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// A helper component for creating a <see cref="DialogPool"/> and showing <see cref="IDialog"/> objects.
    /// </summary>
    public class InspectorDrivenDialog : MonoBehaviour
    {
        /// <summary>
        /// A struct that holds <see cref="IDialog"/> button actions and their labels.
        /// </summary>
        /// <remarks>
        /// This is used with the <see cref="InspectorDrivenDialog"/> helper class.
        /// </remarks>
        [Serializable]
        public struct Option
        {
            public string Text;
            public UnityEvent<DialogButtonEventArgs> Action;
        }

        [SerializeField]
        [Tooltip("The DialogPool to use when showing a dialog.")]
        private DialogPool dialogPool;

        /// <summary>
        /// The <see cref="DialogPool"/> to use when showing a dialog.
        /// </summary>
        public DialogPool DialogPool
        {
            get => dialogPool;
            set => dialogPool = value;
        }


        [SerializeField]
        [Tooltip("The header of the dialog to show.")]
        private string header;

        /// <summary>
        /// The header of the dialog to show.
        /// </summary>
        public string Header
        {
            get => header;
            set => header = value;
        }

        [SerializeField]
        [TextArea]
        [Tooltip("The body of the dialog to show.")]
        private string body;

        /// <summary>
        /// The body of the dialog to show.
        /// </summary>
        public string Body
        {
            get => body;
            set => body = value;
        }

        [SerializeField]
        [Tooltip("The negative button option of the dialog to show.")]
        private Option negative;

        /// <summary>
        /// The negative button option of the dialog to show.
        /// </summary>
        public Option Negative
        {
            get => negative;
            set => negative = value;
        }

        [SerializeField]
        [Tooltip("The positive button option of the dialog to show.")]
        private Option positive;

        /// <summary>
        /// The positive button option of the dialog to show.
        /// </summary>
        public Option Positive
        {
            get => positive;
            set => positive = value;
        }

        [SerializeField]
        [Tooltip("The neutral button option of the dialog to show.")]
        private Option neutral;

        /// <summary>
        /// The neutral button option of the dialog to show.
        /// </summary>
        public Option Neutral
        {
            get => neutral;
            set => neutral = value;
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (DialogPool == null)
            {
                DialogPool = GetComponent<DialogPool>();
            }
        }

        /// <summary>
        /// Show a <see cref="IDialog"/> object using the currently set text and button options.
        /// </summary>
        public void Show()
        {
            // Any fields left blank in the inspector are considered
            // to be null, and we should pass that on to the dialog.
            header = header != "" ? header : null;
            body = body != "" ? body : null;
            negative.Text = negative.Text != "" ? negative.Text : null;
            positive.Text = positive.Text != "" ? positive.Text : null;
            neutral.Text = neutral.Text != "" ? neutral.Text : null;

            // Build and show the dialog.
            DialogPool.Get()
                .SetHeader(header)
                .SetBody(body)
                .SetNegative(negative.Text, ( args ) => negative.Action.Invoke(args))
                .SetPositive(positive.Text, ( args ) => positive.Action.Invoke(args))
                .SetNeutral(neutral.Text, ( args ) => neutral.Action.Invoke(args))
                .Show();
        }
    }
}
#pragma warning restore CS1591

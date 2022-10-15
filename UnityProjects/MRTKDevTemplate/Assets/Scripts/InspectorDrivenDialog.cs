// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UX
{
    public class InspectorDrivenDialog : MonoBehaviour
    {
        // A handy struct for keeping button actions and their labels together.
        [Serializable]
        internal struct Option
        {
            public string Text;
            public DialogButtonEvent Action;
        }
        
        [SerializeField]
        private DialogSpawner dialogSpawner;

        [SerializeField]
        private string header;

        [SerializeField]
        [TextArea]
        private string body;

        [SerializeField]
        private Option negative;

        [SerializeField]
        private Option positive;

        [SerializeField]
        private Option neutral;

        protected virtual void OnEnable()
        {
            if (dialogSpawner == null)
            {
                dialogSpawner = GetComponent<DialogSpawner>();
            }
        }

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
            dialogSpawner.Build()
                .SetHeader(header)
                .SetBody(body)
                .SetNegative(negative.Text, ( args ) => negative.Action.Invoke(args))
                .SetPositive(positive.Text, ( args ) => positive.Action.Invoke(args))
                .SetNeutral(neutral.Text, ( args ) => neutral.Action.Invoke(args))
                .Show();
        }
    }
}

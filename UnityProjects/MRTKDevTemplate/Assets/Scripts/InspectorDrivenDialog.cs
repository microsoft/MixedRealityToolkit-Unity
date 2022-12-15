// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UX;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class InspectorDrivenDialog : MonoBehaviour
    {
        // A handy struct for keeping button actions and their labels together.
        [Serializable]
        internal struct Option
        {
            public string Text;
            public UnityEvent<DialogButtonEventArgs> Action;
        }
        
        [SerializeField]
        private DialogPool DialogPool;

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
            if (DialogPool == null)
            {
                DialogPool = GetComponent<DialogPool>();
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

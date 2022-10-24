// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Abstract class representing a key in the non native keyboard
    /// </summary>
    public abstract class NonNativeKey : MonoBehaviour
    {
        /// <summary>
        /// Reference to the GameObject's button component.
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the GameObject's button component.")]
        protected PressableButton Button { get; set; }

        protected virtual void Awake()
        {
            if (Button == null)
            {
                Button = GetComponent<PressableButton>();
            }
            Button.OnClicked.AddListener(FireKey);
        }

        /// <summary>
        /// Function executed when the key is pressed.
        /// </summary>
        protected abstract void FireKey();
    }
}

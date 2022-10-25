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
        /// Reference to the GameObject's interactable component.
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the GameObject's interactable component.")]
        protected StatefulInteractable Interactable { get; set; }

        protected virtual void Awake()
        {
            if (Interactable == null)
            {
                Interactable = GetComponent<StatefulInteractable>();
            }
            Interactable.OnClicked.AddListener(FireKey);
        }

        /// <summary>
        /// Function executed when the key is pressed.
        /// </summary>
        protected abstract void FireKey();
    }
}

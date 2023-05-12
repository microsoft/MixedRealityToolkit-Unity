// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.UI;

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

        /// <summary>
        /// Reference to the GameObject's button component. Used if there is no StatefulInteractable.
        /// </summary>
        [field: SerializeField, Tooltip("Reference to the GameObject's button component. Used if there is no StatefulInteractable.")]
        protected Button KeyButton { get; set; }

        protected virtual void Awake()
        {
            if (Interactable == null)
            {
                Interactable = GetComponent<StatefulInteractable>();
            }

            // If there is a StatefulInteractable, that is used to trigger the FireKey event. Otherwise the Button is used.
            if (Interactable != null)
            {
                Interactable.OnClicked.AddListener(FireKey);
            }
            else
            {
                if (KeyButton == null)
                {
                    KeyButton = GetComponent<Button>();
                }
                if (KeyButton != null)
                {
                    KeyButton.onClick.AddListener(FireKey);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (Interactable != null)
            {
                Interactable.OnClicked.RemoveListener(FireKey);
            }
            else if (KeyButton != null)
            {
                KeyButton.onClick.RemoveListener(FireKey);
            }
        }

        /// <summary>
        /// Function executed when the key is pressed.
        /// </summary>
        protected abstract void FireKey();
    }
}

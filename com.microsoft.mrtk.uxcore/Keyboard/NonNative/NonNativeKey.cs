// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that is ideal for AR/VR applications that do not provide a native keyboard.
    /// </summary>
    public abstract class NonNativeKey : MonoBehaviour
    {
        /// <summary>
        /// Reference to the GameObject's button component.
        /// </summary>
        [field: SerializeField]
        protected PressableButton button { get; set; }

        /// <summary>
        /// Get the button component.
        /// </summary>
        protected virtual void Awake()
        {
            if (button == null)
            {
                button = GetComponent<PressableButton>();
            }
            button.OnClicked.AddListener(FireKey);
        }

        protected abstract void FireKey();
    }
}

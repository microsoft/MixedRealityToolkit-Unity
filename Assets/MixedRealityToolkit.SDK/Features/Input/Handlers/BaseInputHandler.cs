// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for the Mixed Reality Toolkit's SDK input handlers.
    /// </summary>
    public abstract class BaseInputHandler : InputSystemGlobalHandlerListener
    {
        [SerializeField]
        [Tooltip("Is Focus required to receive input events on this GameObject?")]
        private bool isFocusRequired = true;

        private bool initializationDone = false;

        /// <summary>
        /// Is Focus required to receive input events on this GameObject?
        /// </summary>
        public virtual bool IsFocusRequired
        {
            get { return isFocusRequired; }
            protected set
            {
                bool oldValue = isFocusRequired;
                isFocusRequired = value;
                UpdateHandlersState(oldValue);
            }
        }

        #region MonoBehaviour Implementation

        protected override void OnEnable()
        {
            if (!isFocusRequired)
            {
                base.OnEnable();
            }
        }

        protected override void Start()
        {
            if (!isFocusRequired)
            {
                base.Start();
            }

            initializationDone = true;
        }

        protected override void OnDisable()
        {
            if (!isFocusRequired)
            {
                base.OnDisable();
            }
        }

        #endregion MonoBehaviour Implementation

        private void UpdateHandlersState(bool oldValue)
        {
            if (oldValue == isFocusRequired)
            {
                return;
            }

            if (initializationDone && Application.isPlaying && isActiveAndEnabled)
            {
                // If focus wasn't required before and is required now, unregister global handlers.
                // Otherwise, register them.
                if (isFocusRequired)
                {
                    UnregisterHandlers();
                }
                else
                {
                    RegisterHandlers();
                }
            }
        }
    }
}

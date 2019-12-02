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

        // Helper variable used to register/unregister handlers during play mode
        private bool isFocusRequiredRuntime = true;

        /// <summary>
        /// Is Focus required to receive input events on this GameObject?
        /// </summary>
        public virtual bool IsFocusRequired
        {
            get { return isFocusRequired; }
            protected set { isFocusRequired = value; }
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

            isFocusRequiredRuntime = isFocusRequired;
        }

        protected virtual void Update()
        {
            if(isFocusRequiredRuntime != isFocusRequired)
            {
                isFocusRequiredRuntime = isFocusRequired;

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

        protected override void OnDisable()
        {
            if (!isFocusRequiredRuntime)
            {
                base.OnDisable();
            }
        }

        #endregion MonoBehaviour Implementation
    }
}

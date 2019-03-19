// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for the Mixed Reality Toolkit's SDK input handlers.
    /// </summary>
    public abstract class BaseInputHandler : InputSystemGlobalListener
    {
        [SerializeField]
        [Tooltip("Is Focus required to receive input events on this GameObject?")]
        private bool isFocusRequired = true;

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
        }

        protected override void OnDisable()
        {
            if (!isFocusRequired)
            {
                base.OnDisable();
            }
        }

        #endregion MonoBehaviour Implementation
    }
}

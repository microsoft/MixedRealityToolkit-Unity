// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Input
{
    public class BaseInputHandler : InputSystemGlobalListener
    {
        [SerializeField]
        [Tooltip("Is Focus required to receive input events on this GameObject?")]
        private bool isFocusRequired = false;

        /// <summary>
        /// Is Focus required to receive input events on this GameObject?
        /// </summary>
        public bool IsFocusRequired
        {
            get { return isFocusRequired; }
            set { isFocusRequired = value; }
        }

        #region Monobehaviour Implementation

        protected override void OnEnable()
        {
            if (!isFocusRequired)
            {
                base.OnEnable();
            }
        }

        protected override void OnDisable()
        {
            if (!isFocusRequired)
            {
                base.OnDisable();
            }
        }

        #endregion Monobehaviour Implementation
    }
}

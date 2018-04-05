// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseManager : Interfaces.IManager
    {

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool enabled;

        /// <summary>
        /// 
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public virtual void Initialize() { }
    }
}

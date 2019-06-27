// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Use this component to provide information that manipulation handler may care about specific to this
    /// gameobject.
    /// 
    /// For example, you can use IgnoredByManipulationHandler field to tell manipulationhandler to ignore events from this object.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ManipulationHandlerContext : MonoBehaviour
    {
        [Tooltip("If any ancestor ManipulationHandler receives events originating from here, ignore them.")]
        [SerializeField]
        private bool ignoredByManipulationHandler;
        /// <summary>
        /// If any ancestor ManipulationHandler receives events originating from here, ignore them.
        /// </summary>
        public bool IgnoredByManipulationHandler
        {
            get
            {
                return ignoredByManipulationHandler;
            }
            set
            {
                ignoredByManipulationHandler = value;
            }
        }
    }
}

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Base abstract class for all Mixed Reality profile configurations. 
    /// Extends ScriptableObject and used as a property container to initialize MRTK services.
    /// </summary>
    [Serializable]
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private bool isCustomProfile = true;

        internal bool IsCustomProfile => isCustomProfile;

        /// <summary>
        /// Whether this is a default profile that should be loaded when MRTK is added to the scene
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool isDefaultProfile = false;

        internal bool IsDefaultProfile
        {
            get { return isDefaultProfile; }
            set { isDefaultProfile = value; }
        }
    }
}

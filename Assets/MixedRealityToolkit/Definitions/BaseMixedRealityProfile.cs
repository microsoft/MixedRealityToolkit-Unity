// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    }
}

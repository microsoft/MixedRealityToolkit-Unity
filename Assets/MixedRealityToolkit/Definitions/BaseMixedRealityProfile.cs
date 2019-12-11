// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private bool isCustomProfile = true;

        internal bool IsCustomProfile => isCustomProfile;
    }
}

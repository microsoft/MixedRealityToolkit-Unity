// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MRTKPrefix
{
    public abstract class BaseMixedRealityProfile : ScriptableObject
    {
        [SerializeField]
        private bool isCustomProfile = true;

        internal bool IsCustomProfile => isCustomProfile;
    }
}

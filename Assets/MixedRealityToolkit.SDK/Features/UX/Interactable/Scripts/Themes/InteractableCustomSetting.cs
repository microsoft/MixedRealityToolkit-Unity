// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A simple property with name value and type, used for serialization
    /// </summary>
    
    [System.Serializable]
    public class InteractableCustomSetting
    {
        public string Name;
        public InteractableThemePropertyValueTypes Type;
        public InteractableThemePropertyValue Value;
    }
}

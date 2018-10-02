// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// A InspectorField property definition and value.
    /// </summary>
    [System.Serializable]
    public struct PropertySetting
    {
        public InspectorField.FieldTypes Type;
        public string Label;
        public string Name;
        public string Tooltip;
        public object Value;
        public string[] Options;
    }
}

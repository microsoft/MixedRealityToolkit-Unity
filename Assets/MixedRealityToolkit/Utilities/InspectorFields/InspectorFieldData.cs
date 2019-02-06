// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields
{
    /// <summary>
    /// A reference to the InspectorField and cached info
    /// </summary>
    [System.Serializable]
    public struct InspectorFieldData
    {
        public InspectorField Attributes;
        public object Value;
        public string Name;
    }
}

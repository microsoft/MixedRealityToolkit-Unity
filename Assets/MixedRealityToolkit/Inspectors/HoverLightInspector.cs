// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightInspector : Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as HoverLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }
    }
}

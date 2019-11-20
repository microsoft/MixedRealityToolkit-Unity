// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(ProximityLight))]
    public class ProximityLightInspector : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as ProximityLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Settings.FarRadius);
        }
    }
}

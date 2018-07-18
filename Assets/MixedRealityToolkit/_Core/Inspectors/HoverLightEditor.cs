// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Utilities.UX;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightEditor : Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            HoverLight light = target as HoverLight;
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }
    }
}

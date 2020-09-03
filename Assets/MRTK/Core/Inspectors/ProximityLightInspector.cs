// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

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

        [MenuItem("GameObject/Light/Proximity Light")]
        private static void CreateProximityLight(MenuCommand menuCommand)
        {
            GameObject proximityLight = new GameObject("Proximity Light", typeof(ProximityLight));

            // Ensure the light gets re-parented to the active context.
            GameObjectUtility.SetParentAndAlign(proximityLight, menuCommand.context as GameObject);

            // Register the creation in the undo system.
            Undo.RegisterCreatedObjectUndo(proximityLight, "Create " + proximityLight.name);

            Selection.activeObject = proximityLight;
        }
    }
}

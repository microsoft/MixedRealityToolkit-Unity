// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(HoverLight))]
    public class HoverLightInspector : UnityEditor.Editor
    {
        private bool HasFrameBounds() { return true; }

        private Bounds OnGetFrameBounds()
        {
            var light = target as HoverLight;
            Debug.Assert(light != null);
            return new Bounds(light.transform.position, Vector3.one * light.Radius);
        }

        [MenuItem("GameObject/Light/Hover Light")]
        private static void CreateHoverLight(MenuCommand menuCommand)
        {
            GameObject hoverLight = new GameObject("Hover Light", typeof(HoverLight));

            // Ensure the light gets re-parented to the active context.
            GameObjectUtility.SetParentAndAlign(hoverLight, menuCommand.context as GameObject);

            // Register the creation in the undo system.
            Undo.RegisterCreatedObjectUndo(hoverLight, "Create " + hoverLight.name);

            Selection.activeObject = hoverLight;
        }
    }
}

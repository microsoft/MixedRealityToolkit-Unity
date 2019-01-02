// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.PropertyDrawers
{
    /// <summary>
    /// Renders the physics layer dropdown based on the current layers set in the Tag Manager.
    /// </summary>
    [CustomPropertyDrawer(typeof(PhysicsLayerAttribute))]
    public sealed class PhysicsLayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guiContents = new List<GUIContent>();
            var layerIds = new List<int>();

            for (int i = 0; i < LayerExtensions.TagManagerLayers.arraySize; i++)
            {
                var layer = LayerExtensions.TagManagerLayers.GetArrayElementAtIndex(i);

                if (!string.IsNullOrWhiteSpace(layer.stringValue))
                {
                    guiContents.Add(new GUIContent($"{i}: {layer.stringValue}"));
                    layerIds.Add(i);
                }
            }

            property.intValue = EditorGUI.IntPopup(position, label, property.intValue, guiContents.ToArray(), layerIds.ToArray());
        }
    }
}
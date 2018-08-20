// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Blend
{
    [CustomEditor(typeof(BlendColor))]
    public class BlendColorEditor : ShaderPropertyEditor
    {
        private BlendColor Inspector;

        public override void OnInspectorGUI()
        {
            Inspector = (BlendColor)target;

            DisplayDropDown();

            DrawDefaultInspector();
        }

        protected override int GetCurrentIndex()
        {
            return ReverseLookup(Inspector.ShaderPropertyName);
        }

        protected override ShaderPropertyType[] GetFilters()
        {
            return new ShaderPropertyType[] { ShaderPropertyType.Color };
        }
        
        protected override GameObject GetGameObject()
        {
            return Inspector.gameObject;
        }

        protected override void SelectProperty()
        {
            Inspector.ShaderPropertyName = shaderPropertyList[selectedIndex].Name;
        }
    }
}

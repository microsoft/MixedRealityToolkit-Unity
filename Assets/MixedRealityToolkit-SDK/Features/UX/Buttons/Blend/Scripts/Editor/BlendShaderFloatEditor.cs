// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Blend
{
    [CustomEditor(typeof(BlendShaderFloat))]
    public class BlendShaderFloatEditor : ShaderPropertyEditor
    {
        private BlendShaderFloat Inspector;

        public override void OnInspectorGUI()
        {
            Inspector = (BlendShaderFloat)target;

            DisplayDropDown();

            DrawDefaultInspector();
        }

        protected override int GetCurrentIndex()
        {
            return ReverseLookup(Inspector.ShaderFloatName);
        }

        protected override ShaderPropertyType[] GetFilters()
        {
            return new ShaderPropertyType[] { ShaderPropertyType.Float, ShaderPropertyType.Range };
        }

        protected override GameObject GetGameObject()
        {
            return Inspector.gameObject;
        }

        protected override void SelectProperty()
        {
            Inspector.ShaderFloatName = shaderPropertyList[selectedIndex].Name;
            Inspector.Range = shaderPropertyList[selectedIndex].Range;
        }
    }
}

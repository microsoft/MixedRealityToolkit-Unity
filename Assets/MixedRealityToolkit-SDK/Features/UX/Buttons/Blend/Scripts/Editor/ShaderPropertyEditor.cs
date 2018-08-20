// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Blend
{
    public abstract class ShaderPropertyEditor : Editor
    {
        protected ShaderProperties[] shaderPropertyList;
        protected Material material;
        protected string[] shaderOptions;
        protected string shaderFilter;
        protected int selectedIndex = -1;
        protected int cachedIndex = -1;

        /// <summary>
        /// return the game object the target is attached to
        /// Example: return Inspector.gameObject;
        /// </summary> 
        protected abstract GameObject GetGameObject();

        /// <summary>
        /// set the proptery in the target
        /// Example: Inspector.ShaderPropertyName = shaderPropertyList[selectedIndex].Name;
        /// </summary>
        protected abstract void SelectProperty();
        
        /// <summary>
        /// Set the filters that for the shader property types
        /// Example: return new ShaderPropertyType[] { ShaderPropertyType.Color };
        /// </summary>
        /// <returns></returns>
        protected abstract ShaderPropertyType[] GetFilters();

        /// <summary>
        /// return the currently selected index or 0
        /// </summary>
        /// <returns></returns>
        protected abstract int GetCurrentIndex();

        /// <summary>
        /// displays the drop down, put in the OnGUI.
        /// Example:
        
          /*public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                Inspector = (BlendColor)target;
                DisplayDropDown();
            }*/

        /// </summary>
        protected virtual void DisplayDropDown()
        {
            Material checkMaterial = ColorAbstraction.GetValidMaterial(GetGameObject().GetComponent<Renderer>());
            if (checkMaterial != material || shaderPropertyList == null)
            {
                if (checkMaterial != null)
                {
                    GetShaderProperties(GetGameObject());
                    material = checkMaterial;
                    cachedIndex = 0;
                    selectedIndex = GetCurrentIndex();
                }
            }

            EditorGUILayout.Space();

            selectedIndex = EditorGUILayout.Popup("Shader Property", GetCurrentIndex(), shaderOptions);

            if (selectedIndex != cachedIndex)
            {
                SelectProperty();
            }

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Gets the availble shader properties
        /// </summary>
        /// <param name="gameObject"></param>
        protected virtual void GetShaderProperties(GameObject gameObject)
        {
            shaderPropertyList = BlendShaderUtils.GetShaderProperties(gameObject.GetComponent<Renderer>(), GetFilters());

            if (shaderPropertyList.Length < 1)
            {
                ShaderProperties property = new ShaderProperties() { Name = "No Material", Range = new Vector2(), Type = ShaderPropertyType.None };
                shaderPropertyList = new ShaderProperties[] { property };
            }

            if (shaderPropertyList != null)
            {
                int count = shaderPropertyList.Length;
                shaderOptions = new string[count];

                for (int i = 0; i < count; i++)
                {
                    shaderOptions[i] = shaderPropertyList[i].Name;
                }
            }
        }

        protected int ReverseLookup(string name)
        {
            for (int i = 0; i < shaderOptions.Length; i++)
            {
                if(shaderOptions[i] == name)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}

//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// The base class for button icon profiles
    /// </summary>
    public abstract class ButtonIconProfile : ButtonProfile
    {
        [Header("Defaults")]
        /// <summary>
        /// The icon returned when a requested icon is not found
        /// </summary>
        public Texture2D _IconNotFound;

        /// <summary>
        /// How quickly to animate changing icon alpha at runtime
        /// </summary>
        public float AlphaTransitionSpeed = 0.25f;

        /// <summary>
        /// The default material used for icons
        /// </summary>
        public Material IconMaterial;

        /// <summary>
        /// The default mesh used for icons
        /// </summary>
        public Mesh IconMesh;

        /// <summary>
        /// Property used to modify icon alpha
        /// If this is null alpha will not be applied
        /// </summary>
        public string AlphaColorProperty = "_Color";

        /// <summary>
        /// Gets a list of icon names - used primarily by editor scripts
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetIconKeys()
        {
            return null;
        }
        
        /// <summary>
        /// Searches for an icon
        /// If found, the icon texture is applied to the target renderer's material and the icon mesh is applied to the target mesh filter
        /// A default icon (_IconNotFound_) will be substituted if useDefaultIfNotFound is true
        /// </summary>
        /// <param name="iconName"></param>
        /// <param name="targetRenderer"></param>
        /// <param name="targetMesh"></param>
        /// <param name="useDefaultIfNotFound"></param>
        /// <returns></returns>
        public virtual bool GetIcon(string iconName, MeshRenderer targetRenderer, MeshFilter targetMesh, bool useDefaultIfNotFound)
        {
            return false;
        }        

        #if UNITY_EDITOR
        public virtual string DrawIconSelectField (string iconName)
        {
            iconName = UnityEditor.EditorGUILayout.TextField("Icon name", iconName);
            return iconName;
        }
        #endif
    }
}
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    public class ButtonMeshProfile : ButtonProfile
    {
        /// <summary>
        /// Name of the shader color property that will be modified (default "_Color")
        /// </summary>
        public string ColorPropertyName = "_Color";
        /// <summary>
        /// Name of the shader float property that will be modified
        /// </summary>
        public string ValuePropertyName = string.Empty;

        /// <summary>
        /// If true, button properties are lerped instead of instantaneous
        /// </summary>
        public bool SmoothStateChanges = false;

        /// <summary>
        /// Whether to hold pressed events for a short period
        /// This ensures that pressed events are visually represented after tapped events
        /// </summary>
        public bool StickyPressedEvents = false;

        /// <summary>
        /// How long to hold sticky pressed events
        /// </summary>
        public float StickyPressedTime = 0.15f;

        /// <summary>
        /// How quickly to animate scale, offset, color and value properties
        /// </summary>
        [Range(0.01f, 1f)]
        public float AnimationSpeed = 1f;

        public CompoundButtonMesh.MeshButtonDatum[] ButtonStates = new CompoundButtonMesh.MeshButtonDatum[]{ new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)0), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)1),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)2), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)3),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)4), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)5) };
    }
}
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Buttons.Enums;
using MixedRealityToolkit.UX.Buttons.Utilities;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Profiles
{
    public class ButtonMeshProfile : ButtonProfile
    {
        [Tooltip("Name of the shader color property that will be modified (default '_Color')")]
        public string ColorPropertyName = "_Color";
       
        [Tooltip("Name of the shader float property that will be modified.")]
        public string ValuePropertyName = string.Empty;

        [Tooltip("If true, button properties are lerped instead of instantaneous.")]
        public bool SmoothStateChanges = false;

        [Tooltip("Whether to hold pressed events for a short period.")]
        public bool StickyPressedEvents = false;
        
        [Tooltip("How long to hold sticky pressed events.")]
        public float StickyPressedTime = 0.15f;

        [Range(0.01f, 1f)]
        [Tooltip("How quickly to animate scale, offset, color and value properties")]
        public float AnimationSpeed = 1f;

        [HideInMRTKInspector]
        public CompoundButtonMesh.MeshButtonDatum[] ButtonStates = new CompoundButtonMesh.MeshButtonDatum[]{ new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)0), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)1),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)2), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)3),
            new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)4), new CompoundButtonMesh.MeshButtonDatum((ButtonStateEnum)5) };
    }
}
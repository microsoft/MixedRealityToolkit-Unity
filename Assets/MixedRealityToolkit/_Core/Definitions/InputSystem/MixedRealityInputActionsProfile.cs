// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Input Actions Profile")]
    public class MixedRealityInputActionsProfile : ScriptableObject
    {
        [SerializeField]
        [Header("Input Actions")]
        [Tooltip("Array of MRTK Input Actions and their ID")]
        private string[] inputActions = new string[4] {"Select", "Menu", "Pointer", "Grip"}; // TODO - Need to populate all the MRTK Default Actions

        public string[] InputActions { get { return inputActions; } }
    }
}
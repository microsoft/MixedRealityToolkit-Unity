// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Theme scriptableObject for loading theme settings
    /// </summary>
    [CreateAssetMenu(fileName = "Theme", menuName = "Mixed Reality Toolkit/Interactable/Theme", order = 1)]
    public class Theme : ScriptableObject
    {
        [FormerlySerializedAs("Settings")]
        public List<ThemeDefinition> Definitions;

        public States States;

        // TODO: Troy - Add comment here
        public List<Dictionary<Type, ThemeDefinition>> History = new List<Dictionary<Type, ThemeDefinition>>();

        public State[] GetStates()
        {
            if (States != null)
            {
                return States.GetStates();
            }

            return new State[0];
        }

    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Buttons.Enums;
using System;
using UnityEngine;

namespace MixedRealityToolkit.UX.Buttons.Datums
{
    /// <summary>
    /// Sprite Button State Data Set
    /// </summary>
    [Serializable]
    public class SpriteButtonDatum
    {
        public SpriteButtonDatum(ButtonStateEnum state) { this.ActiveState = state; this.Name = state.ToString(); }

        /// <summary>
        /// Name of Datum entry
        /// </summary>
        public string Name;
        /// <summary>
        /// Button State association
        /// </summary>
        public ButtonStateEnum ActiveState;
        /// <summary>
        /// Button sprite for new state
        /// </summary>
        public Sprite ButtonSprite;
        /// <summary>
        /// Color for sprite in new state
        /// </summary>
        public Color SpriteColor = Color.white;
        /// <summary>
        /// New scale for button state
        /// </summary>
        public Vector3 Scale = Vector3.one;
    }
}

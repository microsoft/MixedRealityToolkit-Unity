//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Mesh Button State Data Set
    /// </summary>
    [Serializable]
    public class MeshButtonDatum
    {
        /// <summary>
        /// Constructor for mesh button datum
        /// </summary>
        public MeshButtonDatum(ButtonStateEnum state) { this.ActiveState = state; this.Name = state.ToString(); }

        /// <summary>
        /// Name string for datum entry
        /// </summary>
        public string Name;
        /// <summary>
        /// Button state the datum is active in
        /// </summary>
        public ButtonStateEnum ActiveState = ButtonStateEnum.Observation;
        /// <summary>
        /// Button mesh color to use in active state
        /// </summary>
        public Color StateColor = Color.white;
        /// <summary>
        /// Offset to translate mesh to in active state.
        /// </summary>
        public Vector3 Offset;
        /// <summary>
        /// Scale for mesh button in active state
        /// </summary>
        public Vector3 Scale;
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [Serializable]
    public class PointerResult
    {
        public Vector3 StartPoint { get; protected set; }

        public FocusDetails End { get; protected set; }

        public GameObject PreviousEndObject { get; protected set; }

        public RaycastHit LastRaycastHit { get; protected set; }

        /// <summary>
        /// The index of the step that produced the last raycast hit
        /// 0 when no raycast hit
        /// </summary>
        public int RayStepIndex { get; protected set; }
    }
}
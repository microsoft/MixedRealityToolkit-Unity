// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// FocusDetails struct contains information about which game object has the focus currently.
    /// Also contains information about the normal of that point.
    /// </summary>
    [Serializable]
    public struct FocusDetails
    {
        public Vector3 Point;
        public Vector3 Normal;
        public GameObject Object;
    }
}

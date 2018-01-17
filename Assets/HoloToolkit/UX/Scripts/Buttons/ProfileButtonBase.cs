// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Unity.Buttons
{
    /// <summary>
    /// Ensures a consistent profile field in compound buttons scripts which use a profile
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProfileButtonBase<T> : MonoBehaviour where T : ButtonProfile
    {
        [Header("Profile")]
        [DrawLast]
        public T Profile;
    }
}

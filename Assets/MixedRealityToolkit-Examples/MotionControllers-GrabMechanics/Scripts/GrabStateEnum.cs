// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

/// <summary>
/// //Intended Usage//
/// Attach a "grabbable_x" script (a script that inherits from this) to any object that is meant to be grabbed
/// create more specific grab behavior by adding additional scripts/components to the game object, such as scalableObject, rotatableObject, throwableObject 
/// </summary>

namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    public enum GrabStateEnum
    {
        Inactive,
        Single,
        Multi,
    }
}
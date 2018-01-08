// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
namespace HoloToolkit.Unity.InputModule.Examples.Grabbables
{
    /// <summary>
    /// Extends from BaseUsable. This is a non-abstract script that's actually attached to usable object 
    /// Define the use behaviour of a script here
    /// This script will not work without a grab script attached to the same gameObject
    /// </summary>

    public class UsableObject : BaseUsable
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            Debug.Log("Do something here with the usable object...");
            Debug.LogWarning("Do something here with the usable object...");
        }

        protected override void OnDisable()
        {
            Debug.Log("Do something here with the usable object...");
            Debug.LogWarning("Do something here with the usable object...");

            base.OnDisable();
        }


        protected override void UseStart()
        {
            Debug.Log("Do something here with the usable object...");
            Debug.LogWarning("Do something here with the usable object...");
        }

        protected override void UseEnd()
        {
            Debug.Log("End of Use on UsableObject...");
            Debug.LogWarning("End of use on usable object...");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MRTK.Grabbables
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
        }

        protected override void OnDisable()
        {
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
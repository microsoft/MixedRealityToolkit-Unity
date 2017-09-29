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
        protected override void UseEnd()
        {
            throw new System.NotImplementedException();
        }

        protected override void UseStart()
        {
            throw new System.NotImplementedException();
        }
    }
}
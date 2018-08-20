// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend.Automation;
using UnityEngine;

namespace Interact.Controls
{
    /// <summary>
    /// Makes the assigned object follow and face another object.
    /// A potential use is moving a UI panel around, but is very flexible.
    /// 
    /// Add the reference object this object should follow and tell it to start running.
    /// To get this to follow the user around, add the camera as the reference object.
    /// Typical Use:
    /// Call Start Running();
    /// 
    /// Additional Features:
    ///     - Link to an interactive object to add manual controls.
    /// </summary>
    public class MoveInteractiveWithObject : MoveWithObject
    {
        [Tooltip("An game object containing an Interactive component to call on air-tap")]
        public GameObject ReferenceInteractive;
        
        // start the object following the reference object
        public override void StartRunning()
        {
            base.StartRunning();
            
            if (ReferenceInteractive != null)
            {
                // TEMP InputManager.Instance.PushModalInputHandler(ReferenceInteractive);
            }
        }

        /// <summary>
        /// stop the object from following
        /// </summary>
        public override void StopRunning()
        {
            base.StopRunning();

            if (ReferenceInteractive != null)
            {
                // TEMP InputManager.Instance.PopModalInputHandler();
            }
        }
    }
}

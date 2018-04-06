// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;

namespace Microsoft.MixedReality.Toolkit.InputSystem
{
    /// <summary>
    /// The Input system controls the orchestration of input events in a scene
    /// </summary>
    public class MixedRealityInputManager : BaseManager, IMixedRealityInputSystem
    {
        /// <summary>
        /// MixedRealityInputManager constructor
        /// </summary>
        public MixedRealityInputManager()
        {

        }

        /// <summary>
        /// IMixedRealityManager Initialize function, called once the Mixed Reality Manager has finished registering all managers
        /// Subscription required to the "InitializeEvent" event of the MixedRealityManager
        /// </summary>
        public override void Initialize()
        {
            //Initialize stuff 
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// Subscription required to the "UpdateEvent" event of the MixedRealityManager
        /// </summary>
        public override void Update()
        {
            if (Enabled)
            {
                //Update stuff 

            }
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// Subscription required to the "ProfileUpdateEvent" event of the MixedRealityManager
        /// </summary>
        public override void ProfileUpdate()
        {
            //React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// Subscription required to the "DestroyEvent" event of the MixedRealityManager
        /// </summary>
        public override void Destroy()
        {
            //Destroy stuff 
        }
    }
}

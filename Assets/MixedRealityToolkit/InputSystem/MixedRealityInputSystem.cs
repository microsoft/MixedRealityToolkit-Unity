// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces;

namespace Microsoft.MixedReality.Toolkit.InputSystem
{
    /// <summary>
    /// The Input system controls the orchestration of input events in a scene
    /// </summary>
    public class MixedRealityInputManager : IMixedRealityInputSystem
    {
        /// <summary>
        /// MixedRealityInputManager constructor
        /// </summary>
        public MixedRealityInputManager()
        {
        }

        /// <summary>
        /// IMixedRealityManager Initialize function, called once the Mixed Reality Manager has finished registering all managers
        /// </summary>
        public void Initialize()
        {
            //Initialize stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public void Reset()
        {
            //React to profile change
        }

        public void Update()
        {
            // Unused.
        }

        public void Destroy() { }
    }
}

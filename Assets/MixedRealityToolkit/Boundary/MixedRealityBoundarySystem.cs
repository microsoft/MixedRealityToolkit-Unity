// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;

namespace Microsoft.MixedReality.Toolkit.InputSystem
{
    /// <summary>
    /// The Boundary system controls the presentation and display of the users boundary in a scene
    /// </summary>
    public class MixedRealityBoundaryManager : BaseManager, IMixedRealityBoundarySystem
    {
        public override void Initialize()
        {
            // TODO Initialize stuff 
        }

        /// <summary>
        /// Optional Update function to perform per-frame updates of the manager
        /// </summary>
        public override void Update()
        {
            // TODO Update stuff 
        }

        /// <summary>
        /// Optional ProfileUpdate function to allow reconfiguration when the active configuration profile of the Mixed Reality Manager is replaced
        /// </summary>
        public override void Reset()
        {
            // TODO React to profile change
        }

        /// <summary>
        /// Optional Destroy function to perform cleanup of the manager before the Mixed Reality Manager is destroyed
        /// </summary>
        public override void Destroy()
        {
            // TODO Destroy stuff 
        }
    }
}

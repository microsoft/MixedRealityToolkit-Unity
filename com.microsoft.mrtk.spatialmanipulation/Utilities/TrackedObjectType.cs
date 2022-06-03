// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// The type of object being tracked.
    /// </summary>
    public enum TrackedObjectType
    {
        /// <summary>
        /// The user's head.
        /// </summary>
        Head = 0,

        /// <summary>
        /// The system-calculated ray of an available controller
        /// (motion controllers, hands, etc.)
        /// </summary>
        Interactor = 1,

        /// <summary>
        /// The system-calculated ray of an available controller
        /// (motion controllers, hands, etc.)
        /// </summary>
        /// <remarks>
        /// This is the MRTK v2 equivalent name for <see cref="TrackedObjectType.Interactor"/>,
        /// it exists to ease code porting.
        /// </remarks>
        ControllerRay = Interactor,

        /// <summary>
        /// A tracked hand joint.
        /// </summary>
        HandJoint = 2,

        /// <summary>
        /// An application specific object.
        /// </summary>
        CustomOverride = int.MaxValue
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.Experimental.InteractiveElement
{
    /// <summary>
    /// The type of interaction an InteractionState is associated with. Utilized by the InteractionState
    /// class.  
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// Does not support any form of input interaction.
        /// </summary>
        None = 0,

        /// <summary>
        /// Near interaction support. Input is considered near interaction when an articulated hand has 
        /// direct contact with another game object, i.e. the position the articulated hand is 
        /// close to the position of the game object in world space.
        /// </summary>
        Near,

        /// <summary>
        /// Far interaction support. Input is considered far interaction when direct contact with 
        /// the game object is not required. For example, input via controller ray or gaze is considered
        /// far interaction input.
        /// </summary>
        Far,
        
        /// <summary>
        /// Encompasses both near and far interaction support. 
        /// </summary>
        NearAndFar,

        /// <summary>
        /// Pointer independent interaction support.
        /// </summary>
        Other
    }
}
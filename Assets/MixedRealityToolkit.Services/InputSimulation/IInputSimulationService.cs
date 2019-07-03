// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{ 
    public interface IInputSimulationService : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityInputSimulationProfile InputSimulationProfile { get; }

        /// <summary>
        /// Pose data for the left hand.
        /// </summary>
        SimulatedHandData HandDataLeft { get; }
        /// <summary>
        /// Pose data for the right hand.
        /// </summary>
        SimulatedHandData HandDataRight { get; }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        bool UserInputEnabled { get; set; }
    }
}
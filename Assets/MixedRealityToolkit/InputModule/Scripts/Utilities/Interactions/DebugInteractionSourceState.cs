// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.InputModule.Utilities.Interations
{
    /// <summary>
    /// Since the InteractionSourceState is internal to UnityEngine.VR.WSA.Input,
    /// this is a fake SourceState structure to keep the test code consistent.
    /// </summary>
    public struct DebugInteractionSourceState
    {
        public bool Pressed;
        public bool Grasped;
        public bool MenuPressed;
        public bool SelectPressed;
        public DebugInteractionSourcePose SourcePose;
    }
}
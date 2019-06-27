// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Utility class to use a simulated hand
    internal class TestHand
    {
        private Handedness handedness;
        private Vector3 position;
        private ArticulatedHandPose.GestureId gestureId = ArticulatedHandPose.GestureId.Open;
        private InputSimulationService simulationService;

        public TestHand(Handedness handedness)
        {
            this.handedness = handedness;
            simulationService = PlayModeTestUtilities.GetInputSimulationService();
        }

        public IEnumerator Show(Vector3 position)
        {
            this.position = position;
            return PlayModeTestUtilities.ShowHand(handedness, simulationService, gestureId, position);
        }

        public IEnumerator Hide()
        {
            return PlayModeTestUtilities.HideHand(handedness, simulationService);
        }

        public IEnumerator MoveTo(Vector3 newPosition, int numSteps = 30)
        {
            Vector3 oldPosition = position;
            position = newPosition;
            return PlayModeTestUtilities.MoveHandFromTo(oldPosition, newPosition, numSteps, gestureId, handedness, simulationService);
        }

        public IEnumerator SetGesture(ArticulatedHandPose.GestureId newGestureId)
        {
            gestureId = newGestureId;
            return PlayModeTestUtilities.MoveHandFromTo(position, position, 1, gestureId, handedness, simulationService);
        }
    }
}
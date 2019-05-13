// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{ 
    /// <summary>
    /// Helper class for managing the visibility of the gaze pointer to match windows mixed reality and HoloLens 2
    /// When application starts, gaze pointer is visible. Then when articulate hands / motion controllers
    /// appear, hide the gaze cursor. Whenever user says the voice wake word, make the 
    /// gaze controller appear.
    /// </summary>
    class GazePointerStateMachine : IMixedRealitySpeechHandler
    {
        private enum GazePointerState
        {
            Initial, // When the application starts up, the gaze pointer should be active
            GazePointerActive, // Gaze pointer is active when no hands are visible, after "select" 
            GazePointerInactive // Gaze pointer is inactive as soon as motion controller or articulated hand pointers appear
        }
        private GazePointerState gazePointerState = GazePointerState.Initial;
        private bool activateGazeKeywordIsSet = false;

        public bool IsGazePointerActive
        {
            get { return gazePointerState != GazePointerState.GazePointerInactive; }
        }

        public void UpdateState(int numNearPointersActive, int numFarPointersActive)
        {
            GazePointerState newState = gazePointerState;
            bool isMotionControllerOrHandUp = numFarPointersActive > 0 || numNearPointersActive > 0;
            switch (gazePointerState)
            {
                case GazePointerState.Initial:
                    if (isMotionControllerOrHandUp)
                    {
                        // There is some pointer other than the gaze pointer in the scene, assume
                        // this is from a motion controller or articulated hand, and that we should
                        // hide the gaze pointer
                        newState = GazePointerState.GazePointerInactive;
                    }
                    break;
                case GazePointerState.GazePointerActive:
                    if (isMotionControllerOrHandUp)
                    {
                        newState = GazePointerState.GazePointerInactive;
                    }
                    break;
                case GazePointerState.GazePointerInactive:
                    // Go from inactive to active if we say the word "select"
                    if (activateGazeKeywordIsSet)
                    {
                        newState = GazePointerState.GazePointerActive;
                        activateGazeKeywordIsSet = false;
                    }
                    break;
                default:
                    break;
            }
            gazePointerState = newState;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.Command.Keyword.Equals("select", StringComparison.CurrentCultureIgnoreCase))
            {
                activateGazeKeywordIsSet = true;
            }
        }
    }

}

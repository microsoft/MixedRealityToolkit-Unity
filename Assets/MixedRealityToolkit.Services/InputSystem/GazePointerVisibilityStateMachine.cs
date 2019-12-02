// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Helper class for managing the visibility of the gaze pointer to match windows mixed reality and HoloLens 2
    /// When application starts, gaze pointer is visible. Then when articulate hands / motion controllers
    /// appear, hide the gaze cursor. Whenever user says "select", make the gaze cursor appear.
    /// </summary>
    /// <remarks>
    /// Has different behavior depending on whether or not eye gaze or head gaze in use - see comments on
    /// GazePointerState for more details.
    /// </remarks>
    public class GazePointerVisibilityStateMachine : IMixedRealitySpeechHandler
    {
        private enum GazePointerState
        {
            // When the application starts up, the gaze pointer should be active
            Initial,

            // If head gaze is in use, then the gaze pointer is active when no hands are visible, after "select"
            // If eye gaze is use, then the gaze pointer is active when no far pointers are active.
            GazePointerActive,

            // If head gaze is in use, then the gaze pointer is inactive as soon as motion controller or
            // articulated hand pointers appear.
            // If eye gaze is in use, then the gaze pointer is inactive when far pointers are active.
            GazePointerInactive
        }
        private GazePointerState gazePointerState = GazePointerState.Initial;
        private bool activateGazeKeywordIsSet = false;
        private bool eyeGazeValid = false;

        public bool IsGazePointerActive
        {
            get { return gazePointerState != GazePointerState.GazePointerInactive; }
        }

        /// <summary>
        /// Updates the state machine based on the number of near pointers, the number of far pointers,
        /// and whether or not eye gaze is valid.
        /// </summary>
        public void UpdateState(int numNearPointersActive, int numFarPointersActive, int numFarPointersWithoutCursorActive, bool isEyeGazeValid)
        {
            if (eyeGazeValid != isEyeGazeValid)
            {
                activateGazeKeywordIsSet = false;
                eyeGazeValid = isEyeGazeValid;
            }

            if (isEyeGazeValid)
            {
                UpdateStateEyeGaze(numNearPointersActive, numFarPointersActive);
            }
            else
            {
                UpdateStateHeadGaze(numNearPointersActive, numFarPointersActive, numFarPointersWithoutCursorActive);
            }
        }

        private void UpdateStateEyeGaze(int numNearPointersActive, int numFarPointersActive)
        {
            // Only enable eye gaze as a pointer if there are no other near or far pointers active.
            bool isEyeGazePointerActive = numFarPointersActive == 0 && numNearPointersActive == 0;

            gazePointerState = isEyeGazePointerActive ?
                GazePointerState.GazePointerActive :
                GazePointerState.GazePointerInactive;
        }

        private void UpdateStateHeadGaze(int numNearPointersActive, int numFarPointersActive, int numFarPointersWithoutCursorActive)
        {
            bool canGazeCursorShow = numFarPointersActive == 0 && numNearPointersActive == 0;
            switch (gazePointerState)
            {
                case GazePointerState.Initial:
                    if (!canGazeCursorShow)
                    {
                        // There is some pointer other than the gaze pointer in the scene, assume
                        // this is from a motion controller or articulated hand, and that we should
                        // hide the gaze pointer
                        gazePointerState = GazePointerState.GazePointerInactive;
                    }
                    break;
                case GazePointerState.GazePointerActive:
                    if (!canGazeCursorShow)
                    {
                        activateGazeKeywordIsSet = false;
                        gazePointerState = GazePointerState.GazePointerInactive;
                    }
                    break;
                case GazePointerState.GazePointerInactive:
                    // Go from inactive to active if we say the word "select"
                    if (activateGazeKeywordIsSet)
                    {
                        activateGazeKeywordIsSet = false;
                        gazePointerState = GazePointerState.GazePointerActive;
                    }
                    if (canGazeCursorShow && numFarPointersWithoutCursorActive > 0)
                    {
                        activateGazeKeywordIsSet = false;
                        gazePointerState = GazePointerState.GazePointerActive;
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (!eyeGazeValid && eventData.Command.Keyword.Equals("select", StringComparison.CurrentCultureIgnoreCase))
            {
                activateGazeKeywordIsSet = true;
            }
        }
    }

}

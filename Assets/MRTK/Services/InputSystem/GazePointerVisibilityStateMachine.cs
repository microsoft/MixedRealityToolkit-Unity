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
            /// <summary>
            /// When the application starts up, the gaze pointer should be active.
            /// </summary>
            Initial,

            /// <summary>
            /// If head gaze is in use, then the gaze pointer is active when no hands are visible, after "select".
            /// If eye gaze is use, then the gaze pointer is active when no far pointers are active.
            /// </summary>
            GazePointerActive,

            /// <summary>
            /// If head gaze is in use, then the gaze pointer is inactive as soon as motion controller or
            /// articulated hand pointers appear.
            /// If eye gaze is in use, then the gaze pointer is inactive when far pointers are active.
            /// </summary>
            GazePointerInactive
        }

        private GazePointerState gazePointerState = GazePointerState.Initial;
        private bool activateGazeKeywordIsSet = false;
        private bool eyeGazeValid = false;

        /// <summary>
        /// Whether the state machine is currently in a state where the gaze pointer should be active.
        /// </summary>
        public bool IsGazePointerActive => gazePointerState != GazePointerState.GazePointerInactive;

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

        /// <summary>
        /// Flags user intention to re activate eye or head based gaze cursor
        /// </summary>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.Command.Keyword.Equals("select", StringComparison.CurrentCultureIgnoreCase))
            {
                activateGazeKeywordIsSet = true;
            }
        }
    }
}

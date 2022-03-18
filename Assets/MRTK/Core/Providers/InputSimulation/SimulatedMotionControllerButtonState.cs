// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Struct storing the states of buttons on the motion controller
    /// </summary>
    public struct SimulatedMotionControllerButtonState : IEquatable<SimulatedMotionControllerButtonState>
    {
        /// <summary>
        /// Whether the motion controller is selecting (i.e. the trigger button is being pressed)
        /// </summary>
        public bool IsSelecting;

        /// <summary>
        /// Whether the motion controller is grabbing (i.e. the grab button is being pressed)
        /// </summary>
        public bool IsGrabbing;

        /// <summary>
        /// Whether the menu button on the motion controller is being pressed
        /// </summary>
        public bool IsPressingMenu;

        public override bool Equals(object obj)
        {
            if (obj is SimulatedMotionControllerButtonState state)
            {
                return Equals(state);
            }
            return false;
        }

        public bool Equals(SimulatedMotionControllerButtonState state)
        {
            return IsSelecting == state.IsSelecting && IsGrabbing == state.IsGrabbing && IsPressingMenu == state.IsPressingMenu;
        }

        public override int GetHashCode()
        {
            return (IsSelecting ? 1 : 0) * 100 + (IsGrabbing ? 1 : 0) * 10 + (IsPressingMenu ? 1 : 0);
        }

        public static bool operator ==(SimulatedMotionControllerButtonState lhs, SimulatedMotionControllerButtonState rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SimulatedMotionControllerButtonState lhs, SimulatedMotionControllerButtonState rhs)
        {
            return !(lhs.Equals(rhs));
        }
    }
}

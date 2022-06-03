// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods that make working with the <see cref="Handedness"/> enum easier.
    /// </summary>
    public static class HandednessExtensions
    {
        /// <summary>
        /// Gets the opposite "hand" flag for the current <see cref="Handedness"/> value.
        /// </summary>
        /// <remarks>
        /// If Left, returns Right, if Right, returns Left otherwise returns None.
        /// Otherwise, returns None
        /// </remarks>
        public static Handedness GetOppositeHandedness(this Handedness current)
        {
            if (current == Handedness.Left)
            {
                return Handedness.Right;
            }
            else if (current == Handedness.Right)
            {
                return Handedness.Left;
            }
            else
            {
                return Handedness.None;
            }
        }

        /// <summary>
        /// Checks whether or not the current <see cref="Handedness"/> value is Right.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Handedness"/> value being checked is Right, otherwise false.
        /// </returns>
        public static bool IsRight(this Handedness current)
        {
            return current == Handedness.Right;
        }

        /// <summary>
        /// Checks whether or not the current <see cref="Handedness"/> value is Left.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Handedness"/> value being checked is Left, otherwise false.
        /// </returns>
        public static bool IsLeft(this Handedness current)
        {
            return current == Handedness.Left;
        }

        /// <summary>
        /// Checks whether or not the current <see cref="Handedness"/> value is None.
        /// </summary>
        /// <returns>
        /// True if the <see cref="Handedness"/> value being checked is None, otherwise false.
        /// </returns>
        public static bool IsNone(this Handedness current)
        {
            return current == Handedness.None;
        }

        /// <summary>
        /// Checks whether or not the current <see cref="Handedness"/> value matches the specified value.
        /// </summary>
        /// <returns>
        /// True if the specified <see cref="Handedness"/> value matches the current, otherwise false.
        /// </returns>
        public static bool IsMatch(this Handedness current, Handedness compare)
        {
            return (current & compare) != 0;
        }

        /// <summary>
        /// Gets the XRNode representing the specified handedness.
        /// </summary>
        /// <param name="hand">The <see cref="Handedness"/> value for
        /// which the XRNode is requested.</param>
        /// <returns>
        /// XRNode representing the specified <see cref="Handedness"/>, or null.
        /// </returns>
        public static XRNode? ToXRNode(this Handedness hand)
        {
            if (hand.IsNone()) { return null; }

            Debug.Assert(hand.IsLeft() || hand.IsRight());
            return (hand.IsLeft() ? XRNode.LeftHand : XRNode.RightHand);
        }
    }
}

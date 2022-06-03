// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Methods which extend the functionality of the Unity XRNode struct.
    /// </summary>
    public static class XRNodeExtensions
    {
        /// <summary>
        /// Returns the <see cref="Handedness"/> of the specified XRNode.
        /// </summary>
        /// <param name="node">The XRNode for which the <see cref="Handedness"/> is requested.</param>
        /// <returns>
        /// <see cref="Handedness"/> value representing the XRNode.
        /// </returns>
        /// <remarks>
        /// This will return <see cref="Handedness.None"/> for XRNode values other than
        /// LeftHand or RightHand.
        /// </remarks>
        public static Handedness ToHandedness(this XRNode node)
        {
            switch (node)
            {
                case XRNode.LeftHand:
                    return Handedness.Left;

                case XRNode.RightHand:
                    return Handedness.Right;

                default:
                    return Handedness.None;
            }
        }

        /// <summary>
        /// Determine if the specified XRNode represents a hand.
        /// </summary>
        /// <param name="node">The XRNode to be queried.</param>
        /// <returns>
        /// True if the specified XRNode represents the left or right hand, or false.
        /// </returns>
        public static bool IsHand(this XRNode node)
        {
            return (node.IsLeftHand() || node.IsRightHand());
        }

        /// <summary>
        /// Determine if the specified XRNode represents the left hand.
        /// </summary>
        /// <param name="node">The XRNode to be queried.</param>
        /// <returns>
        /// True if the specified XRNode represents the left hand, or false.
        /// </returns>
        public static bool IsLeftHand(this XRNode node)
        {
            return (node == XRNode.LeftHand);
        }

        /// <summary>
        /// Determine if the specified XRNode represents the right hand.
        /// </summary>
        /// <param name="node">The XRNode to be queried.</param>
        /// <returns>
        /// True if the specified XRNode represents the right hand, or false.
        /// </returns>
        public static bool IsRightHand(this XRNode node)
        {
            return (node == XRNode.RightHand);
        }
    }
}
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Extension methods specific to the <see cref="ObjectManipulator.ReleaseBehaviorType"/> and <see cref="ManipulationHandler.ReleaseBehaviorType"/> enums.
    /// </summary>
    public static class ReleaseBehaviorTypeExtensions
    {
        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="ObjectManipulator.ReleaseBehaviorType"/> value.</param>
        /// <param name="b"><see cref="ObjectManipulator.ReleaseBehaviorType"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this ObjectManipulator.ReleaseBehaviorType a, ObjectManipulator.ReleaseBehaviorType b)
        {
            return (a & b) == b;
        }

        /// <summary>
        /// Checks to determine if all bits in a provided mask are set.
        /// </summary>
        /// <param name="a"><see cref="ManipulationHandler.ReleaseBehaviorType"/> value.</param>
        /// <param name="b"><see cref="ManipulationHandler.ReleaseBehaviorType"/> mask.</param>
        /// <returns>
        /// True if all of the bits in the specified mask are set in the current value.
        /// </returns>
        public static bool IsMaskSet(this ManipulationHandler.ReleaseBehaviorType a, ManipulationHandler.ReleaseBehaviorType b)
        {
            return (a & b) == b;
        }
    }
}

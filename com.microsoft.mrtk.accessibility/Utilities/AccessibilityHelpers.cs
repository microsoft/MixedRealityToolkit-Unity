// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Helper methods for working with Accessibility components.
    /// </summary>
    public static class AccessibilityHelpers
    {
        private static AccessibilitySubsystem subsystem = null;

        /// <summary>
        /// The first running AccessibilitySubsystem instance.
        /// </summary>
        public static AccessibilitySubsystem Subsystem
        {
            get
            {
                if (subsystem == null || !subsystem.running)
                {
                    subsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<AccessibilitySubsystem>();
                }
                return subsystem;
            }
        }
    }
}

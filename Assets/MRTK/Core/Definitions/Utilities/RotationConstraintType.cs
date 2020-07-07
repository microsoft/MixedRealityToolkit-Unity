// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum RotationConstraintType
    {
        None,
        XAxisOnly,
        YAxisOnly,
        ZAxisOnly
    }

    /// <summary>
    /// Helper class used to convert from RotationConstraintType to AxisFlags
    /// </summary>
    public class RotationConstraintHelper
    {
        /// <summary>
        /// Returns corresponding AxisFlags for given RotationConstraintType
        /// </summary>
        public static AxisFlags ConvertToAxisFlags(RotationConstraintType type)
        {
            switch (type)
            {
                case RotationConstraintType.XAxisOnly:
                    return AxisFlags.YAxis | AxisFlags.ZAxis;
                case RotationConstraintType.YAxisOnly:
                    return AxisFlags.XAxis | AxisFlags.ZAxis;
                case RotationConstraintType.ZAxisOnly:
                    return AxisFlags.XAxis | AxisFlags.YAxis;
                default:
                    return 0;
            }
        }
    }
}
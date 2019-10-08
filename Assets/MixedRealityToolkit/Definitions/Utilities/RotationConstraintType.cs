// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum RotationConstraintType
    {
        None,
        XAxisOnly,
        YAxisOnly,
        ZAxisOnly
    }

    public class RotationConstraintHelper
    {
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
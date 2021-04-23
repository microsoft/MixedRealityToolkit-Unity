// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Rotation handles for <see cref="BoundsControl"/> that are used for rotating the
    /// Gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class RotationHandles : PerAxisHandles
    {
        /// <inheritdoc/>
        protected override string HandlePositionDescription => "midpoint";

        /// <inheritdoc/>
        internal override CardinalAxisType[] handleAxes => VisualUtils.EdgeAxisType;

        /// <inheritdoc/>
        internal override void CalculateHandlePositions(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null && HandlePositions != null)
            {
                for (int i = 0; i < HandlePositions.Length; ++i)
                {
                    HandlePositions[i] = VisualUtils.GetLinkPosition(i, ref boundsCorners);
                }
                UpdateHandles();
            }
        }

        internal RotationHandles(RotationHandlesConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc/>
        protected override Quaternion GetRotationRealignment(int handleIndex)
        {
            if (VisualUtils.EdgeAxisType[handleIndex] == CardinalAxisType.X)
            {
                return Quaternion.FromToRotation(Vector3.up, Vector3.right);
            }
            else if (VisualUtils.EdgeAxisType[handleIndex] == CardinalAxisType.Z)
            {
                return Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            }
            else // y axis 
            {
                return Quaternion.identity;
            }
        }

        #region BoundsControlHandlerBase 

        /// <inheritdoc/>
        internal override HandleType GetHandleType()
        {
            return HandleType.Rotation;
        }

        #endregion BoundsControlHandlerBase

    }
}

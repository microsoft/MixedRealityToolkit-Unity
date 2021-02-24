// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.BoundsControl
{
    /// <summary>
    /// Translation handles for <see cref="BoundsControl"/> that are used for translating the
    /// Gameobject BoundsControl is attached to with near or far interaction
    /// </summary>
    public class TranslationHandles : PerAxisHandles
    {

        /// <inheritdoc/>
        protected override string HandlePositionDescription => "faceCenter";

        /// <inheritdoc/>
        internal override CardinalAxisType[] handleAxes => VisualUtils.FaceAxisType;

        internal override void CalculateHandlePositions(ref Vector3[] boundsCorners)
        {
            if (boundsCorners != null && HandlePositions != null)
            {
                for (int i = 0; i < HandlePositions.Length; ++i)
                {
                    HandlePositions[i] = VisualUtils.GetFaceCenterPosition(i, ref boundsCorners);
                }
            
                UpdateHandles();
            }
        }

        internal TranslationHandles(TranslationHandlesConfiguration configuration) 
            : base(configuration)
        { 
        }

        protected override Quaternion GetRotationRealignment(int handleIndex)
        {
            // Even handle indices point in the positive direction along their axis,
            // and odd handle indices point in the negative direction;
            var directionSign = handleIndex % 2 == 0 ? 1.0f : -1.0f;
            // Align handle with its edge assuming that the prefab is initially aligned with the up direction 
            if (VisualUtils.FaceAxisType[handleIndex] == CardinalAxisType.X)
            {
                return Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.right);
            }
            else if (VisualUtils.FaceAxisType[handleIndex] == CardinalAxisType.Z)
            {
                return Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.forward);
            }
            else // y axis
            {
                return Quaternion.FromToRotation(Vector3.forward, directionSign * Vector3.up);
            }
        }
      
        #region BoundsControlHandlerBase 

        internal override HandleType GetHandleType()
        {
            return HandleType.Translation;
        }

        #endregion BoundsControlHandlerBase

    }
}

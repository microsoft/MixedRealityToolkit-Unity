using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// A transformation which restricts the scale of of the MixedRealityTransform
    /// </summary>
    public class MinMaxConstraintTransformation : ITransformation
    {
        public Vector3 minScale = Vector3.one * 0.2f;

        public Vector3 maxScale = Vector3.one * 2.0f;

        protected const int scale_priority = -1000;
        protected const int rotation_priority = -1000;
        protected const int position_priority = -1000;
        protected const int constraint_priority_modifier = 1;

        public int ExecutionPriority => throw new NotImplementedException();

        public MixedRealityTransform ApplyTransformation(MixedRealityTransform initialTransform)
        {
            Vector3 newScale = new Vector3()
            {
                x = Mathf.Clamp(initialTransform.Scale.x, minScale.x, maxScale.x),
                y = Mathf.Clamp(initialTransform.Scale.y, minScale.y, maxScale.y),
                z = Mathf.Clamp(initialTransform.Scale.z, minScale.z, maxScale.z)
            };

            initialTransform.Scale = newScale;
            return initialTransform;
        }
    }
}

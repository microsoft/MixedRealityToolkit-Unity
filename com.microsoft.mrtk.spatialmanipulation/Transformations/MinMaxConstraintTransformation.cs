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

        public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale)
        {
            initialLocalScale.x = Mathf.Clamp(initialLocalScale.x, minScale.x, maxScale.x);
            initialLocalScale.y = Mathf.Clamp(initialLocalScale.y, minScale.y, maxScale.y);
            initialLocalScale.z = Mathf.Clamp(initialLocalScale.z, minScale.z, maxScale.z);
            return (initialPosition, initialRotation, initialLocalScale);
        }
    }
}

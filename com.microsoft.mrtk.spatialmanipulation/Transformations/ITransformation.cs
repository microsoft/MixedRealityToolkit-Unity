using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Interface which describes a transformation that is applied to MixedRealityTransform
    /// </summary>
    public interface ITransformation
    {
        public (Vector3, Quaternion, Vector3) ApplyTransformation(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialLocalScale);

        /// <summary>
        /// Execution order priority of this constraint. Lower numbers will be executed before higher numbers.
        /// </summary>
        public int ExecutionPriority { get; }
    }
}

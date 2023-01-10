using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Interface which describes a transformation that is applied to MixedRealityTransform
    /// </summary>
    public interface ITransformation
    {
        public MixedRealityTransform ApplyTransformation(MixedRealityTransform initialTransform);

        /// <summary>
        /// Execution order priority of this constraint. Lower numbers will be executed before higher numbers.
        /// </summary>
        public int ExecutionPriority { get; }
    }
}

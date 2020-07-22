// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Scriptable object that configures an elastic system used for linear/one-dimensional values.
    /// </summary>
    [CreateAssetMenu(fileName = "LinearElasticConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Elastic/Linear Elastic Configuration")]
    public class LinearElasticConfiguration : ElasticConfiguration
    {
        [SerializeField]
        [Tooltip("Properties of the linear elastic extent.")]
        protected ElasticExtentProperties<float> extentProperties = new ElasticExtentProperties<float>
        {
            // Reasonable defaults.
            MinStretch = 0.0f,
            MaxStretch = 1.0f,
            SnapToEnds = false,
            SnapPoints = { },
            SnapRadius = 1.0f
        };

        /// <summary>
        /// Properties of the rotation elastic extent.
        /// </summary>
        public ElasticExtentProperties<float> ExtentProperties
        {
            get => extentProperties;
            set => extentProperties = value;
        }

        /// <summary>
        /// Factory method that allows subclasses of LinearElasticConfiguration to fabricate more
        /// specialized versions of a LinearElasticSystem if necessary, such as more advanced
        /// snapping behavior, alternative integrators/algorithms, or other specializations.
        /// </summary>
        /// <param name="initialValue"> The desired initial value for the oscillator sim. </param>
        /// <param name="initialVelocity"> The desired initial velocity for the oscillator sim. </param>
        /// <returns>
        /// A subclass of <see cref="ElasticSystem{Quaternion}"/> that can simulate damped harmonic
        /// oscillations over a one-dimensional range of values.
        /// </returns>
        public virtual ElasticSystem<float> MakeElasticSystem(float initialValue, float initialVelocity)
        {
            return new LinearElasticSystem(initialValue, initialVelocity, extentProperties, elasticProperties);
        }
    }
}


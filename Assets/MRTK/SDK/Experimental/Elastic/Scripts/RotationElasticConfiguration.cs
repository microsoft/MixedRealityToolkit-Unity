// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Scriptable object that configures an elastic system used for rotation/quaternion values.
    /// </summary>
    [CreateAssetMenu(fileName = "RotationElasticConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Elastic/Rotation Elastic Configuration")]
    public class RotationElasticConfiguration : ElasticConfiguration
    {
        [SerializeField]
        [Tooltip("Properties of the rotation elastic extent.")]
        protected ElasticExtentProperties<Quaternion> extentProperties = new ElasticExtentProperties<Quaternion>
        {
            // Reasonable defaults.
            MinStretch = 0.0f,
            MaxStretch = 0.0f,
            SnapToEnds = false,
            SnapPoints = { },
            SnapRadius = 1.0f
        };

        /// <summary>
        /// Properties of the rotation elastic extent.
        /// </summary>
        public ElasticExtentProperties<Quaternion> ExtentProperties
        {
            get => extentProperties;
            set => extentProperties = value;
        }

        /// <summary>
        /// Factory method that allows subclasses of RotationElasticConfiguration to fabricate more
        /// specialized versions of a QuaternionElasticSystem if necessary, such as more advanced
        /// snapping behavior, alternative integrators/algorithms, or other specializations.
        /// </summary>
        /// <param name="initialValue"> The desired initial value for the oscillator sim. </param>
        /// <param name="initialVelocity"> The desired initial velocity for the oscillator sim. </param>
        /// <returns>
        /// A subclass of <see cref="ElasticSystem{Quaternion}"/> that can simulate damped harmonic
        /// oscillations over quaternion 4-space.
        /// </returns>
        public virtual ElasticSystem<Quaternion> MakeElasticSystem(Quaternion initialValue, Quaternion initialVelocity)
        {
            return new ElasticSystemQuaternion(initialValue, initialVelocity, extentProperties, elasticProperties);
        }
    }
}


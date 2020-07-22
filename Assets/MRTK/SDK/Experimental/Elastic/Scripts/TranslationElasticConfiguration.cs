// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Scriptable object that configures an elastic system used for translation/Vector3 values.
    /// </summary>
    [CreateAssetMenu(fileName = "TranslationElasticConfiguration", menuName = "Mixed Reality Toolkit/Experimental/Elastic/Translation Elastic Configuration")]
    public class TranslationElasticConfiguration : ElasticConfiguration
    {
        [SerializeField]
        [Tooltip("Properties of the rotation elastic extent.")]
        protected ElasticExtentProperties<Vector3> extentProperties = new ElasticExtentProperties<Vector3>
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
        public ElasticExtentProperties<Vector3> ExtentProperties
        {
            get => extentProperties;
            set => extentProperties = value;
        }

        /// <summary>
        /// Factory method that allows subclasses of TranslationElasticConfiguration to fabricate more
        /// specialized versions of a TranslationElasticSystem if necessary, such as more advanced
        /// snapping behavior, alternative integrators/algorithms, or other specializations.
        /// </summary>
        /// <param name="initialValue"> The desired initial value for the oscillator sim. </param>
        /// <param name="initialVelocity"> The desired initial velocity for the oscillator sim. </param>
        /// <returns>
        /// A subclass of <see cref="ElasticSystem{Vector3}"/> that can simulate damped harmonic
        /// oscillations over 3-space.
        /// </returns>
        public virtual ElasticSystem<Vector3> MakeElasticSystem(Vector3 initialValue, Vector3 initialVelocity)
        {
            return new ElasticSystem3D(initialValue, initialVelocity, extentProperties, elasticProperties);
        }
    }
}


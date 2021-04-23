// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests")]
namespace Microsoft.MixedReality.Toolkit.Experimental.Physics
{
    /// <summary>
    /// Represents a damped harmonic oscillator over an
    /// N-dimensional vector space, specified by generic type T.
    /// 
    /// This extensibility allows not just for 1, 2, and 3-D springs, but
    /// allows for 4-dimensional quaternion springs.
    /// </summary>
    public interface IElasticSystem<T>
    {
        /// <summary>
        /// Update the internal state of the damped harmonic oscillator,
        /// given the forcing/desired value, returning the new value.
        /// </summary>
        /// <param name="forcingValue">Forcing function, for example, a desired manipulation position.
        /// See https://en.wikipedia.org/wiki/Forcing_function_(differential_equations). It is a non-time-dependent
        /// input function to a differential equation; in our situation, it is the "input position" to the spring.</param>
        /// <param name="deltaTime">Amount of time that has passed since the last update.</param>
        /// <returns>The new value of the system.</returns>
        T ComputeIteration(T forcingValue, float deltaTime);

        /// <summary>
        /// Query the elastic system for the current instantaneous value
        /// </summary>
        /// <returns>Current value of the elastic system</returns>
        T GetCurrentValue();

        /// <summary>
        /// Query the elastic system for the current instantaneous velocity
        /// </summary>
        /// <returns>Current value of the elastic system</returns>
        T GetCurrentVelocity();
    }
}

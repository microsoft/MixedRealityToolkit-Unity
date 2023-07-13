// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable CS1591

using System.Collections;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    /// <summary>
    /// Base class to handle typical code setup/teardown and test utilities
    /// </summary>
    public abstract class BaseRuntimeTests
    {
        int originalTargetFrameRate = 0;

        /// <summary>
        /// Get the target framerate at which tests should run at.
        /// </summary>
        /// <remarks>
        /// This is used so frame timings to be consistent across various machine types. Thus ensure consistent test behavios.
        /// </remarks>
        protected virtual int TargetFrameRate { get; } = 60;

        /// <summary>
        /// Get the target frame time for these tests.
        /// </summary>
        protected float TargetFrameTime => 1.0f / TargetFrameRate;

        [UnitySetUp]
        public virtual IEnumerator Setup()
        {
            originalTargetFrameRate = UnityEngine.Application.targetFrameRate;
            UnityEngine.Application.targetFrameRate = TargetFrameRate;
            RuntimeTestUtilities.SetupScene();
            yield return null;
        }

        [UnityTearDown]
        public virtual IEnumerator TearDown()
        {
            RuntimeTestUtilities.TeardownScene();
            UnityEngine.Application.targetFrameRate = originalTargetFrameRate;
            yield return null;
        }
    }
}
#pragma warning restore CS1591
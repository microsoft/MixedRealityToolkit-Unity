// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    /// <summary>
    /// Base class to handle typical code setup/teardown and test utilities
    /// </summary>
    public abstract class BaseRuntimeTests
    {
        [UnitySetUp]
        public virtual IEnumerator Setup()
        {
            RuntimeTestUtilities.SetupScene();
            yield return null;
        }

        [UnityTearDown]
        public virtual IEnumerator TearDown()
        {
            RuntimeTestUtilities.TeardownScene();
            yield return null;
        }
    }
}
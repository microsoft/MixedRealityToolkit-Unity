// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable 1591

using System.Collections;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Performance.Tests
{
    internal class SmokeTest
    {
        [UnityTest]
        public IEnumerator PerformancePackageTest()
        {
            yield return null;
        }
    }
}
#pragma warning restore 1591
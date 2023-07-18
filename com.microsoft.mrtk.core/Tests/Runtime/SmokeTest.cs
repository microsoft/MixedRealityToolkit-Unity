// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for tests. While nice to have, this documentation is not required.
#pragma warning disable CS1591

using System.Collections;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    internal class SmokeTest
    {
        [UnityTest]
        public IEnumerator CorePackageTest()
        {
            yield return null;
        }
    }
}
#pragma warning restore CS1591
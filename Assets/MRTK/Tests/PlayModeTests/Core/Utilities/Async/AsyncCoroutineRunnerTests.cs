// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class AsyncCoroutineRunnerTests : BasePlayModeTests
    {
        /// <summary>
        /// Validates that the AsyncCoroutineRunner is automatically
        /// moved to the root to ensure that it has proper multi-scene lifetime behavior.
        /// </summary>
        [UnityTest]
        public IEnumerator TestNonRootParenting()
        {
            // Set up the AsyncCoroutineRunner to be parented under the MixedRealityPlayspace,
            // to validate that the runner will correctly unparent it.
            AsyncCoroutineRunner asyncCoroutineRunner = Object.FindObjectOfType<AsyncCoroutineRunner>();
            if (asyncCoroutineRunner == null)
            {
                asyncCoroutineRunner = new GameObject("AsyncCoroutineRunner").AddComponent<AsyncCoroutineRunner>();
            }
            GameObject mixedRealityPlayspace = GameObject.Find("MixedRealityPlayspace");
            asyncCoroutineRunner.transform.parent = mixedRealityPlayspace.transform;

            // Calling AsyncCoroutineRunner.Instance will cause the runner to move
            // the to root.
            Assert.IsNotNull(AsyncCoroutineRunner.Instance);
            Assert.IsNull(asyncCoroutineRunner.transform.parent);
            yield return null;
        }
    }
}

#endif

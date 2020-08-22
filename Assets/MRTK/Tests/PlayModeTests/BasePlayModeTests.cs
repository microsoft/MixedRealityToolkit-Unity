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

using System.Collections;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Base class to handle typical code setup/teardown and test utilities
    /// </summary>
    public abstract class BasePlayModeTests
    {
        [UnitySetUp]
        public virtual IEnumerator Setup()
        {
            PlayModeTestUtilities.Setup();
            yield return null;
        }

        [UnityTearDown]
        public virtual IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
        }
    }
}
#endif


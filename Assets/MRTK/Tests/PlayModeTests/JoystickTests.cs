// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    internal class JoystickTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #region Tests
        [UnityTest]
        public IEnumerator StubTest1()
        {
            yield return null;

            Assert.IsTrue(true);
        }

        [UnityTest]
        public IEnumerator StubTest2()
        {
            yield return null;

            Assert.IsTrue(true);
        }
        #endregion Tests
    }
}
#endif
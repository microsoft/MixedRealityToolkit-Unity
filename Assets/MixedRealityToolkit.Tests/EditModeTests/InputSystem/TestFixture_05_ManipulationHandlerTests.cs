// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
{
    public class TestFixture_05_ManipulationHandlerTests
    {
        [Test]
        public void Test01_TestSourceDetected()
        {
            TestUtilities.CleanupScene();
            MixedRealityToolkit.ConfirmInitialized();

            
        }

        [Test]
        public void Test02_TestSourceRemoved()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
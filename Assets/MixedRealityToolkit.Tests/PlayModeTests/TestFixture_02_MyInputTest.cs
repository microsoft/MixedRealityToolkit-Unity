// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class TestFixture_01_MyInputTest
    {
        [UnityTest]
        public IEnumerator Test01_MyInputTest()
        {
            var loadOp = TestUtilities.LoadTestSceneAsync("TestInputSystemGGV");
            while (loadOp.MoveNext())
            {
                yield return new WaitForFixedUpdate();
            }

            var playOp = TestUtilities.RunPlayableGraphAsync();
            while (playOp.MoveNext())
            {
                // INSERT TEST CONDITIONS HERE

                yield return new WaitForFixedUpdate();
            }
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
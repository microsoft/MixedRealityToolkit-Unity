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

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class DisableEnableInputSystemTest : BasePlayModeTests
    {
        [UnityTest]
        public IEnumerator DisableEnableInputSystem()
        {
            yield return null;

            TestContext.Out.WriteLine("Disable the input system");
            CoreServices.InputSystem.Disable();

            yield return new WaitForSeconds(2);

            TestContext.Out.WriteLine("Enable the input system");
            CoreServices.InputSystem.Enable();

            TestContext.Out.WriteLine("Display a test hand");
            TestHand rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(-0.3f, -0.1f, 0.5f));
        }
    }
}
#endif
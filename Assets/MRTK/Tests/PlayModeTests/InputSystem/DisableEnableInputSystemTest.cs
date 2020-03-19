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

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    /// <summary>
    /// Test suite to validate out-of-box input system can be disabled and enabled correctly
    /// </summary>
    public class DisableEnableInputSystemTest
    {
        [SetUp]
        public void SetUp()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }
        
        [UnityTest]
        public IEnumerator DisableEnableInputSystem()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = 3.0f * Vector3.forward;

            bool wasClicked = false;
            var interactable = cube.AddComponent<Interactable>();
            interactable.IsGlobal = true;
            interactable.OnClick.AddListener(() => { wasClicked = true; });

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();

            TestContext.Out.WriteLine("Disable the input system");
            CoreServices.InputSystem.Disable();

            yield return TestInputUtilities.ExecuteGlobalClick(() =>
            {
                Assert.IsTrue(wasClicked == false, "Input system should be disabled but Interactable was clicked");
                return null;
            });

            GameObject.Destroy(cube);

            yield return new WaitForSeconds(1);

            TestContext.Out.WriteLine("Enable the input system");
            CoreServices.InputSystem.Enable();

            TestContext.Out.WriteLine("Display the test hand");
            var rightHand = new TestHand(Handedness.Right);
            yield return rightHand.Show(new Vector3(-0.3f, -0.1f, 0.5f));

            yield return PlayModeTestUtilities.WaitForInputSystemUpdate();
            yield return rightHand.Hide();
        }
    }
}
#endif
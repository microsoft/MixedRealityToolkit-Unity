
# MRTK Tests

MRTK has a set of tests to ensure that changes to our code do not regress existing behavior. When you add a new feature, please:

1. Run the tests locally to make sure your changes don't regress existing behavior (you will not be able to check in if any tests fail)
2. Write new tests to ensure that other people don't break your feature in the future.

If you fix a bug, please consider writing a test to ensure that this bug doesn't regress in the future as well.

## Executing tests locally

The [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) can be found under
 Window > General > Test Runner and will show all available MRTK play and edit mode tests.

You can also run the [powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-6)
script located at `Scripts\test\run_playmode_tests.ps1`. This will run the playmode tests exactly as they are executed
on github / CI (see below), and print results. Here are some examples of how to run the script

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4.1f1

```ps
.\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.1f1\Editor\Unity.exe"
```

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4.1f1, output results to C:\playmode_test_out

```ps
.\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.1f1\Editor\Unity.exe" -outFolder "C:\playmode_test_out\"
```

## Executing tests on github / CI

MRTK's CI will build MRTK in all configurations and run all edit and play mode tests. CI can be triggered by posting a
comment on the github PR `/azp run mrtk_pr` if the user has sufficient rights. CI runs can be seen in the 'checks' tab of the PR.
Only after all of the tests passed successfully the PR can be merged into mrtk_development.

> [!NOTE]
> Some tests will only fail when run from the command line. You can run the tests locally from command line using similar
setup to what it done in MRTK's CI by running `scripts\test\run_playmode_tests.ps1`

## Writing Tests for your code

To ensure MRTK being a stable and reliable toolkit, every feature should come with unit tests and sample usage in one of the example scenes.

Preferably when fixing a bug there should also be a test added to avoid running into the same issue again in the future.

Having good test coverage in a big codebase like MRTK is crucial for stability and having confidence when doing changes in code.

MRTK uses the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) which uses a Unity
integration of [NUnit](https://nunit.org/).

This guide will give a starting point on how to add tests to MRTK. It will not explain the
[Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) and
[NUnit](https://nunit.org/) which can be looked up in the links provided.

There's two types of tests that can be added for new code

* Play mode tests
* Edit mode tests

## Play mode tests

 MRTK play mode tests have the ability to test how your new feature responds to different input sources such as hands or eyes.

 New play mode tests can inherit [BasePlayModeTests](xref:Microsoft.MixedReality.Toolkit.Tests.BasePlayModeTests) or the skeleton below can be used.

## Creating a new play mode test

To create a new play mode test:

- Navigate to Assets > MixedRealityToolkit.Tests > PlayModeTests
- Right click, Create > Testing > C# Test Script
- Replace the default template with the skeleton below

``` csharp
#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class ExamplePlayModeTests
    {
        // Setup a scene, initialize MRTK and the playspace - this method is called before the start of each test listed below
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();

            // Change the position of the main camera to (0, 0, 0), in PlayModeTestUtilities.Setup()
            // the camera is set to position (1, 1.5, -2)
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        // Destroy the scene - this method is called after each test listed below has completed
        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        #region Tests

        /// <summary>
        /// Skeleton for a new MRTK play mode test.
        /// </summary>
        [UnityTest]
        public IEnumerator TestMyFeature()
        {
            // ----------------------------------------------------------
            // EXAMPLE PLAY MODE TEST METHODS
            // ----------------------------------------------------------
            // Getting the input system
            // var inputSystem = PlayModeTestUtilities.GetInputSystem();

            // Creating a new test hand for input
            // var rightHand = new TestHand(Handedness.Right);
            // yield return rightHand.Show(new Vector3(0, 0, 0.5f));

            // Moving the new test hand
            // We are doing a yield return here because moving the hand to a new position
            // requires multiple frames to complete the action.
            // yield return rightHand.MoveTo(new Vector3(0, 0, 2.0f));

            // Getting a specific pointer from the hand
            // var linePointer = PointerUtils.GetPointer<LinePointer>(Handedness.Right);
            // Assert.IsNotNull(linePointer);
            // ---------------------------------------------------------

            // Your new test here
            yield return null;
        }
        #endregion
    }
}
#endif
```

## Edit mode tests

Edit mode tests are executed in Unity's edit mode and can be added in MixedRealityToolkit.Tests > EditModeTests.
To create a new test the following template can be used:

``` csharp
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class EditModeExampleTest
    {
        [Test]
        /// the name of this method will be used as test name in the unity test runner
        public void TestEditModeExampleFeature()
        {

        }
    }
}
```

## Test naming conventions

Tests should generally be named based on the class they are testing, or the scenario that they are testing.
For example, given a to-be-tested class:

```csharp
namespace Microsoft.MixedReality.Toolkit.Input
{
    class InterestingInputClass
    {
    }
}
```

Consider naming the test

```csharp
namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class InterestingInputClassTest
    {
    }
}
```

Consider placing the test in a folder hierarchy that is similar to its corresponding non-test file.
For example:

```
Non-Test: Assets/MixedRealityToolkit/Utilities/InterestingUtilityClass.cs
Test: Assets/MixedRealityToolkit.Tests/EditModeTests/Core/Utilities/InterestingUtilityClassTest.cs
```

This is to ensure that there's a clear an obvious way of finding each class's corresponding test class,
if such a test class exists.

Placement of scenario based tests is less defined - if the test exercises the overall input system,
for example, consider putting it into an "InputSystem" folder in the corresponding edit mode
or play mode test folder.

## Test script icons

When adding a new test, please modify the script to have the correct MRTK icon. There's an easy MRTK tool to do so:

1. Go go the Mixed Reality Toolkit menu item
1. Click on Utilities, then Update, then Icons
1. Click on Tests, and the updater will run automatically, updating any test scripts missing their icons

## MRTK Utility methods

This section shows some of the commonly used code snippets / methods when writing tests for MRTK.

There are two Utility classes that help with setting up MRTK and testing interactions with components in MRTK

* [`TestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities)
* [`PlayModeTestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities)

TestUtilities provide the following methods to set up your MRTK scene and GameObjects:

``` csharp
/// creates the mrtk GameObject and sets the default profile if passed param is true
TestUtilities.InitializeMixedRealityToolkit()

/// creates an empty scene prior to adding the mrtk GameObject to it
TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

/// sets the initial playspace transform and camera position
TestUtilities.InitializePlayspace();

/// destroys previously created mrtk GameObject and playspace
TestUtilities.ShutdownMixedRealityToolkit();
```

Please refer to the API docs of [`TestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities) and
[`PlayModeTestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities) for further methods
of these util classes as they're extended on a regular basis while new tests get added to MRTK.

## See also

* [Documentation portal generation guide](DevDocGuide.md)

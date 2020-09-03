# Writing and running tests in MRTK

To ensure MRTK is reliable, MRTK has a set of tests to ensure that changes to the code does not regress existing behavior. Having good test coverage in a big codebase like MRTK is crucial for stability and having confidence when making changes.

MRTK uses the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) which uses a Unity
integration of [NUnit](https://nunit.org/). This guide will provide a starting point on how to add tests to MRTK. It will not explain the
[Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) and
[NUnit](https://nunit.org/) which can be looked up in the links provided.

Before submitting a pull request, make sure to:

1. Run the tests locally so your changes don't regress existing behavior (completing PRs won't be allowed if any tests fail).

1. If fixing a bug, write a test to test the fix and ensure that future code modifications won't break it again.

1. If writing a feature, write new tests to prevent upcoming code changes breaking this feature.

Currently playmode tests are meant to be run in Unity 2018.4 and may fail in other versions of Unity

## Running tests

### Unity editor

The [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) can be found under **Window** > **General** > **Test Runner** and will show all available MRTK play and edit mode tests.

### Command line

Tests can also be run by a [powershell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell?view=powershell-6) script located at `Scripts\test\run_playmode_tests.ps1`. This will run the playmode tests exactly as they are executed on github / CI (see below), and print results. Here are some examples of how to run the script

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4 (for example Unity 2018.4.26f1)

```ps
.\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.26f1\Editor\Unity.exe"
```

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4, output results to C:\playmode_test_out

```ps
.\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.26f1\Editor\Unity.exe" -outFolder "C:\playmode_test_out\"
```

It's also possible to run the playmode tests multiple times via the `run_repeat_tests.ps1` script. All parameters used in `run_playmode_tests.ps1` may be used.

```ps
.\run_repeat_tests.ps1 -Times 5
```

### Pull request validation

MRTK's CI will build MRTK in all configurations and run all edit and play mode tests. CI can be triggered by posting a comment on the github PR `/azp run mrtk_pr` if the user has sufficient rights. CI runs can be seen in the 'checks' tab of the PR.

Only after all of the tests have passed successfully can the PR be merged into mrtk_development.

### Stress tests / bulk tests

Sometimes tests will only fail occasionally which can be frustrating to debug.

To have multiple test runs locally, modify the according test scripts. The following python script should make this scenario more convenient.

Prerequisite for running the python script is having [Python 3.X installed](https://www.python.org/downloads/).

For a single test that needs to be executed multiple times:

```c#
[UnityTest]
public IEnumerator MyTest() {...}
```

Run the following from a command line ([PowerShell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell?view=powershell-6#powershell-core) is recommended)

```powershell
cd scripts\tests
# Repeat the test 5 times. Default is 100
python .\generate_repeat_tests.py -n 5 -t MyTest
```

Copy and paste the output into your test file. The following script is for running multiple tests in sequence:

```powershell
cd scripts\tests
# Repeat the test 5 times. Default is 100
python .\generate_repeat_tests.py -n 5 -t MyTest MySecondTest
```

The new test file should now contain

```c#
[UnityTest]
public IEnumerator A1MyTest0(){ yield return MyTest();}
[UnityTest]
public IEnumerator A2MyTest0(){ yield return MyTest();}
[UnityTest]
public IEnumerator A3MyTest0(){ yield return MyTest();}
[UnityTest]
public IEnumerator A4MyTest0(){ yield return MyTest();}
[UnityTest]
public IEnumerator MyTest() {...}
```

Open the test runner and observe the new tests that can now be called repeatedly.

## Writing tests

There are two types of tests that can be added for new code

* Play mode tests
* Edit mode tests

### Play mode tests

MRTK play mode tests have the ability to test how your new feature responds to different input sources such as hands or eyes.

New play mode tests can inherit [BasePlayModeTests](xref:Microsoft.MixedReality.Toolkit.Tests.BasePlayModeTests) or the skeleton below can be used.

To create a new play mode test:

* Navigate to Assets > MRTK > Tests > PlayModeTests
* Right click, Create > Testing > C# Test Script
* Replace the default template with the skeleton below

```c#
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
        // This method is called once before we enter play mode and execute any of the tests
        // do any kind of setup here that can't be done in playmode
        public void Setup()
        {
            // eg installing unity packages is only possible in edit mode
            // so if a test requires TextMeshPro we will need to check for the package before entering play mode
            PlayModeTestUtilities.InstallTextMeshProEssentials();
        }

        // Do common setup for each of your tests here - this will be called for each individual test after entering playmode
        // Note that this uses UnitySetUp instead of [SetUp] because the init function needs to await a frame passing
        // to ensure that the MRTK system has had the chance to fully set up before the test runs.
        [UnitySetUp]
        public IEnumerator Init()
        {
            // in most play mode test cases you would want to at least create an MRTK GameObject using the default profile
            TestUtilities.InitializeMixedRealityToolkit(true);
            yield return null;
        }

        // Destroy the scene - this method is called after each test listed below has completed
        // Note that this uses UnityTearDown instead of [TearDown] because the init function needs to await a frame passing
        // to ensure that the MRTK system has fully torn down before the next test setup->run cycle starts.
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            PlayModeTestUtilities.TearDown();
            yield return null;
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

### Edit mode tests

Edit mode tests are executed in Unity's edit mode and can be added under the **MRTK** > **Tests** > **EditModeTests** folder in the Mixed Reality Toolkit repo.
To create a new test the following template can be used:

```c#
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

### Test naming conventions

Tests should generally be named based on the class they are testing, or the scenario that they are testing.
For example, given a to-be-tested class:

```c#
namespace Microsoft.MixedReality.Toolkit.Input
{
    class InterestingInputClass
    {
    }
}
```

Consider naming the test

```c#
namespace Microsoft.MixedReality.Toolkit.Tests.Input
{
    class InterestingInputClassTest
    {
    }
}
```

Consider placing the test in a folder hierarchy that is similar to its corresponding non-test file.
For example:

```md
Non-Test: Assets/MRTK/Core/Utilities/InterestingUtilityClass.cs
Test: Assets/MRTK/Tests/EditModeTests/Core/Utilities/InterestingUtilityClassTest.cs
```

This is to ensure that there's a clear an obvious way of finding each class's corresponding test class,
if such a test class exists.

Placement of scenario based tests is less defined - if the test exercises the overall input system,
for example, consider putting it into an "InputSystem" folder in the corresponding edit mode
or play mode test folder.

### Test script icons

When adding a new test, please modify the script to have the correct MRTK icon. There's an easy MRTK tool to do so:

1. Go go the Mixed Reality Toolkit menu item
1. Click on Utilities, then Update, then Icons
1. Click on Tests, and the updater will run automatically, updating any test scripts missing their icons

### MRTK Utility methods

This section shows some of the commonly used code snippets / methods when writing tests for MRTK.

There are two Utility classes that help with setting up MRTK and testing interactions with components in MRTK

* [`TestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities)
* [`PlayModeTestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities)

TestUtilities provide the following methods to set up your MRTK scene and GameObjects:

```c#
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

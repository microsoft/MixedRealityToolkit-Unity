
# MRTK Tests

MRTK has a set of tests to ensure that changes to our code do not regress existing behavior. When you add a new feature, please:

1. Run the tests locally to make sure your changes don't regress existing behavior (you will not be able to check in if any tests fail)
2. Write new tests to ensure that other people don't break your feature in the future.

If you fix a bug, please consider writing a test to ensure that this bug doesn't regress in the future as well.

## Executing tests locally

The [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) can be found under Window > General > Test Runner and will show all available MRTK play and edit mode tests.

You can also run the [powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-6) script located at `Scripts\test\run_playmode_tests.ps1`. This will run the playmode tests exactly as they are executed on github / CI (see below), and print results. Here are some examples of how to run the script

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4.1f1

```
    .\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.1f1\Editor\Unity.exe"
```

Run the tests on the project located at H:\mrtk.dev, with Unity 2018.4.1f1, output results to C:\playmode_test_out

```
    .\run_playmode_tests.ps1 H:\mrtk.dev -unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.1f1\Editor\Unity.exe" -outFolder "C:\playmode_test_out\"
```


## Executing tests on github / CI
MRTK's CI will build MRTK in all configurations and run all edit and play mode tests. CI can be triggered by posting a comment on the github PR `/azp run mrtk_pr` if the user has sufficient rights. CI runs can be seen in the 'checks' tab of the PR. 

Only after all of the tests passed successfully the PR can be merged into mrtk_development. 

> [!NOTE]
> Some tests will only fail when run from the command line. You can run the tests locally from command line using similar setup to what it done in MRTK's CI by running `scripts\test\run_playmode_tests.ps1`

## Writing Tests for your code
To ensure MRTK being a stable and reliable toolkit, every feature should come with unit tests and sample usage in one of the example scenes.

Preferably when fixing a bug there should also be a test added to avoid running into the same issue again in the future.

Having good test coverage in a big codebase like MRTK is crucial for stability and having confidence when doing changes in code.


MRTK uses the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) which uses a Unity integration of [NUnit](https://nunit.org/). 

This guide will give a starting point on how to add tests to MRTK. It will not explain the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) and [NUnit](https://nunit.org/) which can be looked up in the links provided.

There's two types of tests that can be added for new code

* Edit mode tests
* Play mode tests

## Play mode tests

Play mode tests will be executed in Unity's play mode and should be added into MixedRealityToolkit.Tests > PlaymodeTests. 
To create a new test the following template can be used:

``` csharp
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
using UnityEngine.TestTools;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ExampleTest : IPrebuildSetup
    {

        // this method is called once before we enter play mode and execute any of the tests
        // do any kind of setup here that can't be done in playmode
        public void Setup()
        {
            // eg installing unity packages is only possible in edit mode 
            // so if a test requires TextMeshPro we will need to check for the package before entering play mode
            PlayModeTestUtilities.EnsureTextMeshProEssentials();
        }

        // do common setup for each of your tests here - this will be called for each individual test after entering playmode
        [Setup]
        public void Init()
        {
            // in most play mode test cases you would want to at least create an MRTK GameObject using the default profile
            TestUtilities.InitializeMixedRealityToolkit(true);
        }


        // destroy commonly initialized objects here - this will be called after each of your tests has finished
        [TearDown]
        public void Shutdown()
        {
            // call shutdown if you've created an mrtk GameObject in your test
            TestUtilities.ShutdownMixedRealityToolkit();
        }


        #region Tests

        [UnityTest]
        /// the name of this method will be used as test name in the unity test runner
        public IEnumerator TestMyFeature()
        {
            // write your test here
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

Please refer to the API docs of [`TestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities) and [`PlayModeTestUtilities`](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities) for further methods of these util classes as they're extended on a regular basis while new tests get added to MRTK.

## See also

* [Documentation portal generation guide](DevDocGuide.md)

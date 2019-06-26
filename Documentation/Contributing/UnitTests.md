
# Writing tests for your code
To ensure MRTK being a stable and reliable toolkit, every feature should come with unit tests and some application in one of the example scenes.

Preferrably when fixing a bug there should also be a test added to avoid running into the same issue again in the future.

Having good test coverage in a big codebase like MRTK is crucial for stability and having confidence when doing changes in code.


MRTK uses the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) which uses a Unity integration of [NUnit](https://nunit.org/). 

This guide will give a starting point on how to add tests to MRTK. It will not explain the [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) and [NUnit](https://nunit.org/) which can be looked up in the links provided.

There's two types of tests that can be added for new code

* Edit mode tests
* Play mode tests

## Play mode tests

Play mode tests will be executed in Unity's play mode and should be added into MixedRealityToolkit.Tests > PlaymodeTests. 
To create a new test the following template can be used: (todo: Setup() and teardown should be moved into a base class of mrtk tests)

``` csharp
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ExampleTest : IPrebuildSetup
    {

        // only add this if your test is using a textmeshpro component
        public void Setup()
        {
            PlayModeTestUtilities.EnsureTextMeshProEssentials();
        }


        // add a teardown if you created an mrtk gameobject in your test
        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }


        #region Tests

        [UnityTest]
        /// the name of this method will be used as test name in the unity test runner
        public IEnumerator TestMyFeature()
        {
            // in most play mode test cases you would want to create an MRTK gameobject using the default profile
            TestUtilities.InitializeMixedRealityToolkit(true);

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

There's two Utility classes that help with setting up MRTK and testing interactions with components in MRTK
* [TestUtilities](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities)
* [PlayModeTestUtilities](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities) 

TestUtilities provide the following methods to set up your MRTK scene and gameobjects

``` csharp
/// creates the mrtk gameobject and sets the default profile if passed param is true
TestUtilities.InitializeMixedRealityToolkit()

/// creates an empty scene prior to adding the mrtk gameobject to it
TestUtilities.InitializeMixedRealityToolkitAndCreateScenes();

/// sets the initial playspace transform and camera position
TestUtilities.InitializePlayspace();

/// destroying previously created mrtk gameobject and playspace
TestUtilities.ShutdownMixedRealityToolkit();
```

Please refer to the API docs of [TestUtilities](xref:Microsoft.MixedReality.Toolkit.Tests.TestUtilities) and [PlayModeTestUtilities](xref:Microsoft.MixedReality.Toolkit.Tests.PlayModeTestUtilities) for further methods of these util classes as they're extended on a regular basis while new tests get added to MRTK.


## Executing tests locally
The [Unity Test Runner](https://docs.unity3d.com/Manual/testing-editortestsrunner.html) can be found under Window > General > Test Runner and will show all available MRTK play and edit mode tests. 


## Executing tests on github / CI
MRTK's CI will build MRTK in all configurations and run all edit and play mode tests. CI can be triggered by posting a comment on the github PR `/azp run mrtk_pr` if the user has sufficient rights. CI runs can be seen in the 'checks' area of the PR. 

Only after all of the tests passed successfully the PR can be merged into mrtk_development. 

## See also
* [Documentation portal generation guide](DevDocGuide.md)


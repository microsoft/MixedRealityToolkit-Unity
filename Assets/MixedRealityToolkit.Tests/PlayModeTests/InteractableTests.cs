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

using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    class InteractableTests
    {
        /// <summary>
        /// Tests that an interactable component can be added to a GameObject
        /// at runtime.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestAddInteractableAtRuntime()
        {
            TestUtilities.InitializeMixedRealityToolkitAndCreateScenes(true);
            TestUtilities.InitializePlayspace();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // This should not throw an exception
            var interactable = cube.AddComponent<Interactable>();

            // clean up
            GameObject.Destroy(cube);
            yield return null;
        }

        [TearDown]
        public void ShutdownMrtk()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }
    }
}
#endif
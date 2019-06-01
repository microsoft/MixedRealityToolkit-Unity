// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    }
}
#endif
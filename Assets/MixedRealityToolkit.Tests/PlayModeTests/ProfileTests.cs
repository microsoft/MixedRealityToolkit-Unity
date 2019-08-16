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
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Editor;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class ProfileTests
    {
        [SetUp]
        public void SetUp()
        {
            TestUtilities.InitializeMixedRealityToolkit(true);
            TestUtilities.PlayspaceToOriginLookingForward();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.ShutdownMixedRealityToolkit();
        }

        private const string HoloLens1ProfileName = "DefaultHoloLens1ConfigurationProfile";
        /// <summary>
        /// Test that HoloLens 1 profile acts as expected (e.g. when hands are up there are no hand rays)
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestHL1Profile()
        {
            var hl1Profile = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>()
                .FirstOrDefault(x => x.name.Equals(HoloLens1ProfileName));
            MixedRealityToolkit.Instance.ActiveProfile = hl1Profile;

            TestHand hand = new TestHand(Handedness.Right);
            yield return hand.Show(Vector3.forward);

            // https://nunit.org/docs/2.5.5/collectionConstraints.html
            foreach(var i in CoreServices.InputSystem.DetectedInputSources)
            {
                Assert.That(i.Pointers, Has.Some.InstanceOf(typeof(GGVPointer)));
                Assert.That(i.Pointers, Has.No.InstanceOf(typeof(ShellHandRayPointer)));
            }
        }
    }
}

#endif

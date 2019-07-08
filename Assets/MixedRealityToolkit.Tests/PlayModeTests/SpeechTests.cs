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
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using System.Runtime.InteropServices;
using System;
using Microsoft.MixedReality.Toolkit.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class SpeechTests
    {
        [SetUp]
        public void Setup()
        {
            PlayModeTestUtilities.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            PlayModeTestUtilities.TearDown();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestToggleProfilerCommand()
        {
            // Confirm that the Diagnostics system is enabled and the VisualProfiler is in the scene.
            IMixedRealityDiagnosticsSystem diagnosticsSystem = null;
            MixedRealityServiceRegistry.TryGetService<IMixedRealityDiagnosticsSystem>(out diagnosticsSystem);
            Assert.IsNotNull(diagnosticsSystem, "The diagnostics system is not enabled in the scene.");
            yield return null;

            // Verfiy that the VisualProfiler is enabled.
            Assert.IsTrue(diagnosticsSystem.ShowProfiler, "The VisualProfiler is not active.");
            yield return null;

            // Toggle the profiler visualization off.
            KeyboardEvent(VirtualKey_9, ScanCode_9, 0, IntPtr.Zero); // key down
            yield return null;
            KeyboardEvent(VirtualKey_9, ScanCode_9, KeyboardFlag_KeyUp, IntPtr.Zero); // key up
            for (int i = 0; i < 10; i++) { yield return null; }

            // Verify that the VisualProfiler is disabled.
            Assert.IsFalse(diagnosticsSystem.ShowProfiler, "The VisualProfiler is active (should be inactive).");
            yield return null;

            // Toggle the profiler visualization on.
            KeyboardEvent(VirtualKey_9, ScanCode_9, 0, IntPtr.Zero); // key down
            yield return null;
            KeyboardEvent(VirtualKey_9, ScanCode_9, KeyboardFlag_KeyUp, IntPtr.Zero); // key up
            for (int i = 0; i < 10; i++) { yield return null; }

            // Verfiy that the VisualProfiler is enabled.
            Assert.IsTrue(diagnosticsSystem.ShowProfiler, "The VisualProfiler is inactive (should be active).");
            yield return null;
        }

        /// <summary>
        /// Virutal Key Code and Scan Code values used in this test.
        /// </summary>
        private const Byte VirtualKey_9 = 0x39;
        private const Byte ScanCode_9 = 0x0A;

        /// <summary>
        /// Flags, for the KeyboardEvent function, used in this test.
        /// </summary>
        private const UInt32 KeyboardFlag_KeyUp = 0x0002;

        /// <summary>
        /// P/Invoke signature for the Windows keybd_event function.
        /// </summary>
        /// <param name="keyCode">Virtual Key Code for the desired keyboard event.</param>
        /// <param name="scanCode">Scan Code for the desired keybaord event.</param>
        /// <param name="flags">Flags used to interpret the keyboard event.</param>
        /// <param name="extraInfo">Additional information associated with the keyboard event.</param>
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        private static extern void KeyboardEvent(
            Byte keyCode,
            Byte scanCode,
            UInt32 flags,
            IntPtr extraInfo);
    }
}
#endif

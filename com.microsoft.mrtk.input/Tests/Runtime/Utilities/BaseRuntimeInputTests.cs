// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Abstract base class for all automated tests that require input simulation.
    /// The included setup and teardown methods will setup/teardown the MRTK rig,
    /// as well as the simulated input devices.
    /// </summary>
    public abstract class BaseRuntimeInputTests : BaseRuntimeTests
    {
        // Isolates/sandboxes the input system state for each test instance.
        private InputTestFixture input = new InputTestFixture();

        // We only want to isolate the input system state when a test is running in batch mode.
        // This is indicated by the test either running in the background or explicitly in batch mode.
        // We do this because some runtime tests utilities rely on keyboard input, and isolating the
        // input system state means that the phyiscal keyboard is never registered with the application
        private bool useInputFixture => !Application.isFocused || Application.isBatchMode;

        private XRInteractionManager cachedInteractionManager = null;

        /// <summary>
        /// A cached reference to the <see cref="MRTKInteractionManager"/> on the XR rig.
        /// Cleared during <see cref="TearDown"/> at the end of each test.
        /// </summary>
        protected XRInteractionManager CachedInteractionManager
        {
            get
            {
                if (cachedInteractionManager == null)
                {
                    cachedInteractionManager = UnityEngine.Object.FindObjectOfType<XRInteractionManager>();
                }
                return cachedInteractionManager;
            }
        }

        private ControllerLookup cachedLookup = null;

        /// <summary>
        /// A cached reference to the <see cref="ControllerLookup"/> on the XR rig.
        /// Cleared during <see cref="TearDown"/> at the end of each test.
        /// </summary>
        protected ControllerLookup CachedLookup
        {
            get
            {
                if (cachedLookup == null)
                {
                    if (CachedInteractionManager == null)
                    {
                        return null;
                    }
                    cachedLookup = CachedInteractionManager.gameObject.GetComponent<ControllerLookup>();
                }
                return cachedLookup;
            }
        }

        public override IEnumerator Setup()
        {
            yield return base.Setup();

            if (useInputFixture)
            {
                input.Setup();

                // XRI needs these... ugh
                InputSystem.RegisterInteraction<SectorInteraction>();
                InputSystem.RegisterBindingComposite<Vector3FallbackComposite>();
                InputSystem.RegisterBindingComposite<QuaternionFallbackComposite>();
            }

            InputTestUtilities.InstantiateRig();
            InputTestUtilities.SetupSimulation();
            yield return null;
        }

        public override IEnumerator TearDown()
        {
            yield return null; // Make sure the input system gets one last tick.
            InputTestUtilities.TeardownRig();
            InputTestUtilities.TeardownSimulation();
            cachedInteractionManager = null;
            cachedLookup = null;

            if (useInputFixture)
            {
                input.TearDown();
            }

            yield return base.TearDown();
        }
    }
}


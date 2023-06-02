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
                    cachedInteractionManager = Object.FindObjectOfType<XRInteractionManager>();
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

            input.Setup();

            // XRI needs these... ugh
            InputSystem.RegisterInteraction<SectorInteraction>();
            InputSystem.RegisterBindingComposite<Vector3FallbackComposite>();
            InputSystem.RegisterBindingComposite<QuaternionFallbackComposite>();
            InputSystem.RegisterBindingComposite<IntegerFallbackComposite>();

            InputTestUtilities.InstantiateRig();
            InputTestUtilities.SetupSimulation(0.0f);

            // Wait for simulation HMD to update camera poses
            yield return RuntimeTestUtilities.WaitForUpdates();
        }

        public override IEnumerator TearDown()
        {
            yield return null; // Make sure the input system gets one last tick.
            InputTestUtilities.TeardownRig();
            InputTestUtilities.TeardownSimulation();
            cachedInteractionManager = null;
            cachedLookup = null;

            input.TearDown();

            yield return base.TearDown();
        }
    }
}


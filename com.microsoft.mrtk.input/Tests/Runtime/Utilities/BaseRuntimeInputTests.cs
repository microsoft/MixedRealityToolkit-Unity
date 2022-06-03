// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Core.Tests;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input.Tests
{
    /// <summary>
    /// Abstract base class for all automated tests that require input simulation.
    /// The included setup and teardown methods will setup/teardown the MRTK rig,
    /// as well as the simulated input devices.
    /// </summary>
    public abstract class BaseRuntimeInputTests : BaseRuntimeTests
    {
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
            InputTestUtilities.InstantiateRig();
            InputTestUtilities.SetupSimulation();
            yield return null;
        }

        public override IEnumerator TearDown()
        {
            InputTestUtilities.TeardownRig();
            InputTestUtilities.TeardownSimulation();
            cachedInteractionManager = null;
            cachedLookup = null;
            yield return base.TearDown();
        }
    }
}


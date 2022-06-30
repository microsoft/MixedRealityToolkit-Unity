// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Subsystem that polyfills missing hand interaction input actions for platforms that
    /// don't provide interaction profiles for hand tracking (Quest, Varjo, etc)
    /// </summary>
    [Preserve]
    [MRTKSubsystem(
         Name = "com.microsoft.mixedreality.interactionpolyfill",
         DisplayName = "MRTK Interaction Polyfill Subsystem",
         Author = "Microsoft",
         ProviderType = typeof(Provider),
         ConfigType = typeof(MRTKHandsAggregatorConfig))]
    public class InteractionPolyfillSubsystem :
        MRTKSubsystem<InteractionPolyfillSubsystem,
            InteractionPolyfillSubsystemDescriptor,
            InteractionPolyfillSubsystem.Provider>
    {
        /// <summary>
        /// Construct the <c>InteractionPolyfillSubsystem</c>.
        /// </summary>
        public InteractionPolyfillSubsystem()
        { }

        /// <summary>
        /// Provider for <c>InteractionPolyfillSubsystem</c>.
        /// </summary>
        public class Provider : MRTKSubsystemProvider<InteractionPolyfillSubsystem>
        {
            public override void Start()
            {
                base.Start();

                Debug.Log("Polyfill startup");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            Debug.Log("Constructing cinfo");
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<InteractionPolyfillSubsystem, InteractionPolyfillSubsystemCinfo>();
            var descriptor = InteractionPolyfillSubsystemDescriptor.Create(cinfo);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            Debug.Log("Registered descriptor, cinfo = " + cinfo + ", descriptor = " + descriptor);
        }
    }

}

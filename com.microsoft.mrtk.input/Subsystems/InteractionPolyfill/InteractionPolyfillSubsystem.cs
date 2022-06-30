// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Preserve]
    /// <summary>
    /// Subsystem that polyfills missing hand interaction input actions for platforms that
    /// don't provide interaction profiles for hand tracking (Quest, Varjo, etc)
    /// </summary>
    internal class InteractionPolyfillSubsystem :
        MRTKSubsystem<InteractionPolyfillSubsystem,
            MRTKSubsystemDescriptor<InteractionPolyfillSubsystem, InteractionPolyfillSubsystem.Provider>,
            InteractionPolyfillSubsystem.Provider>
    {
        /// <summary>
        /// Blank base implementation provider.
        /// </summary>
        public class Provider : MRTKSubsystemProvider<InteractionPolyfillSubsystem> { }

        /// <summary>
        /// Registers a descriptor for the subsystem
        /// with the provided <see cref="MRTKSubsystemCinfo"/> parameters.
        /// </summary>
        public static void Register(MRTKSubsystemCinfo cinfo)
        {
            var descriptor = MRTKSubsystemDescriptor<InteractionPolyfillSubsystem, Provider>.Create(cinfo);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
        }
    }
    
    [Preserve]
    [MRTKSubsystem(
         Name = "com.microsoft.mixedreality.interactionpolyfill",
         DisplayName = "MRTK Interaction Polyfill Subsystem",
         Author = "Microsoft",
         ProviderType = typeof(MRTKProvider),
         SubsystemTypeOverride = typeof(MRTKInteractionPolyfillSubsystem),
         ConfigType = typeof(MRTKHandsAggregatorConfig))]
    internal class MRTKInteractionPolyfillSubsystem : InteractionPolyfillSubsystem
    {
        /// <summary>
        /// Provider for <c>InteractionPolyfillSubsystem</c>.
        /// </summary>
        public class MRTKProvider : Provider
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
            var cinfo = XRSubsystemHelpers.ConstructCinfo<MRTKInteractionPolyfillSubsystem, MRTKSubsystemCinfo>();
            Register(cinfo);
        }
    }

}

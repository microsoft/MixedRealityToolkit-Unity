// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Configuration profile settings for the Mixed Reality Toolkit
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping")]
    public class MixedRealityControllerMapping : ScriptableObject, ISerializationCallbackReceiver
    {
        //TODO - Does this also need to track Axis?

        [SerializeField]
        [Tooltip("Enable the Motion Controllers on Startup")]
        private Handedness controlingHand;

        public Handedness ControllingHand { get { return controlingHand; } }

        [SerializeField]
        [Tooltip("Input System Class to instantiate at runtime.")]
        [Implements(typeof(IMixedRealityController), ClassGrouping.None)]
        private SystemType controllerType;

        public SystemType ControllerType { get { return controllerType; } }


        [SerializeField]
        [Tooltip("Input System Class to instantiate at runtime.")]
        private InteractionDefinitionMapping[] interactions;

        public InteractionDefinitionMapping[] Interactions { get { return interactions == null ? new InteractionDefinitionMapping[0] : interactions; } }

        public InteractionDefinitionMapping GetInteractionmapping(InputType inputType)
        {
            return default(InteractionDefinitionMapping);
        }

        #region ISerializationCallbackReceiver Implementation

        /// <summary>
        /// Unity function to prepare data for serialization.
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //var count = ActiveManagers.Count;
            //initialManagers = new IMixedRealityManager[count];
            //initialManagerTypes = new Type[count];

            //foreach (var manager in ActiveManagers)
            //{
            //    --count;
            //    initialManagers[count] = manager.Value;
            //    initialManagerTypes[count] = manager.Key;
            //}
        }

        /// <summary>
        /// Unity function to resolve data from serialization when a project is loaded
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // From the serialized fields for the MixedRealityConfigurationProfile, populate the Active managers list
            // *Note This will only take effect once the Mixed Reality Toolkit has a custom editor for the MixedRealityConfigurationProfile

            //ActiveManagers.Clear();
            //for (int i = 0; i < initialManagers?.Length; i++)
            //{
            //    Managers.MixedRealityManager.Instance.AddManager(initialManagerTypes[i], initialManagers[i]);
            //}
        }

        #endregion  ISerializationCallbackReceiver Implementation
    }
}

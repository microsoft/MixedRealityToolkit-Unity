// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Describes the types of interaction modes an interactor can belong to.
    /// </summary>
    [Serializable]
    public class InteractionModeDefinition : ISerializationCallbackReceiver
    {
        [SerializeField]
        [Tooltip("Get the mode name that this Interaction Mode Definition instance is targeting.")]
        private string modeName = string.Empty;

        /// <summary>
        /// Get the mode name that this <see cref="InteractionModeDefinition"/> instance is targeting.
        /// </summary>
        public string ModeName => modeName;

        // private field to ensure serialization
        [SerializeField]
        [Extends(typeof(XRBaseControllerInteractor), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The class types of the interactors that this Interaction Mode Definition instance is targeting.")]
        private List<SystemType> associatedTypes = new List<SystemType>();

        private HashSet<Type> associatedTypesHashSet = new HashSet<Type>();
        
        /// <summary>
        /// Stores the types associated with this Interaction Mode Definition.
        /// </summary>
        internal HashSet<Type> AssociatedTypes => associatedTypesHashSet;

        /// <summary>
        /// Constructor for a mode definition, requires a name and the interactor types which are to be enabled while in this mode.
        /// </summary>
        public InteractionModeDefinition(string name, List<SystemType> associatedTypes)
        {
            modeName = name;
            this.associatedTypes = associatedTypes.Distinct().ToList();
            InitializeAssociatedTypes();
        }

        internal void InitializeAssociatedTypes()
        {
            // load contents from the SystemType List into the Type HashSet
            associatedTypesHashSet ??= new HashSet<Type>();
            associatedTypesHashSet.Clear();

            foreach (SystemType allowedType in associatedTypes)
            {
                associatedTypesHashSet.Add(allowedType);
            }
        }

        /// <summary>
        /// Implemented so to receive a callback after Unity deserializes this object.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            InitializeAssociatedTypes();
        }

        /// <summary>
        /// Implemented so to receive a callback before Unity deserializes this object.
        /// </summary>
        /// <remarks>
        /// This is currently not utilized, and no operation will be preformed when called.
        /// </remarks>
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}

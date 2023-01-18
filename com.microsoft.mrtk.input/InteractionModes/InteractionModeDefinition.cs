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
    /// Describes the types of interaction modes an interactor can belong to
    /// </summary>
    /// todo: improve naming here...
    [Serializable]
    public class InteractionModeDefinition : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string modeName = string.Empty;

        public string ModeName => modeName;

        // private field to ensure serialization
        [SerializeField]
        [Extends(typeof(XRBaseControllerInteractor), TypeGrouping.ByNamespaceFlat)]
        private List<SystemType> associatedTypes = new List<SystemType>();

        private HashSet<Type> associatedTypesHashSet = new HashSet<Type>();
        
        /// <summary>
        /// Stores the types associated with this Interaction Mode Definition
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

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            InitializeAssociatedTypes();
        }

        // We don't need to do anything before serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
    }
}

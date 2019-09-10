// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "States", menuName = "Mixed Reality Toolkit/Interactable/State", order = 1)]
    public class States : ScriptableObject
    {
        [FormerlySerializedAs("StateList")]
        [SerializeField]
        private List<State> stateList = new List<State>();

        /// <summary>
        /// List of available states defined by asset
        /// </summary>
        public List<State> StateList
        {
            get {return stateList;}
            set { stateList = value; }
        }

        [FormerlySerializedAs("DefaultIndex")]
        [SerializeField]
        private int defaultIndex = 0;

        /// <summary>
        /// Default index into state list
        /// </summary>
        public int DefaultIndex
        {
            get { return defaultIndex; }
            set { defaultIndex = value; }
        }

        /// <summary>
        /// Defines the type of State Model to associate with this States asset. Type must be a class that extends InteractableStateModel
        /// </summary>
        public Type StateModelType
        {
            get
            {
                if (stateModelType == null)
                {
                    if (string.IsNullOrEmpty(AssemblyQualifiedName))
                    {
                        return null;
                    }

                    stateModelType = Type.GetType(AssemblyQualifiedName);
                }

                return stateModelType;
            }
            set
            {
                if (!value.IsSubclassOf(typeof(InteractableStateModel)))
                {
                    Debug.LogWarning($"Cannot assign type {value} that does not extend {typeof(InteractableStateModel)} to ThemeDefinition");
                    return;
                }

                if (stateModelType != value)
                {
                    stateModelType = value;
                    StateModelClassName = stateModelType.Name;
                    AssemblyQualifiedName = stateModelType.AssemblyQualifiedName;
                }
            }
        }

        // Unity cannot serialize System.Type, thus must save AssemblyQualifiedName
        // Field here for Runtime use
        private Type stateModelType;

        [FormerlySerializedAs("StateLogicName")]
        [SerializeField]
        private string StateModelClassName;

        [SerializeField]
        private string AssemblyQualifiedName;

        public States()
        {
            // Set default type
            StateModelType = typeof(InteractableStates);
        }

        public State[] GetStates()
        {
            return StateList.ToArray();
        }

        public bool CompareStates(States other)
        {
            if (this.StateList.Count != other.StateList.Count 
                || this.StateModelType != other.StateModelType 
                || this.DefaultIndex != other.DefaultIndex)
            {
                return false;
            }

            for (int i = 0; i < this.StateList.Count; i++)
            {
                if (!this.StateList[i].CompareState(other.StateList[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public InteractableStateModel CreateStateModel()
        {
            InteractableStateModel stateLogic = (InteractableStateModel)Activator.CreateInstance(StateModelType, StateList[DefaultIndex]);

            List<State> stateListCopy = new List<State>();
            for (int i = 0; i < StateList.Count; i++)
            {
                State state = new State();
                state.ActiveIndex = StateList[i].ActiveIndex;
                state.Bit = StateList[i].Bit;
                state.Index = StateList[i].Index;
                state.Name = StateList[i].Name;
                state.Value = StateList[i].Value;
                stateListCopy.Add(state);
            }

            stateLogic.ImportStates(stateListCopy);

            return stateLogic;
        }
    }
}

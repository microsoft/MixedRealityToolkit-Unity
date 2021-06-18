// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// States scriptableObject for storing available states and related state model
    /// </summary>
    [CreateAssetMenu(fileName = "States", menuName = "Mixed Reality/Toolkit/State", order = 1)]
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
            get { return stateList; }
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
                if (!value.IsSubclassOf(typeof(BaseStateModel)))
                {
                    Debug.LogWarning($"Cannot assign type {value} that does not extend {typeof(BaseStateModel)} to ThemeDefinition");
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

        [System.Obsolete("Use the StateList property instead")]
        public State[] GetStates()
        {
            return StateList.ToArray();
        }

        /// <summary>
        /// Test whether the current States object and the argument States object have the same internal values and configurations
        /// </summary>
        /// <param name="other">other States object to compare against self</param>
        /// <returns>true if internal list of state values and class configuration matches other, false otherwise</returns>
        public bool Equals(States other)
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

        /// <summary>
        /// Create a State Model class and initialize it with the configuration data from this States ScriptableObject
        /// </summary>
        /// <returns>BaseStateModel or inherited class implementation object initialized with the StateList in this ScriptableObject</returns>
        public BaseStateModel CreateStateModel()
        {
            BaseStateModel stateLogic = (BaseStateModel)Activator.CreateInstance(StateModelType, StateList[DefaultIndex]);

            List<State> stateListCopy = new List<State>();
            foreach (State s in StateList)
            {
                stateListCopy.Add(s.Copy());
            }

            stateLogic.ImportStates(stateListCopy);

            return stateLogic;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [CreateAssetMenu(fileName = "States", menuName = "Mixed Reality Toolkit/Interactable/State", order = 1)]
    public class States : ScriptableObject
    {
        public List<State> StateList;
        public int DefaultIndex = 0;
        public Type StateType;
        public InteractableTypesContainer StateOptions;
        public string StateLogicName = "InteractableStates";
        public string AssemblyQualifiedName = typeof(InteractableStates).AssemblyQualifiedName;

        /// <summary>
        /// The list of base classes whose derived classes will be included in interactable state
        /// selection dropdowns.
        /// </summary>
        private static readonly List<Type> candidateStateTypes = new List<Type>() { typeof(InteractableStates) };

        //!!! finish making states work, they should initiate the type and run the logic during play mode.
        private void OnEnable()
        {
            SetupStateOptions();
        }
        
        public State[] GetStates()
        {
            return StateList.ToArray();
        }

        public InteractableStates SetupLogic()
        {
            StateType = Type.GetType(AssemblyQualifiedName);
            InteractableStates stateLogic = (InteractableStates)Activator.CreateInstance(StateType, StateList[DefaultIndex]);
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

        public void SetupStateOptions()
        {
            StateOptions = InteractableTypeFinder.Find(candidateStateTypes, TypeRestriction.AllowBase);
        }

        // redundant method, put in a utils with static methods!!!
        public static int ReverseLookup(string option, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] == option)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}

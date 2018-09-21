// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    [CreateAssetMenu(fileName = "States", menuName = "Interactable/State", order = 1)]
    public class States : ScriptableObject
    {
        public List<State> StateList;
        public int DefaultIndex = 0;
        public Type StateType;
        public string[] StateOptions;
        public Type[] StateTypes;
        public string StateLogicName = "InteractableStates";

        //!!! finish making states work, they shoulg initiate the type and run the logic during play mode.
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
            int index = ReverseLookup(StateLogicName, StateOptions);
            StateType = StateTypes[index];
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
            List<Type> stateTypes = new List<Type>();
            List<string> names = new List<string>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.Equals(typeof(InteractableStates)) || type.IsSubclassOf(typeof(InteractableStates)))
                    {
                        stateTypes.Add(type);
                        names.Add(type.Name);
                    }
                }
            }

            StateOptions = names.ToArray();
            StateTypes = stateTypes.ToArray();
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

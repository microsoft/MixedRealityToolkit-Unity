// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States
{
    [CreateAssetMenu(fileName = "States", menuName = "Mixed Reality Toolkit/Interactable/State", order = 1)]
    public class States : ScriptableObject
    {
        public List<State> StateList;
        public int DefaultIndex = 0;
        public Type StateType;
        public string[] StateOptions;
        public Type[] StateTypes;
        public string StateLogicName = "InteractableStates";

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
                foreach (Type type in assembly.GetTypes())
                {
                    TypeInfo info = type.GetTypeInfo();
                    if (info.BaseType != null && (info.BaseType.Equals(typeof(InteractableStates)) || type.Equals(typeof(InteractableStates))))
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

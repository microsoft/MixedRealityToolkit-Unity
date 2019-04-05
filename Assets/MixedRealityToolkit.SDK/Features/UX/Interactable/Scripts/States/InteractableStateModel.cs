// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// State data model, state management and comparison instructions
    /// </summary>

    /*
     * Have an enum with all the button states -
     * Create a list using the enums as the state type -
     * Setup the bit and index automatically -
     * Store the values for all the states -
     * Have a sub state with only the states we care about - 
     * On update, set those states and update the current state
     * The other states can be checked anytime through the Interactive.
     * 
     */

    [System.Serializable]
    public class State
    {
        public string Name;
        public int Index;
        public int Bit;
        public int Value;
        public int ActiveIndex;

        public override string ToString()
        {
            return Name;
        }

        public int ToInt()
        {
            return Index;
        }

        public int ToBit()
        {
            return Bit;
        }
    }
    
    public abstract class InteractableStateModel
    {
        protected State currentState;
        protected List<State> stateList;
        protected State[] allStates;

        public void ImportStates(List<State> states)
        {
            stateList = states;
            for (int i = 0; i < stateList.Count; i++)
            {
                State state = allStates[stateList[i].Index];
                state.ActiveIndex = i;
            }
        }

        public virtual void SetStateOn(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                state.Value = 1;
                SetStateListValue(state.ActiveIndex, 1);
            }
        }

        public virtual void SetStateOff(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                state.Value = 0;
                SetStateListValue(state.ActiveIndex, 0);
            }
        }

        public virtual void SetStateValue(int index, int value)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                state.Value = value;
                SetStateListValue(state.ActiveIndex, value);
            }
        }

        protected virtual void SetStateListValue(int index, int value)
        {
            if (index < stateList.Count && index > -1)
            {
                State state = stateList[index];
                state.Value = value;
            }
        }

        public int GetStateValue(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                return state.Value;
            }
            return 0;
        }

        public State GetState(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                return state;
            }
            return new State();
        }

        public InteractableStateModel(State defaultState)
        {
            currentState = defaultState;
        }
        
        public virtual void SetSate(State state)
        {
            currentState = state;
        }
        
        public virtual State CurrentState()
        {
            return currentState;
        }

        public abstract State CompareStates();

        public abstract State[] GetStates();

        protected int GetBit()
        {
            int bit = 0;
            int bitCount = 0;
            for (int i = 0; i < stateList.Count; i++)
            {
                if (i == 0)
                {
                    bit += 1;
                }
                else
                {
                    bit += bit;
                }

                if (stateList[i].Value > 0)
                {
                    bitCount += bit;
                }
            }
            
            return bitCount;
        }
    }
}

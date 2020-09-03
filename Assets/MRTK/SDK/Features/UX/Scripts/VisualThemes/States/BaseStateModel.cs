// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Base class for defining state model logic to use in conjunction with the State class
    /// Allows for retrieving current state mode and comparing states
    /// </summary>
    public abstract class BaseStateModel
    {
        protected State currentState;
        protected List<State> stateList;
        protected State[] allStates;

        /// <summary>
        /// Import the list of states into this state model
        /// </summary>
        /// <param name="states">list of state objects to import</param>
        public void ImportStates(List<State> states)
        {
            stateList = states;
            for (int i = 0; i < stateList.Count; i++)
            {
                State state = allStates[stateList[i].Index];
                state.ActiveIndex = i;
            }
        }

        /// <summary>
        /// Set the value of the state with given index to on (1)
        /// </summary>
        /// <param name="index">index of state to access</param>
        public virtual void SetStateOn(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                state.Value = 1;
                SetStateListValue(state.ActiveIndex, 1);
            }
        }

        /// <summary>
        /// Set the value of the state with given index to off (0)
        /// </summary>
        /// <param name="index">index of state to access</param>
        public virtual void SetStateOff(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                state.Value = 0;
                SetStateListValue(state.ActiveIndex, 0);
            }
        }

        /// <summary>
        /// Set value of state with given index to the provided value
        /// </summary>
        /// <param name="index">index of state to access</param>
        /// <param name="value">value to set for state</param>
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

        /// <summary>
        /// Get the value of the state with the given index
        /// </summary>
        /// <param name="index">index of state to access</param>
        /// <returns>value of the state</returns>
        public int GetStateValue(int index)
        {
            if (allStates.Length > index && index > 0)
            {
                State state = allStates[index];
                return state.Value;
            }
            return 0;
        }

        /// <summary>
        /// Get the State object with the given index
        /// </summary>
        /// <param name="index">index of state to access</param>
        /// <returns>State Object at given index</returns>
        public State GetState(int index)
        {
            if (allStates.Length > index && index >= 0)
            {
                State state = allStates[index];
                return state;
            }
            return new State();
        }

        public BaseStateModel()
        {
        }

        public BaseStateModel(State defaultState)
        {
            currentState = defaultState;
        }

        /// <summary>
        /// Set the current state to the provided State object
        /// </summary>
        /// <param name="state">State object to set</param>
        public virtual void SetCurrentState(State state)
        {
            currentState = state;
        }

        /// <summary>
        /// Return the current State object
        /// </summary>
        /// <returns>Return the current State object</returns>
        public virtual State CurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Compare all state values, set appropriate current State and return that current State
        /// </summary>
        /// <returns>Current State after comparing State values</returns>
        public abstract State CompareStates();

        /// <summary>
        /// Get list of available States for this State Model
        /// </summary>
        /// <returns>Array of available State objects</returns>
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

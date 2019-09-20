// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// State data model, state management and comparison instructions
    /// </summary>
    [System.Serializable]
    public class State
    {
        /// <summary>
        /// Name of state
        /// </summary>
        public string Name;

        /// <summary>
        /// Index of State in all available state list
        /// </summary>
        public int Index;

        /// <summary>
        /// Bitwise value of state for comparison
        /// </summary>
        public int Bit;

        /// <summary>
        /// Current value of state (e.g on/off etc)
        /// </summary>
        public int Value;

        /// <summary>
        /// Index of state in current list
        /// </summary>
        public int ActiveIndex;

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name;
        }

        [System.Obsolete("Use Index property")]
        public int ToInt()
        {
            return Index;
        }

        [System.Obsolete("Use Bit property")]
        public int ToBit()
        {
            return Bit;
        }

        /// <summary>
        /// Create copy of current State with identical values
        /// </summary>
        /// <returns>copied instance of this State</returns>
        public State Copy()
        {
            return new State()
            {
                ActiveIndex = this.ActiveIndex,
                Bit = this.Bit,
                Index = this.Index,
                Name = this.Name,
                Value = this.Value,
            };
        }

        /// <summary>
        /// Returns true if two state objects have identical internal values, false otherwise
        /// </summary>
        /// <param name="s">other State object to compare against</param>
        /// <returns>true if identical internal values, false otherwise</returns>
        public bool CompareState(State s)
        {
            if (s == null)
            {
                return false;
            }

            return this.Name == s.Name
                && this.Index == s.Index
                && this.Bit == s.Bit
                && this.Value == s.Value
                && this.ActiveIndex == s.ActiveIndex;
        }
    }
}

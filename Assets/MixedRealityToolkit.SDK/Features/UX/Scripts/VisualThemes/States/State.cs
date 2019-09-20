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
        public string Name { get; set; }

        /// <summary>
        /// TODO: Troy - Deprecate this
        /// </summary>
        public int Index { get; set; }


        public int Bit { get; set; }

        public int Value { get; set; }

        public int ActiveIndex { get; set; }

        public override string ToString()
        {
            return Name;
        }

        [System.Obsolete("Use Index property")]
        public int ToInt()
        {
            return Index;
        }

        public int ToBit()
        {
            return Bit;
        }

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

        public bool CompareState(State other)
        {
            return this.Name == other.Name
                && this.Index == other.Index
                && this.Bit == other.Bit
                && this.Value == other.Value
                && this.ActiveIndex == other.ActiveIndex;
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Short id used for state synchronization that can be created from a three letter string
    /// </summary>
    public struct ShortID
    {
        public static readonly ShortID Empty = new ShortID(ushort.MaxValue);

        public ShortID(string name)
        {
            Debug.Assert(name.Length == 3, "ShortID error on '" + name + "', names must be 3 characters long!");
            Value = (ushort)((int)(name[0]-'A') | (int)(name[1]-'A')<<5 | (int)(name[2]-'A')<<10);
            Debug.Assert(name == ToString(), "ShortID error on '" + name + "', names must have upper case characters only!");
        }

        public ShortID(ushort value)
        {
            Value = value;
        }

        public ushort Value{ get; private set; }

        public override string ToString()
        {
            return new string(new char[3] { (char)((Value & ((1<<5)-1)) + 'A'), (char)(((Value & ((1<<10)-1)) >> 5) + 'A'), (char)((Value >> 10) + 'A')});
        }

        public static bool operator ==(ShortID first, ShortID second)
        {
            return first.Value == second.Value;
        }

        public static bool operator !=(ShortID first, ShortID second)
        {
            return first.Value != second.Value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ShortID))
            {
                return false;
            }
            return Value == ((ShortID)obj).Value;
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}

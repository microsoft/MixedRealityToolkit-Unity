// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public struct WindowsMixedRealityController : IMixedRealityInputSource
    {
        public uint SourceId { get; set; }

        public string SourceName { get; }

        public InputSourceState InputSourceState { get; set; }

        public Handedness Handedness { get; set; }
        
        public IMixedRealityPointer[] Pointers { get; set; }

        public InputType[] Capabilities { get; set; }
        
        public InteractionDefinition[] Interactions { get; set; }
                
        //TODO - Needs re-implementing, as there may be more than left == right
        #region IEquality Implementation

        public static bool Equals(IMixedRealityInputSource left, IMixedRealityInputSource right)
        {
            return left.Equals(right);
        }

        public new bool Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityInputSource)obj);
        }

        private bool Equals(IMixedRealityInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        //TODO - Needs re-implementing as it returns true if ONE capability is supported, but not multiples
        public bool SupportsInputCapability(InputType[] capabilities)
        {
            for (int i = 0; i < Capabilities.Length; i++)
            {
                for (int j = 0; j < capabilities.Length; j++)
                {
                    if (Capabilities[i] == capabilities[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

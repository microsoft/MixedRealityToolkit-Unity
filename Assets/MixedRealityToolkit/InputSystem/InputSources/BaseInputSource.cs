// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.InputSources
{
    /// <summary>
    /// Base class for input sources that inherit from MonoBehaviour.
    /// </summary>
    public abstract class BaseInputSource : MonoBehaviour, IInputSource
    {
        private static IMixedRealityInputSystem inputSystem = null;
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());

        private uint sourceId = 0;
        public uint SourceId
        {
            get
            {
                if (sourceId == 0)
                {
                    sourceId = InputSystem.GenerateNewSourceId();
                }

                return sourceId;
            }
        }

        public virtual string SourceName
        {
            get { return name; }
            set { name = value; }
        }

        public virtual IPointer[] Pointers => null;

        public virtual InputType[] Capabilities => new[] { InputType.None };

        public bool SupportsInputCapability(InputType[] inputInfo)
        {
            for (int i = 0; i < Capabilities.Length; i++)
            {
                for (int j = 0; j < inputInfo.Length; j++)
                {
                    if (Capabilities[i] == inputInfo[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region IEquality Implementation

        private bool Equals(IInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.SourceId == right.SourceId;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var left = (IInputSource)x;
            var right = (IInputSource)y;
            if (left != null && right != null)
            {
                return Equals(left, right);
            }

            return false;
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
    }
}

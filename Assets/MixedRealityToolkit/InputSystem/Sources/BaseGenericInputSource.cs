﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// <remarks>This base class does not support adding or removing pointers, because many will never
    /// pass pointers in their constructors and will fall back to either the Gaze or Mouse Pointer.</remarks>
    /// </summary>
    public class BaseGenericInputSource : IMixedRealityInputSource
    {
        /// <summary>
        /// The Current Input System for this Input Source.
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());
        private static IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="interactions"></param>
        /// <param name="pointers"></param>
        public BaseGenericInputSource(string name, InteractionDefinition[] interactions, IMixedRealityPointer[] pointers = null)
        {
            SourceId = InputSystem.GenerateNewSourceId();
            SourceName = name;
            Pointers = pointers ?? new[] { InputSystem.GazeProvider.GazePointer };
            Interactions = interactions;
        }

        /// <inheritdoc />
        public uint SourceId { get; }

        /// <inheritdoc />
        public string SourceName { get; }

        /// <inheritdoc />
        public virtual IMixedRealityPointer[] Pointers { get; }

        /// <inheritdoc />
        public InteractionDefinition[] Interactions { get; }

        /// <inheritdoc />
        public bool SupportsCapability(Internal.Definitions.Devices.DeviceInputType inputInfo)
        {
            for (int i = 0; i < Interactions.Length; i++)
            {
                if (Interactions[i].InputType == inputInfo)
                {
                    return true;
                }
            }

            return false;
        }

        #region IEquality Implementation

        public static bool Equals(IMixedRealityInputSource left, IMixedRealityInputSource right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
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
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Gaze;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// </summary>
    public class BaseGenericInputSource : IMixedRealityInputSource
    {
        /// <summary>
        /// The Current Input System for this Input Source.
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>());
        private static IMixedRealityInputSystem inputSystem = null;

        public BaseGenericInputSource(string name, InputType[] capabilities, IMixedRealityPointer[] pointers = null)
        {
            SourceId = InputSystem.GenerateNewSourceId();
            SourceName = name;
            Capabilities = capabilities;
            Pointers = pointers ?? new[] { GazeProvider.GazePointer };
        }

        public uint SourceId { get; }

        public string SourceName { get; }

        public IMixedRealityPointer[] Pointers { get; private set; }

        public InteractionDefinition[] Interactions { get; } = null;

        public InputType[] Capabilities { get; }

        public bool SupportsCapabilities(InputType[] inputTypes)
        {
            return Capabilities == inputTypes;
        }

        public bool SupportsCapability(InputType inputInfo)
        {
            for (int i = 0; i < Capabilities.Length; i++)
            {
                if (Capabilities[i] == inputInfo)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void AddPointer(IMixedRealityPointer pointer)
        {
            for (int i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId == pointer.PointerId)
                {
                    Debug.LogWarning($"This pointer has already been added to {SourceName}.");
                    return;
                }
            }

            var newPointers = new IMixedRealityPointer[Pointers.Length + 1];

            // Set our new pointer at the end.
            newPointers[newPointers.Length - 1] = pointer;

            // Reverse loop and set our existing pointers.
            for (int i = newPointers.Length - 2; i >= 0; i--)
            {
                newPointers[i] = Pointers[i];
            }
        }

        public virtual void RemovePointer(IMixedRealityPointer pointer)
        {
            var oldPointerList = new List<IMixedRealityPointer>(Pointers.Length);

            for (int i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId != pointer.PointerId)
                {
                    oldPointerList.Add(Pointers[i]);
                }
            }

            Pointers = oldPointerList.ToArray();
        }

        public virtual bool TryGetPointerPosition(IMixedRealityPointer pointer, out Vector3 position)
        {
            for (var i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId == pointer.PointerId)
                {
                    return Pointers[i].TryGetPointerPosition(out position);
                }
            }

            position = Vector3.zero;
            return false;
        }

        public virtual bool TryGetPointingRay(IMixedRealityPointer pointer, out Ray pointingRay)
        {
            for (var i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId == pointer.PointerId)
                {
                    return Pointers[i].TryGetPointingRay(out pointingRay);
                }
            }

            pointingRay = default(Ray);
            return false;
        }

        public virtual bool TryGetPointerRotation(IMixedRealityPointer pointer, out Quaternion rotation)
        {
            for (var i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId == pointer.PointerId)
                {
                    return Pointers[i].TryGetPointerRotation(out rotation);
                }
            }

            rotation = Quaternion.identity;
            return false;
        }

        #region IEquality Implementation

        public static bool Equals(IMixedRealityInputSource left, IMixedRealityInputSource right)
        {
            return left.Equals(right);
        }

        bool IEqualityComparer.Equals(object left, object right)
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
    }
}
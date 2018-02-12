// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.Gaze;
using MixedRealityToolkit.InputModule.Pointers;
using MixedRealityToolkit.InputModule.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.InputSources
{
    /// <summary>
    /// Base class for input sources that don't inherit from MonoBehaviour.
    /// </summary>
    public class GenericInputSource : IInputSource
    {
        public GenericInputSource(string name, SupportedInputInfo _supportedInputInfo, IPointer[] pointers = null)
        {
            InputManager.AssertIsInitialized();
            GazeManager.AssertIsInitialized();

            SourceId = InputManager.GenerateNewSourceId();
            SourceName = name;
            supportedInputInfo = _supportedInputInfo;
            Pointers = pointers ?? GazeManager.Instance.Pointers;
        }

        public uint SourceId { get; private set; }

        public string SourceName { get; private set; }

        public IPointer[] Pointers { get; private set; }

        private SupportedInputInfo supportedInputInfo;

        public virtual SupportedInputInfo GetSupportedInputInfo()
        {
            return supportedInputInfo;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        public virtual void AddPointer(IPointer pointer)
        {
            for (int i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId == pointer.PointerId)
                {
                    Debug.LogWarningFormat("This pointer has already been added to {0}.", SourceName);
                    return;
                }
            }

            var newPointers = new IPointer[Pointers.Length + 1];

            // Set our new pointer at the end.
            newPointers[newPointers.Length - 1] = pointer;

            // Reverse loop and set our existing pointers.
            for (int i = newPointers.Length - 2; i >= 0; i--)
            {
                newPointers[i] = Pointers[i];
            }
        }

        public virtual void RemovePointer(IPointer pointer)
        {
            var oldPointerList = new List<IPointer>(Pointers.Length);

            for (int i = 0; i < Pointers.Length; i++)
            {
                if (Pointers[i].PointerId != pointer.PointerId)
                {
                    oldPointerList.Add(Pointers[i]);
                }
            }

            Pointers = oldPointerList.ToArray();
        }

        public virtual bool TryGetPointerPosition(IPointer pointer, out Vector3 position)
        {
            foreach (var sourcePointer in Pointers)
            {
                if (sourcePointer.PointerId == pointer.PointerId)
                {
                    return sourcePointer.TryGetPointerPosition(out position);
                }
            }

            position = Vector3.zero;
            return false;
        }

        public virtual bool TryGetPointingRay(IPointer pointer, out Ray pointingRay)
        {
            foreach (var sourcePointer in Pointers)
            {
                if (sourcePointer.PointerId == pointer.PointerId)
                {
                    return sourcePointer.TryGetPointingRay(out pointingRay);
                }
            }

            pointingRay = default(Ray);
            return false;
        }

        public virtual bool TryGetPointerRotation(IPointer pointer, out Quaternion rotation)
        {
            foreach (var sourcePointer in Pointers)
            {
                if (sourcePointer.PointerId == pointer.PointerId)
                {
                    return sourcePointer.TryGetPointerRotation(out rotation);
                }
            }

            rotation = Quaternion.identity;
            return false;
        }

        #region IEquality Implementation

        public static bool Equals(IInputSource left, IInputSource right)
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

            return Equals((IInputSource)obj);
        }

        private bool Equals(IInputSource other)
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
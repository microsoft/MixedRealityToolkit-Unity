// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Generic Input Pointing source that does not inherit from MonoBehaviour.
    /// </summary>
    public class GenericInputPointingSource : GenericInputSource
    {
        public GenericInputPointingSource(string name) : base(name) { }

        public GenericInputPointingSource(string name, SupportedInputInfo supportedInputInfo) : base(name, supportedInputInfo) { }

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
    }
}
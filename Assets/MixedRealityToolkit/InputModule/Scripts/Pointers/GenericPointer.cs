// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Pointers
{
    /// <summary>
    /// Base Class for pointers that don't inherit from MonoBehaviour.
    /// </summary>
    public class GenericPointer : IPointer
    {
        public GenericPointer(string pointerName, IInputSource inputSourceParent)
        {
            PointerId = FocusManager.GenerateNewPointerId();
            PointerName = pointerName;
            InputSourceParent = inputSourceParent;
        }

        public uint PointerId { get; private set; }

        public string PointerName { get; set; }

        public IInputSource InputSourceParent { get; private set; }

        public BaseCursor BaseCursor { get; set; }

        public ICursorModifier CursorModifier { get; set; }

        public ITeleportTarget TeleportTarget { get; set; }

        public bool InteractionEnabled { get; set; }

        public bool FocusLocked { get; set; }

        public float? PointerExtent { get; set; }

        public RayStep[] Rays
        {
            get { return rays; }
            protected set { rays = value; }
        }

        private RayStep[] rays = { new RayStep(Vector3.zero, Vector3.forward) };

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public IFocusHandler FocusTarget { get; set; }

        public PointerResult Result { get; set; }

        public BaseRayStabilizer RayStabilizer { get; set; }

        public virtual void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                rays[0].CopyRay(pointingRay, (PointerExtent ?? FocusManager.GlobalPointingExtent));
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(rays[0].Origin, rays[0].Direction);
                rays[0].CopyRay(RayStabilizer.StableRay, (PointerExtent ?? FocusManager.GlobalPointingExtent));
            }
        }

        public virtual void OnPostRaycast() { }

        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public virtual bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = default(Ray);
            return false;
        }

        public virtual bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        #region IEquality Implementation

        public static bool Equals(IPointer left, IPointer right)
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

            return Equals((IPointer)obj);
        }

        private bool Equals(IPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
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
                hashCode = (hashCode * 397) ^ (int)PointerId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}

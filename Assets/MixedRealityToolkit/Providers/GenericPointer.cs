// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Teleport;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base Class for pointers that don't inherit from MonoBehaviour.
    /// </summary>
    public abstract class GenericPointer : IMixedRealityPointer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pointerName"></param>
        /// <param name="inputSourceParent"></param>
        public GenericPointer(string pointerName, IMixedRealityInputSource inputSourceParent)
        {
            PointerId = (InputSystem?.FocusProvider != null) ? InputSystem.FocusProvider.GenerateNewPointerId() : 0;
            PointerName = pointerName;
            this.inputSourceParent = inputSourceParent;
        }

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        /// <inheritdoc />
        public virtual IMixedRealityController Controller
        {
            get { return controller; }
            set
            {
                controller = value;

                if (controller != null)
                {
                    inputSourceParent = controller.InputSource;
                }
            }
        }

        private IMixedRealityController controller;

        /// <inheritdoc />
        public uint PointerId { get; }

        /// <inheritdoc />
        public string PointerName { get; set; }

        /// <inheritdoc />
        public virtual IMixedRealityInputSource InputSourceParent
        {
            get { return inputSourceParent; }
            protected set { inputSourceParent = value; }
        }

        private IMixedRealityInputSource inputSourceParent;

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get; set; }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get; set; }

        /// <inheritdoc />
        public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

        private bool isInteractionEnabled = true;

        /// <inheritdoc />
        public bool IsInteractionEnabled
        {
            get { return isInteractionEnabled && IsActive; }
            set
            {
                isInteractionEnabled = value;
                if (BaseCursor != null)
                {
                    BaseCursor.SetVisibility(value);
                }
            }
        }

        public bool IsActive { get; set; }

        /// <inheritdoc />
        public bool IsFocusLocked { get; set; }

        /// <inheritdoc />
        public bool IsTargetPositionLockedOnFocusLock { get; set; }

        /// <summary>
        /// The pointer's maximum extent when raycasting.
        /// </summary>
        public virtual float PointerExtent { get; set; } = 10f;

        /// <inheritdoc />
        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get; set; }

        /// <inheritdoc />
        public IPointerResult Result { get; set; }

        /// <summary>
        /// Ray stabilizer used when calculating position of pointer end point.
        /// </summary>
        public IBaseRayStabilizer RayStabilizer { get; set; }

        /// <inheritdoc />
        public SceneQueryType SceneQueryType { get; set; } = SceneQueryType.SimpleRaycast;
        
        /// <inheritdoc />
        public float SphereCastRadius { get; set; }

        /// <inheritdoc />
        public abstract Vector3 Position { get; }

        /// <inheritdoc />
        public abstract Quaternion Rotation { get; }

        /// <inheritdoc />
        public abstract void OnPreSceneQuery();

        /// <inheritdoc />
        public abstract void OnPostSceneQuery();

        /// <inheritdoc />
        public abstract void OnPreCurrentPointerTargetChange();

        #region IEquality Implementation

        public static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
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

            return Equals((IMixedRealityPointer)obj);
        }

        private bool Equals(IMixedRealityPointer other)
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

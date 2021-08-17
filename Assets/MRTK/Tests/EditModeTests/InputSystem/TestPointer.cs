// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests.EditMode.InputSystem
{
    // Dummy pointer class used as a placeholder for a proper pointer in some unit test.
    // Only the interfaces required for the tests are implemented. Feel free to add implementations as needed.
    public class TestPointer : IMixedRealityPointer
    {
        public TestPointer()
        {
            IsInteractionEnabled = false;
            PointerId = nextId++;
            InputSourceParent = new BaseGenericInputSource("TestPointer");
        }

        private static uint nextId = 1;

        #region IMixedRealityPointer

        /// <inheritdoc />
        public IMixedRealityController Controller { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public uint PointerId { get; set; }

        /// <inheritdoc />
        public string PointerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSourceParent { get; }

        /// <inheritdoc />
        public IMixedRealityCursor BaseCursor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public ICursorModifier CursorModifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public bool IsInteractionEnabled { get; set; }

        /// <inheritdoc />
        public bool IsActive { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public bool IsFocusLocked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public bool IsTargetPositionLockedOnFocusLock { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public RayStep[] Rays => throw new NotImplementedException();

        /// <inheritdoc />
        public LayerMask[] PrioritizedLayerMasksOverride { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public IMixedRealityFocusHandler FocusTarget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public IPointerResult Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public SceneQueryType SceneQueryType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public float SphereCastRadius { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public Vector3 Position => throw new NotImplementedException();

        /// <inheritdoc />
        public Quaternion Rotation => throw new NotImplementedException();

        /// <inheritdoc />
        public new bool Equals(object x, object y)
        {
            return UnityEngine.Object.ReferenceEquals(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            var pointer = obj as TestPointer;
            return (int)pointer.PointerId;
        }

        /// <inheritdoc />
        public void OnPostSceneQuery()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnPreCurrentPointerTargetChange()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void OnPreSceneQuery()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion IMixedRealityPointer
    }
}

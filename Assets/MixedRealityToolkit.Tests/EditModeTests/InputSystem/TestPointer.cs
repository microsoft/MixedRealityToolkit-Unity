// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests.InputSystem
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

        public IMixedRealityController Controller { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public uint PointerId { get; set; }

        public string PointerName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public IMixedRealityInputSource InputSourceParent { get; }

        public IMixedRealityCursor BaseCursor { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ICursorModifier CursorModifier { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public bool IsInteractionEnabled { get; set; }

        public bool IsActive { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsFocusLocked { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsTargetPositionLockedOnFocusLock { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public RayStep[] Rays => throw new System.NotImplementedException();

        public LayerMask[] PrioritizedLayerMasksOverride { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IMixedRealityFocusHandler FocusTarget { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IPointerResult Result { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public SceneQueryType SceneQueryType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float SphereCastRadius { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Vector3 Position => throw new System.NotImplementedException();

        public Quaternion Rotation => throw new System.NotImplementedException();

        public new bool Equals(object x, object y)
        {
            return Object.ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            var pointer = obj as TestPointer;
            return (int) pointer.PointerId;
        }

        public void OnPostSceneQuery()
        {
            throw new System.NotImplementedException();
        }

        public void OnPreCurrentPointerTargetChange()
        {
            throw new System.NotImplementedException();
        }

        public void OnPreSceneQuery()
        {
            throw new System.NotImplementedException();
        }

        #endregion IMixedRealityPointer
    }
}
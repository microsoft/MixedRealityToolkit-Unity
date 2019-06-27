// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class TestPointer : GenericPointer
    {
        public TestPointer() : base("Test Pointer", null)
        {
            IsActive = true;
        }

        public override Vector3 Position => throw new System.NotImplementedException();

        public override Quaternion Rotation => throw new System.NotImplementedException();

        public override void OnPostSceneQuery()
        {
        }

        public override void OnPreCurrentPointerTargetChange()
        {
        }

        public override void OnPreSceneQuery()
        {
        }

        public void SetFromTestProxy(FocusRaycastTestProxy testProxy)
        {
            if (Rays == null || Rays.Length != testProxy.LineCastResolution)
            {
                Rays = new RayStep[testProxy.LineCastResolution];
            }

            var lineBase = testProxy.RayLineData;

            float stepSize = 1f / Rays.Length;
            Vector3 lastPoint = lineBase.GetUnClampedPoint(0f);
            for (int i = 0; i < Rays.Length; i++)
            {
                Vector3 currentPoint = lineBase.GetUnClampedPoint(stepSize * (i + 1));
                Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                lastPoint = currentPoint;
            }

            PrioritizedLayerMasksOverride = testProxy.PrioritizedLayerMasks;
        }
    }
}

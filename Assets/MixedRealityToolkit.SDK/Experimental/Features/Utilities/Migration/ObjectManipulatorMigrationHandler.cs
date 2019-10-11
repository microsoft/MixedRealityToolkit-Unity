// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Interface defining a migration handler, which is used to migrate assets as they
    /// upgrade to new versions of MRTK.
    /// </summary>
    public class ObjectManipulatorMigrationHandler : IMigrationHandler
    {
        /// <inheritdoc />
        public bool CanMigrate(GameObject gameObject)
        {
            return gameObject.GetComponent<ManipulationHandler>() != null;
        }

        /// <inheritdoc />
        public void Migrate(GameObject gameObject)
        {
            var mh1 = gameObject.GetComponent<ManipulationHandler>();
            var mh2 = gameObject.AddComponent<ObjectManipulator>();

            mh2.HostTransform = mh1.HostTransform;

            switch (mh1.ManipulationType)
            {
                case ManipulationHandler.HandMovementType.OneHandedOnly:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
                    break;
                case ManipulationHandler.HandMovementType.TwoHandedOnly:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.TwoHanded;
                    break;
                case ManipulationHandler.HandMovementType.OneAndTwoHanded:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.OneHanded |
                        ObjectManipulator.HandMovementType.TwoHanded;
                    break;
            }

            mh2.AllowFarManipulation = mh1.AllowFarManipulation;
            mh2.OneHandRotationModeNear = (ObjectManipulator.RotateInOneHandType)mh1.OneHandRotationModeNear;
            mh2.OneHandRotationModeFar = (ObjectManipulator.RotateInOneHandType)mh1.OneHandRotationModeFar;

            switch (mh1.TwoHandedManipulationType)
            {
                case ManipulationHandler.TwoHandedManipulation.Scale:
                    mh2.TwoHandedManipulationType = TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.Rotate:
                    mh2.TwoHandedManipulationType = TransformFlags.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveScale:
                    mh2.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotate:
                    mh2.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.RotateScale:
                    mh2.TwoHandedManipulationType = TransformFlags.Rotate |
                        TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotateScale:
                    mh2.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Rotate |
                        TransformFlags.Scale;
                    break;
            }

            mh2.ReleaseBehavior = (ObjectManipulator.ReleaseBehaviorType)mh1.ReleaseBehavior;

            if (mh1.ConstraintOnRotation != RotationConstraintType.None)
            {
                var rotateConstraint = mh2.gameObject.AddComponent<RotationAxisConstraint>();
                rotateConstraint.TargetTransform = mh1.HostTransform;
                rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(mh1.ConstraintOnRotation);
            }

            if (mh1.ConstraintOnMovement == MovementConstraintType.FixDistanceFromHead)
            {
                var moveConstraint = mh2.gameObject.AddComponent<FixedDistanceConstraint>();
                moveConstraint.TargetTransform = mh1.HostTransform;
                moveConstraint.ConstraintTransform = CameraCache.Main.transform;
            }

            mh2.SmoothingActive = mh1.SmoothingActive;
            mh2.MoveLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.RotateLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.ScaleLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.OnManipulationStarted = mh1.OnManipulationStarted;
            mh2.OnManipulationEnded = mh1.OnManipulationEnded;
            mh2.OnHoverEntered = mh1.OnHoverEntered;
            mh2.OnHoverExited = mh1.OnHoverExited;

            Object.DestroyImmediate(mh1);
        }
    }
}
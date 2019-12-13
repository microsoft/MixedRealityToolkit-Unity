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
            var manipHandler = gameObject.GetComponent<ManipulationHandler>();
            var objManip = gameObject.AddComponent<ObjectManipulator>();

            objManip.HostTransform = manipHandler.HostTransform;

            switch (manipHandler.ManipulationType)
            {
                case ManipulationHandler.HandMovementType.OneHandedOnly:
                    objManip.ManipulationType = ManipulationHandFlags.OneHanded;
                    break;
                case ManipulationHandler.HandMovementType.TwoHandedOnly:
                    objManip.ManipulationType = ManipulationHandFlags.TwoHanded;
                    break;
                case ManipulationHandler.HandMovementType.OneAndTwoHanded:
                    objManip.ManipulationType = ManipulationHandFlags.OneHanded |
                        ManipulationHandFlags.TwoHanded;
                    break;
            }

            objManip.AllowFarManipulation = manipHandler.AllowFarManipulation;

            if (manipHandler.OneHandRotationModeNear == manipHandler.OneHandRotationModeFar)
            {
                MigrateOneHandRotationModes(ref objManip, manipHandler.OneHandRotationModeNear, ManipulationProximityFlags.Near | ManipulationProximityFlags.Far);
            }
            else
            {
                MigrateOneHandRotationModes(ref objManip, manipHandler.OneHandRotationModeNear, ManipulationProximityFlags.Near);
                MigrateOneHandRotationModes(ref objManip, manipHandler.OneHandRotationModeFar, ManipulationProximityFlags.Far);
            }

            switch (manipHandler.TwoHandedManipulationType)
            {
                case ManipulationHandler.TwoHandedManipulation.Scale:
                    objManip.TwoHandedManipulationType = TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.Rotate:
                    objManip.TwoHandedManipulationType = TransformFlags.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveScale:
                    objManip.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotate:
                    objManip.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.RotateScale:
                    objManip.TwoHandedManipulationType = TransformFlags.Rotate |
                        TransformFlags.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotateScale:
                    objManip.TwoHandedManipulationType = TransformFlags.Move |
                        TransformFlags.Rotate |
                        TransformFlags.Scale;
                    break;
            }

            objManip.ReleaseBehavior = (ObjectManipulator.ReleaseBehaviorType)manipHandler.ReleaseBehavior;

            if (manipHandler.ConstraintOnRotation != RotationConstraintType.None)
            {
                var rotateConstraint = objManip.EnsureComponent<RotationAxisConstraint>();
                rotateConstraint.TargetTransform = manipHandler.HostTransform;
                rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(manipHandler.ConstraintOnRotation);
            }

            if (manipHandler.ConstraintOnMovement == MovementConstraintType.FixDistanceFromHead)
            {
                var moveConstraint = objManip.EnsureComponent<FixedDistanceConstraint>();
                moveConstraint.TargetTransform = manipHandler.HostTransform;
                moveConstraint.ConstraintTransform = CameraCache.Main.transform;
            }

            objManip.SmoothingActive = manipHandler.SmoothingActive;
            objManip.MoveLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.RotateLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.ScaleLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.OnManipulationStarted = manipHandler.OnManipulationStarted;
            objManip.OnManipulationEnded = manipHandler.OnManipulationEnded;
            objManip.OnHoverEntered = manipHandler.OnHoverEntered;
            objManip.OnHoverExited = manipHandler.OnHoverExited;

            Object.DestroyImmediate(manipHandler);
        }

        private void MigrateOneHandRotationModes(ref ObjectManipulator objManip, ManipulationHandler.RotateInOneHandType oldMode, ManipulationProximityFlags proximity)
        {
            ObjectManipulator.RotateInOneHandType newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

            switch (oldMode)
            {
                case ManipulationHandler.RotateInOneHandType.MaintainRotationToUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var constraint = objManip.EnsureComponent<FixedRotationToUserConstraint>();
                        constraint.TargetTransform = objManip.HostTransform;
                        constraint.HandType = ManipulationHandFlags.OneHanded;
                        constraint.ProximityType = proximity;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.GravityAlignedMaintainRotationToUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FixedRotationToUserConstraint>();
                        rotConstraint.TargetTransform = objManip.HostTransform;
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;

                        var axisConstraint = objManip.EnsureComponent<RotationAxisConstraint>();
                        axisConstraint.TargetTransform = objManip.HostTransform;
                        axisConstraint.HandType = ManipulationHandFlags.OneHanded;
                        axisConstraint.ProximityType = proximity;
                        axisConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.FaceUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FaceUserConstraint>();
                        rotConstraint.TargetTransform = objManip.HostTransform;
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;
                        rotConstraint.FaceAway = false;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.FaceAwayFromUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FaceUserConstraint>();
                        rotConstraint.TargetTransform = objManip.HostTransform;
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;
                        rotConstraint.FaceAway = true;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FixedRotationToWorldConstraint>();
                        rotConstraint.TargetTransform = objManip.HostTransform;
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.RotateAboutObjectCenter:
                    newMode = ObjectManipulator.RotateInOneHandType.RotateAboutObjectCenter;
                    break;
                case ManipulationHandler.RotateInOneHandType.RotateAboutGrabPoint:
                    newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;
                    break;
            }

            if (proximity.HasFlag(ManipulationProximityFlags.Near))
            {
                objManip.OneHandRotationModeNear = newMode;
            }
            if (proximity.HasFlag(ManipulationProximityFlags.Far))
            {
                objManip.OneHandRotationModeFar = newMode;
            }
        }
    }
}
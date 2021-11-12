// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
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
            gameObject.AddComponent<ConstraintManager>();
            var objManip = gameObject.AddComponent<ObjectManipulator>();

            objManip.enabled = manipHandler.enabled;

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
                rotateConstraint.ConstraintOnRotation = RotationConstraintHelper.ConvertToAxisFlags(manipHandler.ConstraintOnRotation);
            }

            if (manipHandler.ConstraintOnMovement == MovementConstraintType.FixDistanceFromHead)
            {
                var moveConstraint = objManip.EnsureComponent<FixedDistanceConstraint>();
                moveConstraint.ConstraintTransform = CameraCache.Main.transform;
            }

            objManip.SmoothingFar = manipHandler.SmoothingActive;
            objManip.MoveLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.RotateLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.ScaleLerpTime = manipHandler.SmoothingAmoutOneHandManip;
            objManip.OnManipulationStarted = manipHandler.OnManipulationStarted;
            objManip.OnManipulationEnded = manipHandler.OnManipulationEnded;
            objManip.OnHoverEntered = manipHandler.OnHoverEntered;
            objManip.OnHoverExited = manipHandler.OnHoverExited;

            // finally check if there's a CursorContextManipulationHandler on the gameObject that we have to swap
            CursorContextManipulationHandler cursorContextManipHandler = gameObject.GetComponent<CursorContextManipulationHandler>();
            if (cursorContextManipHandler)
            {
                gameObject.AddComponent<CursorContextObjectManipulator>();
                // remove old component
                Object.DestroyImmediate(cursorContextManipHandler);
            }

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
                        constraint.HandType = ManipulationHandFlags.OneHanded;
                        constraint.ProximityType = proximity;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.GravityAlignedMaintainRotationToUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FixedRotationToUserConstraint>();
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;

                        var axisConstraint = objManip.EnsureComponent<RotationAxisConstraint>();
                        axisConstraint.HandType = ManipulationHandFlags.OneHanded;
                        axisConstraint.ProximityType = proximity;
                        axisConstraint.ConstraintOnRotation = AxisFlags.XAxis | AxisFlags.ZAxis;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.FaceUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FaceUserConstraint>();
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;
                        rotConstraint.FaceAway = false;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.FaceAwayFromUser:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FaceUserConstraint>();
                        rotConstraint.HandType = ManipulationHandFlags.OneHanded;
                        rotConstraint.ProximityType = proximity;
                        rotConstraint.FaceAway = true;
                        break;
                    }
                case ManipulationHandler.RotateInOneHandType.MaintainOriginalRotation:
                    {
                        newMode = ObjectManipulator.RotateInOneHandType.RotateAboutGrabPoint;

                        var rotConstraint = objManip.EnsureComponent<FixedRotationToWorldConstraint>();
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

            if (proximity.IsMaskSet(ManipulationProximityFlags.Near))
            {
                objManip.OneHandRotationModeNear = newMode;
            }
            if (proximity.IsMaskSet(ManipulationProximityFlags.Far))
            {
                objManip.OneHandRotationModeFar = newMode;
            }
        }
    }
}